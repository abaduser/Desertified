namespace Oxide.Plugins
{
    [Info("Skills", "jukebox, yx", "0.0.1")]
    [Description("Adds skills and skill training to Rust.")]
    public class Skills : RustPlugin
    {
        // jukebox: Either creates or loads an existing playerdata file.
        public DynamicConfigFile playerDataFile = Interface.Oxide.DataFileSystem.GetDatafile("desertified_playerdata.json");
        private struct SkillData {
            public double Strength = 0.0;
        }

        void Init()
        {
            try
            {
                Puts("Hello world!");
                Subscribe("OnPlayerConnected");
            }
            catch
            {
                LogWarning("We failed to initialize!");
                Unsubscribe("OnPlayerConnected");
            }
            return;
        }

        void OnPlayerConnected(BasePlayer player)
        {
            string playerId = player.IPlayer.Id;
            // jukebox: Check if the player already has data - if not, we get them some.    
            if (playerDataFile[playerId] == null || playerDataFile[playerId, "Skills"] == null)
            {
                playerDataFile[playerId, "Skills"] = SkillData();
            }
            SkillData playerSkills = playerDataFile[playerId, "Skills"];
            Puts("!!!!!!!!!!!!!!!!! PLAYER STRENGTH:" + playerSkills.Strength);
            playerDataFile.Save();
        }
    }
//    private class PluginConfig
//    {
//    }
}