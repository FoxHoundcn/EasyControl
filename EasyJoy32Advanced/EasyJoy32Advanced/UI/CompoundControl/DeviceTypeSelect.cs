using SharpDX;
using System;
using System.Collections.Generic;

namespace EasyControl
{
    public class DeviceTypeSelect : iUiLogic
    {
        //---------------------------------------------------------------------------------------
        LayoutControl selectTypeControl;
        LayoutControl deviceTypeLC;
        uiButton oldBtn = null;
        float lcHeight = 0;
        ////////////////////////////////////////////////////////////////////////////////////
        public static readonly DeviceTypeSelect Instance = new DeviceTypeSelect();
        private DeviceTypeSelect()
        {
        }
        public void Init()
        {
            selectTypeControl = XmlUI.Instance.GetLayoutControl("SelectTypeControl");
            deviceTypeLC = XmlUI.Instance.GetLayoutControl("vcDeviceTypeLC");
            int index = 0;
            foreach (DeviceType suit in Enum.GetValues(typeof(DeviceType)))
            {
                LayoutControl newType = XmlUI.Instance.CreateUI(System.Environment.CurrentDirectory + @"\Xml\DeviceType.xml", suit.ToString());
                deviceTypeLC.AddObject(newType);
                LayoutControl lcDevice = XmlUI.Instance.GetLayoutControl(suit.ToString() + "DeviceTypeLC");
                lcHeight = lcDevice.maxHeight;
                uiButton btn = XmlUI.Instance.GetButton(suit.ToString() + "DeviceTypeIndex");
                if (btn != null)
                {
                    btn.Index = index;
                    btn.Name = suit.ToString();
                    btn.LeftButtonClick += OnTypeChangeClick;
                    if (suit == DeviceType.None)
                    {
                        btn.textAlignment = SharpDX.DirectWrite.TextAlignment.Center;
                    }
                    else
                    {
                        btn.textAlignment = SharpDX.DirectWrite.TextAlignment.Leading;
                    }
                }
                uiPanel pan = XmlUI.Instance.GetPanel(suit.ToString() + "DeviceModeColor");
                pan.ForeColor = PublicData.GetDeviceTypeColor(suit);
                index++;
            }
        }
        private void OnTypeChangeClick(object sender, EventArgs e)
        {
            JoyIndexChangeArgs args = (JoyIndexChangeArgs)e;
            JoyObject joyObj = PublicData.GetCurrentSelectJoyObject();
            if (joyObj != null)
            {
                JoyDevice joyDev = joyObj.GetCurrentJoyDevice();
                if (joyDev != null)
                {
                    joyDev.Type = (DeviceType)args.Index;
                }
            }
        }
        public void DxRenderLogic()
        {
            JoyObject joyObj = PublicData.GetCurrentSelectJoyObject();
            if (joyObj != null)
            {
                if (joyObj.TypeSwitchControl == TypeSwitch.DeviceSwitch)
                {
                    #region 开关
                    Dictionary<DeviceType, bool> enableList = joyObj.GetDeviceEnable();
                    foreach (DeviceType suit in Enum.GetValues(typeof(DeviceType)))
                    {
                        uiButton btn = XmlUI.Instance.GetButton(suit.ToString() + "DeviceTypeIndex");
                        if (btn != null)
                        {
                            if (enableList != null && enableList.ContainsKey(suit))
                            {
                                btn.Enable = true;
                            }
                            else
                            {
                                btn.Enable = false;
                            }
                        }
                    }
                    #endregion
                    if (oldBtn != null)
                        oldBtn.BackColor = XmlUI.DxUIBackColor;
                    JoyDevice currentDev = joyObj.GetCurrentJoyDevice();
                    if (currentDev != null)
                    {
                        uiButton btn = XmlUI.Instance.GetButton(currentDev.Type.ToString() + "DeviceTypeIndex");
                        if (btn != null)
                        {
                            btn.BackColor = XmlUI.DxDeviceBlue;
                            oldBtn = btn;
                        }
                    }
                    float height = lcHeight * Enum.GetValues(typeof(DeviceType)).Length;
                    if (height < deviceTypeLC.DrawRect.Height)
                        deviceTypeLC.Rect = new RectangleF(0f, 0f, selectTypeControl.DrawRect.Width, height);
                    else
                        deviceTypeLC.Rect = new RectangleF(0f, 0f, selectTypeControl.DrawRect.Width - ViewControl.sliderWidth, height);
                }
            }
        }
    }
}
