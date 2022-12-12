using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyControl
{
    public class JoyIndexChangeArgs : EventArgs
    {
        public string PluginID;
        public string UIKey;
        public int Index;
        public JoyIndexChangeArgs(string _id, string _key, int _index)
        {
            PluginID = _id;
            UIKey = _key;
            Index = _index;
        }
    }
}
