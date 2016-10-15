using System;

namespace AmbiLightNet.PluginBase
{
	public abstract class OutputService : IDisposable
	{
		public abstract bool IsForOutputType(Type type);

		public abstract void Initialize(IOutputInfo outputInfo);

		public abstract void Output(double r, double g, double b);

		public virtual void Dispose()
		{
		}
	}

	public abstract class OutputService<TOutputType> : OutputService where TOutputType : class, IOutputInfo
	{
		public sealed override bool IsForOutputType(Type type)
		{
			return type == typeof(TOutputType);
		}

		public sealed override void Initialize(IOutputInfo outputInfo)
		{
			Initialize((TOutputType) outputInfo);
		}

		public abstract void Initialize(TOutputType outputType);

	}
}