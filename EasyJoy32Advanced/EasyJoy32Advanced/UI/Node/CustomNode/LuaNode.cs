using ControllorPlugin;
using NLua;
using SharpDX;
using System;
using System.Collections.Generic;
using System.Net;
using System.Windows.Forms;

namespace EasyControl
{
    public class LuaNode : InterfacePlugin
    {
        Lua nLua = new Lua();
        public string path { private set; get; } = "";
        public Color4 color = XmlUI.DxDeviceYellow;
        public LuaNode(string luaPath)
        {
            try
            {
                #region 加载脚本
                path = luaPath;
                nLua.DoFile(luaPath);
                #endregion
            }
            catch (Exception ex)
            {
                MessageBox.Show("new CustomNode Error : " + ex.Message);
            }
        }
        List<Node> moduleList = new List<Node>();
        public string PluginID
        {
            get
            {
                try
                {
                    var scriptFunc = nLua["Name"] as LuaFunction;
                    object[] res = scriptFunc.Call();
                    string id = res[0].ToString();
                    return id;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("CustomNode Name() Error : " + ex.Message);
                    return "Error CustomNode";
                }
            }
        }
        public bool Open { get; set; } = true;
        public bool Auto { get; set; } = false;

        public List<Node> GetModuleList()
        {
            return moduleList;
        }

        public string GetName()
        {
            try
            {
                var scriptFunc = nLua["Name"] as LuaFunction;
                object[] res = scriptFunc.Call();
                string name = res[0].ToString();
                return name;
            }
            catch (Exception ex)
            {
                MessageBox.Show("CustomNode Name() Error : " + ex.Message);
                return "Error CustomNode";
            }
        }
        public void Init()
        {
            try
            {
                moduleList.Clear();
                for (int j = 0; j < Localization.Instance.GetCustomNodeLinkCount(); j++)
                {
                    List<NodePort> pinList = new List<NodePort>();
                    //------------------------------------------------------------------------------
                    var scriptFunc = nLua["PortList"] as LuaFunction;
                    object[] res = scriptFunc.Call();
                    LuaTable portTable = res[0] as LuaTable;
                    if (portTable != null)
                    {
                        LuaTable portList = portTable["PortList"] as LuaTable;
                        if (portList != null)
                        {
                            for (int i = 1; i <= portList.Values.Count; i++)
                            {
                                LuaTable portItem = portList[i] as LuaTable;
                                if (portItem != null)
                                {
                                    PortType portType = PortType.In;
                                    bool typeReady = false;
                                    string name = portItem["name"] as string;
                                    string type = portItem["type"] as string;
                                    switch (type)
                                    {
                                        case "In":
                                            portType = PortType.In;
                                            typeReady = true;
                                            break;
                                        case "Out":
                                            portType = PortType.Out;
                                            typeReady = true;
                                            break;
                                    }
                                    string valueType = portItem["valueType"] as string;
                                    if (!name.Equals("") && typeReady)
                                    {
                                        switch (valueType)
                                        {
                                            case "Int":
                                                pinList.Add(new NodePort(name, portType, 0));
                                                break;
                                            case "Float":
                                                pinList.Add(new NodePort(name, portType, 0f));
                                                break;
                                            case "String":
                                                pinList.Add(new NodePort(name, portType, ""));
                                                break;
                                            default:
                                                MessageBox.Show("JavaNode Init Error : valueType Error !");
                                                break;
                                        }
                                    }
                                }
                            }
                        }
                    }
                    Node newNode = new Node(GetName(), pinList);
                    var scriptFuncInfo = nLua["Info"] as LuaFunction;
                    object[] resInfo = scriptFuncInfo.Call();
                    newNode.Info = resInfo[0] as string;
                    newNode.Open = false;
                    moduleList.Add(newNode);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("CustomNode Init() Error : " + ex.Message);
            }
        }

        public void AutoOpen()
        {
        }
        public void Update()
        {
        }

        public void NodeCloseEvent(int mIndex)
        {
            //if (!moduleList[mIndex].Open)
            //    Open = false;
        }
        public void ValueChangeEvent(int mIndex, int pIndex)
        {
            try
            {
                string tableStr = "   a={  [\"PortList\"] = {";
                List<NodePort> portList = moduleList[mIndex].NodePortList;
                for (int i = 0; i < portList.Count; i++)
                {
                    switch (portList[i].Type)
                    {
                        case PortValue.Int64:
                            tableStr += portList[i].ValueInt64.ToString() + ",";
                            break;
                        case PortValue.Double:
                            tableStr += portList[i].ValueDouble.ToString() + ",";
                            break;
                        case PortValue.String:
                            tableStr += "\"" + portList[i].ValueString.ToString() + "\",";
                            break;
                    }
                }
                tableStr += "}}";
                nLua.DoString(tableStr);
                LuaTable portValue = nLua.GetTable("a");
                var scriptFunc = nLua["Update"] as LuaFunction;
                object[] res = scriptFunc.Call(portValue);

                LuaTable portTable = res[0] as LuaTable;
                if (portTable != null)
                {
                    LuaTable portListTab = portTable["PortList"] as LuaTable;
                    if (portListTab != null)
                    {
                        for (int i = 1; i <= portListTab.Values.Count; i++)
                        {
                            if (portList[i - 1].IO == PortType.Out)
                            {
                                switch (portList[i - 1].Type)
                                {
                                    case PortValue.Int64:
                                        moduleList[mIndex].NodePortList[i - 1].ValueInt64 = long.Parse(portListTab[i].ToString());
                                        break;
                                    case PortValue.Double:
                                        moduleList[mIndex].NodePortList[i - 1].ValueDouble = double.Parse(portListTab[i].ToString());
                                        break;
                                    case PortValue.String:
                                        moduleList[mIndex].NodePortList[i - 1].ValueString = portListTab[i].ToString();
                                        break;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("CustomNode Update() Error : " + ex.Message);
            }
        }
        #region noUse
        public event EventHandler ButtonLeftClick;
        public event EventHandler ButtonRightClick;
        public event EventHandler SwitchButtonChange;
        public event EventHandler TextEditorChange;
        public event EventHandler TrackBarChange;
        public event EventHandler CreateUDP;
        public event EventHandler SendUDP;

        public void DefWndProc(int message)
        {
        }

        public void OnButtonLeftClick(string id)
        {
        }

        public void OnButtonRightClick(string id)
        {
        }

        public void OnReceiveUdpMsg(EndPoint client, byte[] msg)
        {
        }

        public void OnSwitchButtonChange(string id, bool value)
        {
        }

        public void OnTextEditorChange(string id, string value)
        {
        }

        public void OnTrackBarChange(string id, int value)
        {
        }
        #endregion
    }
}
