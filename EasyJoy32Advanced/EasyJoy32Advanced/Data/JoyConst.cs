namespace EasyControl
{
    public class JoyConst
    {
        #region version
        public const byte version1 = 4;
        public const byte version2 = 0;
        public const byte version3 = 47;
        public const byte ProtocolVer1 = 1;
        public const byte ProtocolVer2 = 30;
        #endregion
        #region server
        public const long TimeOut = 50000000;
        #endregion
        #region Dx2D
        public const string FontType = "微软雅黑";
        public const float MinFontSize = 2f;
        public const float MaxFontSize = 20f;
        public const float FontSize = 14f;
        #endregion
        #region USB
        public const int IntputID = 2;
        public const int OutputID = 3;
        public const int MaxUsbReport = 64;
        public const int ReportDataStart = 7;
        public const int MaxUsbData = MaxUsbReport - ReportDataStart - 1;
        public const int MaxUsbName = 28;
        #endregion
        #region MainSwitch
        public const int WindowsWidth = 1280 - MainSwitchBtnWidth;
        public const int MainSwitchBtnWidth = 16;
        public const int MainSwitchBtnExWidth = 128;
        public const int MainSwitchBtnHeigh = 64;
        public const int MainSwitchBtnTipHeigh = 12;
        #endregion
        #region Ejoy
        //Object------------------------------------------------------------------------
        public const int MaxJoyObject = 16;
        //Dev---------------------------------------------------------------------------
        public const int MaxDevice = 144;
        //Custom-----------------------------------------------------------------------
        public const int MaxCustom = 12;
        public const int MaxCustomPin = 36;
        public const int HighSpeedData = 16;         //125
        public const int MidSpeedData = 16;     //62
        public const int MidSpeedCount = 2;
        public const int LowSpeedData = 20;     //32
        public const int LowSpeedCount = 4;
        public const int MaxCustomReport = HighSpeedData + MidSpeedData + LowSpeedData;
        public const int MaxCustomData =
            HighSpeedData +
            MidSpeedData * MidSpeedCount +
            LowSpeedData * LowSpeedCount;
        public const int DT7219DataCount = 8;
        public const int DT1638DataCount = 8;
        public const int DTHT16K33DataCount = 16;
        public const int OUTStepMDataCount = 4;
        public const int OUT595DataCount = 8;
        public const int MaxCustomCount = 8;
        public const int MinFontSetCount = 1;
        public const int MaxFontSetCount = 9;
        public const int MinFontCharCount = 1;
        public const int MaxFontCharCount = 10;
        public const int MinDataCount = 1;
        public const int MaxDataCount = 20;
        //Font--------------------------------------------------------------------------
        public const int MaxFont = 8;
        public const int MaxFontData = 6;
        public const int MaxFontByteLengh = 0x10000;
        public const int MaxFontLibLengh = 0x10000 - MaxFont * MaxFontData;
        public const int MaxFontCount = 95;
        public const int MinFontWidth = 4;
        public const int MaxFontWidth = 64;
        public const int MinFontHeight = 1;
        public const int MaxFontHeight = 8;
        public const int MaxSyncFontCount = 32;
        //Adc---------------------------------------------------------------------------
        public const int MaxADC = 10;
        public const int MaxAdcValue = 4095;
        public const int MaxAdcCC = 15;
        public const int MaxAdcCY = 15;
        public const int MaxAdcPC = 255;
        public const int MaxDZone = 50;
        //Hall---------------------------------------------------------------------------
        public const int MaxHall = 4;
        public const int MaxTLE5010Value = 36000;
        public const int MaxMLX90316Value = 65535;
        public const int MaxMLX90333Value = 65535;
        public const int MaxMLX90363Value = 16383;
        public const int MaxMLX90393Value = 65535;
        public const int MaxHX711Value = 65535;
        public const int MaxN35P112Value = 255;
        //Pwm---------------------------------------------------------------------------
        public const int MaxPWM = 8;
        //Axis---------------------------------------------------------------------------
        public const int MaxAxis = 8;
        public const int MaxFormat = 20;
        //Hat---------------------------------------------------------------------------
        public const int MaxHat = 4;
        //Btn---------------------------------------------------------------------------
        public const int MaxButton = 128;
        public const int MaxSwitch2Btn = 127;
        public const int MaxSwitch3Btn = 126;
        //Pin---------------------------------------------------------------------------
        public const int MaxPin = 144;
        public const int MinBand = 2;
        public const int MaxBand = 20;
        public const int MaxSwitch2Pin = 144;
        public const int MaxSwitch3Pin = 143;
        public const int MaxBandPin = 143;
        public const int MaxHatPin = 141;
        //Led---------------------------------------------------------------------------
        public const int MaxLed = 128;
        public const int MaxLedCount = 127;
        public const int MaxSwitch2Led = 127;
        public const int MaxSwitch3Led = 126;
        public const int MaxLedOrder = 6;
        //SoftData----------------------------------------------------------------------
        public const int MaxSoftData = 48;
        //NRF24L04--------------------------------------------------------------------
        public const int MaxNRF24_Channel = 9;
        //ReportCount-----------------------------------------------------------------
        public const int DeviceInfoCount = 4;
        public const int CustomInfoCount = 1;
        public const int ButtonInfoCount = 12;
        public const int HatInfoCount = 4;
        public const int AdcInfoCount = 10;
        public const int FormatInfoCount = 2;
        public const int LedInfoCount = 4;
        #endregion
        #region NodeLink
        public const float MaxNodeWidth = 300f;
        public const float MaxNodeToolWidth = 304f;
        public const float MaxNodeToolHeight = 32f;
        public const float CustomNodeListX = 32f;
        public const float CustomNodeListWidth = 300f;
        public const float CustomNodeListHeight = 8f;
        #endregion
        private JoyConst() { }
    }
}
