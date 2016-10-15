namespace AmbiLightNet.PluginBase
{
	public interface IOutputPlugin
	{
		string DisplayName { get; }
		string Name { get; }
		IOutputInfo GetNewOutputInfo();
		OutputService GetNewOutputService();
		OutputConfigDialog GetOutputConfigDialog();
	}
}