using System.Xml;

namespace EasyControl
{
    public class JoyDevice
    {
        public int Index { get; private set; }
        public JoyObject Parent { get; private set; }
        //--------------------------------------------------------------
        public InPortType portInType { private set; get; }
        public OutPortType portOutType { private set; get; }
        public LedPortType portLedType { private set; get; }
        //--------------------------------------------------------------
        private DeviceType _Type = DeviceType.None;
        public DeviceType Type
        #region Type
        {
            get { return _Type; }
            set
            {
                if (_Type == value)
                    return;
                InPortType portIn_old = portInType;
                OutPortType portOut_old = portOutType;
                LedPortType portLed_old = portLedType;
                DeviceType type_old = _Type;
                _Type = value;
                if (!SetType(value))
                {
                    //失败，还原
                    _Type = type_old;
                    portInType = portIn_old;
                    portOutType = portOut_old;
                    portLedType = portLed_old;
                }
            }
        }                 //类型
        #endregion
        // 输入>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
        public bool inInversion = true;   //输入反转
        private byte _inPort = 0;
        public byte inPort
        #region inPort
        {
            get { return _inPort; }
            set
            {
                if (_inPort == value)
                    return;
                //处理越界
                byte inPort_old = _inPort;
                _inPort = value;
                bool inOK = true;
                switch (portInType)
                {
                    case InPortType.Pin:
                        if (_inPort >= JoyConst.MaxPin || inEnd >= JoyConst.MaxPin)
                            inOK = false;
                        break;
                    case InPortType.ADC:
                        if (_inPort >= JoyConst.MaxADC || inEnd >= JoyConst.MaxADC)
                            inOK = false;
                        break;
                    case InPortType.Hall:
                        if (_inPort >= JoyConst.MaxHall || inEnd >= JoyConst.MaxHall)
                            inOK = false;
                        break;
                    case InPortType.FormatOut:
                        if (_inPort >= JoyConst.MaxFormat || inEnd >= JoyConst.MaxFormat)
                            inOK = false;
                        break;
                }
                if (!inOK)
                {
                    _inPort = inPort_old;
                }
            }
        }                           //输入点
        #endregion
        private byte _inCount = 1;
        public byte inCount
        #region inCount
        {
            get { return _inCount; }
            private set
            {
                byte inCount_old = _inCount;
                _inCount = value;
                if (!CheckCountOverflow(_inCount, _outCount, _ledCount))
                {
                    _inCount = inCount_old;
                }
            }
        }                     //输入数量
        #endregion
        public byte inEnd { get { return (byte)(_inPort + inCount - 1); } }
        // 输出<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<
        public bool outInversion = false;              //输出反转
        public OutputType outType = OutputType.Joystick;
        public EncodeType encodeType = EncodeType.Step;
        private byte _outPort = 0;
        public byte outPort
        #region outPort
        {
            get { return _outPort; }
            set
            {
                if (_outPort == value)
                    return;
                //处理越界
                byte outPort_old = _outPort;
                _outPort = value;
                bool outOK = true;
                switch (portOutType)
                {
                    case OutPortType.Axis:
                        if (_outPort >= JoyConst.MaxAxis || outEnd >= JoyConst.MaxAxis)
                        {
                            outOK = false;
                        }
                        break;
                    case OutPortType.Button:
                        if (_outPort >= JoyConst.MaxButton || outEnd >= JoyConst.MaxButton)
                        {
                            outOK = false;
                        }
                        break;
                    case OutPortType.DataOut:
                        if (_outPort >= JoyConst.MaxSoftData || outEnd >= JoyConst.MaxSoftData)
                        {
                            outOK = false;
                        }
                        break;
                    case OutPortType.FormatIn:
                        if (_outPort >= JoyConst.MaxFormat || outEnd >= JoyConst.MaxFormat)
                        {
                            outOK = false;
                        }
                        break;
                    case OutPortType.Hat:
                        if (_outPort >= JoyConst.MaxHat || outEnd >= JoyConst.MaxHat)
                        {
                            outOK = false;
                        }
                        break;
                    case OutPortType.Mouse:
                        //nothing
                        break;
                    case OutPortType.PWM:
                        if (_outPort >= JoyConst.MaxPWM || outEnd >= JoyConst.MaxPWM)
                        {
                            outOK = false;
                        }
                        break;
                }
                if (!outOK)
                {
                    _outPort = outPort_old;
                }
            }
        }                       //输出点
        #endregion
        private byte _outCount = 1;
        public byte outCount
        #region outCount
        {
            get { return _outCount; }
            set
            {
                if (_outCount == value)
                    return;
                byte inCount_old = _inCount;
                byte outCount_old = _outCount;
                byte ledCount_old = _ledCount;
                switch (Type)
                {
                    case DeviceType.SB_MultiMode:
                    case DeviceType.MB_MultiModeEncode:
                        _outCount = value;
                        break;
                    case DeviceType.MB_EncodeBand:
                    case DeviceType.MB_EncodeBand_Pulse:
                    case DeviceType.F_Band:
                    case DeviceType.F_Trigger:
                        _outCount = value;
                        _ledCount = value;
                        break;
                    case DeviceType.MB_Band:
                    case DeviceType.MB_Band_Pulse:
                        _inCount = value;
                        _outCount = value;
                        _ledCount = value;
                        break;
                    case DeviceType.MB_SoftBand:
                    case DeviceType.MB_BandModeSwitch:
                        _inCount = value;
                        _ledCount = value;
                        break;
                }
                if (!CheckCountOverflow(_inCount, _outCount, _ledCount))
                {
                    _inCount = inCount_old;
                    _outCount = outCount_old;
                    _ledCount = ledCount_old;
                }
            }
        }                        //输出数量
        #endregion
        public byte outEnd { get { return (byte)(_outPort + _outCount - 1); } }
        public byte pulseCount = 10;//延时，鼠标速度
        // LED============================================
        public LedMode ledMode = LedMode.Switch;
        private bool _outLed = false;
        #region outLed
        public bool outLed
        {
            get
            {
                switch (ledMode)
                {
                    case LedMode.Close:
                        return false;
                    case LedMode.Open:
                        return true;
                    default:
                        return _outLed;
                }
            }
            set
            {
                switch (ledMode)
                {
                    case LedMode.Close:
                        _outLed = false;
                        break;
                    case LedMode.Open:
                        _outLed = true;
                        break;
                    default:
                        _outLed = value;
                        break;
                }
            }
        }
        #endregion
        private byte _ledPort = 0;
        public byte ledPort
        #region ledPort
        {
            get { return _ledPort; }
            set
            {
                if (_ledPort == value)
                    return;
                //处理越界
                byte ledPort_old = _ledPort;
                _ledPort = value;
                bool ledOK = true;
                if (portLedType == LedPortType.Led || portLedType == LedPortType.Brightness)
                {
                    if (_ledPort >= JoyConst.MaxLed || ledEnd >= JoyConst.MaxLed)
                    {
                        ledOK = false;
                    }
                }
                if (!ledOK)
                {
                    _ledPort = ledPort_old;
                }
            }
        }                         //Led端点
        #endregion
        private byte _ledCount = 1;
        public byte ledCount
        #region ledCount
        {
            get { return _ledCount; }
            set
            {
                if (_ledCount == value)
                    return;
                switch (Type)
                {
                    case DeviceType.SB_SoftSwitch:
                    case DeviceType.SB_ModeSwitch:
                    case DeviceType.MB_BandModeSwitch:
                    case DeviceType.F_LedBrightness:
                        byte ledCount_old = _ledCount;
                        _ledCount = value;
                        if (!CheckCountOverflow(inCount, outCount, ledCount))
                        {
                            _ledCount = ledCount_old;
                        }
                        break;
                }
            }
        }                     //Led数量
        #endregion
        public byte ledEnd { get { return (byte)(_ledPort + _ledCount - 1); } }
        //////////////////////////////////////////////////////////////////////////////////////
        public JoyDevice(JoyObject _Parent, int index)
        {
            Parent = _Parent;
            Index = index;
        }
        private void Reset()
        {
            switch (Parent.HardwareVersion)
            {
                case "KB":
                    inInversion = true;
                    inPort = 0;
                    inCount = 1;
                    outInversion = false;
                    outType = 0;
                    outType = OutputType.Keyboard;
                    encodeType = EncodeType.Step;
                    outPort = 0;
                    outCount = 1;
                    outLed = true;
                    ledPort = 0;
                    ledCount = 1;
                    pulseCount = 50;
                    break;
                default:
                    inInversion = true;
                    inPort = 0;
                    inCount = 1;
                    outInversion = false;
                    outType = 0;
                    outType = OutputType.Joystick;
                    encodeType = EncodeType.Step;
                    outPort = 0;
                    outCount = 1;
                    outLed = Localization.Instance.GetLedOnDefault();
                    ledPort = 0;
                    ledCount = 1;
                    pulseCount = 50;
                    break;
            }
        }
        #region 修改类型
        private bool SetType(DeviceType value)
        {
            Reset();
            ledMode = LedMode.Switch;
            switch (value)
            {
                case DeviceType.None:
                    SetPort(InPortType.None, OutPortType.None, LedPortType.None);
                    break;
                //=========================================
                case DeviceType.SB_Normal: //按钮输出------------------------------------------------
                case DeviceType.SB_Lock: //带锁按钮--------------------------------------------------
                case DeviceType.SB_OnPulse: //开脉冲-------------------------------------------------
                case DeviceType.SB_OffPulse: //关脉冲------------------------------------------------
                case DeviceType.SB_AllPulse: //全脉冲------------------------------------------------
                case DeviceType.SB_Turbo: //连击-----------------------------------------------------
                    SetPort(InPortType.Pin, OutPortType.Button, LedPortType.Led);
                    if (!CheckCountOverflow(1, 1, 1)) return false;
                    break;
                case DeviceType.SB_Soft: //按钮输入--------------------------------------------------
                case DeviceType.SB_SoftSwitch: //按钮切换输入----------------------------------------
                case DeviceType.SB_ModeSwitch: //按钮模式切换----------------------------------------
                case DeviceType.SB_ModeClick://按钮模式触发----------------------------------------
                    SetPort(InPortType.Pin, OutPortType.DataOut, LedPortType.Led);
                    if (!CheckCountOverflow(1, 1, 1)) return false;
                    break;
                case DeviceType.SB_MultiMode: //按钮多模式-------------------------------------------
                    SetPort(InPortType.Pin, OutPortType.Button, LedPortType.Led);
                    if (!CheckCountOverflow(1, 1, 1)) return false;
                    break;
                case DeviceType.SB_CombinedAxisSwitch:
                    SetPort(InPortType.Pin, OutPortType.None, LedPortType.Led);
                    if (!CheckCountOverflow(1, 0, 1)) return false;
                    break;
                case DeviceType.SB_MouseLeft: //鼠标左键---------------------------------------------
                case DeviceType.SB_MouseRight: //鼠标右键--------------------------------------------
                case DeviceType.SB_MouseMiddle: //鼠标中键-------------------------------------------
                    SetPort(InPortType.Pin, OutPortType.Mouse, LedPortType.None);
                    ledMode = LedMode.Close;//----------------------------------------------不支持LED
                    if (!CheckCountOverflow(1, 0, 0)) return false;
                    break;
                case DeviceType.SB_RKJX://RKJX系列8方向按下---------------------------------------
                    SetPort(InPortType.Pin, OutPortType.Button, LedPortType.Led);
                    if (!CheckCountOverflow(5, 1, 1)) return false;
                    break;
                //====================================================================
                case DeviceType.MB_Switch2: //2档钮子开关--------------------------------------------
                case DeviceType.MB_Switch2_Pulse: //2档钮子脉冲开关----------------------------------
                    SetPort(InPortType.Pin, OutPortType.Button, LedPortType.Led);
                    if (!CheckCountOverflow(1, 2, 2)) return false;
                    break;
                case DeviceType.MB_SoftSwitch2: //2档钮子开关输入------------------------------------
                    SetPort(InPortType.Pin, OutPortType.DataOut, LedPortType.Led);
                    if (!CheckCountOverflow(1, 1, 2)) return false;
                    break;
                case DeviceType.MB_Switch2ModeSwitch: //2档钮子开关模式切换------------------------------------
                    SetPort(InPortType.Pin, OutPortType.DataOut, LedPortType.Led);
                    if (!CheckCountOverflow(1, 1, 2)) return false;
                    break;
                case DeviceType.MB_Switch3: //3档钮子开关--------------------------------------------
                case DeviceType.MB_Switch3_Pulse: //3档钮子脉冲开关----------------------------------
                    SetPort(InPortType.Pin, OutPortType.Button, LedPortType.Led);
                    if (!CheckCountOverflow(2, 3, 3)) return false;
                    break;
                case DeviceType.MB_SoftSwitch3: //3档钮子开关输入------------------------------------
                    SetPort(InPortType.Pin, OutPortType.DataOut, LedPortType.Led);
                    if (!CheckCountOverflow(2, 1, 3)) return false;
                    break;
                case DeviceType.MB_Switch3ModeSwitch: //3档钮子开关模式切换------------------------------------
                    SetPort(InPortType.Pin, OutPortType.DataOut, LedPortType.Led);
                    if (!CheckCountOverflow(2, 1, 3)) return false;
                    break;
                case DeviceType.MB_Band: //波段开关--------------------------------------------------
                case DeviceType.MB_Band_Pulse: //波段脉冲开关----------------------------------------
                    SetPort(InPortType.Pin, OutPortType.Button, LedPortType.Led);
                    if (!CheckCountOverflow(2, 2, 2)) return false;
                    break;
                case DeviceType.MB_SoftBand: //波段开关输入------------------------------------------
                    SetPort(InPortType.Pin, OutPortType.DataOut, LedPortType.Led);
                    if (!CheckCountOverflow(2, 1, 2)) return false;
                    break;
                case DeviceType.MB_BandModeSwitch: //波段模式切换------------------------------------
                    SetPort(InPortType.Pin, OutPortType.DataOut, LedPortType.Led);
                    if (!CheckCountOverflow(2, 1, 2)) return false;
                    break;
                case DeviceType.MB_Encode: //编码器--------------------------------------------------
                    SetPort(InPortType.Pin, OutPortType.Button, LedPortType.None);
                    ledMode = LedMode.Close;//----------------------------------------------不支持LED
                    if (!CheckCountOverflow(2, 2, 0)) return false;
                    break;
                case DeviceType.MB_SoftEncode: //编码器输入------------------------------------------
                    SetPort(InPortType.Pin, OutPortType.DataOut, LedPortType.None);
                    ledMode = LedMode.Close;//----------------------------------------------不支持LED
                    if (!CheckCountOverflow(2, 2, 0)) return false;
                    break;
                case DeviceType.MB_MultiModeEncode: //编码器多模式-----------------------------------
                    SetPort(InPortType.Pin, OutPortType.Button, LedPortType.None);
                    ledMode = LedMode.Close;//----------------------------------------------不支持LED
                    if (!CheckCountOverflow(2, 4, 0)) return false;
                    break;
                case DeviceType.MB_EncodeBand: //编码转波段开关--------------------------------------
                case DeviceType.MB_EncodeBand_Pulse: //编码转波段脉冲开关----------------------------
                    SetPort(InPortType.Pin, OutPortType.Button, LedPortType.Led);
                    if (!CheckCountOverflow(2, 2, 2)) return false;
                    break;
                case DeviceType.MB_EncodeCombinedAxis:
                    SetPort(InPortType.Pin, OutPortType.None, LedPortType.None);
                    ledMode = LedMode.Close;//----------------------------------------------不支持LED
                    if (!CheckCountOverflow(2, 0, 0)) return false;
                    break;
                case DeviceType.MB_EncodeMouseX: //编码转鼠标横向------------------------------------
                case DeviceType.MB_EncodeMouseY: //编码转鼠标纵向------------------------------------
                case DeviceType.MB_EncodeMouseWheel: //编码转鼠标滚轮--------------------------------
                    SetPort(InPortType.Pin, OutPortType.Mouse, LedPortType.None);
                    ledMode = LedMode.Close;//----------------------------------------------不支持LED
                    if (!CheckCountOverflow(2, 0, 0)) return false;
                    break;
                case DeviceType.MB_Hat: //苦力帽-----------------------------------------------------
                    SetPort(InPortType.Pin, OutPortType.Hat, LedPortType.None);
                    ledMode = LedMode.Close;//----------------------------------------------不支持LED
                    if (!CheckCountOverflow(4, 1, 0)) return false;
                    break;
                //====================================================================
                case DeviceType.A_ADC: //模数转换轴数据----------------------------------------------
                    SetPort(InPortType.ADC, OutPortType.FormatIn, LedPortType.None);
                    ledMode = LedMode.Close;//----------------------------------------------不支持LED
                    if (!CheckCountOverflow(1, 1, 0)) return false;
                    break;
                case DeviceType.H_TLE5010: //TLE5010轴数据-------------------------------------------
                case DeviceType.H_MLX90316: //MLX90316轴数据-----------------------------------------
                    SetPort(InPortType.Hall, OutPortType.FormatIn, LedPortType.None);
                    ledMode = LedMode.Close;//----------------------------------------------不支持LED
                    if (!CheckCountOverflow(1, 1, 0)) return false;
                    break;
                case DeviceType.H_MLX90333: //MLX90333轴数据-----------------------------------------
                    SetPort(InPortType.Hall, OutPortType.FormatIn, LedPortType.None);
                    ledMode = LedMode.Close;//----------------------------------------------不支持LED
                    if (!CheckCountOverflow(1, 3, 0)) return false;
                    break;
                case DeviceType.H_MLX90363: //MLX90363轴数据-----------------------------------------
                    SetPort(InPortType.Hall, OutPortType.FormatIn, LedPortType.None);
                    ledMode = LedMode.Close;//----------------------------------------------不支持LED
                    if (!CheckCountOverflow(2, 2, 0)) return false;
                    break;
                case DeviceType.H_MLX90393: //MLX90393轴数据-----------------------------------------
                    SetPort(InPortType.Hall, OutPortType.FormatIn, LedPortType.None);
                    ledMode = LedMode.Close;//----------------------------------------------不支持LED
                    if (!CheckCountOverflow(1, 3, 0)) return false;
                    break;
                case DeviceType.H_N35P112: //N35P112轴数据-------------------------------------------
                    SetPort(InPortType.Hall, OutPortType.FormatIn, LedPortType.None);
                    ledMode = LedMode.Close;//----------------------------------------------不支持LED
                    if (!CheckCountOverflow(2, 2, 0)) return false;
                    break;
                case DeviceType.H_HX711:
                    SetPort(InPortType.Hall, OutPortType.FormatIn, LedPortType.None);
                    ledMode = LedMode.Close;//----------------------------------------------不支持LED
                    if (!CheckCountOverflow(2, 1, 0)) return false;
                    break;
                case DeviceType.H_HX717:
                    SetPort(InPortType.Hall, OutPortType.FormatIn, LedPortType.None);
                    ledMode = LedMode.Close;//----------------------------------------------不支持LED
                    if (!CheckCountOverflow(2, 2, 0)) return false;
                    break;
                //====================================================================
                case DeviceType.F_Normal: //轴输出---------------------------------------------------
                    SetPort(InPortType.FormatOut, OutPortType.Axis, LedPortType.None);
                    ledMode = LedMode.Close;//----------------------------------------------不支持LED
                    if (!CheckCountOverflow(1, 1, 0)) return false;
                    break;
                case DeviceType.F_CombinedAxis:
                    SetPort(InPortType.FormatOut, OutPortType.Axis, LedPortType.None);
                    ledMode = LedMode.Close;//----------------------------------------------不支持LED
                    if (!CheckCountOverflow(2, 1, 0)) return false;
                    break;
                case DeviceType.F_LedBrightness: //轴背光控制----------------------------------------
                    SetPort(InPortType.FormatOut, OutPortType.None, LedPortType.Brightness);
                    ledMode = LedMode.Open;//------------------------------------------------强制开启
                    if (!CheckCountOverflow(1, 0, 1)) return false;
                    break;
                case DeviceType.F_ButtonMin: //轴带按键----------------------------------------------
                case DeviceType.F_ButtonMax: //轴带按键----------------------------------------------
                    SetPort(InPortType.FormatOut, OutPortType.Button, LedPortType.Led);
                    if (!CheckCountOverflow(1, 1, 1)) return false;
                    break;
                case DeviceType.F_Band: //轴转波段开关-----------------------------------------------
                case DeviceType.F_Trigger: //轴转多段扳机--------------------------------------------
                    SetPort(InPortType.FormatOut, OutPortType.Button, LedPortType.Led);
                    if (!CheckCountOverflow(1, 2, 2)) return false;
                    break;
                case DeviceType.F_Hat: //轴转苦力帽--------------------------------------------------
                    SetPort(InPortType.FormatOut, OutPortType.Hat, LedPortType.None);
                    ledMode = LedMode.Close;//----------------------------------------------不支持LED
                    if (!CheckCountOverflow(2, 1, 0)) return false;
                    break;
                case DeviceType.F_Soft: //轴输入-----------------------------------------------------
                    SetPort(InPortType.FormatOut, OutPortType.DataOut, LedPortType.None);
                    ledMode = LedMode.Close;//----------------------------------------------不支持LED
                    if (!CheckCountOverflow(1, 1, 0)) return false;
                    break;
                case DeviceType.F_MouseX: //轴转鼠标横向---------------------------------------------
                case DeviceType.F_MouseY: //轴转鼠标纵向---------------------------------------------
                    SetPort(InPortType.FormatOut, OutPortType.Mouse, LedPortType.None);
                    ledMode = LedMode.Close;//----------------------------------------------不支持LED
                    if (!CheckCountOverflow(1, 1, 0)) return false;
                    break;
                case DeviceType.F_PWM: //占空比------------------------------------------------------
                    SetPort(InPortType.FormatOut, OutPortType.PWM, LedPortType.None);
                    ledMode = LedMode.Close;//----------------------------------------------不支持LED
                    if (!CheckCountOverflow(1, 1, 0)) return false;
                    break;
                case DeviceType.Brightness_PWM: //软件背光占空比-------------------------------------------
                    SetPort(InPortType.None, OutPortType.PWM, LedPortType.SoftBrightness);
                    ledMode = LedMode.Close;//----------------------------------------------不支持LED
                    if (!CheckCountOverflow(0, 1, 0)) return false;
                    break;
                case DeviceType.Soft_PWM: //软件数据占空比-------------------------------------------
                    SetPort(InPortType.None, OutPortType.PWM, LedPortType.None);
                    ledMode = LedMode.Close;//----------------------------------------------不支持LED
                    if (!CheckCountOverflow(0, 1, 0)) return false;
                    break;
                case DeviceType.Soft_Button:
                    SetPort(InPortType.None, OutPortType.Button, LedPortType.Led);
                    if (!CheckCountOverflow(0, 1, 1)) return false;
                    break;
                case DeviceType.Soft_Axis:
                    SetPort(InPortType.None, OutPortType.Axis, LedPortType.None);
                    ledMode = LedMode.Close;//----------------------------------------------不支持LED
                    if (!CheckCountOverflow(0, 1, 0)) return false;
                    break;
                case DeviceType.Led_Only:
                    SetPort(InPortType.None, OutPortType.None, LedPortType.Led);
                    ledMode = LedMode.Open;//------------------------------------------------强制开启
                    if (!CheckCountOverflow(0, 0, 1)) return false;
                    break;
                //====================================================================
                default:
                    SetPort(InPortType.None, OutPortType.None, LedPortType.None);
                    _Type = DeviceType.None;
                    WarningForm.Instance.OpenUI("Set Device Type ERROR : " + value, false);
                    return false;
            }
            return PortCheck();
        }
        private void SetPort(InPortType inSide, OutPortType outSide, LedPortType led)
        {
            portInType = inSide;
            portOutType = outSide;
            portLedType = led;
        }
        #endregion
        #region Check
        private bool CheckCountOverflow(byte _in, byte _out, byte _led)
        {
            bool inOK = true;
            bool outOK = true;
            bool ledOK = true;
            switch (portInType)
            {
                case InPortType.Pin:
                    if (_in > Parent.GetLeftInput(JoyConst.MaxPin, InPortType.Pin, Index) || _inPort + _in - 1 >= JoyConst.MaxPin)
                    {
                        inOK = false;
                        WarningForm.Instance.OpenUI("PinMax");
                    }
                    break;
                case InPortType.ADC:
                    if (_in > Parent.GetLeftInput(JoyConst.MaxADC, InPortType.ADC, Index) || _inPort + _in - 1 >= JoyConst.MaxADC)
                    {
                        inOK = false;
                        WarningForm.Instance.OpenUI("AdcMax");
                    }
                    break;
                case InPortType.Hall:
                    if (_in > Parent.GetLeftInput(JoyConst.MaxHall, InPortType.Hall, Index) || _inPort + _in - 1 >= JoyConst.MaxHall)
                    {
                        inOK = false;
                        WarningForm.Instance.OpenUI("HallMax");
                    }
                    break;
                case InPortType.FormatOut:
                    if (_in > Parent.GetLeftInput(JoyConst.MaxFormat, InPortType.FormatOut, Index) || _inPort + _in - 1 >= JoyConst.MaxFormat)
                    {
                        inOK = false;
                        WarningForm.Instance.OpenUI("FormatMax");
                    }
                    break;
            }
            switch (portOutType)
            {
                case OutPortType.Button:
                    if (_out > Parent.GetLeftButton(outType, Index) || _outPort + _out - 1 >= JoyConst.MaxButton)
                    {
                        outOK = false;
                        WarningForm.Instance.OpenUI("ButtonMax");
                    }
                    break;
                case OutPortType.Axis:
                    if (_out > Parent.GetLeftOutput(JoyConst.MaxAxis, OutPortType.Axis, Index) || _outPort + _out - 1 >= JoyConst.MaxAxis)
                    {
                        outOK = false;
                        WarningForm.Instance.OpenUI("AxisMax");
                    }
                    break;
                case OutPortType.Hat:
                    if (_out > Parent.GetLeftOutput(JoyConst.MaxHat, OutPortType.Hat, Index) || _outPort + _out - 1 >= JoyConst.MaxHat)
                    {
                        outOK = false;
                        WarningForm.Instance.OpenUI("HatMax");
                    }
                    break;
                case OutPortType.PWM:
                    if (_out > Parent.GetLeftOutput(JoyConst.MaxPWM, OutPortType.PWM, Index) || _outPort + _out - 1 >= JoyConst.MaxPWM)
                    {
                        outOK = false;
                        WarningForm.Instance.OpenUI("PwmMax");
                    }
                    break;
                case OutPortType.DataOut:
                    if (_out > Parent.GetLeftOutput(JoyConst.MaxSoftData, OutPortType.DataOut, Index) || _outPort + _out - 1 >= JoyConst.MaxSoftData)
                    {
                        outOK = false;
                        WarningForm.Instance.OpenUI("SoftDataMax");
                    }
                    break;
                case OutPortType.FormatIn:
                    if (_out > Parent.GetLeftOutput(JoyConst.MaxFormat, OutPortType.FormatIn, Index) || _outPort + _out - 1 >= JoyConst.MaxSoftData)
                    {
                        outOK = false;
                        WarningForm.Instance.OpenUI("FormatMax");
                    }
                    break;
                case OutPortType.Mouse:
                    //nothing
                    break;
            }
            if (outLed && portLedType != LedPortType.None)
            {
                if (_led > Parent.GetLeftLed(Index, portLedType) || _ledPort + _led - 1 >= JoyConst.MaxLed)
                {
                    ledOK = false;
                    WarningForm.Instance.OpenUI("LedMax");
                }
            }
            if (inOK && outOK && ledOK)
            {
                _inCount = _in;
                _outCount = _out;
                _ledCount = _led;
                return true;
            }
            return false;
        }
        private bool PortCheck()
        {
            switch (portInType)
            {
                case InPortType.Pin:
                    if (_inPort < 0 || inEnd >= JoyConst.MaxPin)
                    {
                        _inPort = 0;
                    }
                    break;
                case InPortType.ADC:
                    if (_inPort < 0 || inEnd >= JoyConst.MaxADC)
                    {
                        _inPort = 0;
                    }
                    break;
                case InPortType.Hall:
                    if (_inPort < 0 || inEnd >= JoyConst.MaxHall)
                    {
                        _inPort = 0;
                    }
                    break;
                case InPortType.FormatOut:
                    if (_inPort < 0 || inEnd >= JoyConst.MaxFormat)
                    {
                        _inPort = 0;
                    }
                    break;
            }
            if (portLedType == LedPortType.Led || portLedType == LedPortType.Brightness)
            {
                if (_ledPort < 0 || ledEnd >= JoyConst.MaxLed)
                {
                    _ledPort = 0;
                }
            }
            return true;
        }
        #endregion
        #region 同步数据
        public void SyncData(XmlNode x_device)
        {
            DeviceType type;
            OutputType _outType;
            EncodeType _encodeType;
            bool _inInversion, _outInversion, _outLed;
            byte portIn, endIn, portOut, countOut, portLed, _pulseCount, countLed;
            if (XmlUI.Instance.GetAttribute<DeviceType>(x_device, "Type", out type))
            {
                _Type = type;
                SetType(_Type);
            }
            if (XmlUI.Instance.GetAttribute<OutputType>(x_device, "outType", out _outType))
                outType = _outType;
            if (XmlUI.Instance.GetAttribute<EncodeType>(x_device, "encodeType", out _encodeType))
                encodeType = _encodeType;
            if (XmlUI.Instance.GetAttribute(x_device, "inInversion", out _inInversion))
                inInversion = _inInversion;
            if (XmlUI.Instance.GetAttribute(x_device, "outInversion", out _outInversion))
                outInversion = _outInversion;
            if (XmlUI.Instance.GetAttribute(x_device, "outLed", out _outLed))
                outLed = _outLed;
            if (XmlUI.Instance.GetAttribute(x_device, "inPort", out portIn) && XmlUI.Instance.GetAttribute(x_device, "inEnd", out endIn))
            {
                _inPort = portIn;
                _inCount = (byte)(endIn + 1 - _inPort);
            }
            if (XmlUI.Instance.GetAttribute(x_device, "outPort", out portOut))
                _outPort = portOut;
            if (XmlUI.Instance.GetAttribute(x_device, "outCount", out countOut))
            {
                _outCount = countOut;
                if (_outCount < 1)
                    _outCount = 1;
            }
            if (XmlUI.Instance.GetAttribute(x_device, "pulseCount", out _pulseCount))
                pulseCount = _pulseCount;
            if (XmlUI.Instance.GetAttribute(x_device, "ledPort", out portLed))
                _ledPort = portLed;
            if (XmlUI.Instance.GetAttribute(x_device, "ledCount", out countLed))
            {
                _ledCount = countLed;
                if (_ledCount < 1)
                    _ledCount = 1;
            }
        }
        public void SyncData(Report usbReport, int count)
        {
            _Type = (DeviceType)usbReport.data[count];
            SetType(_Type);
            inInversion = usbReport.data[count + 1] == 1 ? true : false;
            outType = (OutputType)usbReport.data[count + 2];
            outInversion = usbReport.data[count + 3] == 1 ? true : false;
            outLed = usbReport.data[count + 4] == 1 ? true : false;
            encodeType = (EncodeType)usbReport.data[count + 5];
            _inPort = usbReport.data[count + 6];
            _inCount = (byte)(usbReport.data[count + 7] + 1 - _inPort);
            _outPort = usbReport.data[count + 8];
            _outCount = usbReport.data[count + 9];
            if (_outCount < 1)
                _outCount = 1;
            pulseCount = usbReport.data[count + 10];
            _ledPort = usbReport.data[count + 11];
            _ledCount = usbReport.data[count + 12];
            if (_ledCount < 1)
                _ledCount = 1;
        }
        public void SyncData(JoyDevice dev)
        {
            _Type = dev._Type;
            SetType(_Type);
            inInversion = dev.inInversion;
            outType = dev.outType;
            outInversion = dev.outInversion;
            outLed = dev.outLed;
            encodeType = dev.encodeType;
            _inPort = dev._inPort;
            _inCount = dev._inCount;
            _outPort = dev._outPort;
            _outCount = dev._outCount;
            if (_outCount < 1)
                _outCount = 1;
            pulseCount = dev.pulseCount;
            _ledPort = dev._ledPort;
            _ledCount = dev._ledCount;
            if (_ledCount < 1)
                _ledCount = 1;
        }
        #endregion
    }
}
