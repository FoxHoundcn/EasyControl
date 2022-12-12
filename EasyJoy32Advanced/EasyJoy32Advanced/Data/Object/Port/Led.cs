namespace EasyControl
{
    public class Led
    {
        public int Index { get; private set; }
        //--------------------------------------------------------------
        public bool SoftValue = false;
        #region 同步数据
        public LedControlType ControlType = 0;
        public byte R = 0;
        public byte G = 0;
        public byte B = 0;
        public byte OpenR = 0;
        public byte OpenG = 0;
        public byte OpenB = 0;
        #endregion
        //--------------------------------------------------------------
        public Led(int index)
        {
            Index = index;
        }
    }
}
