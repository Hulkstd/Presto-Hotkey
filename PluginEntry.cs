using Presto.Common;
using Presto.Common.Services;
using Presto.SDK;
using System.Linq;
using Presto.Hotkey.Input;
using Presto.Hotkey.Dialogs;
using System.IO;
using System.Reflection;
using System;

[assembly: PrestoTitle("단축키")]
[assembly: PrestoAuthor("HulkSTD")]
[assembly: PrestoDescription("사용중 풀편했던 단축키 기능울 추가해 줍니다.")]

namespace Presto.Hotkey
{
    public class PluginEntry : PrestoPlugin
    {
        #region 변수
        private HotkeySetting _hotkeySetting;
        #endregion

        #region 속성
        private IPlaylistService Playlist => PrestoSDK.PrestoService.Playlist;
        #endregion

        #region 내부 함수
        private void OpenDialog()
        {
            if (_hotkeySetting != null)
            {
                _hotkeySetting.Activate();
            }
            else
            {
                _hotkeySetting = new HotkeySetting();
                _hotkeySetting.Closed += (s, e) =>
                {
                    _hotkeySetting = null;
                };

                _hotkeySetting.Show();
            }
        }
        #endregion

        public override void OnLoad()
        {
            PluginData.basePath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\Presto\\Plugins\\Presto-Hotkey";

            if(PluginData.GlobalInputEvent == null)
            {
                PluginData.GlobalInputEvent = new GlobalInput();
            }
            if(PluginData.jsonSerializer == null)
            {
                PluginData.jsonSerializer = new Newtonsoft.Json.JsonSerializer();
                PluginData.jsonSerializer.NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore;
                PluginData.jsonSerializer.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
            }
        }

        public override void OnUnload()
        {
            _hotkeySetting?.Close();
            _hotkeySetting = null;
            PluginData.GlobalInputEvent.thread.Abort();
        }

        [PrestoMenu(PrestoKey.MenuLibrary, "단축키 설정")]
        private void SettingKeys()
        {
            OpenDialog();
        }

        [PrestoMenu(PrestoKey.MenuPlaylistContent, "단축키 설정")]
        private void SettingKeysPlaylist()
        {
            OpenDialog();
        }


    }
}
