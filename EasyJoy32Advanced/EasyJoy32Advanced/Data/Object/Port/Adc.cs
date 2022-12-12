using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyControl
{
    public class Adc
    {
        public int Index { get; private set; }
        //--------------------------------------------------------------
        #region 同步数据
        public byte maxCC = 5;
        public byte maxCY = 5;
        public byte maxPC = 15;
        #endregion
        //--------------------------------------------------------------
        public Adc(int index)
        {
            Index = index;
        }
    }
}
