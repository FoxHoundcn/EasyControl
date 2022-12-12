using ControllorPlugin;
using SharpDX;
using System;
using System.Collections.Generic;
using System.IO;
using System.Management;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;

namespace EasyControl
{
    public static class PublicData
    {
        public static bool AutoLogin = false;
        public static bool Debug = false;
        public static bool PluginDebug = false;
        public static ServerState LastVersion = ServerState.Offline;
        public static string URL = @"https://easyfox.com.cn";
        public static string TutorialURL = @"https://space.bilibili.com/8840273";
        //--------------------------------------------------------
        private static int _autoSaveTime = 0;
        public static int autoSaveTime
        {
            get { return _autoSaveTime; }
            set
            {
                _autoSaveTime = value;
                if (Localization.Instance.CheckAutoSaveOn() && Localization.Instance.CheckAutoTime(_autoSaveTime / 60))
                {
                    _autoSaveTime = 0;
                    int autoSaveIndex = Localization.Instance.GetAutoSaveIndex();
                    if (autoSaveIndex != -1)
                    {
                        if (PublicData.AutoSaveAll(autoSaveIndex.ToString()))
                        {
                            Localization.Instance.SetAutoSaveIndex();
                        }
                    }
                }
            }
        }
        public static UIType ui_Type = UIType.NodeLink;
        public static SettingType set_Type = SettingType.Settings;
        public static int devCount = 0;
        public static UInt16 publicColorCount = 0;//颜色同步
        #region Update
        public static bool BinReady = false;
        public static byte[] UpdateBinArrayV31;
        public static byte[] UpdateBinArrayV35;
        public static byte[] UpdateBinArrayVKB;
        public static byte[] UpdateBinArrayVNRF;
        public static byte[] UpdateBinArrayV4b;
        public const int BinLengthV3x = 0x10000;
        public const ushort BinStartV3x = 0x3000;
        public const int PageLengthV3x = 0x400;
        public const int BinLengthV4b = 0x40000;
        public const ushort BinStartV4b = 0x3000;
        public const int PageLengthV4b = 0x800;
        #endregion
        public static byte checkUpdateTime = 200;
        //--------------------------------------------------------
        private static int _currentJoyObjectIndex = -1;
        public static int BeforeJoyObjectIndex = -1;
        public static int CurrentJoyObjectIndex
        {
            get
            {
                return _currentJoyObjectIndex;
            }
            set
            {
                if (_currentJoyObjectIndex != value)
                {
                    BeforeJoyObjectIndex = _currentJoyObjectIndex;
                    _currentJoyObjectIndex = value;
                }
            }
        }
        #region 鼠标
        public static int MouseX { get; set; }
        public static int MouseY { get; set; }
        public static Vector2 MousePoint { get { return new Vector2(MouseX, MouseY); } }
        public static bool MoveFrom = false;
        public static int MoveFromX { get; set; }
        public static int MoveFromY { get; set; }
        #endregion
        #region node move
        private static uiNode _MoveNode = null;
        public static uiNode MoveNode
        {
            get { return _MoveNode; }
            set
            {
                if (_MoveNode == value)
                    return;
                _MoveNode = value;
                //if (_MoveNode == null)
                //    NodeLinkControl.Instance.Save();
            }
        }
        private static uiPort _SelectPort = null;
        public static uiPort SelectPort
        {
            get { return _SelectPort; }
            set
            {
                if (_SelectPort == value)
                    return;
                _SelectPort = value;
                //if (_SelectPort == null)
                //    NodeLinkControl.Instance.Save();
            }
        }
        #endregion
        #region 文本输入
        public static IntPtr m_hImc;
        public static uiTextEditor currentTextEdit = null;
        #endregion
        #region 字库编辑
        public static List<eFont> fontTypeList = new List<eFont>();
        public static float fontScale = 1f;
        public static int fontOffsetX;
        public static int fontOffsetY;
        #endregion
        #region MAC
        public static string GetNetworkAdpaterID()
        {
            try
            {
                string mac = "";
                ManagementClass mc = new ManagementClass("Win32_NetworkAdapterConfiguration");
                ManagementObjectCollection moc = mc.GetInstances();
                foreach (ManagementObject mo in moc)
                    if ((bool)mo["IPEnabled"] == true)
                    {
                        mac = mo["MacAddress"].ToString();
                        break;
                    }
                moc = null;
                mc = null;
                return mac.Trim().Replace(':', '-');
            }
            catch (Exception e)
            {
                return "error:" + e.Message;
            }
        }
        #endregion
        #region 存档
        public static SaveData saveData;
        #endregion
        //////////////////////////////////////////////////////////////////////////////////////////////////////////
        #region 读取保存
        public static bool AutoSaveAll(string fileName)
        {
            List<JoyObject> joyList = JoyUSB.Instance.GetJoyList();
            foreach (JoyObject joyObj in joyList)
            {
                string logMsg;
                if (!SaveJoy(false, joyObj, "autoSave_eJoy_" + joyObj.McuID + "_" + fileName, out logMsg) ||
                    !SaveFont(false, joyObj, "autoSave_eFont_" + joyObj.McuID + "_" + fileName, out logMsg))
                {
                    return false;
                }
            }
            return true;
        }
        private static bool CheckFileExists(string path, bool isExists, string fileName, out string fullPath)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            string checkFileName = fileName + @".xml";
            StringBuilder rBuilder = new StringBuilder(checkFileName);
            foreach (char rInvalidChar in Path.GetInvalidFileNameChars())
            {
                rBuilder.Replace(rInvalidChar.ToString(), string.Empty);
            }
            fullPath = path + @"\" + rBuilder.ToString();
            if (File.Exists(fullPath) && isExists)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public static bool SaveControl(bool isExists, string fileName, out string logMsg)
        {
            string ejoyPath = System.Environment.CurrentDirectory + @"\Econtrol";
            string fullPath;
            if (CheckFileExists(ejoyPath, isExists, fileName, out fullPath))
            {
                logMsg = "FileExists";
                return false;
            }
            else
            {
                NodeLinkControl.Instance.Save();
                saveData.SaveAs(fullPath);
                logMsg = "SaveSuccess";
                return true;
            }
        }
        public static bool SaveJoy(bool isExists, JoyObject currentObj, string fileName, out string logMsg)
        {
            string ejoyPath = System.Environment.CurrentDirectory + @"\Ejoy";
            string fullPath;
            if (CheckFileExists(ejoyPath, isExists, fileName, out fullPath))
            {
                logMsg = "FileExists";
                return false;
            }
            else
            {
                currentObj.Save(fullPath);
                logMsg = "SaveSuccess";
                return true;
            }
        }
        public static bool SaveFont(bool isExists, JoyObject currentObj, string fileName, out string logMsg)
        {
            string filePath;
            string fontPath = System.Environment.CurrentDirectory + @"\Fonts";
            if (!Directory.Exists(fontPath))
            {
                Directory.CreateDirectory(fontPath);
            }
            eFont font = currentObj.GetCurrentFont();
            if (font != null)
            {
                filePath = fontPath + @"\" + fileName + @".ecf";
                if (File.Exists(filePath) && isExists)
                {
                    logMsg = "FileExists";
                    return false;
                }
                else
                {
                    byte[] saveFont = font.GetFontByteArrayAll();
                    byte[] saveData = PublicData.CopyByte(new byte[4] { font.FontWidth, font.FontHeight, font.StartChar, font.EndChar }, saveFont);
                    File.WriteAllBytes(filePath, saveData);
                    logMsg = "SaveSuccess";
                    return true;
                }
            }
            logMsg = "Font is null !";
            return false;
        }
        #endregion
        #region 方法
        public static JoyObject GetCurrentSelectJoyObject()
        {
            List<JoyObject> joyList = JoyUSB.Instance.GetJoyList();
            if (CurrentJoyObjectIndex >= 0 && CurrentJoyObjectIndex < joyList.Count)
            {
                JoyObject joy = joyList[CurrentJoyObjectIndex];
                joy.CheckDeviceOutputList();
                return joy;
            }
            return null;
        }
        public static Color4 GetDxColor(int R, int G, int B, int A = 255)
        {
            return new Color4(R / 255f, G / 255f, B / 255f, A / 255f);
        }
        public static RectangleF GetActualRange(RectangleF _rect, int actualRange)
        {
            if (actualRange < 1)
                actualRange = 1;
            float x = _rect.X + actualRange;
            float y = _rect.Y + actualRange;
            float w = _rect.Width - actualRange * 2;
            if (w <= 0)
                w = 1;
            float h = _rect.Height - actualRange * 2;
            if (h <= 0)
                h = 1;
            return new RectangleF(x, y, w, h);
        }
        public static RectangleF GetActualRange(RectangleF _rect, int widthRange, int heightRange)
        {
            if (widthRange < 1)
                widthRange = 1;
            if (heightRange < 1)
                heightRange = 1;
            float x = _rect.X + widthRange;
            float y = _rect.Y + heightRange;
            float w = _rect.Width - widthRange * 2;
            if (w <= 0)
                w = 1;
            float h = _rect.Height - heightRange * 2;
            if (h <= 0)
                h = 1;
            return new RectangleF(x, y, w, h);
        }
        public static Color4 GetIOColor(PortValue type)
        {
            Color4 color = XmlUI.DxDeviceYellow;
            switch (type)
            {
                case PortValue.Int64:
                    color = XmlUI.DxDeviceBlue;
                    break;
                case PortValue.Double:
                    color = XmlUI.DxDeviceGreen;
                    break;
                case PortValue.String:
                    color = XmlUI.DxDeviceRed;
                    break;
            }
            return color;
        }
        public static bool InSide(int X, int Y, RectangleF Rect)
        {
            if (X >= Rect.X && X < Rect.X + Rect.Width &&
                Y >= Rect.Y && Y < Rect.Y + Rect.Height)
            {
                return true;
            }
            return false;
        }
        #endregion
        #region 颜色
        public static Color4 GetDeviceTypeColor(DeviceType type)
        {
            if (type <= DeviceType.None)
            {
                return XmlUI.DxDeviceRed;
            }
            else if (type > DeviceType.None && type <= DeviceType.SB_MouseMiddle)
            {
                return XmlUI.DxDeviceBlue;
            }
            else if (type > DeviceType.SB_MouseMiddle && type <= DeviceType.MB_Hat)
            {
                return XmlUI.DxDeviceGreen;
            }
            else if (type > DeviceType.MB_Hat && type <= DeviceType.F_MouseY)
            {
                return XmlUI.DxDeviceYellow;
            }
            else
            {
                return XmlUI.DxDevicePurple;
            }
        }
        public static Color4 GetCustomTypeColor(CustomType type)
        {
            if (type <= CustomType.NoneCustom)
            {
                return XmlUI.DxDeviceRed;
            }
            else if (type > CustomType.NoneCustom && type <= CustomType.DT_HT16K33)
            {
                return XmlUI.DxDeviceBlue;
            }
            else if (type > CustomType.DT_HT16K33 && type <= CustomType.OLED_256_64_SSD1322x4)
            {
                return XmlUI.DxDeviceGreen;
            }
            else if (type > CustomType.OLED_256_64_SSD1322x4 && type <= CustomType.OUT_IO)
            {
                return XmlUI.DxDeviceYellow;
            }
            else
            {
                return XmlUI.DxDevicePurple;
            }
        }
        #endregion
        #region PortShowType
        public static void SetButtonEnable(string ID, List<PortShowType> typeList)
        {
            for (int i = 0; i < typeList.Count; i++)
            {
                uiButton btn = XmlUI.Instance.GetButton(ID + i);
                if (btn != null)
                {
                    if (typeList[i] == PortShowType.Used)
                        btn.Enable = true;
                    else
                        btn.Enable = false;
                }
                else
                {
                    WarningForm.Instance.OpenUI("SetButtonEnable Error !!! " + ID + i, false);
                }
            }
        }
        public static void SetButtonSelect(string ID, List<PortShowType> typeList)
        {
            for (int i = 0; i < typeList.Count; i++)
            {
                uiButton btn = XmlUI.Instance.GetButton(ID + i);
                if (btn != null)
                {
                    switch (typeList[i])
                    {
                        case PortShowType.None:
                            btn.SelectOn = false;
                            break;
                        case PortShowType.Used:
                            btn.SelectOn = false;
                            break;
                        case PortShowType.Apply:
                            btn.SelectOn = true;
                            break;
                        case PortShowType.Error:
                            btn.SelectOn = false;
                            break;
                        case PortShowType.UsedError:
                            btn.SelectOn = false;
                            break;
                        default:
                            WarningForm.Instance.OpenUI("SetButtonList PortShowType Error !!!", false);
                            break;
                    }
                }
                else
                {
                    WarningForm.Instance.OpenUI("SetButtonList Error !!! " + ID + i, false);
                }
            }
        }
        public static void SetButtonList(string ID, List<PortShowType> typeList)
        {
            for (int i = 0; i < typeList.Count; i++)
            {
                uiButton btn = XmlUI.Instance.GetButton(ID + i);
                if (btn != null)
                {
                    switch (typeList[i])
                    {
                        case PortShowType.None:
                            btn.BackColor = XmlUI.DxUIBackColor;
                            break;
                        case PortShowType.Used:
                            btn.BackColor = XmlUI.DxUsedColor;
                            break;
                        case PortShowType.Apply:
                            btn.BackColor = XmlUI.DxApplyColor;
                            break;
                        case PortShowType.Error:
                            btn.BackColor = XmlUI.DxWarningColor;
                            break;
                        case PortShowType.UsedError:
                            btn.BackColor = XmlUI.DxSelectColor;
                            break;
                        default:
                            WarningForm.Instance.OpenUI("SetButtonList PortShowType Error !!!", false);
                            break;
                    }
                }
                else
                {
                    WarningForm.Instance.OpenUI("SetButtonList Error !!! " + ID + i, false);
                }
            }
        }
        public static void SetButtonColor(string ID, List<PortShowType> typeList)
        {
            for (int i = 0; i < typeList.Count; i++)
            {
                uiButton btn = XmlUI.Instance.GetButton(ID + i);
                if (btn != null)
                {
                    btn.AlwaysOn = true;
                    switch (typeList[i])
                    {
                        case PortShowType.None:
                            btn.AlwaysOn = false;
                            btn.ForeColor = XmlUI.DxUIBackColor;
                            break;
                        case PortShowType.Used:
                            btn.ForeColor = XmlUI.DxUsedColor;
                            break;
                        case PortShowType.Apply:
                            btn.ForeColor = XmlUI.DxApplyColor;
                            break;
                        case PortShowType.Error:
                            btn.ForeColor = XmlUI.DxWarningColor;
                            break;
                        case PortShowType.UsedError:
                            btn.ForeColor = XmlUI.DxSelectColor;
                            break;
                        default:
                            WarningForm.Instance.OpenUI("SetButtonColor PortShowType Error !!!", false);
                            break;
                    }
                }
                else
                {
                    WarningForm.Instance.OpenUI("SetButtonColor Error !!! " + ID + i, false);
                }
            }
        }
        #endregion
        #region Panel
        public static void SetPanelColor(string ID, List<Color4> colorList)
        {
            for (int i = 0; i < colorList.Count; i++)
            {
                uiPanel panel = XmlUI.Instance.GetPanel(ID + i);
                if (panel != null)
                {
                    panel.ForeColor = colorList[i];
                }
                else
                {
                    WarningForm.Instance.OpenUI("SetPanelColor Error !!! " + ID + i);
                }
            }
        }
        public static void SetPanelUsed(string ID, List<PortShowType> markList, Color4 color)
        {
            for (int i = 0; i < markList.Count; i++)
            {
                uiPanel panel = XmlUI.Instance.GetPanel(ID + i);
                if (panel != null)
                {
                    panel.ForeColor = color;
                    switch (markList[i])
                    {
                        case PortShowType.None:
                            panel.ForeColor = XmlUI.DxUIBackColor;
                            break;
                        case PortShowType.Used:
                        case PortShowType.Apply:
                        case PortShowType.Error:
                        case PortShowType.UsedError:
                            break;
                        default:
                            WarningForm.Instance.OpenUI("SetPanelSwitch PortShowType Error !!!", false);
                            break;
                    }
                }
                else
                {
                    WarningForm.Instance.OpenUI("SetPanelSwitch Error !!! " + ID + i, false);
                }
            }
        }
        public static void SetPanelSwitch(string ID, List<PortShowType> markList)
        {
            for (int i = 0; i < markList.Count; i++)
            {
                uiPanel panel = XmlUI.Instance.GetPanel(ID + i);
                if (panel != null)
                {
                    switch (markList[i])
                    {
                        case PortShowType.None:
                            panel.ForeColor = XmlUI.DxUIBackColor;
                            break;
                        case PortShowType.Used:
                            panel.ForeColor = XmlUI.DxDeviceBlue;
                            break;
                        case PortShowType.Apply:
                            panel.ForeColor = XmlUI.DxDeviceGreen;
                            break;
                        case PortShowType.Error:
                            panel.ForeColor = XmlUI.DxDeviceRed;
                            break;
                        case PortShowType.UsedError:
                            panel.ForeColor = XmlUI.DxDevicePurple;
                            break;
                        default:
                            WarningForm.Instance.OpenUI("SetPanelSwitch PortShowType Error !!!", false);
                            break;
                    }
                }
                else
                {
                    WarningForm.Instance.OpenUI("SetPanelSwitch Error !!! " + ID + i, false);
                }
            }
        }
        #endregion
        #region select
        public static void SetButtonSelect(string ID, int index, int max)
        {
            for (int i = 0; i < max; i++)
            {
                uiButton btn = XmlUI.Instance.GetButton(ID + i);
                if (btn != null)
                {
                    if (i == index)
                        btn.SelectOn = true;
                    else
                        btn.SelectOn = false;
                }
            }
        }
        #endregion
        #region char2byte
        public static string ByteArray2String(byte[] guid, bool x0 = true)
        {
            string guidString = "";
            for (int i = 0; i < guid.Length; i++)
            {
                if (x0)
                {
                    guidString += "0x";
                }
                guidString += guid[i].ToString("x2");
                if (i < guid.Length - 1 && x0)
                    guidString += ", ";
            }
            return guidString;
        }
        public static byte[] String2ByteArray(string hex)
        {
            string temp = hex.Replace("0x", "").Replace(",", "").Replace(" ", "").Trim();
            byte[] inString = new byte[temp.Length / 2];
            for (int i = 0; i < temp.Length; i += 2)
            {
                int a = char2byte(temp[i]);
                int b = char2byte(temp[i + 1]);
                int c = (a << 4) + b;
                inString[i / 2] = (byte)c;
            }
            return inString;
        }
        public static byte char2byte(char _num)
        {
            switch (_num)
            {
                case '0':
                    return 0;
                case '1':
                    return 1;
                case '2':
                    return 2;
                case '3':
                    return 3;
                case '4':
                    return 4;
                case '5':
                    return 5;
                case '6':
                    return 6;
                case '7':
                    return 7;
                case '8':
                    return 8;
                case '9':
                    return 9;
                case 'A':
                case 'a':
                    return 10;
                case 'B':
                case 'b':
                    return 11;
                case 'C':
                case 'c':
                    return 12;
                case 'D':
                case 'd':
                    return 13;
                case 'E':
                case 'e':
                    return 14;
                case 'F':
                case 'f':
                    return 15;
            }
            return 0;
        }
        #endregion
        #region SHA1
        public static bool CheckSHA1(string filePath, byte[] check)
        {
            byte[] bBuffer;
            FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
            BinaryReader binReader = new BinaryReader(fs);
            bBuffer = new byte[fs.Length];
            binReader.Read(bBuffer, 0, (int)fs.Length);
            binReader.Close();
            fs.Close();
            using (SHA1Managed sha1 = new SHA1Managed())
            {
                byte[] hash = sha1.ComputeHash(bBuffer);
                for (int code = 0; code < hash.Length; code++)
                {
                    if (hash[code] != check[code])
                    {
                        return false;
                    }
                }
            }
            return true;
        }
        public static byte[] GetFileByte(string filePath)
        {
            byte[] bBuffer;
            FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
            BinaryReader binReader = new BinaryReader(fs);
            bBuffer = new byte[fs.Length];
            binReader.Read(bBuffer, 0, (int)fs.Length);
            binReader.Close();
            fs.Close();
            return bBuffer;
        }
        public static byte[] GetSHA1Byte(string filePath)
        {
            byte[] bBuffer = GetFileByte(filePath);
            using (SHA1Managed sha1 = new SHA1Managed())
            {
                return sha1.ComputeHash(bBuffer);
            }
        }
        public static string GetSHA1String(string filePath)
        {
            byte[] buffer = GetSHA1Byte(filePath);
            string sha1 = "";
            for (int i = 0; i < buffer.Length; i++)
            {
                sha1 += buffer[i].ToString("x2");
            }
            return sha1;
        }
        #endregion
        #region 拼接byte[]
        public static byte[] CopyByte(byte[] a, byte[] b)
        {
            byte[] c = new byte[a.Length + b.Length];
            a.CopyTo(c, 0);
            b.CopyTo(c, a.Length);
            return c;
        }
        #endregion
        #region 缓存升级bin文件
        public static void ReadBinFile(string _ver)
        {
            string filePathVKB = "";
            string filePathV31 = "";
            string filePathV35 = "";
            string filePathVNRF = "";
            string filePathV4b = "";
            switch (_ver)
            {
                case "3xU":
                    filePathVKB = Environment.CurrentDirectory + "\\\\EasyKeyBoard.bin";
                    filePathV31 = Environment.CurrentDirectory + "\\\\EasyJoy32_v31.bin";
                    filePathV35 = Environment.CurrentDirectory + "\\\\EasyJoy32_v35.bin";
                    filePathVNRF = Environment.CurrentDirectory + "\\\\EasyJoy32_NRF24.bin";
                    break;
                case "41U":
                    filePathV4b = Environment.CurrentDirectory + "\\\\EasyJoy32_v4b.bin";
                    break;
            }
            if (filePathVKB.Length > 0 && File.Exists(filePathVKB))
            {
                FileStream fs = new FileStream(filePathVKB, FileMode.Open);
                long size = fs.Length;
                UpdateBinArrayVKB = new byte[size];
                fs.Read(UpdateBinArrayVKB, 0, UpdateBinArrayVKB.Length);
                fs.Close();
                BinReady = true;
            }
            if (filePathV31.Length > 0 && File.Exists(filePathV31))
            {
                FileStream fs = new FileStream(filePathV31, FileMode.Open);
                long size = fs.Length;
                UpdateBinArrayV31 = new byte[size];
                fs.Read(UpdateBinArrayV31, 0, UpdateBinArrayV31.Length);
                fs.Close();
                BinReady = true;
            }
            if (filePathV35.Length > 0 && File.Exists(filePathV35))
            {
                FileStream fs = new FileStream(filePathV35, FileMode.Open);
                long size = fs.Length;
                UpdateBinArrayV35 = new byte[size];
                fs.Read(UpdateBinArrayV35, 0, UpdateBinArrayV35.Length);
                fs.Close();
                BinReady = true;
            }
            if (filePathVNRF.Length > 0 && File.Exists(filePathVNRF))
            {
                FileStream fs = new FileStream(filePathVNRF, FileMode.Open);
                long size = fs.Length;
                UpdateBinArrayVNRF = new byte[size];
                fs.Read(UpdateBinArrayVNRF, 0, UpdateBinArrayVNRF.Length);
                fs.Close();
                BinReady = true;
            }
            if (filePathV4b.Length > 0 && File.Exists(filePathV4b))
            {
                FileStream fs = new FileStream(filePathV4b, FileMode.Open);
                long size = fs.Length;
                UpdateBinArrayV4b = new byte[size];
                fs.Read(UpdateBinArrayV4b, 0, UpdateBinArrayV4b.Length);
                fs.Close();
                BinReady = true;
            }
        }
        #endregion
    }
}
