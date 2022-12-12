using ControllorPlugin;
using SharpDX;
using SharpDX.Direct2D1;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace EasyControl
{
    public class NodeLinkControl : iControl
    {
        static int maxIndex = 0;
        //--------------------------------------------------------
        public bool NodeLinkMode { get; private set; }
        public iControl Parent { get; set; }
        public RectangleF DrawRect { get { return Rect; } }
        private RectangleF _rect = new RectangleF();
        public RectangleF Rect
        {
            private get { return _rect; }
            set
            {
                if (value != _rect)
                {
                    _rect = value;
                }
            }
        }
        public Vector2 _offset = new Vector2();
        public Vector2 Offset
        {
            get
            {
                Vector2 offset = new Vector2(Rect.X + _offset.X, Rect.Y + _offset.Y);
                return offset;
            }
            set
            {
                _offset = value;
            }
        }
        public bool Hide { get; set; } = false;
        public int Index { get; private set; }
        public string Name { get; set; }
        public string PluginID { get; set; }
        public string UIKey { set; private get; }
        //---------------------------------------------------------------------------------
        public Vector2 beforeOffset = new Vector2();
        private float _ScalingValue = 1f;
        public float ScalingValue
        {
            get
            {
                return _ScalingValue;
            }
            private set
            {
                _ScalingValue = value;
                if (_ScalingValue < 0.20f)
                    _ScalingValue = 0.20f;
                if (_ScalingValue > 1f)
                    _ScalingValue = 1f;
            }
        }
        private LayerParameters lp = new LayerParameters();
        private Layer lay;
        private bool moveOn = false;
        private int sourceX = 0;
        private int sourceY = 0;
        #region NodeList
        static Dictionary<string, List<uiNode>> uiPluginNodeList = new Dictionary<string, List<uiNode>>();
        #endregion
        #region customList
        float CustomNodeHeight;
        bool ShowCustomNodeList = false;
        LayoutControl uiCustomNodeList;
        LayoutControl vcCustomNodeListLC;
        Dictionary<string, LuaNode> ipCustomNodeList = new Dictionary<string, LuaNode>();
        #endregion
        ///////////////////////////////////////////////////////////////////////// 
        public static readonly NodeLinkControl Instance = new NodeLinkControl();
        private NodeLinkControl()
        {
        }
        public void Init()
        {
            Index = maxIndex;
            Name = "NodeLinkControl" + maxIndex;
            maxIndex++;
            //-------------------------------------------------------------------
            uiCustomNodeList = XmlUI.Instance.CreateUI(System.Environment.CurrentDirectory + @"\Xml\CustomNodeList.xml");
            vcCustomNodeListLC = XmlUI.Instance.GetLayoutControl("vcCustomNodeListLC");
            //===============================
            DirectoryInfo LuaDir = new DirectoryInfo(System.Environment.CurrentDirectory + @"\LuaNode");
            foreach (FileInfo file in LuaDir.GetFiles())
            {
                string ex = Path.GetExtension(file.FullName);
                if (ex.Equals(".lua"))
                {
                    LuaNode newNode = new LuaNode(file.FullName);
                    newNode.color = XmlUI.DxDeviceGreen;
                    if (ipCustomNodeList.ContainsKey(newNode.GetName()))
                        MessageBox.Show(ipCustomNodeList[newNode.GetName()].path + "\n" + file.FullName + "\n" + Localization.Instance.GetLS("CustomNodeNameError"));
                    else
                        ipCustomNodeList.Add(newNode.GetName(), newNode);
                }
            }
            DirectoryInfo CustomDir = new DirectoryInfo(System.Environment.CurrentDirectory + @"\CustomNode");
            foreach (FileInfo file in CustomDir.GetFiles())
            {
                string ex = Path.GetExtension(file.FullName);
                if (ex.Equals(".lua"))
                {
                    LuaNode newNode = new LuaNode(file.FullName);
                    newNode.color = XmlUI.DxDeviceYellow;
                    if (ipCustomNodeList.ContainsKey(newNode.GetName()))
                        MessageBox.Show(ipCustomNodeList[newNode.GetName()].path + "\n" + file.FullName + "\n" + Localization.Instance.GetLS("CustomNodeNameError"));
                    else
                        ipCustomNodeList.Add(newNode.GetName(), newNode);
                }
            }
            //===============================
            foreach (var item in ipCustomNodeList)
            {
                LayoutControl lcCustomItem = XmlUI.Instance.CreateUI(System.Environment.CurrentDirectory + @"\Xml\CustomNodeItem.xml", item.Key);
                LayoutControl lcCustomNodeItem = XmlUI.Instance.GetLayoutControl(item.Key + "CustomNodeItemLC");
                CustomNodeHeight = lcCustomNodeItem.maxHeight;
                uiButton customPluginBtn = XmlUI.Instance.GetButton(item.Key + "CustomNodeButton");
                customPluginBtn.Name = ipCustomNodeList[item.Key].GetName();
                uiButton plusBtn = XmlUI.Instance.GetButton(item.Key + "CustomNodePlus");
                plusBtn.Name = item.Key;
                plusBtn.LeftButtonClick += OnCustomPluginPlus;
                uiPanel panel = XmlUI.Instance.GetPanel(item.Key + "CustomNodeType");
                panel.ForeColor = item.Value.color;
                vcCustomNodeListLC.AddObject(lcCustomItem);
                AddNewPluginNode(ipCustomNodeList[item.Key], true);
            }
        }
        #region Save&Load
        public void Save()
        {
            if (PublicData.LastVersion == ServerState.Offline)
                return;
            if (PublicData.saveData == null)
                PublicData.saveData = new SaveData();
            PublicData.saveData.OffsetX = _offset.X;
            PublicData.saveData.OffsetY = _offset.Y;
            PublicData.saveData.ScalingValue = ScalingValue;
            lock (uiPluginNodeList)
            {
                foreach (var item in uiPluginNodeList)
                {
                    List<SaveNode> nodeList = new List<SaveNode>();
                    for (int i = 0; i < item.Value.Count; i++)
                    {
                        nodeList.Add(new SaveNode(item.Value[i]));
                    }
                    if (PublicData.saveData.nodeList.ContainsKey(item.Key))
                    {
                        PublicData.saveData.nodeList[item.Key] = nodeList;
                    }
                    else
                    {
                        PublicData.saveData.nodeList.Add(item.Key, nodeList);
                    }
                }
            }
            foreach (var item in UI_PluginControl.pluginList)
            {
                //插件开关
                if (PublicData.saveData.pluginSetList.ContainsKey(item.Key))
                {
                    PublicData.saveData.pluginSetList[item.Key].Open = item.Value.Open;
                    PublicData.saveData.pluginSetList[item.Key].Auto = item.Value.Auto;
                }
                else
                {
                    pluginSet set = new pluginSet();
                    set.Open = item.Value.Open;
                    set.Auto = item.Value.Auto;
                    PublicData.saveData.pluginSetList.Add(item.Key, set);
                }
                //模块开关
                List<bool> nodeOpenList = new List<bool>();
                List<Node> nodeList = item.Value.GetModuleList();
                for (int i = 0; i < nodeList.Count; i++)
                {
                    nodeOpenList.Add(nodeList[i].Open);
                }
                if (PublicData.saveData.pluginNodeList.ContainsKey(item.Key))
                {
                    PublicData.saveData.pluginNodeList[item.Key] = nodeOpenList;
                }
                else
                {
                    PublicData.saveData.pluginNodeList.Add(item.Key, nodeOpenList);
                }
            }
            foreach (var item in ipCustomNodeList)
            {
                List<bool> customOpenList = new List<bool>();
                List<Node> nodeList = ipCustomNodeList[item.Key].GetModuleList();
                for (int j = 0; j < nodeList.Count; j++)
                {
                    customOpenList.Add(nodeList[j].Open);
                }
                if (PublicData.saveData.customNodeList.ContainsKey(item.Key))
                {
                    PublicData.saveData.customNodeList[item.Key] = customOpenList;
                }
                else
                {
                    PublicData.saveData.customNodeList.Add(item.Key, customOpenList);
                }
            }
            PublicData.saveData.Save();
        }
        private void LoadSave()
        {
            Offset = new Vector2(PublicData.saveData.OffsetX, PublicData.saveData.OffsetY);
            ScalingValue = PublicData.saveData.ScalingValue;
            if (PublicData.saveData.nodeList != null)
            {
                foreach (var item in PublicData.saveData.nodeList)
                {
                    if (uiPluginNodeList.ContainsKey(item.Key))
                    {
                        if (item.Value.Count == uiPluginNodeList[item.Key].Count)
                        {
                            for (int i = 0; i < item.Value.Count; i++)
                            {
                                uiPluginNodeList[item.Key][i].Load(item.Value[i]);
                            }
                        }
                    }
                }
            }
            else
            {
                PublicData.saveData.nodeList = new Dictionary<string, List<SaveNode>>();
            }
            if (PublicData.saveData.pluginSetList != null)
            {
                foreach (var item in PublicData.saveData.pluginSetList)
                {
                    if (UI_PluginControl.pluginList.ContainsKey(item.Key))
                    {
                        UI_PluginControl.pluginList[item.Key].Open = item.Value.Open;
                        UI_PluginControl.pluginList[item.Key].Auto = item.Value.Auto;
                    }
                }
            }
            else
            {
                PublicData.saveData.pluginSetList = new Dictionary<string, pluginSet>();
            }
            if (PublicData.saveData.pluginNodeList != null)
            {
                foreach (var item in PublicData.saveData.pluginNodeList)
                {
                    if (UI_PluginControl.pluginList.ContainsKey(item.Key))
                    {
                        List<Node> nodeList = UI_PluginControl.pluginList[item.Key].GetModuleList();
                        if (nodeList.Count == item.Value.Count)
                        {
                            for (int i = 0; i < item.Value.Count; i++)
                            {
                                nodeList[i].Open = item.Value[i];
                            }
                        }
                    }
                }
            }
            else
            {
                PublicData.saveData.pluginNodeList = new Dictionary<string, List<bool>>();
            }
            if (PublicData.saveData.customNodeList != null)
            {
                foreach (var item in PublicData.saveData.customNodeList)
                {
                    for (int j = 0; j < item.Value.Count; j++)
                    {
                        if (ipCustomNodeList.ContainsKey(item.Key))
                        {
                            List<Node> nodeList = ipCustomNodeList[item.Key].GetModuleList();
                            if (j < nodeList.Count)
                            {
                                nodeList[j].Open = PublicData.saveData.customNodeList[item.Key][j];
                            }
                        }
                    }
                }
            }
            else
            {
                PublicData.saveData.customNodeList = new Dictionary<string, List<bool>>();
            }
        }
        public bool LoadAs(string path)
        {
            PublicData.saveData = new SaveData();
            if (PublicData.saveData.LoadAs(path))
            {
                LoadSave();
                return true;
            }
            return false;
        }
        public void Load()
        {
            PublicData.saveData = new SaveData();
            if (PublicData.saveData.Load())
            {
                LoadSave();
            }
        }
        #endregion
        private void OnCustomPluginPlus(object sender, EventArgs e)
        {
            uiButton btn = (uiButton)sender;
            InterfacePlugin ip = ipCustomNodeList[btn.Name];
            List<Node> nodeList = ip.GetModuleList();
            if (uiPluginNodeList.ContainsKey(ip.PluginID))
            {
                List<uiNode> uiNodeList = uiPluginNodeList[ip.PluginID];
                for (int i = 0; i < uiNodeList.Count; i++)
                {
                    if (!uiNodeList[i].parentNode.Open)
                    {
                        uiNodeList[i].parentNode.Open = true;
                        uiNodeList[i].Offset = new Vector2();
                        return;
                    }
                }
            }
        }
        public void AddNewPluginNode(InterfacePlugin ip, bool close)
        {
            ip.Init();
            string id = ip.PluginID;
            List<Node> nodeList = ip.GetModuleList();
            if (nodeList != null && nodeList.Count > 0)
            {
                List<uiNode> uiNodeList = new List<uiNode>();
                for (int nodeIndex = 0; nodeIndex < nodeList.Count; nodeIndex++)
                {
                    Node _node = nodeList[nodeIndex];
                    if (_node.NodePortList.Count > 0)
                    {
                        uiNode node = new uiNode(id, nodeIndex, ip, _node, close);
                        node.Parent = this;
                        uiNodeList.Add(node);
                    }
                }
                if (uiNodeList.Count > 0)
                {
                    lock (uiPluginNodeList)
                    {
                        uiPluginNodeList.Add(id, uiNodeList);
                        if (PublicData.saveData != null && PublicData.saveData.nodeList.ContainsKey(id))
                        {
                            if (PublicData.saveData.nodeList[id].Count == uiPluginNodeList[id].Count)
                            {
                                for (int i = 0; i < PublicData.saveData.nodeList[id].Count; i++)
                                {
                                    uiPluginNodeList[id][i].Load(PublicData.saveData.nodeList[id][i]);
                                }
                            }
                        }
                    }
                }
            }
        }
        public void DeletePluginNode(InterfacePlugin ip)
        {
            string id = ip.PluginID;
            lock (uiPluginNodeList)
                uiPluginNodeList.Remove(id);
        }
        public void TileAllNode()
        {
            float x = JoyConst.MaxNodeWidth + 10f;
            lock (uiPluginNodeList)
            {
                foreach (List<uiNode> uiNodeList in uiPluginNodeList.Values)
                {
                    for (int i = 0; i < uiNodeList.Count; i++)
                    {
                        uiNode node = uiNodeList[i];
                        node.SetPosition((int)x, 0);
                        x += node.DrawRect.Width + 10f;
                    }
                }
            }
        }
        public void ScreneAlign()
        {
            lock (uiPluginNodeList)
            {
                foreach (List<uiNode> _node in uiPluginNodeList.Values)
                {
                    for (int i = 0; i < _node.Count; i++)
                    {
                        uiNode nodeUI = _node[i];
                        if (nodeUI.parentInterfacePlugin.Open && nodeUI.parentNode.Open)
                        {
                            Vector2 pointA = new Vector2(nodeUI.Offset.X * ScalingValue, nodeUI.Offset.Y * ScalingValue);
                            Vector2 pointB = new Vector2((nodeUI.Offset.X + nodeUI.DrawRect.Width) * ScalingValue, (nodeUI.Offset.Y + nodeUI.DrawRect.Height) * ScalingValue);
                            float x = nodeUI.Offset.X, y = nodeUI.Offset.Y;
                            if (pointA.X < -Offset.X && pointB.X < -Offset.X)
                            {
                                //DebugConstol.AddLog(nodeUI.Name + " - L");
                                x = -Offset.X / ScalingValue;
                            }
                            if (pointA.Y < -Offset.Y && pointB.Y < -Offset.Y)
                            {
                                //DebugConstol.AddLog(nodeUI.Name + " - U");
                                y = -Offset.Y / ScalingValue;
                            }
                            if (pointA.X > Dx2D.Instance.Width - Offset.X && pointB.X > Dx2D.Instance.Width - Offset.X)
                            {
                                //DebugConstol.AddLog(nodeUI.Name + " - R");
                                x = (Dx2D.Instance.Width - Offset.X) / ScalingValue - nodeUI.DrawRect.Width;
                            }
                            if (pointA.Y > Dx2D.Instance.Height - Offset.Y && pointB.Y > Dx2D.Instance.Height - Offset.Y)
                            {
                                //DebugConstol.AddLog(nodeUI.Name + " - D");
                                y = (Dx2D.Instance.Height - Offset.Y) / ScalingValue - nodeUI.DrawRect.Height;
                            }
                            nodeUI.Offset = new Vector2(x, y);
                        }
                    }
                }
            }
            //Offset = new Vector2(0, 0);
        }
        public uiPort GetPort(NodePortLink npl)
        {
            if (uiPluginNodeList.ContainsKey(npl.PluginID))
            {
                List<uiNode> nodeList = uiPluginNodeList[npl.PluginID];
                if (npl.NodeIndex >= 0 && npl.NodeIndex < nodeList.Count)
                {
                    uiNode node = nodeList[npl.NodeIndex];
                    if (node.parentInterfacePlugin.Open)
                    {
                        if (npl.PortIndex >= 0 && npl.PortIndex < node.portList.Count)
                        {
                            uiPort port = node.portList[npl.PortIndex];
                            return port;
                        }
                    }
                }
            }
            return null;
        }
        #region 鼠标操作
        public void SetNodeCentered(string name, int index)
        {
            if (uiPluginNodeList.ContainsKey(name))
            {
                List<uiNode> nodeList = uiPluginNodeList[name];
                if (index >= 0 && index < nodeList.Count)
                {
                    uiNode node = nodeList[index];
                    if (!node.Hide)
                    {
                        _offset.X = (Rect.Width / 2) - (node.DrawRect.X + node.Offset.X + node.DrawRect.Width / 2) * ScalingValue;
                        _offset.Y = (Rect.Height / 2) - (node.DrawRect.Y + node.Offset.Y + node.DrawRect.Height / 2) * ScalingValue;
                    }
                }
            }
        }
        public void JoyMouseDownEvent(MouseEventArgs e)
        {
            if (Hide) return;
            uiCustomNodeList.JoyMouseDownEvent(e);
            if (e.Button == MouseButtons.Right &&
                e.X >= Rect.X && e.X < Rect.X + Rect.Width &&
                e.Y >= Rect.Y && e.Y < Rect.Y + Rect.Height)
            {
                moveOn = true;
                beforeOffset = _offset;
                sourceX = e.X;
                sourceY = e.Y;
            }
            else
            {
                moveOn = false;
            }
            PublicData.SelectPort = null;
            lock (uiPluginNodeList)
            {
                foreach (List<uiNode> _node in uiPluginNodeList.Values)
                {
                    for (int i = 0; i < _node.Count; i++)
                    {
                        uiNode nodeUI = _node[i];
                        if (nodeUI.parentInterfacePlugin.Open)
                        {
                            for (int j = 0; j < _node[i].portList.Count; j++)
                            {
                                if (_node[i].portList[j].InSide(e))
                                {
                                    PublicData.SelectPort = _node[i].portList[j];
                                }
                            }
                        }
                    }
                }
                foreach (List<uiNode> _node in uiPluginNodeList.Values)
                {
                    for (int nodeIndex = 0; nodeIndex < _node.Count; nodeIndex++)
                    {
                        uiNode nodeUI = _node[nodeIndex];
                        nodeUI.JoyMouseDownEvent(e);
                    }
                }
            }
        }
        public void JoyMouseUpEvent(MouseEventArgs e)
        {
            if (Hide) return;
            uiCustomNodeList.JoyMouseUpEvent(e);
            moveOn = false;
            lock (uiPluginNodeList)
            {
                foreach (List<uiNode> _node in uiPluginNodeList.Values)
                {
                    for (int i = 0; i < _node.Count; i++)
                    {
                        uiNode nodeUI = _node[i];
                        if (nodeUI.parentInterfacePlugin.Open)
                        {
                            for (int j = 0; j < _node[i].portList.Count; j++)
                            {
                                if (_node[i].portList[j].InSide(e))
                                {
                                    if (PublicData.SelectPort != null &&
                                        PublicData.SelectPort.nodePort.IO != _node[i].portList[j].nodePort.IO /*&&
                                    PublicData.SelectPort.nodePort.Type == _node[i].portList[j].nodePort.Type*/)
                                    {
                                        uiPort inPort, outPort = null;
                                        if (_node[i].portList[j].nodePort.IO == PortType.In)
                                        {
                                            inPort = _node[i].portList[j];
                                            outPort = PublicData.SelectPort;
                                        }
                                        else
                                        {
                                            inPort = PublicData.SelectPort;
                                            outPort = _node[i].portList[j];
                                        }
                                        NodePortLink nplOut = new NodePortLink(outPort.PluginID, outPort.NodeIndex, outPort.Index);
                                        if (!inPort.AddNodePortLink(nplOut))
                                        {
                                            WarningForm.Instance.OpenUI("NodePortLinkError");
                                            inPort.ClearNodePortLink();
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                PublicData.SelectPort = null;
                foreach (List<uiNode> _node in uiPluginNodeList.Values)
                {
                    for (int nodeIndex = 0; nodeIndex < _node.Count; nodeIndex++)
                    {
                        uiNode nodeUI = _node[nodeIndex];
                        nodeUI.JoyMouseUpEvent(e);
                    }
                }
            }
        }
        public void JoyMouseMoveEvent(MouseEventArgs e)
        {
            if (Hide) return;
            uiCustomNodeList.JoyMouseMoveEvent(e);
            if (moveOn)
            {
                _offset.X = beforeOffset.X - (sourceX - e.X);
                _offset.Y = beforeOffset.Y - (sourceY - e.Y);
            }
            lock (uiPluginNodeList)
            {
                foreach (List<uiNode> _node in uiPluginNodeList.Values)
                {
                    for (int nodeIndex = 0; nodeIndex < _node.Count; nodeIndex++)
                    {
                        uiNode nodeUI = _node[nodeIndex];
                        nodeUI.JoyMouseMoveEvent(e);
                    }
                }
            }
            if (ShowCustomNodeList)
            {
                if (e.X >= Rect.X + JoyConst.CustomNodeListX && e.X < Rect.X + JoyConst.CustomNodeListX + JoyConst.CustomNodeListWidth &&
                    e.Y >= Rect.Y && e.Y < Rect.Y + Dx2D.Instance.Height - 50)
                {
                    ShowCustomNodeList = true;
                }
                else
                {
                    ShowCustomNodeList = false;
                }
            }
            else
            {
                if (e.X >= Rect.X + JoyConst.CustomNodeListX && e.X < Rect.X + JoyConst.CustomNodeListX + JoyConst.CustomNodeListWidth &&
                    e.Y >= Rect.Y && e.Y < Rect.Y + JoyConst.CustomNodeListHeight)
                {
                    ShowCustomNodeList = true;
                }
                else
                {
                    ShowCustomNodeList = false;
                }
            }
        }
        public void JoyMouseMoveWheel(MouseEventArgs e)
        {
            if (Hide) return;
            uiCustomNodeList.JoyMouseMoveWheel(e);
            if (e.X >= Rect.X && e.X < Rect.X + Rect.Width &&
                e.Y >= Rect.Y && e.Y < Rect.Y + Rect.Height)
            {
                float oldValue = ScalingValue;
                ScalingValue += e.Delta > 0 ? 0.05f : -0.05f;
                if (oldValue != ScalingValue)
                {
                    float x = (e.X - Offset.X) / (Rect.Width * oldValue) * Rect.Width * (ScalingValue - oldValue);
                    float y = (e.Y - Offset.Y) / (Rect.Height * oldValue) * Rect.Height * (ScalingValue - oldValue);
                    _offset.X -= x;
                    _offset.Y -= y;
                }
            }
            lock (uiPluginNodeList)
            {
                foreach (List<uiNode> _node in uiPluginNodeList.Values)
                {
                    for (int nodeIndex = 0; nodeIndex < _node.Count; nodeIndex++)
                    {
                        uiNode nodeUI = _node[nodeIndex];
                        nodeUI.JoyMouseMoveWheel(e);
                    }
                }
            }
        }
        #endregion
        #region 计算连线
        public void NodeLinkLogic()
        {
            lock (uiPluginNodeList)
            {
                foreach (List<uiNode> _node in uiPluginNodeList.Values)
                {
                    for (int nodeIndex = 0; nodeIndex < _node.Count; nodeIndex++)
                    {
                        uiNode nodeUI = _node[nodeIndex];
                        if (nodeUI.parentInterfacePlugin.Open && nodeUI.parentNode.Open)
                        {
                            for (int portIndex = 0; portIndex < nodeUI.portList.Count; portIndex++)
                            {
                                uiPort port = nodeUI.portList[portIndex];
                                NodePort np = port.nodePort;
                                List<string> removeList = new List<string>();
                                foreach (NodePortLink npl in port.portLinkList.Values)
                                {
                                    uiPort tarPort = GetPort(npl);
                                    if (tarPort != null)
                                    {
                                        NodePort tarNp = tarPort.nodePort;
                                        if (np.IO != tarNp.IO /*&& np.Type == tarNp.Type*/)
                                        {
                                            switch (np.Type)
                                            {
                                                case PortValue.Int64:
                                                    long valueInt = GetNodePortIntValue(tarNp);
                                                    if (np.ValueInt64 != valueInt)
                                                    {
                                                        np.ValueInt64 = valueInt;
                                                        nodeUI.portValueChange(portIndex);
                                                    }
                                                    break;
                                                case PortValue.Double:
                                                    double valueFloat = GetNodePortFloatValue(tarNp);
                                                    if (np.ValueDouble != valueFloat)
                                                    {
                                                        np.ValueDouble = valueFloat;
                                                        nodeUI.portValueChange(portIndex);
                                                    }
                                                    break;
                                                case PortValue.String:
                                                    string valueString = GetNodePortStringValue(tarNp);
                                                    if (!np.ValueString.Equals(valueString))
                                                    {
                                                        np.ValueString = valueString;
                                                        nodeUI.portValueChange(portIndex);
                                                    }
                                                    break;
                                            }
                                        }
                                        else
                                        {
                                            removeList.Add(npl.Key);
                                        }
                                    }
                                }
                                foreach (string npKey in removeList)
                                {
                                    port.portLinkList.Remove(npKey);
                                }
                            }
                        }
                    }
                }
            }
        }
        long GetNodePortIntValue(NodePort np)
        {
            switch (np.Type)
            {
                case PortValue.Int64:
                    return np.ValueInt64;
                case PortValue.Double:
                    return (int)(np.ValueDouble + 0.5f);
                case PortValue.String:
                    int value = 0;
                    if (int.TryParse(np.ValueString, out value))
                    {
                        return value;
                    }
                    break;
            }
            return 0;
        }
        double GetNodePortFloatValue(NodePort np)
        {
            switch (np.Type)
            {
                case PortValue.Int64:
                    return np.ValueInt64;
                case PortValue.Double:
                    return np.ValueDouble;
                case PortValue.String:
                    float value = 0;
                    if (float.TryParse(np.ValueString, out value))
                    {
                        return value;
                    }
                    break;
            }
            return 0f;
        }
        string GetNodePortStringValue(NodePort np)
        {
            switch (np.Type)
            {
                case PortValue.Int64:
                    return np.ValueInt64.ToString();
                case PortValue.Double:
                    return np.ValueDouble.ToString();
                case PortValue.String:
                    return np.ValueString;
            }
            return "";
        }
        #endregion
        public void Dx2DResize()
        {
            if (Hide) return;
            lock (uiPluginNodeList)
            {
                foreach (List<uiNode> _node in uiPluginNodeList.Values)
                {
                    for (int nodeIndex = 0; nodeIndex < _node.Count; nodeIndex++)
                    {
                        uiNode nodeUI = _node[nodeIndex];
                        nodeUI.Dx2DResize();
                    }
                }
            }
            uiCustomNodeList.Dx2DResize();
        }
        public void DxRenderLogic()
        {
            if (Hide) return;
            vcCustomNodeListLC.Rect = new RectangleF(0, 0, JoyConst.CustomNodeListWidth - ViewControl.sliderWidth, CustomNodeHeight * ipCustomNodeList.Count);
            uiCustomNodeList.Rect = new RectangleF(Rect.X + JoyConst.CustomNodeListX, Rect.Y, JoyConst.CustomNodeListWidth, Dx2D.Instance.Height - 50);
            uiCustomNodeList.DxRenderLogic();
            lock (uiPluginNodeList)
            {
                foreach (List<uiNode> _node in uiPluginNodeList.Values)
                {
                    for (int nodeIndex = 0; nodeIndex < _node.Count; nodeIndex++)
                    {
                        uiNode nodeUI = _node[nodeIndex];
                        nodeUI.Hide = true;
                        if (nodeUI.parentInterfacePlugin.Open && nodeUI.parentNode.Open)
                        {
                            nodeUI.Hide = false;
                        }
                        if (nodeUI.parentInterfacePlugin.Open && nodeUI.parentNode.Open)
                        {
                            nodeUI.DxRenderLogic();
                        }
                    }
                }
            }
        }
        public void DxRenderHigh()
        {
            if (Hide) return;
        }

        public void DxRenderMedium()
        {
            if (Hide) return;
            lp.ContentBounds = Rect;
            lp.LayerOptions = LayerOptions.InitializeForCleartype;
            lp.Opacity = 1f;
            lay = new Layer(Dx2D.Instance.RenderTarget2D);
            Dx2D.Instance.RenderTarget2D.PushLayer(ref lp, lay);
            Matrix3x2 offsetT = Matrix3x2.Translation(Offset);
            Matrix3x2 scaleT = Matrix3x2.Scaling(ScalingValue, ScalingValue, Offset);
            Dx2D.Instance.RenderTarget2D.Transform = offsetT * scaleT;
            #region 绘制node
            lock (uiPluginNodeList)
            {
                foreach (List<uiNode> _node in uiPluginNodeList.Values)
                {
                    for (int i = 0; i < _node.Count; i++)
                    {
                        uiNode nodeUI = _node[i];
                        if (nodeUI.parentInterfacePlugin.Open && nodeUI.parentNode.Open)
                        {
                            _node[i].DxRenderLow();
                            _node[i].DxRenderMedium();
                            _node[i].DxRenderHigh();
                        }
                    }
                }
            }
            #endregion
            #region 模块连线
            lock (uiPluginNodeList)
            {
                foreach (List<uiNode> _node in uiPluginNodeList.Values)
                {
                    for (int nodeIndex = 0; nodeIndex < _node.Count; nodeIndex++)
                    {
                        uiNode nodeUI = _node[nodeIndex];
                        if (nodeUI.parentInterfacePlugin.Open && nodeUI.parentNode.Open)
                        {
                            for (int portIndex = 0; portIndex < nodeUI.portList.Count; portIndex++)
                            {
                                uiPort port = nodeUI.portList[portIndex];
                                foreach (NodePortLink npl in port.portLinkList.Values)
                                {
                                    uiPort tarPort = GetPort(npl);
                                    if (tarPort != null && tarPort.parentNode != null && tarPort.parentNode.parentNode.Open)
                                    {
                                        Color4 color = PublicData.GetIOColor(port.nodePort.Type);
                                        Dx2D.Instance.DrawBezier(new Vector2(port.DrawRect.X + port.DrawRect.Width / 2f, port.DrawRect.Y + port.DrawRect.Height / 2f),
                                            new Vector2(tarPort.DrawRect.X + tarPort.DrawRect.Width / 2f, tarPort.DrawRect.Y + tarPort.DrawRect.Height / 2f),
                                            tarPort.nodePort.IO, color, port.nodePort.isDash());
                                        Ellipse epsS = new Ellipse(port.portRect.Point, port.portRect.RadiusX * 0.5f, port.portRect.RadiusX * 0.5f);
                                        Dx2D.Instance.RenderTarget2D.FillEllipse(epsS, Dx2D.Instance.GetSolidColorBrush(color));
                                        Ellipse epsE = new Ellipse(tarPort.portRect.Point, tarPort.portRect.RadiusX * 0.5f, tarPort.portRect.RadiusX * 0.5f);
                                        Dx2D.Instance.RenderTarget2D.FillEllipse(epsE, Dx2D.Instance.GetSolidColorBrush(color));
                                    }
                                    else
                                    {
                                        //port.portLinkList.Remove(npl.Key);
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            #endregion
            #region 鼠标连线
            if (PublicData.SelectPort != null)
            {
                Color4 color = PublicData.GetIOColor(PublicData.SelectPort.nodePort.Type);
                Dx2D.Instance.DrawBezier((PublicData.MousePoint - Offset) / ScalingValue,
                    new Vector2(PublicData.SelectPort.DrawRect.X + PublicData.SelectPort.DrawRect.Width / 2f,
                    PublicData.SelectPort.DrawRect.Y + PublicData.SelectPort.DrawRect.Height / 2f),
                    PublicData.SelectPort.nodePort.IO, color, PublicData.SelectPort.nodePort.isDash());
            }
            #endregion
            Dx2D.Instance.RenderTarget2D.Transform = Matrix3x2.Identity;
            Dx2D.Instance.RenderTarget2D.PopLayer();
            lay.Dispose();

            #region CustomNodeList
            if (ShowCustomNodeList)
            {
                Dx2D.Instance.RenderTarget2D.DrawRectangle(uiCustomNodeList.DrawRect, Dx2D.Instance.GetSolidColorBrush(XmlUI.DxUIBackColor), 4f);
                uiCustomNodeList.DxRenderLow();
                uiCustomNodeList.DxRenderMedium();
                uiCustomNodeList.DxRenderHigh();
            }
            else
            {
                Dx2D.Instance.RenderTarget2D.FillRectangle(
                    new RectangleF(Rect.X + JoyConst.CustomNodeListX, Rect.Y, JoyConst.CustomNodeListWidth, JoyConst.CustomNodeListHeight),
                    Dx2D.Instance.GetSolidColorBrush(XmlUI.DxDeviceGreen));
                Dx2D.Instance.RenderTarget2D.DrawRectangle(
                    new RectangleF(Rect.X + JoyConst.CustomNodeListX, Rect.Y, JoyConst.CustomNodeListWidth, JoyConst.CustomNodeListHeight),
                    Dx2D.Instance.GetSolidColorBrush(XmlUI.DxUIBackColor), 2f);
                Dx2D.Instance.RenderTarget2D.DrawLine(
                    new Vector2(Rect.X + JoyConst.CustomNodeListX + 32f, Rect.Y + JoyConst.CustomNodeListHeight / 2f),
                    new Vector2(Rect.X + JoyConst.CustomNodeListX + JoyConst.CustomNodeListWidth - 32f, Rect.Y + JoyConst.CustomNodeListHeight / 2f),
                    Dx2D.Instance.GetSolidColorBrush(XmlUI.DxBackColor), 2f);
            }
            #endregion
            #region 缩略图
            float scaleCount = 1f / 4f;
            float maxWidth = Rect.Width * scaleCount;
            float maxHeight = Rect.Height * scaleCount;
            float x = Rect.X + Rect.Width * (1f - scaleCount) - 10f;
            float y = Rect.Y + Rect.Height * (1f - scaleCount) - 10f;
            Dx2D.Instance.RenderTarget2D.FillRectangle(new RectangleF(x, y, maxWidth, maxHeight), Dx2D.Instance.GetSolidColorBrush(XmlUI.DxBackColor, 0.1f));
            Dx2D.Instance.RenderTarget2D.DrawRectangle(new RectangleF(x, y, maxWidth, maxHeight), Dx2D.Instance.GetSolidColorBrush(XmlUI.DxDeviceGreen, 0.2f), 2f);
            float left = Rect.X - Offset.X;
            float right = Rect.X - Offset.X + Rect.Width;
            float top = Rect.Y - Offset.Y;
            float bottom = Rect.Y - Offset.Y + Rect.Height;
            lock (uiPluginNodeList)
            {
                foreach (List<uiNode> _node in uiPluginNodeList.Values)
                {
                    for (int i = 0; i < _node.Count; i++)
                    {
                        uiNode nodeUI = _node[i];
                        if (nodeUI.parentInterfacePlugin.Open && nodeUI.parentNode.Open)
                        {
                            if (_node[i].DrawRect.X + _node[i].Offset.X * ScalingValue < left)
                                left = _node[i].DrawRect.X + _node[i].Offset.X * ScalingValue;
                            if (_node[i].DrawRect.Y + _node[i].Offset.Y * ScalingValue < top)
                                top = _node[i].DrawRect.Y + _node[i].Offset.Y * ScalingValue;
                            if (_node[i].DrawRect.X + (_node[i].Offset.X + _node[i].DrawRect.Width) * ScalingValue > right)
                                right = _node[i].DrawRect.X + (_node[i].Offset.X + _node[i].DrawRect.Width) * ScalingValue;
                            if (_node[i].DrawRect.Y + (_node[i].Offset.Y + _node[i].DrawRect.Height) * ScalingValue > bottom)
                                bottom = _node[i].DrawRect.Y + (_node[i].Offset.Y + _node[i].DrawRect.Height) * ScalingValue;
                        }
                    }
                }
            }
            float wScale = Rect.Width / (right - left);
            float hScale = Rect.Height / (bottom - top);
            float scale = wScale < hScale ? wScale : hScale;
            scale *= scaleCount;
            RectangleF rectT = new RectangleF(
                x + (Rect.X - Offset.X - left) * scale,
                y + (Rect.Y - Offset.Y - top) * scale,
                Rect.Width * scale,
                Rect.Height * scale);
            Dx2D.Instance.RenderTarget2D.FillRectangle(rectT, Dx2D.Instance.GetSolidColorBrush(XmlUI.DxDeviceBlue, 0.1f));
            Dx2D.Instance.RenderTarget2D.DrawRectangle(rectT, Dx2D.Instance.GetSolidColorBrush(XmlUI.DxDeviceBlue, 0.2f), 2f);
            lock (uiPluginNodeList)
            {
                foreach (List<uiNode> _node in uiPluginNodeList.Values)
                {
                    for (int i = 0; i < _node.Count; i++)
                    {
                        uiNode nodeUI = _node[i];
                        if (nodeUI.parentInterfacePlugin.Open && nodeUI.parentNode.Open)
                        {
                            RectangleF rectNode = new RectangleF(
                             x + (_node[i].DrawRect.X + _node[i].Offset.X * ScalingValue - left) * scale,
                             y + (_node[i].DrawRect.Y + _node[i].Offset.Y * ScalingValue - top) * scale,
                             _node[i].DrawRect.Width * scale * ScalingValue,
                             _node[i].DrawRect.Height * scale * ScalingValue);
                            Dx2D.Instance.RenderTarget2D.FillRectangle(rectNode, Dx2D.Instance.GetSolidColorBrush(XmlUI.DxTextColor, 0.1f));
                            Dx2D.Instance.RenderTarget2D.DrawRectangle(rectNode, Dx2D.Instance.GetSolidColorBrush(XmlUI.DxTextColor, 0.2f), 2f);
                        }
                    }
                }
            }
            #endregion
        }

        public void DxRenderLow()
        {
            if (Hide) return;
            Dx2D.Instance.RenderTarget2D.DrawRectangle(Rect, Dx2D.Instance.GetSolidColorBrush(XmlUI.DxUIBackColor), 4f);
        }
    }
}
