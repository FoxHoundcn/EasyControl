namespace EasyControl
{
    public class Format
    {
        public int Index { get; private set; }
        //--------------------------------------------------------------
        #region 同步数据
        public bool Reverse = false;
        public bool AutoRange = false;
        public bool Calibration = false;
        public byte Shift = 0;
        public int minValue = 0;
        public int midValue = 2047;
        public int maxValue = 4095;
        public byte minDzone = 0;
        public byte maxDzone = 0;
        public byte midDzone = 0;
        #endregion
        //--------------------------------------------------------------
        public Format(int index)
        {
            Index = index;
        }
    }
}
