using Oxide.Core;
using Oxide.Core.Configuration;
using Oxide.Core.Plugins;
using System.Collections.Generic;
using Oxide.Core.Libraries.Covalence;

namespace Oxide.Plugins
{
    [Info("Skills", "jukebox, yx", "0.0.1")]
    [Description("Adds skills and skill training to Rust.")]
    public class Skills : RustPlugin
    {
        // jukebox: Either creates or loads an existing playerdata file.
        public DynamicConfigFile playerDataFile;

        void Init()
        {
            try
            {
                Puts("Initializing...");
                playerDataFile = Interface.Oxide.DataFileSystem.GetDatafile("desertified_playerdata");
                Subscribe("OnPlayerConnected");
                Subscribe("OnWeaponFired");
                Subscribe("OnMeleeAttack");
                Subscribe("OnMeleeThrown");
            }
            catch
            {
                Puts("We failed to intialize!");
                Unsubscribe("OnPlayerConnected");
                Unsubscribe("OnWeaponFired");
                Unsubscribe("OnMeleeAttack");
                Unsubscribe("OnMeleeThrown");
            }
            return;
        }

        void OnWeaponFired(BaseProjectile projectile, BasePlayer player, ItemModProjectile mod, ProtoBuf.ProjectileShoot projectiles)
        {
            Puts("OnWeaponFired works!");

            mod.projectileVelocity *= 0.1f;

            // Changing the Projectile from the ProjectileShoot protobuffer to have less startVel
            /*
            foreach (ProtoBuf.ProjectileShoot.Projectile proj in projectiles.projectiles)
            {
                proj.startVel = proj.startVel * 0.05f;
                // jukebox: Trying DESPERATELY to sync the server's understanding of a projectile to the client.
                // Note that this could be completely impossible and despair.
            }
            */

        }

        void OnMeleeThrown(BasePlayer player, Item item)
        {
            // yx: item.info is the Item's ItemDefinition
            Puts("OnMeleeThrown works!");
            string playerId = player.IPlayer.Id;
            string playerName = player.IPlayer.Id;
            float playerStrength = System.Convert.ToSingle(playerDataFile[playerId, "skills", "muscles"]);

            foreach (KeyValuePair<int, BasePlayer.FiredProjectile> kvp in player.firedProjectiles)
            {
                Puts(kvp.Value.itemDef.shortname);
                // jukebox: grab FiredProjectile, boot it, and make our own
                
            }
        }

        void OnMeleeAttack(BasePlayer player, HitInfo info)
        {
            string playerId = player.IPlayer.Id;
            string playerName = player.IPlayer.Name;
            
            Puts(playerName + " melee attacked " + info.HitEntity + " using " + info.Weapon);
            // jukebox: Adjusting the gather scale based on the muscles skill
            float playerMuscles = System.Convert.ToSingle(playerDataFile[playerId, "skills", "muscles"]);
            info.gatherScale = info.gatherScale * playerMuscles;
        }

        void OnPlayerConnected(BasePlayer player)
        {
            string playerId = player.IPlayer.Id;
            string playerName = player.IPlayer.Name;

            // jukebox: Check if the player already has data - if not, we get them some.    
            if (playerDataFile[playerId] == null)
            {
                Puts("Creating fresh skills for " + playerName);
                // jukebox: Each skill has a double that acts as a percentage of a normal
                // rust character's ability.
                playerDataFile[playerId, "skills", "muscles"] = 0.50;
                playerDataFile[playerId, "skills", "guts"] = 0.50;
                playerDataFile[playerId, "skills", "tendons"] = 0.50;
                // jukebox: Spine is acting as an overall boost, so it should be lower.
                playerDataFile[playerId, "skills", "spine"] = 0.25;
            }

            Puts("Retrieved " + playerName + "'s skills:");
            Puts("\tMUSCLES: " + playerDataFile[playerId, "skills", "muscles"]);
            Puts("\tGUTS: " + playerDataFile[playerId, "skills", "guts"]);
            Puts("\tTENDONS: " + playerDataFile[playerId, "skills", "tendons"]);
            Puts("\tSPINE: " + playerDataFile[playerId, "skills", "spine"]);
            playerDataFile.Save();
        }

        // Convenience command for checking skills in game.
        [ChatCommand("checkskills")]
        private void CheckStats(BasePlayer player, string command, string[] args)
        {
            string playerId = player.IPlayer.Id;
            // jukebox: Creating a key collection so we can iterate over all the skills
            foreach( KeyValuePair<string, object> kvp in (Dictionary<string, object>)playerDataFile[playerId, "skills"])
            {
                SendReply(player, "Your " + kvp.Key + " is " + kvp.Value);
            }
        }

        // Convenience command for setting skills in game.
        [ChatCommand("setskill")]
        private void SetStats(BasePlayer player, string command, string[] args)
        {
            string playerId = player.IPlayer.Id;
            string skill = args[0];
            if (skill == "muscles" || skill == "m")
            {
                playerDataFile[playerId, "skills", "muscles"] = double.Parse(args[1]);
            }
            if (skill == "guts" || skill == "g")
            {
                playerDataFile[playerId, "skills", "guts"] = double.Parse(args[1]);
            }
            if (skill == "tendons" || skill == "t")
            {
                playerDataFile[playerId, "skills", "tendons"] = double.Parse(args[1]);
            }
            if (skill == "spine" || skill == "s")
            {
                playerDataFile[playerId, "skills", "spine"] = double.Parse(args[1]);
            }
        }
    }
//    private class PluginConfig
//    {
//    }
}