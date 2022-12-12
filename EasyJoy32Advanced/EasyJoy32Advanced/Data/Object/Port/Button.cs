using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyControl
{
    public class Button
    {
        public int Index { get; private set; }
        //--------------------------------------------------------------
        public bool SoftValue = false;
        #region 同步数据
        public byte Code = 0;
        public byte Fun = 0;
        public byte CodeFN = 0;
        public byte FunFN = 0;
        #endregion
        //--------------------------------------------------------------
        public Button(int index)
        {
            Index = index;
        }
    }
}
