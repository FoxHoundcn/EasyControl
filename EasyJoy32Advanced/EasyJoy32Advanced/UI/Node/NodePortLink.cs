namespace EasyControl
{
    public class NodePortLink
    {
        public string Key
        {
            get
            {
                return PluginID + NodeIndex.ToString() + PortIndex.ToString();
            }
        }
        public string PluginID { get; private set; }
        public int NodeIndex { get; private set; }
        public int PortIndex { get; private set; }
        public NodePortLink(string _ID, int _NodeIndex, int _PortIndex)
        {
            PluginID = _ID;
            NodeIndex = _NodeIndex;
            PortIndex = _PortIndex; ;
        }
        public static bool operator !=(NodePortLink a1, NodePortLink a2)
        {
            if (a1.PluginID != a2.PluginID || a1.NodeIndex != a2.NodeIndex || a1.PortIndex != a2.PortIndex)
                return true;
            else
                return false;
        }
        public static bool operator ==(NodePortLink a1, NodePortLink a2)
        {
            if (a1.PluginID == a2.PluginID && a1.NodeIndex == a2.NodeIndex && a1.PortIndex == a2.PortIndex)
                return true;
            else
                return false;
        }
        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            NodePortLink a2 = obj as NodePortLink;
            if ((System.Object)a2 == null)
            {
                return false;
            }

            if (PluginID == a2.PluginID && NodeIndex == a2.NodeIndex && PortIndex == a2.PortIndex)
                return true;
            else
                return false;
        }

        public override int GetHashCode()
        {
            return PortIndex;
        }
    }
}
