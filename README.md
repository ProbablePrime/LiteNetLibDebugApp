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

### Server
The server has no interactive capabilities, it just takes incoming messages and sends them back out to any connected users.

### Client
Once connected, type any message into the console and press enter, it will be sent to the user, which will hopefully return it back.

## Configuration
All application settings can be found in appsettings.json. Some common Configuration items are presented here to assist.

- You can enable and disable the client/server side by setting the AppOptions.Server/Client Enabled properties to true/false
- You can edit the ports of the client/server by editing that section.
    - The Client configuration requires both a Local Listen Port and a Remote Connection Port and Remote Address to connect to.
    - The Server configuration just requires a Local Listen Port.


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
1. File based Logging