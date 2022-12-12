using System;
using System.Collections.Generic;

namespace EasyControl
{
    public class eFont
    {
        #region 同步数据
        public UInt16 StartIndex = 0;
        #region StartChar
        private byte _startChar = 0;
        public byte StartChar
        {
            get { return _startChar; }
            set
            {
                if (value < EndChar)
                    _startChar = value;
            }
        }
        #endregion
        #region EndChar
        private byte _endChar = JoyConst.MaxFontCount;
        public byte EndChar
        {
            get { return _endChar; }
            set
            {
                if (value > _startChar && value <= JoyConst.MaxFontCount)
                    _endChar = value;
            }
        }
        #endregion
        public byte MaxChar { get { return (byte)(EndChar - StartChar); } }
        public int MaxCharLength { get { return FontWidth * FontHeight * MaxChar; } }
        public List<PortShowType> GetCharEnable()
        {
            List<PortShowType> charList = new List<PortShowType>();
            for (int i = 0; i < JoyConst.MaxFontCount; i++)
            {
                if (i < StartChar || i >= EndChar)
                {
                    charList.Add(PortShowType.None);
                }
                else
                {
                    charList.Add(PortShowType.Used);
                }
            }
            return charList;
        }
        #region FontWidth
        private byte _fontWidth = 8;
        public byte FontWidth
        {
            get
            {
                return _fontWidth;
            }
            set
            {
                if (value >= JoyConst.MinFontWidth && value <= JoyConst.MaxFontWidth)
                    _fontWidth = value;
            }
        }
        #endregion
        #region FontHeight
        private byte _fontHeight = JoyConst.MinFontHeight;
        public byte FontHeight
        {
            get
            {
                return _fontHeight;
            }
            set
            {
                if (value >= JoyConst.MinFontHeight && value <= JoyConst.MaxFontHeight)
                    _fontHeight = value;
            }
        }
        #endregion
        public int FontSize { get { return FontWidth * FontHeight; } }
        #endregion
        #region SelectFontIndex
        private int _selectFontIndex = 0;
        public int SelectFontIndex
        {
            get
            {
                return _selectFontIndex;
            }
            set
            {
                if (_selectFontIndex != value)
                {
                    _selectFontIndex = value;
                }
            }
        }
        #endregion
        private byte[] byteArray = new byte[JoyConst.MaxFontWidth * JoyConst.MaxFontHeight * JoyConst.MaxFontCount];
        public bool GetFontByte(int index, out byte data)
        {
            if (index >= 0 && index < byteArray.Length)
            {
                data = byteArray[index];
                return true;
            }
            data = 0;
            return false;
        }
        public bool SetFontByte(int index, byte data)
        {
            if (index >= 0 && index < byteArray.Length)
            {
                byteArray[index] = data;
                return true;
            }
            return false;
        }
        public byte[] GetFontByteArrayAll()
        {
            byte[] arrayFont = new byte[FontSize * JoyConst.MaxFontCount];
            for (int i = 0; i < arrayFont.Length; i++)
            {
                arrayFont[i] = byteArray[i];
            }
            return arrayFont;
        }
        public byte[] GetFontByteArray()
        {
            byte[] arrayFont = new byte[MaxCharLength];
            for (int i = 0; i < arrayFont.Length; i++)
            {
                arrayFont[i] = byteArray[FontSize * StartChar + i];
            }
            return arrayFont;
        }
        public eFont()
        {

        }
    }
}
