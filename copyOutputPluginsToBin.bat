@echo off

SET pluginName=AmbientLightNet.MagicHomePlugin
SET executingAssembly=AmbientLightNet.Configurator
CALL :copyForAllConfigs

SET pluginName=AmbientLightNet.MagicHomePlugin
SET executingAssembly=AmbientLightNet.Service
CALL :copyForAllConfigs

SET pluginName=AmbientLightNet.DebugOutputPlugin
SET executingAssembly=AmbientLightNet.Configurator
CALL :copyForAllConfigs

SET pluginName=AmbientLightNet.DebugOutputPlugin
SET executingAssembly=AmbientLightNet.Service
CALL :copyForAllConfigs

GOTO :end

:copyForAllConfigs

SET config=Debug
CALL :copy
SET config=x64\Debug
CALL :copy
SET config=Release
CALL :copy
SET config=x64\Release
CALL :copy

GOTO :eof

:copy

mkdir src\AmbientLightNet\%executingAssembly%\bin\%config%\OutputPlugins
copy /Y src\AmbientLightNet\%pluginName%\bin\%config%\*.dll src\AmbientLightNet\%executingAssembly%\bin\%config%\OutputPlugins

GOTO :eof


:end
pause
