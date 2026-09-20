# FarmAdmin

[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](LICENSE)

A [SMAPI](https://smapi.io/) mod for Stardew Valley that gives designated farmhands **host-level chat commands** — built for co-op farms running on an **always-on/dedicated server**, where the actual host isn't always the one logged in to handle things like kicking a disconnected player, adjusting the shared wallet, or renaming the pet.

If you're hosting Stardew Valley on a cloud VM or dedicated box — for example with [puppy-stardew-server](https://github.com/AmigaMeow/puppy-stardew-server), a Dockerized Stardew Valley server with an always-on/always-online setup — so friends can drop in and out on their own schedule, FarmAdmin lets any trusted farmhand run admin actions from chat — no need to wait on the host.

## Commands

Type these in the in-game chat. Only players listed in `admins.json` can run them.

| Command | Example | What it does |
|---|---|---|
| `!buildpermission on/off` | `!buildpermission on` | Toggle whether farmhands can move/place buildings |
| `!money <amount>` | `!money 50000` | Set the shared farm wallet to a specific amount |
| `!kick <player name>` | `!kick Alex` | Kick a player from the current session |
| `!time <military time>` | `!time 1800` | Set the in-game clock (e.g. `1800` = 6:00 PM) |
| `!renamepet <name>` | `!renamepet Rex` | **Rename the host's pet** (dog or cat) without opening the naming menu |
| `!listadmin` | `!listadmin` | List everyone currently granted admin commands |

**Rename your pet from chat:** Stardew doesn't normally let you rename your dog or cat after adoption without console/save-editing tricks. `!renamepet` does it instantly, in-game, no editing required — this is one of the most requested features for multiplayer farms.

## Built for always-on servers

FarmAdmin also ships with an optional auto-pause feature for dedicated/always-on hosts: when the last connected farmhand disconnects, it automatically sends `!pause` so the farm doesn't sit running (and burning in-game time) with nobody on it. It's off by default — turn it on in `config.json`:

```json
{
    "enableAutoPauseOnDisconnect": true
}
```

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
   Replace both entries with real in-game farmer names — these are placeholders.
4. (Optional) Enable `enableAutoPauseOnDisconnect` in `config.json` if you're running an always-on server (see above).

## Built with

- C# / .NET 6
- [SMAPI](https://smapi.io/) (Stardew Modding API)
- [Harmony](https://harmony.pardeike.net/) for runtime patching

## License

MIT — see [LICENSE](LICENSE). Free to use, modify, and share.
