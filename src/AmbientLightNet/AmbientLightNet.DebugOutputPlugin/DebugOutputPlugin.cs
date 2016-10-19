using AmbiLightNet.PluginBase;

namespace AmbientLightNet.DebugOutputPlugin
{
    public class DebugOutputPlugin : IOutputPlugin
    {
	    public const string PluginName = "Debug";

	    public string DisplayName { get { return "Debug Output Plugin"; } }
	    public string Name { get { return PluginName; } }

	    public IOutputInfo GetNewOutputInfo()
	    {
		    return new DebugOutputInfo();
	    }

	    public OutputService GetNewOutputService()
	    {
		    return new DebugOutputService();
	    }

	    public OutputConfigDialog GetOutputConfigDialog()
	    {
		    return new DebugOutputConfigDialog();
	    }
	}
}
