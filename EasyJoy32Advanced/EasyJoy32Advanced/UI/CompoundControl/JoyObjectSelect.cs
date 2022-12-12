using SharpDX;
using System;
using System.Collections.Generic;

namespace EasyControl
{
    public class JoyObjectSelect : iUiLogic
    {
        //---------------------------------------------------------------------------------------
        uiButton btnSelect;
        uiButton btnGetInfo;
        uiButton btnSetData;
        uiButton btnOpen;
        uiButton btnSave;
        uiButton btnLicense;
        uiButton btnDevice;
        uiButton btnCustom;
        uiButton btnFont;
        LayoutControl joyObjectControl;
        LayoutControl joyObjectList;
        LayoutControl joyFunction;
        LayoutControl TypeSwitchControl;
        LayoutControl deviceControl;
        LayoutControl deviceList;
        LayoutControl customControl;
        LayoutControl customList;
        LayoutControl fontControl;
        LayoutControl fontList;
        List<LayoutControl> objLcList = new List<LayoutControl>();
        float objectHeight = 0f;
        float deviceHeight = 0f;
        float customHeight = 0f;
        float fontHeight = 0f;
        //////////////////////////////////////////////////////////////////////////////////////////        
        public static readonly JoyObjectSelect Instance = new JoyObjectSelect();
        private JoyObjectSelect()
        {
        }
        public void Init()
        {
            #region Refresh
            uiButton btnRefresh = XmlUI.Instance.GetButton("Refresh");
            btnRefresh.LeftButtonClick += OnRefreshClick;
            #endregion
            #region Back
            uiButton btnBack = XmlUI.Instance.GetButton("Back");
            btnBack.LeftButtonClick += OnBackClick;
            #endregion
            #region JoyObjectSelect
            btnSelect = XmlUI.Instance.GetButton("JoyObjectSelect");
            btnSelect.LeftButtonClick += OnSelectClick;
            #endregion
            #region joyFunction
            joyFunction = XmlUI.Instance.GetLayoutControl("JoyFunction");
            btnSetData = XmlUI.Instance.GetButton("JoySetData");
            btnSetData.LeftButtonClick += OnSetDataClick;
            btnOpen = XmlUI.Instance.GetButton("OpenSetData");
            btnOpen.LeftButtonClick += OnOpenClick;
            btnSave = XmlUI.Instance.GetButton("SaveSetData");
            btnSave.LeftButtonClick += OnSaveClick;
            btnLicense = XmlUI.Instance.GetButton("JoyLicense");
            btnLicense.LeftButtonClick += OnLicenseClick;
            #endregion
            #region TypeSwitchControl
            TypeSwitchControl = XmlUI.Instance.GetLayoutControl("TypeSwitchControl");
            btnDevice = XmlUI.Instance.GetButton("JoyDeviceSelect");
            btnDevice.LeftButtonClick += OnDeviceTypeClick;
            btnCustom = XmlUI.Instance.GetButton("JoyCustomSelect");
            btnCustom.LeftButtonClick += OnCustomTypeClick;
            btnFont = XmlUI.Instance.GetButton("FontLibrarySelect");
            btnFont.LeftButtonClick += OnFontLibraryClick;
            #endregion
            #region JoyObjectList
            joyObjectControl = XmlUI.Instance.GetLayoutControl("JoyObjectControl");
            joyObjectList = XmlUI.Instance.GetLayoutControl("JoyObjectListLC");
            for (int i = 0; i < JoyConst.MaxJoyObject; i++)
            {
                LayoutControl lcJoyObject = XmlUI.Instance.CreateUI(System.Environment.CurrentDirectory + @"\Xml\JoyObjectSelect.xml", i.ToString());
                if (lcJoyObject != null)
                {
                    LayoutControl lcObject = XmlUI.Instance.GetLayoutControl(i + "JoyObjectLC");
                    objectHeight = lcObject.maxHeight;
                    objLcList.Add(lcJoyObject);
                    uiButton btn = XmlUI.Instance.GetButton(i + "JoyObjectIndex");
                    if (btn != null)
                    {
                        btn.Index = i;
                        btn.LeftButtonClick += OnObjClick;
                    }
                    uiButton btnSetting = XmlUI.Instance.GetButton(i + "Setting");
                    if (btnSetting != null)
                    {
                        btnSetting.Index = i;
                        btnSetting.LeftButtonClick += OnSettingClick;
                    }
                    uiButton btnReboot = XmlUI.Instance.GetButton(i + "ReBoot");
                    if (btnReboot != null)
                    {
                        btnReboot.Index = i;
                        btnReboot.LeftButtonClick += OnRebootClick;
                    }
                    joyObjectList.AddObject(lcJoyObject);
                }
            }
            #endregion
            #region DeviceList
            deviceControl = XmlUI.Instance.GetLayoutControl("DeviceControl");
            deviceControl.Hide = true;
            deviceList = XmlUI.Instance.GetLayoutControl("DeviceListLC");
            for (int i = 0; i < JoyConst.MaxDevice; i++)
            {
                LayoutControl lcJoyObject = XmlUI.Instance.CreateUI(System.Environment.CurrentDirectory + @"\Xml\DeviceSelect.xml", i.ToString());
                deviceList.AddObject(lcJoyObject);
                LayoutControl lcDevice = XmlUI.Instance.GetLayoutControl(i + "JoyDeviceLC");
                deviceHeight = lcDevice.maxHeight;
                uiButton btn = XmlUI.Instance.GetButton(i + "JoyDeviceIndex");
                if (btn != null)
                {
                    btn.Index = i;
                    btn.LeftButtonClick += OnDeviceClick;
                }
                uiButton btnUp = XmlUI.Instance.GetButton(i + "JoyDeviceUp");
                if (btnUp != null)
                {
                    btnUp.Index = i;
                    btnUp.TextColor = XmlUI.DxDeviceGreen;
                    btnUp.LeftButtonClick += OnDeviceUpClick;
                }
                uiButton btnDown = XmlUI.Instance.GetButton(i + "JoyDeviceDown");
                if (btnDown != null)
                {
                    btnDown.Index = i;
                    btnDown.TextColor = XmlUI.DxDeviceRed;
                    btnDown.LeftButtonClick += OnDeviceDownClick;
                }
            }
            #endregion
            #region CustomList
            customControl = XmlUI.Instance.GetLayoutControl("CustomControl");
            customControl.Hide = true;
            customList = XmlUI.Instance.GetLayoutControl("CustomListLC");
            for (int i = 0; i < JoyConst.MaxCustom; i++)
            {
                LayoutControl lcJoyObject = XmlUI.Instance.CreateUI(System.Environment.CurrentDirectory + @"\Xml\CustomSelect.xml", i.ToString());
                customList.AddObject(lcJoyObject);
                LayoutControl lcCustom = XmlUI.Instance.GetLayoutControl(i + "JoyCustomLC");
                customHeight = lcCustom.maxHeight;
                uiButton btn = XmlUI.Instance.GetButton(i + "JoyCustomIndex");
                if (btn != null)
                {
                    btn.Index = i;
                    btn.LeftButtonClick += OnCustomClick;
                }
                uiButton btnUp = XmlUI.Instance.GetButton(i + "JoyCustomUp");
                if (btnUp != null)
                {
                    btnUp.Index = i;
                    btnUp.TextColor = XmlUI.DxDeviceGreen;
                    btnUp.LeftButtonClick += OnCustomUpClick;
                }
                uiButton btnDown = XmlUI.Instance.GetButton(i + "JoyCustomDown");
                if (btnDown != null)
                {
                    btnDown.Index = i;
                    btnDown.TextColor = XmlUI.DxDeviceRed;
                    btnDown.LeftButtonClick += OnCustomDownClick;
                }
            }
            #endregion
            #region FontList
            fontControl = XmlUI.Instance.GetLayoutControl("FontControl");
            fontControl.Hide = true;
            fontList = XmlUI.Instance.GetLayoutControl("FontListLC");
            for (int i = 0; i < JoyConst.MaxFont; i++)
            {
                LayoutControl lcFontSelect = XmlUI.Instance.CreateUI(System.Environment.CurrentDirectory + @"\Xml\FontSelect.xml", i.ToString());
                fontList.AddObject(lcFontSelect);
                LayoutControl lcFont = XmlUI.Instance.GetLayoutControl(i + "FontSelectLC");
                fontHeight = lcFont.maxHeight;
                uiButton btn = XmlUI.Instance.GetButton(i + "FontSelectIndex");
                if (btn != null)
                {
                    btn.Index = i;
                    btn.LeftButtonClick += OnFontClick;
                }
            }
            #endregion
        }
        private void SetDeviceButton(int index)
        {
            JoyObject joyObj = PublicData.GetCurrentSelectJoyObject();
            if (joyObj != null)
            {
                if (index >= 0 && index < JoyConst.MaxDevice)
                {
                    uiButton btn = XmlUI.Instance.GetButton(index + "JoyDeviceIndex");
                    uiPanel panel = XmlUI.Instance.GetPanel(index + "CurrentDevice");
                    JoyDevice joyDev = joyObj.GetJoyDevice(index);
                    if (btn != null && joyDev != null)
                    {
                        btn.Name = Localization.Instance.GetLS(joyDev.Type.ToString());
                        if (joyDev.Type == DeviceType.None)
                        {
                            btn.textAlignment = SharpDX.DirectWrite.TextAlignment.Center;
                        }
                        else
                        {
                            btn.textAlignment = SharpDX.DirectWrite.TextAlignment.Leading;
                        }
                        panel.ForeColor = XmlUI.DxUIBackColor;
                        if (joyDev.portInType == InPortType.Pin && joyDev.inPort == joyObj.currentChangePin)
                        {
                            panel.ForeColor = XmlUI.DxDeviceBlue;
                        }
                        if (joyDev.portInType == InPortType.FormatOut && joyDev.inPort == joyObj.currentChangeFormat ||
                            joyDev.portOutType == OutPortType.FormatIn && joyDev.outPort == joyObj.currentChangeFormat)
                        {
                            panel.ForeColor = XmlUI.DxDeviceGreen;
                        }
                        uiPanel pan = XmlUI.Instance.GetPanel(index + "DeviceColor");
                        pan.ForeColor = PublicData.GetDeviceTypeColor(joyDev.Type);
                    }
                }
            }
        }
        private void SetCustomButton(int index)
        {
            JoyObject joyObj = PublicData.GetCurrentSelectJoyObject();
            if (joyObj != null)
            {
                if (index >= 0 && index < JoyConst.MaxCustom)
                {
                    uiButton btn = XmlUI.Instance.GetButton(index + "JoyCustomIndex");
                    JoyCustom joyCus = joyObj.GetJoyCustom(index);
                    if (btn != null && joyCus != null)
                    {
                        btn.Name = Localization.Instance.GetLS(joyCus.Type.ToString());
                        if (joyCus.Type == CustomType.NoneCustom)
                        {
                            btn.textAlignment = SharpDX.DirectWrite.TextAlignment.Center;
                        }
                        else
                        {
                            btn.textAlignment = SharpDX.DirectWrite.TextAlignment.Leading;
                        }
                        uiPanel pan = XmlUI.Instance.GetPanel(index + "CustomColor");
                        pan.ForeColor = PublicData.GetCustomTypeColor(joyCus.Type);
                    }
                }
            }
        }
        public void RefreshClick()
        {
            OnRefreshClick(null, null);
        }
        private void OnRefreshClick(object sender, EventArgs e)
        {
            PublicData.CurrentJoyObjectIndex = -1;
            JoyUSB.Instance.Refresh();
        }
        private void OnBackClick(object sender, EventArgs e)
        {
            PublicData.CurrentJoyObjectIndex = -1;
        }
        private void OnSelectClick(object sender, EventArgs e)
        {
        }
        private void OnSetDataClick(object sender, EventArgs e)
        {
            JoyObject joyObj = PublicData.GetCurrentSelectJoyObject();
            if (joyObj != null)
            {
                string info;
                if (joyObj.CheckJoyError(out info))
                {
                    string logMsg;
                    PublicData.SaveJoy(false, joyObj, "Ejoy_" + joyObj.McuID + "_" + DateTime.Now.ToString("yyyy_MM_dd_HH_mm_ssffff"), out logMsg);
                    joyObj.AddReport(new Report(ReportType.DeviceData));
                    joyObj.AddReport(new Report(ReportType.CustomData));
                    joyObj.AddReport(new Report(ReportType.ButtonData));
                    joyObj.AddReport(new Report(ReportType.HatData));
                    joyObj.AddReport(new Report(ReportType.AdcData));
                    joyObj.AddReport(new Report(ReportType.FormatData));
                    joyObj.AddReport(new Report(ReportType.LedData));
                    joyObj.AddReport(new Report(ReportType.SaveUsbData));
                }
                else
                {
                    WarningForm.Instance.OpenUI(info);
                }
            }
        }
        private void OnOpenClick(object sender, EventArgs e)
        {
            OpenFileForm.Instance.OpenEasyJoy();
        }
        private void OnSaveClick(object sender, EventArgs e)
        {
            SaveFileForm.Instance.SaveEasyJoy();
        }
        private void OnLicenseClick(object sender, EventArgs e)
        {
            JoyObject joyObj = PublicData.GetCurrentSelectJoyObject();
            if (joyObj != null)
            {
                joyObj.GetLicense();
            }
        }
        private void OnDeviceTypeClick(object sender, EventArgs e)
        {
            JoyObject joyObj = PublicData.GetCurrentSelectJoyObject();
            if (joyObj != null)
            {
                joyObj.TypeSwitchControl = TypeSwitch.DeviceSwitch;
            }
        }
        private void OnCustomTypeClick(object sender, EventArgs e)
        {
            JoyObject joyObj = PublicData.GetCurrentSelectJoyObject();
            if (joyObj != null)
            {
                joyObj.TypeSwitchControl = TypeSwitch.CustomSwitch;
            }
        }
        private void OnFontLibraryClick(object sender, EventArgs e)
        {
            JoyObject joyObj = PublicData.GetCurrentSelectJoyObject();
            if (joyObj != null)
            {
                joyObj.TypeSwitchControl = TypeSwitch.FontLibrarySwitch;
            }
        }
        private void OnObjClick(object sender, EventArgs e)
        {
            JoyIndexChangeArgs args = (JoyIndexChangeArgs)e;
            JoyObject obj = JoyUSB.Instance.GetJoyObjectAtIndex(args.Index);
            if (obj != null)
                NodeLinkControl.Instance.SetNodeCentered(obj.PluginID, 0);
        }
        private void OnSettingClick(object sender, EventArgs e)
        {
            JoyIndexChangeArgs args = (JoyIndexChangeArgs)e;
            PublicData.CurrentJoyObjectIndex = args.Index;
        }
        private void OnRebootClick(object sender, EventArgs e)
        {
            JoyIndexChangeArgs args = (JoyIndexChangeArgs)e;
            List<JoyObject> joyList = JoyUSB.Instance.GetJoyList();
            if (args.Index >= 0 && args.Index < joyList.Count)
            {
                joyList[args.Index].AddReport(new Report(ReportType.ReBoot));
            }
        }
        private void OnDeviceClick(object sender, EventArgs e)
        {
            JoyObject joyObj = PublicData.GetCurrentSelectJoyObject();
            if (joyObj != null)
            {
                JoyIndexChangeArgs args = (JoyIndexChangeArgs)e;
                joyObj.SelectDevice = args.Index;
            }
        }
        private void OnDeviceUpClick(object sender, EventArgs e)
        {
            JoyIndexChangeArgs args = (JoyIndexChangeArgs)e;
            if (args.Index > 0 && args.Index < JoyConst.MaxDevice)
            {
                JoyObject joyObj = PublicData.GetCurrentSelectJoyObject();
                if (joyObj != null)
                {
                    joyObj.SwitchDevice(args.Index, args.Index - 1);
                }
            }
        }
        private void OnDeviceDownClick(object sender, EventArgs e)
        {
            JoyIndexChangeArgs args = (JoyIndexChangeArgs)e;
            if (args.Index >= 0 && args.Index < JoyConst.MaxDevice - 1)
            {
                JoyObject joyObj = PublicData.GetCurrentSelectJoyObject();
                if (joyObj != null)
                {
                    joyObj.SwitchDevice(args.Index, args.Index + 1);
                }
            }
        }
        private void OnCustomClick(object sender, EventArgs e)
        {
            JoyObject joyObj = PublicData.GetCurrentSelectJoyObject();
            if (joyObj != null)
            {
                JoyIndexChangeArgs args = (JoyIndexChangeArgs)e;
                joyObj.SelectCustom = args.Index;
            }
        }
        private void OnCustomUpClick(object sender, EventArgs e)
        {
            JoyIndexChangeArgs args = (JoyIndexChangeArgs)e;
            if (args.Index > 0 && args.Index < JoyConst.MaxCustom)
            {
                JoyObject joyObj = PublicData.GetCurrentSelectJoyObject();
                if (joyObj != null)
                {
                    joyObj.SwitchCustom(args.Index, args.Index - 1);
                }
            }
        }
        private void OnCustomDownClick(object sender, EventArgs e)
        {
            JoyIndexChangeArgs args = (JoyIndexChangeArgs)e;
            if (args.Index >= 0 && args.Index < JoyConst.MaxCustom - 1)
            {
                JoyObject joyObj = PublicData.GetCurrentSelectJoyObject();
                if (joyObj != null)
                {
                    joyObj.SwitchCustom(args.Index, args.Index + 1);
                }
            }
        }
        private void OnFontClick(object sender, EventArgs e)
        {
            JoyObject joyObj = PublicData.GetCurrentSelectJoyObject();
            if (joyObj != null)
            {
                JoyIndexChangeArgs args = (JoyIndexChangeArgs)e;
                joyObj.SelectFont = args.Index;
            }
        }

        public void DxRenderLogic()
        {
            JoyObject joyObj = PublicData.GetCurrentSelectJoyObject();
            if (joyObj != null)
            {
                joyObjectControl.Hide = true;
                for (int i = 0; i < JoyConst.MaxDevice; i++)
                {
                    SetDeviceButton(i);
                }
                for (int i = 0; i < JoyConst.MaxCustom; i++)
                {
                    SetCustomButton(i);
                }
                #region DeviceSelect
                for (int i = 0; i < JoyConst.MaxDevice; i++)
                {
                    SetDeviceButton(joyObj.SelectDevice);
                    uiButton devBtn = XmlUI.Instance.GetButton(i + "JoyDeviceIndex");
                    if (devBtn != null)
                    {
                        if (joyObj.SelectDevice == i)
                            devBtn.BackColor = XmlUI.DxDeviceBlue;
                        else
                            devBtn.BackColor = XmlUI.DxUIBackColor;
                    }
                }
                #endregion
                #region CustomSelect
                btnCustom.Enable = joyObj.GetCustomPageEnble();
                for (int i = 0; i < JoyConst.MaxCustom; i++)
                {
                    SetCustomButton(joyObj.SelectCustom);
                    uiButton cusBtn = XmlUI.Instance.GetButton(i + "JoyCustomIndex");
                    if (cusBtn != null)
                    {
                        if (joyObj.SelectCustom == i)
                            cusBtn.BackColor = XmlUI.DxDeviceBlue;
                        else
                            cusBtn.BackColor = XmlUI.DxUIBackColor;
                    }
                }
                #endregion
                #region FontLib
                btnFont.Enable = joyObj.GetFontPageEnable();
                for (int i = 0; i < JoyConst.MaxFont; i++)
                {
                    uiButton fontBtn = XmlUI.Instance.GetButton(i + "FontSelectIndex");
                    if (fontBtn != null)
                    {
                        if (joyObj.SelectFont == i)
                            fontBtn.BackColor = XmlUI.DxDeviceBlue;
                        else
                            fontBtn.BackColor = XmlUI.DxUIBackColor;
                    }
                }
                #endregion
                btnSelect.Name = joyObj.ToString();
                joyFunction.Hide = false;
                TypeSwitchControl.Hide = false;
                switch (joyObj.TypeSwitchControl)
                {
                    case TypeSwitch.DeviceSwitch:
                        for (int devIndex = 0; devIndex < JoyConst.MaxDevice; devIndex++)
                        {
                            SetDeviceButton(devIndex);
                        }
                        btnDevice.BackColor = XmlUI.DxDeviceBlue;
                        btnCustom.BackColor = XmlUI.DxUIBackColor;
                        btnFont.BackColor = XmlUI.DxUIBackColor;
                        deviceControl.Hide = false;
                        customControl.Hide = true;
                        fontControl.Hide = true;
                        break;
                    case TypeSwitch.CustomSwitch:
                        btnDevice.BackColor = XmlUI.DxUIBackColor;
                        btnCustom.BackColor = XmlUI.DxDeviceBlue;
                        btnFont.BackColor = XmlUI.DxUIBackColor;
                        deviceControl.Hide = true;
                        customControl.Hide = false;
                        fontControl.Hide = true;
                        break;
                    case TypeSwitch.FontLibrarySwitch:
                        btnDevice.BackColor = XmlUI.DxUIBackColor;
                        btnCustom.BackColor = XmlUI.DxUIBackColor;
                        btnFont.BackColor = XmlUI.DxDeviceBlue;
                        deviceControl.Hide = true;
                        customControl.Hide = true;
                        fontControl.Hide = false;
                        break;
                }
                deviceList.Rect = new RectangleF(0f, 0f, deviceControl.DrawRect.Width - ViewControl.sliderWidth, deviceHeight * JoyConst.MaxDevice);
                customList.Rect = new RectangleF(0f, 0f, customControl.DrawRect.Width - ViewControl.sliderWidth, customHeight * JoyConst.MaxCustom);
                fontList.Rect = new RectangleF(0f, 0f, fontControl.DrawRect.Width - ViewControl.sliderWidth, fontHeight * JoyConst.MaxFont);
                #region 信息数量条
                int reportCount = joyObj.reportCount;
                for (int msgIndex = 0; msgIndex < 30; msgIndex++)
                {
                    uiPanel msgPan = XmlUI.Instance.GetPanel("msgPublic" + msgIndex);
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
                #endregion
            }
            else
            {
                joyObjectControl.Hide = false;
                deviceControl.Hide = true;
                customControl.Hide = true;
                fontControl.Hide = true;
                btnSelect.Name = "……";
                joyFunction.Hide = true;
                TypeSwitchControl.Hide = true;
                #region 信息数量条
                for (int msgIndex = 0; msgIndex < 30; msgIndex++)
                {
                    uiPanel msgPan = XmlUI.Instance.GetPanel("msgPublic" + msgIndex);
                    msgPan.ForeColor = XmlUI.DxUIBackColor;
                }
                #endregion
                List<JoyObject> joyList = JoyUSB.Instance.GetJoyList();
                int btnIndex = 0;
                int index = joyList.Count < JoyConst.MaxJoyObject ? joyList.Count : JoyConst.MaxJoyObject;
                for (btnIndex = 0; btnIndex < index; btnIndex++)
                {
                    LayoutControl lcObject = XmlUI.Instance.GetLayoutControl(btnIndex + "JoyObjectLC");
                    JoyObject joy = joyList[btnIndex];
                    int reportCount = joy.reportCount;
                    for (int msgIndex = 0; msgIndex < 20; msgIndex++)
                    {
                        uiPanel msgPan = XmlUI.Instance.GetPanel(btnIndex + "msg" + msgIndex);
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
                    uiButton btn = XmlUI.Instance.GetButton(btnIndex + "JoyObjectIndex");
                    btn.Name = joy.ToString();
                    btn.BackColor = joy.LicenseUID ? XmlUI.DxUIBackColor : XmlUI.DxDeviceRed;
                    uiPanel pan = XmlUI.Instance.GetPanel(btnIndex + "Link");
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
                float height = objectHeight * btnIndex;
                if (height < joyObjectList.DrawRect.Height)
                    joyObjectList.Rect = new RectangleF(0f, 0f, joyObjectControl.DrawRect.Width, height);
                else
                    joyObjectList.Rect = new RectangleF(0f, 0f, joyObjectControl.DrawRect.Width - ViewControl.sliderWidth, height);
                for (int i = btnIndex; i < JoyConst.MaxJoyObject; i++)
                {
                    objLcList[i].Hide = true;
                }
            }
        }
    }
}
