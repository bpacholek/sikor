Sikor, a Jira-compatible Timetracker
==================================

Sikor is a tool which simplifies the work with timetracking in Jira. Allows to actively find, select and track using live timer the time spent on particular issues. Allows also changing of statuses.

# Why Sikor?

There are not many tools in the Internet which are complete even in a level delivered by the development version of Sikor _(a bit of autopromotion!)_. Apart from a usable search engine the most important feature is **offline operation** which may be especially useful whenever working behind often disconnecting **VPN networks**.

During each **online** search Sikor caches the issues which allows basic search usage during **offline** mode and saving of worklogs to later, when your Jira connection becomes available again.

Sikor is highly platform-independent thanks to the use of **.NET Core 3.1** and cross-platform UI **AvaloniaUI**. Supports Windows, OSX and Debian/Ubuntu-based GNU/Linux operating systems.

# Requirements

At the moment the default builds are delivered only for x64 platforms. Each release (as found in the Releases section) provides versions for OSX, Windows and Debian/Ubuntu GNU/Linux OSes. In order to run the application you do not need to have **.NET Core** in your system as it is bundled in a portable form together with the executable.

# How to run the app?

* On Windows it is simple as possible: just run the .exe file. In case your system informs you about potential problems due to unknown vendor please ignore such warning. Possibly in the future I shall provide the app via Windows Store, yet especially during the development phase such approach is not considered.

* On OSX and Debian/Ubuntu you first need to mark the file as executable: using the properties menu (right click) or command-line interface: `chmod +x [filename]`. In case of any security warnings please ignore them.

Application requires access to your user profile's directory: it creates there a folder called `.sikor` which stores projects' data and `lastrun.log` file (which at the moment holds handled exceptions during the last run).

# How to build the app manually?

In case you do not trust me and the executables which I deliver (although they are built on GitHub using GitHub actions) you can easily build the application yourself. In order to do so you will first need to install **.NET Core 3.1** development tools in your system. For instructions please go to the website provided by [Microsoft (click here)](https://dotnet.microsoft.com/download). Later pull or download the code, go to the folder which holds the code and execute:

### Windows x64
```
dotnet publish Sikor.csproj --configuration Windows --framework netcoreapp3.1 --self-contained true --runtime win-x64 /p:PublishSingleFile=true --output out
```

### OSX x64
```
dotnet publish Sikor.csproj --configuration OSX --framework netcoreapp3.1 --self-contained true --runtime osx-x64 /p:PublishSingleFile=true --output out
```

### GNU/Linux x64
```
dotnet publish Sikor.csproj --configuration Linux --framework netcoreapp3.1 --self-contained true --runtime linux-x64 /p:PublishSingleFile=true --output out
```

The executable file will be created in the `out` directory.

# How to join development?

First of all set up your IDE: the project is highly and out-of-the-box compatible with **Visual Studio 2019** and **Visual Studio Code** with the **C# extension**. Using one of those IDEs you should be able to load and build the project without any issues.

### How can I help?

The project is currently developed in a Rapid-Application-Development mode, which means that no real planning is going on in the background: features are added and code is adapted as needed. I try to add `<todo>` and `//TODO` comments in places where I already know about potential problems. Please check the roadmap below about general plans for this project.

Whenever you experience a problem please file an **Issue** using GitHub's interface, when you have a code improvement then please create a **Pull Request** from a branch which has a descriptive name (never try to merge from `master` to `master`).

### Two words on the structure

I have tried to mimic the structure of popular web-frameworks as much as possible to allow easier development for developers coming from that background. Shared elements are stored under `Services` namespace while models of temporary entities are in `Models`. `Program.cs` acts as a boostrap.

### Known issues

At the moment there are two major problems: first one is still the level of reactivity: for example changing of a status will not automatically change its value in the `Selected issue` preview box. Also the ViewModels are still to complex: more logic should be removed from them into `AppState` which is a bridge between the view and the services.

One major issue which may grow in the future and which probably should be changed ASAP is the way Profiles are stored: in a single storage file while in fact each should be a separate element to reduce potential memory issues.

### Where are the tests?

Project was never meant to be developed using any TDD approaches as the idea was to deliver a simple tool ASAP. If you check the commits between 0.1 and 0.2 you shall notice that most of the code was actually changed, therefore the tests are planned for version around 0.7.

### Versions

Durining the development (until version 1.0) **SemVer** approach is not meant to be expected, therefore please consider project files created by the current versions potentially to be incompatible with the next releases.

Roadmap
=======

# 0.2.1
* Search-race fix: currently each search triggers a new thread and potentially they
can race each other. Fix should involve cancellation of a previously created thread.

# 0.3
* Further cleanup of ViewModels in favor of AppState
* More comments in existing classes
* Automatic detection of a new version
* Option to filter by Types

# 0.4
* "PM Token" feature which allows sending notifications to a project manager using a different channel (Slack, Mattermost etc.)
* Profile edit screen
* Worklog edit screen

# 0.5
* Improved UI Reactivity: update of statuses on preview screens during changes

# 0.6
* "My day" view which displays worklog of the current day and allows minor modifications

# 0.7
* Feature-freeze and Unit Tests development

# 0.8
* Release Candidate

Disclaimer
==========

This application is delivered **without any warranty** and is expected to be treated as a unfinished product therefore authors are not responsible for any harm caused by using it. **You use it at your own risk**.


