# .NET core 6 LTS Console App template

## Overview

The .NET Console App template:

- From Visual Studio:

![Visual Studio 2022 Console App project template](/img/ConsoleApp.png)

- Or command-line:

`dotnet new console --framework net6.0 --use-program-main`

generates a very basic Console App.

The template lacks important and useful features:

- [Logging](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/logging/?view=aspnetcore-6.0),
- [Configuration / Application settings](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/configuration/?view=aspnetcore-6.0),
- Separation of "doing something (modules/pluging/interfaces)" from "launching the program (core)", which is close to a [Microkernel architecture pattern](https://en.wikipedia.org/wiki/Microkernel.net),
- [Dependency Injection](https://learn.microsoft.com/en-us/dotnet/core/extensions/dependency-injection) (for adaptive coding and unit test capabilities),
- [Asynchronous programming](https://learn.microsoft.com/en-us/dotnet/csharp/asynchronous-programming/) (recommended to use with WebAPI REST calls and Cloud (Azure) resources).

So I created this template that adds all of these built-in.

It allows me to get started fast for all my coding Proof of Concepts.

Enjoy,

Emmanuel.

## Disclaimer

It is a starting point.

There are a lots of features and practices that can, and should, be added.

I use `.NET 6 LTS` version. It is a personal choice, not a recommendation.

This is a **WAY BETTER** starting point than the default "Console App".
