using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace EasyControl
{
    public class pluginSet
    {
        public bool Open;
        public bool Auto;
    }
    public class SaveData
    {
        string savePath = System.Environment.CurrentDirectory + @"\save.xml";
        public float OffsetX = 0f;
        public float OffsetY = 0f;
        public float ScalingValue = 1f;
        public Dictionary<string, List<SaveNode>> nodeList = new Dictionary<string, List<SaveNode>>();
        public Dictionary<string, pluginSet> pluginSetList = new Dictionary<string, pluginSet>();
        public Dictionary<string, List<bool>> pluginNodeList = new Dictionary<string, List<bool>>();
        public Dictionary<string, List<bool>> customNodeList = new Dictionary<string, List<bool>>();
        public SaveData()
        {
        }
        public void SaveAs(string path)
        {
            XmlDocument xmlDoc = new XmlDocument();
            XmlElement save = xmlDoc.CreateElement("Save");
            #region offset
            XmlElement offset = xmlDoc.CreateElement("Offset");
            offset.SetAttribute("X", OffsetX.ToString());
            offset.SetAttribute("Y", OffsetY.ToString());
            offset.SetAttribute("Scaling", ScalingValue.ToString());
            save.AppendChild(offset);
            #endregion
            #region nodeList
            XmlElement x_nodeList = xmlDoc.CreateElement("nodeList");
            foreach (var item in nodeList)
            {
                XmlElement x_nodeListItem = xmlDoc.CreateElement("nl-" + item.Key);
                #region List<SaveNode>
                for (int i = 0; i < item.Value.Count; i++)
                {
                    XmlElement x_SaveNode = xmlDoc.CreateElement("SaveNode" + i);
                    #region SaveNode
                    SaveNode sn = item.Value[i];
                    x_SaveNode.SetAttribute("sourceOffsetX", sn.sourceOffsetX.ToString());
                    x_SaveNode.SetAttribute("sourceOffsetY", sn.sourceOffsetY.ToString());
                    XmlElement x_portList = xmlDoc.CreateElement("portList");
                    #region List<SavePort>
                    for (int j = 0; j < sn.portList.Count; j++)
                    {
                        XmlElement x_SavePort = xmlDoc.CreateElement("SavePort" + j);
                        #region SavePort
                        SavePort sp = sn.portList[j];
                        foreach (var port in sp.portLinkList)
                        {
                            NodePortLink npl = port.Value;
                            XmlElement x_portLink = xmlDoc.CreateElement("sp-" + port.Key);
                            #region NodePortLink
                            x_portLink.SetAttribute("PluginID", npl.PluginID);
                            x_portLink.SetAttribute("NodeIndex", npl.NodeIndex.ToString());
                            x_portLink.SetAttribute("PortIndex", npl.PortIndex.ToString());
                            #endregion
                            x_SavePort.AppendChild(x_portLink);
                        }
                        #endregion
                        x_portList.AppendChild(x_SavePort);
                    }
                    #endregion
                    x_SaveNode.AppendChild(x_portList);
                    #endregion
                    x_nodeListItem.AppendChild(x_SaveNode);
                }
                #endregion
                x_nodeList.AppendChild(x_nodeListItem);
            }
            save.AppendChild(x_nodeList);
            #endregion
            #region pluginList
            XmlElement x_pluginList = xmlDoc.CreateElement("pluginList");
            foreach (var item in pluginSetList)
            {
                XmlElement x_pluginOpen = xmlDoc.CreateElement("pl-" + item.Key);
                x_pluginOpen.SetAttribute("Open", item.Value.Open.ToString());
                x_pluginOpen.SetAttribute("Auto", item.Value.Auto.ToString());
                x_pluginList.AppendChild(x_pluginOpen);
            }
            save.AppendChild(x_pluginList);
            #endregion
            #region pluginNodeList
            XmlElement x_pluginNodeList = xmlDoc.CreateElement("pluginNodeList");
            foreach (var item in pluginNodeList)
            {
                XmlElement x_pluginNode = xmlDoc.CreateElement("pn-" + item.Key);
                for (int i = 0; i < item.Value.Count; i++)
                {
                    XmlElement x_pluginNodeOpen = xmlDoc.CreateElement("pluginNodeOpen" + i);
                    x_pluginNodeOpen.SetAttribute("Open", item.Value[i].ToString());
                    x_pluginNode.AppendChild(x_pluginNodeOpen);
                }
                x_pluginNodeList.AppendChild(x_pluginNode);
            }
            save.AppendChild(x_pluginNodeList);
            #endregion
            #region customNodeLink
            XmlElement x_customNodeLinkList = xmlDoc.CreateElement("customNodeLinkList");
            foreach (var item in customNodeList)
            {
                XmlElement x_customNode = xmlDoc.CreateElement("cn-" + item.Key);
                for (int i = 0; i < item.Value.Count; i++)
                {
                    XmlElement x_customNodeOpen = xmlDoc.CreateElement("customNodeOpen" + i);
                    x_customNodeOpen.SetAttribute("Open", item.Value[i].ToString());
                    x_customNode.AppendChild(x_customNodeOpen);
                }
                x_customNodeLinkList.AppendChild(x_customNode);
            }
            save.AppendChild(x_customNodeLinkList);
            #endregion
            xmlDoc.AppendChild(save);
            xmlDoc.Save(path);
        }
        public void Save()
        {
            SaveAs(savePath);
        }
        public bool Load()
        {
            if (LoadAs(savePath))
            {
                return true;
            }
            return false;
        }
        public bool LoadAs(string path)
        {
            #region 读取
            if (!File.Exists(path))
            {
                return false;
            }
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(path);
            #endregion
            return LoadXml(xmlDoc);
        }
        public bool LoadXml(XmlDocument xmlDoc)
        {
            if (xmlDoc == null)
                return false;
            XmlNode save = xmlDoc.SelectSingleNode("Save");
            if (save != null && save.HasChildNodes)
            {
                #region Offset
                XmlNode offset = save.SelectSingleNode("Offset");
                XmlUI.Instance.GetAttribute(offset, "X", out OffsetX);
                XmlUI.Instance.GetAttribute(offset, "Y", out OffsetY);
                XmlUI.Instance.GetAttribute(offset, "Scaling", out ScalingValue);
                #endregion
                #region nodeList
                nodeList.Clear();
                XmlNode x_nodeList = save.SelectSingleNode("nodeList");
                if (x_nodeList != null && x_nodeList.HasChildNodes)
                {
                    foreach (XmlNode nodeListNode in x_nodeList.ChildNodes)
                    {
                        string nodeListKey = nodeListNode.Name.Substring(3);
                        List<SaveNode> saveNodeList = new List<SaveNode>();
                        for (int i = 0; i < nodeListNode.ChildNodes.Count; i++)
                        {
                            #region SaveNode
                            XmlNode x_saveNode = nodeListNode.SelectSingleNode("SaveNode" + i);
                            SaveNode sn = new SaveNode();
                            XmlUI.Instance.GetAttribute(x_saveNode, "sourceOffsetX", out sn.sourceOffsetX);
                            XmlUI.Instance.GetAttribute(x_saveNode, "sourceOffsetY", out sn.sourceOffsetY);
                            #endregion
                            #region portList
                            XmlNode x_portList = x_saveNode.SelectSingleNode("portList");
                            if (x_portList != null && x_portList.HasChildNodes)
                            {
                                for (int j = 0; j < x_portList.ChildNodes.Count; j++)
                                {
                                    XmlNode x_savePort = x_portList.SelectSingleNode("SavePort" + j);
                                    SavePort sp = new SavePort();
                                    foreach (XmlNode portLink in x_savePort.ChildNodes)
                                    {
                                        string id;
                                        int nodeIndex;
                                        int portIndex;
                                        if (XmlUI.Instance.GetAttribute(portLink, "PluginID", out id) &&
                                            XmlUI.Instance.GetAttribute(portLink, "NodeIndex", out nodeIndex) &&
                                            XmlUI.Instance.GetAttribute(portLink, "PortIndex", out portIndex))
                                        {
                                            NodePortLink npl = new NodePortLink(id, nodeIndex, portIndex);
                                            sp.portLinkList.Add(npl.Key, npl);
                                        }
                                    }
                                    sn.portList.Add(sp);
                                }
                            }
                            saveNodeList.Add(sn);
                            #endregion
                        }
                        nodeList.Add(nodeListKey, saveNodeList);
                    }
                }
                #endregion
                #region pluginList
                pluginSetList.Clear();
                XmlNode x_pluginList = save.SelectSingleNode("pluginList");
                if (x_pluginList != null && x_pluginList.HasChildNodes)
                {
                    foreach (XmlNode pluginListNode in x_pluginList)
                    {
                        bool open = true;
                        XmlUI.Instance.GetAttribute(pluginListNode, "Open", out open);
                        bool auto = false;
                        XmlUI.Instance.GetAttribute(pluginListNode, "Auto", out auto);
                        pluginSet set = new pluginSet();
                        set.Open = open;
                        set.Auto = auto;
                        pluginSetList.Add(pluginListNode.Name.Substring(3), set);
                    }
                }
                #endregion
                #region pluginNodeList
                pluginNodeList.Clear();
                XmlNode x_pluginNodeList = save.SelectSingleNode("pluginNodeList");
                if (x_pluginNodeList != null && x_pluginNodeList.HasChildNodes)
                {
                    foreach (XmlNode pluginNodeListNode in x_pluginNodeList)
                    {
                        List<bool> nodeListOpen = new List<bool>();
                        for (int i = 0; i < pluginNodeListNode.ChildNodes.Count; i++)
                        {
                            XmlNode pluginNodeOpen = pluginNodeListNode.SelectSingleNode("pluginNodeOpen" + i);
                            bool open = true;
                            XmlUI.Instance.GetAttribute(pluginNodeOpen, "Open", out open);
                            nodeListOpen.Add(open);
                        }
                        pluginNodeList.Add(pluginNodeListNode.Name.Substring(3), nodeListOpen);
                    }
                }
                #endregion
                #region customNodeLink
                customNodeList.Clear();
                XmlNode x_customNodeLinkList = save.SelectSingleNode("customNodeLinkList");
                if (x_customNodeLinkList != null && x_customNodeLinkList.HasChildNodes)
                {
                    foreach (XmlNode customNodeListNode in x_customNodeLinkList)
                    {
                        List<bool> openList = new List<bool>();
                        for (int j = 0; j < customNodeListNode.ChildNodes.Count; j++)
                        {
                            XmlNode customNodeOpen = customNodeListNode.SelectSingleNode("customNodeOpen" + j);
                            bool open = false;
                            XmlUI.Instance.GetAttribute(customNodeOpen, "Open", out open);
                            openList.Add(open);
                        }
                        customNodeList.Add(customNodeListNode.Name.Substring(3), openList);
                    }
                }
                #endregion
            }
            return true;
        }
    }
}
