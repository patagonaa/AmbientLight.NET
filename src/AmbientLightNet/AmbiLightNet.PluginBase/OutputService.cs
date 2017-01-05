using System;

namespace AmbiLightNet.PluginBase
{
	public abstract class OutputService : IDisposable
	{
		public abstract void Initialize(IOutputInfo outputInfo);

		public abstract void Output(ColorF color);

		public abstract bool ColorsEqual(ColorF first, ColorF second);

		public virtual void Dispose()
		{
		}
	}

	public abstract class OutputService<TOutputType> : OutputService where TOutputType : class, IOutputInfo
	{
		public sealed override void Initialize(IOutputInfo outputInfo)
		{
			Initialize((TOutputType) outputInfo);
		}

		public abstract void Initialize(TOutputType outputType);

	}
}