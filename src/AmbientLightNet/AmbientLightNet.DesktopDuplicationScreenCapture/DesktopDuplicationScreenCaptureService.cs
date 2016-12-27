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
		private const int FrameTimeout = 5;

		private readonly IDictionary<string, Capture> _capturesByScreen = new Dictionary<string, Capture>();

		private readonly IScreenRegionBitmapProvider _bitmapProvider;

		public DesktopDuplicationScreenCaptureService(bool useCache)
		{
			_bitmapProvider = useCache
				? (IScreenRegionBitmapProvider) new CachedScreenRegionBitmapProvider()
				: (IScreenRegionBitmapProvider) new NonCachedScreenRegionBitmapProvider();
		}

		public IList<Bitmap> CaptureScreenRegions(IList<ScreenRegion> regions)
		{
			var regionBitmapsDictionary = new Dictionary<ScreenRegion, Bitmap>();
			foreach (IGrouping<string, ScreenRegion> grouping in regions.GroupBy(x => x.ScreenName))
			{
				Capture capture;
				if (!_capturesByScreen.TryGetValue(grouping.Key, out capture))
					_capturesByScreen[grouping.Key] = capture = new Capture(grouping.Key);

				OutputDuplicateFrameInformation frameInformation;
				if (!CaptureFrame(capture, out frameInformation)) 
					continue; // if false, capture timed out

				FrameMetadata metaData = GetMetadata(capture, frameInformation);

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

		private bool RegionIsDirty(Rectangle screenRegionRectangle, RawRectangle[] dirtyRectangles, OutputDuplicateMoveRectangle[] moveRectangles)
		{
			return dirtyRectangles.Any(x => screenRegionRectangle.IntersectsWith(x.ToRectangle())) || 
				moveRectangles.Select(x => x.DestinationRect).Any(x => screenRegionRectangle.IntersectsWith(x.ToRectangle()));
		}

		[DllImport("msvcrt.dll", EntryPoint = "memcpy", CallingConvention = CallingConvention.Cdecl, SetLastError = false)]
		private static extern IntPtr memcpy(IntPtr dest, IntPtr src, uint count);

		private void CopyPart(Bitmap dest, DataBox src, int srcPositionX, int srcPositionY)
		{
			int width = dest.Width;
			int height = dest.Height;
			BitmapData destData = dest.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.WriteOnly, PixelFormat.Format32bppRgb);

			const int bytesPerPixel = 4;
			var rowBytes = (uint)(width * bytesPerPixel);
			for (var y = 0; y < height; y++)
			{
				memcpy(destData.Scan0 + (destData.Stride * y), src.DataPointer + (src.RowPitch * (y + srcPositionY) + (srcPositionX * bytesPerPixel)), rowBytes);
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

		private bool CaptureFrame(Capture capture, out OutputDuplicateFrameInformation frameInformation)
		{
			try
			{
				Resource desktopResource;
				capture.OutputDuplication.AcquireNextFrame(FrameTimeout, out frameInformation, out desktopResource);

				using (var tempTexture = desktopResource.QueryInterface<Texture2D>())
				{
					capture.Device.ImmediateContext.CopyResource(tempTexture, capture.Texture);
				}

				desktopResource.Dispose();
			}
			catch (SharpDXException ex)
			{
				frameInformation = default(OutputDuplicateFrameInformation);

				if (ex.ResultCode.Code == ResultCode.WaitTimeout.Result.Code)
				{
					return false;
				}

				throw;
			}

			return true;
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
			public Capture(string deviceName)
			{
				var factory = new Factory1();

				Adapter[] adapters = factory.Adapters;

				foreach (Adapter adapter in adapters)
				{
					foreach (Output output in adapter.Outputs)
					{
						OutputDescription outputDescription = output.Description;

						if (!outputDescription.IsAttachedToDesktop || outputDescription.DeviceName.Trim('\0') != deviceName) 
							continue;

						RawRectangle desktopBounds = outputDescription.DesktopBounds;
						DesktopBounds = desktopBounds;
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
							SampleDescription = { Count = 1, Quality = 0 },
							Usage = ResourceUsage.Staging
						};

						Device = new Device(adapter);

						var output1 = output.QueryInterface<Output1>();
						OutputDuplication = output1.DuplicateOutput(Device);
						Texture = new Texture2D(Device, textureDescription);
						return;
					}
				}
			}

			public RawRectangle DesktopBounds { get; private set; }
			public Device Device { get; private set; }
			public OutputDuplication OutputDuplication { get; private set; }
			public Texture2D Texture { get; private set; }

			public void Dispose()
			{
				Device.Dispose();
				OutputDuplication.Dispose();
				Texture.Dispose();
			}
		}

		private class FrameMetadata
		{
			public OutputDuplicateMoveRectangle[] MoveRectangles { get; set; }
			public RawRectangle[] DirtyRectangles { get; set; }
		}
    }
}
