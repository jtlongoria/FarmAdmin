# FarmAdmin

A [SMAPI](https://smapi.io/) mod for Stardew Valley that gives designated farmhands host-level chat commands on a multiplayer/co-op farm — handy for an always-on/dedicated server where the actual host isn't always around to manage things.

## Commands

Type these in the in-game chat. Only players listed in `admins.json` can run them.

| Command | Example | What it does |
|---|---|---|
| `!buildpermission on/off` | `!buildpermission on` | Toggle whether farmhands can move/place buildings |
| `!money <amount>` | `!money 50000` | Set the shared farm wallet |
| `!kick <player name>` | `!kick Alex` | Kick a player from the session |
| `!time <military time>` | `!time 1800` | Set the in-game time |
| `!renamepet <name>` | `!renamepet Rex` | Rename the host's pet |
| `!listadmin` | `!listadmin` | List the current admins |

## Setup

1. Install [SMAPI](https://smapi.io/) and [Harmony](https://www.nuget.org/packages/Lib.Harmony) (pulled in automatically when building).
2. Build the project or drop the compiled mod into your `Stardew Valley/Mods` folder.
3. Edit `admins.json` in the mod folder and list the exact in-game farmer name(s) you want to grant admin commands to:
   ```json
   [
       "YourFarmerName",
       "FriendFarmerName"
   ]
   ```
4. (Optional) Edit `config.json` to enable `enableAutoPauseOnDisconnect`, which auto-pauses the game when the last connected farmhand disconnects.

## Built with

- C# / .NET 6
- [SMAPI](https://smapi.io/) (Stardew Modding API)
- [Harmony](https://harmony.pardeike.net/) for runtime patching

## Why

Built for a self-hosted, always-on Stardew Valley multiplayer server, so farmhands can manage the session themselves without needing the host online.
