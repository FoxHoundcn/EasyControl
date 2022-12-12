using System.Runtime.InteropServices;

namespace EasyControl
{
    [StructLayout(LayoutKind.Sequential)]
    public struct RECT
    {
        public int Left;
        public int Top;
        public int Right;
        public int Bottom;
    }
    [StructLayout(LayoutKind.Sequential)]
    public struct COMPOSITIONFORM
    {
        public uint dwStyle;
        public System.Drawing.Point ptCurrentPos;
        public RECT rcArea;
    }
    public enum EncodeType
    {
        Step,
        TwoStep,
        FourStep,
    }
    public enum ServerState
    {
        Offline,
        LastVersion,
        OldVersion,
    }
    public enum UIType
    {
        None = 0,
        NodeLink,
        PluginControl,
        JoyControl,
        LAN,
        Setting,
    }
    public enum LedUItype
    {
        AllHide = 0,
        NoneDevice,
        LedClose,
        LedLink,
        LedCount,
        OnlyBrightness,
    }
    public enum SettingType
    {
        Settings = 0,
        Localization,
    }
    public enum OpenFileType
    {
        Error = 0,
        EasyJoy,
        EasyControl,
    }
    public enum SaveFileType
    {
        Error = 0,
        FontLib,
        EasyJoy,
        EasyControl,
    }
    public enum V3xFirmware
    {
        Error = 0,
        v31,
        v35,
        vKB,
        vNRF,
        v4b,
    }
    public enum MessageType
    {
        GetVersionNC = 0,
        CheckEjoyVersionNC,
        LicenseNC,
        CheckPluginNC,
    }
    public enum TypeSwitch
    {
        NoneSwitch = 0,
        DeviceSwitch,
        CustomSwitch,
        FontLibrarySwitch
    }
    public enum LogType
    {
        Normal = 0,
        NormalB,
        NormalC,
        Warning,
        Error
    }
    public enum jBtnType
    {
        Normal = 0,
        NoLocalization,
        Image,
    }
    public enum LayoutType
    {
        High,
        Medium,
        Low
    }
    public enum OrientationType
    {
        Vertical = 0,       //横
        Horizontal,         //竖
        Object
    }
    public enum LinkMode
    {
        OffLine = 0,
        OnLine,
        Error,
    }
    public enum LinkType
    {
        None = 0,
        USB,
        LAN,
    }
    public enum ReportType
    {
        LinkTest = 0,
        ReBoot,
        Update,
        //-------------------------------
        LicenseKey,
        LicenseInfo,
        //-------------------------------
        DeviceData,
        DeviceInfo,
        //-------------------------------
        CustomData,
        CustomInfo,
        //-------------------------------
        ButtonData,
        ButtonInfo,
        //-------------------------------
        HatData,
        HatInfo,
        //-------------------------------
        AdcData,
        AdcInfo,
        //-------------------------------
        FormatData,
        FormatInfo,
        //-------------------------------
        LedData,
        LedInfo,
        //-------------------------------
        SaveUsbData,
        GetUsbInfo,
        //==================
        SyncProperty,
        DeviceSync,
        CustomSync,
        //==================
        GetFont0,
        GetFont1,
        GetFont2,
        GetFont3,
        GetFont4,
        GetFont5,
        GetFont6,
        GetFont7,
        GetFontOver,
        //==================
        ClearFont,
        SyncFont0,
        SyncFont1,
        SyncFont2,
        SyncFont3,
        SyncFont4,
        SyncFont5,
        SyncFont6,
        SyncFont7,
        SaveFont,
        //==================
        OledClear,
        KeyBoardSync,
    }
    public enum DeviceType
    {
        None = 0,
        //--------------------------
        //按钮
        SB_Normal,
        SB_Lock,
        SB_OnPulse,
        SB_OffPulse,
        SB_AllPulse,
        SB_Turbo,
        SB_Soft,
        SB_SoftSwitch,
        SB_ModeSwitch,
        SB_ModeClick,
        SB_MultiMode,
        SB_CombinedAxisSwitch,
        SB_MouseLeft,
        SB_MouseRight,
        SB_MouseMiddle,
        SB_RKJX,
        //钮子开关
        MB_Switch2,
        MB_Switch2_Pulse,
        MB_SoftSwitch2,
        MB_Switch2ModeSwitch,
        MB_Switch3,
        MB_Switch3_Pulse,
        MB_SoftSwitch3,
        MB_Switch3ModeSwitch,
        //波段开关
        MB_Band,
        MB_Band_Pulse,
        MB_SoftBand,
        MB_BandModeSwitch,
        //编码器
        MB_Encode,
        MB_SoftEncode,
        MB_MultiModeEncode,
        MB_EncodeBand,
        MB_EncodeBand_Pulse,
        MB_EncodeCombinedAxis,
        MB_EncodeMouseX,
        MB_EncodeMouseY,
        MB_EncodeMouseWheel,
        //苦力帽
        MB_Hat,
        //轴
        A_ADC,
        H_TLE5010,
        H_MLX90316,
        H_MLX90333,
        H_MLX90363,
        H_MLX90393,
        H_N35P112,
        H_HX711,
        H_HX717,
        F_Normal,
        F_CombinedAxis,
        F_ButtonMin,
        F_ButtonMax,
        F_LedBrightness,
        F_Band,
        F_Trigger,
        F_Hat,
        F_Soft,
        F_MouseX,
        F_MouseY,
        //PWM
        F_PWM,
        Brightness_PWM,
        Soft_PWM,
        Soft_Button,
        Soft_Axis,
        Led_Only,
    }
    public enum CustomType
    {
        NoneCustom = 0,
        //--------------------------数码管
        DT_Max7219,
        DT_TM1638,
        DT_HT16K33,
        //--------------------------显示字符
        Matrix_Max7219,
        //----
        OLED_70_40_SSD1306,//0.42
        OLED_70_40_SSD1306x2,
        OLED_70_40_SSD1306x3,
        OLED_70_40_SSD1306x4,
        //----
        OLED_48_64_SSD1306,//0.71
        OLED_48_64_SSD1306x2,
        OLED_48_64_SSD1306x3,
        OLED_48_64_SSD1306x4,
        //----
        OLED_64_32_SSD1306,//0.49
        OLED_64_32_SSD1306x2,
        OLED_64_32_SSD1306x3,
        OLED_64_32_SSD1306x4,
        //----
        OLED_64_48_SSD1306,//0.66
        OLED_64_48_SSD1306x2,
        OLED_64_48_SSD1306x3,
        OLED_64_48_SSD1306x4,
        //----
        OLED_96_16_SSD1306,//0.86
        OLED_96_16_SSD1306x2,
        OLED_96_16_SSD1306x3,
        OLED_96_16_SSD1306x4,
        //----
        OLED_128_32_SSD1306,//0.91
        OLED_128_32_SSD1306x2,
        OLED_128_32_SSD1306x3,
        OLED_128_32_SSD1306x4,
        //----
        OLED_128_64_SSD1306,//0.96, 1.09
        OLED_128_64_SSD1306x2,
        OLED_128_64_SSD1306x3,
        OLED_128_64_SSD1306x4,
        //----
        OLED_128_64_SH1106,//1.3
        OLED_128_64_SH1106x2,
        OLED_128_64_SH1106x3,
        OLED_128_64_SH1106x4,
        //----
        OLED_128_88_SH1107,//0.73
        OLED_128_88_SH1107x2,
        OLED_128_88_SH1107x3,
        OLED_128_88_SH1107x4,
        //----
        OLED_256_64_SSD1322,//3.12
        OLED_256_64_SSD1322x2,
        OLED_256_64_SSD1322x3,
        OLED_256_64_SSD1322x4,
        //--------------------------步进控制
        OUT_StepperMotor,
        //--------------------------单点输出
        OUT_74HC595,
        OUT_IO,
        //--------------------------无线模块
        OUT_NRF24,
        //--------------------------网络模块
        OUT_W5500,
    }
    public enum RotateType
    {
        Rotate0 = 0,
        Rotate90,
        Rotate180,
        Rotate270,
    }
    public enum CustomLinkPin
    {
        None = 0,
        Data,
        CS,
        CLK,
        DC,
        RST,
        CS2,
        CS3,
        CS4
    }
    public enum OutputType
    {
        Joystick,
        Keyboard,
    }
    public enum PortShowType
    {
        None = 0,
        Apply,
        Used,
        Error,
        UsedError
    }
    public enum InPortType
    {
        None = 0,
        Pin,
        ADC,
        Hall,
        FormatOut,
    }
    public enum OutPortType
    {
        None = 0,
        Button,
        Axis,
        Hat,
        PWM,
        Mouse,
        FormatIn,
        DataOut,
    }
    public enum LedPortType
    {
        None = 0,
        Led,
        Brightness,
        SoftBrightness,
    }
    public enum ColorInfoType
    {
        ColorNone = 0,
        ColorRainbow,
        ColorDynamicRainbow,
        ColorRGBWave,
        ColorCustom,
        ColorClick,
    }
    public enum LedMode
    {
        Close = 0,
        Switch,
        Open,
    }
    public enum ControlOutputType
    {
        Custom = 0,
        Led,
        Pwm,
        StepperMotor,
        HC595,
        IO,
        SoftInButton,
        SoftInAxis,
        SoftOutButton,
        SoftOutAxis,
        SoftInEncode,
    }
    public enum LedControlType
    {
        LedNone = 0,
        LedAlways,
        LedControl,
        LedFN,
        LedCapsLock,
        LedNumLock,
    }
    public class JoyEnum
    {
        private JoyEnum() { }
    }
}
