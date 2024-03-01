//#define usbDebug
//#define tcpDebug
using ControllorPlugin;
using FoxH.HID;
using SharpDX;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Xml;

namespace EasyControl
{
    public class pinData
    {
        public int index { get; private set; }
        public int count { get; private set; }
        public ControlOutputType type { get; private set; }
        public pinData(int _index, int _count, ControlOutputType _type)
        {
            index = _index;
            count = _count;
            type = _type;
        }
    }
    //EasyJoy对象，描述一个ejoy设备
    public class JoyObject : IDisposable, IComparable<JoyObject>, InterfacePlugin
    {
        private Thread threadUSB = null;
        private Thread threadLAN = null;
        //------------------------------------------------------------------------
        public int Index { get; private set; }
        public LinkType linkType = LinkType.None;
        public string HardwareVersion { get; private set; }
        #region nodeList
        private List<Node> moduleList = new List<Node>();
        private List<pinData> pinIDList = new List<pinData>();
        #endregion
        #region 设备读写
        public bool loop = true;
        public bool JoyReady = false;
        public bool InReady = false;
        public bool OutReady = false;

        HIDDev devJoy = new HIDDev();
        HIDDev devIn = new HIDDev();
        HIDDev devOut = new HIDDev();

        Socket clientSocket = null;
        #endregion
        #region ReportManager
        private ReportManager reportMgr;
        public int reportCount { get { return reportMgr.GetCount(); } }
        public void AddReport(Report report)
        {
            reportMgr.AddReport(report);
        }
        public bool InCommunication()
        {
            return reportMgr.InCommunication();
        }
        #endregion
        #region USB Open
        public bool Ready = false;
        private bool _open = false;
        public bool Open
        {
            get { return _open; }
            set
            {
                try
                {
                    if (linkType != LinkType.USB)//检查连接模式
                    {
                        _open = false;
                        return;
                    }
                    if (value != _open)
                    {
                        if (value)
                        {
                            if (CheckJoy())
                            {
                                DebugConstol.AddLog(Index + " - " + usbName + " - OpenJoy");
                                _open = true;
                                loop = true;
                                #region Usb报文处理
                                threadUSB = new Thread(MessageLink);
                                threadUSB.IsBackground = true;
                                threadUSB.Start();
                                #endregion
                                #region 初始化报文
                                reportMgr.ReSet();
                                if (McuID.Length == 24)
                                {
                                    reportMgr.AddReport(new Report(ReportType.LinkTest));
                                    reportMgr.AddReport(new Report(ReportType.LicenseInfo));
                                    reportMgr.AddReport(new Report(ReportType.DeviceInfo));
                                    reportMgr.AddReport(new Report(ReportType.CustomInfo));
                                    reportMgr.AddReport(new Report(ReportType.ButtonInfo));
                                    reportMgr.AddReport(new Report(ReportType.HatInfo));
                                    reportMgr.AddReport(new Report(ReportType.AdcInfo));
                                    reportMgr.AddReport(new Report(ReportType.FormatInfo));
                                    reportMgr.AddReport(new Report(ReportType.LedInfo));
                                    reportMgr.AddReport(new Report(ReportType.GetUsbInfo));
                                }
                                else
                                {
                                    reportMgr.AddReport(new Report(ReportType.LinkTest));
                                    reportMgr.AddReport(new Report(ReportType.ReBoot));
                                }
                                #endregion
                                return;
                            }
                            else
                            {
                                DebugConstol.AddLog(Index + " - " + usbName + " - OpenJoy ERROR", LogType.Error);
                                _open = false;
                            }
                        }
                    }
                    if (!_open)
                        loop = false;
                }
                catch (Exception ex)
                {
                    DebugConstol.AddLog(Index + " - " + usbName + " - ERROR : " + ex.ToString(), LogType.Error);
                    loop = false;
                }
            }
        }
        #endregion
        #region LAN Open
        private byte[] tcpData = new byte[JoyConst.MaxUsbReport];
        public string ip { get; private set; } = "";
        private bool _Reconnection = false;
        public bool Reconnection
        {
            get { return _Reconnection; }
            set
            {
                try
                {
                    if (linkType != LinkType.LAN)//检查连接模式
                    {
                        _open = false;
                        return;
                    }
                    if (value != _open)
                    {
                        if (value)
                        {
                            DebugConstol.AddLog(Index + " - " + ip + " - Reconnection");
                            _open = true;
                            loop = true;
                            #region Usb报文处理
                            threadLAN = new Thread(TCPLink);
                            threadLAN.IsBackground = true;
                            threadLAN.Start();
                            #endregion
                            #region 初始化报文
                            reportMgr.ReSet();
                            reportMgr.AddReport(new Report(ReportType.LinkTest));
                            reportMgr.AddReport(new Report(ReportType.LicenseInfo));
                            reportMgr.AddReport(new Report(ReportType.DeviceInfo));
                            reportMgr.AddReport(new Report(ReportType.CustomInfo));
                            reportMgr.AddReport(new Report(ReportType.ButtonInfo));
                            reportMgr.AddReport(new Report(ReportType.HatInfo));
                            reportMgr.AddReport(new Report(ReportType.AdcInfo));
                            reportMgr.AddReport(new Report(ReportType.FormatInfo));
                            reportMgr.AddReport(new Report(ReportType.LedInfo));
                            reportMgr.AddReport(new Report(ReportType.GetUsbInfo));
                            #endregion
                            return;
                        }
                    }
                    if (!_open)
                        loop = false;
                }
                catch (Exception ex)
                {
                    DebugConstol.AddLog(Index + " - " + usbName + " - ERROR : " + ex.ToString(), LogType.Error);
                    loop = false;
                }
            }
        }
        public bool Connected//获取该客户端的状态
        {
            get { return clientSocket.Connected; }
        }
        #endregion
        #region 设备属性
        private string _usbName = "No Name";
        public string usbName
        {
            get { return _usbName; }
            private set
            {
                _usbName = value;
                if (moduleList.Count > 0)
                    moduleList[0].Name = value;
            }
        }                 //joy名称
        public byte version1 = 0;
        public byte version2 = 0;
        public byte version3 = 0;
        public bool errorVersion = false;
        public LinkMode linkMode { private set; get; } = LinkMode.OffLine;
        public ushort PID { private set; get; }
        public ushort VID { private set; get; }
        public byte BackLightBrightness { private set; get; } = 0; //背光亮度
        public byte ColorOrder { private set; get; } = 0; //色彩排序
        public bool HC165 { private set; get; } = true; //HC165 or CD4021
        public bool TwoKeyMode { private set; get; } = true; //键盘双按键模式
        public bool USBPower { private set; get; } = true;
        public bool NeedPowerControl = false;
        public ColorInfoType IdleColor { private set; get; } = ColorInfoType.ColorDynamicRainbow;//待机颜色
        public UInt16 currentColorCount = 0;//颜色同步
        public bool keyBoardFN = false;
        public byte DynamicSpeed { private set; get; } = 10;//动态效果速度
        public byte currentChangePin { private set; get; } = 255; //当前编辑的设备
        public byte currentChangeFormat { private set; get; } = 255; //当前变化的format
        public bool KeyBoardOn { private set; get; } = false; //键盘开关
        public bool LicenseUID { private set; get; } = false; //是否注册
        public byte[] axisID { private set; get; } = new byte[JoyConst.MaxAxis];
        public string McuID { private set; get; } = ""; //机器码
        public byte[] licenseKey { private set; get; } = new byte[16]; //注册码
        #region LAN
        public byte[] MAC { private set; get; } = { 0x00, 0x08, 0xdc, 0x11, 0x11, 0x11 };
        /*定义默认IP信息*/
        public byte[] Local_IP { private set; get; } = { 192, 168, 0, 40 };  /*定义W5500默认IP地址*/
        public ushort Local_Port { private set; get; } = 5000;               /*定义本地端口*/
        public byte[] Subnet { private set; get; } = { 255, 255, 255, 0 };   /*定义W5500默认子网掩码*/
        public byte[] Gateway { private set; get; } = { 192, 168, 0, 1 };    /*定义W5500默认网关*/
        public byte[] DNS { private set; get; } = { 192, 168, 0, 1 }; /*定义W5500默认DNS*/
        /*定义远端IP信息*/
        public byte[] Remote_IP { private set; get; } = { 192, 168, 0, 80 }; /*远端IP地址*/
        public ushort Remote_Port { private set; get; } = 8080;              /*远端端口号*/
        #endregion
        #region MaxADC
        private byte _maxADC = 0;
        public List<PortShowType> adcUsed { get; private set; } = new List<PortShowType>();
        public byte joyMaxADC
        {
            get { return _maxADC; }
            set
            {
                if (_maxADC == value)
                    return;
                _maxADC = value;
                adcUsed.Clear();
                for (int i = 0; i < JoyConst.MaxADC; i++)
                {
                    if (i < _maxADC)
                        adcUsed.Add(PortShowType.Used);
                    else
                        adcUsed.Add(PortShowType.Error);
                }
            }
        }
        #endregion
        #region MaxHall
        private byte _maxHall = 0;
        public List<PortShowType> hallUsed { get; private set; } = new List<PortShowType>();
        public byte joyMaxHall
        {
            get { return _maxHall; }
            set
            {
                if (_maxHall == value)
                    return;
                _maxHall = value;
                hallUsed.Clear();
                for (int i = 0; i < JoyConst.MaxHall; i++)
                {
                    if (i < _maxHall)
                        hallUsed.Add(PortShowType.Used);
                    else
                        hallUsed.Add(PortShowType.Error);
                }
            }
        }
        #endregion
        #region MaxPWM
        private byte _maxPWM = 0;
        public List<PortShowType> pwmUsed { get; private set; } = new List<PortShowType>();
        public byte joyMaxPWM
        {
            get { return _maxPWM; }
            set
            {
                if (_maxPWM == value)
                    return;
                _maxPWM = value;
                pwmUsed.Clear();
                for (int i = 0; i < JoyConst.MaxPWM; i++)
                {
                    if (i < _maxPWM)
                        pwmUsed.Add(PortShowType.Used);
                    else
                        pwmUsed.Add(PortShowType.Error);
                }
            }
        }
        #endregion
        #region MaxPin
        private byte _maxPin = 0;
        public List<PortShowType> pinUsed { get; private set; } = new List<PortShowType>();
        public byte joyMaxPin
        {
            get { return _maxPin; }
            set
            {
                if (_maxPin == value)
                    return;
                _maxPin = value;
                pinUsed.Clear();
                for (int i = 0; i < JoyConst.MaxPin; i++)
                {
                    if (i < _maxPin)
                        pinUsed.Add(PortShowType.Used);
                    else
                        pinUsed.Add(PortShowType.Error);
                }
            }
        }
        #endregion
        public string keyID { get { return version1.ToString() + version2.ToString() + "." + McuID; } }
        public byte[] pinValue { private set; get; } = new byte[JoyConst.MaxPin / 8];//pin实时数据
        public byte[] ledControlReport { private set; get; } = new byte[JoyConst.MaxLed / 8];//led软件控制数据
        public byte[] pwmControlReport { private set; get; } = new byte[JoyConst.MaxPWM];//pwm软件控制数据
        public byte[] buttonControlReport { private set; get; } = new byte[JoyConst.MaxButton / 8];//按钮软件控制
        public int[] axisControlReport { private set; get; } = new int[JoyConst.MaxAxis];//轴软件控制

        public int inFormatValue = 0;
        public int inFormatMax = 1;
        public int outFormatValue = 0;
        public int outFormatMax = 1;

        private Adc[] adcList = new Adc[JoyConst.MaxADC];//adc
        private Button[] btnList = new Button[JoyConst.MaxButton];//keyboard
        private Format[] formatList = new Format[JoyConst.MaxFormat];//轴格式化
        private Hall[] hallList = new Hall[JoyConst.MaxHat];//霍尔
        private Hat[] hatList = new Hat[JoyConst.MaxHat];//苦力帽
        private Led[] ledList = new Led[JoyConst.MaxLed];//Led
        private ColorInfo[] colorInfoList = new ColorInfo[JoyConst.MaxLed];//ColorInfo
        //-----
        private JoyDevice[] deviceList = new JoyDevice[JoyConst.MaxDevice];
        private JoyCustom[] customList = new JoyCustom[JoyConst.MaxCustom];
        //-----
        public byte[] customDataList = new byte[JoyConst.MaxCustomData];
        public byte currentMidSpeedData = 0;
        public byte currentLowSpeedData = 0;
        public byte[] softDataList = new byte[JoyConst.MaxSoftData];
        //--------------------------------------------------------------------
        public byte maxOutJoystick { private set; get; } = 1;
        public byte maxOutAxis { private set; get; } = 0;
        public byte maxOutHat { private set; get; } = 0;
        //--------------------------------------------------------------------
        public Dictionary<DeviceType, bool> v31_DeviceOpen = new Dictionary<DeviceType, bool>();
        public Dictionary<DeviceType, bool> v35_DeviceOpen = new Dictionary<DeviceType, bool>();
        public Dictionary<DeviceType, bool> vKB_DeviceOpen = new Dictionary<DeviceType, bool>();
        public Dictionary<DeviceType, bool> v4B_DeviceOpen = new Dictionary<DeviceType, bool>();
        public Dictionary<DeviceType, bool> v4P_DeviceOpen = new Dictionary<DeviceType, bool>();
        //
        public Dictionary<CustomType, bool> v31_CustomOpen = new Dictionary<CustomType, bool>();
        public Dictionary<CustomType, bool> v35_CustomOpen = new Dictionary<CustomType, bool>();
        public Dictionary<CustomType, bool> vKB_CustomOpen = new Dictionary<CustomType, bool>();
        public Dictionary<CustomType, bool> v4B_CustomOpen = new Dictionary<CustomType, bool>();
        public Dictionary<CustomType, bool> v4P_CustomOpen = new Dictionary<CustomType, bool>();
        //
        string tempSave = System.Environment.CurrentDirectory + @"\Ejoy\ejoy.xml";
        //========================================
        #endregion
        #region 字库编辑
        eFont[] fontList = new eFont[JoyConst.MaxFont];
        private byte[] tempFontLibrary = new byte[JoyConst.MaxFontByteLengh];
        public bool GetFontLibrary(eFont font, int charIndex, int byteIndex, out byte data)
        {
            byte temp;
            if (font.GetFontByte(font.FontSize * charIndex + byteIndex, out temp))
            {
                data = temp;
                return true;
            }
            data = 0;
            return false;
        }
        public byte[] GetFontLibraryArray()
        {
            byte[] fontLibrary = new byte[JoyConst.MaxFontByteLengh];
            int startIndex = 0;
            for (int i = 0; i < fontList.Length; i++)
            {
                eFont font = fontList[i];
                font.StartIndex = (UInt16)startIndex;
                byte[] fontArray = font.GetFontByteArray();
                for (int fontIndex = 0; fontIndex < fontArray.Length; fontIndex++)
                {
                    int libIndex = startIndex + fontIndex;
                    if (libIndex >= 0 && libIndex < JoyConst.MaxFontLibLengh)
                    {
                        fontLibrary[libIndex] = fontArray[fontIndex];
                    }
                }
                startIndex += font.MaxCharLength;
                fontLibrary[JoyConst.MaxFontLibLengh + i * JoyConst.MaxFontData] = (byte)fontList[i].StartIndex;
                fontLibrary[JoyConst.MaxFontLibLengh + i * JoyConst.MaxFontData + 1] = (byte)(fontList[i].StartIndex >> 8);
                fontLibrary[JoyConst.MaxFontLibLengh + i * JoyConst.MaxFontData + 2] = fontList[i].StartChar;
                fontLibrary[JoyConst.MaxFontLibLengh + i * JoyConst.MaxFontData + 3] = fontList[i].EndChar;
                fontLibrary[JoyConst.MaxFontLibLengh + i * JoyConst.MaxFontData + 4] = fontList[i].FontWidth;
                fontLibrary[JoyConst.MaxFontLibLengh + i * JoyConst.MaxFontData + 5] = fontList[i].FontHeight;
            }
            return fontLibrary;
        }
        public bool SetFontLibrary(eFont font, int charIndex, int byteIndex, byte data)
        {
            if (font.SetFontByte(font.FontSize * charIndex + byteIndex, data))
            {
                return true;
            }
            return false;
        }
        public void SetFontLibraryType(eFont target, eFont from)
        {
            target.FontWidth = from.FontWidth;
            target.FontHeight = from.FontHeight;
            target.StartChar = from.StartChar;
            target.EndChar = from.EndChar;
            byte[] arrayFont = from.GetFontByteArrayAll();
            for (int i = 0; i < arrayFont.Length; i++)
            {
                target.SetFontByte(i, arrayFont[i]);
            }
        }
        public Color4 FontColor = XmlUI.DxDeviceRed;
        #endregion
        #region fps
        public int msTims = 0;
        public int fps = 0;
        public int showFps = 0;
        #endregion
        //========================================
        private TypeSwitch _TypeSwitchControl = TypeSwitch.DeviceSwitch;
        public TypeSwitch TypeSwitchControl
        #region TypeSwitchControl
        {
            get
            {
                return _TypeSwitchControl;
            }
            set
            {
                if (_TypeSwitchControl != value)
                {
                    _TypeSwitchControl = value;
                }
            }
        }
        #endregion
        //-------------------------------------------------------------------
        private int _selectDevice = 0;
        public int SelectDevice
        #region  SelectDevice
        {
            get
            {
                return _selectDevice;
            }
            set
            {
                if (_selectDevice != value)
                {
                    _selectDevice = value;
                    JoyDevice dev = GetJoyDevice(_selectDevice);
                    if (dev != null)
                    {
                        switch (dev.portInType)
                        {
                            case InPortType.Pin:
                                break;
                            case InPortType.ADC:
                                break;
                            case InPortType.Hall:
                                break;
                            case InPortType.FormatOut:
                                break;
                        }
                        switch (dev.portOutType)
                        {
                            case OutPortType.Button:
                                SelectButton = dev.outPort;
                                break;
                            case OutPortType.Axis:
                                SelectAxis = dev.outPort;
                                break;
                            case OutPortType.Hat:
                                SelectHat = dev.outPort;
                                break;
                            case OutPortType.PWM:
                                SelectPwm = dev.outPort;
                                break;
                            case OutPortType.FormatIn:
                                SelectFormatIn = dev.outPort;
                                break;
                            case OutPortType.DataOut:
                                break;
                            case OutPortType.Mouse:
                                break;
                        }
                        switch (dev.portLedType)
                        {
                            case LedPortType.Led:
                            case LedPortType.Brightness:
                                SelectLed = dev.ledPort;
                                break;
                        }
                    }
                }
            }
        }
        #endregion
        //选中的按键
        private int _selectButton = 0;
        public int SelectButton
        #region SelectButton
        {
            get
            {
                return _selectButton;
            }
            set
            {
                if (_selectButton != value)
                {
                    _selectButton = value;
                }
            }
        }
        #endregion
        //选中的苦力帽
        private int _selectHat = 0;
        public int SelectHat
        #region SelectHat
        {
            get
            {
                return _selectHat;
            }
            set
            {
                if (_selectHat != value)
                {
                    _selectHat = value;
                }
            }
        }
        #endregion
        //选中的轴
        private int _selectAxis = 0;
        public int SelectAxis
        #region SelectAxis
        {
            get
            {
                return _selectAxis;
            }
            set
            {
                if (_selectAxis != value)
                {
                    _selectAxis = value;
                }
            }
        }
        #endregion
        //选中的校准
        private int _selectFormatIn = 0;
        public int SelectFormatIn
        #region SelectFormat
        {
            get
            {
                return _selectFormatIn;
            }
            set
            {
                if (_selectFormatIn != value)
                {
                    _selectFormatIn = value;
                }
            }
        }
        #endregion
        //选中的LED
        private int _selectLed = 0;
        public int SelectLed
        #region SelectLed
        {
            get
            {
                return _selectLed;
            }
            set
            {
                if (_selectLed != value)
                {
                    _selectLed = value;
                }
            }
        }
        #endregion
        //选中的PWM
        private int _selectPwm = 0;
        public int SelectPwm
        #region SelectPwm
        {
            get
            {
                return _selectPwm;
            }
            set
            {
                if (_selectPwm != value)
                {
                    _selectPwm = value;
                }
            }
        }
        #endregion
        //-------------------------------------------------------------------
        private int _selectCustom = 0;
        public int SelectCustom
        #region SelectCustom
        {
            get
            {
                return _selectCustom;
            }
            set
            {
                if (_selectCustom != value)
                {
                    _selectCustom = value;
                }
            }
        }
        #endregion
        private int _selectCustomPin = 0;
        public int SelectCustomPin
        #region SelectCustomPin
        {
            get
            {
                return _selectCustomPin;
            }
            set
            {
                if (_selectCustomPin != value)
                {
                    _selectCustomPin = value;
                }
            }
        }
        #endregion
        //-------------------------------------------------------------------
        private int _selectFontSet = 0;
        public int SelectFontSet
        #region SelectFontSet
        {
            get
            {
                return _selectFontSet;
            }
            set
            {
                if (_selectFontSet != value)
                {
                    _selectFontSet = value;
                }
            }
        }
        #endregion
        private int _selectFont = 0;
        public int SelectFont
        #region SelectFont
        {
            get
            {
                return _selectFont;
            }
            set
            {
                if (_selectFont != value)
                {
                    eFont font = GetFont(value);
                    if (font != null)
                    {
                        _selectFont = value;
                    }
                }
            }
        }
        #endregion
        //========================================
        public JoyObject(int _Index, string _key, string _name, string _ver)
        {
            linkType = LinkType.USB;
            reportMgr = new ReportManager(this);
            Index = _Index;
            McuID = _key;
            usbName = _name;
            HardwareVersion = _ver;
            ReSet();
        }
        public JoyObject(int _Index, Socket _socket)
        {
            linkType = LinkType.LAN;
            reportMgr = new ReportManager(this);
            Index = _Index;
            clientSocket = _socket;
            ip = (clientSocket.RemoteEndPoint as IPEndPoint).Address.ToString();//获取客户端的ip
            ReSet();
        }
        private void ReSet()
        {
            #region 设备列表
            for (int i = 0; i < JoyConst.MaxDevice; i++)
            {
                deviceList[i] = new JoyDevice(this, i);
            }
            #endregion
            #region 自定义列表
            for (int i = 0; i < JoyConst.MaxCustom; i++)
            {
                customList[i] = new JoyCustom(this, i);
            }
            #endregion
            #region 子类型
            for (int i = 0; i < JoyConst.MaxADC; i++)
            {
                adcList[i] = new Adc(i);
            }
            for (int i = 0; i < JoyConst.MaxButton; i++)
            {
                btnList[i] = new Button(i);
            }
            for (int i = 0; i < JoyConst.MaxFormat; i++)
            {
                formatList[i] = new Format(i);
            }
            for (int i = 0; i < JoyConst.MaxAxis; i++)
            {
                axisID[i] = (byte)(0x30 + i);
            }
            for (int i = 0; i < JoyConst.MaxHall; i++)
            {
                hallList[i] = new Hall(i);
            }
            for (int i = 0; i < JoyConst.MaxHat; i++)
            {
                hatList[i] = new Hat(i);
            }
            for (int i = 0; i < JoyConst.MaxLed; i++)
            {
                ledList[i] = new Led(i);
                colorInfoList[i] = new ColorInfo(i);
            }
            #endregion
            #region CustomData
            for (byte i = 0; i < JoyConst.MaxCustomData; i++)
            {
                customDataList[i] = 0;
            }
            #endregion
            #region 字库
            for (int i = 0; i < JoyConst.MaxFont; i++)
            {
                fontList[i] = new eFont();
            }
            #endregion
            #region 外设数量
            joyMaxADC = JoyConst.MaxADC;
            joyMaxHall = JoyConst.MaxHall;
            joyMaxPin = JoyConst.MaxPin;
            joyMaxPWM = JoyConst.MaxPWM;
            #endregion
            #region 开关
            v31_DeviceOpen.Clear();
            v35_DeviceOpen.Clear();
            vKB_DeviceOpen.Clear();
            v4B_DeviceOpen.Clear();
            v4P_DeviceOpen.Clear();
            //
            v31_CustomOpen.Clear();
            v35_CustomOpen.Clear();
            vKB_CustomOpen.Clear();
            v4B_CustomOpen.Clear();
            v4P_CustomOpen.Clear();
            #region v31
            v31_DeviceOpen.Add(DeviceType.None, true);
            v31_DeviceOpen.Add(DeviceType.SB_Normal, true);
            v31_DeviceOpen.Add(DeviceType.SB_Lock, true);
            v31_DeviceOpen.Add(DeviceType.SB_OnPulse, true);
            v31_DeviceOpen.Add(DeviceType.SB_OffPulse, true);
            v31_DeviceOpen.Add(DeviceType.SB_AllPulse, true);
            v31_DeviceOpen.Add(DeviceType.SB_Turbo, true);
            v31_DeviceOpen.Add(DeviceType.SB_Soft, true);
            v31_DeviceOpen.Add(DeviceType.SB_SoftSwitch, true);
            v31_DeviceOpen.Add(DeviceType.SB_ModeSwitch, true);
            v31_DeviceOpen.Add(DeviceType.SB_ModeClick, true);
            v31_DeviceOpen.Add(DeviceType.SB_MultiMode, true);
            v31_DeviceOpen.Add(DeviceType.SB_CombinedAxisSwitch, true);
            v31_DeviceOpen.Add(DeviceType.SB_MouseLeft, true);
            v31_DeviceOpen.Add(DeviceType.SB_MouseRight, true);
            v31_DeviceOpen.Add(DeviceType.SB_MouseMiddle, true);
            v31_DeviceOpen.Add(DeviceType.SB_RKJX, true);
            v31_DeviceOpen.Add(DeviceType.MB_Switch2, true);
            v31_DeviceOpen.Add(DeviceType.MB_Switch2_Pulse, true);
            v31_DeviceOpen.Add(DeviceType.MB_SoftSwitch2, true);
            v31_DeviceOpen.Add(DeviceType.MB_Switch2ModeSwitch, true);
            v31_DeviceOpen.Add(DeviceType.MB_Switch3, true);
            v31_DeviceOpen.Add(DeviceType.MB_Switch3_Pulse, true);
            v31_DeviceOpen.Add(DeviceType.MB_SoftSwitch3, true);
            v31_DeviceOpen.Add(DeviceType.MB_Switch3ModeSwitch, true);
            v31_DeviceOpen.Add(DeviceType.MB_Band, true);
            v31_DeviceOpen.Add(DeviceType.MB_Band_Pulse, true);
            v31_DeviceOpen.Add(DeviceType.MB_SoftBand, true);
            v31_DeviceOpen.Add(DeviceType.MB_BandModeSwitch, true);
            v31_DeviceOpen.Add(DeviceType.MB_Encode, true);
            v31_DeviceOpen.Add(DeviceType.MB_SoftEncode, true);
            v31_DeviceOpen.Add(DeviceType.MB_MultiModeEncode, true);
            v31_DeviceOpen.Add(DeviceType.MB_EncodeBand, true);
            v31_DeviceOpen.Add(DeviceType.MB_EncodeBand_Pulse, true);
            v31_DeviceOpen.Add(DeviceType.MB_EncodeCombinedAxis, true);
            v31_DeviceOpen.Add(DeviceType.MB_EncodeMouseX, true);
            v31_DeviceOpen.Add(DeviceType.MB_EncodeMouseY, true);
            v31_DeviceOpen.Add(DeviceType.MB_EncodeMouseWheel, true);
            v31_DeviceOpen.Add(DeviceType.MB_Hat, true);
            v31_DeviceOpen.Add(DeviceType.A_ADC, true);
            //v31_DeviceOpen.Add(DeviceType.H_TLE5010, true);
            //v31_DeviceOpen.Add(DeviceType.H_MLX90316, true);
            //v31_DeviceOpen.Add(DeviceType.H_MLX90333, true);
            //v31_DeviceOpen.Add(DeviceType.H_MLX90363, true);
            //v31_DeviceOpen.Add(DeviceType.H_MLX90393, true);
            //v31_DeviceOpen.Add(DeviceType.H_N35P112, true);
            //v31_DeviceOpen.Add(DeviceType.H_HX711, true);
            //v31_DeviceOpen.Add(DeviceType.H_HX717, true);
            v31_DeviceOpen.Add(DeviceType.F_Normal, true);
            v31_DeviceOpen.Add(DeviceType.F_CombinedAxis, true);
            v31_DeviceOpen.Add(DeviceType.F_ButtonMin, true);
            v31_DeviceOpen.Add(DeviceType.F_ButtonMax, true);
            //v31_DeviceOpen.Add(DeviceType.F_LedBrightness, true);
            v31_DeviceOpen.Add(DeviceType.F_Band, true);
            v31_DeviceOpen.Add(DeviceType.F_Trigger, true);
            v31_DeviceOpen.Add(DeviceType.F_Hat, true);
            v31_DeviceOpen.Add(DeviceType.F_Soft, true);
            v31_DeviceOpen.Add(DeviceType.F_MouseX, true);
            v31_DeviceOpen.Add(DeviceType.F_MouseY, true);
            //v31_DeviceOpen.Add(DeviceType.F_PWM, true);
            //v31_DeviceOpen.Add(DeviceType.Brightness_PWM, true);
            // v31_DeviceOpen.Add(DeviceType.Soft_PWM, true);
            v31_DeviceOpen.Add(DeviceType.Soft_Button, true);
            v31_DeviceOpen.Add(DeviceType.Soft_Axis, true);
            //v31_DeviceOpen.Add(DeviceType.Led_Only, true);
            #endregion
            #region v35
            v35_DeviceOpen.Add(DeviceType.None, true);
            v35_DeviceOpen.Add(DeviceType.SB_Normal, true);
            v35_DeviceOpen.Add(DeviceType.SB_Lock, true);
            v35_DeviceOpen.Add(DeviceType.SB_OnPulse, true);
            v35_DeviceOpen.Add(DeviceType.SB_OffPulse, true);
            v35_DeviceOpen.Add(DeviceType.SB_AllPulse, true);
            v35_DeviceOpen.Add(DeviceType.SB_Turbo, true);
            v35_DeviceOpen.Add(DeviceType.SB_Soft, true);
            v35_DeviceOpen.Add(DeviceType.SB_SoftSwitch, true);
            v35_DeviceOpen.Add(DeviceType.SB_ModeSwitch, true);
            v35_DeviceOpen.Add(DeviceType.SB_ModeClick, true);
            v35_DeviceOpen.Add(DeviceType.SB_MultiMode, true);
            v35_DeviceOpen.Add(DeviceType.SB_CombinedAxisSwitch, true);
            v35_DeviceOpen.Add(DeviceType.SB_MouseLeft, true);
            v35_DeviceOpen.Add(DeviceType.SB_MouseRight, true);
            v35_DeviceOpen.Add(DeviceType.SB_MouseMiddle, true);
            v35_DeviceOpen.Add(DeviceType.SB_RKJX, true);
            v35_DeviceOpen.Add(DeviceType.MB_Switch2, true);
            v35_DeviceOpen.Add(DeviceType.MB_Switch2_Pulse, true);
            v35_DeviceOpen.Add(DeviceType.MB_SoftSwitch2, true);
            v35_DeviceOpen.Add(DeviceType.MB_Switch2ModeSwitch, true);
            v35_DeviceOpen.Add(DeviceType.MB_Switch3, true);
            v35_DeviceOpen.Add(DeviceType.MB_Switch3_Pulse, true);
            v35_DeviceOpen.Add(DeviceType.MB_SoftSwitch3, true);
            v35_DeviceOpen.Add(DeviceType.MB_Switch3ModeSwitch, true);
            v35_DeviceOpen.Add(DeviceType.MB_Band, true);
            v35_DeviceOpen.Add(DeviceType.MB_Band_Pulse, true);
            v35_DeviceOpen.Add(DeviceType.MB_SoftBand, true);
            v35_DeviceOpen.Add(DeviceType.MB_BandModeSwitch, true);
            v35_DeviceOpen.Add(DeviceType.MB_Encode, true);
            v35_DeviceOpen.Add(DeviceType.MB_SoftEncode, true);
            v35_DeviceOpen.Add(DeviceType.MB_MultiModeEncode, true);
            v35_DeviceOpen.Add(DeviceType.MB_EncodeBand, true);
            v35_DeviceOpen.Add(DeviceType.MB_EncodeBand_Pulse, true);
            v35_DeviceOpen.Add(DeviceType.MB_EncodeCombinedAxis, true);
            v35_DeviceOpen.Add(DeviceType.MB_EncodeMouseX, true);
            v35_DeviceOpen.Add(DeviceType.MB_EncodeMouseY, true);
            v35_DeviceOpen.Add(DeviceType.MB_EncodeMouseWheel, true);
            v35_DeviceOpen.Add(DeviceType.MB_Hat, true);
            v35_DeviceOpen.Add(DeviceType.A_ADC, true);
            v35_DeviceOpen.Add(DeviceType.H_TLE5010, true);
            v35_DeviceOpen.Add(DeviceType.H_MLX90316, true);
            v35_DeviceOpen.Add(DeviceType.H_MLX90333, true);
            v35_DeviceOpen.Add(DeviceType.H_MLX90363, true);
            v35_DeviceOpen.Add(DeviceType.H_MLX90393, true);
            v35_DeviceOpen.Add(DeviceType.H_N35P112, true);
            v35_DeviceOpen.Add(DeviceType.H_HX711, true);
            v35_DeviceOpen.Add(DeviceType.H_HX717, true);
            v35_DeviceOpen.Add(DeviceType.F_Normal, true);
            v35_DeviceOpen.Add(DeviceType.F_CombinedAxis, true);
            v35_DeviceOpen.Add(DeviceType.F_ButtonMin, true);
            v35_DeviceOpen.Add(DeviceType.F_ButtonMax, true);
            v35_DeviceOpen.Add(DeviceType.F_LedBrightness, true);
            v35_DeviceOpen.Add(DeviceType.F_Band, true);
            v35_DeviceOpen.Add(DeviceType.F_Trigger, true);
            v35_DeviceOpen.Add(DeviceType.F_Hat, true);
            v35_DeviceOpen.Add(DeviceType.F_Soft, true);
            v35_DeviceOpen.Add(DeviceType.F_MouseX, true);
            v35_DeviceOpen.Add(DeviceType.F_MouseY, true);
            v35_DeviceOpen.Add(DeviceType.F_PWM, true);
            v35_DeviceOpen.Add(DeviceType.Brightness_PWM, true);
            v35_DeviceOpen.Add(DeviceType.Soft_PWM, true);
            v35_DeviceOpen.Add(DeviceType.Soft_Button, true);
            v35_DeviceOpen.Add(DeviceType.Soft_Axis, true);
            v35_DeviceOpen.Add(DeviceType.Led_Only, true);
            #endregion
            #region vKB
            vKB_DeviceOpen.Add(DeviceType.None, true);
            vKB_DeviceOpen.Add(DeviceType.SB_Normal, true);
            vKB_DeviceOpen.Add(DeviceType.SB_Lock, true);
            vKB_DeviceOpen.Add(DeviceType.SB_OnPulse, true);
            //vKB_DeviceOpen.Add(DeviceType.SB_OffPulse, true);
            //vKB_DeviceOpen.Add(DeviceType.SB_AllPulse, true);
            vKB_DeviceOpen.Add(DeviceType.SB_Turbo, true);
            //vKB_DeviceOpen.Add(DeviceType.SB_Soft, true);
            //vKB_DeviceOpen.Add(DeviceType.SB_SoftSwitch, true);
            vKB_DeviceOpen.Add(DeviceType.SB_ModeSwitch, true);
            vKB_DeviceOpen.Add(DeviceType.SB_ModeClick, true);
            vKB_DeviceOpen.Add(DeviceType.SB_MultiMode, true);
            //vKB_DeviceOpen.Add(DeviceType.SB_MouseLeft, true);
            //vKB_DeviceOpen.Add(DeviceType.SB_MouseRight, true);
            //vKB_DeviceOpen.Add(DeviceType.SB_MouseMiddle, true);
            //vKB_DeviceOpen.Add(DeviceType.SB_RKJX, true);
            //vKB_DeviceOpen.Add(DeviceType.MB_Switch2, true);
            //vKB_DeviceOpen.Add(DeviceType.MB_Switch2_Pulse, true);
            //vKB_DeviceOpen.Add(DeviceType.MB_SoftSwitch2, true);
            //vKB_DeviceOpen.Add(DeviceType.MB_Switch2ModeSwitch, true);
            //vKB_DeviceOpen.Add(DeviceType.MB_Switch3, true);
            //vKB_DeviceOpen.Add(DeviceType.MB_Switch3_Pulse, true);
            //vKB_DeviceOpen.Add(DeviceType.MB_SoftSwitch3, true);
            //vKB_DeviceOpen.Add(DeviceType.MB_Switch3ModeSwitch, true);
            //vKB_DeviceOpen.Add(DeviceType.MB_Band, true);
            //vKB_DeviceOpen.Add(DeviceType.MB_Band_Pulse, true);
            //vKB_DeviceOpen.Add(DeviceType.MB_SoftBand, true);
            vKB_DeviceOpen.Add(DeviceType.MB_BandModeSwitch, true);
            vKB_DeviceOpen.Add(DeviceType.MB_Encode, true);
            //vKB_DeviceOpen.Add(DeviceType.MB_SoftEncode, true);
            vKB_DeviceOpen.Add(DeviceType.MB_MultiModeEncode, true);
            //vKB_DeviceOpen.Add(DeviceType.MB_EncodeBand, true);
            //vKB_DeviceOpen.Add(DeviceType.MB_EncodeBand_Pulse, true);
            //vKB_DeviceOpen.Add(DeviceType.MB_EncodeMouseX, true);
            //vKB_DeviceOpen.Add(DeviceType.MB_EncodeMouseY, true);
            //vKB_DeviceOpen.Add(DeviceType.MB_EncodeMouseWheel, true);
            //vKB_DeviceOpen.Add(DeviceType.MB_Hat, true);
            //vKB_DeviceOpen.Add(DeviceType.A_ADC, true);
            //vKB_DeviceOpen.Add(DeviceType.H_TLE5010, true);
            //vKB_DeviceOpen.Add(DeviceType.H_MLX90316, true);
            //vKB_DeviceOpen.Add(DeviceType.H_MLX90333, true);
            //vKB_DeviceOpen.Add(DeviceType.H_MLX90363, true);
            //vKB_DeviceOpen.Add(DeviceType.H_MLX90393, true);
            //vKB_DeviceOpen.Add(DeviceType.H_N35P112, true);
            //vKB_DeviceOpen.Add(DeviceType.H_HX711, true);
            //vKB_DeviceOpen.Add(DeviceType.H_HX717, true);
            //vKB_DeviceOpen.Add(DeviceType.F_Normal, true);
            //vKB_DeviceOpen.Add(DeviceType.F_ButtonMin, true);
            //vKB_DeviceOpen.Add(DeviceType.F_ButtonMax, true);
            //vKB_DeviceOpen.Add(DeviceType.F_LedBrightness, true);
            //vKB_DeviceOpen.Add(DeviceType.F_Band, true);
            //vKB_DeviceOpen.Add(DeviceType.F_Trigger, true);
            //vKB_DeviceOpen.Add(DeviceType.F_Hat, true);
            //vKB_DeviceOpen.Add(DeviceType.F_Soft, true);
            //vKB_DeviceOpen.Add(DeviceType.F_MouseX, true);
            //vKB_DeviceOpen.Add(DeviceType.F_MouseY, true);
            //vKB_DeviceOpen.Add(DeviceType.F_PWM, true);
            //vKB_DeviceOpen.Add(DeviceType.Brightness_PWM, true);
            //vKB_DeviceOpen.Add(DeviceType.Soft_PWM, true);
            //vKB_DeviceOpen.Add(DeviceType.Soft_Button, true);
            //vKB_DeviceOpen.Add(DeviceType.Soft_Axis, true);
            vKB_DeviceOpen.Add(DeviceType.Led_Only, true);
            #endregion
            #region v4b
            v4B_DeviceOpen.Add(DeviceType.None, true);
            v4B_DeviceOpen.Add(DeviceType.SB_Normal, true);
            v4B_DeviceOpen.Add(DeviceType.SB_Lock, true);
            v4B_DeviceOpen.Add(DeviceType.SB_OnPulse, true);
            v4B_DeviceOpen.Add(DeviceType.SB_OffPulse, true);
            v4B_DeviceOpen.Add(DeviceType.SB_AllPulse, true);
            v4B_DeviceOpen.Add(DeviceType.SB_Turbo, true);
            v4B_DeviceOpen.Add(DeviceType.SB_Soft, true);
            v4B_DeviceOpen.Add(DeviceType.SB_SoftSwitch, true);
            v4B_DeviceOpen.Add(DeviceType.SB_ModeSwitch, true);
            v4B_DeviceOpen.Add(DeviceType.SB_ModeClick, true);
            v4B_DeviceOpen.Add(DeviceType.SB_MultiMode, true);
            v4B_DeviceOpen.Add(DeviceType.SB_CombinedAxisSwitch, true);
            v4B_DeviceOpen.Add(DeviceType.SB_MouseLeft, true);
            v4B_DeviceOpen.Add(DeviceType.SB_MouseRight, true);
            v4B_DeviceOpen.Add(DeviceType.SB_MouseMiddle, true);
            v4B_DeviceOpen.Add(DeviceType.SB_RKJX, true);
            v4B_DeviceOpen.Add(DeviceType.MB_Switch2, true);
            v4B_DeviceOpen.Add(DeviceType.MB_Switch2_Pulse, true);
            v4B_DeviceOpen.Add(DeviceType.MB_SoftSwitch2, true);
            v4B_DeviceOpen.Add(DeviceType.MB_Switch2ModeSwitch, true);
            v4B_DeviceOpen.Add(DeviceType.MB_Switch3, true);
            v4B_DeviceOpen.Add(DeviceType.MB_Switch3_Pulse, true);
            v4B_DeviceOpen.Add(DeviceType.MB_SoftSwitch3, true);
            v4B_DeviceOpen.Add(DeviceType.MB_Switch3ModeSwitch, true);
            v4B_DeviceOpen.Add(DeviceType.MB_Band, true);
            v4B_DeviceOpen.Add(DeviceType.MB_Band_Pulse, true);
            v4B_DeviceOpen.Add(DeviceType.MB_SoftBand, true);
            v4B_DeviceOpen.Add(DeviceType.MB_BandModeSwitch, true);
            v4B_DeviceOpen.Add(DeviceType.MB_Encode, true);
            v4B_DeviceOpen.Add(DeviceType.MB_SoftEncode, true);
            v4B_DeviceOpen.Add(DeviceType.MB_MultiModeEncode, true);
            v4B_DeviceOpen.Add(DeviceType.MB_EncodeBand, true);
            v4B_DeviceOpen.Add(DeviceType.MB_EncodeBand_Pulse, true);
            v4B_DeviceOpen.Add(DeviceType.MB_EncodeCombinedAxis, true);
            v4B_DeviceOpen.Add(DeviceType.MB_EncodeMouseX, true);
            v4B_DeviceOpen.Add(DeviceType.MB_EncodeMouseY, true);
            v4B_DeviceOpen.Add(DeviceType.MB_EncodeMouseWheel, true);
            v4B_DeviceOpen.Add(DeviceType.MB_Hat, true);
            v4B_DeviceOpen.Add(DeviceType.A_ADC, true);
            v4B_DeviceOpen.Add(DeviceType.H_TLE5010, true);
            v4B_DeviceOpen.Add(DeviceType.H_MLX90316, true);
            v4B_DeviceOpen.Add(DeviceType.H_MLX90333, true);
            v4B_DeviceOpen.Add(DeviceType.H_MLX90363, true);
            v4B_DeviceOpen.Add(DeviceType.H_MLX90393, true);
            v4B_DeviceOpen.Add(DeviceType.H_N35P112, true);
            v4B_DeviceOpen.Add(DeviceType.H_HX711, true);
            v4B_DeviceOpen.Add(DeviceType.H_HX717, true);
            v4B_DeviceOpen.Add(DeviceType.F_Normal, true);
            v4B_DeviceOpen.Add(DeviceType.F_CombinedAxis, true);
            v4B_DeviceOpen.Add(DeviceType.F_ButtonMin, true);
            v4B_DeviceOpen.Add(DeviceType.F_ButtonMax, true);
            v4B_DeviceOpen.Add(DeviceType.F_LedBrightness, true);
            v4B_DeviceOpen.Add(DeviceType.F_Band, true);
            v4B_DeviceOpen.Add(DeviceType.F_Trigger, true);
            v4B_DeviceOpen.Add(DeviceType.F_Hat, true);
            v4B_DeviceOpen.Add(DeviceType.F_Soft, true);
            v4B_DeviceOpen.Add(DeviceType.F_MouseX, true);
            v4B_DeviceOpen.Add(DeviceType.F_MouseY, true);
            v4B_DeviceOpen.Add(DeviceType.F_PWM, true);
            v4B_DeviceOpen.Add(DeviceType.Brightness_PWM, true);
            v4B_DeviceOpen.Add(DeviceType.Soft_PWM, true);
            v4B_DeviceOpen.Add(DeviceType.Soft_Button, true);
            v4B_DeviceOpen.Add(DeviceType.Soft_Axis, true);
            v4B_DeviceOpen.Add(DeviceType.Led_Only, true);
            //==============================================
            v4B_CustomOpen.Add(CustomType.NoneCustom, true);
            //--------------------------数码管
            v4B_CustomOpen.Add(CustomType.DT_Max7219, true);
            v4B_CustomOpen.Add(CustomType.DT_TM1638, true);
            v4B_CustomOpen.Add(CustomType.DT_HT16K33, true);
            //--------------------------显示字符
            v4B_CustomOpen.Add(CustomType.Matrix_Max7219, true);
            //----
            v4B_CustomOpen.Add(CustomType.OLED_70_40_SSD1306, true);//0.42
            v4B_CustomOpen.Add(CustomType.OLED_70_40_SSD1306x2, true);
            v4B_CustomOpen.Add(CustomType.OLED_70_40_SSD1306x3, true);
            v4B_CustomOpen.Add(CustomType.OLED_70_40_SSD1306x4, true);
            //----
            v4B_CustomOpen.Add(CustomType.OLED_48_64_SSD1306, true);//0.71
            v4B_CustomOpen.Add(CustomType.OLED_48_64_SSD1306x2, true);
            v4B_CustomOpen.Add(CustomType.OLED_48_64_SSD1306x3, true);
            v4B_CustomOpen.Add(CustomType.OLED_48_64_SSD1306x4, true);
            //----
            v4B_CustomOpen.Add(CustomType.OLED_64_32_SSD1306, true);//0.49
            v4B_CustomOpen.Add(CustomType.OLED_64_32_SSD1306x2, true);
            v4B_CustomOpen.Add(CustomType.OLED_64_32_SSD1306x3, true);
            v4B_CustomOpen.Add(CustomType.OLED_64_32_SSD1306x4, true);
            //----
            v4B_CustomOpen.Add(CustomType.OLED_64_48_SSD1306, true);//0.66
            v4B_CustomOpen.Add(CustomType.OLED_64_48_SSD1306x2, true);
            v4B_CustomOpen.Add(CustomType.OLED_64_48_SSD1306x3, true);
            v4B_CustomOpen.Add(CustomType.OLED_64_48_SSD1306x4, true);
            //----
            v4B_CustomOpen.Add(CustomType.OLED_96_16_SSD1306, true);//0.86
            v4B_CustomOpen.Add(CustomType.OLED_96_16_SSD1306x2, true);
            v4B_CustomOpen.Add(CustomType.OLED_96_16_SSD1306x3, true);
            v4B_CustomOpen.Add(CustomType.OLED_96_16_SSD1306x4, true);
            //----
            v4B_CustomOpen.Add(CustomType.OLED_128_32_SSD1306, true);//0.91
            v4B_CustomOpen.Add(CustomType.OLED_128_32_SSD1306x2, true);
            v4B_CustomOpen.Add(CustomType.OLED_128_32_SSD1306x3, true);
            v4B_CustomOpen.Add(CustomType.OLED_128_32_SSD1306x4, true);
            //----
            v4B_CustomOpen.Add(CustomType.OLED_128_64_SSD1306, true);//0.96, 1.09
            v4B_CustomOpen.Add(CustomType.OLED_128_64_SSD1306x2, true);
            v4B_CustomOpen.Add(CustomType.OLED_128_64_SSD1306x3, true);
            v4B_CustomOpen.Add(CustomType.OLED_128_64_SSD1306x4, true);
            //----
            v4B_CustomOpen.Add(CustomType.OLED_128_64_SH1106, true);//1.3
            v4B_CustomOpen.Add(CustomType.OLED_128_64_SH1106x2, true);
            v4B_CustomOpen.Add(CustomType.OLED_128_64_SH1106x3, true);
            v4B_CustomOpen.Add(CustomType.OLED_128_64_SH1106x4, true);
            //----
            v4B_CustomOpen.Add(CustomType.OLED_128_88_SH1107, true);//0.73
            v4B_CustomOpen.Add(CustomType.OLED_128_88_SH1107x2, true);
            v4B_CustomOpen.Add(CustomType.OLED_128_88_SH1107x3, true);
            v4B_CustomOpen.Add(CustomType.OLED_128_88_SH1107x4, true);
            //----
            v4B_CustomOpen.Add(CustomType.OLED_256_64_SSD1322, true);//3.12
            v4B_CustomOpen.Add(CustomType.OLED_256_64_SSD1322x2, true);
            v4B_CustomOpen.Add(CustomType.OLED_256_64_SSD1322x3, true);
            v4B_CustomOpen.Add(CustomType.OLED_256_64_SSD1322x4, true);
            //--------------------------步进控制
            v4B_CustomOpen.Add(CustomType.OUT_StepperMotor, true);
            //--------------------------单点输出
            v4B_CustomOpen.Add(CustomType.OUT_74HC595, true);
            v4B_CustomOpen.Add(CustomType.OUT_IO, true);
            //--------------------------无线模块
            v4B_CustomOpen.Add(CustomType.OUT_NRF24, true);
            //--------------------------网络模块
            v4B_CustomOpen.Add(CustomType.OUT_W5500, true);
            #endregion
            #region v4p
            v4P_DeviceOpen.Add(DeviceType.None, true);
            v4P_DeviceOpen.Add(DeviceType.SB_Normal, true);
            v4P_DeviceOpen.Add(DeviceType.SB_Lock, true);
            v4P_DeviceOpen.Add(DeviceType.SB_OnPulse, true);
            v4P_DeviceOpen.Add(DeviceType.SB_OffPulse, true);
            v4P_DeviceOpen.Add(DeviceType.SB_AllPulse, true);
            v4P_DeviceOpen.Add(DeviceType.SB_Turbo, true);
            v4P_DeviceOpen.Add(DeviceType.SB_Soft, true);
            v4P_DeviceOpen.Add(DeviceType.SB_SoftSwitch, true);
            v4P_DeviceOpen.Add(DeviceType.SB_ModeSwitch, true);
            v4P_DeviceOpen.Add(DeviceType.SB_ModeClick, true);
            v4P_DeviceOpen.Add(DeviceType.SB_MultiMode, true);
            v4P_DeviceOpen.Add(DeviceType.SB_CombinedAxisSwitch, true);
            v4P_DeviceOpen.Add(DeviceType.SB_MouseLeft, true);
            v4P_DeviceOpen.Add(DeviceType.SB_MouseRight, true);
            v4P_DeviceOpen.Add(DeviceType.SB_MouseMiddle, true);
            v4P_DeviceOpen.Add(DeviceType.SB_RKJX, true);
            v4P_DeviceOpen.Add(DeviceType.MB_Switch2, true);
            v4P_DeviceOpen.Add(DeviceType.MB_Switch2_Pulse, true);
            v4P_DeviceOpen.Add(DeviceType.MB_SoftSwitch2, true);
            v4P_DeviceOpen.Add(DeviceType.MB_Switch2ModeSwitch, true);
            v4P_DeviceOpen.Add(DeviceType.MB_Switch3, true);
            v4P_DeviceOpen.Add(DeviceType.MB_Switch3_Pulse, true);
            v4P_DeviceOpen.Add(DeviceType.MB_SoftSwitch3, true);
            v4P_DeviceOpen.Add(DeviceType.MB_Switch3ModeSwitch, true);
            v4P_DeviceOpen.Add(DeviceType.MB_Band, true);
            v4P_DeviceOpen.Add(DeviceType.MB_Band_Pulse, true);
            v4P_DeviceOpen.Add(DeviceType.MB_SoftBand, true);
            v4P_DeviceOpen.Add(DeviceType.MB_BandModeSwitch, true);
            v4P_DeviceOpen.Add(DeviceType.MB_Encode, true);
            v4P_DeviceOpen.Add(DeviceType.MB_SoftEncode, true);
            v4P_DeviceOpen.Add(DeviceType.MB_MultiModeEncode, true);
            v4P_DeviceOpen.Add(DeviceType.MB_EncodeBand, true);
            v4P_DeviceOpen.Add(DeviceType.MB_EncodeBand_Pulse, true);
            v4P_DeviceOpen.Add(DeviceType.MB_EncodeCombinedAxis, true);
            v4P_DeviceOpen.Add(DeviceType.MB_EncodeMouseX, true);
            v4P_DeviceOpen.Add(DeviceType.MB_EncodeMouseY, true);
            v4P_DeviceOpen.Add(DeviceType.MB_EncodeMouseWheel, true);
            v4P_DeviceOpen.Add(DeviceType.MB_Hat, true);
            v4P_DeviceOpen.Add(DeviceType.A_ADC, true);
            v4P_DeviceOpen.Add(DeviceType.H_TLE5010, true);
            v4P_DeviceOpen.Add(DeviceType.H_MLX90316, true);
            v4P_DeviceOpen.Add(DeviceType.H_MLX90333, true);
            v4P_DeviceOpen.Add(DeviceType.H_MLX90363, true);
            v4P_DeviceOpen.Add(DeviceType.H_MLX90393, true);
            v4P_DeviceOpen.Add(DeviceType.H_N35P112, true);
            v4P_DeviceOpen.Add(DeviceType.H_HX711, true);
            v4P_DeviceOpen.Add(DeviceType.H_HX717, true);
            v4P_DeviceOpen.Add(DeviceType.F_Normal, true);
            v4P_DeviceOpen.Add(DeviceType.F_CombinedAxis, true);
            v4P_DeviceOpen.Add(DeviceType.F_ButtonMin, true);
            v4P_DeviceOpen.Add(DeviceType.F_ButtonMax, true);
            v4P_DeviceOpen.Add(DeviceType.F_LedBrightness, true);
            v4P_DeviceOpen.Add(DeviceType.F_Band, true);
            v4P_DeviceOpen.Add(DeviceType.F_Trigger, true);
            v4P_DeviceOpen.Add(DeviceType.F_Hat, true);
            v4P_DeviceOpen.Add(DeviceType.F_Soft, true);
            v4P_DeviceOpen.Add(DeviceType.F_MouseX, true);
            v4P_DeviceOpen.Add(DeviceType.F_MouseY, true);
            v4P_DeviceOpen.Add(DeviceType.F_PWM, true);
            v4P_DeviceOpen.Add(DeviceType.Brightness_PWM, true);
            v4P_DeviceOpen.Add(DeviceType.Soft_PWM, true);
            v4P_DeviceOpen.Add(DeviceType.Soft_Button, true);
            v4P_DeviceOpen.Add(DeviceType.Soft_Axis, true);
            v4P_DeviceOpen.Add(DeviceType.Led_Only, true);
            //==============================================
            v4P_CustomOpen.Add(CustomType.NoneCustom, true);
            //--------------------------数码管
            v4P_CustomOpen.Add(CustomType.DT_Max7219, true);
            v4P_CustomOpen.Add(CustomType.DT_TM1638, true);
            v4P_CustomOpen.Add(CustomType.DT_HT16K33, true);
            //--------------------------显示字符
            v4P_CustomOpen.Add(CustomType.Matrix_Max7219, true);
            //----
            v4P_CustomOpen.Add(CustomType.OLED_70_40_SSD1306, true);//0.42
            v4P_CustomOpen.Add(CustomType.OLED_70_40_SSD1306x2, true);
            v4P_CustomOpen.Add(CustomType.OLED_70_40_SSD1306x3, true);
            v4P_CustomOpen.Add(CustomType.OLED_70_40_SSD1306x4, true);
            //----
            v4P_CustomOpen.Add(CustomType.OLED_48_64_SSD1306, true);//0.71
            v4P_CustomOpen.Add(CustomType.OLED_48_64_SSD1306x2, true);
            v4P_CustomOpen.Add(CustomType.OLED_48_64_SSD1306x3, true);
            v4P_CustomOpen.Add(CustomType.OLED_48_64_SSD1306x4, true);
            //----
            v4P_CustomOpen.Add(CustomType.OLED_64_32_SSD1306, true);//0.49
            v4P_CustomOpen.Add(CustomType.OLED_64_32_SSD1306x2, true);
            v4P_CustomOpen.Add(CustomType.OLED_64_32_SSD1306x3, true);
            v4P_CustomOpen.Add(CustomType.OLED_64_32_SSD1306x4, true);
            //----
            v4P_CustomOpen.Add(CustomType.OLED_64_48_SSD1306, true);//0.66
            v4P_CustomOpen.Add(CustomType.OLED_64_48_SSD1306x2, true);
            v4P_CustomOpen.Add(CustomType.OLED_64_48_SSD1306x3, true);
            v4P_CustomOpen.Add(CustomType.OLED_64_48_SSD1306x4, true);
            //----
            v4P_CustomOpen.Add(CustomType.OLED_96_16_SSD1306, true);//0.86
            v4P_CustomOpen.Add(CustomType.OLED_96_16_SSD1306x2, true);
            v4P_CustomOpen.Add(CustomType.OLED_96_16_SSD1306x3, true);
            v4P_CustomOpen.Add(CustomType.OLED_96_16_SSD1306x4, true);
            //----
            v4P_CustomOpen.Add(CustomType.OLED_128_32_SSD1306, true);//0.91
            v4P_CustomOpen.Add(CustomType.OLED_128_32_SSD1306x2, true);
            v4P_CustomOpen.Add(CustomType.OLED_128_32_SSD1306x3, true);
            v4P_CustomOpen.Add(CustomType.OLED_128_32_SSD1306x4, true);
            //----
            v4P_CustomOpen.Add(CustomType.OLED_128_64_SSD1306, true);//0.96, 1.09
            v4P_CustomOpen.Add(CustomType.OLED_128_64_SSD1306x2, true);
            v4P_CustomOpen.Add(CustomType.OLED_128_64_SSD1306x3, true);
            v4P_CustomOpen.Add(CustomType.OLED_128_64_SSD1306x4, true);
            //----
            v4P_CustomOpen.Add(CustomType.OLED_128_64_SH1106, true);//1.3
            v4P_CustomOpen.Add(CustomType.OLED_128_64_SH1106x2, true);
            v4P_CustomOpen.Add(CustomType.OLED_128_64_SH1106x3, true);
            v4P_CustomOpen.Add(CustomType.OLED_128_64_SH1106x4, true);
            //----
            v4P_CustomOpen.Add(CustomType.OLED_128_88_SH1107, true);//0.73
            v4P_CustomOpen.Add(CustomType.OLED_128_88_SH1107x2, true);
            v4P_CustomOpen.Add(CustomType.OLED_128_88_SH1107x3, true);
            v4P_CustomOpen.Add(CustomType.OLED_128_88_SH1107x4, true);
            //----
            v4P_CustomOpen.Add(CustomType.OLED_256_64_SSD1322, true);//3.12
            v4P_CustomOpen.Add(CustomType.OLED_256_64_SSD1322x2, true);
            v4P_CustomOpen.Add(CustomType.OLED_256_64_SSD1322x3, true);
            v4P_CustomOpen.Add(CustomType.OLED_256_64_SSD1322x4, true);
            //--------------------------步进控制
            v4P_CustomOpen.Add(CustomType.OUT_StepperMotor, true);
            //--------------------------单点输出
            v4P_CustomOpen.Add(CustomType.OUT_74HC595, true);
            v4P_CustomOpen.Add(CustomType.OUT_IO, true);
            //--------------------------无线模块
            //v4P_CustomOpen.Add(CustomType.OUT_NRF24, true);
            //--------------------------网络模块
            v4P_CustomOpen.Add(CustomType.OUT_W5500, true);
            #endregion
            #endregion
        }
        #region 获取设备开关
        public Dictionary<DeviceType, bool> GetDeviceEnable()
        {
            NeedPowerControl = true;
            switch (HardwareVersion)
            {
                case "31":
                    NeedPowerControl = false;
                    return v31_DeviceOpen;
                case "35":
                    return v35_DeviceOpen;
                case "KB":
                    return vKB_DeviceOpen;
                case "41"://v4B
                    return v4B_DeviceOpen;
                case "40"://v4P
                    return v4P_DeviceOpen;
            }
            return null;
        }
        public Dictionary<CustomType, bool> GetCustomEnable()
        {
            switch (HardwareVersion)
            {
                case "31":
                    return v31_CustomOpen;
                case "35":
                    return v35_CustomOpen;
                case "KB":
                    return vKB_CustomOpen;
                case "41"://v4B
                    return v4B_CustomOpen;
                case "40"://v4P
                    return v4P_CustomOpen;
            }
            return null;
        }
        public bool GetCustomPageEnble()
        {
            switch (HardwareVersion)
            {
                case "31":
                    return false;
                case "35":
                    return false;
                case "KB":
                    return false;
                case "41"://v4B
                    return true;
                case "40"://v4P
                    return true;
            }
            return false;
        }
        public bool GetFontPageEnable()
        {
            switch (HardwareVersion)
            {
                case "31":
                    return false;
                case "35":
                    return false;
                case "KB":
                    return false;
                case "41"://v4B
                    return true;
                case "40"://v4P
                    return true;
            }
            return false;
        }
        #endregion
        #region 保存&读取
        public bool Load(string path)
        {
            if (!File.Exists(path))
            {
                WarningForm.Instance.OpenUI("LoadFail");
                return false;
            }
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(path);
            XmlNode save = xmlDoc.SelectSingleNode("EasyJoy");
            if (save != null && save.HasChildNodes)
            {
                ReSet();
                #region joy属性
                XmlNode x_usb = save.SelectSingleNode("UsbSetting");
                string name;
                if (XmlUI.Instance.GetAttribute(x_usb, "Name", out name))
                {
                    usbName = name;
                }
                int vid;
                if (XmlUI.Instance.GetAttribute(x_usb, "VID", out vid))
                {
                    VID = (ushort)vid;
                }
                int pid;
                if (XmlUI.Instance.GetAttribute(x_usb, "PID", out pid))
                {
                    PID = (ushort)pid;
                }
                byte brightness;
                if (XmlUI.Instance.GetAttribute(x_usb, "BackLightBrightness", out brightness))
                {
                    BackLightBrightness = brightness;
                }
                byte colorOrder;
                if (XmlUI.Instance.GetAttribute(x_usb, "ColorOrder", out colorOrder))
                {
                    ColorOrder = colorOrder;
                }
                bool hc165;
                if (XmlUI.Instance.GetAttribute(x_usb, "HC165", out hc165))
                {
                    HC165 = hc165;
                }
                byte _maxADC;
                if (XmlUI.Instance.GetAttribute(x_usb, "maxADC", out _maxADC))
                {
                    this.joyMaxADC = _maxADC;
                }
                byte _maxHall;
                if (XmlUI.Instance.GetAttribute(x_usb, "maxHall", out _maxHall))
                {
                    this.joyMaxHall = _maxHall;
                }
                byte _maxPWM;
                if (XmlUI.Instance.GetAttribute(x_usb, "maxPWM", out _maxPWM))
                {
                    this.joyMaxPWM = _maxPWM;
                }
                byte _maxPin;
                if (XmlUI.Instance.GetAttribute(x_usb, "maxPin", out _maxPin))
                {
                    this.joyMaxPin = _maxPin;
                }
                ColorInfoType _idleColor;
                if (XmlUI.Instance.GetAttribute<ColorInfoType>(x_usb, "IdleColor", out _idleColor))
                {
                    this.IdleColor = _idleColor;
                }
                byte _dynamicSpeed;
                if (XmlUI.Instance.GetAttribute(x_usb, "DynamicSpeed", out _dynamicSpeed))
                {
                    this.DynamicSpeed = _dynamicSpeed;
                }
                #endregion
                #region AxisID
                XmlNode x_axisID = save.SelectSingleNode("AxisID");
                for (int i = 0; i < axisID.Length; i++)
                {
                    XmlUI.Instance.GetAttribute(x_axisID, "ID" + i, out axisID[i]);
                }
                #endregion
                #region adcList
                XmlNode x_adcList = save.SelectSingleNode("AdcList");
                for (int i = 0; i < adcList.Length; i++)
                {
                    XmlNode x_adc = x_adcList.SelectSingleNode("Adc" + i);
                    XmlUI.Instance.GetAttribute(x_adc, "maxCC", out adcList[i].maxCC);
                    XmlUI.Instance.GetAttribute(x_adc, "maxCY", out adcList[i].maxCY);
                    XmlUI.Instance.GetAttribute(x_adc, "maxPC", out adcList[i].maxPC);
                }
                #endregion
                #region btnList
                XmlNode x_btnList = save.SelectSingleNode("KeyBoard");
                for (int i = 0; i < btnList.Length; i++)
                {
                    XmlNode x_btn = x_btnList.SelectSingleNode("Key" + i);
                    XmlUI.Instance.GetAttribute(x_btn, "Code", out btnList[i].Code);
                    XmlUI.Instance.GetAttribute(x_btn, "Function", out btnList[i].Fun);
                    XmlUI.Instance.GetAttribute(x_btn, "CodeFN", out btnList[i].CodeFN);
                    XmlUI.Instance.GetAttribute(x_btn, "FunctionFN", out btnList[i].FunFN);
                }
                #endregion
                #region formatList
                XmlNode x_formatList = save.SelectSingleNode("FormatList");
                for (int i = 0; i < formatList.Length; i++)
                {
                    XmlNode x_format = x_formatList.SelectSingleNode("Format" + i);
                    XmlUI.Instance.GetAttribute(x_format, "Reverse", out formatList[i].Reverse);
                    XmlUI.Instance.GetAttribute(x_format, "AutoRange", out formatList[i].AutoRange);
                    XmlUI.Instance.GetAttribute(x_format, "Calibration", out formatList[i].Calibration);
                    XmlUI.Instance.GetAttribute(x_format, "Shift", out formatList[i].Shift);
                    XmlUI.Instance.GetAttribute(x_format, "minValue", out formatList[i].minValue);
                    XmlUI.Instance.GetAttribute(x_format, "midValue", out formatList[i].midValue);
                    XmlUI.Instance.GetAttribute(x_format, "maxValue", out formatList[i].maxValue);
                    XmlUI.Instance.GetAttribute(x_format, "minDzone", out formatList[i].minDzone);
                    XmlUI.Instance.GetAttribute(x_format, "midDzone", out formatList[i].midDzone);
                    XmlUI.Instance.GetAttribute(x_format, "maxDzone", out formatList[i].maxDzone);
                }
                #endregion
                #region hallList
                //----------------------------------------------
                #endregion
                #region hatList
                XmlNode x_hatList = save.SelectSingleNode("HatList");
                for (int i = 0; i < hatList.Length; i++)
                {
                    XmlNode x_hat = x_hatList.SelectSingleNode("Hat" + i);
                    XmlUI.Instance.GetAttribute(x_hat, "Input0", out hatList[i].InputIndex[0]);
                    XmlUI.Instance.GetAttribute(x_hat, "Input1", out hatList[i].InputIndex[1]);
                    XmlUI.Instance.GetAttribute(x_hat, "Input2", out hatList[i].InputIndex[2]);
                    XmlUI.Instance.GetAttribute(x_hat, "Input3", out hatList[i].InputIndex[3]);
                }
                #endregion
                #region ledList
                XmlNode x_ledList = save.SelectSingleNode("LedList");
                for (int i = 0; i < ledList.Length; i++)
                {
                    XmlNode x_led = x_ledList.SelectSingleNode("Led" + i);
                    //XmlUI.Instance.GetAttribute(x_led, "Always", out ledList[i].Always);
                    XmlUI.Instance.GetAttribute<LedControlType>(x_led, "ControlType", out ledList[i].ControlType);
                    XmlUI.Instance.GetAttribute(x_led, "R", out ledList[i].R);
                    XmlUI.Instance.GetAttribute(x_led, "G", out ledList[i].G);
                    XmlUI.Instance.GetAttribute(x_led, "B", out ledList[i].B);
                    XmlUI.Instance.GetAttribute(x_led, "OpenR", out ledList[i].OpenR);
                    XmlUI.Instance.GetAttribute(x_led, "OpenG", out ledList[i].OpenG);
                    XmlUI.Instance.GetAttribute(x_led, "OpenB", out ledList[i].OpenB);
                    //----
                    XmlUI.Instance.GetAttribute<ColorInfoType>(x_led, "ClickMode", out colorInfoList[i].ClickMode);
                    XmlUI.Instance.GetAttribute<ColorInfoType>(x_led, "FnMode", out colorInfoList[i].FnMode);
                    XmlUI.Instance.GetAttribute(x_led, "Offset", out colorInfoList[i].Offset);
                }
                #endregion
                #region deviceList
                XmlNode x_deviceList = save.SelectSingleNode("DeviceList");
                for (int i = 0; i < deviceList.Length; i++)
                {
                    XmlNode x_device = x_deviceList.SelectSingleNode("Device" + i);
                    deviceList[i].SyncData(x_device);
                }
                #endregion
                #region customList
                XmlNode x_customList = save.SelectSingleNode("CustomList");
                for (int i = 0; i < customList.Length; i++)
                {
                    XmlNode x_custom = x_customList.SelectSingleNode("Custom" + i);
                    customList[i].SyncData(x_custom);
                }
                #endregion
            }
            return true;
        }
        public void Save(string path)
        {
            XmlDocument xmlDoc = new XmlDocument();
            XmlElement x_save = xmlDoc.CreateElement("EasyJoy");
            #region joy属性
            XmlElement x_usb = xmlDoc.CreateElement("UsbSetting");
            x_usb.SetAttribute("Name", usbName);
            x_usb.SetAttribute("VID", VID.ToString());
            x_usb.SetAttribute("PID", PID.ToString());
            x_usb.SetAttribute("BackLightBrightness", BackLightBrightness.ToString());
            x_usb.SetAttribute("ColorOrder", ColorOrder.ToString());
            x_usb.SetAttribute("HC165", HC165.ToString());
            x_usb.SetAttribute("maxADC", joyMaxADC.ToString());
            x_usb.SetAttribute("maxHall", joyMaxHall.ToString());
            x_usb.SetAttribute("maxPWM", joyMaxPWM.ToString());
            x_usb.SetAttribute("maxPin", joyMaxPin.ToString());
            x_usb.SetAttribute("IdleColor", IdleColor.ToString());
            x_usb.SetAttribute("DynamicSpeed", DynamicSpeed.ToString());
            x_save.AppendChild(x_usb);
            #endregion
            #region AxisID
            XmlElement x_axisID = xmlDoc.CreateElement("AxisID");
            for (int i = 0; i < axisID.Length; i++)
            {
                x_axisID.SetAttribute("ID" + i, axisID[i].ToString());
            }
            x_save.AppendChild(x_axisID);
            #endregion
            #region adcList
            XmlElement x_adcList = xmlDoc.CreateElement("AdcList");
            for (int i = 0; i < adcList.Length; i++)
            {
                XmlElement x_adc = xmlDoc.CreateElement("Adc" + i);
                x_adc.SetAttribute("maxCC", adcList[i].maxCC.ToString());
                x_adc.SetAttribute("maxCY", adcList[i].maxCY.ToString());
                x_adc.SetAttribute("maxPC", adcList[i].maxPC.ToString());
                x_adcList.AppendChild(x_adc);
            }
            x_save.AppendChild(x_adcList);
            #endregion
            #region btnList
            XmlElement x_btnList = xmlDoc.CreateElement("KeyBoard");
            for (int i = 0; i < btnList.Length; i++)
            {
                XmlElement x_btn = xmlDoc.CreateElement("Key" + i);
                x_btn.SetAttribute("Code", btnList[i].Code.ToString());
                x_btn.SetAttribute("Function", btnList[i].Fun.ToString());
                x_btn.SetAttribute("CodeFN", btnList[i].CodeFN.ToString());
                x_btn.SetAttribute("FunctionFN", btnList[i].FunFN.ToString());
                x_btnList.AppendChild(x_btn);
            }
            x_save.AppendChild(x_btnList);
            #endregion
            #region formatList
            XmlElement x_formatList = xmlDoc.CreateElement("FormatList");
            for (int i = 0; i < formatList.Length; i++)
            {
                XmlElement x_format = xmlDoc.CreateElement("Format" + i);
                x_format.SetAttribute("Reverse", formatList[i].Reverse.ToString());
                x_format.SetAttribute("AutoRange", formatList[i].AutoRange.ToString());
                x_format.SetAttribute("Calibration", formatList[i].Calibration.ToString());
                x_format.SetAttribute("Shift", formatList[i].Shift.ToString());
                x_format.SetAttribute("minValue", formatList[i].minValue.ToString());
                x_format.SetAttribute("midValue", formatList[i].midValue.ToString());
                x_format.SetAttribute("maxValue", formatList[i].maxValue.ToString());
                x_format.SetAttribute("minDzone", formatList[i].minDzone.ToString());
                x_format.SetAttribute("midDzone", formatList[i].midDzone.ToString());
                x_format.SetAttribute("maxDzone", formatList[i].maxDzone.ToString());
                x_formatList.AppendChild(x_format);
            }
            x_save.AppendChild(x_formatList);
            #endregion
            #region hallList
            //----------------------------------------------
            #endregion
            #region hatList
            XmlElement x_hatList = xmlDoc.CreateElement("HatList");
            for (int i = 0; i < hatList.Length; i++)
            {
                XmlElement x_hat = xmlDoc.CreateElement("Hat" + i);
                x_hat.SetAttribute("Input0", hatList[i].InputIndex[0].ToString());
                x_hat.SetAttribute("Input1", hatList[i].InputIndex[1].ToString());
                x_hat.SetAttribute("Input2", hatList[i].InputIndex[2].ToString());
                x_hat.SetAttribute("Input3", hatList[i].InputIndex[3].ToString());
                x_hatList.AppendChild(x_hat);
            }
            x_save.AppendChild(x_hatList);
            #endregion
            #region ledList
            XmlElement x_ledList = xmlDoc.CreateElement("LedList");
            for (int i = 0; i < ledList.Length; i++)
            {
                XmlElement x_led = xmlDoc.CreateElement("Led" + i);
                //x_led.SetAttribute("Always", ledList[i].Always.ToString());
                x_led.SetAttribute("ControlType", ledList[i].ControlType.ToString());
                x_led.SetAttribute("R", ledList[i].R.ToString());
                x_led.SetAttribute("G", ledList[i].G.ToString());
                x_led.SetAttribute("B", ledList[i].B.ToString());
                x_led.SetAttribute("OpenR", ledList[i].OpenR.ToString());
                x_led.SetAttribute("OpenG", ledList[i].OpenG.ToString());
                x_led.SetAttribute("OpenB", ledList[i].OpenB.ToString());
                //----
                x_led.SetAttribute("ClickMode", colorInfoList[i].ClickMode.ToString());
                x_led.SetAttribute("FnMode", colorInfoList[i].FnMode.ToString());
                x_led.SetAttribute("Offset", colorInfoList[i].Offset.ToString());
                x_ledList.AppendChild(x_led);
            }
            x_save.AppendChild(x_ledList);
            #endregion
            #region deviceList
            XmlElement x_deviceList = xmlDoc.CreateElement("DeviceList");
            for (int i = 0; i < deviceList.Length; i++)
            {
                XmlElement x_device = xmlDoc.CreateElement("Device" + i);
                x_device.SetAttribute("Type", deviceList[i].Type.ToString());
                x_device.SetAttribute("inInversion", deviceList[i].inInversion.ToString());
                x_device.SetAttribute("outType", deviceList[i].outType.ToString());
                x_device.SetAttribute("outInversion", deviceList[i].outInversion.ToString());
                x_device.SetAttribute("outLed", deviceList[i].outLed.ToString());
                x_device.SetAttribute("encodeType", deviceList[i].encodeType.ToString());
                x_device.SetAttribute("inPort", deviceList[i].inPort.ToString());
                x_device.SetAttribute("inEnd", deviceList[i].inEnd.ToString());
                x_device.SetAttribute("outPort", deviceList[i].outPort.ToString());
                x_device.SetAttribute("outCount", deviceList[i].outCount.ToString());
                x_device.SetAttribute("pulseCount", deviceList[i].pulseCount.ToString());
                x_device.SetAttribute("ledPort", deviceList[i].ledPort.ToString());
                x_device.SetAttribute("ledCount", deviceList[i].ledCount.ToString());
                x_deviceList.AppendChild(x_device);
            }
            x_save.AppendChild(x_deviceList);
            #endregion
            #region customList
            XmlElement x_customList = xmlDoc.CreateElement("CustomList");
            for (int i = 0; i < customList.Length; i++)
            {
                XmlElement x_custom = xmlDoc.CreateElement("Custom" + i);
                x_custom.SetAttribute("Type", customList[i].Type.ToString());
                x_custom.SetAttribute("rotateType", customList[i].rotateType.ToString());
                x_custom.SetAttribute("data", customList[i].data.ToString());
                x_custom.SetAttribute("cs", customList[i].cs.ToString());
                x_custom.SetAttribute("clk", customList[i].clk.ToString());
                x_custom.SetAttribute("dc", customList[i].dc.ToString());
                x_custom.SetAttribute("rst", customList[i].rst.ToString());
                x_custom.SetAttribute("cs2", customList[i].cs2.ToString());
                x_custom.SetAttribute("cs3", customList[i].cs3.ToString());
                x_custom.SetAttribute("cs4", customList[i].cs4.ToString());
                x_custom.SetAttribute("dataStart", customList[i].dataStart.ToString());
                x_custom.SetAttribute("dataCount", customList[i].dataCount.ToString());
                for (int j = 0; j < JoyConst.MaxFontSetCount; j++)
                {
                    XmlElement x_fontSet = xmlDoc.CreateElement("Font" + j);
                    x_fontSet.SetAttribute("X", customList[i].FontSetList[j].X.ToString());
                    x_fontSet.SetAttribute("Y", customList[i].FontSetList[j].Y.ToString());
                    x_fontSet.SetAttribute("LibIndex", customList[i].FontSetList[j].LibIndex.ToString());
                    x_fontSet.SetAttribute("Count", customList[i].FontSetList[j].Count.ToString());
                    x_custom.AppendChild(x_fontSet);
                }
                x_customList.AppendChild(x_custom);
            }
            x_save.AppendChild(x_customList);
            #endregion
            xmlDoc.AppendChild(x_save);
            xmlDoc.Save(path);
        }
        #endregion
        #region 检查Device
        #region 输入接点数量
        public int GetLeftInput(int Max, InPortType Type, int Index)
        {
            int pin = Max;
            for (int i = 0; i < JoyConst.MaxDevice; i++)
            {
                if (i != Index)//排除当前序号的设备
                {
                    JoyDevice dev = deviceList[i];
                    if (dev.portInType == Type)
                    {
                        pin -= dev.inCount;
                    }
                }
            }
            return pin;
        }
        #endregion
        #region 输出接点数量
        public int GetLeftOutput(int Max, OutPortType Type, int Index)
        {
            int pin = Max;
            for (int i = 0; i < JoyConst.MaxDevice; i++)
            {
                if (i != Index)//排除当前序号的设备
                {
                    JoyDevice dev = deviceList[i];
                    if (dev.portOutType == Type)
                    {
                        pin -= dev.outCount;
                    }
                }
            }
            return pin;
        }
        public int GetLeftButton(OutputType type, int index)
        {
            int pin = JoyConst.MaxButton;
            for (int i = 0; i < JoyConst.MaxDevice; i++)
            {
                if (i != index)//排除当前序号的设备
                {
                    JoyDevice dev = deviceList[i];
                    if (dev.portOutType == OutPortType.Button && dev.outType == type)
                    {
                        pin -= dev.outCount;
                    }
                }
            }
            return pin;
        }
        #endregion
        #region LED接点数量
        public int GetLeftLed(int index, LedPortType type)
        {
            int pin = JoyConst.MaxLed;
            for (int i = 0; i < JoyConst.MaxDevice; i++)
            {
                if (i != index)//排除当前序号的设备
                {
                    JoyDevice dev = deviceList[i];
                    if (dev.portLedType == type)
                    {
                        pin -= dev.ledCount;
                    }
                }
            }
            return pin;
        }
        #endregion
        #region format数量
        public InPortType GetInFormatDataType()
        {
            for (int i = 0; i < JoyConst.MaxDevice; i++)
            {
                JoyDevice dev = deviceList[i];
                if (dev.portOutType == OutPortType.FormatIn && SelectFormatIn >= dev.outPort && SelectFormatIn <= dev.outEnd)
                {
                    return dev.portInType;
                }
            }
            return InPortType.None;
        }
        #endregion
        #region 调整Device顺序
        public bool SwitchDevice(int index, int switchIndex)
        {
            if (index != switchIndex)
            {
                if (index >= 0 && index < JoyConst.MaxDevice &&
                    switchIndex >= 0 && switchIndex < JoyConst.MaxDevice)
                {
                    JoyDevice devTemp = new JoyDevice(this, -1);
                    devTemp.SyncData(deviceList[index]);
                    deviceList[index].SyncData(deviceList[switchIndex]);
                    deviceList[switchIndex].SyncData(devTemp);
                    return true;
                }
            }
            return false;
        }
        #endregion
        #region 调整Custom顺序
        public bool SwitchCustom(int index, int switchIndex)
        {
            if (index != switchIndex)
            {
                if (index >= 0 && index < JoyConst.MaxCustom &&
                    switchIndex >= 0 && switchIndex < JoyConst.MaxCustom)
                {
                    JoyCustom customTemp = new JoyCustom(this, -1);
                    customTemp.SyncData(customList[index]);
                    customList[index].SyncData(customList[switchIndex]);
                    customList[switchIndex].SyncData(customTemp);
                    return true;
                }
            }
            return false;
        }
        #endregion
        /// <summary>
        /// 用于自动分配节点
        /// </summary>
        public void CheckDeviceOutputList()
        {
            byte joyOut = 0;
            byte keyOut = 0;
            byte axisOut = 0;
            byte hatOut = 0;
            byte formatIn = 0;
            byte pwmOut = 0;
            byte softOut = 0;
            for (int i = 0; i < deviceList.Length; i++)
            {
                JoyDevice dev = deviceList[i];
                switch (dev.portOutType)
                {
                    case OutPortType.None:
                        break;
                    case OutPortType.Button:
                        switch (dev.outType)
                        {
                            case OutputType.Joystick:
                                dev.outPort = joyOut;
                                joyOut += dev.outCount;
                                if (joyOut > JoyConst.MaxButton)
                                {
                                    WarningForm.Instance.OpenUI("outOfJoystick");
                                }
                                else
                                {
                                    maxOutJoystick = joyOut;
                                }
                                break;
                            case OutputType.Keyboard:
                                dev.outPort = keyOut;
                                keyOut += dev.outCount;
                                if (keyOut > JoyConst.MaxButton)
                                {
                                    WarningForm.Instance.OpenUI("outOfKeyboard");
                                }
                                break;
                            default:
                                break;
                        }
                        break;
                    case OutPortType.Axis:
                        dev.outPort = axisOut;
                        axisOut++;
                        if (axisOut > JoyConst.MaxAxis)
                        {
                            WarningForm.Instance.OpenUI("outOfAxis");
                        }
                        else
                        {
                            maxOutAxis = axisOut;
                        }
                        break;
                    case OutPortType.Hat:
                        dev.outPort = hatOut;
                        hatOut++;
                        if (hatOut > JoyConst.MaxHat)
                        {
                            WarningForm.Instance.OpenUI("outOfHat");
                        }
                        else
                        {
                            maxOutHat = hatOut;
                        }
                        break;
                    case OutPortType.PWM:
                        //dev.outPort = pwmOut;不自动分配PWM
                        pwmOut += dev.outCount;
                        if (pwmOut > JoyConst.MaxPWM)
                        {
                            WarningForm.Instance.OpenUI("outOfPWM");
                        }
                        break;
                    case OutPortType.DataOut:
                        dev.outPort = softOut;
                        softOut += dev.outCount;
                        if (softOut > JoyConst.MaxSoftData)
                        {
                            WarningForm.Instance.OpenUI("outOfSoftData");
                        }
                        break;
                    case OutPortType.FormatIn:
                        dev.outPort = formatIn;
                        formatIn += dev.outCount;
                        if (formatIn > JoyConst.MaxFormat)
                        {
                            WarningForm.Instance.OpenUI("outOfFormat");
                        }
                        break;
                    case OutPortType.Mouse:
                        //nothing
                        break;
                    default:
                        WarningForm.Instance.OpenUI("Error port type !!!", false);
                        break;
                }
            }
            maxOutJoystick = joyOut;
            maxOutAxis = axisOut;
            maxOutHat = hatOut;
        }
        #endregion
        #region 获取Device
        public JoyDevice GetCurrentJoyDevice()
        {
            if (SelectDevice >= 0 && SelectDevice < deviceList.Length)
            {
                JoyDevice dev = deviceList[SelectDevice];
                return dev;
            }
            return null;
        }
        public JoyDevice GetJoyDevice(int index)
        {
            if (index >= 0 && index < deviceList.Length)
            {
                JoyDevice dev = deviceList[index];
                return dev;
            }
            return null;
        }
        #endregion
        #region 获取Custom
        public JoyCustom GetCurrentJoyCustom()
        {
            if (SelectCustom >= 0 && SelectCustom < customList.Length)
            {
                JoyCustom dev = customList[SelectCustom];
                return dev;
            }
            return null;
        }
        public JoyCustom GetJoyCustom(int index)
        {
            if (index >= 0 && index < customList.Length)
            {
                JoyCustom dev = customList[index];
                return dev;
            }
            return null;
        }
        #endregion
        #region 获取字库
        public eFont GetCurrentFont()
        {
            if (SelectFont >= 0 && SelectFont < fontList.Length)
            {
                eFont font = fontList[SelectFont];
                return font;
            }
            return null;
        }
        public eFont GetFont(int index)
        {
            if (index >= 0 && index < fontList.Length)
            {
                eFont font = fontList[index];
                return font;
            }
            return null;
        }
        #endregion
        #region 设置Usb信息
        public void SetUsbName(string _newName)
        {
            if (usbName != _newName)
                usbName = _newName;
        }
        public void SetVID(ushort _vid)
        {
            if (VID != _vid)
                VID = _vid;
        }
        public void SetPID(ushort _pid)
        {
            if (PID != _pid)
                PID = _pid;
        }
        public void SetHC165(bool _hc165)
        {
            if (HC165 != _hc165)
                HC165 = _hc165;
        }
        public void SetTwoKeyMode(bool _Mode)
        {
            TwoKeyMode = _Mode;
        }
        public void SetUsbPower(bool _usbPower)
        {
            USBPower = _usbPower;
        }
        public void SetIdleColor(ColorInfoType _type)
        {
            if (IdleColor != _type)
                IdleColor = _type;
        }
        public void SetDynamicSpeed(byte _speed)
        {
            if (DynamicSpeed != _speed)
                DynamicSpeed = _speed;
        }
        public void SetChangePin(byte index)
        {
            if (currentChangePin != index)
                currentChangePin = index;
        }
        public void SetChangeFormat(byte index)
        {
            if (currentChangeFormat != index)
                currentChangeFormat = index;
        }
        public void SetBackLightBrightness(byte _Brightness)
        {
            if (BackLightBrightness != _Brightness)
                BackLightBrightness = _Brightness;
        }
        public void SetColorOrder(byte _ColorOrder)
        {
            if (ColorOrder != _ColorOrder)
                ColorOrder = _ColorOrder;
        }
        public void SetMaxOutJoystick(byte _value)
        {
            if (maxOutJoystick != _value)
                maxOutJoystick = _value;
        }
        public void SetMaxOutAxis(byte _value)
        {
            if (maxOutAxis != _value)
                maxOutAxis = _value;
        }
        public void SetMaxOutHat(byte _value)
        {
            if (maxOutHat != _value)
                maxOutHat = _value;
        }
        public void SetLinkMode(LinkMode _mode)
        {
            if (linkMode != _mode)
                linkMode = _mode;
        }
        public void SetLicenseUID(bool _uid)
        {
            if (LicenseUID != _uid)
                LicenseUID = _uid;
        }
        #endregion
        public void SetAxisID(int outPort, int index)
        {
            byte temp = axisID[outPort];
            for (int i = 0; i < JoyConst.MaxAxis; i++)
            {
                if (axisID[i] == (byte)(0x30 + index))
                {
                    axisID[i] = temp;
                    break;
                }
            }
            axisID[outPort] = (byte)(0x30 + index);
        }
        public void SetHatState(int index, byte state)
        {
            if (index >= 0 && index < JoyConst.MaxHat)
            {
                hatList[index].State[0] = (state & 0x01) == 0x01;
                hatList[index].State[1] = (state & 0x02) == 0x02;
                hatList[index].State[2] = (state & 0x04) == 0x04;
                hatList[index].State[3] = (state & 0x08) == 0x08;
            }
        }
        #region 设置KEY
        public void SetHardwareVersion(string ver)
        {
            HardwareVersion = ver;
        }
        public void SetMcuID(string id)
        {
            if (id.Length == 24)
                McuID = id.ToUpper();
        }
        public bool SetKeyText(string key)
        {
            try
            {
                string keyIn = key.Replace(" ", "");
                string[] keyList = keyIn.Split('-');
                if (keyList.Length == 4)
                {
                    for (int i = 0; i < 4; i++)
                    {
                        if (keyList[i].Length == 8)
                        {
                            licenseKey[i * 4] = (byte)((PublicData.char2byte(keyList[i][0]) << 4) + PublicData.char2byte(keyList[i][1]));
                            licenseKey[i * 4 + 1] = (byte)((PublicData.char2byte(keyList[i][2]) << 4) + PublicData.char2byte(keyList[i][3]));
                            licenseKey[i * 4 + 2] = (byte)((PublicData.char2byte(keyList[i][4]) << 4) + PublicData.char2byte(keyList[i][5]));
                            licenseKey[i * 4 + 3] = (byte)((PublicData.char2byte(keyList[i][6]) << 4) + PublicData.char2byte(keyList[i][7]));
                        }
                        else
                        {
                            return false;
                        }
                    }
                }
                else
                {
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n" + ex.StackTrace);
            }
            return false;
        }
        public void GetLicense()
        {
            AddReport(new Report(ReportType.LicenseKey));
        }
        #endregion
        #region 设置Font
        public void ReSetFontLib()
        {
            tempFontLibrary = GetFontLibraryArray();
        }
        public void SetSyncFont(int Count, ref byte[] report)
        {
            for (int i = 0; i < JoyConst.MaxSyncFontCount; i++)
            {
                int index = Count + i;
                if (index < JoyConst.MaxFontByteLengh)
                {
                    report[JoyConst.ReportDataStart + i] = tempFontLibrary[index];
                }
            }
        }
        public void GetSyncFont(int index, byte[] fontSync)
        {
            for (int i = 0; i < JoyConst.MaxSyncFontCount; i++)
            {
                int addr = index + i;
                if (addr < JoyConst.MaxFontByteLengh)
                {
                    tempFontLibrary[addr] = fontSync[i];
                }
            }
        }
        public void GetSyncFontLib()
        {
            for (int i = 0; i < JoyConst.MaxFont; i++)
            {
                fontList[i].StartIndex = (ushort)(tempFontLibrary[JoyConst.MaxFontLibLengh + i * JoyConst.MaxFontData] +
                    (tempFontLibrary[JoyConst.MaxFontLibLengh + i * JoyConst.MaxFontData + 1] << 8));
                fontList[i].StartChar = tempFontLibrary[JoyConst.MaxFontLibLengh + i * JoyConst.MaxFontData + 2];
                fontList[i].EndChar = tempFontLibrary[JoyConst.MaxFontLibLengh + i * JoyConst.MaxFontData + 3];
                fontList[i].FontWidth = tempFontLibrary[JoyConst.MaxFontLibLengh + i * JoyConst.MaxFontData + 4];
                fontList[i].FontHeight = tempFontLibrary[JoyConst.MaxFontLibLengh + i * JoyConst.MaxFontData + 5];
                if (fontList[i].StartIndex + fontList[i].MaxCharLength < JoyConst.MaxFontLibLengh)
                    for (int charIndex = 0; charIndex < fontList[i].MaxCharLength; charIndex++)
                    {
                        int tempIndex = fontList[i].StartIndex + charIndex;
                        if (tempIndex >= 0 && tempIndex < tempFontLibrary.Length)
                            fontList[i].SetFontByte(fontList[i].FontSize * fontList[i].StartChar + charIndex, tempFontLibrary[tempIndex]);
                    }
                else
                    break;
            }
        }
        #endregion
        #region LAN
        public void SetMAC(byte mac1, byte mac2, byte mac3, byte mac4, byte mac5, byte mac6)
        {
            MAC[0] = mac1;
            MAC[1] = mac2;
            MAC[2] = mac3;
            MAC[3] = mac4;
            MAC[4] = mac5;
            MAC[5] = mac6;
        }
        public void SetIP(byte ip1, byte ip2, byte ip3, byte ip4, ushort port)
        {
            Local_IP[0] = ip1;
            Local_IP[1] = ip2;
            Local_IP[2] = ip3;
            Local_IP[3] = ip4;
            Local_Port = port;
        }
        public void SetSubNet(byte subnet1, byte subnet2, byte subnet3, byte subnet4)
        {
            Subnet[0] = subnet1;
            Subnet[1] = subnet2;
            Subnet[2] = subnet3;
            Subnet[3] = subnet4;
        }
        public void SetGateway(byte gateway1, byte gateway2, byte gateway3, byte gateway4)
        {
            Gateway[0] = gateway1;
            Gateway[1] = gateway2;
            Gateway[2] = gateway3;
            Gateway[3] = gateway4;
        }
        public void SetDNS(byte dns1, byte dns2, byte dns3, byte dns4)
        {
            DNS[0] = dns1;
            DNS[1] = dns2;
            DNS[2] = dns3;
            DNS[3] = dns4;
        }
        public void SetServerIP(byte ip1, byte ip2, byte ip3, byte ip4, ushort port)
        {
            Remote_IP[0] = ip1;
            Remote_IP[1] = ip2;
            Remote_IP[2] = ip3;
            Remote_IP[3] = ip4;
            Remote_Port = port;
        }
        #endregion
        // 资源占用==================================
        #region Pin实时信息
        public List<PortShowType> GetPinClick()
        {
            List<PortShowType> used = new List<PortShowType>();
            for (int i = 0; i < 18; i++)
            {
                byte Value = pinValue[i];
                for (int j = 0; j < 8; j++)
                {
                    if (((Value >> j) & 1) == 1)
                        used.Add(PortShowType.None);
                    else
                        used.Add(PortShowType.Apply);
                }
            }
            return used;
        }
        #endregion
        #region input
        public List<PortShowType> GetInputUsed(int Count, InPortType Type)
        {
            List<PortShowType> used = new List<PortShowType>();
            for (int i = 0; i < Count; i++)
            {
                used.Add(PortShowType.None);
            }
            JoyDevice selectDev = null;
            foreach (JoyDevice dev in deviceList)
            {
                if (dev.Type != DeviceType.None &&
                    dev.portInType == Type)
                {
                    if (dev.Index != SelectDevice)
                    {
                        for (int i = dev.inPort; i <= dev.inEnd; i++)
                        {
                            if (i >= 0 && i < used.Count)
                                if (used[i] == PortShowType.None)
                                    used[i] = PortShowType.Used;
                                else
                                    used[i] = PortShowType.Error;
                        }
                    }
                    else
                    {
                        selectDev = dev;
                    }
                }
            }
            if (selectDev != null)
            {
                for (int i = selectDev.inPort; i <= selectDev.inEnd; i++)
                {
                    if (i >= 0 && i < used.Count)
                        if (used[i] == PortShowType.None)
                            used[i] = PortShowType.Apply;
                        else
                            used[i] = PortShowType.UsedError;
                }
            }
            return used;
        }
        #endregion
        #region output
        public List<PortShowType> GetButtonUsed(OutputType _outType)
        {
            List<PortShowType> used = new List<PortShowType>();
            for (int i = 0; i < JoyConst.MaxButton; i++)
            {
                used.Add(PortShowType.None);
            }
            JoyDevice selectDev = null;
            foreach (JoyDevice dev in deviceList)
            {
                if (dev.Type != DeviceType.None &&
                    dev.portOutType == OutPortType.Button &&
                    dev.outType == _outType)
                {
                    if (dev.Index != SelectDevice)
                    {
                        for (int i = dev.outPort; i <= dev.outEnd; i++)
                        {
                            if (i >= 0 && i < used.Count)
                                if (used[i] == PortShowType.None)
                                    used[i] = PortShowType.Used;
                                else
                                    used[i] = PortShowType.Error;
                        }
                    }
                    else
                    {
                        selectDev = dev;
                    }
                }
            }
            if (selectDev != null)
            {
                for (int i = selectDev.outPort; i <= selectDev.outEnd; i++)
                {
                    if (i >= 0 && i < used.Count)
                        if (used[i] == PortShowType.None)
                            used[i] = PortShowType.Apply;
                        else
                            used[i] = PortShowType.UsedError;
                }
            }
            return used;
        }
        public List<PortShowType> GetOutputUsed(int Count, OutPortType Type)
        {
            List<PortShowType> used = new List<PortShowType>();
            for (int i = 0; i < Count; i++)
            {
                used.Add(PortShowType.None);
            }
            JoyDevice selectDev = null;
            foreach (JoyDevice dev in deviceList)
            {
                if (dev.Type != DeviceType.None &&
                    dev.portOutType == Type)
                {
                    if (dev.Index != SelectDevice)
                    {
                        for (int i = dev.outPort; i <= dev.outEnd; i++)
                        {
                            if (i >= 0 && i < used.Count)
                                if (used[i] == PortShowType.None)
                                    used[i] = PortShowType.Used;
                                else
                                    used[i] = PortShowType.Error;
                        }
                    }
                    else
                    {
                        selectDev = dev;
                    }
                }
            }
            if (selectDev != null)
            {
                for (int i = selectDev.outPort; i <= selectDev.outEnd; i++)
                {
                    if (i >= 0 && i < used.Count)
                        if (used[i] == PortShowType.None)
                            used[i] = PortShowType.Apply;
                        else
                            used[i] = PortShowType.UsedError;
                }
            }
            return used;
        }
        public List<PortShowType> GetAxisID()
        {
            List<PortShowType> used = new List<PortShowType>();
            for (int i = 0; i < JoyConst.MaxAxis; i++)
            {
                used.Add(PortShowType.None);
            }
            for (int i = 0; i < JoyConst.MaxAxis; i++)
            {
                for (int j = 0; j < JoyConst.MaxAxis; j++)
                {
                    if (i != j && axisID[i] == axisID[j])
                    {
                        used[i] = PortShowType.Error;
                    }
                }
            }
            if (SelectAxis >= 0 && SelectAxis < JoyConst.MaxAxis)
            {
                if (used[axisID[SelectAxis] - 0x30] == PortShowType.None)
                    used[axisID[SelectAxis] - 0x30] = PortShowType.Apply;
                else
                    used[axisID[SelectAxis] - 0x30] = PortShowType.UsedError;
            }
            return used;
        }
        #endregion
        #region Led
        public List<PortShowType> GetLedOrder()
        {
            List<PortShowType> used = new List<PortShowType>();
            for (int i = 0; i < JoyConst.MaxLedOrder; i++)
            {
                used.Add(PortShowType.None);
            }
            used[ColorOrder] = PortShowType.Apply;
            return used;
        }
        public List<PortShowType> GetLedOpen()
        {
            List<PortShowType> used = new List<PortShowType>();
            for (int i = 0; i < JoyConst.MaxLed; i++)
            {
                used.Add(PortShowType.None);
            }
            for (int i = 0; i < ledList.Length; i++)
            {
                switch (ledList[i].ControlType)
                {
                    case LedControlType.LedNone:
                        break;
                    case LedControlType.LedAlways:
                        used[i] = PortShowType.Apply;
                        break;
                    case LedControlType.LedControl:
                        used[i] = PortShowType.Used;
                        break;
                    case LedControlType.LedFN:
                        used[i] = PortShowType.Error;
                        break;
                    case LedControlType.LedCapsLock:
                        used[i] = PortShowType.Error;
                        break;
                    case LedControlType.LedNumLock:
                        used[i] = PortShowType.Error;
                        break;
                }
            }
            return used;
        }
        public List<PortShowType> GetLedUsed()
        {
            List<PortShowType> used = new List<PortShowType>();
            for (int i = 0; i < JoyConst.MaxLed; i++)
            {
                used.Add(PortShowType.None);
            }
            JoyDevice selectDev = null;
            foreach (JoyDevice dev in deviceList)
            {
                if (dev.Type != DeviceType.None &&
                    dev.Type != DeviceType.F_LedBrightness &&
                    dev.portLedType == LedPortType.Led &&
                    dev.outLed)
                {
                    if (dev.Index != SelectDevice)
                    {
                        for (int i = dev.ledPort; i <= dev.ledEnd; i++)
                        {
                            if (i >= 0 && i < used.Count)
                                if (used[i] == PortShowType.None)
                                    used[i] = PortShowType.Used;
                                else
                                    used[i] = PortShowType.Error;
                        }
                    }
                    else
                    {
                        selectDev = dev;
                    }
                }
            }
            if (selectDev != null)
            {
                for (int i = selectDev.ledPort; i <= selectDev.ledEnd; i++)
                {
                    if (i >= 0 && i < used.Count)
                        if (used[i] == PortShowType.None)
                            used[i] = PortShowType.Apply;
                        else
                            used[i] = PortShowType.UsedError;
                }
            }
            return used;
        }
        public List<PortShowType> GetLedBrightnessUsed()
        {
            List<PortShowType> used = new List<PortShowType>();
            for (int i = 0; i < JoyConst.MaxLed; i++)
            {
                used.Add(PortShowType.None);
            }
            JoyDevice selectDev = null;
            foreach (JoyDevice dev in deviceList)
            {
                if (dev.Type != DeviceType.None &&
                    dev.portLedType == LedPortType.Brightness &&
                    dev.outLed)
                {
                    if (dev.Index != SelectDevice)
                    {
                        for (int i = dev.ledPort; i <= dev.ledEnd; i++)
                        {
                            if (i >= 0 && i < used.Count)
                                if (used[i] == PortShowType.None)
                                    used[i] = PortShowType.Used;
                                else
                                    used[i] = PortShowType.Error;
                        }
                    }
                    else
                    {
                        selectDev = dev;
                    }
                }
            }
            if (selectDev != null)
            {
                for (int i = selectDev.ledPort; i <= selectDev.ledEnd; i++)
                {
                    if (i >= 0 && i < used.Count)
                        if (used[i] == PortShowType.None)
                            used[i] = PortShowType.Apply;
                        else
                            used[i] = PortShowType.UsedError;
                }
            }
            return used;
        }
        public List<Color4> GetLedColor()
        {
            List<Color4> colorList = new List<Color4>();
            for (int i = 0; i < ledList.Length; i++)
            {
                colorList.Add(new Color4(ledList[i].R / 255f, ledList[i].G / 255f, ledList[i].B / 255f, 1f));
            }
            return colorList;
        }
        public List<Color4> GetLedOpenColor()
        {
            List<Color4> colorList = new List<Color4>();
            for (int i = 0; i < ledList.Length; i++)
            {
                colorList.Add(new Color4(ledList[i].OpenR / 255f, ledList[i].OpenG / 255f, ledList[i].OpenB / 255f, 1f));
            }
            return colorList;
        }
        #endregion
        #region softData
        public List<PortShowType> GetDataOutUsed()
        {
            List<PortShowType> used = new List<PortShowType>();
            for (int i = 0; i < JoyConst.MaxSoftData; i++)
            {
                used.Add(PortShowType.None);
            }
            JoyDevice selectDev = null;
            foreach (JoyDevice dev in deviceList)
            {
                if (dev.Type != DeviceType.None &&
                    dev.portOutType == OutPortType.DataOut)
                {
                    if (dev.Index != SelectDevice)
                    {
                        for (int i = dev.outPort; i <= dev.outEnd; i++)
                        {
                            if (i >= 0 && i < used.Count)
                                if (used[i] == PortShowType.None)
                                    used[i] = PortShowType.Used;
                                else
                                    used[i] = PortShowType.Error;
                        }
                    }
                    else
                    {
                        selectDev = dev;
                    }
                }
            }
            if (selectDev != null)
            {
                for (int i = selectDev.outPort; i <= selectDev.outEnd; i++)
                {
                    if (i >= 0 && i < used.Count)
                        if (used[i] == PortShowType.None)
                            used[i] = PortShowType.Apply;
                        else
                            used[i] = PortShowType.UsedError;
                }
            }
            return used;
        }
        #endregion
        #region custom
        public bool GetLeftCustomPin(int index)
        {
            List<PortShowType> used = GetCustomPinUsed();
            if (index >= 0 && index < used.Count)
            {
                if (used[index] == PortShowType.None)
                {
                    return true;
                }
            }
            return false;
        }
        public List<byte> GetLeftCustomPinList()
        {
            List<PortShowType> used = GetCustomPinUsed();
            List<byte> leftList = new List<byte>();
            for (int i = 0; i < JoyConst.MaxCustomPin; i++)
            {
                if (used[i] == PortShowType.None || used[i] == PortShowType.Apply)
                {
                    leftList.Add((byte)i);
                }
            }
            return leftList;
        }
        public void SetCustomPinUsed(List<PortShowType> setPin, byte pin, PortShowType used, PortShowType error)
        {
            if (pin >= 0 && pin < setPin.Count)
            {
                if (setPin[pin] == PortShowType.None)
                    setPin[pin] = used;
                else
                    setPin[pin] = error;
            }
        }
        public List<PortShowType> GetCustomPinEnable()
        {
            List<PortShowType> used = new List<PortShowType>();
            for (int i = 0; i < JoyConst.MaxCustomPin; i++)
            {
                used.Add(PortShowType.Used);
            }
            switch (version1)
            {
                case 3://v3
                    for (int i = 0; i < JoyConst.MaxCustomPin; i++)
                    {
                        used[i] = PortShowType.Error;
                    }
                    break;
                case 4:
                    switch (version2)
                    {
                        case 1://v4b
                            //ADC=================================================================
                            if (joyMaxADC > 0)
                            {
                                used[0] = PortShowType.Error;
                                used[1] = PortShowType.Error;
                                used[2] = PortShowType.Error;
                                used[3] = PortShowType.Error;
                                used[4] = PortShowType.Error;
                                used[5] = PortShowType.Error;
                                used[28] = PortShowType.Error;
                                used[29] = PortShowType.Error;
                                used[30] = PortShowType.Error;
                                used[31] = PortShowType.Error;
                            }
                            //PWM=================================================================
                            if (joyMaxPWM > 0)
                            {
                                used[15] = PortShowType.Error;
                                used[16] = PortShowType.Error;
                                used[17] = PortShowType.Error;
                                used[18] = PortShowType.Error;
                            }
                            if (joyMaxPWM > 4)
                            {
                                used[24] = PortShowType.Error;
                                used[25] = PortShowType.Error;
                                used[26] = PortShowType.Error;
                                used[27] = PortShowType.Error;
                            }
                            //Hall================================================================
                            if (joyMaxHall > 0)
                            {
                                used[6] = PortShowType.Error;
                                if (joyMaxHall > 1)
                                    used[7] = PortShowType.Error;
                                if (joyMaxHall > 2)
                                    used[8] = PortShowType.Error;
                                if (joyMaxHall > 3)
                                    used[9] = PortShowType.Error;
                                used[10] = PortShowType.Error;//TLE5010_SCK
                                used[11] = PortShowType.Error;//TLE5010_CS
                            }
                            //Pin=================================================================
                            if (joyMaxPin > 0)
                            {
                                used[20] = PortShowType.Error;//HC165_SL
                                used[21] = PortShowType.Error;//HC165_CLK
                                used[22] = PortShowType.Error;//HC165_DATA_2
                                used[23] = PortShowType.Error;//HC165_DATA_1
                            }
                            used[32] = PortShowType.Error;
                            used[33] = PortShowType.Error;
                            used[34] = PortShowType.Error;
                            used[35] = PortShowType.Error;
                            break;
                    }
                    break;
            }
            return used;
        }
        public List<PortShowType> GetCustomPinUsed()
        {
            List<PortShowType> used = new List<PortShowType>();
            for (int i = 0; i < JoyConst.MaxCustomPin; i++)
            {
                used.Add(PortShowType.None);
            }
            foreach (JoyCustom cus in customList)
            {
                if (cus.Type != CustomType.NoneCustom)
                {
                    if (cus.Index != SelectCustom)
                    {
                        SetCustomPinUsed(used, cus.data, PortShowType.Used, PortShowType.Error);
                        SetCustomPinUsed(used, cus.cs, PortShowType.Used, PortShowType.Error);
                        SetCustomPinUsed(used, cus.clk, PortShowType.Used, PortShowType.Error);
                        SetCustomPinUsed(used, cus.dc, PortShowType.Used, PortShowType.Error);
                        SetCustomPinUsed(used, cus.rst, PortShowType.Used, PortShowType.Error);
                        SetCustomPinUsed(used, cus.cs2, PortShowType.Used, PortShowType.Error);
                        SetCustomPinUsed(used, cus.cs3, PortShowType.Used, PortShowType.Error);
                        SetCustomPinUsed(used, cus.cs4, PortShowType.Used, PortShowType.Error);
                    }
                }
            }
            JoyCustom currectCus = GetJoyCustom(SelectCustom);
            if (currectCus != null && currectCus.Type != CustomType.NoneCustom)
            {
                SetCustomPinUsed(used, currectCus.data, PortShowType.Apply, PortShowType.UsedError);
                SetCustomPinUsed(used, currectCus.cs, PortShowType.Apply, PortShowType.UsedError);
                SetCustomPinUsed(used, currectCus.clk, PortShowType.Apply, PortShowType.UsedError);
                SetCustomPinUsed(used, currectCus.dc, PortShowType.Apply, PortShowType.UsedError);
                SetCustomPinUsed(used, currectCus.rst, PortShowType.Apply, PortShowType.UsedError);
                SetCustomPinUsed(used, currectCus.cs2, PortShowType.Apply, PortShowType.UsedError);
                SetCustomPinUsed(used, currectCus.cs3, PortShowType.Apply, PortShowType.UsedError);
                SetCustomPinUsed(used, currectCus.cs4, PortShowType.Apply, PortShowType.UsedError);
            }
            return used;
        }
        public List<Color4> GetCustomPinColor()
        {
            List<Color4> color = new List<Color4>();
            JoyCustom currectCus = GetJoyCustom(SelectCustom);
            for (int i = 0; i < JoyConst.MaxCustomPin; i++)
            {
                Color4 currentColor = XmlUI.DxUIBackColor;
                if (currectCus != null && currectCus.Type != CustomType.NoneCustom)
                {
                    if (currectCus.data == i)
                        currentColor = XmlUI.DxDeviceBlue;
                    if (currectCus.cs == i)
                        currentColor = XmlUI.DxDeviceGreen;
                    if (currectCus.clk == i)
                        currentColor = XmlUI.DxDeviceRed;
                    if (currectCus.dc == i)
                        currentColor = XmlUI.DxDeviceYellow;
                    if (currectCus.rst == i)
                        currentColor = XmlUI.DxDevicePurple;
                    if (currectCus.cs2 == i)
                        currentColor = XmlUI.DxDeviceGreen;
                    if (currectCus.cs3 == i)
                        currentColor = XmlUI.DxDeviceGreen;
                    if (currectCus.cs4 == i)
                        currentColor = XmlUI.DxDeviceGreen;
                }
                color.Add(currentColor);
            }
            return color;
        }
        public List<Color4> GetFontSetUsed()
        {
            List<Color4> used = new List<Color4>();
            JoyCustom currectCus = GetJoyCustom(SelectCustom);
            if (currectCus != null && currectCus.Type != CustomType.NoneCustom)
            {
                for (int i = 0; i < JoyConst.MaxFontSetCount; i++)
                {
                    if (i < currectCus.dataCount)
                    {
                        used.Add(XmlUI.DxDeviceGreen);
                    }
                    else
                    {
                        used.Add(XmlUI.DxDeviceRed);
                    }
                }
            }
            return used;
        }
        public List<Color4> GetCustomDataSpeed()
        {
            List<Color4> used = new List<Color4>();
            for (int i = 0; i < JoyConst.MaxCustomData; i++)
            {
                if (i < JoyConst.HighSpeedData)
                {
                    used.Add(XmlUI.DxDeviceRed);
                }
                else if (i < JoyConst.HighSpeedData + JoyConst.MidSpeedData * JoyConst.MidSpeedCount)
                {
                    used.Add(XmlUI.DxDeviceBlue);
                }
                else if (i < JoyConst.HighSpeedData + JoyConst.MidSpeedData * JoyConst.MidSpeedCount + JoyConst.LowSpeedData * JoyConst.LowSpeedCount)
                {
                    used.Add(XmlUI.DxDeviceGreen);
                }
                else
                {
                    used.Add(XmlUI.DxDeviceYellow);
                }
            }
            return used;
        }
        public List<PortShowType> GetCustomDataUsed()
        {
            List<PortShowType> used = new List<PortShowType>();
            for (int i = 0; i < JoyConst.MaxCustomData; i++)
            {
                used.Add(PortShowType.None);
            }
            JoyCustom selectCus = null;
            foreach (JoyCustom cus in customList)
            {
                if (cus.Type != CustomType.NoneCustom)
                {
                    if (cus.Index != SelectCustom)
                    {
                        for (int i = cus.dataStart; i <= cus.dataEnd; i++)
                        {
                            if (i >= 0 && i < used.Count)
                            {
                                if (used[i] == PortShowType.None)
                                    used[i] = PortShowType.Used;
                                else
                                    used[i] = PortShowType.Error;
                            }
                        }
                    }
                    else
                    {
                        selectCus = cus;
                    }
                }
            }
            if (selectCus != null)
            {
                for (int i = selectCus.dataStart; i <= selectCus.dataEnd; i++)
                {
                    if (i >= 0 && i < used.Count)
                    {
                        if (used[i] == PortShowType.None)
                            used[i] = PortShowType.Apply;
                        else
                            used[i] = PortShowType.UsedError;
                    }
                }
            }
            return used;
        }
        #endregion
        #region 检查设备配置错误
        public bool CheckJoyError(out string info)
        {
            #region 检查授权
            if (!LicenseUID)
            {
                info = "LicenseError";
                return false;
            }
            #endregion
            #region 检查键盘开关
            KeyBoardOn = false;
            foreach (JoyDevice dev in deviceList)
            {
                if (dev.outType == OutputType.Keyboard)
                    KeyBoardOn = true;
            }
            #endregion
            List<PortShowType> usedPin = GetInputUsed(JoyConst.MaxPin, InPortType.Pin);
            #region Pin 不检测
            //foreach (PortShowType pst in usedPin)
            //{
            //    if (pst == PortShowType.Error || pst == PortShowType.UsedError)
            //    {
            //        info = "PinError";
            //        return false;
            //    }
            //}
            #endregion
            List<PortShowType> usedAdc = GetInputUsed(JoyConst.MaxADC, InPortType.ADC);
            #region ADC
            foreach (PortShowType pst in usedAdc)
            {
                if (pst == PortShowType.Error || pst == PortShowType.UsedError)
                {
                    info = "ADCError";
                    return false;
                }
            }
            #endregion
            List<PortShowType> usedHall = GetInputUsed(JoyConst.MaxHall, InPortType.Hall);
            #region Hall
            foreach (PortShowType pst in usedHall)
            {
                if (pst == PortShowType.Error || pst == PortShowType.UsedError)
                {
                    info = "HallError";
                    return false;
                }
            }
            #endregion
            List<PortShowType> usedFormatOut = GetInputUsed(JoyConst.MaxFormat, InPortType.FormatOut);
            #region FormatOut 不检测
            //foreach (PortShowType pst in usedFormatOut)
            //{
            //    if (pst == PortShowType.Error || pst == PortShowType.UsedError)
            //    {
            //        info = "FormatOutError";
            //        return false;
            //    }
            //}
            #endregion
            List<PortShowType> usedJoy = GetButtonUsed(OutputType.Joystick);
            #region Joystick
            foreach (PortShowType pst in usedJoy)
            {
                if (pst == PortShowType.Error || pst == PortShowType.UsedError)
                {
                    info = "ButtonJoystickError";
                    return false;
                }
            }
            #endregion
            List<PortShowType> usedKey = GetButtonUsed(OutputType.Keyboard);
            #region Keyboard
            foreach (PortShowType pst in usedKey)
            {
                if (pst == PortShowType.Error || pst == PortShowType.UsedError)
                {
                    info = "ButtonKeyboardError";
                    return false;
                }
            }
            #endregion
            List<PortShowType> usedPWM = GetOutputUsed(JoyConst.MaxPWM, OutPortType.PWM);
            #region PWM
            foreach (PortShowType pst in usedPWM)
            {
                if (pst == PortShowType.Error || pst == PortShowType.UsedError)
                {
                    info = "PWMError";
                    return false;
                }
            }
            #endregion
            List<PortShowType> usedDataOut = GetOutputUsed(JoyConst.MaxSoftData, OutPortType.DataOut);
            #region DataOut
            foreach (PortShowType pst in usedDataOut)
            {
                if (pst == PortShowType.Error || pst == PortShowType.UsedError)
                {
                    info = "DataOutError";
                    return false;
                }
            }
            #endregion
            List<PortShowType> usedFormatIn = GetOutputUsed(JoyConst.MaxFormat, OutPortType.FormatIn);
            #region FormatIn
            foreach (PortShowType pst in usedFormatIn)
            {
                if (pst == PortShowType.Error || pst == PortShowType.UsedError)
                {
                    info = "FormatInError";
                    return false;
                }
            }
            #endregion
            List<PortShowType> usedHat = GetOutputUsed(JoyConst.MaxHat, OutPortType.Hat);
            #region Hat
            foreach (PortShowType pst in usedHat)
            {
                if (pst == PortShowType.Error || pst == PortShowType.UsedError)
                {
                    info = "HatError";
                    return false;
                }
            }
            #endregion
            List<PortShowType> usedAxis = GetOutputUsed(JoyConst.MaxAxis, OutPortType.Axis);
            #region Axis
            foreach (PortShowType pst in usedAxis)
            {
                if (pst == PortShowType.Error || pst == PortShowType.UsedError)
                {
                    info = "AxisError";
                    return false;
                }
            }
            #endregion
            List<PortShowType> usedAxisID = GetAxisID();
            #region AxisID
            foreach (PortShowType pst in usedAxisID)
            {
                if (pst == PortShowType.Error || pst == PortShowType.UsedError)
                {
                    info = "AxisIDError";
                    return false;
                }
            }
            #endregion
            List<PortShowType> usedLed = GetLedUsed();
            #region Led
            foreach (PortShowType pst in usedLed)
            {
                if (pst == PortShowType.Error || pst == PortShowType.UsedError)
                {
                    info = "LedError";
                    return false;
                }
            }
            #endregion
            List<PortShowType> usedLedBrightness = GetLedBrightnessUsed();
            #region Brightness
            foreach (PortShowType pst in usedLedBrightness)
            {
                if (pst == PortShowType.Error || pst == PortShowType.UsedError)
                {
                    info = "LedBrightnessError";
                    return false;
                }
            }
            #endregion
            List<PortShowType> usedCustomPin = GetCustomPinUsed();
            #region CustomPin
            foreach (PortShowType pst in usedCustomPin)
            {
                if (pst == PortShowType.Error || pst == PortShowType.UsedError)
                {
                    info = "CustomPinError";
                    return false;
                }
            }
            #endregion
            List<PortShowType> usedCustomData = GetCustomDataUsed();
            #region CustomData
            foreach (PortShowType pst in usedCustomData)
            {
                if (pst == PortShowType.Error || pst == PortShowType.UsedError)
                {
                    info = "CustomDataError";
                    return false;
                }
            }
            #endregion
            info = "None";
            return true;
        }
        #endregion
        //========================================
        #region 获取接口
        public Adc GetAdc(int index)
        {
            if (index >= 0 && index < JoyConst.MaxADC)
            {
                return adcList[index];
            }
            return null;
        }
        public Format GetSelectFormat()
        {
            if (SelectFormatIn >= 0 && SelectFormatIn < JoyConst.MaxFormat)
            {
                return formatList[SelectFormatIn];
            }
            return null;
        }
        public Format GetFormat(int index)
        {
            if (index >= 0 && index < JoyConst.MaxFormat)
            {
                return formatList[index];
            }
            return null;
        }
        public Button GetButton(int index)
        {
            if (index >= 0 && index < JoyConst.MaxButton)
            {
                return btnList[index];
            }
            return null;
        }
        public Hat GetHat(int index)
        {
            if (index >= 0 && index < JoyConst.MaxHat)
            {
                return hatList[index];
            }
            return null;
        }
        public Led GetLed(int index)
        {
            if (index >= 0 && index < JoyConst.MaxLed)
            {
                return ledList[index];
            }
            return null;
        }
        public ColorInfo GetColorInfo(int index)
        {
            if (index >= 0 && index < JoyConst.MaxLed)
            {
                return colorInfoList[index];
            }
            return null;
        }
        #endregion
        #region MessageLink
        private void MessageLink()
        {
            while (loop)
            {
                //Thread.Sleep(1);
                try
                {
                    //-------------------------------------------------------------------------------
                    if (_open)
                    {
                        fps++;
                        #region 通信
                        byte[] report;
                        Report _report = reportMgr.GetReport();
                        if (!_report.GetReport(this, out report))
                        {
                            throw new Win32Exception("GetReport Error !!!");
                        }
                        if (!Send(report))
                        {
                            WarningForm.Instance.OpenUI("JoyObjectSendError");
                        }
                        else
                        {
                            if (_report.Type == ReportType.SaveUsbData)
                            {
                                reportMgr.ReSet();
                            }
#if usbDebug
                            string reportSend = "";
                            string sendType = "";
                            for (int i = 0; i < JoyConst.MaxUsbReport; i++)
                            {
                                switch (i)
                                {
                                    case 0://ID
                                        reportSend += "<<";
                                        break;
                                    case 1://ver1
                                        reportSend += "[";
                                        reportSend += report[i].ToString() + ".";
                                        break;
                                    case 2://ver2
                                        reportSend += report[i].ToString() + ".";
                                        break;
                                    case 3://ver3
                                        reportSend += report[i].ToString();
                                        reportSend += "], ";
                                        break;
                                    case 4://type
                                        sendType = ((ReportType)report[i]).ToString();
                                        break;
                                    case 5://Index
                                        sendType += "(";
                                        sendType += report[i].ToString();
                                        sendType += " - ";
                                        break;
                                    case 6://maxIndex
                                        sendType += report[i].ToString();
                                        sendType += "), ";
                                        break;
                                    case 7://Successful
                                        switch (report[i])
                                        {
                                            case 0:
                                                reportSend += "▼";
                                                break;
                                            case 1:
                                                reportSend += "▲";
                                                break;
                                            default:
                                                reportSend += "X";
                                                break;
                                        }
                                        break;
                                    case 8://data0
                                        reportSend += " -->";
                                        reportSend += report[i].ToString("X2") + ", ";
                                        break;
                                    default:
                                        reportSend += report[i].ToString("X2") + ", ";
                                        break;
                                }
                            }
                            DebugConstol.AddLog(Index + " - " + usbName + " - TX : " + reportSend + " - " + sendType, LogType.NormalB);
#endif
                        }
                        byte[] receive;
                        if (!Receive(out receive))
                        {
                            WarningForm.Instance.OpenUI("JoyObjectReceiveError");
                        }
                        else
                        {
                            if (receive != null && receive[0] == JoyConst.IntputID)
                            {
#if usbDebug
                                string reportReceive = "";
                                string receiveType = "";
                                for (int i = 0; i < receive.Length; i++)
                                {
                                    switch (i)
                                    {
                                        case 0://ID
                                            reportReceive += ">>";
                                            break;
                                        case 1://ver1
                                            reportReceive += "[";
                                            reportReceive += receive[i].ToString() + ".";
                                            break;
                                        case 2://ver2
                                            reportReceive += receive[i].ToString() + ".";
                                            break;
                                        case 3://ver3
                                            reportReceive += receive[i].ToString();
                                            reportReceive += "], ";
                                            break;
                                        case 4://type
                                            receiveType = ((ReportType)receive[i]).ToString();
                                            break;
                                        case 5://Index
                                            receiveType += "(";
                                            receiveType += receive[i].ToString();
                                            receiveType += " - ";
                                            break;
                                        case 6://maxIndex
                                            receiveType += receive[i].ToString();
                                            receiveType += "), ";
                                            break;
                                        case 7://Successful
                                            switch (receive[i])
                                            {
                                                case 0:
                                                    reportReceive += "▼";
                                                    break;
                                                case 1:
                                                    reportReceive += "▲";
                                                    break;
                                                default:
                                                    reportReceive += "X";
                                                    break;
                                            }
                                            break;
                                        case 8://data0
                                            reportReceive += " -->";
                                            reportReceive += receive[i].ToString("X2") + ", ";
                                            break;
                                        default:
                                            reportReceive += receive[i].ToString("X2") + ", ";
                                            break;
                                    }
                                }
#endif
                                string msg = "";
                                if (reportMgr.CheckReport(this, receive, out msg))
                                {
#if usbDebug
                                    DebugConstol.AddLog(Index + " - " + usbName + " - RX : " + reportReceive + " - " + receiveType + " : " + msg);
                                }
                                else
                                {
                                    DebugConstol.AddLog(Index + " - " + usbName + " - ERROR : " + msg, LogType.Error);
                                    DebugConstol.AddLog(Index + " - " + usbName + " - ERROR : " + reportReceive + " - " + receiveType, LogType.Error);
#endif
                                }
                            }
                        }
                        #endregion
                    }
                    //-------------------------------------------------------------------------------
                }
                catch (Exception ex)
                {
                    DebugConstol.AddLog(Index + " - " + usbName + " - ERROR : " + ex.ToString(), LogType.Error);
                    loop = false;
                }
            }
            Dispose();
        }
        private bool Receive(out byte[] report)
        {
            try
            {
                report = new byte[devIn.GetInputReportByteLength()];
                if (Open)
                {
                    devIn.Read(report);
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                throw new Win32Exception(ex.ToString());
            }
        }
        private bool Send(byte[] report)
        {
            try
            {
                if (Open && report.Length == devOut.GetOutputReportByteLength())
                {
                    devOut.Write(report);
                }
                else
                {
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                throw new Win32Exception(ex.ToString());
            }
        }
        public void OpenJoy(HIDInfo dev)
        {
            HIDDev tempDev = new HIDDev();
            try
            {
                if (dev != null)
                {
                    if (tempDev.Open(dev))
                    {
                        if (tempDev.GetInputReportByteLength() == 35)//Joy
                        {
                            devJoy = tempDev;
                            JoyReady = true;
                        }
                        if (tempDev.GetInputReportByteLength() == 64)//In
                        {
                            devIn = tempDev;
                            InReady = true;
                        }
                        if (tempDev.GetOutputReportByteLength() == 64)//Out
                        {
                            devOut = tempDev;
                            OutReady = true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Win32Exception(ex.ToString());
            }
        }
        public bool CheckJoy()
        {
            switch (HardwareVersion)
            {
                case "KB":
                    return InReady & OutReady;
                default:
                    return JoyReady & InReady & OutReady;
            }
        }
        #endregion
        #region TCPLink
        private void TCPLink()
        {
            int length = 0;//初始化消息的长度
            while (true)//循环接收消息
            {
                //Thread.Sleep(100);
                try
                {
                    fps++;
                    byte[] report;
                    Report _report = reportMgr.GetReport();
                    if (!_report.GetReport(this, out report))
                    {
                        throw new Win32Exception("GetReport Error !!!");
                    }
                    clientSocket.Send(report);//发数据
#if tcpDebug
                    string reportSend = "";
                    string sendType = "";
                    for (int i = 0; i < JoyConst.MaxUsbReport; i++)
                    {
                        switch (i)
                        {
                            case 0://ID
                                reportSend += "<<";
                                break;
                            case 1://ver1
                                reportSend += "[";
                                reportSend += report[i].ToString() + ".";
                                break;
                            case 2://ver2
                                reportSend += report[i].ToString() + ".";
                                break;
                            case 3://ver3
                                reportSend += report[i].ToString();
                                reportSend += "], ";
                                break;
                            case 4://type
                                sendType = ((ReportType)report[i]).ToString();
                                break;
                            case 5://Index
                                sendType += "(";
                                sendType += report[i].ToString();
                                sendType += " - ";
                                break;
                            case 6://maxIndex
                                sendType += report[i].ToString();
                                sendType += "), ";
                                break;
                            case 7://Successful
                                switch (report[i])
                                {
                                    case 0:
                                        reportSend += "▼";
                                        break;
                                    case 1:
                                        reportSend += "▲";
                                        break;
                                    default:
                                        reportSend += "X";
                                        break;
                                }
                                break;
                            case 8://data0
                                reportSend += " -->";
                                reportSend += report[i].ToString("X2") + ", ";
                                break;
                            default:
                                reportSend += report[i].ToString("X2") + ", ";
                                break;
                        }
                    }
                    DebugConstol.AddLog(Index + " - " + usbName + " - TX : " + reportSend + " - " + sendType, LogType.NormalB);
#endif
                    length = clientSocket.Receive(tcpData);//获取存放消息数据数组的长度
                    if (clientSocket.Poll(2, SelectMode.SelectRead))//是否可读
                    {
                        clientSocket.Close();
                        break;
                    }
                    if (length == JoyConst.MaxUsbReport)//判断是否有数组内是否存放消息数据
                    {
#if tcpDebug
                        string reportReceive = "";
                        string receiveType = "";
                        for (int i = 0; i < JoyConst.MaxUsbReport; i++)
                        {
                            switch (i)
                            {
                                case 0://ID
                                    reportReceive += ">>";
                                    break;
                                case 1://ver1
                                    reportReceive += "[";
                                    reportReceive += tcpData[i].ToString() + ".";
                                    break;
                                case 2://ver2
                                    reportReceive += tcpData[i].ToString() + ".";
                                    break;
                                case 3://ver3
                                    reportReceive += tcpData[i].ToString();
                                    reportReceive += "], ";
                                    break;
                                case 4://type
                                    receiveType = ((ReportType)tcpData[i]).ToString();
                                    break;
                                case 5://Index
                                    receiveType += "(";
                                    receiveType += tcpData[i].ToString();
                                    receiveType += " - ";
                                    break;
                                case 6://maxIndex
                                    receiveType += tcpData[i].ToString();
                                    receiveType += "), ";
                                    break;
                                case 7://Successful
                                    switch (tcpData[i])
                                    {
                                        case 0:
                                            reportReceive += "▼";
                                            break;
                                        case 1:
                                            reportReceive += "▲";
                                            break;
                                        default:
                                            reportReceive += "X";
                                            break;
                                    }
                                    break;
                                case 8://data0
                                    reportReceive += " -->";
                                    reportReceive += tcpData[i].ToString("X2") + ", ";
                                    break;
                                default:
                                    reportReceive += tcpData[i].ToString("X2") + ", ";
                                    break;
                            }
                        }
#endif
                        string msg = "";
                        if (!reportMgr.CheckReport(this, tcpData, out msg))
                        {
#if tcpDebug
                            DebugConstol.AddLog(Index + " - " + usbName + " - RX : " + reportReceive + " - " + receiveType + " : " + msg);
                        }
                        else
                        {
                            DebugConstol.AddLog(Index + " - " + usbName + " - ERROR : " + msg, LogType.Error);
                            DebugConstol.AddLog(Index + " - " + usbName + " - ERROR : " + reportReceive + " - " + receiveType, LogType.Error);
#endif
                        }
                        length = 0;
                    }
                    else
                    {
                        DebugConstol.AddLog("TCP Receive length Error ：" + length, LogType.Warning);
                    }
                }
                catch (Exception e)
                {
                    DebugConstol.AddLog("TCP Client ReceiveMessage\n" + e.Message + "\n" + e.StackTrace, LogType.Warning);
                    return;
                }
            }
        }
        #endregion
        #region InterfacePlugin

        public event EventHandler CreateUDP;
        public event EventHandler SendUDP;
        public event EventHandler ButtonLeftClick;
        public event EventHandler ButtonRightClick;
        public event EventHandler SwitchButtonChange;
        public event EventHandler TextEditorChange;
        public event EventHandler TrackBarChange;
        public bool Auto { get; set; }
        public string PluginID
        {
            get
            {
                switch (linkType)
                {
                    case LinkType.USB:
                        return McuID;
                    case LinkType.LAN:
                        return ip;
                }
                return null;
            }
        }
        public void Init()
        {
            #region InterfacePlugin
            List<NodePort> pinList = new List<NodePort>();
            Node dataNode = new Node(usbName, pinList);
            moduleList.Add(dataNode);
            #endregion
            moduleList[0].NodePortList.Clear();
            #region Custom
            pinIDList.Clear();
            for (int i = 0; i < JoyConst.MaxCustom; i++)
            {
                switch (customList[i].Type)
                {
                    case CustomType.NoneCustom:
                        break;
                    case CustomType.DT_Max7219:
                        for (int j = 0; j < customList[i].dataCount; j++)
                        {
                            pinIDList.Add(new pinData(i, j, ControlOutputType.Custom));
                            moduleList[0].NodePortList.Add(new NodePort("[" + (i + 1).ToString() + "-" + (j + 1).ToString() + "] " + Localization.Instance.GetLS(customList[i].Type.ToString()), PortType.In, ""));
                        }
                        break;
                    case CustomType.DT_TM1638:
                        pinIDList.Add(new pinData(i, 0, ControlOutputType.Custom));
                        moduleList[0].NodePortList.Add(new NodePort("[" + (i + 1).ToString() + "] " + Localization.Instance.GetLS(customList[i].Type.ToString()), PortType.In, ""));
                        break;
                    case CustomType.DT_HT16K33:
                        pinIDList.Add(new pinData(i, 0, ControlOutputType.Custom));
                        moduleList[0].NodePortList.Add(new NodePort("[" + (i + 1).ToString() + "] " + Localization.Instance.GetLS(customList[i].Type.ToString()), PortType.In, ""));
                        break;
                    case CustomType.Matrix_Max7219:
                        for (int j = 0; j < customList[i].dataCount; j++)
                        {
                            pinIDList.Add(new pinData(i, j, ControlOutputType.Custom));
                            moduleList[0].NodePortList.Add(new NodePort("[" + (i + 1).ToString() + "-" + (j + 1).ToString() + "] " + Localization.Instance.GetLS(customList[i].Type.ToString()), PortType.In, ""));
                        }
                        break;
                    //----
                    case CustomType.OLED_70_40_SSD1306://0.42
                    case CustomType.OLED_70_40_SSD1306x2:
                    case CustomType.OLED_70_40_SSD1306x3:
                    case CustomType.OLED_70_40_SSD1306x4:
                    //----
                    case CustomType.OLED_48_64_SSD1306://0.71
                    case CustomType.OLED_48_64_SSD1306x2:
                    case CustomType.OLED_48_64_SSD1306x3:
                    case CustomType.OLED_48_64_SSD1306x4:
                    //----
                    case CustomType.OLED_64_32_SSD1306://0.49
                    case CustomType.OLED_64_32_SSD1306x2:
                    case CustomType.OLED_64_32_SSD1306x3:
                    case CustomType.OLED_64_32_SSD1306x4:
                    //----
                    case CustomType.OLED_64_48_SSD1306://0.66
                    case CustomType.OLED_64_48_SSD1306x2:
                    case CustomType.OLED_64_48_SSD1306x3:
                    case CustomType.OLED_64_48_SSD1306x4:
                    //----
                    case CustomType.OLED_96_16_SSD1306://0.86
                    case CustomType.OLED_96_16_SSD1306x2:
                    case CustomType.OLED_96_16_SSD1306x3:
                    case CustomType.OLED_96_16_SSD1306x4:
                    //----
                    case CustomType.OLED_128_32_SSD1306://0.91
                    case CustomType.OLED_128_32_SSD1306x2:
                    case CustomType.OLED_128_32_SSD1306x3:
                    case CustomType.OLED_128_32_SSD1306x4:
                    //----
                    case CustomType.OLED_128_64_SSD1306://0.96, 1.09
                    case CustomType.OLED_128_64_SSD1306x2:
                    case CustomType.OLED_128_64_SSD1306x3:
                    case CustomType.OLED_128_64_SSD1306x4:
                    //----
                    case CustomType.OLED_128_64_SH1106://1.3
                    case CustomType.OLED_128_64_SH1106x2:
                    case CustomType.OLED_128_64_SH1106x3:
                    case CustomType.OLED_128_64_SH1106x4:
                    //----
                    case CustomType.OLED_128_88_SH1107://0.73
                    case CustomType.OLED_128_88_SH1107x2:
                    case CustomType.OLED_128_88_SH1107x3:
                    case CustomType.OLED_128_88_SH1107x4:
                    //----
                    case CustomType.OLED_256_64_SSD1322://3.12
                    case CustomType.OLED_256_64_SSD1322x2:
                    case CustomType.OLED_256_64_SSD1322x3:
                    case CustomType.OLED_256_64_SSD1322x4:
                        for (int j = 0; j < customList[i].dataCount; j++)
                        {
                            pinIDList.Add(new pinData(i, j, ControlOutputType.Custom));
                            moduleList[0].NodePortList.Add(new NodePort("[" + (i + 1).ToString() + "-" + (j + 1).ToString() + "] " + Localization.Instance.GetLS(customList[i].Type.ToString()), PortType.In, ""));
                        }
                        break;
                    case CustomType.OUT_StepperMotor:
                        pinIDList.Add(new pinData(i, 0, ControlOutputType.StepperMotor));
                        moduleList[0].NodePortList.Add(new NodePort("[" + (i + 1).ToString().ToString().ToString() + "] " + Localization.Instance.GetLS(customList[i].Type.ToString()), PortType.In, 0));
                        break;
                    case CustomType.OUT_74HC595:
                        for (int j = 0; j < customList[i].dataCount; j++)
                        {
                            for (int pin = 0; pin < 8; pin++)
                            {
                                pinIDList.Add(new pinData(i, j * 8 + pin, ControlOutputType.HC595));
                                moduleList[0].NodePortList.Add(new NodePort("[" + (i + 1).ToString() + "-" + (j + 1).ToString() + "-" + (pin + 1).ToString() + "] " + Localization.Instance.GetLS(customList[i].Type.ToString()), PortType.In, 0));
                            }
                        }
                        break;
                    case CustomType.OUT_IO:
                        pinIDList.Add(new pinData(i, 0, ControlOutputType.IO));
                        moduleList[0].NodePortList.Add(new NodePort("[" + (i + 1).ToString().ToString().ToString() + "] " + Localization.Instance.GetLS(customList[i].Type.ToString()), PortType.In, 0));
                        break;
                    case CustomType.OUT_NRF24:
                    case CustomType.OUT_W5500:
                        //没有数据接点
                        break;
                }
            }
            #endregion
            #region Led
            for (int i = 0; i < ledList.Length; i++)
            {
                if (ledList[i].ControlType == LedControlType.LedControl)
                {
                    pinIDList.Add(new pinData(i, 0, ControlOutputType.Led));
                    moduleList[0].NodePortList.Add(new NodePort("[" + (i + 1).ToString() + "] RGB Led", PortType.In, 0));
                }
            }
            #endregion
            for (int i = 0; i < deviceList.Length; i++)
            {
                switch (deviceList[i].Type)
                {
                    #region Pwm
                    case DeviceType.Soft_PWM:
                        pinIDList.Add(new pinData(deviceList[i].outPort, 0, ControlOutputType.Pwm));
                        moduleList[0].NodePortList.Add(new NodePort("[" + (i + 1).ToString() + "] " + Localization.Instance.GetLS(deviceList[i].Type.ToString()), PortType.In, 0f));
                        break;
                    #endregion
                    #region Button
                    case DeviceType.Soft_Button:
                        pinIDList.Add(new pinData(deviceList[i].outPort, 0, ControlOutputType.SoftOutButton));
                        moduleList[0].NodePortList.Add(new NodePort("[" + (i + 1).ToString() + "] " + Localization.Instance.GetLS(deviceList[i].Type.ToString()), PortType.In, 0));
                        break;
                    #endregion
                    #region Axis
                    case DeviceType.Soft_Axis:
                        pinIDList.Add(new pinData(deviceList[i].outPort, 0, ControlOutputType.SoftOutAxis));
                        moduleList[0].NodePortList.Add(new NodePort("[" + (i + 1).ToString() + "] " + Localization.Instance.GetLS(deviceList[i].Type.ToString()), PortType.In, 0));
                        break;
                    #endregion
                    #region SoftData
                    case DeviceType.SB_Soft:
                    case DeviceType.SB_SoftSwitch:
                    case DeviceType.SB_ModeSwitch:
                    case DeviceType.SB_ModeClick:
                    case DeviceType.MB_SoftSwitch2:
                    case DeviceType.MB_Switch2ModeSwitch:
                    case DeviceType.MB_SoftSwitch3:
                    case DeviceType.MB_Switch3ModeSwitch:
                    case DeviceType.MB_SoftBand:
                    case DeviceType.MB_BandModeSwitch:
                        pinIDList.Add(new pinData(deviceList[i].outPort, 0, ControlOutputType.SoftInButton));
                        moduleList[0].NodePortList.Add(new NodePort("[" + (i + 1).ToString() + "] " + Localization.Instance.GetLS(deviceList[i].Type.ToString()), PortType.Out, 0));
                        break;
                    case DeviceType.MB_SoftEncode:
                        pinIDList.Add(new pinData(deviceList[i].outPort, 0, ControlOutputType.SoftInEncode));
                        moduleList[0].NodePortList.Add(new NodePort("[" + (i + 1).ToString() + "] " + Localization.Instance.GetLS(deviceList[i].Type.ToString()), PortType.Out, 0));
                        break;
                    case DeviceType.F_Soft:
                        pinIDList.Add(new pinData(deviceList[i].outPort, 0, ControlOutputType.SoftInAxis));
                        moduleList[0].NodePortList.Add(new NodePort("[" + (i + 1).ToString() + "] " + Localization.Instance.GetLS(deviceList[i].Type.ToString()), PortType.Out, 0));
                        break;
                        #endregion
                }
            }
            //do it
        }
        public string GetName()
        {
            return usbName;
        }
        public List<Node> GetModuleList()
        {
            return moduleList;
        }
        private string SetAlignRight(string temp, int count)
        {
            while (temp.Length < count)
            {
                temp = ' ' + temp;
            }
            if (temp.Length > count)
            {
                string newTemp = "";
                for (int i = 0; i < count; i++)
                {
                    newTemp = temp[temp.Length - 1 - i] + newTemp;
                }
                temp = newTemp;
            }
            return temp;
        }
        public void NodeCloseEvent(int mIndex)
        {
        }
        public void ValueChangeEvent(int mIndex, int pIndex)
        {
            //do it
            if (mIndex == 0 && pIndex >= 0 && pIndex < pinIDList.Count)
            {
                pinData pin = pinIDList[pIndex];
                switch (pin.type)
                {
                    case ControlOutputType.Custom:
                        if (pin.index >= 0 && pin.index < JoyConst.MaxCustom)
                        {
                            JoyCustom cus = customList[pin.index];
                            if (cus != null)
                            {
                                string temp = moduleList[mIndex].NodePortList[pIndex].ValueString;
                                switch (cus.Type)
                                {
                                    case CustomType.DT_Max7219:
                                        string[] strArray7219 = temp.Split(',');
                                        byte[] dtList7219 = new byte[strArray7219.Length];
                                        for (int i = 0; i < strArray7219.Length; i++)
                                        {
                                            byte tempByte;
                                            if (byte.TryParse(strArray7219[i], out tempByte))
                                                dtList7219[i] = tempByte;
                                        }
                                        int max7219Length = JoyConst.DT7219DataCount > dtList7219.Length ? dtList7219.Length : JoyConst.DT7219DataCount;
                                        for (int i = 0; i < max7219Length; i++)
                                        {
                                            customDataList[cus.dataStart + pin.count * JoyConst.DT7219DataCount + i] = dtList7219[i];
                                        }
                                        break;
                                    case CustomType.DT_TM1638:
                                        string[] strArray1638 = temp.Split(',');
                                        byte[] dtList1638 = new byte[strArray1638.Length];
                                        for (int i = 0; i < strArray1638.Length; i++)
                                        {
                                            byte tempByte;
                                            if (byte.TryParse(strArray1638[i], out tempByte))
                                                dtList1638[i] = tempByte;
                                        }
                                        int tm1638Length = JoyConst.DT1638DataCount > dtList1638.Length ? dtList1638.Length : JoyConst.DT1638DataCount;
                                        for (int i = 0; i < tm1638Length; i++)
                                        {
                                            customDataList[cus.dataStart + i] = dtList1638[dtList1638.Length - 1 - i];
                                        }
                                        break;
                                    case CustomType.DT_HT16K33:
                                        string[] strArrayHT16K33 = temp.Split(',');
                                        byte[] dtListHT16K33 = new byte[strArrayHT16K33.Length];
                                        for (int i = 0; i < strArrayHT16K33.Length; i++)
                                        {
                                            byte tempByte;
                                            if (byte.TryParse(strArrayHT16K33[i], out tempByte))
                                                dtListHT16K33[i] = tempByte;
                                        }
                                        int HT16K33Length = JoyConst.DTHT16K33DataCount > dtListHT16K33.Length ? dtListHT16K33.Length : JoyConst.DTHT16K33DataCount;
                                        for (int i = 0; i < HT16K33Length; i++)
                                        {
                                            customDataList[cus.dataStart + i] = dtListHT16K33[dtListHT16K33.Length - 1 - i];
                                        }
                                        break;
                                    case CustomType.Matrix_Max7219:
                                    //----
                                    case CustomType.OLED_70_40_SSD1306://0.42
                                    case CustomType.OLED_70_40_SSD1306x2:
                                    case CustomType.OLED_70_40_SSD1306x3:
                                    case CustomType.OLED_70_40_SSD1306x4:
                                    //----
                                    case CustomType.OLED_48_64_SSD1306://0.71
                                    case CustomType.OLED_48_64_SSD1306x2:
                                    case CustomType.OLED_48_64_SSD1306x3:
                                    case CustomType.OLED_48_64_SSD1306x4:
                                    //----
                                    case CustomType.OLED_64_32_SSD1306://0.49
                                    case CustomType.OLED_64_32_SSD1306x2:
                                    case CustomType.OLED_64_32_SSD1306x3:
                                    case CustomType.OLED_64_32_SSD1306x4:
                                    //----
                                    case CustomType.OLED_64_48_SSD1306://0.66
                                    case CustomType.OLED_64_48_SSD1306x2:
                                    case CustomType.OLED_64_48_SSD1306x3:
                                    case CustomType.OLED_64_48_SSD1306x4:
                                    //----
                                    case CustomType.OLED_96_16_SSD1306://0.86
                                    case CustomType.OLED_96_16_SSD1306x2:
                                    case CustomType.OLED_96_16_SSD1306x3:
                                    case CustomType.OLED_96_16_SSD1306x4:
                                    //----
                                    case CustomType.OLED_128_32_SSD1306://0.91
                                    case CustomType.OLED_128_32_SSD1306x2:
                                    case CustomType.OLED_128_32_SSD1306x3:
                                    case CustomType.OLED_128_32_SSD1306x4:
                                    //----
                                    case CustomType.OLED_128_64_SSD1306://0.96, 1.09
                                    case CustomType.OLED_128_64_SSD1306x2:
                                    case CustomType.OLED_128_64_SSD1306x3:
                                    case CustomType.OLED_128_64_SSD1306x4:
                                    //----
                                    case CustomType.OLED_128_64_SH1106://1.3
                                    case CustomType.OLED_128_64_SH1106x2:
                                    case CustomType.OLED_128_64_SH1106x3:
                                    case CustomType.OLED_128_64_SH1106x4:
                                    //----
                                    case CustomType.OLED_128_88_SH1107://0.73
                                    case CustomType.OLED_128_88_SH1107x2:
                                    case CustomType.OLED_128_88_SH1107x3:
                                    case CustomType.OLED_128_88_SH1107x4:
                                    //----
                                    case CustomType.OLED_256_64_SSD1322://3.12
                                    case CustomType.OLED_256_64_SSD1322x2:
                                    case CustomType.OLED_256_64_SSD1322x3:
                                    case CustomType.OLED_256_64_SSD1322x4:
                                        int start = 0;
                                        for (int i = 0; i < pin.count; i++)
                                        {
                                            start += cus.FontSetList[i].Count;
                                        }
                                        byte count = cus.FontSetList[pin.count].Count;
                                        #region 右对齐
                                        temp = SetAlignRight(temp, count);
                                        #endregion
                                        for (int i = 0; i < count; i++)
                                        {
                                            customDataList[cus.dataStart + start + i] = (byte)temp[i];
                                        }
                                        break;
                                    case CustomType.OUT_StepperMotor:
                                        //下面单独的步进类型处理
                                        break;
                                    case CustomType.OUT_74HC595:
                                        //下面单独的HC595类型处理
                                        break;
                                    case CustomType.OUT_IO:
                                        //下面单独的IO类型处理
                                        break;
                                    case CustomType.OUT_NRF24:
                                    case CustomType.OUT_W5500:
                                        //不需要处理
                                        break;
                                }
                            }
                        }
                        break;
                    case ControlOutputType.Led:
                        if (pin.index >= 0 && pin.index < JoyConst.MaxLed)
                        {
                            Led led = ledList[pin.index];
                            if (led != null)
                            {
                                led.SoftValue = moduleList[mIndex].NodePortList[pIndex].ValueInt64 != 0;
                            }
                        }
                        break;
                    case ControlOutputType.Pwm:
                        if (pin.index >= 0 && pin.index < JoyConst.MaxPWM)
                        {
                            pwmControlReport[pin.index] = (byte)(moduleList[mIndex].NodePortList[pIndex].ValueDouble * 200f);
                        }
                        break;
                    case ControlOutputType.SoftOutButton:
                        if (pin.index >= 0 && pin.index < JoyConst.MaxButton)
                        {
                            Button btn = btnList[pin.index];
                            if (btn != null)
                            {
                                btn.SoftValue = moduleList[mIndex].NodePortList[pIndex].ValueInt64 != 0;
                            }
                        }
                        break;
                    case ControlOutputType.SoftOutAxis:
                        if (pin.index >= 0 && pin.index < JoyConst.MaxAxis)
                        {
                            axisControlReport[pin.index] = (int)moduleList[mIndex].NodePortList[pIndex].ValueInt64;
                        }
                        break;
                    case ControlOutputType.SoftInButton:
                        moduleList[mIndex].NodePortList[pIndex].ValueInt64 = softDataList[pin.index];
                        break;
                    case ControlOutputType.SoftInAxis:
                        moduleList[mIndex].NodePortList[pIndex].ValueInt64 = (softDataList[pin.index] << 8) + softDataList[pin.index + 1];
                        break;
                    case ControlOutputType.StepperMotor:
                        if (pin.index >= 0 && pin.index < JoyConst.MaxCustom)
                        {
                            JoyCustom cus = customList[pin.index];
                            if (cus != null)
                            {
                                switch (cus.Type)
                                {
                                    case CustomType.OUT_StepperMotor:
                                        long temp = moduleList[mIndex].NodePortList[pIndex].ValueInt64;
                                        if (temp < 0)
                                        {
                                            customDataList[cus.dataStart] = 1;
                                            customDataList[cus.dataStart + 1] = (byte)(-temp);
                                            customDataList[cus.dataStart + 2] = (byte)((-temp) >> 8);
                                            customDataList[cus.dataStart + 3] = (byte)((-temp) >> 16);
                                        }
                                        else
                                        {
                                            customDataList[cus.dataStart] = 0;
                                            customDataList[cus.dataStart + 1] = (byte)(temp);
                                            customDataList[cus.dataStart + 2] = (byte)((temp) >> 8);
                                            customDataList[cus.dataStart + 3] = (byte)((temp) >> 16);
                                        }
                                        break;
                                    default:
                                        //其它类型在Custom处理
                                        break;
                                }
                            }
                        }
                        break;
                    case ControlOutputType.HC595:
                        if (pin.index >= 0 && pin.index < JoyConst.MaxCustom)
                        {
                            JoyCustom cus = customList[pin.index];
                            if (cus != null)
                            {
                                switch (cus.Type)
                                {
                                    case CustomType.OUT_74HC595:
                                        long temp = moduleList[mIndex].NodePortList[pIndex].ValueInt64;
                                        int chipIndex = pin.count / 8;
                                        byte pinValue = (byte)(1 << (pin.count % 8));
                                        if (temp == 0)
                                        {
                                            customDataList[cus.dataStart + chipIndex] |= pinValue;
                                            customDataList[cus.dataStart + chipIndex] -= pinValue;
                                        }
                                        else
                                        {
                                            customDataList[cus.dataStart + chipIndex] |= pinValue;
                                        }
                                        break;
                                    default:
                                        //其它类型在Custom处理
                                        break;
                                }
                            }
                        }
                        break;
                    case ControlOutputType.IO:
                        if (pin.index >= 0 && pin.index < JoyConst.MaxCustom)
                        {
                            JoyCustom cus = customList[pin.index];
                            if (cus != null)
                            {
                                switch (cus.Type)
                                {
                                    case CustomType.OUT_IO:
                                        customDataList[cus.dataStart] = (byte)moduleList[mIndex].NodePortList[pIndex].ValueInt64;
                                        break;
                                    default:
                                        //其它类型在Custom处理
                                        break;
                                }
                            }
                        }
                        break;
                }
            }
        }
        public void AutoOpen()
        {
        }
        public void Update()
        {
            for (int i = 0; i < pinIDList.Count; i++)
            {
                pinData pin = pinIDList[i];
                switch (pin.type)
                {
                    case ControlOutputType.SoftInButton:
                        moduleList[0].NodePortList[i].ValueInt64 = softDataList[pin.index];
                        break;
                    case ControlOutputType.SoftInAxis:
                        moduleList[0].NodePortList[i].ValueInt64 = (softDataList[pin.index] << 8) + softDataList[pin.index + 1];
                        break;
                    case ControlOutputType.SoftInEncode:
                        moduleList[0].NodePortList[i].ValueInt64 = softDataList[pin.index] + (softDataList[pin.index + 1] << 8);
                        break;
                }
            }
        }
        public void DefWndProc(int message)
        {
            //no use
        }
        public void OnButtonLeftClick(string id)
        {
        }
        public void OnButtonRightClick(string id)
        {
        }
        public void OnSwitchButtonChange(string id, bool value)
        {
        }
        public void OnTextEditorChange(string id, string value)
        {
        }
        public void OnTrackBarChange(string id, int value)
        {
        }
        public void OnReceiveUdpMsg(EndPoint client, byte[] msg)
        {
        }
        #endregion
        #region 排序，ToString
        public override string ToString()
        {
            return _usbName + "[" + showFps + "]";
        }

        public int CompareTo(JoyObject obj)
        {
            if (obj == null)
                return 1;
            if (obj.Index > Index)
                return 0;
            else
                return 1;
        }
        #endregion
        public void Dispose()
        {
            try
            {
                NodeLinkControl.Instance.DeletePluginNode(this);
                _open = false;
                switch (linkType)
                {
                    case LinkType.USB:
                        if (devJoy != null)
                        {
                            devJoy.Close();
                        }
                        if (devIn != null)
                        {
                            devIn.Close();
                        }
                        if (devOut != null)
                        {
                            devOut.Close();
                        }
                        threadUSB.Abort();
                        threadUSB.Join();
                        break;
                    case LinkType.LAN:
                        clientSocket.Shutdown(SocketShutdown.Both);
                        clientSocket.Close();
                        threadLAN.Abort();
                        threadLAN.Join();
                        break;
                }
                GC.SuppressFinalize(this);
            }
            catch (Exception ex)
            {
                DebugConstol.AddLog(ex.ToString(), LogType.Error);
            }
        }
    }
}
