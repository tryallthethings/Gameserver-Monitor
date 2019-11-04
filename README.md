# Gameserver-Monitor [![Build Status](https://travis-ci.org/tryallthethings/Gameserver-Monitor.svg?branch=master)](https://travis-ci.org/tryallthethings/Gameserver-Monitor)
A simple command line application to check the status of one or multiple game servers. Supports single run, continous check and [PRTG XML export](https://www.paessler.com/prtg)
As it uses the [Gameserverinfo](https://github.com/tryallthethings/gameserverinfo) library to check the various game servers only those are currently supported by this application.

## Requirements
- Requires at least Microsoft .NET Framework 4.0 to run
## Installation
- Download latest version from [Releases](https://github.com/tryallthethings/Gameserver-Monitor/releases)
- Extract gsmon.exe from the .zip to any folder you'd like to run it from
- Profit :smiley:
## How to use
The application accepts parameters in various formats. The formats can even be mixed. Choose your preferred style:

- gsmon.exe -s servername
- gsmon.exe --server servername
- gsmon.exe /s servername
- gsmon.exe --server servername -p 12345 /t Type

These are all the supported paramters:
```
Usage: gsmon.exe [OPTIONS]
Example: gsmon.exe -s servername.com -p 12345 -t Quake3
Version 1.0.0.0 - Visit https://github.com/tryallthethings/Gameserver-Monitor for support

Options:
  -s, --server=VALUE         the server name (FQDN) or IP address.
  -t, --type=VALUE           the server type (protocol).
  -p, --port=VALUE           the server port.
  -o, --output=VALUE         output formatting.
  -w, --wait=VALUE           wait time between checks - only for autorefresh.
  -v                         increase debug message verbosity
  -h, --help                 show this message and exit
```

## Examples

### Check a single server status and output it immediately
> gsmon.exe --server example.com --port 27015 --type Quake3
```
Server: example.com (My great CoD4 server) is online: Scan time 22ms and 0/20 players are connected. Map: mp_strike
```
### Check multiple servers and output immediately
> gsmon.exe --server example.com --port 27015 --type Quake3 --server example2.com --port 27016 --type Quake3
```
Server: example.com (My great CoD4 server) is online: Scan time 22ms and 0/20 players are connected. Map: mp_strike
Server: example.com (My even greater CoD4 server) is online: Scan time 22ms and 0/12 players are connected. Map: mp_crash
```
### Checking multiple servers continously (default refresh is every 10 seconds)
> gsmon.exe --server example.com --port 27015 --type Quake3 --server example2.com --port 27016 --type Quake3 --output autorefresh
```
Automatic refresh is enabled. Press CTRL+C to cancel
Checking 1 server(s) every 10 seconds...
Last update: 07.01.2018 04:05:02

Server: example.com (My great CoD4 server) is online: Scan time 22ms and 0/20 players are connected. Map: mp_strike
Server: example.com (My even greater CoD4 server) is online: Scan time 22ms and 0/12 players are connected. Map: mp_crash
```
The option **-w** can be used to increase or decrease the refresh time (in seconds).
### Checking a single server and return the result as an PRTG compatible XML
> gsmon.exe --server example.com --port 27015 --type Quake3 --output xml
```
<prtg><result><channel>Server scan time</channel><value>30</value><unit>TimeResponse</unit></result><result><channel>Players connected</channel><value>0</value><unit>Count</unit><LimitMaxWarning>19</LimitMaxWarning></result></prtg>
```



## Supported game server types
- **Samp**
  - SAMP
- **Ase** (All seeing eye)
  - Multi-Theft Auto
  - Multi-Theft Auto: Vice City
  - Age of Empires 2
  - Age of Empires 2: The Conquerors
  - Soldat
  - Devastation
  - Universal Combat
  - Xpand Rally
  - Far Cry
  - Medal of Honor: Pacific Assault
  - Chaser
  - Chrome
- **Quake3** (Quake 3 Engine)
  - Quake 3
  - Call of Duty
  - Call of Duty 4
  - Star Wars Jedi Knight: Jedi Academy
  - Call of Duty: United Offensive
  - Star Trek: Voyager - Elite Force
  - Star Trek: Voyager - Elite Force 2
  - Soldier of Fortune 2
  - Return to Castle Wolfenstein
  - Wolfenstein: Enemy Territory
  - Star Wars Jedi Knight 2: Jedi Outcast
  - Soldier of Fortune
  - Daikatana
- **HalfLife** (Half-Life 1 Engine)
  - Half-Life
  - Counter-Strike
  - Counter-Strike v48 Protocol
  - Counter-Strike: Condition Zero
  - Day of Defeat
  - Gunman Chronicles
- **Doom3**
  - Doom 3
- **GameSpy** (GameSpy)
  - Unreal Tournament 2003
  - Unreal Tournament 2004
  - Unreal 2 XMP
  - Medal of Honor: Allied Assault
  - Medal of Honor: Breakthrough
  - Medal of Honor: Spearhead
  - GameSpy (Generic Protocol)
  - Unreal Tournament
  - Descent 3
  - Postal 2
  - Deus Ex
  - IL-2 Sturmovik
  - IL-2 Sturmovik: Forgotten Battles
  - Heretic 2
  - IGI-2: Covert Strike
  - Gore
  - Vietcong
  - Serious Sam
  - Serious Sam: The Second Encounter
  - Aliens vs. Predator 2
  - No One Lives Forever
  - No One Lives Forever 2
  - Shogo
  - Codename: Eagle
  - Giants: Citizen Kabuto
  - Global Operations
  - Nerf ArenaBlast
  - RalliSport Challenge
  - Rally Masters
  - Command and Conquer: Renegade
  - Rune
  - Sin
  - Tactical Ops
  - Unreal
  - Wheel of Time
  - Deadly Dozen: Pacific Theater
  - Dirt Track Racing 2
  - Drakan: Order of the Flame
  - F1 2002
  - Iron Storm
  - James Bond: Nightfire
  - Kingpin
  - Need for Speed: Hot Pursuit 2
  - Redline
  - Turok 2
  - Tron 2.0
  - Tony Hawk's Pro Skater 3
  - Tony Hawk's Pro Skater 4
  - V8 Supercar Challenge
  - Team Factor
  - Rainbow Six
  - Rainbow Six: Rogue Spear
  - Nitro Family
  - Rise of Nations
  - Contract J.A.C.K.
  - Homeworld 2
  - Breed
  - Operation Flashpoint: Resistance
  - Star Trek: Bridge Commander
  - Tribes: Vengeance
  - Tony Hawk's Underground 2
- **GameSpy2** (Gamespy version 2)
  - Star Wars: Battlefront
  - Battlefield Vietnam
  - Painkiller
  - Halo: Combat Evolved
  - America's Army
  - Neverwinter Nights
  - Operation Flashpoint
- **Source** (Source Engine)
  - Source Engine (Generic Protocol)
  - Half-Life 2
  - Team Fortress 2
  - Counter-Strike: Source
  - Left 4 Dead
  - Left 4 Dead 2
