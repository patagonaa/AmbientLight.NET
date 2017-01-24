@echo off

SET pluginName=AmbientLightNet.MagicHomePlugin
CALL :copyForAllExecutablesAndConfigs

SET pluginName=AmbientLightNet.DebugOutputPlugin
CALL :copyForAllExecutablesAndConfigs

SET pluginName=AmbientLightNet.SerialOutputPlugin
CALL :copyForAllExecutablesAndConfigs

GOTO :end

:copyForAllExecutablesAndConfigs

SET executingAssembly=AmbientLightNet.Service
CALL :copyForAllConfigs

SET executingAssembly=AmbientLightNet.Configurator
CALL :copyForAllConfigs

GOTO :eof

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
