using Oxide.Core;
using Oxide.Core.Configuration;
using Oxide.Core.Plugins;
using System.Collections.Generic;


namespace Oxide.Plugins
{
    [Info("desertworld", "jukebox, yx", "0.0.1")]
    [Description("Changes the Map to feel more like a desert.")]
    public class desertworld : RustPlugin
    {
        void Init()
        {
            Puts("Loaded Desert World!");
        }
        object OnRunPlayerMetabolism(PlayerMetabolism metabolism, BasePlayer player, float delta)
        {
            Puts("OnRunPlayerMetabolism works!");
            // yx : changing the min and max - Not a good solution, just keeps going up.
            // metabolism.temperature.min += 10.0f;
            // metabolism.temperature.max += 10.0f;
            // Doesn't change much
            //metabolism.temperature.startMin += 10.0f;
            //metabolism.temperature.startMax += 10.0f;
            Puts(metabolism.temperature.lastValue.ToString());
            Puts(metabolism.temperature.value.ToString());
            return null;
        }
    }
}