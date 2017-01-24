namespace AmbiLightNet.PluginBase
{
	public abstract class SimpleOutputService<TOutputInfo> : OutputService<TOutputInfo> where TOutputInfo : class, IOutputInfo
	{
		private ColorF Color { get; set; }

		public override void SetColor(ColorF color, TOutputInfo outputInfo)
		{
			Color = color;
		}

		public sealed override void Commit()
		{
			Output(Color);
		}

		protected abstract void Output(ColorF color);

		public sealed override bool IsReusableFor(TOutputInfo outputInfo)
		{
			return false;
		}
	}
}