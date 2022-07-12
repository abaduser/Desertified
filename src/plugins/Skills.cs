using Oxide.Core;
using Oxide.Core.Configuration;
using Oxide.Core.Plugins;
using System.Collections.Generic;
using Oxide.Core.Libraries.Covalence;
using System;

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
                Subscribe("CanCreateWorldProjectile");
                Subscribe("OnMeleeAttack");
                Subscribe("OnMeleeThrown");
            }
            catch
            {
                Puts("We failed to intialize!");
                Unsubscribe("OnPlayerConnected");
                Unsubscribe("OnWeaponFired");
                Unsubscribe("CanCreateWorldProjectile");
                Unsubscribe("OnMeleeAttack");
                Unsubscribe("OnMeleeThrown");
            }
            return;
        }

        void CanCreateWorldProjectile(HitInfo info,
            ItemDefinition itemDef,
            ItemModProjectile mod,
            Projectile projectile,
            Item item)
        {
            //Puts("Fuck");
            //projectile.initialVelocity *= 0.1f;
        }

        void OnWeaponFired(BaseProjectile projectile, BasePlayer player, ItemModProjectile mod, ProtoBuf.ProjectileShoot projectiles)
        {
            Puts("OnWeaponFired works!");
            mod.numProjectiles = 500;
        }

        void OnMeleeThrown(ref BasePlayer player, Item item)
        {
            // yx: item.info is the Item's ItemDefinition
            string playerId = player.IPlayer.Id;
            string playerName = player.IPlayer.Id;
            float playerMuscles = System.Convert.ToSingle(playerDataFile[playerId, "skills", "muscles"]);

            foreach (KeyValuePair<int, BasePlayer.FiredProjectile> proj in player.firedProjectiles)
            {
                if (proj.Value.weaponPrefab != null) { Puts("WE GOT THE POWER"); }
                else { return; }
                UnityEngine.GameObject rockInPotentia = GameManager.server.FindPrefab(proj.Value.weaponPrefab) ;
                if (rockInPotentia != null)
                {
                    Puts("Rock achieved its full potential!");
                    rockInPotentia.SetVelocity(UnityEngine.Vector3(0.0f, 0.0f, 0.0f));
                }
                Puts("\tWe didn't find the speedrock you were looking for :<");
                return;
            }
        }

        void OnMeleeAttack(BasePlayer player, HitInfo info)
        {
            string playerId = player.IPlayer.Id;
            string playerName = player.IPlayer.Name;
            float playerMuscles = System.Convert.ToSingle(playerDataFile[playerId, "skills", "muscles"]);

            if (!info.DidHit) { return; }
            //Puts(playerName + " melee attacked " + info.HitEntity + " using " + info.Weapon);
            // jukebox: Adjusting the gather scale based on the muscles skill
            info.gatherScale = info.gatherScale * playerMuscles;
            // yx: Adjust the Melee Hit Damage
            DamageProperties.HitAreaProperty[] sandBones = new DamageProperties.HitAreaProperty[7];
            for (int i=0;i<7;i++)
            {
                // yx: Create a new HitAreaProperty for each element, to replace default damage
                DamageProperties.HitAreaProperty newBone = new DamageProperties.HitAreaProperty();
                newBone.area = info.damageProperties.fallback.bones[i].area;
                newBone.damage = info.damageProperties.fallback.bones[i].damage * (0.25f + playerMuscles);
                sandBones[i] = newBone;
            }
            info.damageProperties.bones = sandBones;
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
                playerDataFile[playerId, "skills", "muscles"] = 0.50f;
                playerDataFile[playerId, "skills", "guts"] = 0.50f;
                playerDataFile[playerId, "skills", "tendons"] = 0.50f;
                // jukebox: Spine is acting as an overall boost, so it should be lower.
                playerDataFile[playerId, "skills", "spine"] = 0.25f;
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
                playerDataFile[playerId, "skills", "muscles"] = float.Parse(args[1]);
            }
            if (skill == "guts" || skill == "g")
            {
                playerDataFile[playerId, "skills", "guts"] = float.Parse(args[1]);
            }
            if (skill == "tendons" || skill == "t")
            {
                playerDataFile[playerId, "skills", "tendons"] = float.Parse(args[1]);
            }
            if (skill == "spine" || skill == "s")
            {
                playerDataFile[playerId, "skills", "spine"] = float.Parse(args[1]);
            }
        }
    }


//    private class PluginConfig
//    {
//    }
}