# LiteNetLibDebugApp

A very tiny, very minimal LNL Debugging/Chat App. When you run this app, it will start a very simple LNL Server and/or(see #Configuration), that can send messages between each other. It is designed to assist in LNl Debugging.

## Purpose
1. Test LNL without the overhead of Resonite
2. Be a learning tool for the [.NET Generic Host](https://learn.microsoft.com/en-us/dotnet/core/extensions/generic-host?tabs=appbuilder)
3. Be as simple as possible.
4. Use as much Microsoft/.NET Stuff as possible

## Usage 
1. Download and Build (Built Libraries coming soon)
1. Edit appsettings.json as you'd like (see #Configuration)
1. Run

### Configuration
Application settings can be found in appsettings.json. Some common Configuration items are explained here to assist, but the options should be fairly self-explanatory.

You can:
- Enable and disable the client/server side by setting the AppOptions.Server/Client Enabled properties to true/false
- Edit the ports of the client/server by editing that section.
    - The Client configuration requires both a Local Listen Port and a Remote Connection Port and Remote Address to connect to.
    - The Server configuration just requires a Local Listen Port.

You can also use CLI flags, if you want to, to do this:
1. Disable both the client and server in the `appsettings.json` file
1. Run the application using `--server=true` or `--client=true` to selectively run the app as a client and/or server

## Operation
### Server
The server has no interactive capabilities, it just takes incoming messages and sends them back out to any connected users.

### Client
Once connected, type any message into the console and press enter, it will be sent to the user, which will hopefully return it back.

## Recommendations
We recommend using this in the following way:
1. Deploy a copy of this app to your "Server" machine
1. Set the client to disabled on this server machine, set the ports appropriate
1. Deploy a copy of this app to your "Client" machine
1. Set the Client to connect to the machine and port from the previous steps
1. Run the server
1. Run the client

You should have a valid connection at this point.

## Guidelines for contributing
1. Avoid non System.* or Microsoft.* packages.
    - The goal here is to learn those weirder packages, within the .NET Runtime that are really useful but that no one seems to use that much.
1. Avoid adding additional bloat
    - If it is being added, it should have a good reason.
1. Keep in mind the Purpose


## Why?
Part of being in this industry is ensuring you're staying up to date, it is not often I get to play around in .NET 8.0 and I wanted to learn the generic host.

The generic host/host stack/I dunno basically Microsoft.Extensions.* area is full of tasks I find myself doing manually all the time, including:
1. Logging
1. Configuration
    - ENV Variables
    - JSON Files (Without System.Text.Json OR Newtonsoft)
    - CLI Flags
1. Dependency Injection

Which I tend to accidentally do a lot in smaller apps or scripts that I write, It's been much easier in the past to just JsonSerialize.Deserialize a file for example.


## Won't
I won't / this project won't:
1. Aim to be compatible in any way with Resonite
1. Aim to be a game or any concrete product

## Might
1. LNL Hole Punching and Relaying
1. Usernames to make this a "Real" chat room. (I'll probably just use the machine name)


## Resources
When I write stuff like this I always like to keep a running list of resources where I've learnt information from, it helps with Searching and helps you see how difficult this was:
- [Command Line Parsing](https://learn.microsoft.com/en-us/dotnet/api/microsoft.extensions.configuration.commandlineconfigurationextensions.addcommandline?view=dotnet-plat-ext-8.0#microsoft-extensions-configuration-commandlineconfigurationextensions-addcommandline(microsoft-extensions-configuration-iconfigurationbuilder-system-string()-system-collections-generic-idictionary((system-string-system-string)))): This was really confusing, till I read this specify version of AddCommandLine
- [.NET Generic Host docs](https://learn.microsoft.com/en-us/dotnet/core/extensions/generic-host)
- [Configuration](https://learn.microsoft.com/en-us/dotnet/core/extensions/configuration)
- [Logging](https://learn.microsoft.com/en-us/dotnet/core/extensions/logging?tabs=command-line)
- [Lite Net Lib](https://github.com/RevenantX/LiteNetLib)
- [net-generic-host-boilerplate](https://github.com/marceln/net-generic-host-boilerplate)
- [NReco.Logging.File](https://github.com/nreco/logging): This was the simplest file logger I could find, .NET itself doesn't have one and I didn't want to roll out SeriLog/NLog
- [Options Pattern](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/configuration/options?view=aspnetcore-8.0)