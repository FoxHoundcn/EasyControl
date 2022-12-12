namespace EasyControl
{
    public class FontSet
    {
        public byte X = 0;
        public byte Y = 0;
        byte _libIndex = 0;//字库编号
        public byte LibIndex
        {
            get { return _libIndex; }
            set
            {
                if (_libIndex == value)
                    return;
                _libIndex = value;
                if (_libIndex < 0)
                    _libIndex = 0;
                if (_libIndex >= JoyConst.MaxFont)
                    _libIndex = JoyConst.MaxFont - 1;
            }
        }
        byte _count = 1;//字符数量
        public byte Count
        {
            get { return _count; }
            set
            {
                if (_count == value)
                    return;
                _count = value;
                if (_count < JoyConst.MinDataCount)
                    _count = JoyConst.MinDataCount;
                if (_count > JoyConst.MaxDataCount)
                    _count = JoyConst.MaxDataCount;
            }
        }
        public FontSet()
        {

        }
    }
}
