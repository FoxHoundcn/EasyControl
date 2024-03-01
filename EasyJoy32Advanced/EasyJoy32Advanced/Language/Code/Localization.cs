using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace EasyControl
{
    public sealed class Localization
    {
        private int _currentIndex = -1;
        public int CurrentIndex
        {
            get { return _currentIndex; }
            set
            {
                if (_currentIndex == value)
                    return;
                if (value >= 0 && value < LanguageString.Count)
                {
                    _currentIndex = value;
                    string language;
                    if (LanguageString[_currentIndex].IniReadValue("Language", out language))
                    {
                        Settings.IniWriteValue("Language", language);
                        LSList.Clear();
                    }
                }
            }
        }
        //---------------------------------------------------------------------------
        private iniString Settings = new iniString("Settings", System.Environment.CurrentDirectory + SettingPath);                              //设置
        private List<iniString> LanguageString = new List<iniString>();       //语言
        private const string SettingPath = @"\Config\Settings.ini";        //设置路径
        private static Dictionary<string, string> LSList = new Dictionary<string, string>();
        private byte[] License = { 0x3c, 0x83, 0xee, 0xaf, 0x3b, 0xa9, 0x4d, 0x4d, 0xba, 0x53, 0x3c, 0xdd, 0xea, 0xa7, 0xc6, 0xf9 };
        //---------------------------------------------------------------------------

        public static readonly Localization Instance = new Localization();
        private Localization()
        {
        }
        #region Settings
        public bool GetHideWindows()
        {
            string Value;
            if (!Settings.IniReadValue("HideWindows", out Value))
            {
                Value = "0";
                Settings.IniWriteValue("HideWindows", Value);
            }
            return !Value.Equals("0");
        }
        public int GetCustomNodeCount(int count)
        {
            string Value;
            if (!Settings.IniReadValue("CustomNodeLinkCount", out Value))
            {
                Value = "16";
                Settings.IniWriteValue("CustomNodeLinkCount", Value);
            }
            int saveIndex;
            if (int.TryParse(Value, out saveIndex))
            {
                return saveIndex;
            }
            return count;
        }
        public void SetCustomNodeCount(int count)
        {
            Settings.IniWriteValue("CustomNodeLinkCount", count.ToString());
        }
        public int GetAutoSaveTime(int time)
        {
            string Value;
            if (!Settings.IniReadValue("AutoSaveTime", out Value))
            {
                Value = "1";
                Settings.IniWriteValue("AutoSaveTime", Value);
            }
            int saveIndex;
            if (int.TryParse(Value, out saveIndex))
            {
                return saveIndex;
            }
            return time;
        }
        public void SetAutoSaveTime(int time)
        {
            Settings.IniWriteValue("AutoSaveTime", time.ToString());
        }
        public int GetAutoSaveCount(int count)
        {
            string Value;
            if (!Settings.IniReadValue("AutoSaveCount", out Value))
            {
                Value = "1";
                Settings.IniWriteValue("AutoSaveCount", Value);
            }
            int saveIndex;
            if (int.TryParse(Value, out saveIndex))
            {
                return saveIndex;
            }
            return count;
        }
        public void SetAutoSaveCount(int count)
        {
            Settings.IniWriteValue("AutoSaveCount", count.ToString());
        }
        #endregion
        #region checkUSB
        public bool GetCheckUSB()
        {
            string Value;
            if (!Settings.IniReadValue("CheckUSB", out Value))
            {
                Value = "0";
                Settings.IniWriteValue("CheckUSB", Value);
            }
            int checkUSBDefault;
            if (int.TryParse(Value, out checkUSBDefault))
            {
                if (checkUSBDefault != 0)
                    return true;
            }
            return false;
        }
        public void SetCheckUSB(bool open)
        {
            if (open)
            {
                Settings.IniWriteValue("CheckUSB", "1");
            }
            else
            {
                Settings.IniWriteValue("CheckUSB", "0");
            }
        }
        #endregion
        #region ledOnDefault
        public bool GetLedOnDefault()
        {
            string Value;
            if (!Settings.IniReadValue("LedOnSwitch", out Value))
            {
                Value = "0";
                Settings.IniWriteValue("LedOnSwitch", Value);
            }
            int ledOnDefault;
            if (int.TryParse(Value, out ledOnDefault))
            {
                if (ledOnDefault != 0)
                    return true;
            }
            return false;
        }
        public void SetLedOnDefault(bool open)
        {
            if (open)
            {
                Settings.IniWriteValue("LedOnSwitch", "1");
            }
            else
            {
                Settings.IniWriteValue("LedOnSwitch", "0");
            }
        }
        #endregion
        #region AutoLogin
        public bool CheckAutoLogin()
        {
            string Value;
            if (!Settings.IniReadValue("AutoLogin", out Value))
            {
                Value = "0";
                Settings.IniWriteValue("AutoLogin", Value);
            }
            int autoLogin;
            if (int.TryParse(Value, out autoLogin))
            {
                if (autoLogin != 0)
                    return true;
            }
            return false;
        }
        public void SetAutoLogin(bool open)
        {
            if (open)
            {
                Settings.IniWriteValue("AutoLogin", "1");
            }
            else
            {
                Settings.IniWriteValue("AutoLogin", "0");
            }
        }
        private byte[] GetAutoLoginLicense()
        {
            string value = PublicData.GetNetworkAdpaterID().Replace("-", "");
            if (value.Length <= 0 || value.Length % 2 == 1)
                return new byte[0];
            byte[] tempLKb = new byte[value.Length / 2];
            for (int i = 0; i < tempLKb.Length; i++)
            {
                int a = PublicData.char2byte(value[i * 2]);
                int b = PublicData.char2byte(value[i * 2 + 1]);
                int c = (a << 4) + b;
                tempLKb[i] = (byte)c;
            }
            License[0] = tempLKb[0];
            License[3] = tempLKb[1];
            License[6] = tempLKb[2];
            License[9] = tempLKb[3];
            License[12] = tempLKb[4];
            License[15] = tempLKb[5];
            return License;
        }
        public string GetAutoLoginCode()
        {
            string Value = "";
            if (Settings.IniReadValue("AutoSaveCode", out Value))
                return Value;
            else
                return "";
        }
        public void SetAutoLoginCode(string value)
        {
            Settings.IniWriteValue("AutoSaveCode", value);
        }
        #endregion
        #region 自动保存
        public bool CheckAutoSaveOn()
        {
            string Value;
            if (!Settings.IniReadValue("AutoSaveOn", out Value))
            {
                Value = "1";
                Settings.IniWriteValue("AutoSaveOn", Value);
            }
            int autoSaveOn;
            if (int.TryParse(Value, out autoSaveOn))
            {
                if (autoSaveOn != 0)
                    return true;
            }
            return false;
        }
        public void SetAutoSaveOn(bool On)
        {
            if (On)
                Settings.IniWriteValue("AutoSaveOn", "1");
            else
                Settings.IniWriteValue("AutoSaveOn", "0");
        }
        public bool CheckAutoTime(int time)
        {
            string Value;
            if (!Settings.IniReadValue("AutoSaveTime", out Value))
            {
                Value = "1";
                Settings.IniWriteValue("AutoSaveTime", Value);
            }
            int saveTime;
            if (int.TryParse(Value, out saveTime))
            {
                if (time >= saveTime)
                    return true;
            }
            return false;
        }
        public int GetAutoSaveIndex()
        {
            string Value;
            if (!Settings.IniReadValue("AtuoSaveIndex", out Value))
            {
                Value = "1";
                Settings.IniWriteValue("AtuoSaveIndex", Value);
            }
            int saveIndex;
            if (int.TryParse(Value, out saveIndex))
            {
                return saveIndex;
            }
            return -1;
        }
        public void SetAutoSaveIndex()
        {
            string valueIndex;
            if (!Settings.IniReadValue("AtuoSaveIndex", out valueIndex))
            {
                valueIndex = "1";
                Settings.IniWriteValue("AtuoSaveIndex", valueIndex);
            }
            string valueCount;
            if (!Settings.IniReadValue("AutoSaveCount", out valueCount))
            {
                valueCount = "10";
                Settings.IniWriteValue("AutoSaveCount", valueCount);
            }
            int saveIndex, saveCount;
            if (int.TryParse(valueIndex, out saveIndex) &&
                int.TryParse(valueCount, out saveCount))
            {
                saveIndex++;
                if (saveIndex > saveCount)
                    saveIndex = 1;
                Settings.IniWriteValue("AtuoSaveIndex", saveIndex.ToString());
            }
        }
        #endregion
        //自定义连接模块数量
        public int GetCustomNodeLinkCount()
        {
            string Value;
            if (!Settings.IniReadValue("CustomNodeLinkCount", out Value))
            {
                Value = "16";
                Settings.IniWriteValue("CustomNodeLinkCount", Value);
            }
            int outValue;
            if (!int.TryParse(Value, out outValue))
                return 16;
            return outValue;
        }
        //debug模式
        public bool GetDebug()
        {
#if DEBUG
            return true;
#else
            string Value;
            if (Settings.IniReadValue("Debug", out Value))
            {
                return Value.Equals("1");
            }
            return false;
#endif
        }
        public void SetDebug(bool debug)
        {
            Settings.IniWriteValue("Debug", debug ? "1" : "0");
        }
        public List<string> Init()
        {
            LanguageString.Clear();
            string LanguageSet;
            if (!Settings.IniReadValue("Language", out LanguageSet))
            {
                if (System.Threading.Thread.CurrentThread.CurrentCulture.Name.Equals("zh-CN"))
                {
                    LanguageSet = "中文";
                }
                else
                {
                    LanguageSet = "English";
                }
                Settings.IniWriteValue("Language", LanguageSet);
            }
            string[] files = Directory.GetFiles(System.Environment.CurrentDirectory + @"\Language");
            int index = 0;
            foreach (string file in files)
            {
                string ext = file.Substring(file.LastIndexOf("."));
                if (ext != ".ini") continue;        //判断ini文件
                string path = file.Substring(file.LastIndexOf("\\"));
                iniString LanguageINI = new iniString("Language", file);
                string Language;
                if (LanguageINI.IniReadValue("Language", out Language))
                {
                    if (!Language.Equals(""))
                    {
                        LanguageString.Add(LanguageINI);
                        if (Language == LanguageSet)
                        {
                            CurrentIndex = index;
                        }
                        index++;
                    }
                }
            }
            if (LanguageString.Count > 0 && CurrentIndex == -1)
            {
                CurrentIndex = 0;
            }
            List<string> nameList = new List<string>();
            for (int i = 0; i < LanguageString.Count; i++)
            {
                string Value;
                if (LanguageString[i].IniReadValue("Language", out Value))
                    nameList.Add(Value);
            }
            return nameList;
        }

        public string GetLS(string key)
        {
            if (key.Trim().Equals(""))
                return "";
            if (LSList.ContainsKey(key))
            {
                return LSList[key];
            }
            else
            {
                if (CurrentIndex >= 0 && CurrentIndex < LanguageString.Count)
                {
                    string output;
                    if (LanguageString[CurrentIndex].IniReadValue(key, out output))
                    {
                        output = Regex.Replace(output, @"\\n", "\n");
                        if (output.Equals(""))
                        {
                            LSList.Add(key, "@" + key);
                            return "@" + key;
                        }
                        else
                        {
                            if (!LSList.ContainsKey(key))
                            {
                                LSList.Add(key, output);
                                return output;
                            }
                        }
                    }
                }
            }
            return key + " - NO Language";
        }
    }
}
