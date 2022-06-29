using Oxide.Core;
using Oxide.Core.Configuration;
using Oxide.Core.Plugins;

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
            }
            catch
            {
                Puts("We failed to intialize!");
                Unsubscribe("OnPlayerConnected");
            }
            return;
        }

        void OnPlayerConnected(BasePlayer player)
        {
            string playerId = player.IPlayer.Id;
            // jukebox: Check if the player already has data - if not, we get them some.    
            if (playerDataFile[playerId] == null)
            {
               playerDataFile[playerId, "Skills", "Strength"] = 0.0; 
            }
            Puts("!!!!!!!!!!!!!!!!! PLAYER STRENGTH:" + playerDataFile[playerId, "Skills", "Strength"]);
            playerDataFile.Save();
        }

        [Libraries.Commands.Command("checkstats")]
        private void CheckStats(Libraries.Covalence.IPlayer player, string command, string[] args)
        {
            string playerId = Libraries.Covalence.IPlayer.Id;
            Libraries.Covalence.IPlayer.Reply(playerDataFile[playerId, "Skills", "Strength"]);
        }
    }
//    private class PluginConfig
//    {
//    }
}