# __ionide-websharper__
This package is part of the [Ionide](https://github.com/ionide/) plugin suite which enables [WebSharper](http://websharper.com) usage in the [Atom](https://atom.io) text editor. It depends only on [ionide-fsharp](https://github.com/ionide/ionide/ionide-fsharp) to provide error highlighting and many other IDE-like capabilities.

## Features
* Compile and run for client-server applications
* Error reporting

## Basic usage
It is really easy to get started with __ionide-websharper__, you just need to follow 3 main steps

1. Open up a WebSharper project that contains a [sitelet](http://websharper.com/docs/sitelets)

    This can be an existing project created with for example Visual Studio or you can use [ionide-yeoman](https://github.com/ionide/ionide-yeoman/) to generate one directly in Atom.
    
2. Press `Ctrl` + `Shift` + `P` and start typing "Sitelet" (without the quotes)
3. Select the command called `sitelet:run` and hit `Enter`

![ionide-websharper](http://i.imgur.com/cTy9wnx.gif)

Voilá, your Sitelet started to listen on `http://localhost:9000/`.

## Commands
Command      | Description
------------ | -----------
sitelet:run  | Tries to start the first project or restarts the running one.
sitelet:stop | Stops the recently started project.

## Building
1. Run `build.cmd` (for Windows) or `build.sh`, this will start a clean build and then it will copy the necessary binary files to the right place
2. Go into the `ionide-websharper` directory and install the package with `apm link`

## Contributing
We are hosting the project on GitHub, so feel free to report issues, fork the repository and send pull request!

## Licensing
__ionide-websharper__ is licensed under [Apache 2.0](https://github.com/intellifactory/ionide-websharper/blob/master/LICENSE.md).
