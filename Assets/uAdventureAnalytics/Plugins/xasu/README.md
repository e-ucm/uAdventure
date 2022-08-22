# Xasu - xAPI Analytics Supplier

<img src="https://user-images.githubusercontent.com/3171485/173609418-2cc2def0-2631-4c1a-adeb-b68e072a02e0.png" width="300px" height="300px">

The Xasu (xAPI Analytics Supplier) is a Unity (https://unity.com/) asset that simplifies use of [xAPI](https://xapi.com/) Learning Analytics from Unity with straightforward cmi5 support.

Xasu is a *tracker*, which, when integrated into a game, can send and/or store player interactions for later analysis. This is especially important when proving that a serious game is effective at its goal, be it teaching, training, or changing the players' perspective on real-world issues. As a tracker, Xasu simplifies the work of serious games developers (or of any application using LA) by providing both simple tracking and multipe quality-of-life utilities with which to configure the collection of xAPI traces in a flexible and robust way. Xasu has been developed by the [e-UCM group](https://www.e-ucm.es) and is part of the e-UCM ecosystem of tools for Learning Analytics (Simva, T-mon, Pumva, μfasa and Xasu).

# The "Super" in Xasu

The *su* in Xasu also stands for *super*, since it is:

- Super **Simple** (High-Level API): provides a high-level API that represents xAPI profiles for serious games and CMI-5. Using these APIs, the system will take care of setting up most of the trace structure automatically and with sane defaults. This reduces the learning curve with xAPI, while also allowing developers to refine the produced trace. 

- Super **Supportive** (multi-platform/protocol/cmi5): Xasu has been designed to run on Unity, respecting the nature of cross-platform games and environments (Windows, Mac, Linux, Android and WebGL, iOS incoming). In addition, Xasu supports multiple authorization protocols (basic/oauth/oauth2) and the cmi5 protocol for conducting courses and activities with xAPI in Learning Management Systems (LMSs).

- Super **Asyncronous** (uses async/await): Xasu's architecture provides a proper asynchronous queue to avoid interruptions that allows deveñpèrs to use the .NET asynchronous API (async/await) and to check on the result of sending synchronously-sent traces, even if the traces are sent in batches.

- Super **Flexible** (working modes/backups): Xasu can operate in different modes, including an online mode (connected to an LRS), offline mode (generating a local log file in xAPI or CSV), in fallback mode (hybrid online and local depending on connectivity), and in backup mode (generating and/or sending a single file with all traces at the end).

- Super **Reliable** (communication policies and error resiliency): Xasu uses retry policies with exponential backoff and circuit-breakers to provide recovery intervals and fallback mechanisms.

# Documentation

Please find the full Xasu documentation in the Wiki section of this project.
* Wiki: https://github.com/e-ucm/xasu/wiki

# Quick Start Guide

Xasu requires at least **Unity 2019.4 (LTS)**.

## Installation

Xasu can be downloaded through the Unity Package Manager using the [repository link](https://github.com/e-ucm/xasu.git) of this project.

To add it to your proyect:
* Go to ``Window > Package Manager``
* Press the "+" icon.
* Select ``Add package from git...``.
* Insert ```https://github.com/e-ucm/xasu.git``` 
* Press "Add".

If you want to manually include Xasu into your project (for example, by downloading the repository as a .zip), make sure you install also the NewtonSoft.JSON library using the Unity Package Manager.

## Setting up the configuration file

The Xasu configuration file contains settings for overall system tuning, LRS endpoint location, authorization protocols and options, and working mode selection. The tracker configuration can be provided either using the `StreamingAssets` folder (recommended) or via scripting. We recommend using the `StreamingAssets` folder to allow configuration to be changed after the game is exported, allowing simpler adaptation of the game to different scenarios without having to recompila the whole game.

### Minimal "tracker_config.json"

The configuration file must be placed in:

```path
Assets/StreamingAssets/tracker_config.json
```

The following configuration file represents the minimal tracker configuration required to send traces to an LRS using basic authentication. 

```json
{
    "online": "true",
    "lrs_endpoint": "<your-lrs-endpoint>",
    "auth_protocol": "basic", 
    "auth_parameters": {
        "username": "<your-username>",
        "password": "<your-password>"
    }
}
```

### Authorization

Other types of supported authorizations include: None, Basic Http-Based Authorization, OAuth 1.0 and OAuth 2.0.
Please visit our Wiki to get more details about how to configure the authorization according to your platform.

* Authorization protocols: https://github.com/e-ucm/xasu/wiki/Authorization-protocols

### Working modes

Besides of being able to send traces to an LRS, Xasu is able to work locally, to make backups and even to work in a fallback mode when no connectivity is available.
Please visit our Wiki to get more details about the different working modes and how to configure them.

* Working modes: https://github.com/e-ucm/xasu/wiki/Working-Modes

### Cmi5

Xasu is compatible with the cmi5 xAPI profile and launch protocol. By enabling it, you will be able to launch your game or application from a cmi5 enabled LMS. With cmi5 the realization of activities is greatly simplified, and the results are captured in the LMS for later review.
Please visit our Wiki to get more details on using cmi5 in Xasu.

* Cmi5: https://github.com/e-ucm/xasu/wiki/Cmi5

## Adding Xasu to your game

Once Xasu is installed and configured, to add Xasu to your game you just have to create a new GameObject in Unity and include the Xasu component.

If you want to know more about how Xasu works, please check the Wiki:
* Working with Xasu: https://github.com/e-ucm/xasu/wiki/Working-with-Xasu

### Initializing Xasu

When Xasu is added to your scene it won't initialize and connect by default. 

To initialize it automatically, please check the "AutoStart" property in the object inspector.

![image](https://user-images.githubusercontent.com/3171485/173599958-9dbafc4b-81ba-466c-a58f-2efe7e0fa59f.png)

You can also initialize Xasu manually by using the ```Init``` method:
```cs
    await Xasu.Instance.Init();
```

If you want to learn more about how to initialize Xasu please visit our Wiki.
* Initializing Xasu: https://github.com/e-ucm/xasu/wiki/Working-with-Xasu#initialization

## Sending your first xAPI statement

Trace submission from Xasu is done using an asynchronous queue. This prevents the game from freezing and allows Xasu managing the trace submission in batches, reducing the connection load and handling the different errors. In addition, it is still possible for the game to know when a trace is submitted and if any (handled) error happened while sending the specific trace.

There are two possibilities when sending traces:
* Using the High-Level API: Xasu High-Level API simplifies the trace creation by using static templates from the Serious Game xAPI Profile and the CMI-5 Profile.

To use any of the APIs make sure you include the appropiate namespace in your .cs files.

```cs
using UnityTracker.HighLevel;
```

Here's an example of how can you send one trace using Xasu High-Level API:

```cs
    GameObjectTracker.Instance.Interacted("mesita"); // await is possible but not required
```
* Using the TinCan.NET API: Xasu uses the TinCan.NET library to model the traces. Thus, custom traces created using this API can be sent using Xasu too.

If you want to learn more about how to send statements using Xasu please visit our Wiki.
* Sending Statements using Xasu: https://github.com/e-ucm/xasu/wiki/Sending-xAPI-Traces

## Finalizing Xasu

Before the game is closed, Xasu has to be finalized manually so its processors (online, offline or backup) perform their final tasks.

```cs
    await Xasu.Instance.Finalize();
    Debug.Log("Tracker finalized, game is now ready to close...");
```

If you want to learn more about how to finalize Xasu please visit our Wiki.
* Finalizing Xasu: https://github.com/e-ucm/xasu/wiki/Working-with-Xasu#finalizing-xasu

# Important links

Referenced repositories:

* xAPI and LRS Specification: https://github.com/adlnet/xAPI-Spec
* TinCan.NET library: https://rusticisoftware.github.io/TinCan.NET/
* Polly.NET Resiliency library: https://github.com/App-vNext/Polly
* WixSharp.NET library for creating Windows Installers: https://github.com/oleg-shilo/wixsharp
