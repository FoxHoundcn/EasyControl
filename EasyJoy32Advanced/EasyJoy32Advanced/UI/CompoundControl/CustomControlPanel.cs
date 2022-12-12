using System;

namespace EasyControl
{
    public class CustomControlPanel : iUiLogic
    {
        //---------------------------------------------------------------------------------------
        JoyObject currentObj = null;
        JoyCustom currentCus = null;
        LayoutControl customPinControl;
        LayoutControl dataLC, csLC, clkLC, dcLC, rstLC, cs2LC, cs3LC, cs4LC;
        uiButton dataBtn, csBtn, clkBtn, dcBtn, rstBtn, cs2Btn, cs3Btn, cs4Btn;
        LayoutControl linkPinControl;
        LayoutControl rotateControl;
        LayoutControl oledControl;
        LayoutControl linkCountControl;
        LayoutControl fontSetControl;
        LayoutControl linkDataControl;
        uiSwitchButton rotate0, rotate90, rotate180, rotate270;
        uiOLED oledShow;
        uiTrackBar tbLinkCount;
        uiTextLable linkCountTip, tlLinkCount;
        uiTextEditor teFontX, teFontY;
        uiTrackBar tbFontLibSet, tbFontCountSet;
        LayoutControl lanControl;
        uiTextEditor teMAC1, teMAC2, teMAC3, teMAC4, teMAC5, teMAC6;
        uiTextEditor teIP1, teIP2, teIP3, teIP4, tePort;
        uiTextEditor teSubNet1, teSubNet2, teSubNet3, teSubNet4;
        uiTextEditor teGateway1, teGateway2, teGateway3, teGateway4;
        uiTextEditor teDNS1, teDNS2, teDNS3, teDNS4;
        uiTextEditor teServerIP1, teServerIP2, teServerIP3, teServerIP4, teServerPort;
        //////////////////////////////////////////////////////////////////////////////////////////
        public static readonly CustomControlPanel Instance = new CustomControlPanel();
        private CustomControlPanel()
        {
        }
        public void Init()
        {
            customPinControl = XmlUI.Instance.GetLayoutControl("CustomPinControl");
            #region Pin
            for (int i = 0; i < JoyConst.MaxCustomPin; i++)
            {
                uiButton btn = XmlUI.Instance.GetButton("CustomPinButton" + i);
                btn.Index = i;
                btn.LeftButtonClick += OnButtonClick;
            }
            linkPinControl = XmlUI.Instance.GetLayoutControl("LinkPinControl");
            dataLC = XmlUI.Instance.GetLayoutControl("CustomLinkPin" + 0);
            csLC = XmlUI.Instance.GetLayoutControl("CustomLinkPin" + 1);
            clkLC = XmlUI.Instance.GetLayoutControl("CustomLinkPin" + 2);
            dcLC = XmlUI.Instance.GetLayoutControl("CustomLinkPin" + 3);
            rstLC = XmlUI.Instance.GetLayoutControl("CustomLinkPin" + 4);
            cs2LC = XmlUI.Instance.GetLayoutControl("CustomLinkPin" + 5);
            cs3LC = XmlUI.Instance.GetLayoutControl("CustomLinkPin" + 6);
            cs4LC = XmlUI.Instance.GetLayoutControl("CustomLinkPin" + 7);
            dataBtn = XmlUI.Instance.GetButton("CustomLink" + 0);
            csBtn = XmlUI.Instance.GetButton("CustomLink" + 1);
            clkBtn = XmlUI.Instance.GetButton("CustomLink" + 2);
            dcBtn = XmlUI.Instance.GetButton("CustomLink" + 3);
            rstBtn = XmlUI.Instance.GetButton("CustomLink" + 4);
            cs2Btn = XmlUI.Instance.GetButton("CustomLink" + 5);
            cs3Btn = XmlUI.Instance.GetButton("CustomLink" + 6);
            cs4Btn = XmlUI.Instance.GetButton("CustomLink" + 7);
            for (int i = 0; i < JoyConst.MaxCustomCount; i++)
            {
                uiButton btnData = XmlUI.Instance.GetButton("CustomLink" + i);
                btnData.Index = i;
                btnData.Name = ((CustomLinkPin)(i + 1)).ToString();
                btnData.RightButtonClick += OnLinkButtonClick;
                uiPanel panel = XmlUI.Instance.GetPanel("CustomColor" + i);
                switch (i)
                {
                    case 0:
                        panel.ForeColor = XmlUI.DxDeviceBlue;
                        break;
                    case 1:
                        panel.ForeColor = XmlUI.DxDeviceGreen;
                        break;
                    case 2:
                        panel.ForeColor = XmlUI.DxDeviceRed;
                        break;
                    case 3:
                        panel.ForeColor = XmlUI.DxDeviceYellow;
                        break;
                    case 4:
                        panel.ForeColor = XmlUI.DxDevicePurple;
                        break;
                    case 5:
                        panel.ForeColor = XmlUI.DxDeviceGreen;
                        break;
                    case 6:
                        panel.ForeColor = XmlUI.DxDeviceGreen;
                        break;
                    case 7:
                        panel.ForeColor = XmlUI.DxDeviceGreen;
                        break;
                }
            }
            #endregion
            #region OLED
            rotate0 = XmlUI.Instance.GetSwitchButton("Rotate0");
            rotate0.ValueChange += OnRorate0Click;
            rotate90 = XmlUI.Instance.GetSwitchButton("Rotate90");
            rotate90.ValueChange += OnRorate90Click;
            rotate180 = XmlUI.Instance.GetSwitchButton("Rotate180");
            rotate180.ValueChange += OnRorate180Click;
            rotate270 = XmlUI.Instance.GetSwitchButton("Rotate270");
            rotate270.ValueChange += OnRorate270Click;
            oledControl = XmlUI.Instance.GetLayoutControl("OledControl");
            oledShow = XmlUI.Instance.GetOLED("OLED");
            linkCountControl = XmlUI.Instance.GetLayoutControl("LinkCountControl");
            rotateControl = XmlUI.Instance.GetLayoutControl("RotateControl");
            fontSetControl = XmlUI.Instance.GetLayoutControl("FontSetControl");
            for (int i = 0; i < JoyConst.MaxFontSetCount; i++)
            {
                uiButton btnFontSet = XmlUI.Instance.GetButton("FontSetButton" + i);
                btnFontSet.Index = i;
                btnFontSet.RightButtonClick += OnFontSetButtonClick;
            }
            teFontX = XmlUI.Instance.GetTextEditor("FontSetXTE");
            teFontX.TextChange += OnFontXChange;
            teFontY = XmlUI.Instance.GetTextEditor("FontSetYTE");
            teFontY.TextChange += OnFontYChange;
            tbFontLibSet = XmlUI.Instance.GetTrackBar("FontLibSetTB");
            tbFontLibSet.ValueChange += OnFontLibChange;
            tbFontCountSet = XmlUI.Instance.GetTrackBar("FontCountSetTB");
            tbFontCountSet.ValueChange += OnFontCountChange;
            uiButton clearButton = XmlUI.Instance.GetButton("OledClearButton");
            clearButton.LeftButtonClick += OnClearOled;
            #endregion
            #region Data
            linkDataControl = XmlUI.Instance.GetLayoutControl("LinkDataControl");
            tbLinkCount = XmlUI.Instance.GetTrackBar("LinkCountTB");
            tbLinkCount.ValueChange += OnLinkCountChange;
            linkCountTip = XmlUI.Instance.GetTextLable("LinkCountTip");
            tlLinkCount = XmlUI.Instance.GetTextLable("LinkCountTL");
            for (int i = 0; i < JoyConst.MaxCustomData; i++)
            {
                uiButton btnData = XmlUI.Instance.GetButton("CustomDataButton" + i);
                btnData.Index = i;
                btnData.LeftButtonClick += OnDataButtonClick;
            }
            #endregion
            #region LAN
            lanControl = XmlUI.Instance.GetLayoutControl("LANControl");
            //MAC
            teMAC1 = XmlUI.Instance.GetTextEditor("MacTE1");
            teMAC1.TextChange += OnChangeMAC;
            teMAC2 = XmlUI.Instance.GetTextEditor("MacTE2");
            teMAC2.TextChange += OnChangeMAC;
            teMAC3 = XmlUI.Instance.GetTextEditor("MacTE3");
            teMAC3.TextChange += OnChangeMAC;
            teMAC4 = XmlUI.Instance.GetTextEditor("MacTE4");
            teMAC4.TextChange += OnChangeMAC;
            teMAC5 = XmlUI.Instance.GetTextEditor("MacTE5");
            teMAC5.TextChange += OnChangeMAC;
            teMAC6 = XmlUI.Instance.GetTextEditor("MacTE6");
            teMAC6.TextChange += OnChangeMAC;
            //IP
            teIP1 = XmlUI.Instance.GetTextEditor("IPTE1");
            teIP1.TextChange += OnChangeIP;
            teIP2 = XmlUI.Instance.GetTextEditor("IPTE2");
            teIP2.TextChange += OnChangeIP;
            teIP3 = XmlUI.Instance.GetTextEditor("IPTE3");
            teIP3.TextChange += OnChangeIP;
            teIP4 = XmlUI.Instance.GetTextEditor("IPTE4");
            teIP4.TextChange += OnChangeIP;
            tePort = XmlUI.Instance.GetTextEditor("Port");
            tePort.TextChange += OnChangeIP;
            //SubNet
            teSubNet1 = XmlUI.Instance.GetTextEditor("SubNetTE1");
            teSubNet1.TextChange += OnChangeSubNet;
            teSubNet2 = XmlUI.Instance.GetTextEditor("SubNetTE2");
            teSubNet2.TextChange += OnChangeSubNet;
            teSubNet3 = XmlUI.Instance.GetTextEditor("SubNetTE3");
            teSubNet3.TextChange += OnChangeSubNet;
            teSubNet4 = XmlUI.Instance.GetTextEditor("SubNetTE4");
            teSubNet4.TextChange += OnChangeSubNet;
            //Gateway
            teGateway1 = XmlUI.Instance.GetTextEditor("GatewayTE1");
            teGateway1.TextChange += OnChangeGateway;
            teGateway2 = XmlUI.Instance.GetTextEditor("GatewayTE2");
            teGateway2.TextChange += OnChangeGateway;
            teGateway3 = XmlUI.Instance.GetTextEditor("GatewayTE3");
            teGateway3.TextChange += OnChangeGateway;
            teGateway4 = XmlUI.Instance.GetTextEditor("GatewayTE4");
            teGateway4.TextChange += OnChangeGateway;
            //DNS
            teDNS1 = XmlUI.Instance.GetTextEditor("DNSTE1");
            teDNS1.TextChange += OnChangeDNS;
            teDNS2 = XmlUI.Instance.GetTextEditor("DNSTE2");
            teDNS2.TextChange += OnChangeDNS;
            teDNS3 = XmlUI.Instance.GetTextEditor("DNSTE3");
            teDNS3.TextChange += OnChangeDNS;
            teDNS4 = XmlUI.Instance.GetTextEditor("DNSTE4");
            teDNS4.TextChange += OnChangeDNS;
            //ServerIP
            teServerIP1 = XmlUI.Instance.GetTextEditor("ServerIPTE1");
            teServerIP1.TextChange += OnChangeServerIP;
            teServerIP2 = XmlUI.Instance.GetTextEditor("ServerIPTE2");
            teServerIP2.TextChange += OnChangeServerIP;
            teServerIP3 = XmlUI.Instance.GetTextEditor("ServerIPTE3");
            teServerIP3.TextChange += OnChangeServerIP;
            teServerIP4 = XmlUI.Instance.GetTextEditor("ServerIPTE4");
            teServerIP4.TextChange += OnChangeServerIP;
            teServerPort = XmlUI.Instance.GetTextEditor("ServerPort");
            teServerPort.TextChange += OnChangeServerIP;
            #endregion
        }
        private void OnButtonClick(object sender, EventArgs e)
        {
            JoyIndexChangeArgs args = (JoyIndexChangeArgs)e;
            if (currentObj != null && currentCus != null)
            {
                switch ((CustomLinkPin)currentObj.SelectCustomPin + 1)
                {
                    case CustomLinkPin.Data:
                        currentCus.data = (byte)args.Index;
                        break;
                    case CustomLinkPin.CS:
                        currentCus.cs = (byte)args.Index;
                        break;
                    case CustomLinkPin.CLK:
                        currentCus.clk = (byte)args.Index;
                        break;
                    case CustomLinkPin.DC:
                        currentCus.dc = (byte)args.Index;
                        break;
                    case CustomLinkPin.RST:
                        currentCus.rst = (byte)args.Index;
                        break;
                    case CustomLinkPin.CS2:
                        currentCus.cs2 = (byte)args.Index;
                        break;
                    case CustomLinkPin.CS3:
                        currentCus.cs3 = (byte)args.Index;
                        break;
                    case CustomLinkPin.CS4:
                        currentCus.cs4 = (byte)args.Index;
                        break;
                }
            }
        }
        private void OnLinkButtonClick(object sender, EventArgs e)
        {
            JoyIndexChangeArgs args = (JoyIndexChangeArgs)e;
            if (currentObj != null)
            {
                currentObj.SelectCustomPin = args.Index;
            }
        }
        private void OnRorate0Click(object sender, EventArgs e)
        {
            JoyIndexChangeArgs args = (JoyIndexChangeArgs)e;
            if (currentObj != null && currentCus != null)
            {
                currentCus.rotateType = RotateType.Rotate0;
            }
        }
        private void OnRorate90Click(object sender, EventArgs e)
        {
            JoyIndexChangeArgs args = (JoyIndexChangeArgs)e;
            if (currentObj != null && currentCus != null)
            {
                currentCus.rotateType = RotateType.Rotate90;
            }
        }
        private void OnRorate180Click(object sender, EventArgs e)
        {
            JoyIndexChangeArgs args = (JoyIndexChangeArgs)e;
            if (currentObj != null && currentCus != null)
            {
                currentCus.rotateType = RotateType.Rotate180;
            }
        }
        private void OnRorate270Click(object sender, EventArgs e)
        {
            JoyIndexChangeArgs args = (JoyIndexChangeArgs)e;
            if (currentObj != null && currentCus != null)
            {
                currentCus.rotateType = RotateType.Rotate270;
            }
        }
        private void OnLinkCountChange(object sender, EventArgs e)
        {
            if (currentObj != null && currentCus != null)
            {
                currentCus.dataCount = (byte)tbLinkCount.Value;
            }
        }
        private void OnFontXChange(object sender, EventArgs e)
        {
            if (currentObj != null)
            {
                JoyCustom custom = currentObj.GetCurrentJoyCustom();
                if (custom != null)
                {
                    if (currentObj.SelectFontSet >= 0 && currentObj.SelectFontSet < custom.FontSetList.Length)
                    {
                        int x = 0;
                        if (!int.TryParse(teFontX.Text, out x))
                        {
                            x = 0;
                        }
                        custom.FontSetList[currentObj.SelectFontSet].X = (byte)x;
                    }
                }
            }
        }
        private void OnFontYChange(object sender, EventArgs e)
        {
            if (currentObj != null)
            {
                JoyCustom custom = currentObj.GetCurrentJoyCustom();
                if (custom != null)
                {
                    if (currentObj.SelectFontSet >= 0 && currentObj.SelectFontSet < custom.FontSetList.Length)
                    {
                        int y = 0;
                        if (!int.TryParse(teFontY.Text, out y))
                        {
                            y = 0;
                        }
                        custom.FontSetList[currentObj.SelectFontSet].Y = (byte)y;
                    }
                }
            }
        }
        private void OnFontLibChange(object sender, EventArgs e)
        {
            if (currentObj != null)
            {
                JoyCustom custom = currentObj.GetCurrentJoyCustom();
                if (custom != null)
                {
                    if (currentObj.SelectFontSet >= 0 && currentObj.SelectFontSet < custom.FontSetList.Length)
                    {
                        if (tbFontLibSet.Value >= 1 && tbFontLibSet.Value <= JoyConst.MaxFont)
                            custom.FontSetList[currentObj.SelectFontSet].LibIndex = (byte)(tbFontLibSet.Value - 1);
                    }
                }
            }
        }
        private void OnFontCountChange(object sender, EventArgs e)
        {
            if (currentObj != null)
            {
                JoyCustom custom = currentObj.GetCurrentJoyCustom();
                if (custom != null)
                {
                    if (currentObj.SelectFontSet >= 0 && currentObj.SelectFontSet < custom.FontSetList.Length)
                    {
                        if (tbFontCountSet.Value >= JoyConst.MinFontCharCount && tbFontCountSet.Value <= JoyConst.MaxFontCharCount)
                            custom.FontSetList[currentObj.SelectFontSet].Count = (byte)tbFontCountSet.Value;
                    }
                }
            }
        }
        private void OnClearOled(object sender, EventArgs e)
        {
            JoyObject joyObj = PublicData.GetCurrentSelectJoyObject();
            if (joyObj != null)
            {
                joyObj.AddReport(new Report(ReportType.OledClear));
            }
        }
        private void OnFontSetButtonClick(object sender, EventArgs e)
        {
            JoyIndexChangeArgs args = (JoyIndexChangeArgs)e;
            if (currentObj != null)
            {
                currentObj.SelectFontSet = (byte)args.Index;
            }
        }
        private void OnDataButtonClick(object sender, EventArgs e)
        {
            JoyIndexChangeArgs args = (JoyIndexChangeArgs)e;
            if (currentObj != null && currentCus != null)
            {
                currentCus.dataStart = (byte)args.Index;
            }
        }
        private void SetOled(JoyCustom custom)
        {
            linkCountControl.Hide = false;
            linkCountTip.Text = "LinkCountTL";
            tlLinkCount.Text = "LinkCount";
            fontSetControl.Hide = false;
            if (currentObj.SelectFontSet >= 0 && currentObj.SelectFontSet < custom.FontSetList.Length)
            {
                FontSet fs = custom.FontSetList[currentObj.SelectFontSet];
                if (!teFontX.Text.Equals(fs.X.ToString()))
                    teFontX.Text = fs.X.ToString();
                if (!teFontY.Text.Equals(fs.Y.ToString()))
                    teFontY.Text = fs.Y.ToString();
                if (tbFontLibSet.Value != fs.LibIndex + 1)
                    tbFontLibSet.Value = fs.LibIndex + 1;
                if (tbFontCountSet.Value != fs.Count)
                    tbFontCountSet.Value = fs.Count;
            }
        }
        public void OnChangeMAC(object sender, EventArgs e)
        {
            if (currentObj != null)
            {
                string textMAC1 = teMAC1.Text.Length >= 2 ? teMAC1.Text.Substring(0, 2) : teMAC1.Text;
                int mac1 = int.Parse(textMAC1, System.Globalization.NumberStyles.HexNumber);
                string textMAC2 = teMAC2.Text.Length >= 2 ? teMAC2.Text.Substring(0, 2) : teMAC2.Text;
                int mac2 = int.Parse(textMAC2, System.Globalization.NumberStyles.HexNumber);
                string textMAC3 = teMAC3.Text.Length >= 2 ? teMAC3.Text.Substring(0, 2) : teMAC3.Text;
                int mac3 = int.Parse(textMAC3, System.Globalization.NumberStyles.HexNumber);
                string textMAC4 = teMAC4.Text.Length >= 2 ? teMAC4.Text.Substring(0, 2) : teMAC4.Text;
                int mac4 = int.Parse(textMAC4, System.Globalization.NumberStyles.HexNumber);
                string textMAC5 = teMAC5.Text.Length >= 2 ? teMAC5.Text.Substring(0, 2) : teMAC5.Text;
                int mac5 = int.Parse(textMAC5, System.Globalization.NumberStyles.HexNumber);
                string textMAC6 = teMAC6.Text.Length >= 2 ? teMAC6.Text.Substring(0, 2) : teMAC6.Text;
                int mac6 = int.Parse(textMAC6, System.Globalization.NumberStyles.HexNumber);
                currentObj.SetMAC((byte)mac1, (byte)mac2, (byte)mac3, (byte)mac4, (byte)mac5, (byte)mac6);
            }
        }
        private void SetMAC()
        {
            if (currentObj != null)
            {
                if (teMAC1.Text != currentObj.MAC[0].ToString("x2").ToUpper())
                    teMAC1.Text = currentObj.MAC[0].ToString("x2").ToUpper();
                if (teMAC2.Text != currentObj.MAC[1].ToString("x2").ToUpper())
                    teMAC2.Text = currentObj.MAC[1].ToString("x2").ToUpper();
                if (teMAC3.Text != currentObj.MAC[2].ToString("x2").ToUpper())
                    teMAC3.Text = currentObj.MAC[2].ToString("x2").ToUpper();
                if (teMAC4.Text != currentObj.MAC[3].ToString("x2").ToUpper())
                    teMAC4.Text = currentObj.MAC[3].ToString("x2").ToUpper();
                if (teMAC5.Text != currentObj.MAC[4].ToString("x2").ToUpper())
                    teMAC5.Text = currentObj.MAC[4].ToString("x2").ToUpper();
                if (teMAC6.Text != currentObj.MAC[5].ToString("x2").ToUpper())
                    teMAC6.Text = currentObj.MAC[5].ToString("x2").ToUpper();
            }
        }
        public void OnChangeIP(object sender, EventArgs e)
        {
            if (currentObj != null)
            {
                int ip1, ip2, ip3, ip4, port;
                if (int.TryParse(teIP1.Text, out ip1) &&
                   int.TryParse(teIP2.Text, out ip2) &&
                   int.TryParse(teIP3.Text, out ip3) &&
                   int.TryParse(teIP4.Text, out ip4) &&
                   int.TryParse(tePort.Text, out port))
                    currentObj.SetIP((byte)ip1, (byte)ip2, (byte)ip3, (byte)ip4, (ushort)port);
            }
        }
        private void SetIP()
        {
            if (currentObj != null)
            {
                if (teIP1.Text != currentObj.Local_IP[0].ToString())
                    teIP1.Text = currentObj.Local_IP[0].ToString();
                if (teIP2.Text != currentObj.Local_IP[1].ToString())
                    teIP2.Text = currentObj.Local_IP[1].ToString();
                if (teIP3.Text != currentObj.Local_IP[2].ToString())
                    teIP3.Text = currentObj.Local_IP[2].ToString();
                if (teIP4.Text != currentObj.Local_IP[3].ToString())
                    teIP4.Text = currentObj.Local_IP[3].ToString();
                if (tePort.Text != currentObj.Local_Port.ToString())
                    tePort.Text = currentObj.Local_Port.ToString();
            }
        }
        public void OnChangeSubNet(object sender, EventArgs e)
        {
            if (currentObj != null)
            {
                int subnet1, subnet2, subnet3, subnet4;
                if (int.TryParse(teSubNet1.Text, out subnet1) &&
                    int.TryParse(teSubNet2.Text, out subnet2) &&
                    int.TryParse(teSubNet3.Text, out subnet3) &&
                    int.TryParse(teSubNet4.Text, out subnet4))
                    currentObj.SetSubNet((byte)subnet1, (byte)subnet2, (byte)subnet3, (byte)subnet4);
            }
        }
        private void SetSubNet()
        {
            if (currentObj != null)
            {
                if (teSubNet1.Text != currentObj.Subnet[0].ToString())
                    teSubNet1.Text = currentObj.Subnet[0].ToString();
                if (teSubNet2.Text != currentObj.Subnet[1].ToString())
                    teSubNet2.Text = currentObj.Subnet[1].ToString();
                if (teSubNet3.Text != currentObj.Subnet[2].ToString())
                    teSubNet3.Text = currentObj.Subnet[2].ToString();
                if (teSubNet4.Text != currentObj.Subnet[3].ToString())
                    teSubNet4.Text = currentObj.Subnet[3].ToString();
            }
        }
        public void OnChangeGateway(object sender, EventArgs e)
        {
            if (currentObj != null)
            {
                int gateway1, gateway2, gateway3, gateway4;
                if (int.TryParse(teGateway1.Text, out gateway1) &&
                    int.TryParse(teGateway2.Text, out gateway2) &&
                    int.TryParse(teGateway3.Text, out gateway3) &&
                    int.TryParse(teGateway4.Text, out gateway4))
                    currentObj.SetGateway((byte)gateway1, (byte)gateway2, (byte)gateway3, (byte)gateway4);
            }
        }
        private void SetGateway()
        {
            if (currentObj != null)
            {
                if (teGateway1.Text != currentObj.Gateway[0].ToString())
                    teGateway1.Text = currentObj.Gateway[0].ToString();
                if (teGateway2.Text != currentObj.Gateway[1].ToString())
                    teGateway2.Text = currentObj.Gateway[1].ToString();
                if (teGateway3.Text != currentObj.Gateway[2].ToString())
                    teGateway3.Text = currentObj.Gateway[2].ToString();
                if (teGateway4.Text != currentObj.Gateway[3].ToString())
                    teGateway4.Text = currentObj.Gateway[3].ToString();
            }
        }
        public void OnChangeDNS(object sender, EventArgs e)
        {
            if (currentObj != null)
            {
                int dns1, dns2, dns3, dns4;
                if (int.TryParse(teDNS1.Text, out dns1) &&
                    int.TryParse(teDNS2.Text, out dns2) &&
                    int.TryParse(teDNS3.Text, out dns3) &&
                    int.TryParse(teDNS4.Text, out dns4))
                    currentObj.SetDNS((byte)dns1, (byte)dns2, (byte)dns3, (byte)dns4);
            }
        }
        private void SetDNS()
        {
            if (currentObj != null)
            {
                if (teDNS1.Text != currentObj.DNS[0].ToString())
                    teDNS1.Text = currentObj.DNS[0].ToString();
                if (teDNS2.Text != currentObj.DNS[1].ToString())
                    teDNS2.Text = currentObj.DNS[1].ToString();
                if (teDNS3.Text != currentObj.DNS[2].ToString())
                    teDNS3.Text = currentObj.DNS[2].ToString();
                if (teDNS4.Text != currentObj.DNS[3].ToString())
                    teDNS4.Text = currentObj.DNS[3].ToString();
            }
        }
        public void OnChangeServerIP(object sender, EventArgs e)
        {
            if (currentObj != null)
            {
                int ip1, ip2, ip3, ip4, port;
                if (int.TryParse(teServerIP1.Text, out ip1) &&
                    int.TryParse(teServerIP2.Text, out ip2) &&
                    int.TryParse(teServerIP3.Text, out ip3) &&
                    int.TryParse(teServerIP4.Text, out ip4) &&
                    int.TryParse(teServerPort.Text, out port))
                    currentObj.SetServerIP((byte)ip1, (byte)ip2, (byte)ip3, (byte)ip4, (ushort)port);
            }
        }
        private void SetServerIP()
        {
            if (currentObj != null)
            {
                if (teServerIP1.Text != currentObj.Remote_IP[0].ToString())
                    teServerIP1.Text = currentObj.Remote_IP[0].ToString();
                if (teServerIP2.Text != currentObj.Remote_IP[1].ToString())
                    teServerIP2.Text = currentObj.Remote_IP[1].ToString();
                if (teServerIP3.Text != currentObj.Remote_IP[2].ToString())
                    teServerIP3.Text = currentObj.Remote_IP[2].ToString();
                if (teServerIP4.Text != currentObj.Remote_IP[3].ToString())
                    teServerIP4.Text = currentObj.Remote_IP[3].ToString();
                if (teServerPort.Text != currentObj.Remote_Port.ToString())
                    teServerPort.Text = currentObj.Remote_Port.ToString();
            }
        }
        public void DxRenderLogic()
        {
            currentObj = PublicData.GetCurrentSelectJoyObject();
            customPinControl.Hide = false;
            linkPinControl.Hide = false;
            linkDataControl.Hide = false;
            if (currentObj != null)
            {
                PublicData.SetPanelColor("CustomPinColor", currentObj.GetCustomPinColor());
                PublicData.SetButtonList("CustomPinButton", currentObj.GetCustomPinUsed());
                PublicData.SetButtonEnable("CustomPinButton", currentObj.GetCustomPinEnable());
                PublicData.SetButtonSelect("CustomLink", currentObj.SelectCustomPin, JoyConst.MaxCustomCount);
                PublicData.SetButtonSelect("FontSetButton", currentObj.SelectFontSet, JoyConst.MaxFontSetCount);
                PublicData.SetPanelColor("CustomDataSpeed", currentObj.GetCustomDataSpeed());
                PublicData.SetPanelColor("FontSetColor", currentObj.GetFontSetUsed());
                PublicData.SetButtonList("CustomDataButton", currentObj.GetCustomDataUsed());
                currentCus = currentObj.GetCurrentJoyCustom();
                //----
                SetMAC();
                SetIP();
                SetSubNet();
                SetGateway();
                SetDNS();
                SetServerIP();
                //----
                dataLC.Hide = true;
                csLC.Hide = true;
                clkLC.Hide = true;
                dcLC.Hide = true;
                rstLC.Hide = true;
                cs2LC.Hide = true;
                cs3LC.Hide = true;
                cs4LC.Hide = true;
                //----
                oledControl.Hide = true;
                linkCountControl.Hide = true;
                rotateControl.Hide = true;
                fontSetControl.Hide = true;
                lanControl.Hide = true;
                if (currentCus != null)
                {
                    switch (currentCus.rotateType)
                    {
                        case RotateType.Rotate0:
                            rotate0.bSwitchOn = true;
                            rotate90.bSwitchOn = false;
                            rotate180.bSwitchOn = false;
                            rotate270.bSwitchOn = false;
                            break;
                        case RotateType.Rotate90:
                            rotate0.bSwitchOn = false;
                            rotate90.bSwitchOn = true;
                            rotate180.bSwitchOn = false;
                            rotate270.bSwitchOn = false;
                            break;
                        case RotateType.Rotate180:
                            rotate0.bSwitchOn = false;
                            rotate90.bSwitchOn = false;
                            rotate180.bSwitchOn = true;
                            rotate270.bSwitchOn = false;
                            break;
                        case RotateType.Rotate270:
                            rotate0.bSwitchOn = false;
                            rotate90.bSwitchOn = false;
                            rotate180.bSwitchOn = false;
                            rotate270.bSwitchOn = true;
                            break;
                    }
                    if (tbLinkCount.Value != currentCus.dataCount)
                        tbLinkCount.Value = currentCus.dataCount;
                    switch (currentCus.Type)
                    {
                        case CustomType.NoneCustom:
                            linkPinControl.Hide = true;
                            linkDataControl.Hide = true;
                            break;
                        case CustomType.DT_Max7219:
                            dataLC.Hide = false;
                            dataBtn.Name = "DIN";
                            csLC.Hide = false;
                            csBtn.Name = "LOAD";
                            clkLC.Hide = false;
                            clkBtn.Name = "CLK";
                            //------------------------------
                            linkCountControl.Hide = false;
                            linkCountTip.Text = "LinkCountTL";
                            tlLinkCount.Text = "LinkCount";
                            break;
                        case CustomType.DT_TM1638:
                            dataLC.Hide = false;
                            dataBtn.Name = "DIN";
                            csLC.Hide = false;
                            csBtn.Name = "STB";
                            clkLC.Hide = false;
                            clkBtn.Name = "CLK";
                            //------------------------------
                            break;
                        case CustomType.DT_HT16K33:
                            dataLC.Hide = false;
                            dataBtn.Name = "SDA";
                            clkLC.Hide = false;
                            clkBtn.Name = "SCL";
                            //------------------------------
                            break;
                        case CustomType.Matrix_Max7219:
                            dataLC.Hide = false;
                            dataBtn.Name = "DIN";
                            csLC.Hide = false;
                            csBtn.Name = "LOAD";
                            clkLC.Hide = false;
                            clkBtn.Name = "CLK";
                            //------------------------------
                            linkCountControl.Hide = false;
                            linkCountTip.Text = "LinkCountTL";
                            tlLinkCount.Text = "LinkCount";
                            SetOled(currentCus);
                            break;
                        //----
                        case CustomType.OLED_70_40_SSD1306://0.42
                        case CustomType.OLED_48_64_SSD1306://0.71
                        case CustomType.OLED_64_32_SSD1306://0.49
                        case CustomType.OLED_64_48_SSD1306://0.66
                        case CustomType.OLED_96_16_SSD1306://0.86
                        case CustomType.OLED_128_32_SSD1306://0.91
                        case CustomType.OLED_128_64_SSD1306://0.96, 1.09
                        case CustomType.OLED_128_64_SH1106://1.3
                        case CustomType.OLED_128_88_SH1107://0.73
                        case CustomType.OLED_256_64_SSD1322://3.12
                            dataLC.Hide = false;
                            dataBtn.Name = "MOSI";
                            csLC.Hide = false;
                            csBtn.Name = "CS";
                            clkLC.Hide = false;
                            clkBtn.Name = "CLK";
                            dcLC.Hide = false;
                            dcBtn.Name = "D/C";
                            rstLC.Hide = false;
                            rstBtn.Name = "RES";
                            //------------------------------
                            oledControl.Hide = false;
                            rotateControl.Hide = false;
                            SetOled(currentCus);
                            oledShow.Update(currentObj);
                            break;
                        case CustomType.OLED_70_40_SSD1306x2:
                        case CustomType.OLED_48_64_SSD1306x2:
                        case CustomType.OLED_64_32_SSD1306x2:
                        case CustomType.OLED_64_48_SSD1306x2:
                        case CustomType.OLED_96_16_SSD1306x2:
                        case CustomType.OLED_128_32_SSD1306x2:
                        case CustomType.OLED_128_64_SSD1306x2:
                        case CustomType.OLED_128_64_SH1106x2:
                        case CustomType.OLED_128_88_SH1107x2:
                        case CustomType.OLED_256_64_SSD1322x2:
                            dataLC.Hide = false;
                            dataBtn.Name = "MOSI";
                            csLC.Hide = false;
                            csBtn.Name = "CS";
                            clkLC.Hide = false;
                            clkBtn.Name = "CLK";
                            dcLC.Hide = false;
                            dcBtn.Name = "D/C";
                            rstLC.Hide = false;
                            rstBtn.Name = "RES";
                            cs2LC.Hide = false;
                            cs2Btn.Name = "CS 2";
                            //------------------------------
                            oledControl.Hide = false;
                            rotateControl.Hide = false;
                            SetOled(currentCus);
                            oledShow.Update(currentObj);
                            break;
                        case CustomType.OLED_70_40_SSD1306x3:
                        case CustomType.OLED_48_64_SSD1306x3:
                        case CustomType.OLED_64_32_SSD1306x3:
                        case CustomType.OLED_64_48_SSD1306x3:
                        case CustomType.OLED_96_16_SSD1306x3:
                        case CustomType.OLED_128_32_SSD1306x3:
                        case CustomType.OLED_128_64_SSD1306x3:
                        case CustomType.OLED_128_64_SH1106x3:
                        case CustomType.OLED_128_88_SH1107x3:
                        case CustomType.OLED_256_64_SSD1322x3:
                            dataLC.Hide = false;
                            dataBtn.Name = "MOSI";
                            csLC.Hide = false;
                            csBtn.Name = "CS";
                            clkLC.Hide = false;
                            clkBtn.Name = "CLK";
                            dcLC.Hide = false;
                            dcBtn.Name = "D/C";
                            rstLC.Hide = false;
                            rstBtn.Name = "RES";
                            cs2LC.Hide = false;
                            cs2Btn.Name = "CS 2";
                            cs3LC.Hide = false;
                            cs3Btn.Name = "CS 3";
                            //------------------------------
                            oledControl.Hide = false;
                            rotateControl.Hide = false;
                            SetOled(currentCus);
                            oledShow.Update(currentObj);
                            break;
                        case CustomType.OLED_70_40_SSD1306x4:
                        case CustomType.OLED_48_64_SSD1306x4:
                        case CustomType.OLED_64_32_SSD1306x4:
                        case CustomType.OLED_64_48_SSD1306x4:
                        case CustomType.OLED_96_16_SSD1306x4:
                        case CustomType.OLED_128_32_SSD1306x4:
                        case CustomType.OLED_128_64_SSD1306x4:
                        case CustomType.OLED_128_64_SH1106x4:
                        case CustomType.OLED_128_88_SH1107x4:
                        case CustomType.OLED_256_64_SSD1322x4:
                            dataLC.Hide = false;
                            dataBtn.Name = "MOSI";
                            csLC.Hide = false;
                            csBtn.Name = "CS";
                            clkLC.Hide = false;
                            clkBtn.Name = "CLK";
                            dcLC.Hide = false;
                            dcBtn.Name = "D/C";
                            rstLC.Hide = false;
                            rstBtn.Name = "RES";
                            cs2LC.Hide = false;
                            cs2Btn.Name = "CS 2";
                            cs3LC.Hide = false;
                            cs3Btn.Name = "CS 3";
                            cs4LC.Hide = false;
                            cs4Btn.Name = "CS 4";
                            //------------------------------
                            oledControl.Hide = false;
                            rotateControl.Hide = false;
                            SetOled(currentCus);
                            oledShow.Update(currentObj);
                            break;
                        case CustomType.OUT_StepperMotor:
                            dataLC.Hide = false;
                            dataBtn.Name = "STEP";
                            clkLC.Hide = false;
                            clkBtn.Name = "DIR";
                            //------------------------------
                            break;
                        case CustomType.OUT_74HC595:
                            dataLC.Hide = false;
                            dataBtn.Name = "DS";
                            csLC.Hide = false;
                            csBtn.Name = "SHCP";
                            clkLC.Hide = false;
                            clkBtn.Name = "STCP";
                            //------------------------------
                            linkCountControl.Hide = false;
                            linkCountTip.Text = "LinkCountTL";
                            tlLinkCount.Text = "LinkCount";
                            break;
                        case CustomType.OUT_IO:
                            dataLC.Hide = false;
                            dataBtn.Name = "IO";
                            break;
                        case CustomType.OUT_NRF24:
                            dataLC.Hide = false;
                            dataBtn.Name = "MOSI";
                            csLC.Hide = false;
                            csBtn.Name = "CSN";
                            clkLC.Hide = false;
                            clkBtn.Name = "SCK";
                            dcLC.Hide = false;
                            dcBtn.Name = "MISO";
                            rstLC.Hide = false;
                            rstBtn.Name = "CE";
                            //------------------------------
                            linkCountControl.Hide = false;
                            linkCountTip.Text = "NRF24ChannelTL";
                            tlLinkCount.Text = "NRF24Channel";
                            break;
                        case CustomType.OUT_W5500:
                            rstLC.Hide = false;
                            rstBtn.Name = "RST";
                            //------------------------------
                            lanControl.Hide = false;
                            break;
                    }
                }
            }
        }
    }
}
