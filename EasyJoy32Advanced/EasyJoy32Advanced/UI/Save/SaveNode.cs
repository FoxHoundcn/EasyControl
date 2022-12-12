using System.Collections.Generic;

namespace EasyControl
{
    public class SaveNode
    {
        public float sourceOffsetX = 0f;
        public float sourceOffsetY = 0f;
        public List<SavePort> portList = new List<SavePort>();
        public SaveNode()
        {

        }
        public SaveNode(uiNode node)
        {
            sourceOffsetX = node.Offset.X;
            sourceOffsetY = node.Offset.Y;
            for (int i = 0; i < node.portList.Count; i++)
            {
                portList.Add(new SavePort(node.portList[i]));
            }
        }
    }
}
