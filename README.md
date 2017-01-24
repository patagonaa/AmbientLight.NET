# AmbientLight.NET

An open source .NET ambient light

It supports:
* multiple capture regions
* different kinds of outputs (plugin support)

supported by the service but not selectable in the configurator (look at the config models 
* multiple outputs per region (currently not configurable in configurator)
* color transformers (gamma correction, brightness correction, hysteresis, ...)

It consists of the two main executables and the output plugins

## Configurator

UI to configure the capture regions and outputs.

Config files can be loaded from and saved to any directory.

## Service

Captures the screen regions and outputs the colors via the configured output plugin.

It can be run as a console application or installed as a Windows service.

The config file path is configured in the App.config of the service (the default path is `./config.json`).

If the service is running and the config is replaced, the new config will be loaded and applied automatically.

## Output plugins

They contain the code to configure the output and actually control the output.

For example, the `MagicHomePlugin` output plugin:

On the configurator side it finds the LED controller via UDP broadcast and saves the MAC address in the config file,
on the service side it gets the device by the MAC address and sends the packet to set the color via TCP.