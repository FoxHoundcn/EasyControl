using SharpDX;
using SharpDX.DirectWrite;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;

namespace EasyControl
{
    public class MainUI : iControl
    {
        public bool NodeLinkMode { get; private set; }
        public iControl Parent { get { return null; } set { } }

        public RectangleF DrawRect { get { return new RectangleF(0, 0, Dx2D.Instance.Width, Dx2D.Instance.Height); } }

        public RectangleF Rect { set { } }
        public Vector2 Offset { get { return new Vector2(0, 0); } set { } }
        public bool Hide { get { return true; } set { } }

        public int Index { get { return -1; } }

        public string Name { get { return "MainUI"; } }

        public string UIKey { set { } }

        public string PluginID { get { return ""; } set { } }
        #region UI
        LayoutControl nodeLinkControl;
        LayoutControl currentUI = null;
        //-----
        LayoutControl nodeLinkTool;
        uiButton btnSaveSet;
        uiButton btnLoadSet;
        uiButton btnSavingMode;
        uiTextLable textFPS;
        uiPanel colorFPS;
        //----
        bool savingMode = false;
        uiImage background_Image;
        uiTextLable info;
        public Dictionary<LayoutControl, string> InfoList = new Dictionary<LayoutControl, string>();
        //---------------------------------------------------------------------------------------
        private MainUI()
        {
        }
        public static readonly MainUI Instance = new MainUI();
        #endregion
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void Init()
        {
            XmlUI.Instance.Init();
            //----
            UI_Login.Instance.Init();
            UI_NodeLink.Instance.Init();
            UI_JoyControl.Instance.Init();
            //UI_PluginControl.Instance.Init();//登录时加载
            UI_LAN.Instance.Init();
            UI_Setting.Instance.Init();
            //----
            currentUI = UI_Login.Instance.mainLayout;
            #region Tool
            nodeLinkTool = XmlUI.Instance.CreateUI(System.Environment.CurrentDirectory + @"\Xml\NodeLinkTool.xml");
            btnSaveSet = XmlUI.Instance.GetButton("SaveSet");
            btnSaveSet.LeftButtonClick += OnSaveSet;
            btnLoadSet = XmlUI.Instance.GetButton("LoadSet");
            btnLoadSet.LeftButtonClick += OnLoadSet;
            btnSavingMode = XmlUI.Instance.GetButton("SavingModeBtn");
            btnSavingMode.LeftButtonClick += OnSavingMode;
            textFPS = XmlUI.Instance.GetTextLable("FPS");
            colorFPS = XmlUI.Instance.GetPanel("ColorFPS");
            #region ScreneAlign
            uiButton btnScreneAlign = XmlUI.Instance.GetButton("ScreneAlign");
            btnScreneAlign.LeftButtonClick += OnScreneAlign;
            #endregion
            #region pluginDebug
            uiButton btnPluginDebug = XmlUI.Instance.GetButton("PluginDebug");
            btnPluginDebug.LeftButtonClick += OnPluginDebug;
            #endregion
            #endregion
            #region NodeLink
            nodeLinkControl = XmlUI.Instance.GetLayoutControl("NodeLinkControl");
            NodeLinkControl.Instance.Init();
            nodeLinkControl.AddObject(new LayoutControl(NodeLinkControl.Instance));
            #endregion
            #region textInfo
            info = new uiTextLable("Test", XmlUI.DxDeviceYellow, XmlUI.DxBackColor, JoyConst.FontSize, true, false);
            info.textAlignment = TextAlignment.Leading;
            info.AutoSize = true;
            #endregion
            #region imageBack
            background_Image = new uiImage("LOGO.png", "png", false);
            background_Image.AspectRatio = 1f;
            background_Image.Opacity = 0.1f;
            #endregion
            #region 弹出窗口
            SaveFileForm.Instance.Init();
            OpenFileForm.Instance.Init();
            WarningForm.Instance.Init();
            UpdateForm.Instance.Init();
            V3xUpdateForm.Instance.Init();
            #endregion
            Dx2DResize();
            UI_Login.Instance.LoginReady();
        }
        private void OnSaveSet(object sender, EventArgs e)
        {
            SaveFileForm.Instance.SaveEasyControl();
        }
        private void OnLoadSet(object sender, EventArgs e)
        {
            OpenFileForm.Instance.OpenEasyControl();
        }
        private void OnSavingMode(object sender, EventArgs e)
        {
            savingMode = !savingMode;
        }
        private void OnScreneAlign(object sender, EventArgs e)
        {
            NodeLinkControl.Instance.ScreneAlign();
        }
        private void OnPluginDebug(object sender, EventArgs e)
        {
            PublicData.PluginDebug = !PublicData.PluginDebug;
        }
        public void Dx2DResize()
        {
            UI_NodeLink.Instance.mainLayout.Dx2DResize();
            currentUI.Dx2DResize();
            background_Image.Dx2DResize();
            info.Dx2DResize();
            nodeLinkTool.Dx2DResize();
            //MainSwitch
            MainSwitch.Instance.Dx2DResize();
            #region Dialog
            SaveFileForm.Instance.Dx2DResize();
            OpenFileForm.Instance.Dx2DResize();
            WarningForm.Instance.Dx2DResize();
            UpdateForm.Instance.Dx2DResize();
            V3xUpdateForm.Instance.Dx2DResize();
            #endregion
        }
        #region 鼠标
        public void JoyMouseMoveEvent(MouseEventArgs e)
        {
            #region Dialog
            SaveFileForm.Instance.JoyMouseMoveEvent(e);
            OpenFileForm.Instance.JoyMouseMoveEvent(e);
            WarningForm.Instance.JoyMouseMoveEvent(e);
            UpdateForm.Instance.JoyMouseMoveEvent(e);
            V3xUpdateForm.Instance.JoyMouseMoveEvent(e);
            #endregion
            if (SaveFileForm.Instance.Hide &&
                OpenFileForm.Instance.Hide &&
                WarningForm.Instance.Hide &&
                UpdateForm.Instance.Hide &&
                V3xUpdateForm.Instance.Hide)
            {
                if (!currentUI.Hide && e.X < JoyConst.WindowsWidth)
                    currentUI.JoyMouseMoveEvent(e);
                else
                {
                    UI_NodeLink.Instance.mainLayout.JoyMouseMoveEvent(e);
                    nodeLinkTool.JoyMouseMoveEvent(e);
                }
                //MainSwitch
                MainSwitch.Instance.JoyMouseMoveEvent(e);
            }
        }
        public void JoyMouseDownEvent(MouseEventArgs e)
        {
            #region Dialog
            SaveFileForm.Instance.JoyMouseDownEvent(e);
            OpenFileForm.Instance.JoyMouseDownEvent(e);
            WarningForm.Instance.JoyMouseDownEvent(e);
            UpdateForm.Instance.JoyMouseDownEvent(e);
            V3xUpdateForm.Instance.JoyMouseDownEvent(e);
            #endregion
            if (SaveFileForm.Instance.Hide &&
                OpenFileForm.Instance.Hide &&
                WarningForm.Instance.Hide &&
                UpdateForm.Instance.Hide &&
                V3xUpdateForm.Instance.Hide)
            {
                if (!currentUI.Hide && e.X < JoyConst.WindowsWidth)
                    currentUI.JoyMouseDownEvent(e);
                else
                {
                    UI_NodeLink.Instance.mainLayout.JoyMouseDownEvent(e);
                    nodeLinkTool.JoyMouseDownEvent(e);
                }
                //MainSwitch
                MainSwitch.Instance.JoyMouseDownEvent(e);
            }
        }
        public void JoyMouseUpEvent(MouseEventArgs e)
        {
            #region Dialog
            SaveFileForm.Instance.JoyMouseUpEvent(e);
            OpenFileForm.Instance.JoyMouseUpEvent(e);
            WarningForm.Instance.JoyMouseUpEvent(e);
            UpdateForm.Instance.JoyMouseUpEvent(e);
            V3xUpdateForm.Instance.JoyMouseUpEvent(e);
            #endregion
            if (SaveFileForm.Instance.Hide &&
                OpenFileForm.Instance.Hide &&
                WarningForm.Instance.Hide &&
                UpdateForm.Instance.Hide &&
                V3xUpdateForm.Instance.Hide)
            {
                if (!currentUI.Hide && e.X < JoyConst.WindowsWidth)
                    currentUI.JoyMouseUpEvent(e);
                else
                {
                    UI_NodeLink.Instance.mainLayout.JoyMouseUpEvent(e);
                    nodeLinkTool.JoyMouseUpEvent(e);
                }
                //MainSwitch
                MainSwitch.Instance.JoyMouseUpEvent(e);
            }
        }
        public void JoyMouseMoveWheel(MouseEventArgs e)
        {
            #region Dialog
            SaveFileForm.Instance.JoyMouseMoveWheel(e);
            OpenFileForm.Instance.JoyMouseMoveWheel(e);
            WarningForm.Instance.JoyMouseMoveWheel(e);
            UpdateForm.Instance.JoyMouseMoveWheel(e);
            V3xUpdateForm.Instance.JoyMouseMoveWheel(e);
            #endregion
            if (SaveFileForm.Instance.Hide &&
                OpenFileForm.Instance.Hide &&
                WarningForm.Instance.Hide &&
                UpdateForm.Instance.Hide &&
                V3xUpdateForm.Instance.Hide)
            {
                if (!currentUI.Hide && e.X < JoyConst.WindowsWidth)
                    currentUI.JoyMouseMoveWheel(e);
                else
                {
                    UI_NodeLink.Instance.mainLayout.JoyMouseMoveWheel(e);
                    nodeLinkTool.JoyMouseMoveWheel(e);
                }
                //MainSwitch
                MainSwitch.Instance.JoyMouseMoveWheel(e);
            }
        }
        #endregion
        public void DxRenderLogic()
        {
            #region Update
            if (PublicData.checkUpdateTime < byte.MaxValue)
            {
                PublicData.checkUpdateTime++;
            }
            else
            {
                PublicData.checkUpdateTime = 0;
                UpdateUSB.Instance.Refresh();
            }
            #endregion
            JoyObject joyObj = PublicData.GetCurrentSelectJoyObject();
            if (joyObj == null)
            {
                background_Image.Hide = false;
            }
            else
            {
                background_Image.Hide = true;
            }
            #region Info
            List<LayoutControl> remove = new List<LayoutControl>();
            foreach (var info in InfoList.Keys)
            {
                if (!info.InSide(PublicData.MouseX, PublicData.MouseY))
                {
                    remove.Add(info);
                }
            }
            for (int i = 0; i < remove.Count; i++)
            {
                InfoList.Remove(remove[i]);
            }
            string infoOld = info.Text;
            string infoText = "";
            bool start = true;
            foreach (string info in InfoList.Values)
            {
                if (!info.Equals(""))
                {
                    if (!start)
                        infoText += "\n";
                    infoText += "  " + info;
                    if (start)
                        start = false;
                }
            }
            info.Text = infoText;
            if (infoOld != infoText)
            {
                info.Hide = true;
            }
            else
            {
                if (info.Text.Trim().Length != 0)
                {
                    info.Hide = false;
                }
            }
            if (!info.Hide)
            {
                RectangleF infoRect = new RectangleF(PublicData.MouseX + 15f, PublicData.MouseY + 15f, info.DrawRect.Width, info.DrawRect.Height);
                if (infoRect.X + infoRect.Width > Dx2D.Instance.Width)
                    infoRect.X = Dx2D.Instance.Width - infoRect.Width;
                if (infoRect.Y + infoRect.Height > Dx2D.Instance.Height)
                    infoRect.Y = Dx2D.Instance.Height - infoRect.Height;
                info.Rect = infoRect;
                info.Dx2DResize();
                info.DxRenderLogic();
            }
            #endregion
            #region nodeLinkTool
            nodeLinkTool.Rect = new RectangleF(Dx2D.Instance.Width - JoyConst.MaxNodeToolWidth, 4, JoyConst.MaxNodeToolWidth - 4, JoyConst.MaxNodeToolHeight - 4);
            nodeLinkTool.DxRenderLogic();
            btnSavingMode.ForeColor = savingMode ? XmlUI.DxDeviceGreen : XmlUI.DxDeviceYellow;
            textFPS.Text = Dx2D.showFps.ToString();
            if (Dx2D.showFps > 50)
                colorFPS.ForeColor = XmlUI.DxDeviceGreen;
            else if (Dx2D.showFps > 30)
                colorFPS.ForeColor = XmlUI.DxDeviceYellow;
            else
                colorFPS.ForeColor = XmlUI.DxDeviceRed;
            #endregion
            #region Dialog
            SaveFileForm.Instance.DxRenderLogic();
            OpenFileForm.Instance.DxRenderLogic();
            WarningForm.Instance.DxRenderLogic();
            UpdateForm.Instance.DxRenderLogic();
            V3xUpdateForm.Instance.DxRenderLogic();
            #endregion
            #region NodeLinkLogic 保证一直运行
            NodeLinkControl.Instance.NodeLinkLogic();
            UI_PluginControl.Instance.Update();
            #endregion
            #region UI_Type
            UI_Login.Instance.mainLayout.Hide = true;
            UI_NodeLink.Instance.mainLayout.Hide = false;
            UI_JoyControl.Instance.mainLayout.Hide = true;
            if (UI_PluginControl.Instance.ready)
                UI_PluginControl.Instance.mainLayout.Hide = true;
            UI_Setting.Instance.mainLayout.Hide = true;
            //MainSwitch
            MainSwitch.Instance.Hide = false;
            switch (PublicData.ui_Type)
            {
                case UIType.NodeLink:
                    currentUI.Hide = true;
                    UI_NodeLink.Instance.DxRenderLogic();
                    break;
                case UIType.PluginControl:
                    if (UI_PluginControl.Instance.ready)
                    {
                        UI_PluginControl.Instance.mainLayout.Hide = false;
                        currentUI = UI_PluginControl.Instance.mainLayout;
                        UI_PluginControl.Instance.DxRenderLogic();
                    }
                    break;
                case UIType.JoyControl:
                    UI_JoyControl.Instance.mainLayout.Hide = false;
                    currentUI = UI_JoyControl.Instance.mainLayout;
                    UI_JoyControl.Instance.DxRenderLogic();
                    break;
                case UIType.LAN:
                    UI_LAN.Instance.mainLayout.Hide = false;
                    currentUI = UI_LAN.Instance.mainLayout;
                    UI_LAN.Instance.DxRenderLogic();
                    break;
                case UIType.Setting:
                    UI_Setting.Instance.mainLayout.Hide = false;
                    currentUI = UI_Setting.Instance.mainLayout;
                    UI_Setting.Instance.DxRenderLogic();
                    break;
                default:
                    throw new Win32Exception("PublicData.ui_Type Error !!! " + PublicData.ui_Type.ToString());
            }
            #endregion
            if (!background_Image.Hide)
            {
                background_Image.Rect = new RectangleF(0, 0, Dx2D.Instance.Width, Dx2D.Instance.Height);
                background_Image.DxRenderLogic();
            }
            if (!UI_NodeLink.Instance.mainLayout.Hide)
            {
                UI_NodeLink.Instance.mainLayout.Rect = new RectangleF(0, 0, Dx2D.Instance.Width, Dx2D.Instance.Height);
                UI_NodeLink.Instance.mainLayout.DxRenderLogic();
            }
            if (!currentUI.Hide)
            {
                currentUI.Rect = new RectangleF(0, 0, JoyConst.WindowsWidth, Dx2D.Instance.Height);
                currentUI.DxRenderLogic();
            }
            if (!MainSwitch.Instance.Hide)
            {
                MainSwitch.Instance.Rect = new RectangleF(0, 0, Dx2D.Instance.Width, Dx2D.Instance.Height);
                MainSwitch.Instance.DxRenderLogic();
            }
        }

        #region Render
        public void DxRenderHigh()
        {
            if (!UI_NodeLink.Instance.mainLayout.Hide)
            {
                if (!savingMode)
                    UI_NodeLink.Instance.mainLayout.DxRenderHigh();
            }
            if (!currentUI.Hide)
                currentUI.DxRenderHigh();
            #region 鼠标悬停提示
            if (!info.Hide)
                info.DxRenderMedium();
            #endregion
            //MainSwitch
            MainSwitch.Instance.DxRenderHigh();
            #region Dialog
            SaveFileForm.Instance.DxRenderHigh();
            OpenFileForm.Instance.DxRenderHigh();
            WarningForm.Instance.DxRenderHigh();
            UpdateForm.Instance.DxRenderHigh();
            V3xUpdateForm.Instance.DxRenderHigh();
            #endregion
        }

        public void DxRenderMedium()
        {
            if (!UI_NodeLink.Instance.mainLayout.Hide)
            {
                if (savingMode)
                {
                    Dx2D.Instance.RenderTarget2D.DrawLine(new Vector2(0, 0), new Vector2(Dx2D.Instance.Width, Dx2D.Instance.Height),
                        Dx2D.Instance.GetSolidColorBrush(XmlUI.DxDeviceYellow), 4);
                    Dx2D.Instance.RenderTarget2D.DrawLine(new Vector2(0, Dx2D.Instance.Height), new Vector2(Dx2D.Instance.Width, 0),
                        Dx2D.Instance.GetSolidColorBrush(XmlUI.DxDeviceYellow), 4);
                }
                else
                    UI_NodeLink.Instance.mainLayout.DxRenderMedium();
                //saveModeButton
                nodeLinkTool.DxRenderMedium();
            }
            if (!currentUI.Hide)
                currentUI.DxRenderMedium();
            //MainSwitch
            MainSwitch.Instance.DxRenderMedium();
            #region Dialog
            SaveFileForm.Instance.DxRenderMedium();
            OpenFileForm.Instance.DxRenderMedium();
            WarningForm.Instance.DxRenderMedium();
            UpdateForm.Instance.DxRenderMedium();
            V3xUpdateForm.Instance.DxRenderMedium();
            #endregion
        }

        public void DxRenderLow()
        {
            #region 背景图
            if (!background_Image.Hide)
                background_Image.DxRenderMedium();
            #endregion
            if (!UI_NodeLink.Instance.mainLayout.Hide)
            {
                if (!savingMode)
                    UI_NodeLink.Instance.mainLayout.DxRenderLow();
            }
            if (!currentUI.Hide)
                currentUI.DxRenderLow();
            //MainSwitch
            MainSwitch.Instance.DxRenderLow();
            #region Dialog
            SaveFileForm.Instance.DxRenderLow();
            OpenFileForm.Instance.DxRenderLow();
            WarningForm.Instance.DxRenderLow();
            UpdateForm.Instance.DxRenderLow();
            V3xUpdateForm.Instance.DxRenderLow();
            #endregion
        }
        #endregion
    }
}
