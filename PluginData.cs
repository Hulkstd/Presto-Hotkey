using Newtonsoft.Json;
using Presto.Common.Models;
using Presto.Hotkey.Input;

namespace Presto.Hotkey
{
    static class PluginData
    {
        public static IPlaylist CurrentPlaylist { get; set; }
        public static GlobalInput GlobalInputEvent { get; set; }
        public static string basePath { get; set; }

        public static JsonSerializer jsonSerializer { get; set; }
    }
}
