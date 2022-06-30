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
            foreach (ProtoBuf.ProjectileShoot.Projectile proj in projectiles.projectiles)
            {
                proj.startVel = proj.startVel * 0.05f;
                foreach (KeyValuePair<int, BasePlayer.FiredProjectile> kvp in player.firedProjectiles)
                {
                    player.firedProjectiles[kvp.Key].initialVelocity = proj.startVel;
                }
            }
        }

        void OnMeleeThrown(BasePlayer player, Item item)
        {
            // yx: item.info is the Item's ItemDefinition
            Puts("OnMeleeThrown works!");
            string playerId = player.IPlayer.Id;
            float playerStrength = System.Convert.ToSingle(playerDataFile[playerId, "Skills", "Strength"]);
        }

        void OnMeleeAttack(BasePlayer player, HitInfo info)
        {
            Puts(player.IPlayer.Name + " melee attacked " + info.HitEntity + " using " + info.Weapon);
            info.gatherScale = info.gatherScale*2;
            Puts("Gather Scale: " + info.gatherScale);
            Puts("Did Gather?: " + info.DidGather);
            Puts("Can Gather?: " + info.CanGather);
        }

        void OnPlayerConnected(BasePlayer player)
        {
            string playerId = player.IPlayer.Id;
            // jukebox: Check if the player already has data - if not, we get them some.    
            if (playerDataFile[playerId] == null)
            {
               // jukebox: Players start with 0.50 (50%) in their strength stat
               playerDataFile[playerId, "Skills", "Strength"] = 0.50;
            }
            Puts("!!!!!!!!!!!!!!!!! PLAYER STRENGTH:" + playerDataFile[playerId, "Skills", "Strength"]);
            playerDataFile.Save();
        }

        [ChatCommand("checkstats")]
        private void CheckStats(BasePlayer player, string command, string[] args)
        {
            string playerId = player.IPlayer.Id;
            // jukebox: Creating a key collection so we can iterate over all the skills
            foreach( KeyValuePair<string, object> kvp in (Dictionary<string, object>)playerDataFile[playerId, "Skills"])
            {
                SendReply(player, "Your " + kvp.Key + " is " + kvp.Value);
            }
        }

        [ChatCommand("setstat")]
        private void SetStats(BasePlayer player, string command, string[] args)
        {
            string playerId = player.IPlayer.Id;
            string stat = args[0];
            if (stat == "strength" || stat == "str" || stat == "Strength")
            {
                playerDataFile[playerId, "Skills", "Strength"] = double.Parse(args[1]);
            }
        }


    }
//    private class PluginConfig
//    {
//    }
}