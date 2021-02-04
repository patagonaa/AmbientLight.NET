using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AmbientLightNet.Infrastructure;
using AmbientLightNet.Infrastructure.AmbiLightConfig;
using AmbientLightNet.Infrastructure.ColorAveraging;
using AmbientLightNet.Infrastructure.ColorTransformer;
using AmbientLightNet.Infrastructure.ScreenCapture;
using AmbientLightNet.Infrastructure.Utils;
using AmbientLightNet.ScreenCapture.Infrastructure;
using AmbiLightNet.PluginBase;
using Newtonsoft.Json;
using Topshelf.Logging;

namespace AmbientLightNet.Service
{
	public class AmbiLightService : IDisposable
	{
		private readonly string _configPath;
		private readonly LogWriter _logger;
		private bool _running;
		private IScreenCaptureService _screenCaptureService;
		private readonly object _configLock = new object();
		private IList<RegionConfiguration> _regionConfigurations;
		private readonly ColorAveragingServiceProvider _colorAveragingServiceProvider;
		private readonly ColorTransformerService _colorTransformerService;
		private IList<ScreenRegion> _screenRegions; //needed as reference for caching
		private readonly ScreenCaptureServiceProvider _screenCaptureServiceProvider;
		private Thread _mainThread;

		public AmbiLightService(string configPath)
		{
			_configPath = configPath;
			_logger = HostLogger.Get<AmbiLightService>();

			_screenCaptureServiceProvider = new ScreenCaptureServiceProvider();
			_colorAveragingServiceProvider = new ColorAveragingServiceProvider();
			_colorTransformerService = new ColorTransformerService();

			AmbiLightConfig config = ReadConfig(_configPath);

			ApplyConfig(config);

			var configFileInfo = new FileInfo(configPath);
			var watcher = new FileSystemWatcher(configFileInfo.DirectoryName, configFileInfo.Name);
			watcher.Changed += (sender, eventArgs) =>
			{
				watcher.EnableRaisingEvents = false;
				while (FileIsLocked(eventArgs.FullPath))
				{
					Thread.Sleep(100);
				}
				ReloadConfig();
				watcher.EnableRaisingEvents = true;
			};
			watcher.EnableRaisingEvents = true;
		}

		private static bool FileIsLocked(string filename)
		{
			try
			{
				using (File.Open(filename, FileMode.Open, FileAccess.Read, FileShare.None))
				{
					return false;
				}
			}
			catch (IOException)
			{
				return true;
			}
		}

		private void ApplyConfig(AmbiLightConfig config)
		{
			if(_screenCaptureService != null)
				_screenCaptureService.Dispose();

			_screenCaptureService = _screenCaptureServiceProvider.Provide(true);

			List<IOutputPlugin> plugins = OutputPlugins.GetAvailablePlugins();

			List<ScreenRegionOutput> regions = config.RegionsToOutput;

			if (_regionConfigurations != null)
			{
				foreach (RegionConfiguration regionConfiguration in _regionConfigurations)
				{
					regionConfiguration.Dispose();
				}
			}

			_regionConfigurations = new List<RegionConfiguration>();

			var regionConfigId = 0;

			foreach (ScreenRegionOutput region in regions)
			{
				var regionConfig = new RegionConfiguration(regionConfigId++)
				{
					ScreenRegion = region.ScreenRegion,
					ColorAveragingService = _colorAveragingServiceProvider.Provide(region.ColorAveragingConfig),
					OutputConfigs = new List<OutputConfiguration>()
				};

				var outputId = 0;

				foreach (Output output in region.Outputs)
				{
					IOutputInfo outputInfo = output.OutputInfo;
					IOutputPlugin plugin = plugins.FirstOrDefault(y => y.Name == outputInfo.PluginName);
					if (plugin == null)
						throw new InvalidOperationException(string.Format("Missing OutputPlugin {0}", outputInfo.PluginName));

					OutputService outputService = _regionConfigurations.SelectMany(x => x.OutputConfigs).Select(x => x.OutputService).FirstOrDefault(x => x.IsReusableFor(outputInfo)) ?? plugin.GetNewOutputService();

					outputService.Initialize(outputInfo);

					IList<ColorTransformerConfig> colorTransformerConfigs = output.ColorTransformerConfigs;

					if (colorTransformerConfigs == null)
					{
						colorTransformerConfigs = new List<ColorTransformerConfig>
						{
							new ColorTransformerConfig
							{
								Type = typeof (HysteresisColorTransformer),
								Config = new Dictionary<string, object>
								{
									{"hysteresis", "0.01"}
								}
							},
							new ColorTransformerConfig
							{
								Type = typeof (BrightnessColorTransformer),
								Config = new Dictionary<string, object>
								{
									{"factorR", "1"},
									{"factorG", "1"},
									{"factorB", "0.9"}
								}
							},
							//new ColorTransformerConfig
							//{
							//	Type = typeof (GammaColorTransformer),
							//	Config = new Dictionary<string, object>
							//	{
							//		{"gammaR", "1"},
							//		{"gammaG", "1.2"},
							//		{"gammaB", "1.2"}
							//	}
							//},
							new ColorTransformerConfig
							{
								Type = typeof (ThresholdColorTransformer),
								Config = new Dictionary<string, object>
								{
									{"threshold", "0.01"}
								}
							}
						};
					}

					var outputConfig = new OutputConfiguration(regionConfig.Id, outputId++)
					{
						ColorTransformerContexts = colorTransformerConfigs.Select(x => new ColorTransformerContext(x)).ToList(),
						OutputService = outputService,
						ResendIntervalFunc = outputService.GetResendInterval,
						OutputInfo = outputInfo
					};

					regionConfig.OutputConfigs.Add(outputConfig);
				}

				_regionConfigurations.Add(regionConfig);
			}

			_screenRegions = _regionConfigurations.Select(x => x.ScreenRegion).ToList();
		}

		public void Start()
		{
			if(_running)
				return;

			if(_mainThread != null && _mainThread.ThreadState == ThreadState.Running)
				_mainThread.Abort();

			_running = true;

			if(_mainThread == null || _mainThread.ThreadState != ThreadState.Running)
				_mainThread = new Thread(MainLoop);

			_mainThread.Start();
		}

		private void MainLoop()
		{
			_running = true;

			const int maxFps = 65;

			TimeSpan minWaitTime = TimeSpan.FromMilliseconds(1000d / maxFps);

			var captureErrorCount = 0;

			var numSamples = 10;

			var timeSamples = new Queue<TimeSpan>();

			while (_running)
			{
				lock (_configLock)
				{
					DateTime startDt = DateTime.UtcNow;

					Task<IList<CaptureResult>> captureTask = Task.Run(() => _screenCaptureService.CaptureScreenRegions(_screenRegions, 10000));
					//Task<IList<CaptureResult>> captureTask = Task.FromResult(_screenCaptureService.CaptureScreenRegions(_screenRegions, null));

					while (_running)
					{
						try
						{
							if (captureTask.Wait(10))
							{
								captureErrorCount = 0;
								break;
							}
						}
						catch (AggregateException ex)
						{
							if (captureErrorCount >= 5)
								throw;

							captureErrorCount++;
							_logger.Warn(string.Format("Capture failed with exception! Try count: {0}", captureErrorCount), ex);
							Thread.Sleep(1000);
							break;
						}

						var utcNow = DateTime.UtcNow;

						var toCommit = new HashSet<OutputService>(new ReferenceComparer<OutputService>());

						foreach (RegionConfiguration regionConfig in _regionConfigurations)
						{
							foreach (OutputConfiguration outputConfig in regionConfig.OutputConfigs)
							{
								TimeSpan? resendInterval = outputConfig.ResendIntervalFunc(outputConfig.ResendCount);

								if (resendInterval != null && outputConfig.LastColorSetTime + resendInterval.Value < utcNow)
								{
									SetColor(outputConfig, regionConfig.LastColor, true);
									toCommit.Add(outputConfig.OutputService);
									_logger.InfoFormat("[Region {0} Output {1}] Resend timeout elapsed.", outputConfig.RegionId, outputConfig.OutputId);
								}
							}
						}

						Task.WhenAll(toCommit.Select(x => Task.Run((Action) x.Commit))).Wait();
					}

					if (!captureTask.IsFaulted && _running)
						HandleCaptureResults(captureTask.Result);

					DateTime endDt = DateTime.UtcNow;

					var timeSpan = (endDt - startDt);

					TimeSpan waitTime = TimeSpan.Zero;
					if (timeSpan < minWaitTime)
					{
						waitTime = (minWaitTime - timeSpan);
						Thread.Sleep(waitTime);
					}

					timeSamples.Enqueue(timeSpan + waitTime);

					if (timeSamples.Count >= numSamples)
					{
						_logger.DebugFormat("{0:0.00} fps", (1000*numSamples)/timeSamples.Sum(x => x.TotalMilliseconds));
						timeSamples.Dequeue();
					}
				}
			}
		}

		private void HandleCaptureResults(IList<CaptureResult> captureResults)
		{
			var toCommit = new HashSet<OutputService>(new ReferenceComparer<OutputService>());

			for (var i = 0; i < _regionConfigurations.Count; i++)
			{
				RegionConfiguration regionConfig = _regionConfigurations[i];

				CaptureResult captureResult = captureResults[i];

				ColorF colorToSet;

				switch (captureResult.State)
				{
					case CaptureState.NoChanges:
						colorToSet = regionConfig.LastColor;
						break;
					case CaptureState.NewBitmap:
						_logger.DebugFormat("[Region {0}] New frame available.", regionConfig.Id);
						colorToSet = regionConfig.ColorAveragingService.GetAverageColor(captureResult.Bitmap);
						break;
					case CaptureState.ScreenNotFound:
						colorToSet = (ColorF) Color.Black;
						break;
					default:
						throw new ArgumentOutOfRangeException();
				}

				regionConfig.LastColor = colorToSet;

				foreach (OutputConfiguration outputConfig in regionConfig.OutputConfigs)
				{
					bool colorChanged = SetColor(outputConfig, colorToSet, false);
					if (colorChanged)
					{
						toCommit.Add(outputConfig.OutputService);
						_logger.InfoFormat("[Region {0} Output {1}] Color changed ({2})", outputConfig.RegionId, outputConfig.OutputId, colorToSet);
					}
				}
			}

			Task.WhenAll(toCommit.Select(x => Task.Run((Action)x.Commit))).Wait();
		}

		private bool SetColor(OutputConfiguration outputConfig, ColorF colorToSet, bool isResend)
		{
			OutputService outputService = outputConfig.OutputService;

			IList<ColorTransformerContext> colorTransformerContexts = outputConfig.ColorTransformerContexts;

			var outputColor = _colorTransformerService.Transform(colorTransformerContexts, colorToSet);

			if (!isResend && outputConfig.LastColor.HasValue && outputService.ColorsEqual(outputColor, outputConfig.LastColor.Value))
				return false;

			outputConfig.LastColor = outputColor;
			outputConfig.LastColorSetTime = DateTime.UtcNow;

			if (isResend)
				outputConfig.ResendCount++;
			else
				outputConfig.ResendCount = 0;

			outputService.SetColor(outputColor, outputConfig.OutputInfo);
			return true;
		}

		public void ReloadConfig()
		{
			lock (_configLock)
			{
				_logger.Info("Reloading config...");
				AmbiLightConfig config = ReadConfig(_configPath);
				ApplyConfig(config);
			}
		}

		private static AmbiLightConfig ReadConfig(string fileName)
		{
			return JsonConvert.DeserializeObject<AmbiLightConfig>(File.ReadAllText(fileName, Encoding.UTF8), new JsonSerializerSettings {MissingMemberHandling = MissingMemberHandling.Ignore});
		}

		public void Stop()
		{
			_running = false;
		}

		public void Dispose()
		{
			_screenCaptureService.Dispose();

			foreach (var regionConfiguration in _regionConfigurations)
			{
				regionConfiguration.Dispose();
			}
		}

		private class RegionConfiguration : IDisposable
		{
			public RegionConfiguration(int id)
			{
				Id = id;
				LastColor = (ColorF) Color.Black;
			}

			public int Id { get; private set; }
			public ScreenRegion ScreenRegion { get; set; }
			public IColorAveragingService ColorAveragingService { get; set; }
			public IList<OutputConfiguration> OutputConfigs { get; set; }
			public ColorF LastColor { get; set; }

			public void Dispose()
			{
				ColorAveragingService.Dispose();
				foreach (var outputConfig in OutputConfigs)
				{
					outputConfig.Dispose();
				}
			}
		}

		private class OutputConfiguration : IDisposable
		{
			public OutputConfiguration(int regionId, int outputId)
			{
				RegionId = regionId;
				OutputId = outputId;
			}

			public int RegionId { get; private set; }
			public int OutputId { get; private set; }
			public OutputService OutputService { get; set; }
			public IList<ColorTransformerContext> ColorTransformerContexts { get; set; }
			public ColorF? LastColor { get; set; }
			public DateTime LastColorSetTime { get; set; }
			public int ResendCount { get; set; }
			public Func<int, TimeSpan?> ResendIntervalFunc { get; set; }
			public IOutputInfo OutputInfo { get; set; }

			public void Dispose()
			{
				OutputService.Dispose();
			}
		}
	}
}