using SharpDX;
using System;
using System.Collections.Generic;

namespace EasyControl
{
    public class UI_Setting : iUiLogic
    {
        public LayoutControl mainLayout { get; private set; }
        //----        
        List<string> localizationList;               //本地化列表
        List<uiButton> locBtnList = new List<uiButton>();
        LayoutControl locListLC;
        LayoutControl setListLC;
        uiButton setBtn;
        uiButton locBtn;
        uiTrackBar customNodeCountTB;
        uiTrackBar autoSaveTimeTB;
        uiTrackBar autoSaveCountTB;
        uiSwitchButton autoSaveOnSB;
        uiSwitchButton autoLoginSB;
        uiSwitchButton debugModeSB;
        uiSwitchButton ledOnDefaultSB;
        public float maxHeight { get; } = 28f;
        //------------------------------------------------------------------------------------------------------
        public static readonly UI_Setting Instance = new UI_Setting();
        private UI_Setting()
        {
        }
        //============================================================
        public void DxRenderLogic()
        {
            LayoutControl lcControl = XmlUI.Instance.GetLayoutControl("SettingProperty");
            LayoutControl vcSettingPropertyLC = XmlUI.Instance.GetLayoutControl("vcSettingPropertyLC");
            //----
            setListLC.Hide = true;
            locListLC.Hide = true;
            setBtn.AlwaysOn = false;
            locBtn.AlwaysOn = false;
            float height = 0f;
            switch (PublicData.set_Type)
            {
                case SettingType.Settings:
                    setListLC.Hide = false;
                    height = maxHeight * 7;
                    setBtn.AlwaysOn = true;
                    break;
                case SettingType.Localization:
                    locListLC.Hide = false;
                    height = maxHeight * localizationList.Count;
                    locBtn.AlwaysOn = true;
                    break;
                    //do it
            }
            if (height < lcControl.DrawRect.Height)
                vcSettingPropertyLC.Rect = new RectangleF(0f, 0f, lcControl.DrawRect.Width, height);
            else
                vcSettingPropertyLC.Rect = new RectangleF(0f, 0f, lcControl.DrawRect.Width - ViewControl.sliderWidth, height);
            autoSaveOnSB.bSwitchOn = Localization.Instance.CheckAutoSaveOn();
            autoLoginSB.bSwitchOn = PublicData.AutoLogin;
            for (int i = 0; i < locBtnList.Count; i++)
            {
                if (i == Localization.Instance.CurrentIndex)
                    locBtnList[i].AlwaysOn = true;
                else
                    locBtnList[i].AlwaysOn = false;
            }
        }

        public void Init()
        {
            localizationList = Localization.Instance.Init();
            //----------------------------------------------------------------------------------------------------------------------------------
            mainLayout = XmlUI.Instance.CreateUI(System.Environment.CurrentDirectory + @"\Xml\UI\UI_Setting.xml");
            //----
            uiButton backBtn = XmlUI.Instance.GetButton("UI_Setting_Back");
            backBtn.LeftButtonClick += settingBack;
            #region ButtonList
            setBtn = XmlUI.Instance.GetButton("Settings");
            setBtn.LeftButtonClick += Settings;
            locBtn = XmlUI.Instance.GetButton("SetLocalization");
            locBtn.LeftButtonClick += SetLocalization;
            //do it
            #endregion
            LayoutControl vcSettingPropertyLC = XmlUI.Instance.GetLayoutControl("vcSettingPropertyLC");
            #region Settings
            setListLC = XmlUI.Instance.GetLayoutControl("SettingsList");
            customNodeCountTB = XmlUI.Instance.GetTrackBar("CustomNodeCountTB");
            customNodeCountTB.Value = Localization.Instance.GetCustomNodeCount(16);
            customNodeCountTB.ValueChange += CustomNodeCountChange;
            autoSaveTimeTB = XmlUI.Instance.GetTrackBar("AutoSaveTimeTB");
            autoSaveTimeTB.Value = Localization.Instance.GetAutoSaveTime(1);
            autoSaveTimeTB.ValueChange += AutoSaveTimeChange;
            autoSaveCountTB = XmlUI.Instance.GetTrackBar("AutoSaveCountTB");
            autoSaveCountTB.Value = Localization.Instance.GetAutoSaveCount(10);
            autoSaveCountTB.ValueChange += AutoSaveCountChange;
            ledOnDefaultSB = XmlUI.Instance.GetSwitchButton("LedOnDefaultSB");
            ledOnDefaultSB.bSwitchOn = Localization.Instance.GetLedOnDefault();
            ledOnDefaultSB.ValueChange += LedOnDefaultChange;
            debugModeSB = XmlUI.Instance.GetSwitchButton("DebugModeSB");
            debugModeSB.bSwitchOn = PublicData.Debug;
            debugModeSB.ValueChange += DebugModeChange;
            autoLoginSB = XmlUI.Instance.GetSwitchButton("AutoLoginSB");
            autoLoginSB.bSwitchOn = Localization.Instance.CheckAutoLogin();
            autoLoginSB.ValueChange += AutoLoginChange;
            autoSaveOnSB = XmlUI.Instance.GetSwitchButton("AutoSaveOnSB");
            autoSaveOnSB.bSwitchOn = Localization.Instance.CheckAutoSaveOn();
            autoSaveOnSB.ValueChange += AutoSaveOnChange;
            #endregion
            #region Localization
            locListLC = XmlUI.Instance.GetLayoutControl("LocalizationList");
            if (localizationList.Count > 0)
            {
                for (int i = 0; i < localizationList.Count; i++)
                {
                    LayoutControl lcNewLoc = XmlUI.Instance.CreateUI(System.Environment.CurrentDirectory + @"\Xml\LocalizationSelect.xml", i.ToString());
                    if (lcNewLoc != null)
                    {
                        locListLC.AddObject(lcNewLoc);
                        uiButton btn = XmlUI.Instance.GetButton(i + "LocalizationIndex");
                        if (btn != null)
                        {
                            btn.Index = i;
                            btn.Name = localizationList[i];
                            btn.LeftButtonClick += OnButtonClick;
                            if (i == Localization.Instance.CurrentIndex)
                                btn.AlwaysOn = true;
                        }
                        locBtnList.Add(btn);
                    }
                }
            }
            #endregion
        }
        private void Settings(object sender, EventArgs e)
        {
            PublicData.set_Type = SettingType.Settings;
        }
        private void CustomNodeCountChange(object sender, EventArgs e)
        {
            Localization.Instance.SetCustomNodeCount(customNodeCountTB.Value);
        }
        private void AutoSaveTimeChange(object sender, EventArgs e)
        {
            Localization.Instance.SetAutoSaveTime(autoSaveTimeTB.Value);
        }
        private void AutoSaveCountChange(object sender, EventArgs e)
        {
            Localization.Instance.SetAutoSaveCount(autoSaveCountTB.Value);
        }
        private void AutoLoginChange(object sender, EventArgs e)
        {
            PublicData.AutoLogin = autoLoginSB.bSwitchOn;
            Localization.Instance.SetAutoLogin(autoLoginSB.bSwitchOn);
        }
        private void AutoSaveOnChange(object sender, EventArgs e)
        {
            Localization.Instance.SetAutoSaveOn(autoLoginSB.bSwitchOn);
        }
        private void LedOnDefaultChange(object sender, EventArgs e)
        {
            Localization.Instance.SetLedOnDefault(ledOnDefaultSB.bSwitchOn);
        }
        private void DebugModeChange(object sender, EventArgs e)
        {
            Localization.Instance.SetDebug(debugModeSB.bSwitchOn);
        }
        private void SetLocalization(object sender, EventArgs e)
        {
            PublicData.set_Type = SettingType.Localization;
        }
        private void settingBack(object sender, EventArgs e)
        {
            PublicData.ui_Type = UIType.NodeLink;
        }
        private void OnButtonClick(object sender, EventArgs e)
        {
            JoyIndexChangeArgs args = (JoyIndexChangeArgs)e;
            Localization.Instance.CurrentIndex = args.Index;
        }
    }
}
