using ControllorPlugin;
using SharpDX;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using System.Xml;

namespace EasyControl
{
    public delegate void ButtonLeftClickHandler(string id);
    public delegate void ButtonRightClickHandler(string id);
    public delegate void DefWndProcHandler(int message);
    public delegate void SwitchButtonChangeHandler(string id, bool value);
    public delegate void TextEditorChangeHandler(string id, string value);
    public delegate void TrackBarChangeHandler(string id, int value);
    public delegate void ValueChangeEventHandler(int mIndex, int pIndex);
    public delegate void UpdataHandler();
    public class UI_PluginControl : iUiLogic
    {
        public LayoutControl mainLayout { get; private set; }
        //------------------------------------------------------------------------------------------------------
        LayoutControl pluginListLC;
        LayoutControl nodeLC;
        LayoutControl propertyLC;
        public static Dictionary<string, InterfacePlugin> pluginList { private set; get; } = new Dictionary<string, InterfacePlugin>();
        public static Dictionary<string, XmlDocument> pluginXmlList { private set; get; } = new Dictionary<string, XmlDocument>();
        public static Dictionary<string, string> pluginXmlPathList { private set; get; } = new Dictionary<string, string>();
        static Dictionary<string, LayoutControl> pluginSelectList = new Dictionary<string, LayoutControl>();//插件列表
        static Dictionary<string, List<LayoutControl>> pluginNodeList = new Dictionary<string, List<LayoutControl>>();//模块列表
        static Dictionary<string, LayoutControl> propertyList = new Dictionary<string, LayoutControl>();//属性列表
        static string SelectPluginName = "";
        float lcPluginHeight = 0;
        float lcNodeHeight = 0;
        public bool ready { get; private set; } = false;
        //------------------------------------------------------------------------------------------------------
        public static readonly UI_PluginControl Instance = new UI_PluginControl();
        private UI_PluginControl()
        {
        }
        //============================================================
        public void Update()
        {
            if (!ready) return;
            List<InterfacePlugin> removeList = new List<InterfacePlugin>();
            foreach (InterfacePlugin ip in pluginList.Values)
            {
                try
                {
                    if (ip.Open)
                    {
                        UpdataHandler handler = new UpdataHandler(ip.Update);
                        handler.BeginInvoke(null, null);
                        // ip.Updata();
                    }
                }
                catch (Exception ex)
                {
                    removeList.Add(ip);
                    DebugConstol.AddLog("Plugin Update\n" + ex.Message + ex.StackTrace, LogType.Warning);
                }
            }
            if (removeList.Count > 0)
            {
                foreach (var ip in removeList)
                {
                    pluginList.Remove(ip.PluginID);
                }
            }
            //插件Update
            foreach (JoyObject joyObj in JoyUSB.eJoyList.Values)
            {
                UpdataHandler handler = new UpdataHandler(joyObj.Update);
                handler.BeginInvoke(null, null);
            }
        }
        public void DxRenderLogic()
        {
            try
            {
                if (!ready) return;
                //----------------------------------------------------------------------------------------------------------------------
                LayoutControl lcControl = XmlUI.Instance.GetLayoutControl("PluginListControl");
                LayoutControl lcNodeControl = XmlUI.Instance.GetLayoutControl("PluginNodeList");
                LayoutControl lcPropertyControl = XmlUI.Instance.GetLayoutControl("PluginControlProperty");
                float maxHeight = 0f;
                float height = 0f;
                foreach (InterfacePlugin ip in pluginList.Values)
                {
                    List<Node> nodeList = ip.GetModuleList();
                    if (nodeList == null)
                        continue;
                    height = 0f;
                    height += lcPluginHeight;
                    uiSwitchButton pluginSB = XmlUI.Instance.GetSwitchButton(ip.PluginID + "PluginSwitch");
                    pluginSB.bSwitchOn = ip.Open;
                    uiSwitchButton pluginAO = XmlUI.Instance.GetSwitchButton(ip.PluginID + "PluginAuto");
                    pluginAO.bSwitchOn = ip.Auto;
                    for (int i = 0; i < nodeList.Count; i++)
                    {
                        Node node = nodeList[i];
                        uiSwitchButton nodeOpen = XmlUI.Instance.GetSwitchButton(ip.GetName() + i + "PluginNodeOpen");
                        nodeOpen.bSwitchOn = node.Open;
                    }
                    uiButton pluginProperty = XmlUI.Instance.GetButton(ip.PluginID + "PluginProperty");
                    if (SelectPluginName.Equals(ip.PluginID))
                    {
                        pluginProperty.BackColor = XmlUI.DxDeviceBlue;
                        #region 插件模块
                        foreach (LayoutControl _lcNode in pluginNodeList[ip.PluginID])
                        {
                            _lcNode.Hide = false;
                        }
                        float maxListHeight = lcNodeHeight * pluginNodeList[ip.PluginID].Count;
                        if (maxListHeight < lcNodeControl.DrawRect.Height)
                            nodeLC.Rect = new RectangleF(0f, 0f, lcNodeControl.DrawRect.Width, maxListHeight);
                        else
                            nodeLC.Rect = new RectangleF(0f, 0f, lcNodeControl.DrawRect.Width - ViewControl.sliderWidth, maxListHeight);
                        #endregion
                        #region 插件属性
                        if (propertyList.ContainsKey(ip.PluginID))
                        {
                            propertyList[ip.PluginID].Hide = false;
                            float maxPropertyHeight = JoyConst.WindowsWidth;
                            LayoutControl _pluginMain = XmlUI.Instance.GetLayoutControl(ip.PluginID + ip.GetName() + "Main");
                            if (_pluginMain != null)
                                maxPropertyHeight = _pluginMain.maxHeight;
                            if (maxPropertyHeight < lcPropertyControl.DrawRect.Height)
                                propertyLC.Rect = new RectangleF(0f, 0f, lcPropertyControl.DrawRect.Width, maxPropertyHeight);
                            else
                                propertyLC.Rect = new RectangleF(0f, 0f, lcPropertyControl.DrawRect.Width - ViewControl.sliderWidth, maxPropertyHeight);
                        }
                        #endregion
                    }
                    else
                    {
                        pluginProperty.BackColor = XmlUI.DxUIBackColor;
                        foreach (LayoutControl _lcNode in pluginNodeList[ip.PluginID])
                        {
                            _lcNode.Hide = true;
                        }
                        if (propertyList.ContainsKey(ip.PluginID))
                            propertyList[ip.PluginID].Hide = true;
                    }
                    #region 计算占位
                    pluginSelectList[ip.PluginID].Placeholder = height / lcPluginHeight;
                    #endregion
                    maxHeight += height;
                }
                if (maxHeight < pluginListLC.DrawRect.Height)
                    pluginListLC.Rect = new RectangleF(0f, 0f, lcControl.DrawRect.Width, maxHeight);
                else
                    pluginListLC.Rect = new RectangleF(0f, 0f, lcControl.DrawRect.Width - ViewControl.sliderWidth, maxHeight);
            }
            catch (Exception ex)
            {
                DebugConstol.AddLog("Plugin DxRenderLogic\n" + ex.Message + ex.StackTrace, LogType.Warning);
            }
        }

        public void Init()
        {
            mainLayout = XmlUI.Instance.CreateUI(System.Environment.CurrentDirectory + @"\Xml\UI\UI_PluginControl.xml");
            //----
            uiButton backBtn = XmlUI.Instance.GetButton("UI_PluginControl_Back");
            backBtn.LeftButtonClick += pluginControlBack;
            LoadAllPlugins();
            #region dxInput插件
            pluginList.Add(DxInput.Instance.PluginID, DxInput.Instance);
            #endregion
            #region objView
            pluginListLC = XmlUI.Instance.GetLayoutControl("PluginListLC");
            nodeLC = XmlUI.Instance.GetLayoutControl("vcPluginNodeListLC");
            propertyLC = XmlUI.Instance.GetLayoutControl("vcPluginPropertyListLC");
            //----
            foreach (InterfacePlugin ip in pluginList.Values)
            {
                //插件加载
                NodeLinkControl.Instance.AddNewPluginNode(ip, true);
                List<Node> nodeList = ip.GetModuleList();
                if (nodeList == null)
                    continue;
                if (ip.Open)
                {
                    if (ip.Auto)
                        ip.AutoOpen();
                }
                LayoutControl newPlugin = XmlUI.Instance.CreateUI(System.Environment.CurrentDirectory + @"\Xml\PluginSelect.xml", ip.PluginID);
                pluginListLC.AddObject(newPlugin);
                pluginSelectList.Add(ip.PluginID, newPlugin);
                LayoutControl lcPlugin = XmlUI.Instance.GetLayoutControl(ip.PluginID + "PluginSelectLC");
                lcPlugin.Placeholder = 1;
                lcPluginHeight = lcPlugin.maxHeight;
                uiButton pluginProperty = XmlUI.Instance.GetButton(ip.PluginID + "PluginProperty");
                pluginProperty.Name = ip.GetName();
                pluginProperty.PluginID = ip.PluginID;
                pluginProperty.LeftButtonClick += OnPluginProperty;
                uiSwitchButton pluginSB = XmlUI.Instance.GetSwitchButton(ip.PluginID + "PluginSwitch");
                //pluginSB.Name = ip.GetName();
                pluginSB.PluginID = ip.PluginID;
                pluginSB.ValueChange += OnPluginSwitchChange;
                uiSwitchButton pluginAO = XmlUI.Instance.GetSwitchButton(ip.PluginID + "PluginAuto");
                //pluginSB.Name = ip.GetName();
                pluginAO.PluginID = ip.PluginID;
                pluginAO.ValueChange += OnPluginAutoChange;
                //-------------------------------------------------------------------------------------------------------
                List<LayoutControl> tempNodeList = new List<LayoutControl>();
                for (int j = 0; j < nodeList.Count; j++)
                {
                    LayoutControl newNode = XmlUI.Instance.CreateUI(System.Environment.CurrentDirectory + @"\Xml\PluginNode.xml", ip.GetName() + j.ToString());
                    newNode.Hide = true;
                    tempNodeList.Add(newNode);
                    nodeLC.AddObject(newNode);
                    LayoutControl lcNode = XmlUI.Instance.GetLayoutControl(ip.GetName() + j + "PluginNodeLC");
                    lcNode.Info = nodeList[j].Info;
                    lcNode.InfoLocalization = false;
                    lcNodeHeight = lcNode.maxHeight;
                    uiSwitchButton nodeOpen = XmlUI.Instance.GetSwitchButton(ip.GetName() + j + "PluginNodeOpen");
                    nodeOpen.Index = j;
                    nodeOpen.PluginID = ip.PluginID;
                    nodeOpen.bSwitchOn = nodeList[j].Open;
                    nodeOpen.ValueChange += OnOpenNode;
                    uiButton nodeBtn = XmlUI.Instance.GetButton(ip.GetName() + j + "PluginNodeIndex");
                    nodeBtn.Index = j;
                    nodeBtn.PluginID = ip.PluginID;
                    nodeBtn.Name = nodeList[j].Name;
                    nodeBtn.LeftButtonClick += OnClickNode;
                }
                pluginNodeList.Add(ip.PluginID, tempNodeList);
                #region 配置属性页
                if (propertyList.ContainsKey(ip.PluginID))
                {
                    propertyLC.AddObject(propertyList[ip.PluginID]);
                }
                #endregion
            }
            NodeLinkControl.Instance.TileAllNode();
            ready = true;
            #endregion
            #region 事件       
            XmlUI.Instance.ButtonLeftClick += OnButtonLeftClick;
            XmlUI.Instance.ButtonRightClick += OnButtonRightClick;
            XmlUI.Instance.SwitchButtonChange += OnSwitchButtonChange;
            XmlUI.Instance.TextEditorChange += OnTextEditorChange;
            XmlUI.Instance.TrackBarChange += OnTrackBarChange;
            #endregion
        }

        private void pluginControlBack(object sender, EventArgs e)
        {
            PublicData.ui_Type = UIType.NodeLink;
        }

        #region 插件列表操作
        private void OnPluginProperty(object sender, EventArgs e)
        {
            JoyIndexChangeArgs args = (JoyIndexChangeArgs)e;
            SelectPluginName = args.PluginID;
        }
        private void OnPluginSwitchChange(object sender, EventArgs e)
        {
            JoyIndexChangeArgs args = (JoyIndexChangeArgs)e;
            uiSwitchButton sbtn = (uiSwitchButton)sender;
            if (pluginList.ContainsKey(args.PluginID))
            {
                pluginList[args.PluginID].Open = sbtn.bSwitchOn;
                if (!pluginList[args.PluginID].Open)
                    pluginList[args.PluginID].Auto = false;

            }
        }
        private void OnPluginAutoChange(object sender, EventArgs e)
        {
            JoyIndexChangeArgs args = (JoyIndexChangeArgs)e;
            uiSwitchButton sbtn = (uiSwitchButton)sender;
            if (pluginList.ContainsKey(args.PluginID))
            {
                if (pluginList[args.PluginID].Open)
                    pluginList[args.PluginID].Auto = sbtn.bSwitchOn;
            }
        }
        #endregion
        #region Node列表操作
        private void OnOpenNode(object sender, EventArgs e)
        {
            JoyIndexChangeArgs args = (JoyIndexChangeArgs)e;
            uiSwitchButton sbtn = (uiSwitchButton)sender;
            if (pluginList.ContainsKey(args.PluginID))
            {
                List<Node> nodeList = pluginList[args.PluginID].GetModuleList();
                if (args.Index >= 0 && args.Index < nodeList.Count)
                {
                    nodeList[args.Index].Open = !nodeList[args.Index].Open;
                    if (nodeList[args.Index].Open)
                        pluginList[args.PluginID].Open = true;
                    sbtn.bSwitchOn = nodeList[args.Index].Open;
                }
            }
        }
        private void OnClickNode(object sender, EventArgs e)
        {
            JoyIndexChangeArgs args = (JoyIndexChangeArgs)e;
            NodeLinkControl.Instance.SetNodeCentered(args.PluginID, args.Index);
        }
        #endregion
        #region 插件控件事件
        #region UI事件
        private bool GetUIKey(JoyIndexChangeArgs args, out InterfacePlugin ip, out string id)
        {
            foreach (string key in pluginList.Keys)
            {
                if (args.UIKey.Length > key.Length)
                {
                    string pluginID = args.UIKey.Substring(0, key.Length);
                    if (pluginID.Equals(key))
                    {
                        ip = pluginList[key];
                        if (ip != null)
                        {
                            string pluginName = ip.GetName();
                            if (key.Length + pluginName.Length < args.UIKey.Length &&
                                args.UIKey.Contains(key + pluginName))
                            {
                                id = args.UIKey.Substring(key.Length + pluginName.Length);
                                return true;
                            }
                        }
                    }
                }
            }
            ip = null;
            id = "";
            return false;
        }
        private void OnButtonLeftClick(object sender, EventArgs e)
        {
            try
            {
                JoyIndexChangeArgs args = e as JoyIndexChangeArgs;
                InterfacePlugin ip;
                string id;
                if (GetUIKey(args, out ip, out id))
                {
                    ButtonLeftClickHandler handler = new ButtonLeftClickHandler(ip.OnButtonLeftClick);
                    handler.BeginInvoke(id, null, null);
                    //ip.ButtonLeftClick(id);
                }
            }
            catch (Exception ex)
            {
                DebugConstol.AddLog("Plugin OnButtonLeftClick\n" + ex.Message + ex.StackTrace, LogType.Warning);
            }
        }
        private void OnButtonRightClick(object sender, EventArgs e)
        {
            try
            {
                JoyIndexChangeArgs args = e as JoyIndexChangeArgs;
                InterfacePlugin ip;
                string id;
                if (GetUIKey(args, out ip, out id))
                {
                    ButtonRightClickHandler handler = new ButtonRightClickHandler(ip.OnButtonRightClick);
                    handler.BeginInvoke(id, null, null);
                    //ip.ButtonRightClick(id);
                }
            }
            catch (Exception ex)
            {
                DebugConstol.AddLog("Plugin OnButtonRightClick\n" + ex.Message + ex.StackTrace, LogType.Warning);
            }
        }
        private void OnSwitchButtonChange(object sender, EventArgs e)
        {
            try
            {
                JoyIndexChangeArgs args = e as JoyIndexChangeArgs;
                InterfacePlugin ip;
                string id;
                if (GetUIKey(args, out ip, out id))
                {
                    uiSwitchButton sb = XmlUI.Instance.GetSwitchButton(args.UIKey);
                    if (sb != null)
                    {
                        SwitchButtonChangeHandler handler = new SwitchButtonChangeHandler(ip.OnSwitchButtonChange);
                        handler.BeginInvoke(id, sb.bSwitchOn, null, null);
                        //ip.SwitchButtonChange(id, sb.bSwitchOn);
                        if (pluginXmlPathList.ContainsKey(ip.GetName()) && pluginXmlList.ContainsKey(ip.GetName()))
                            CheckSettingXML(pluginXmlPathList[ip.GetName()], pluginXmlList[ip.GetName()], "SwitchButton", id, sb.bSwitchOn.ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                DebugConstol.AddLog("Plugin OnSwitchButtonChange\n" + ex.Message + ex.StackTrace, LogType.Warning);
            }
        }
        private void OnTextEditorChange(object sender, EventArgs e)
        {
            try
            {
                JoyIndexChangeArgs args = e as JoyIndexChangeArgs;
                InterfacePlugin ip;
                string id;
                if (GetUIKey(args, out ip, out id))
                {
                    uiTextEditor te = XmlUI.Instance.GetTextEditor(args.UIKey);
                    if (te != null)
                    {
                        TextEditorChangeHandler handler = new TextEditorChangeHandler(ip.OnTextEditorChange);
                        handler.BeginInvoke(id, te.Text, null, null);
                        //ip.TextEditorChange(id, te.Text);
                        if (pluginXmlPathList.ContainsKey(ip.GetName()) && pluginXmlList.ContainsKey(ip.GetName()))
                            CheckSettingXML(pluginXmlPathList[ip.GetName()], pluginXmlList[ip.GetName()], "TextEditor", id, te.Text);
                    }
                }
            }
            catch (Exception ex)
            {
                DebugConstol.AddLog("Plugin OnTextEditorChange\n" + ex.Message + ex.StackTrace, LogType.Warning);
            }
        }
        private void OnTrackBarChange(object sender, EventArgs e)
        {
            try
            {
                JoyIndexChangeArgs args = e as JoyIndexChangeArgs;
                InterfacePlugin ip;
                string id;
                if (GetUIKey(args, out ip, out id))
                {
                    uiTrackBar tb = XmlUI.Instance.GetTrackBar(args.UIKey);
                    if (tb != null)
                    {
                        TrackBarChangeHandler handler = new TrackBarChangeHandler(ip.OnTrackBarChange);
                        handler.BeginInvoke(id, tb.Value, null, null);
                        //ip.TrackBarChange(id, tb.Value);
                        if (pluginXmlPathList.ContainsKey(ip.GetName()) && pluginXmlList.ContainsKey(ip.GetName()))
                            CheckSettingXML(pluginXmlPathList[ip.GetName()], pluginXmlList[ip.GetName()], "TrackBar", id, tb.Value.ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                DebugConstol.AddLog("Plugin OnTrackBarChange\n" + ex.Message + ex.StackTrace, LogType.Warning);
            }
        }
        private void CheckSettingXML(string path, XmlDocument xmlDoc, string setType = "", string id = "", string value = "")
        {
            XmlNode rootNode = xmlDoc.SelectSingleNode("Plugin");
            if (rootNode != null && rootNode.HasChildNodes)
            {
                XmlNode moduleNode = rootNode.SelectSingleNode("Settings");
                if (moduleNode != null)
                {
                    #region 保存属性
                    if (!setType.Equals(""))
                    {
                        List<XmlNode> delete = new List<XmlNode>();
                        foreach (XmlNode node in moduleNode)
                        {
                            string _id = "";
                            if (node.Name == setType && XmlUI.Instance.GetAttribute(node, "ID", out _id))
                            {
                                if (_id.Equals(id))
                                    delete.Add(node);
                            }
                        }
                        for (int i = 0; i < delete.Count; i++)
                        {
                            moduleNode.RemoveChild(delete[i]);
                        }
                        XmlElement newNode = xmlDoc.CreateElement(setType);
                        newNode.SetAttribute("ID", id);
                        newNode.SetAttribute("Value", value);
                        moduleNode.AppendChild(newNode);
                    }
                    #endregion
                }
                else
                {
                    XmlElement newNode = xmlDoc.CreateElement("Settings");
                    rootNode.AppendChild(newNode);
                }
            }
            else
            {
                XmlElement newNode = xmlDoc.CreateElement("Plugin");
                xmlDoc.AppendChild(newNode);
            }
            //存回xml
            xmlDoc.Save(path);
        }
        #endregion
        #endregion

        #region LoadDll
        private void LoadAllPlugins()
        {
            try
            {
                pluginList.Clear();
                #region 加载DLL
                string[] pluginPathArray = Directory.GetFiles(System.Environment.CurrentDirectory + "\\Plugins\\", "*.dll");
                if (pluginPathArray.Length != 0)
                {
                    foreach (string pluginPath in pluginPathArray)
                    {
                        LoadDll(pluginPath);
                    }
                }
                #endregion
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + ex.StackTrace);
            }
        }
        private void LoadDll(string file)
        {
            try
            {
                #region 读取二进制
                FileStream fs = new FileStream(file, FileMode.Open);
                long size = fs.Length;
                byte[] array = new byte[size];
                fs.Read(array, 0, array.Length);
                fs.Close();
                #endregion
                Assembly tmp = Assembly.Load(array);
                Type[] types = tmp.GetTypes();
                foreach (Type t in types)
                {
                    if (IsValidPlugin(t))
                    {
                        InterfacePlugin plugin = (InterfacePlugin)tmp.CreateInstance(t.FullName);
                        //do it
                        plugin.CreateUDP += PluginCreateUdp;
                        plugin.SendUDP += PluginSendUdp;
                        plugin.ButtonLeftClick += PluginButtonLeftClick;
                        plugin.ButtonRightClick += PluginButtonRightClick;
                        plugin.SwitchButtonChange += PluginSwitchButtonChange;
                        plugin.TextEditorChange += PluginTextEditorChange;
                        plugin.TrackBarChange += PluginTrackBarChange;
                        pluginList.Add(plugin.PluginID, plugin);
                        #region 属性UI
                        string propertyPath = Path.ChangeExtension(file, "xml");
                        if (File.Exists(propertyPath))
                        {
                            LayoutControl property = XmlUI.Instance.CreateUI(propertyPath, plugin.PluginID + plugin.GetName());
                            if (property != null)
                            {
                                propertyList.Add(plugin.PluginID, property);
                            }
                        }
                        #endregion
                        #region 默认值
                        string settingFileName = Path.GetFileName(file);
                        string settingFilePath = Path.GetDirectoryName(file) + @"\Settings\" + settingFileName;
                        string settingXmlPath = Path.ChangeExtension(settingFilePath, "xml");
                        if (File.Exists(settingXmlPath))
                        {
                            XmlDocument xmlDoc = new XmlDocument();
                            xmlDoc.Load(settingXmlPath);
                            CheckSettingXML(settingXmlPath, xmlDoc);
                            pluginXmlList.Add(plugin.GetName(), xmlDoc);
                            pluginXmlPathList.Add(plugin.GetName(), settingXmlPath);
                            XmlNode rootNode = xmlDoc.SelectSingleNode("Plugin");
                            if (rootNode != null && rootNode.HasChildNodes)
                            {
                                XmlNode moduleNode = rootNode.SelectSingleNode("Settings");
                                if (moduleNode != null && moduleNode.HasChildNodes)
                                {
                                    foreach (XmlNode node in moduleNode.ChildNodes)
                                    {
                                        switch (node.Name)
                                        {
                                            case "SwitchButton":
                                                string sBtnID = "";
                                                bool sBtnSwitch = false;
                                                if (XmlUI.Instance.GetAttribute(node, "ID", out sBtnID) &&
                                                    XmlUI.Instance.GetAttribute(node, "Value", out sBtnSwitch))
                                                {
                                                    SwitchButtonChangeArgs args = new SwitchButtonChangeArgs(sBtnID, sBtnSwitch);
                                                    PluginSwitchButtonChange(plugin, args);
                                                    SwitchButtonChangeHandler handler = new SwitchButtonChangeHandler(plugin.OnSwitchButtonChange);
                                                    handler.BeginInvoke(sBtnID, sBtnSwitch, null, null);
                                                }
                                                break;
                                            case "TextEditor":
                                                string teID = "";
                                                string teText = "";
                                                if (XmlUI.Instance.GetAttribute(node, "ID", out teID) &&
                                                    XmlUI.Instance.GetAttribute(node, "Value", out teText))
                                                {
                                                    TextEditorChangeArgs args = new TextEditorChangeArgs(teID, teText);
                                                    PluginTextEditorChange(plugin, args);
                                                    TextEditorChangeHandler handler = new TextEditorChangeHandler(plugin.OnTextEditorChange);
                                                    handler.BeginInvoke(teID, teText, null, null);
                                                }
                                                break;
                                            case "TrackBar":
                                                string tbID = "";
                                                int tbValue = 0;
                                                if (XmlUI.Instance.GetAttribute(node, "ID", out tbID) &&
                                                    XmlUI.Instance.GetAttribute(node, "Value", out tbValue))
                                                {
                                                    TrackBarChangeArgs args = new TrackBarChangeArgs(tbID, tbValue);
                                                    PluginTrackBarChange(plugin, args);
                                                    TrackBarChangeHandler handler = new TrackBarChangeHandler(plugin.OnTrackBarChange);
                                                    handler.BeginInvoke(tbID, tbValue, null, null);
                                                }
                                                break;
                                        }
                                    }
                                }
                            }
                        }
                        #endregion
                        if (PublicData.saveData != null && PublicData.saveData.pluginSetList.ContainsKey(plugin.PluginID))
                        {
                            pluginList[plugin.PluginID].Open = PublicData.saveData.pluginSetList[plugin.PluginID].Open;
                            pluginList[plugin.PluginID].Auto = PublicData.saveData.pluginSetList[plugin.PluginID].Auto;
                        }
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                DebugConstol.AddLog("LoadDll->" + ex.Message + ex.StackTrace, LogType.Warning);
            }
        }
        private bool IsValidPlugin(Type t)
        {
            bool ret = false;
            Type[] interfaces = t.GetInterfaces();
            foreach (Type theInterface in interfaces)
            {
                if (theInterface.FullName == "InterfacePlugin")
                {
                    ret = true;
                    break;
                }
            }
            return ret;
        }
        #endregion
        #region 处理插件事件
        private void PluginCreateUdp(object sender, EventArgs e)
        {
            try
            {
                InterfacePlugin ip = sender as InterfacePlugin;
                UdpEventArgs uea = e as UdpEventArgs;
                if (ip != null && uea != null)
                {
                    UdpReceive.Instance.CreateReceivePort(ip, uea.Port);
                }
            }
            catch (Exception ex)
            {
                DebugConstol.AddLog("Plugin OnCreateUdp\n" + ex.Message + ex.StackTrace, LogType.Warning);
            }
        }
        private void PluginSendUdp(object sender, EventArgs e)
        {
            try
            {
                InterfacePlugin ip = sender as InterfacePlugin;
                UdpEventArgs uea = e as UdpEventArgs;
                if (ip != null && uea != null)
                {
                    UdpReceive.Instance.SendUDPMessage(uea.Msg, uea.Port, uea.tarClient);
                }
            }
            catch (Exception ex)
            {
                DebugConstol.AddLog("Plugin OnSendUdp\n" + ex.Message + ex.StackTrace, LogType.Warning);
            }
        }
        private void PluginButtonLeftClick(object sender, EventArgs e)
        {
            InterfacePlugin ip = sender as InterfacePlugin;
            ButtonClickArgs args = e as ButtonClickArgs;
            if (ip != null && args != null)
            {
                uiButton btn = XmlUI.Instance.GetButton(ip.PluginID + ip.GetName() + args.id);
                if (btn != null)
                {
                    btn.TriggerLeftButtonClick();
                }
            }
        }
        private void PluginButtonRightClick(object sender, EventArgs e)
        {
            InterfacePlugin ip = sender as InterfacePlugin;
            ButtonClickArgs args = e as ButtonClickArgs;
            if (ip != null && args != null)
            {
                uiButton btn = XmlUI.Instance.GetButton(ip.PluginID + ip.GetName() + args.id);
                if (btn != null)
                {
                    btn.TriggerRightButtonClick();
                }
            }
        }
        private void PluginSwitchButtonChange(object sender, EventArgs e)
        {
            InterfacePlugin ip = sender as InterfacePlugin;
            SwitchButtonChangeArgs args = e as SwitchButtonChangeArgs;
            if (ip != null && args != null)
            {
                uiSwitchButton sbtn = XmlUI.Instance.GetSwitchButton(ip.PluginID + ip.GetName() + args.id);
                if (sbtn != null)
                {
                    sbtn.bSwitchOn = args.open;
                    sbtn.TriggerValueChange();
                }
            }
        }
        private void PluginTextEditorChange(object sender, EventArgs e)
        {
            InterfacePlugin ip = sender as InterfacePlugin;
            TextEditorChangeArgs args = e as TextEditorChangeArgs;
            if (ip != null && args != null)
            {
                uiTextEditor te = XmlUI.Instance.GetTextEditor(ip.PluginID + ip.GetName() + args.id);
                if (te != null)
                {
                    te.Text = args.text;
                    te.TriggerTextChange();
                }
            }
        }
        private void PluginTrackBarChange(object sender, EventArgs e)
        {
            InterfacePlugin ip = sender as InterfacePlugin;
            TrackBarChangeArgs args = e as TrackBarChangeArgs;
            if (ip != null && args != null)
            {
                uiTrackBar tb = XmlUI.Instance.GetTrackBar(ip.PluginID + ip.GetName() + args.id);
                if (tb != null)
                {
                    tb.Value = args.value;
                    tb.TriggerValueChange();
                }
            }
        }
        private void DoNothing(object sender, EventArgs e)
        {

        }
        #endregion
    }
}
