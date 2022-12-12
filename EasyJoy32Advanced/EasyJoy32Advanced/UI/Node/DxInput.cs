using ControllorPlugin;
using SharpDX.XInput;
using System;
using System.Collections.Generic;
using System.Net;

namespace EasyControl
{
    public class DxInput : InterfacePlugin
    {
        public static readonly DxInput Instance = new DxInput();
        private DxInput()
        {

        }
        private List<Node> moduleList = new List<Node>();
        private Controller controller;
        public string PluginID
        {
            get { return "EasyXInput"; }
        }

        public bool Open { get; set; } = true;
        public bool Auto { get; set; } = false;


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

        public List<Node> GetModuleList()
        {
            return moduleList;
        }

        public string GetName()
        {
            return "Easy X Input";
        }

        public void Init()
        {
            Open = true;
            moduleList.Clear();
            List<NodePort> pinList = new List<NodePort>();
            pinList.Add(new NodePort("LeftTrigger", PortType.Out, 0f));
            pinList.Add(new NodePort("RightTrigger", PortType.Out, 0f));
            pinList.Add(new NodePort("LeftThumbX", PortType.Out, 0f));
            pinList.Add(new NodePort("LeftThumbY", PortType.Out, 0f));
            pinList.Add(new NodePort("RightThumbX", PortType.Out, 0f));
            pinList.Add(new NodePort("RightThumbY", PortType.Out, 0f));
            pinList.Add(new NodePort("DPadUp", PortType.Out, 0));
            pinList.Add(new NodePort("DPadDown", PortType.Out, 0));
            pinList.Add(new NodePort("DPadLeft", PortType.Out, 0));
            pinList.Add(new NodePort("DPadRight", PortType.Out, 0));
            pinList.Add(new NodePort("Start", PortType.Out, 0));
            pinList.Add(new NodePort("Back", PortType.Out, 0));
            pinList.Add(new NodePort("LeftThumb", PortType.Out, 0));
            pinList.Add(new NodePort("RightThumb", PortType.Out, 0));
            pinList.Add(new NodePort("LeftShoulder", PortType.Out, 0));
            pinList.Add(new NodePort("RightShoulder", PortType.Out, 0));
            pinList.Add(new NodePort("A", PortType.Out, 0));
            pinList.Add(new NodePort("B", PortType.Out, 0));
            pinList.Add(new NodePort("X", PortType.Out, 0));
            pinList.Add(new NodePort("Y", PortType.Out, 0));
            Node dataNode = new Node(GetName(), pinList);
            moduleList.Add(dataNode);
            controller = new Controller(UserIndex.One);
        }

        public void AutoOpen()
        {
        }
        public void Update()
        {
            if (controller != null && controller.IsConnected)
            {
                State state = controller.GetState();
                moduleList[0].NodePortList[0].ValueDouble = state.Gamepad.LeftTrigger / 255f;
                moduleList[0].NodePortList[1].ValueDouble = state.Gamepad.RightTrigger / 255f;
                moduleList[0].NodePortList[2].ValueDouble = state.Gamepad.LeftThumbX / 65535f + 0.5f;
                moduleList[0].NodePortList[3].ValueDouble = state.Gamepad.LeftThumbY / 65535f + 0.5f;
                moduleList[0].NodePortList[4].ValueDouble = state.Gamepad.RightThumbX / 65535f + 0.5f;
                moduleList[0].NodePortList[5].ValueDouble = state.Gamepad.RightThumbY / 65535f + 0.5f;
                moduleList[0].NodePortList[6].ValueInt64 = state.Gamepad.Buttons.HasFlag(GamepadButtonFlags.DPadUp) ? 1 : 0;
                moduleList[0].NodePortList[7].ValueInt64 = state.Gamepad.Buttons.HasFlag(GamepadButtonFlags.DPadDown) ? 1 : 0;
                moduleList[0].NodePortList[8].ValueInt64 = state.Gamepad.Buttons.HasFlag(GamepadButtonFlags.DPadLeft) ? 1 : 0;
                moduleList[0].NodePortList[9].ValueInt64 = state.Gamepad.Buttons.HasFlag(GamepadButtonFlags.DPadRight) ? 1 : 0;
                moduleList[0].NodePortList[10].ValueInt64 = state.Gamepad.Buttons.HasFlag(GamepadButtonFlags.Start) ? 1 : 0;
                moduleList[0].NodePortList[11].ValueInt64 = state.Gamepad.Buttons.HasFlag(GamepadButtonFlags.Back) ? 1 : 0;
                moduleList[0].NodePortList[12].ValueInt64 = state.Gamepad.Buttons.HasFlag(GamepadButtonFlags.LeftThumb) ? 1 : 0;
                moduleList[0].NodePortList[13].ValueInt64 = state.Gamepad.Buttons.HasFlag(GamepadButtonFlags.RightThumb) ? 1 : 0;
                moduleList[0].NodePortList[14].ValueInt64 = state.Gamepad.Buttons.HasFlag(GamepadButtonFlags.LeftShoulder) ? 1 : 0;
                moduleList[0].NodePortList[15].ValueInt64 = state.Gamepad.Buttons.HasFlag(GamepadButtonFlags.RightShoulder) ? 1 : 0;
                moduleList[0].NodePortList[16].ValueInt64 = state.Gamepad.Buttons.HasFlag(GamepadButtonFlags.A) ? 1 : 0;
                moduleList[0].NodePortList[17].ValueInt64 = state.Gamepad.Buttons.HasFlag(GamepadButtonFlags.B) ? 1 : 0;
                moduleList[0].NodePortList[18].ValueInt64 = state.Gamepad.Buttons.HasFlag(GamepadButtonFlags.X) ? 1 : 0;
                moduleList[0].NodePortList[19].ValueInt64 = state.Gamepad.Buttons.HasFlag(GamepadButtonFlags.Y) ? 1 : 0;
            }
        }

        public void NodeCloseEvent(int mIndex)
        {
        }

        public void ValueChangeEvent(int mIndex, int pIndex)
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
    }
}
