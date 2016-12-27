using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using AmbientLightNet.ScreenCapture.Infrastructure;

namespace AmbientLightNet.Infrastructure.AmbiLightConfig
{
	[Serializable]
	public class ScreenRegionOutput : ISerializable
	{
		public ScreenRegion ScreenRegion { get; set; }
		public IList<Output> Outputs { get; set; }
		public ColorAveragingConfig ColorAveragingConfig { get; set; }

		public ScreenRegionOutput()
		{
		}

		protected ScreenRegionOutput(SerializationInfo info, StreamingContext context)
		{
			try
			{
				ColorAveragingConfig = (ColorAveragingConfig)info.GetValue("ColorAveragingConfig", typeof(ColorAveragingConfig));
			}
			catch (SerializationException)
			{
			}

			ScreenRegion = (ScreenRegion)info.GetValue("ScreenRegion", typeof(ScreenRegion));
			Outputs = (IList<Output>)info.GetValue("Outputs", typeof(IList<Output>));
		}

		public void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.AddValue("ScreenRegion", ScreenRegion);
			info.AddValue("Outputs", Outputs);
			info.AddValue("ColorAveragingConfig", ColorAveragingConfig);
		}

		public override string ToString()
		{
			return ScreenRegion == null ? "" : ScreenRegion.ToString();
		}
	}
}