using System.Collections.Generic;
using System.Xml;

namespace EasyControl
{
    public class JoyCustom
    {
        public int Index { get; private set; }
        public JoyObject Parent { get; private set; }
        //--------------------------------------------------------------
        private CustomType _Type = CustomType.NoneCustom;
        public CustomType Type
        #region Type
        {
            get { return _Type; }
            set
            {
                if (_Type == value)
                    return;
                byte oldData = data;
                byte oldCs = cs;
                byte oldClk = clk;
                byte oldDc = dc;
                byte oldRst = rst;
                byte oldCs2 = cs2;
                byte oldCs3 = cs3;
                byte oldCs4 = cs4;
                CustomType oldType = _Type;
                _Type = value;
                if (!SetType())
                {
                    _data = oldData;
                    _cs = oldCs;
                    _clk = oldClk;
                    _dc = oldDc;
                    _rst = oldRst;
                    _Type = oldType;
                    _cs2 = oldCs2;
                    _cs3 = oldCs3;
                    _cs4 = oldCs4;
                    WarningForm.Instance.OpenUI("CustomPinMax");
                }
            }
        }
        #endregion
        public RotateType rotateType = RotateType.Rotate0;
        private byte _data = 255;
        public byte data
        #region data
        {
            get { return _data; }
            set
            {
                if (_data == value)
                    return;
                byte oldValue = _data;
                if (Parent.GetLeftCustomPin(value))
                {
                    _data = value;
                }
                else
                {
                    _data = oldValue;
                }
            }
        }
        #endregion
        private byte _cs = 255;
        public byte cs
        #region cs
        {
            get { return _cs; }
            set
            {
                if (_cs == value)
                    return;
                byte oldValue = _cs;
                if (Parent.GetLeftCustomPin(value))
                {
                    _cs = value;
                }
                else
                {
                    _cs = oldValue;
                }
            }
        }
        #endregion
        private byte _clk = 255;
        public byte clk
        #region clk
        {
            get { return _clk; }
            set
            {
                if (_clk == value)
                    return;
                byte oldValue = _clk;
                if (Parent.GetLeftCustomPin(value))
                {
                    _clk = value;
                }
                else
                {
                    _clk = oldValue;
                }
            }
        }
        #endregion
        private byte _dc = 255;
        public byte dc
        #region dc
        {
            get { return _dc; }
            set
            {
                if (_dc == value)
                    return;
                byte oldValue = _dc;
                if (Parent.GetLeftCustomPin(value))
                {
                    _dc = value;
                }
                else
                {
                    _dc = oldValue;
                }
            }
        }
        #endregion
        private byte _rst = 255;
        public byte rst
        #region rst
        {
            get { return _rst; }
            set
            {
                if (_rst == value)
                    return;
                byte oldValue = _rst;
                if (Parent.GetLeftCustomPin(value))
                {
                    _rst = value;
                }
                else
                {
                    _rst = oldValue;
                }
            }
        }
        #endregion
        private byte _cs2 = 255;
        public byte cs2
        #region cs2
        {
            get { return _cs2; }
            set
            {
                if (_cs2 == value)
                    return;
                byte oldValue = _cs2;
                if (Parent.GetLeftCustomPin(value))
                {
                    _cs2 = value;
                }
                else
                {
                    _cs2 = oldValue;
                }
            }
        }
        #endregion
        private byte _cs3 = 255;
        public byte cs3
        #region cs3
        {
            get { return _cs3; }
            set
            {
                if (_cs3 == value)
                    return;
                byte oldValue = _cs3;
                if (Parent.GetLeftCustomPin(value))
                {
                    _cs3 = value;
                }
                else
                {
                    _cs3 = oldValue;
                }
            }
        }
        #endregion
        private byte _cs4 = 255;
        public byte cs4
        #region cs4
        {
            get { return _cs4; }
            set
            {
                if (_cs4 == value)
                    return;
                byte oldValue = _cs4;
                if (Parent.GetLeftCustomPin(value))
                {
                    _cs4 = value;
                }
                else
                {
                    _cs4 = oldValue;
                }
            }
        }
        #endregion
        public byte dataStart = 0;
        private byte _dataCount = 1;
        public byte dataCount
        #region dataCount
        {
            get
            {
                return _dataCount;
            }
            set
            {
                switch (Type)
                {
                    case CustomType.DT_Max7219:
                        _dataCount = value;
                        if (_dataCount < 1)
                            _dataCount = 1;
                        if (_dataCount > JoyConst.DT7219DataCount)
                            _dataCount = JoyConst.DT7219DataCount;
                        break;
                    case CustomType.DT_TM1638:
                        _dataCount = JoyConst.DT1638DataCount;
                        break;
                    case CustomType.DT_HT16K33:
                        _dataCount = JoyConst.DTHT16K33DataCount;
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
                        _dataCount = value;
                        if (_dataCount < JoyConst.MinFontSetCount)
                            _dataCount = JoyConst.MinFontSetCount;
                        if (_dataCount > JoyConst.MaxFontSetCount)
                            _dataCount = JoyConst.MaxFontSetCount;
                        break;
                    case CustomType.OUT_StepperMotor:
                        _dataCount = JoyConst.OUTStepMDataCount;
                        break;
                    case CustomType.OUT_74HC595:
                        _dataCount = value;
                        if (_dataCount < 1)
                            _dataCount = 1;
                        if (_dataCount > JoyConst.OUT595DataCount)
                            _dataCount = JoyConst.OUT595DataCount;
                        break;
                    case CustomType.OUT_IO:
                        _dataCount = 1;
                        break;
                    case CustomType.OUT_NRF24:
                        _dataCount = value;
                        if (_dataCount < 1)
                            _dataCount = 1;
                        if (_dataCount > JoyConst.MaxNRF24_Channel)
                            _dataCount = JoyConst.MaxNRF24_Channel;
                        break;
                    case CustomType.OUT_W5500:
                        _dataCount = 1;
                        break;
                }
            }
        }
        #endregion
        public byte dataEnd
        #region dataEnd
        {
            get
            {
                switch (Type)
                {
                    case CustomType.DT_Max7219:
                        return (byte)(dataStart + _dataCount * JoyConst.DT1638DataCount - 1);
                    case CustomType.DT_TM1638:
                        return (byte)(dataStart + JoyConst.DT1638DataCount - 1);
                    case CustomType.DT_HT16K33:
                        return (byte)(dataStart + JoyConst.DTHT16K33DataCount - 1);
                    case CustomType.Matrix_Max7219:
                        return (byte)(dataStart + _dataCount - 1);
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
                        return (byte)(dataStart + FontDataCount - 1);
                    case CustomType.OUT_StepperMotor:
                    case CustomType.OUT_74HC595:
                    case CustomType.OUT_IO:
                        return (byte)(dataStart + _dataCount - 1);
                    case CustomType.OUT_NRF24:
                    case CustomType.OUT_W5500:
                        return dataStart;
                    default:
                        WarningForm.Instance.OpenUI("Error Custom Type !!!" + Type.ToString(), false);
                        return 0;
                }
            }
        }
        #endregion
        public FontSet[] FontSetList = new FontSet[JoyConst.MaxFontSetCount];
        public int FontDataCount
        #region FontDataCount
        {
            get
            {
                int count = 0;
                for (int i = 0; i < dataCount; i++)
                {
                    if (i >= 0 && i < FontSetList.Length)
                    {
                        count += FontSetList[i].Count;
                    }
                }
                return count;
            }
        }
        #endregion
        ///////////////////////////////////////////////////////////////////////
        public JoyCustom(JoyObject _Parent, int index)
        {
            Parent = _Parent;
            Index = index;
            for (int i = 0; i < JoyConst.MaxFontSetCount; i++)
            {
                FontSetList[i] = new FontSet();
            }
        }
        private bool SetType()
        {
            bool newOK = true;
            List<byte> leftList = Parent.GetLeftCustomPinList();
            switch (_Type)
            {
                case CustomType.NoneCustom:
                    _data = 255;
                    _cs = 255;
                    _clk = 255;
                    _dc = 255;
                    _rst = 255;
                    _cs2 = 255;
                    _cs3 = 255;
                    _cs4 = 255;
                    break;
                //--------------------------
                case CustomType.DT_Max7219:
                case CustomType.Matrix_Max7219:
                case CustomType.DT_TM1638:
                    if (leftList.Count >= 3)
                    {
                        _data = leftList[0];
                        _cs = leftList[1];
                        _clk = leftList[2];
                        _dc = 255;
                        _rst = 255;
                        _cs2 = 255;
                        _cs3 = 255;
                        _cs4 = 255;
                        dataCount = 1;
                    }
                    else
                    {
                        newOK = false;
                    }
                    break;
                //--------------------------
                case CustomType.DT_HT16K33:
                    if (leftList.Count >= 2)
                    {
                        _data = leftList[0];
                        _cs = 255;
                        _clk = leftList[1];
                        _dc = 255;
                        _rst = 255;
                        _cs2 = 255;
                        _cs3 = 255;
                        _cs4 = 255;
                        dataCount = 1;
                    }
                    else
                    {
                        newOK = false;
                    }
                    break;
                //----
                case CustomType.OLED_70_40_SSD1306://0.42
                case CustomType.OLED_48_64_SSD1306://0.71
                case CustomType.OLED_64_32_SSD1306://0.49
                case CustomType.OLED_64_48_SSD1306://0.66
                case CustomType.OLED_96_16_SSD1306://0.86
                case CustomType.OLED_128_32_SSD1306://0.91
                case CustomType.OLED_128_64_SSD1306://0.96, 1.09
                case CustomType.OLED_128_64_SH1106://1.3
                case CustomType.OLED_128_88_SH1107://0.73
                case CustomType.OLED_256_64_SSD1322://3.12
                    if (leftList.Count >= 5)
                    {
                        _data = leftList[0];
                        _cs = leftList[1];
                        _clk = leftList[2];
                        _dc = leftList[3];
                        _rst = leftList[4];
                        _cs2 = 255;
                        _cs3 = 255;
                        _cs4 = 255;
                        dataCount = 1;
                    }
                    else
                    {
                        newOK = false;
                    }
                    break;
                //----
                case CustomType.OLED_70_40_SSD1306x2:
                case CustomType.OLED_48_64_SSD1306x2:
                case CustomType.OLED_64_32_SSD1306x2:
                case CustomType.OLED_64_48_SSD1306x2:
                case CustomType.OLED_96_16_SSD1306x2:
                case CustomType.OLED_128_32_SSD1306x2:
                case CustomType.OLED_128_64_SSD1306x2:
                case CustomType.OLED_128_64_SH1106x2:
                case CustomType.OLED_128_88_SH1107x2:
                case CustomType.OLED_256_64_SSD1322x2:
                    if (leftList.Count >= 6)
                    {
                        _data = leftList[0];
                        _cs = leftList[1];
                        _clk = leftList[2];
                        _dc = leftList[3];
                        _rst = leftList[4];
                        _cs2 = leftList[5];
                        _cs3 = 255;
                        _cs4 = 255;
                        dataCount = 1;
                    }
                    else
                    {
                        newOK = false;
                    }
                    break;
                //----
                case CustomType.OLED_70_40_SSD1306x3:
                case CustomType.OLED_48_64_SSD1306x3:
                case CustomType.OLED_64_32_SSD1306x3:
                case CustomType.OLED_64_48_SSD1306x3:
                case CustomType.OLED_96_16_SSD1306x3:
                case CustomType.OLED_128_32_SSD1306x3:
                case CustomType.OLED_128_64_SSD1306x3:
                case CustomType.OLED_128_64_SH1106x3:
                case CustomType.OLED_128_88_SH1107x3:
                case CustomType.OLED_256_64_SSD1322x3:
                    if (leftList.Count >= 7)
                    {
                        _data = leftList[0];
                        _cs = leftList[1];
                        _clk = leftList[2];
                        _dc = leftList[3];
                        _rst = leftList[4];
                        _cs2 = leftList[5];
                        _cs3 = leftList[6];
                        _cs4 = 255;
                        dataCount = 1;
                    }
                    else
                    {
                        newOK = false;
                    }
                    break;
                //----
                case CustomType.OLED_70_40_SSD1306x4:
                case CustomType.OLED_48_64_SSD1306x4:
                case CustomType.OLED_64_32_SSD1306x4:
                case CustomType.OLED_64_48_SSD1306x4:
                case CustomType.OLED_96_16_SSD1306x4:
                case CustomType.OLED_128_32_SSD1306x4:
                case CustomType.OLED_128_64_SSD1306x4:
                case CustomType.OLED_128_64_SH1106x4:
                case CustomType.OLED_128_88_SH1107x4:
                case CustomType.OLED_256_64_SSD1322x4:
                    if (leftList.Count >= 8)
                    {
                        _data = leftList[0];
                        _cs = leftList[1];
                        _clk = leftList[2];
                        _dc = leftList[3];
                        _rst = leftList[4];
                        _cs2 = leftList[5];
                        _cs3 = leftList[6];
                        _cs4 = leftList[7];
                        dataCount = 1;
                    }
                    else
                    {
                        newOK = false;
                    }
                    break;
                case CustomType.OUT_StepperMotor:
                    if (leftList.Count >= 2)
                    {
                        _data = leftList[0];
                        _cs = 255;
                        _clk = leftList[1];
                        _dc = 255;
                        _rst = 255;
                        _cs2 = 255;
                        _cs3 = 255;
                        _cs4 = 255;
                        dataCount = JoyConst.OUTStepMDataCount;
                    }
                    else
                    {
                        newOK = false;
                    }
                    break;
                case CustomType.OUT_74HC595:
                    if (leftList.Count >= 3)
                    {
                        _data = leftList[0];
                        _cs = leftList[1];
                        _clk = leftList[2];
                        _dc = 255;
                        _rst = 255;
                        _cs2 = 255;
                        _cs3 = 255;
                        _cs4 = 255;
                        dataCount = 1;
                    }
                    else
                    {
                        newOK = false;
                    }
                    break;
                case CustomType.OUT_IO:
                    if (leftList.Count >= 1)
                    {
                        _data = leftList[0];
                        _cs = 255;
                        _clk = 255;
                        _dc = 255;
                        _rst = 255;
                        _cs2 = 255;
                        _cs3 = 255;
                        _cs4 = 255;
                        dataCount = 1;
                    }
                    else
                    {
                        newOK = false;
                    }
                    break;
                case CustomType.OUT_NRF24:
                    if (leftList.Count >= 5)
                    {
                        _data = leftList[0];
                        _cs = leftList[1];
                        _clk = leftList[2];
                        _dc = leftList[3];
                        _rst = leftList[4];
                        _cs2 = 255;
                        _cs3 = 255;
                        _cs4 = 255;
                        dataCount = 1;
                    }
                    else
                    {
                        newOK = false;
                    }
                    break;
                case CustomType.OUT_W5500:
                    if (leftList.Count >= 1)
                    {
                        _data = 255;
                        _cs = 255;
                        _clk = 255;
                        _dc = 255;
                        _rst = leftList[0];
                        _cs2 = 255;
                        _cs3 = 255;
                        _cs4 = 255;
                        dataCount = 1;
                    }
                    else
                    {
                        newOK = false;
                    }
                    break;
                default:
                    _Type = CustomType.NoneCustom;
                    break;
            }
            return newOK;
        }
        #region 同步数据
        public void SyncData(XmlNode x_custom)
        {
            CustomType s_type;
            RotateType s_rotateType;
            byte s_data, s_cs, s_clk, s_dc, s_rst, s_cs2, s_cs3, s_cs4, s_dataStart, s_dataCount;
            if (XmlUI.Instance.GetAttribute<CustomType>(x_custom, "Type", out s_type) &&
                XmlUI.Instance.GetAttribute(x_custom, "rotateType", out s_rotateType) &&
                XmlUI.Instance.GetAttribute(x_custom, "data", out s_data) &&
                XmlUI.Instance.GetAttribute(x_custom, "cs", out s_cs) &&
                XmlUI.Instance.GetAttribute(x_custom, "clk", out s_clk) &&
                XmlUI.Instance.GetAttribute(x_custom, "dc", out s_dc) &&
                XmlUI.Instance.GetAttribute(x_custom, "rst", out s_rst) &&
                XmlUI.Instance.GetAttribute(x_custom, "cs2", out s_cs2) &&
                XmlUI.Instance.GetAttribute(x_custom, "cs3", out s_cs3) &&
                XmlUI.Instance.GetAttribute(x_custom, "cs4", out s_cs4) &&
                XmlUI.Instance.GetAttribute(x_custom, "dataStart", out s_dataStart) &&
                XmlUI.Instance.GetAttribute(x_custom, "dataCount", out s_dataCount))
            {
                _Type = s_type;
                SetType();
                rotateType = s_rotateType;
                _data = s_data;
                _cs = s_cs;
                _clk = s_clk;
                _dc = s_dc;
                _rst = s_rst;
                _cs2 = s_cs2;
                _cs3 = s_cs3;
                _cs4 = s_cs4;
                dataStart = s_dataStart;
                _dataCount = s_dataCount;
            }
            for (int j = 0; j < JoyConst.MaxFontSetCount; j++)
            {
                XmlNode x_fontSet = x_custom.SelectSingleNode("Font" + j);
                byte x, y, LibIndex, Count;
                if (XmlUI.Instance.GetAttribute(x_fontSet, "X", out x) &&
                    XmlUI.Instance.GetAttribute(x_fontSet, "Y", out y) &&
                    XmlUI.Instance.GetAttribute(x_fontSet, "LibIndex", out LibIndex) &&
                    XmlUI.Instance.GetAttribute(x_fontSet, "Count", out Count))
                {
                    FontSetList[j].X = x;
                    FontSetList[j].Y = y;
                    FontSetList[j].LibIndex = LibIndex;
                    FontSetList[j].Count = Count;
                }
            }
        }
        public void SyncData(Report usbReport)
        {
            _Type = (CustomType)usbReport.data[0];
            SetType();
            rotateType = (RotateType)usbReport.data[1];
            _data = usbReport.data[2];
            _cs = usbReport.data[3];
            _clk = usbReport.data[4];
            _dc = usbReport.data[5];
            _rst = usbReport.data[6];
            _cs2 = usbReport.data[7];
            _cs3 = usbReport.data[8];
            _cs4 = usbReport.data[9];
            dataStart = usbReport.data[10];
            _dataCount = usbReport.data[11];
            switch (_Type)
            {
                case CustomType.NoneCustom:
                case CustomType.DT_Max7219:
                case CustomType.DT_TM1638:
                case CustomType.DT_HT16K33:
                //--------------------------
                case CustomType.OUT_StepperMotor:
                //--------------------------单点输出
                case CustomType.OUT_74HC595:
                case CustomType.OUT_IO:
                //--------------------------无线模块
                case CustomType.OUT_NRF24:
                    break;
                case CustomType.OUT_W5500:
                    //MAC
                    Parent.SetMAC(usbReport.data[12], usbReport.data[13], usbReport.data[14], usbReport.data[15], usbReport.data[16], usbReport.data[17]);
                    //Local_IP
                    Parent.SetIP(usbReport.data[18], usbReport.data[19], usbReport.data[20], usbReport.data[21], (ushort)((usbReport.data[22] << 8) + usbReport.data[23]));
                    //Subnet
                    Parent.SetSubNet(usbReport.data[24], usbReport.data[25], usbReport.data[26], usbReport.data[27]);
                    //Gateway
                    Parent.SetGateway(usbReport.data[28], usbReport.data[29], usbReport.data[30], usbReport.data[31]);
                    //DNS
                    Parent.SetDNS(usbReport.data[32], usbReport.data[33], usbReport.data[34], usbReport.data[35]);
                    //Remote_IP
                    Parent.SetServerIP(usbReport.data[36], usbReport.data[37], usbReport.data[38], usbReport.data[39], (ushort)((usbReport.data[40] << 8) + usbReport.data[41]));
                    break;
                default:
                    for (int i = 0; i < JoyConst.MaxFontSetCount; i++)
                    {
                        FontSetList[i].X = usbReport.data[12 + 4 * i];
                        FontSetList[i].Y = usbReport.data[12 + 4 * i + 1];
                        FontSetList[i].LibIndex = usbReport.data[12 + 4 * i + 2];
                        FontSetList[i].Count = usbReport.data[12 + 4 * i + 3];
                    }
                    break;
            }
        }
        public void SyncData(JoyCustom custom)
        {
            _Type = custom._Type;
            SetType();
            rotateType = custom.rotateType;
            _data = custom._data;
            _cs = custom._cs;
            _clk = custom._clk;
            _dc = custom._dc;
            _rst = custom._rst;
            _cs2 = custom._cs2;
            _cs3 = custom._cs3;
            _cs4 = custom._cs4;
            dataStart = custom.dataStart;
            _dataCount = custom._dataCount;
            switch (_Type)
            {
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
                    for (int i = 0; i < JoyConst.MaxFontSetCount; i++)
                    {
                        FontSetList[i].X = custom.FontSetList[i].X;
                        FontSetList[i].Y = custom.FontSetList[i].Y;
                        FontSetList[i].LibIndex = custom.FontSetList[i].LibIndex;
                        FontSetList[i].Count = custom.FontSetList[i].Count;
                    }
                    break;
            }
        }
        #endregion
    }
}
