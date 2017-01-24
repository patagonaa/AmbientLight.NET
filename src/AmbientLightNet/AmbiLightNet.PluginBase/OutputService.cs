using System;

namespace AmbiLightNet.PluginBase
{
	public abstract class OutputService : IDisposable
	{
		public abstract void Initialize(IOutputInfo outputInfo);

		public abstract void SetColor(ColorF color, IOutputInfo outputInfo);

		public abstract void Commit();

		public abstract bool ColorsEqual(ColorF first, ColorF second);

		public abstract TimeSpan? GetResendInterval(int resendCount);

		public abstract bool IsReusableFor(IOutputInfo outputInfo);

		public virtual void Dispose()
		{
		}
	}

	public abstract class OutputService<TOutputInfo> : OutputService where TOutputInfo : class, IOutputInfo
	{
		public sealed override void Initialize(IOutputInfo outputInfo)
		{
			Initialize((TOutputInfo) outputInfo);
		}

		public abstract void Initialize(TOutputInfo outputInfo);

		public sealed override void SetColor(ColorF color, IOutputInfo outputInfo)
		{
			SetColor(color, (TOutputInfo) outputInfo);
		}

		public abstract void SetColor(ColorF color, TOutputInfo outputInfo);

		public sealed override bool IsReusableFor(IOutputInfo outputInfo)
		{
			return (outputInfo is TOutputInfo) && IsReusableFor((TOutputInfo) outputInfo);
		}

		public abstract bool IsReusableFor(TOutputInfo outputInfo);
	}
}