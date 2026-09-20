using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using HarmonyLib;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Characters;
using StardewValley.Menus;

namespace FarmAdmin
{
    public class ModConfig
    {
        public bool EnableAutoPauseOnDisconnect { get; set; } = false;
    }

    public class ModEntry : Mod
    {
        private static IMonitor? ModMonitor;
        private static List<string> Admins = new();
        private static string AdminsFilePath = "";

        public override void Entry(IModHelper helper)
        {
            ModMonitor = this.Monitor;
            AdminsFilePath = Path.Combine(this.Helper.DirectoryPath, "admins.json");
            LoadAdmins();

            ModConfig config = helper.ReadConfig<ModConfig>();

            var harmony = new Harmony(this.ModManifest.UniqueID);
            harmony.Patch(
                original: AccessTools.Method(typeof(ChatBox), nameof(ChatBox.receiveChatMessage)),
                postfix: new HarmonyMethod(typeof(ModEntry), nameof(OnChatMessageReceived))
            );

            if (config.EnableAutoPauseOnDisconnect)
            {
                helper.Events.Multiplayer.PeerDisconnected += OnPeerDisconnected;
            }

            this.Monitor.Log("FarmAdmin loaded successfully!", LogLevel.Info);
        }

        private static void LoadAdmins()
        {
            if (File.Exists(AdminsFilePath))
            {
                string json = File.ReadAllText(AdminsFilePath);
                Admins = JsonSerializer.Deserialize<List<string>>(json) ?? new List<string>();
            }
        }

        private static Farmer? FindOnlineFarmerByName(string name)
        {
            foreach (Farmer farmer in Game1.getOnlineFarmers())
            {
                if (farmer.Name.Equals(name, System.StringComparison.OrdinalIgnoreCase))
                    return farmer;
            }
            return null;
        }

        private static void OnChatMessageReceived(long sourceFarmer, int chatKind, LocalizedContentManager.LanguageCode language, string message)
        {
            if (!Game1.IsMasterGame || !message.StartsWith("!"))
                return;

            Farmer? sender = Game1.getFarmer(sourceFarmer);
            if (sender == null || !Admins.Contains(sender.Name))
                return;

            string[] parts = message.Substring(1).Split(' ');
            string command = parts[0].ToLower();

            switch (command)
            {
                case "buildpermission":
                    if (parts.Length > 1 && parts[1].ToLower() == "on")
                    {
                        Game1.player.team.farmhandsCanMoveBuildings.Value = FarmerTeam.RemoteBuildingPermissions.On;
                        Game1.chatBox.addInfoMessage("Farmhands can now move all buildings.");
                    }
                    else if (parts.Length > 1 && parts[1].ToLower() == "off")
                    {
                        Game1.player.team.farmhandsCanMoveBuildings.Value = FarmerTeam.RemoteBuildingPermissions.Off;
                        Game1.chatBox.addInfoMessage("Farmhand building permissions turned off.");
                    }
                    break;

                case "money":
                    if (parts.Length > 1 && int.TryParse(parts[1], out int amount))
                    {
                        Game1.player.team.money.Value = amount;
                        Game1.chatBox.addInfoMessage($"Shared farm wallet set to {amount}g.");
                    }
                    break;

                case "kick":
                    if (parts.Length > 1)
                    {
                        Farmer? target = FindOnlineFarmerByName(parts[1]);
                        if (target != null)
                        {
                            Game1.server?.kick(target.UniqueMultiplayerID);
                            Game1.chatBox.addInfoMessage($"Kicked {target.Name}.");
                        }
                        else
                        {
                            Game1.chatBox.addInfoMessage($"Couldn't find an online player named {parts[1]}.");
                        }
                    }
                    break;

                case "time":
                    if (parts.Length > 1 && int.TryParse(parts[1], out int newTime))
                    {
                        Game1.timeOfDay = newTime;
                        Game1.chatBox.addInfoMessage($"Time set to {newTime}.");
                    }
                    break;

                case "renamepet":
                    if (parts.Length > 1)
                    {
                        string currentPetName = Game1.player.getPetName();
                        Pet? pet = Game1.getCharacterFromName<Pet>(currentPetName, false);
                        if (pet != null)
                        {
                            pet.Name = parts[1];
                            pet.displayName = parts[1];
                            Game1.chatBox.addInfoMessage($"Pet renamed to {parts[1]}.");
                        }
                        else
                        {
                            Game1.chatBox.addInfoMessage("Couldn't find your pet.");
                        }
                    }
                    break;

                case "listadmin":
                    Game1.chatBox.addInfoMessage("Admins: " + string.Join(", ", Admins));
                    break;
            }
        }

        private static void OnPeerDisconnected(object? sender, PeerDisconnectedEventArgs e)
        {
            if (!Game1.IsMasterGame) return;

            int onlineFarmhands = 0;
            foreach (Farmer farmer in Game1.getOnlineFarmers())
            {
                if (farmer.UniqueMultiplayerID != Game1.player.UniqueMultiplayerID)
                    onlineFarmhands++;
            }

            if (onlineFarmhands == 0)
            {
                Game1.chatBox.receiveChatMessage(Game1.player.UniqueMultiplayerID, 0, LocalizedContentManager.LanguageCode.en, "!pause");
                ModMonitor?.Log("Last player disconnected — sent auto-pause.", LogLevel.Info);
            }
        }
    }
}