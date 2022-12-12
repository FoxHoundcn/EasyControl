using System.Collections.Generic;

namespace EasyControl
{
    public class SavePort
    {
        public Dictionary<string, NodePortLink> portLinkList = new Dictionary<string, NodePortLink>();
        public SavePort()
        {

        }
        public SavePort(uiPort port)
        {
            portLinkList = port.portLinkList;
        }
    }
}
