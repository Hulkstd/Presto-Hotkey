using Presto.Component.Controls;
using Presto.SDK;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Input;
using Presto.Input;
using System.IO;
using System.Runtime.Serialization;
using System;
using Newtonsoft.Json;

namespace Presto.Hotkey.Dialogs
{
    [Serializable]
    public class KeySetList : SortedSet<string>
    {
        public string this[int index]
        {
            get
            {
                int i = 0;

                foreach(string str in this)
                {
                    if(i == index)
                    {
                        return str;
                    }
                    else
                    {
                        i++;
                    }
                }

                return "";
            }
        }

        public KeySetList(IComparer<string> comparer) : base(comparer)
        {
            
        }

        [JsonConstructor]
        public KeySetList(List<string> List) : base(new HotkeySetting.StringCompare())
        {
            foreach(string str in List)
            {
                Add(str);
            }
        }
    }
    [Serializable]
    public class KeySet
    {
        public KeySetList List;

        public KeySet(IComparer<string> comparer)
        {
            List = new KeySetList(comparer);
        }

        [JsonConstructor]
        public KeySet(KeySetList List)
        {
            this.List = List;
        }
    }

    /// <summary>
    /// HotkeySetting.xaml에 대한 상호 작용 논리
    /// </summary>
    [System.Serializable]
    public partial class HotkeySetting : PrestoWindow
    {
        private GlobalInput instance;

        KeySet PlaybackStateKeys;
        KeySet VolumeDownKeys;
        KeySet VolumeUpKeys;
        KeySet PreviousSongKeys;
        KeySet NextSongKeys;

        string PlaybackStateKeysText = "";
        string VolumeDownKeysText = "";
        string VolumeUpKeysText = "";
        string PreviousSongKeysText = "";
        string NextSongKeysText = "";

        string basePath = PluginData.basePath;

        public HotkeySetting()
        {
            InitializeComponent();

            if (instance == null) instance = PluginData.GlobalInputEvent;

            //PlaybackStateKeys = new KeySet(new StringCompare());
            //VolumeUpKeys = new KeySet(new StringCompare());
            //VolumeDownKeys = new KeySet(new StringCompare());
            //PreviousSongKeys = new KeySet(new StringCompare());
            //NextSongKeys = new KeySet(new StringCompare());
            
            #region PlaybackStateKeys

            PlaybackStateKeys = null;

            if (File.Exists(basePath + "\\PlaybackStateKeys.json"))
            {
                using (TextReader textReader = File.OpenText(basePath + "\\PlaybackStateKeys.json"))
                {
                    PlaybackStateKeys = JsonConvert.DeserializeObject<KeySet>(textReader.ReadLine());

                    SetTextAndTextboxText(ref PlaybackStateKeys, ref PlaybackStateKeysText, ref PlaybackState);
                }
            }
            else
            { 
                PlaybackStateKeys = new KeySet(new StringCompare());
            }

            #endregion

            #region VolumeDownKeys

            VolumeDownKeys = null;

            if (File.Exists(basePath + "\\VolumeDownKeys.json"))
            {
                using (TextReader textReader = File.OpenText(basePath + "\\VolumeDownKeys.json"))
                {
                    VolumeDownKeys = JsonConvert.DeserializeObject<KeySet>(textReader.ReadLine());

                    SetTextAndTextboxText(ref VolumeDownKeys, ref VolumeDownKeysText, ref VolumeDown);
                }
            }
            else
            {
                VolumeDownKeys = new KeySet(new StringCompare());
            }

            #endregion

            #region VolumeUpKeys

            VolumeUpKeys = null;

            if (File.Exists(basePath + "\\VolumeUpKeys.json"))
            {
                using (TextReader textReader = File.OpenText(basePath + "\\VolumeUpKeys.json"))
                {
                    VolumeUpKeys = JsonConvert.DeserializeObject<KeySet>(textReader.ReadLine());

                    SetTextAndTextboxText(ref VolumeUpKeys, ref VolumeUpKeysText, ref VolumeUp);
                }
            }
            else
            {
                VolumeUpKeys = new KeySet(new StringCompare());
            }

            #endregion

            #region PreviousSongKeys

            PreviousSongKeys = null;

            if (File.Exists(basePath + "\\PreviousSongKeys.json"))
            {
                using (TextReader textReader = File.OpenText(basePath + "\\PreviousSongKeys.json"))
                {
                    PreviousSongKeys = JsonConvert.DeserializeObject<KeySet>(textReader.ReadLine());

                    SetTextAndTextboxText(ref PreviousSongKeys, ref PreviousSongKeysText, ref PreviousSong);
                }
            }
            else
            {
                PreviousSongKeys = new KeySet(new StringCompare());
            }

            #endregion

            #region NextSongKeys

            NextSongKeys = null;

            if (File.Exists(basePath + "\\NextSongKeys.json"))
            {
                using (TextReader textReader = File.OpenText(basePath + "\\NextSongKeys.json"))
                {
                    NextSongKeys = JsonConvert.DeserializeObject<KeySet>(textReader.ReadLine());

                    SetTextAndTextboxText(ref NextSongKeys, ref NextSongKeysText, ref NextSong);
                }
            }
            else
            {
                NextSongKeys = new KeySet(new StringCompare());
            }

            #endregion
        }

        #region KeyDownEvent

        private void PlaybackStateKeyDown(object sender, KeyEventArgs e)
        {
            TextBoxKeyDown(e, ref PlaybackStateKeys, ref PlaybackStateKeysText, ref PlaybackState);
        }

        private void VolumeDownKeyDown(object sender, KeyEventArgs e)
        {
            TextBoxKeyDown(e, ref VolumeDownKeys, ref VolumeDownKeysText, ref VolumeDown);
        }

        private void VolumeUpKeyDown(object sender, KeyEventArgs e)
        {
            TextBoxKeyDown(e, ref VolumeUpKeys, ref VolumeUpKeysText, ref VolumeUp);
        }

        private void PreviousSongKeyDown(object sender, KeyEventArgs e)
        {
            TextBoxKeyDown(e, ref PreviousSongKeys, ref PreviousSongKeysText, ref PreviousSong);
        }

        private void NextSongKeyDown(object sender, KeyEventArgs e)
        {
            TextBoxKeyDown(e, ref NextSongKeys, ref NextSongKeysText, ref NextSong);
        }

        #endregion

        #region Button Click Event

        private void Setting(object sender, System.Windows.RoutedEventArgs e)
        {
            instance.Reset();

            instance.OnRelease(PlaybackStateKeysText, "PlayStateFunction");

            instance.OnRelease(VolumeDownKeysText, "VolumeDownFunction");

            instance.OnRelease(VolumeUpKeysText, "VolumeUpFunction");

            instance.OnRelease(PreviousSongKeysText, "PreviousFunction");

            instance.OnRelease(NextSongKeysText, "NextFunction");

            #region serialize

            using (StreamWriter sw = new StreamWriter(basePath + "\\PlaybackStateKeys.json"))
            {
                string json = JsonConvert.SerializeObject(PlaybackStateKeys);
                sw.WriteLine(json);
            }
            using (StreamWriter sw = new StreamWriter(basePath + "\\VolumeDownKeys.json"))
            {
                string json = JsonConvert.SerializeObject(VolumeDownKeys);
                sw.WriteLine(json);
            }
            using (StreamWriter sw = new StreamWriter(basePath + "\\VolumeUpKeys.json"))
            {
                string json = JsonConvert.SerializeObject(VolumeUpKeys);
                sw.WriteLine(json);
            }
            using (StreamWriter sw = new StreamWriter(basePath + "\\PreviousSongKeys.json"))
            {
                string json = JsonConvert.SerializeObject(PreviousSongKeys);
                sw.WriteLine(json);
            }
            using (StreamWriter sw = new StreamWriter(basePath + "\\NextSongKeys.json"))
            {
                string json = JsonConvert.SerializeObject(NextSongKeys);
                sw.WriteLine(json);
            }

            PluginData.GlobalInputEvent.Serialize();

            #endregion

            Close();
        }

        private void Cancel(object sender, System.Windows.RoutedEventArgs e)
        {
            Close();
        }

        private void ResetBindedFunction(object sender, System.Windows.RoutedEventArgs e)
        {
            PlaybackStateKeys.List.Clear();
            PlaybackStateKeysText = "";
            PlaybackState.Text = "";
            
            VolumeDownKeys.List.Clear();
            VolumeDownKeysText = "";
            VolumeDown.Text = "";

            VolumeUpKeys.List.Clear();
            VolumeUpKeysText = "";
            VolumeUp.Text = "";

            PreviousSongKeys.List.Clear();
            PreviousSongKeysText = "";
            PreviousSong.Text = "";

            NextSongKeys.List.Clear();
            NextSongKeysText = "";
            NextSong.Text = "";

            PluginData.GlobalInputEvent.Reset();
        }

        #endregion

        #region Util

        #region Function

        private string SortedSettoString(KeySet set)
        {
            string str = "";

            foreach (string s in set.List)
            {
                if (str == "")
                {
                    str += s;
                }
                else
                {
                    str += " + " + s;
                }
            }

            return str;
        }

        private void TextBoxKeyDown(KeyEventArgs e, ref KeySet set, ref string textBoxTextstr, ref TextBox text)
        {
            string str = KeyToString(e.Key);

            set.List.Add(str);
            SetTextAndTextboxText(ref set, ref textBoxTextstr, ref text);
        }

        private void SetTextAndTextboxText(ref KeySet set, ref string textBoxTextstr, ref TextBox text)
        {
            textBoxTextstr = SortedSettoString(set);
            text.Text = textBoxTextstr;
        }

        private string KeyToString(Key key)
        {
            string str = key.ToString();

            if (key == Key.LeftCtrl || key == Key.RightCtrl)
            {
                str = "Ctrl";
            }
            if (key == Key.LeftAlt || key == Key.RightAlt)
            {
                str = "Alt";
            }
            if (key == Key.LeftShift || key == Key.RightShift)
            {
                str = "Shift";
            }

            return str;
        }

        #endregion

        #region Class

        [System.Serializable]
        public class StringCompare : IComparer<string>
        {
            public int Compare(string x, string y)
            {
                if (x == y)
                {
                    return 0;
                }

                if (x == "Ctrl")
                {
                    return -1;
                }
                else if (y == "Ctrl")
                {
                    return 1;
                }

                if (x == "Alt" && y != "Ctrl")
                {
                    return -1;
                }

                if (y == "Alt" && x != "Ctrl")
                {
                    return 1;
                }

                if (x == "Shift" && (y != "Ctrl" || y != "Alt"))
                {
                    return -1;
                }

                if (y == "Shift" && (x != "Ctrl" || x != "Alt"))
                {
                    return 1;
                }

                return string.Compare(x, y);
            }
        }

        #endregion

        #endregion
    }
}
