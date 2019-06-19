using System;
using System.Collections.Generic;
using System.Windows.Input;
using System.Threading;
using System.IO;
using Newtonsoft.Json;
using Presto.SDK;

namespace Presto.Hotkey.Input
{
    public class GlobalInput
    {
        #region Key map

        private Dictionary<string, Key> keyMap = new Dictionary<string, Key>();

        #endregion

        #region variables

        public Thread thread;

        private Dictionary<string, bool> currentSingleKeyStates;
        private Dictionary<string, bool> lastSingleKeyStates;
        public Dictionary<string, string> pressAction;
        public Dictionary<string, string> releaseAction;

        string basePath = PluginData.basePath;

        #endregion

        #region contructor

        public GlobalInput()
        {
            currentSingleKeyStates = new Dictionary<string, bool>();
            lastSingleKeyStates = new Dictionary<string, bool>();

            pressAction = null;

            if (File.Exists(basePath + "\\PressAction.json"))
            {
                using (TextReader textReader = File.OpenText(basePath + "\\PressAction.json"))
                {
                    pressAction = JsonConvert.DeserializeObject<Dictionary<string, string>>(textReader.ReadLine());
                }
            }
            else
            {
                pressAction = new Dictionary<string, string>();
            }

            releaseAction = null;

            if (File.Exists(basePath + "\\ReleaseAction.json"))
            {
                using (TextReader textReader = File.OpenText(basePath + "\\ReleaseAction.json"))
                {
                    releaseAction = JsonConvert.DeserializeObject<Dictionary<string, string>>(textReader.ReadLine());
                }
            }
            else
            {
                releaseAction = new Dictionary<string, string>();
            }

            thread = new Thread(timer_Elapsed);
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
        }

        #endregion

        #region private function

        private void timer_Elapsed()
        {
            while (true)
            {
                Thread.Sleep(40);
                CheckAllKeyboardInput();
                CallAction();
            }
        }

        private void CheckAllKeyboardInput()
        {
            foreach (KeyValuePair<string, bool> pair in currentSingleKeyStates)
            {
                if (lastSingleKeyStates.ContainsKey(pair.Key))
                {
                    lastSingleKeyStates[pair.Key] = pair.Value;
                }
                else
                {
                    lastSingleKeyStates.Add(pair.Key, pair.Value);
                }
            }

            currentSingleKeyStates.Clear();

            for (Key key = (Key)1; key <= (Key)172; key++)
            {
                string str = KeyToString(key);

                if(key == Key.LeftCtrl)
                {
                    str = str;
                }

                if (str == "Ctrl" || str == "Alt" || str == "Shift")
                {
                    if (currentSingleKeyStates.ContainsKey(str))
                    {
                        if (currentSingleKeyStates[str])
                        {
                            continue;
                        }
                    }
                }

                if (Keyboard.IsKeyDown(key))
                {
                    if (!keyMap.ContainsValue(key))
                    {
                        if (!keyMap.ContainsKey(str))
                        {
                            keyMap.Add(str, key);
                        }
                    }

                    if (!currentSingleKeyStates.ContainsKey(str))
                    {
                        currentSingleKeyStates.Add(str, true);
                    }
                    else
                    {
                        currentSingleKeyStates[str] = true;
                    }
                }
                else
                {
                    if (!keyMap.ContainsValue(key))
                    {
                        if (!keyMap.ContainsKey(str))
                        {
                            keyMap.Add(str, key);
                        }
                    }

                    if (!currentSingleKeyStates.ContainsKey(str))
                    {
                        currentSingleKeyStates.Add(str, false);
                    }
                    else
                    {
                        currentSingleKeyStates[str] = false;
                    }
                }
            }
        }

        private void CallAction()
        {
            foreach (KeyValuePair<string, string> keyValue in pressAction)
            {
                string[] keys = keyValue.Key.Split(new string[] { " + " }, StringSplitOptions.None);
                bool flag = true;

                foreach (string key in keys)
                {
                    if (currentSingleKeyStates.ContainsKey(key))
                    {
                        if (!currentSingleKeyStates[key])
                        {
                            flag = false;
                        }
                    }
                    else
                    {
                        flag = false;
                    }

                    if (!flag)
                    {
                        break;
                    }
                }
                if (flag)
                {
                    CallFunction(keyValue.Value);
                }
            }

            foreach (KeyValuePair<string, string> keyValue in releaseAction)
            {
                string[] keys = keyValue.Key.Split(new string[] { " + " }, StringSplitOptions.None);
                bool[] flag = new bool[keys.Length + 1];

                flag[keys.Length] = true;

                for (int i = 0; i < keys.Length; i++)
                {
                    flag[i] = true;
                    string key = keys[i];

                    if (currentSingleKeyStates.ContainsKey(key))
                    {
                        if (!currentSingleKeyStates[key])
                        {
                            flag[i] = false;
                        }
                    }
                    else
                    {
                        flag[i] = false;
                    }

                    flag[keys.Length] = flag[keys.Length] && flag[i];
                }

                bool[] flag1 = new bool[keys.Length + 1];
                flag1[keys.Length] = true;

                for (int i = 0; i < keys.Length; i++)
                {
                    flag1[i] = false;
                    string key = keys[i];

                    if (lastSingleKeyStates.ContainsKey(key))
                    {
                        if (lastSingleKeyStates[key])
                        {
                            flag1[i] = true;
                        }
                    }
                    else
                    {
                        flag1[i] = false;
                    }

                    flag1[keys.Length] = flag1[keys.Length] && flag1[i];
                }

                if (flag[keys.Length] == false && flag1[keys.Length] == true)
                {
                    CallFunction(keyValue.Value);
                }
            }
        }

        private void CallFunction(string action)
        {
            switch(action)
            {
                case "PlayStateFunction":
                    {
                        PlayStateFunction();
                    }
                    break;

                case "VolumeDownFunction":
                    {
                        VolumeDownFunction();
                    }
                    break;

                case "VolumeUpFunction":
                    {
                        VolumeUpFunction();
                    }
                    break;

                case "PreviousFunction":
                    {
                        PreviousFunction();
                    }
                    break;

                case "NextFunction":
                    {
                        NextFunction();
                    }
                    break;
            }
        }

        #endregion

        #region public function

        public void OnPress(string key, string action)
        {
            if (!pressAction.ContainsKey(key))
            {
                pressAction.Add(key, action);
            }
        }

        public void OnRelease(string key, string action)
        {
            if (!releaseAction.ContainsKey(key))
            {
                releaseAction.Add(key, action);
            }
        }

        public void Reset()
        {
            pressAction.Clear();
            releaseAction.Clear();
        }

        public void Serialize()
        {
            using (StreamWriter sw = new StreamWriter(basePath + "\\PressAction.json"))
            {
                string json = JsonConvert.SerializeObject(pressAction);
                sw.WriteLine(json);
            }
            using (StreamWriter sw = new StreamWriter(basePath + "\\ReleaseAction.json"))
            {
                string json = JsonConvert.SerializeObject(releaseAction);
                sw.WriteLine(json);
            }
        }

        #endregion

        #region Util

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

        #region ETC

        public void PlayStateFunction()
        {
            if (PrestoSDK.PrestoService.Player.PlaybackState.Equals(Presto.Common.PlaybackState.Playing))
            {
                PrestoSDK.PrestoService.Player.Pause();
            }
            else if (PrestoSDK.PrestoService.Player.PlaybackState.Equals(Presto.Common.PlaybackState.Paused))
            {
                PrestoSDK.PrestoService.Player.Resume();
            }
            else
            {
                PrestoSDK.PrestoService.Player.Play();
            }
        }

        public void VolumeDownFunction()
        {
            PrestoSDK.PrestoService.Player.Volume -= 0.01f;
        }

        public void VolumeUpFunction()
        {
            PrestoSDK.PrestoService.Player.Volume += 0.01f;
        }

        public void PreviousFunction()
        {
            PrestoSDK.PrestoService.Player.PlayPrevious();
        }
        
        public void NextFunction()
        {
            PrestoSDK.PrestoService.Player.PlayNext();
        }

        #endregion
    }
}
