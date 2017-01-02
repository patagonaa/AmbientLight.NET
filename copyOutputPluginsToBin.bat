mkdir src\AmbientLightNet\AmbientLightNet.Configurator\bin\Debug\OutputPlugins
mkdir src\AmbientLightNet\AmbientLightNet.Configurator\x64\bin\Debug\OutputPlugins
mkdir src\AmbientLightNet\AmbientLightNet.Service\bin\Debug\OutputPlugins
mkdir src\AmbientLightNet\AmbientLightNet.Service\bin\x64\Debug\OutputPlugins

copy /Y src\AmbientLightNet\AmbientLightNet.MagicHomePlugin\bin\Debug\AmbientLightNet.MagicHomePlugin.dll src\AmbientLightNet\AmbientLightNet.Configurator\bin\Debug\OutputPlugins\
copy /Y src\AmbientLightNet\AmbientLightNet.MagicHomePlugin\bin\x64\Debug\AmbientLightNet.MagicHomePlugin.dll src\AmbientLightNet\AmbientLightNet.Configurator\bin\x64\Debug\OutputPlugins\

copy /Y src\AmbientLightNet\AmbientLightNet.MagicHomePlugin\bin\Debug\AmbientLightNet.MagicHomePlugin.dll src\AmbientLightNet\AmbientLightNet.Service\bin\Debug\OutputPlugins\
copy /Y src\AmbientLightNet\AmbientLightNet.MagicHomePlugin\bin\x64\Debug\AmbientLightNet.MagicHomePlugin.dll src\AmbientLightNet\AmbientLightNet.Service\bin\x64\Debug\OutputPlugins\

copy /Y src\AmbientLightNet\AmbientLightNet.DebugOutputPlugin\bin\Debug\AmbientLightNet.DebugOutputPlugin.dll src\AmbientLightNet\AmbientLightNet.Configurator\bin\Debug\OutputPlugins\
copy /Y src\AmbientLightNet\AmbientLightNet.DebugOutputPlugin\bin\x64\Debug\AmbientLightNet.DebugOutputPlugin.dll src\AmbientLightNet\AmbientLightNet.Configurator\bin\x64\Debug\OutputPlugins\

copy /Y src\AmbientLightNet\AmbientLightNet.DebugOutputPlugin\bin\Debug\AmbientLightNet.DebugOutputPlugin.dll src\AmbientLightNet\AmbientLightNet.Service\bin\Debug\OutputPlugins\
copy /Y src\AmbientLightNet\AmbientLightNet.DebugOutputPlugin\bin\x64\Debug\AmbientLightNet.DebugOutputPlugin.dll src\AmbientLightNet\AmbientLightNet.Service\bin\x64\Debug\OutputPlugins\

copy /Y src\AmbientLightNet\AmbientLightNet.MagicHomePlugin\bin\Debug\MagicHomeController.dll src\AmbientLightNet\AmbientLightNet.Configurator\bin\Debug\OutputPlugins\
copy /Y src\AmbientLightNet\AmbientLightNet.MagicHomePlugin\bin\x64\Debug\MagicHomeController.dll src\AmbientLightNet\AmbientLightNet.Configurator\bin\x64\Debug\OutputPlugins\

copy /Y src\AmbientLightNet\AmbientLightNet.MagicHomePlugin\bin\Debug\MagicHomeController.dll src\AmbientLightNet\AmbientLightNet.Service\bin\Debug\OutputPlugins\
copy /Y src\AmbientLightNet\AmbientLightNet.MagicHomePlugin\bin\x64\Debug\MagicHomeController.dll src\AmbientLightNet\AmbientLightNet.Service\bin\x64\Debug\OutputPlugins\

pause
