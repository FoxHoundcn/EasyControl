using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace EasyControl
{
    public class UI_LAN : iUiLogic
    {
        public LayoutControl mainLayout { get; private set; }
        //----        
        List<LayoutControl> objLcList = new List<LayoutControl>();
        LayoutControl joyClientControl;
        LayoutControl joyClientList;
        uiPanel serverState;
        uiTextEditor teServerIP1, teServerIP2, teServerIP3, teServerIP4, teServerPort;
        byte serverIP1, serverIP2, serverIP3, serverIP4;
        ushort serverPort;
        float clientHeight = 0f;
        //------------------------------------------------------------------------------------------------------
        public static readonly UI_LAN Instance = new UI_LAN();
        private UI_LAN()
        {
        }
        //============================================================
        public void DxRenderLogic()
        {
            List<JoyObject> joyList = TCPServer.Instance.GetJoyClientList();
            int btnIndex = 0;
            int index = joyList.Count < JoyConst.MaxJoyObject ? joyList.Count : JoyConst.MaxJoyObject;
            for (btnIndex = 0; btnIndex < index; btnIndex++)
            {
                LayoutControl lcObject = XmlUI.Instance.GetLayoutControl("LAN_" + btnIndex + "JoyObjectLC");
                JoyObject joy = joyList[btnIndex];
                int reportCount = joy.reportCount;
                for (int msgIndex = 0; msgIndex < 20; msgIndex++)
                {
                    uiPanel msgPan = XmlUI.Instance.GetPanel("LAN_" + btnIndex + "msg" + msgIndex);
                    Color4 msgColor = XmlUI.DxDeviceGreen;
                    if (reportCount > 8)
                    {
                        msgColor = XmlUI.DxDeviceRed;
                    }
                    else if (reportCount > 5)
                    {
                        msgColor = XmlUI.DxDeviceYellow;
                    }
                    if (msgIndex < reportCount)
                        msgPan.ForeColor = msgColor;
                    else
                        msgPan.ForeColor = XmlUI.DxUIBackColor;
                }
                objLcList[btnIndex].Hide = false;
                uiButton btn = XmlUI.Instance.GetButton("LAN_" + btnIndex + "JoyObjectIndex");
                btn.Name = joy.ToString();
                btn.BackColor = joy.LicenseUID ? XmlUI.DxUIBackColor : XmlUI.DxDeviceRed;
                uiPanel pan = XmlUI.Instance.GetPanel("LAN_" + btnIndex + "Link");
                switch (joy.linkMode)
                {
                    case LinkMode.OnLine:
                        pan.ForeColor = XmlUI.DxDeviceGreen;
                        break;
                    case LinkMode.OffLine:
                        pan.ForeColor = XmlUI.DxDeviceYellow;
                        break;
                    case LinkMode.Error:
                        pan.ForeColor = XmlUI.DxDeviceRed;
                        break;
                }
            }
            float height = clientHeight * btnIndex;
            if (height < joyClientList.DrawRect.Height)
                joyClientList.Rect = new RectangleF(0f, 0f, joyClientControl.DrawRect.Width, height);
            else
                joyClientList.Rect = new RectangleF(0f, 0f, joyClientControl.DrawRect.Width - ViewControl.sliderWidth, height);
            for (int i = btnIndex; i < JoyConst.MaxJoyObject; i++)
            {
                objLcList[i].Hide = true;
            }
            serverState.ForeColor = TCPServer.Instance.Running ? XmlUI.DxDeviceGreen : XmlUI.DxDeviceRed;
        }

        public void Init()
        {
            //----------------------------------------------------------------------------------------------------------------------------------
            mainLayout = XmlUI.Instance.CreateUI(System.Environment.CurrentDirectory + @"\Xml\UI\UI_LAN.xml");
            //----
            uiButton backBtn = XmlUI.Instance.GetButton("UI_LAN_Back");
            backBtn.LeftButtonClick += LanBack;
            joyClientControl = XmlUI.Instance.GetLayoutControl("JoyClientControl");
            joyClientList = XmlUI.Instance.GetLayoutControl("JoyClientListLC");
            serverState = XmlUI.Instance.GetPanel("ServerState");
            for (int i = 0; i < JoyConst.MaxJoyObject; i++)
            {
                LayoutControl lcJoyObject = XmlUI.Instance.CreateUI(System.Environment.CurrentDirectory + @"\Xml\JoyObjectSelect.xml", "LAN_" + i.ToString());
                if (lcJoyObject != null)
                {
                    LayoutControl lcObject = XmlUI.Instance.GetLayoutControl("LAN_" + i + "JoyObjectLC");
                    clientHeight = lcObject.maxHeight;
                    objLcList.Add(lcJoyObject);
                    uiButton btn = XmlUI.Instance.GetButton("LAN_" + i + "JoyObjectIndex");
                    if (btn != null)
                    {
                        btn.Index = i;
                        btn.LeftButtonClick += OnObjClick;
                    }
                    uiButton btnSetting = XmlUI.Instance.GetButton("LAN_" + i + "Setting");
                    if (btnSetting != null)
                    {
                        btnSetting.Index = i;
                        btnSetting.Hide = true;//隐藏设置按钮
                    }
                    uiButton btnReboot = XmlUI.Instance.GetButton("LAN_" + i + "ReBoot");
                    if (btnReboot != null)
                    {
                        btnReboot.Index = i;
                        btnReboot.LeftButtonClick += OnRebootClick;
                    }
                    joyClientList.AddObject(lcJoyObject);
                }
            }
            //SetServer
            uiButton runServerBtn = XmlUI.Instance.GetButton("RunServer");
            runServerBtn.LeftButtonClick += OnRunServer;
            uiButton stopServerBtn = XmlUI.Instance.GetButton("StopServer");
            stopServerBtn.LeftButtonClick += OnStopServer;
            teServerIP1 = XmlUI.Instance.GetTextEditor("SetServerIPTE1");
            teServerIP1.TextChange += OnServerIPChange;
            teServerIP2 = XmlUI.Instance.GetTextEditor("SetServerIPTE2");
            teServerIP2.TextChange += OnServerIPChange;
            teServerIP3 = XmlUI.Instance.GetTextEditor("SetServerIPTE3");
            teServerIP3.TextChange += OnServerIPChange;
            teServerIP4 = XmlUI.Instance.GetTextEditor("SetServerIPTE4");
            teServerIP4.TextChange += OnServerIPChange;
            teServerPort = XmlUI.Instance.GetTextEditor("SetServerPort");
            teServerPort.TextChange += OnServerIPChange;
        }
        private void OnObjClick(object sender, EventArgs e)
        {

        }
        private void OnRebootClick(object sender, EventArgs e)
        {
            JoyIndexChangeArgs args = (JoyIndexChangeArgs)e;
            List<JoyObject> joyList = TCPServer.Instance.GetJoyClientList();
            if (args.Index >= 0 && args.Index < joyList.Count)
            {
                joyList[args.Index].AddReport(new Report(ReportType.ReBoot));
            }
        }
        private void OnServerIPChange(object sender, EventArgs e)
        {
            int ip1, ip2, ip3, ip4, port;
            if (int.TryParse(teServerIP1.Text, out ip1) &&
                int.TryParse(teServerIP2.Text, out ip2) &&
                int.TryParse(teServerIP3.Text, out ip3) &&
                int.TryParse(teServerIP4.Text, out ip4) &&
                int.TryParse(teServerPort.Text, out port))
            {
                serverIP1 = (byte)ip1;
                serverIP2 = (byte)ip2;
                serverIP3 = (byte)ip3;
                serverIP4 = (byte)ip4;
                serverPort = (ushort)port;
                if (teServerIP1.Text != serverIP1.ToString())
                    teServerIP1.Text = serverIP1.ToString();
                if (teServerIP2.Text != serverIP2.ToString())
                    teServerIP2.Text = serverIP2.ToString();
                if (teServerIP3.Text != serverIP3.ToString())
                    teServerIP3.Text = serverIP3.ToString();
                if (teServerIP4.Text != serverIP4.ToString())
                    teServerIP4.Text = serverIP4.ToString();
                if (teServerPort.Text != serverPort.ToString())
                    teServerPort.Text = serverPort.ToString();
            }
        }
        private void OnRunServer(object sender, EventArgs e)
        {
            OnServerIPChange(null, null);
            TCPServer.Instance.RunServer(new IPEndPoint(IPAddress.Parse(serverIP1.ToString() + "." + serverIP2.ToString() + "." + serverIP3.ToString() + "." + serverIP4.ToString()), serverPort));
        }
        private void OnStopServer(object sender, EventArgs e)
        {
            TCPServer.Instance.StopServer();
        }
        private void LanBack(object sender, EventArgs e)
        {
            PublicData.ui_Type = UIType.NodeLink;
        }
    }
}
