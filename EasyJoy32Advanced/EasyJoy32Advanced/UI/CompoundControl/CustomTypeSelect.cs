using SharpDX;
using System;
using System.Collections.Generic;

namespace EasyControl
{
    public class CustomTypeSelect : iUiLogic
    {
        //---------------------------------------------------------------------------------------
        LayoutControl selectTypeControl;
        LayoutControl customTypeLC;
        uiButton oldBtn = null;
        float lcHeight = 0;
        ////////////////////////////////////////////////////////////////////////////////////
        public static readonly CustomTypeSelect Instance = new CustomTypeSelect();
        private CustomTypeSelect()
        {
        }
        public void Init()
        {
            selectTypeControl = XmlUI.Instance.GetLayoutControl("SelectTypeControl");
            customTypeLC = XmlUI.Instance.GetLayoutControl("vcCustomTypeLC");
            int index = 0;
            foreach (CustomType suit in Enum.GetValues(typeof(CustomType)))
            {
                LayoutControl newType = XmlUI.Instance.CreateUI(System.Environment.CurrentDirectory + @"\Xml\CustomType.xml", suit.ToString());
                customTypeLC.AddObject(newType);
                LayoutControl lcCustom = XmlUI.Instance.GetLayoutControl(suit.ToString() + "CustomTypeLC");
                lcHeight = lcCustom.maxHeight;
                uiButton btn = XmlUI.Instance.GetButton(suit.ToString() + "CustomTypeIndex");
                if (btn != null)
                {
                    btn.Index = index;
                    btn.Name = suit.ToString();
                    btn.LeftButtonClick += OnTypeChangeClick;
                    if (suit == CustomType.NoneCustom)
                    {
                        btn.textAlignment = SharpDX.DirectWrite.TextAlignment.Center;
                    }
                    else
                    {
                        btn.textAlignment = SharpDX.DirectWrite.TextAlignment.Leading;
                    }
                }
                uiPanel pan = XmlUI.Instance.GetPanel(suit.ToString() + "CustomModeColor");
                pan.ForeColor = PublicData.GetCustomTypeColor(suit);
                index++;
            }
        }
        private void OnTypeChangeClick(object sender, EventArgs e)
        {
            JoyIndexChangeArgs args = (JoyIndexChangeArgs)e;
            JoyObject joyObj = PublicData.GetCurrentSelectJoyObject();
            if (joyObj != null)
            {
                JoyCustom joyDev = joyObj.GetCurrentJoyCustom();
                if (joyDev != null)
                {
                    joyDev.Type = (CustomType)args.Index;
                }
            }
        }
        public void DxRenderLogic()
        {
            JoyObject joyObj = PublicData.GetCurrentSelectJoyObject();
            if (joyObj != null)
            {
                if (joyObj.TypeSwitchControl == TypeSwitch.CustomSwitch)
                {
                    #region 开关
                    Dictionary<CustomType, bool> enableList = joyObj.GetCustomEnable();
                    foreach (CustomType suit in Enum.GetValues(typeof(CustomType)))
                    {
                        uiButton btn = XmlUI.Instance.GetButton(suit.ToString() + "CustomTypeIndex");
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
                    JoyCustom currentCus = joyObj.GetCurrentJoyCustom();
                    if (currentCus != null)
                    {
                        uiButton btn = XmlUI.Instance.GetButton(currentCus.Type.ToString() + "CustomTypeIndex");
                        if (btn != null)
                        {
                            btn.BackColor = XmlUI.DxDeviceBlue;
                            oldBtn = btn;
                        }
                    }
                    float height = lcHeight * Enum.GetValues(typeof(CustomType)).Length;
                    if (height < customTypeLC.DrawRect.Height)
                        customTypeLC.Rect = new RectangleF(0f, 0f, selectTypeControl.DrawRect.Width, height);
                    else
                        customTypeLC.Rect = new RectangleF(0f, 0f, selectTypeControl.DrawRect.Width - ViewControl.sliderWidth, height);
                }
            }
        }
    }
}
