using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyControl
{
    public class Hat
    {
        public int Index { get; private set; }
        //--------------------------------------------------------------
        #region 同步数据
        public byte[] InputIndex = new byte[4] { 0, 1, 2, 3 };//up,left,down,right
        #endregion
        public bool[] State = new bool[4] { false, false, false, false };
        //--------------------------------------------------------------
        public Hat(int index)
        {
            Index = index;
        }
    }
}
