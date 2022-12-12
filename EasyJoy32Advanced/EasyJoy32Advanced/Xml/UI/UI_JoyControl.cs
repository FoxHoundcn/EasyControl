using SharpDX;
using System;

namespace EasyControl
{
    public class UI_JoyControl : iUiLogic
    {
        public LayoutControl mainLayout { get; private set; }
        //------------------------------------------------------------------------------------------------------
        LayoutControl ejoyControlMain;
        LayoutControl deviceTypeControl;
        LayoutControl customTypeControl;
        LayoutControl fontTypeControl;
        LayoutControl deviceControlControl;
        LayoutControl customControlControl;
        LayoutControl fontControlControl;
        //------------------------------------------------------------------------------------------------------
        public static readonly UI_JoyControl Instance = new UI_JoyControl();
        private UI_JoyControl()
        {
        }
        //============================================================
        public void DxRenderLogic()
        {
            JoyObjectSelect.Instance.DxRenderLogic();
            DeviceTypeSelect.Instance.DxRenderLogic();
            CustomTypeSelect.Instance.DxRenderLogic();
            FontTypeSelect.Instance.DxRenderLogic();
            JoyControlPanel.Instance.DxRenderLogic();
            CustomControlPanel.Instance.DxRenderLogic();
            DeviceControlControl.Instance.DxRenderLogic();
            FontLibraryControl.Instance.DxRenderLogic();
            //--------------------------------------------------------------------------------------------
            LayoutControl leftMain = XmlUI.Instance.GetLayoutControl("LeftMain");
            JoyObject joyObj = PublicData.GetCurrentSelectJoyObject();
            if (joyObj == null)
            {
                SaveFileForm.Instance.Close();
                OpenFileForm.Instance.Close();
                ejoyControlMain.Hide = true;
            }
            else
            {
                ejoyControlMain.Hide = false;
                deviceTypeControl.Hide = false;
                customTypeControl.Hide = false;
                fontTypeControl.Hide = false;
                deviceControlControl.Hide = false;
                customControlControl.Hide = false;
                fontControlControl.Hide = false;
                switch (joyObj.TypeSwitchControl)
                {
                    case TypeSwitch.DeviceSwitch:
                        customTypeControl.Hide = true;
                        customControlControl.Hide = true;
                        fontTypeControl.Hide = true;
                        fontControlControl.Hide = true;
                        break;
                    case TypeSwitch.CustomSwitch:
                        deviceTypeControl.Hide = true;
                        deviceControlControl.Hide = true;
                        fontTypeControl.Hide = true;
                        fontControlControl.Hide = true;
                        break;
                    case TypeSwitch.FontLibrarySwitch:
                        customTypeControl.Hide = true;
                        customControlControl.Hide = true;
                        deviceTypeControl.Hide = true;
                        deviceControlControl.Hide = true;
                        break;
                }
            }
            //--------------------------------------------------------------------------------------------
            ViewControl vc = XmlUI.Instance.GetViewControl("vcInputControl");
            float width = vc.DrawRect.Width - ViewControl.sliderWidth - 1f;
            if (width > 700f)
                width = 700f;
            LayoutControl nlcLC = XmlUI.Instance.GetLayoutControl("vcInputControlLC");
            float maxPlace = nlcLC.maxPlaceholder;
            if (!deviceControlControl.Hide)
            {
                maxPlace += deviceControlControl.maxPlaceholder;
            }
            if (!customControlControl.Hide)
            {
                maxPlace += customControlControl.maxPlaceholder;
            }
            if (!fontControlControl.Hide)
            {
                maxPlace += fontControlControl.maxPlaceholder;
            }
            float height = width * (maxPlace / 22f);
            nlcLC.Rect = new RectangleF(0, 0, width, height);
        }

        public void Init()
        {
            mainLayout = XmlUI.Instance.CreateUI(System.Environment.CurrentDirectory + @"\Xml\UI\UI_JoyControl.xml");
            //----
            JoyObjectSelect.Instance.Init();
            DeviceTypeSelect.Instance.Init();
            CustomTypeSelect.Instance.Init();
            FontTypeSelect.Instance.Init();
            JoyControlPanel.Instance.Init();
            CustomControlPanel.Instance.Init();
            DeviceControlControl.Instance.Init();
            FontLibraryControl.Instance.Init();
            //----
            ejoyControlMain = XmlUI.Instance.GetLayoutControl("EjoyControlMain");
            deviceTypeControl = XmlUI.Instance.GetLayoutControl("DeviceTypeControl");
            customTypeControl = XmlUI.Instance.GetLayoutControl("CustomTypeControl");
            fontTypeControl = XmlUI.Instance.GetLayoutControl("FontTypeControl");
            deviceControlControl = XmlUI.Instance.GetLayoutControl("DeviceControlControl");
            customControlControl = XmlUI.Instance.GetLayoutControl("CustomControlControl");
            fontControlControl = XmlUI.Instance.GetLayoutControl("FontLibraryControl");
            //----
            uiButton backBtn = XmlUI.Instance.GetButton("UI_JoyControl_Back");
            backBtn.LeftButtonClick += joyControlBack;
        }

        private void joyControlBack(object sender, EventArgs e)
        {
            PublicData.ui_Type = UIType.NodeLink;
        }
    }
}
