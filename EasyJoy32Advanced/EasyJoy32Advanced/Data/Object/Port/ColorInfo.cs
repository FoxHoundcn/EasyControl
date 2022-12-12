using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyControl
{
    public class ColorInfo
    {
        public int Index { get; private set; }
        //--------------------------------------------------------------
        #region 同步数据
        public ColorInfoType ClickMode = ColorInfoType.ColorNone;
        public ColorInfoType FnMode = ColorInfoType.ColorNone;
        public UInt16 Offset = 0;
        #endregion
        //--------------------------------------------------------------
        public ColorInfo(int index)
        {
            Index = index;
        }
    }
}
