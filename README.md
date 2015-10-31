# __ionide-websharper__
This package implements a plugin for [Atom](https://atom.io) that you can use alongside with [Ionide](http://ionide.io) to run [WebSharper](http://websharper.com) sitelets conveniently. It depends only on [ionide-fsharp](https://github.com/ionide/ionide-fsharp), which provides syntax highlighting and code assistance features for F# projects.

## Features
* Compile and run client-server WebSharper applications
* Error reporting

## Basic usage
It is really easy to get started with __ionide-websharper__, you just need to follow three main steps:

1. Open up a WebSharper project that contains a [sitelet](http://websharper.com/docs/sitelets)

    This can be an existing project created with, for example, Visual Studio or you can use [ionide-yeoman](https://github.com/ionide/ionide-yeoman/) to generate one directly in Atom.

2. Press `Ctrl` + `Shift` + `P` and start typing "Sitelet" (without the quotes)
3. Select the command `sitelet:run` and hit `Enter`

![ionide-websharper](http://i.imgur.com/uw20nIf.gif)

And voilá, your Sitelet is running on `http://localhost:9000/`.

## Commands
Command      | Description
------------ | -----------
sitelet:run  | Starts your WebSharper project or restarts the running one.
sitelet:stop | Stops the recently started project.

## Building
1. Run `build.cmd` (for Windows) or `build.sh`, this will start a clean build and copy the necessary files in place.
2. Go into the `ionide-websharper` directory and install the package with `apm link`.

## Contributing
We are hosting the project on GitHub, so feel free to report issues, fork the repository, and send pull requests!

## Licensing
__ionide-websharper__ is licensed under [Apache 2.0](https://github.com/intellifactory/ionide-websharper/blob/master/LICENSE.md).
