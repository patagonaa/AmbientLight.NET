using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using AmbientLightNet.ScreenCapture.Infrastructure;
using SharpDX;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using SharpDX.Mathematics.Interop;
using Device = SharpDX.Direct3D11.Device;
using MapFlags = SharpDX.Direct3D11.MapFlags;
using Resource = SharpDX.DXGI.Resource;
using ResultCode = SharpDX.DXGI.ResultCode;

namespace AmbientLightNet.DesktopDuplicationScreenCapture
{
	public class DesktopDuplicationScreenCaptureService : IScreenCaptureService
	{
		private readonly IDictionary<string, Capture> _capturesByScreen = new Dictionary<string, Capture>();

		private readonly IScreenRegionBitmapProvider _bitmapProvider;

		public DesktopDuplicationScreenCaptureService(bool useCache)
		{
			if (useCache)
				_bitmapProvider = new CachedScreenRegionBitmapProvider();
			else
				_bitmapProvider = new NonCachedScreenRegionBitmapProvider();
		}

		public IList<Bitmap> CaptureScreenRegions(IList<ScreenRegion> regions, bool mayBlockIfNoChanges)
		{
			var regionBitmapsDictionary = new Dictionary<ScreenRegion, Bitmap>();

			var regionsByScreen = regions.GroupBy(x => x.ScreenName).ToList();

			if (regionsByScreen.Count == 0)
				return new List<Bitmap>();

			int timeoutMilliseconds;

			if (regionsByScreen.Count == 1 && mayBlockIfNoChanges)
			{
				timeoutMilliseconds = int.MaxValue; // we could actually use windows.h's "INFINITE" constant but SharpDX's OutputDuplication.AcquireNextFrame doesn't take a UINT
			}
			else
			{
				timeoutMilliseconds = 0; // do not wait for frame, use latest frame if one is available
			}
			
			do
			{
				foreach (IGrouping<string, ScreenRegion> grouping in regionsByScreen)
				{
					Capture capture;
					string screenName = grouping.Key;
					if (!_capturesByScreen.TryGetValue(screenName, out capture))
						_capturesByScreen[screenName] = capture = new Capture(screenName);

					CaptureFrameIfAvailable(capture, timeoutMilliseconds);
				}
			} while (mayBlockIfNoChanges && regionsByScreen.All(x => _capturesByScreen[x.Key].LastFrameInfo == null));

			foreach (IGrouping<string, ScreenRegion> grouping in regionsByScreen)
			{
				Capture capture = _capturesByScreen[grouping.Key];

				if (capture.LastFrameInfo == null) //last frame does not exist or timed out
					continue;

				FrameMetadata metaData = GetMetadata(capture, capture.LastFrameInfo.Value);

				if (metaData != null) // if null, there are no dirty regions in this frame
				{
					DataBox dataBox = capture.Device.ImmediateContext.MapSubresource(capture.Texture, 0, MapMode.Read, MapFlags.None);

					Rectangle desktopBounds = capture.DesktopBounds.ToRectangle();

					foreach (ScreenRegion screenRegion in grouping)
					{
						var screenRegionX = (int) (desktopBounds.Width*screenRegion.Rectangle.X);
						var screenRegionY = (int) (desktopBounds.Height*screenRegion.Rectangle.Y);

						var screenRegionWidth = (int) (desktopBounds.Width*screenRegion.Rectangle.Width);
						var screenRegionHeight = (int) (desktopBounds.Height*screenRegion.Rectangle.Height);

						var screenRegionRectangle = new Rectangle(screenRegionX, screenRegionY, screenRegionWidth, screenRegionHeight);

						if (!RegionIsDirty(screenRegionRectangle, metaData.DirtyRectangles, metaData.MoveRectangles))
							continue; // no changes in this area

						Bitmap bitmap = _bitmapProvider.ProvideForScreenRegion(screenRegion, screenRegionWidth, screenRegionHeight, PixelFormat.Format32bppRgb);

						CopyPart(bitmap, dataBox, screenRegionX, screenRegionY);

						regionBitmapsDictionary[screenRegion] = bitmap;
					}

					capture.Device.ImmediateContext.UnmapSubresource(capture.Texture, 0);
				}

				capture.OutputDuplication.ReleaseFrame();
			}

			var toReturn = new List<Bitmap>();
			foreach (ScreenRegion screenRegion in regions)
			{
				Bitmap bitmap;
				toReturn.Add(regionBitmapsDictionary.TryGetValue(screenRegion, out bitmap) ? bitmap : null);
			}

			return toReturn;
		}

		private void CaptureFrameIfAvailable(Capture capture, int timeoutMilliseconds)
		{
			OutputDuplicateFrameInformation frameInformation = default(OutputDuplicateFrameInformation);

			bool frameAvailable;

			if (!capture.Initialized)
			{
				try
				{
					capture.ReInitialize();
				}
				catch (ScreenNotFoundException)
				{
					capture.LastFrameInfo = null;
					return;
				}
			}

			try
			{
				Resource desktopResource;
				
				capture.OutputDuplication.AcquireNextFrame(timeoutMilliseconds, out frameInformation, out desktopResource);
				frameAvailable = true;

				using (var tempTexture = desktopResource.QueryInterface<Texture2D>())
				{
					capture.Device.ImmediateContext.CopyResource(tempTexture, capture.Texture);
				}

				desktopResource.Dispose();
			}
			catch (SharpDXException ex)
			{
				if (ex.ResultCode.Code == ResultCode.WaitTimeout.Result.Code)
				{
					frameAvailable = false;
				}
				else if (ex.ResultCode.Code == ResultCode.AccessLost.Code)
				{
					capture.Dispose();
					
					frameAvailable = false;
				}
				else
				{
					throw;
				}
			}

			capture.LastFrameInfo = frameAvailable ? frameInformation : (OutputDuplicateFrameInformation?) null;
		}

		private bool RegionIsDirty(Rectangle screenRegionRectangle, RawRectangle[] dirtyRectangles, OutputDuplicateMoveRectangle[] moveRectangles)
		{
			return dirtyRectangles.Select(x => x.ToRectangle()).Any(screenRegionRectangle.IntersectsWith) || 
				moveRectangles.Select(x => x.DestinationRect.ToRectangle()).Any(screenRegionRectangle.IntersectsWith);
		}

		private static void CopyPart(Bitmap dest, DataBox src, int srcPositionX, int srcPositionY)
		{
			int width = dest.Width;
			int height = dest.Height;
			BitmapData destData = dest.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.WriteOnly, PixelFormat.Format32bppRgb);

			const int bytesPerPixel = 4;
			var rowBytes = width * bytesPerPixel;

			for (var y = 0; y < height; y++)
			{
				IntPtr dstPtr = destData.Scan0 + (destData.Stride * y);
				IntPtr srcPtr = src.DataPointer + (src.RowPitch * (y + srcPositionY) + (srcPositionX * bytesPerPixel));

				Utilities.CopyMemory(dstPtr, srcPtr, rowBytes);
			}

			dest.UnlockBits(destData);
		}

		private FrameMetadata GetMetadata(Capture capture, OutputDuplicateFrameInformation frameInformation)
		{
			int totalBufferSize = frameInformation.TotalMetadataBufferSize;
			if (totalBufferSize == 0)
				return null;

			var toReturn = new FrameMetadata();

			int actualLength;

			var moveBuffer = new OutputDuplicateMoveRectangle[totalBufferSize];
			capture.OutputDuplication.GetFrameMoveRects(moveBuffer.Length, moveBuffer, out actualLength);
			Array.Resize(ref moveBuffer, actualLength/Marshal.SizeOf(typeof (OutputDuplicateMoveRectangle)));
			toReturn.MoveRectangles = moveBuffer;

			var dirtyBuffer = new RawRectangle[totalBufferSize];
			capture.OutputDuplication.GetFrameDirtyRects(dirtyBuffer.Length, dirtyBuffer, out actualLength);
			Array.Resize(ref dirtyBuffer, actualLength/Marshal.SizeOf(typeof (RawRectangle)));
			toReturn.DirtyRectangles = dirtyBuffer;

			return toReturn;
		}

		public void Dispose()
		{
			foreach (Capture capture in _capturesByScreen.Values)
			{
				capture.Dispose();
			}

			_bitmapProvider.Dispose();
		}

		private class Capture : IDisposable
		{
			public Capture(string screenName)
			{
				ScreenName = screenName;
				InitInternal();
			}

			public void ReInitialize()
			{
				CleanupInternal();
				InitInternal();
			}

			private void InitInternal()
			{
				var factory = new Factory1();

				Adapter[] adapters = factory.Adapters;

				foreach (Adapter adapter in adapters)
				{
					foreach (Output output in adapter.Outputs)
					{
						OutputDescription outputDescription = output.Description;

						if (!outputDescription.IsAttachedToDesktop || outputDescription.DeviceName.Trim('\0') != ScreenName)
							continue;

						RawRectangle desktopBounds = outputDescription.DesktopBounds;
						_desktopBounds = desktopBounds;
						var textureDescription = new Texture2DDescription
						{
							CpuAccessFlags = CpuAccessFlags.Read,
							BindFlags = BindFlags.None,
							Format = Format.B8G8R8A8_UNorm,
							Width = desktopBounds.Right - desktopBounds.Left,
							Height = desktopBounds.Bottom - desktopBounds.Top,
							OptionFlags = ResourceOptionFlags.None,
							MipLevels = 1,
							ArraySize = 1,
							SampleDescription = {Count = 1, Quality = 0},
							Usage = ResourceUsage.Staging
						};

						_device = new Device(adapter);

						var output1 = output.QueryInterface<Output1>();
						_outputDuplication = output1.DuplicateOutput(_device);
						_texture = new Texture2D(_device, textureDescription);
						_initialized = true;
						return;
					}
				}

				throw new ScreenNotFoundException(ScreenName);
			}

			public readonly string ScreenName;
			private RawRectangle _desktopBounds;
			private Device _device;
			private OutputDuplication _outputDuplication;
			private Texture2D _texture;
			private bool _initialized;

			public OutputDuplicateFrameInformation? LastFrameInfo { get; set; }

			public RawRectangle DesktopBounds
			{
				get { return _desktopBounds; }
			}

			public Device Device
			{
				get { return _device; }
			}

			public OutputDuplication OutputDuplication
			{
				get { return _outputDuplication; }
			}

			public Texture2D Texture
			{
				get { return _texture; }
			}

			public bool Initialized
			{
				get { return _initialized; }
			}

			public void Dispose()
			{
				CleanupInternal();
				_initialized = false;
			}

			private void CleanupInternal()
			{
				if (_device != null)
					_device.Dispose();
				if (_outputDuplication != null)
					_outputDuplication.Dispose();
				if (_texture != null)
					_texture.Dispose();
			}
		}

		private class FrameMetadata
		{
			public OutputDuplicateMoveRectangle[] MoveRectangles { get; set; }
			public RawRectangle[] DirtyRectangles { get; set; }
		}
    }
}
