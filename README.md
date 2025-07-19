# Afterburner
Just a little hobby project I decided to make public. Use at your own risk, and feel free to poke around or suggest improvements. No promises, and certainly no guarantees!

## What is this?
Afterburner is a CLI utility for modifying DCS .miz mission files to enable unlimited fuel. It works by patching the relevant option inside the mission archive.

## Why though?
Sometimes you want to go full **Afterburner** (*wink wink*) or not wanting to worry about fuel management. This tool essentially does the same thing if you were to open up the in-game mission editor and tick the "Unlimited Fuel" box. 
While this is quite easy to do once, if you play a dynamic single-player campaigns like DCS Liberation or DCS Retribution, you're dealing with a newly generated `.miz` every turn, 
and that simple act becomes a hassle now, and this tool aims to fix that.

## How to Use
### Prerequisites
- .NET 9.0 SDK or later
- Windows (tested on win-x64)
- DCS World dedicated server (or single-player) with .miz mission files you’d like to patch
  
### Usage
After building (see below), you’ll have an executable you can run from the command line.

Basic Command:

```sh
Afterburner.exe unlimited-fuel -p "<path to your .miz file>"
```

There's also a shorthand `uf` command that does basically the same thing:
```sh
Afterburner.exe uf -p "<path to your .miz file>"
```

If you omit the -p/--path option, it defaults to:
```
%USERPROFILE%\Saved Games\DCS.dcs_serverrelease\Missions\liberation_nextturn.miz
```
The tool will patch the mission file in-place to enable unlimited fuel.

## How to Build
1. Clone this repository:
```sh
git clone https://github.com/razyoboy/Afterburner.git
cd Afterburner
```

2. Build & publish:
There’s a handy PowerShell script:
> ⚠️ **Warning:** Note that this is a very niche build script as this would copy the built binary to `C:/tools` as this is where I personally store all my built binaries, if you are not comfortable with this, there is a manual build command below 😊
```sh
./build.ps1
```
This will:

- Build the project (Release, win-x64, .NET 9.0)
- Output a single-file executable to `Afterburner/publish`
- Optionally copy it to `C:\tools` if that folder exists
  
Or, build manually:

```sh
dotnet publish Afterburner/Afterburner.csproj --configuration Release --runtime win-x64 --framework net9.0 --self-contained false /p:PublishSingleFile=true --output Afterburner/publish
```
3. Find your executable:
The published executable will be in `Afterburner/publish/`

## Notes
- This is not an official tool, just something I hacked together for fun.
- It directly modifies your `.miz` files — always keep backups!
- If you run into issues, check the code or open an issue, or even feel free to fork it and modify them yourself 😊
