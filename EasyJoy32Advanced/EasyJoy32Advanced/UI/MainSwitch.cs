using SharpDX;
using SharpDX.DirectWrite;
using System.Windows.Forms;

namespace EasyControl
{
    public class MainSwitch : iControl
    {
        public iControl Parent { get; set; }
        public RectangleF DrawRect { get { return Rect; } }
        public RectangleF Rect { private get; set; }
        public Vector2 Offset { get; set; }
        public bool Hide { get; set; } = false;
        public int Index { get; set; }
        public string Name { get; set; }
        public string PluginID { get; set; }
        public string UIKey { set; private get; }
        public bool NodeLinkMode { get; private set; }
        //---------------------------------------------------------------------------------
        UIType selectBtn = UIType.None;
        RectangleF rectJoyControl;
        RectangleF rectPluginControl;
        RectangleF rectLAN;
        RectangleF rectSetting;
        //----------------------------------------------------------------------------------
        string textJoyControl;
        TextFormat tfJoyControl;
        string textPluginControl;
        TextFormat tfPluginControl;
        string textLAN;
        TextFormat tfLAN;
        string textSetting;
        TextFormat tfSetting;
        /////////////////////////////////////////////////////////////////////////////////////
        public static readonly MainSwitch Instance = new MainSwitch();
        private MainSwitch()
        {
        }
        //---------------------------------------------------------------------------------
        public void Dx2DResize()
        {
            if (Hide) return;
            textPluginControl = Dx2D.Instance.GetDrawText(Localization.Instance.GetLS("UI_PluginControl"),
                new RectangleF(0, JoyConst.MainSwitchBtnTipHeigh + 2, JoyConst.MainSwitchBtnExWidth - 8, JoyConst.MainSwitchBtnHeigh - 4 - JoyConst.MainSwitchBtnTipHeigh),
               ref tfPluginControl, TextAlignment.Center);
            //------------------------------------------------------
            textJoyControl = Dx2D.Instance.GetDrawText(Localization.Instance.GetLS("UI_JoyControl"),
                new RectangleF(0, JoyConst.MainSwitchBtnTipHeigh + 2, JoyConst.MainSwitchBtnExWidth - 8, JoyConst.MainSwitchBtnHeigh - 4 - JoyConst.MainSwitchBtnTipHeigh),
               ref tfJoyControl, TextAlignment.Center);
            //------------------------------------------------------
            textLAN = Dx2D.Instance.GetDrawText(Localization.Instance.GetLS("UI_LAN"),
                new RectangleF(0, JoyConst.MainSwitchBtnTipHeigh + 2, JoyConst.MainSwitchBtnExWidth - 8, JoyConst.MainSwitchBtnHeigh - 4 - JoyConst.MainSwitchBtnTipHeigh),
               ref tfLAN, TextAlignment.Center);
            //------------------------------------------------------
            textSetting = Dx2D.Instance.GetDrawText(Localization.Instance.GetLS("UI_Setting"),
                new RectangleF(0, JoyConst.MainSwitchBtnTipHeigh + 2, JoyConst.MainSwitchBtnExWidth - 8, JoyConst.MainSwitchBtnHeigh - 4 - JoyConst.MainSwitchBtnTipHeigh),
               ref tfSetting, TextAlignment.Center);
        }
        public void DxRenderLogic()
        {
            if (Hide) return;
        }
        #region Render
        private void DrawSelectExBtn(UIType type, RectangleF rect, Color4 color)
        {
            Dx2D.Instance.RenderTarget2D.FillRectangle(rect, Dx2D.Instance.GetSolidColorBrush(XmlUI.DxUIBackColor));
            Dx2D.Instance.RenderTarget2D.DrawRectangle(rect, Dx2D.Instance.GetSolidColorBrush(XmlUI.DxUIClickColor), 2);
            RectangleF rectTip = new RectangleF(rect.X + 2, rect.Y + 2, rect.Width - 4, JoyConst.MainSwitchBtnTipHeigh);
            Dx2D.Instance.RenderTarget2D.FillRectangle(rectTip, Dx2D.Instance.GetSolidColorBrush(color));
            RectangleF rectText = new RectangleF(rect.X + 4, JoyConst.MainSwitchBtnTipHeigh + 2 + rect.Y,
                JoyConst.MainSwitchBtnExWidth - 8, JoyConst.MainSwitchBtnHeigh - 4 - JoyConst.MainSwitchBtnTipHeigh);
            switch (type)
            {
                case UIType.PluginControl:
                    Dx2D.Instance.RenderTarget2D.DrawText(textPluginControl, tfPluginControl, rectText, Dx2D.Instance.GetSolidColorBrush(XmlUI.DxTextColor));
                    break;
                case UIType.JoyControl:
                    Dx2D.Instance.RenderTarget2D.DrawText(textJoyControl, tfJoyControl, rectText, Dx2D.Instance.GetSolidColorBrush(XmlUI.DxTextColor));
                    break;
                case UIType.LAN:
                    Dx2D.Instance.RenderTarget2D.DrawText(textLAN, tfLAN, rectText, Dx2D.Instance.GetSolidColorBrush(XmlUI.DxTextColor));
                    break;
                case UIType.Setting:
                    Dx2D.Instance.RenderTarget2D.DrawText(textSetting, tfSetting, rectText, Dx2D.Instance.GetSolidColorBrush(XmlUI.DxTextColor));
                    break;
            }
        }
        private void DrawSelectBtn(RectangleF rect, Color4 color)
        {
            Dx2D.Instance.RenderTarget2D.DrawRectangle(rect, Dx2D.Instance.GetSolidColorBrush(XmlUI.DxUIBackColor), 2);
            Dx2D.Instance.RenderTarget2D.FillRectangle(PublicData.GetActualRange(rect, 2), Dx2D.Instance.GetSolidColorBrush(color));
        }
        public void DxRenderHigh()
        {
            if (Hide) return;
            RectangleF topFill = new RectangleF(2, 2, JoyConst.MainSwitchBtnWidth, JoyConst.MainSwitchBtnWidth);
            Dx2D.Instance.RenderTarget2D.FillRectangle(topFill, Dx2D.Instance.GetSolidColorBrush(XmlUI.DxDeviceRed));
            #region PluginControl
            if (selectBtn == UIType.PluginControl)
            {
                rectPluginControl = new RectangleF(2, 2 + JoyConst.MainSwitchBtnWidth, JoyConst.MainSwitchBtnExWidth, JoyConst.MainSwitchBtnHeigh - 4);
                DrawSelectExBtn(selectBtn, rectPluginControl, XmlUI.DxDeviceBlue);
            }
            else
            {
                rectPluginControl = new RectangleF(2, 2 + JoyConst.MainSwitchBtnWidth, JoyConst.MainSwitchBtnWidth, JoyConst.MainSwitchBtnHeigh - 4);
                DrawSelectBtn(rectPluginControl, XmlUI.DxDeviceBlue);
            }
            #endregion
            #region JoyControl
            if (selectBtn == UIType.JoyControl)
            {
                rectJoyControl = new RectangleF(2, JoyConst.MainSwitchBtnHeigh + 2 + JoyConst.MainSwitchBtnWidth, JoyConst.MainSwitchBtnExWidth, JoyConst.MainSwitchBtnHeigh - 4);
                DrawSelectExBtn(selectBtn, rectJoyControl, XmlUI.DxDeviceGreen);
            }
            else
            {
                rectJoyControl = new RectangleF(2, JoyConst.MainSwitchBtnHeigh + 2 + JoyConst.MainSwitchBtnWidth, JoyConst.MainSwitchBtnWidth, JoyConst.MainSwitchBtnHeigh - 4);
                DrawSelectBtn(rectJoyControl, XmlUI.DxDeviceGreen);
            }
            #endregion
            #region LAN
            if (selectBtn == UIType.LAN)
            {
                rectLAN = new RectangleF(2, JoyConst.MainSwitchBtnHeigh * 2 + 2 + JoyConst.MainSwitchBtnWidth, JoyConst.MainSwitchBtnExWidth, JoyConst.MainSwitchBtnHeigh - 4);
                DrawSelectExBtn(selectBtn, rectLAN, XmlUI.DxDeviceYellow);
            }
            else
            {
                rectLAN = new RectangleF(2, JoyConst.MainSwitchBtnHeigh * 2 + 2 + JoyConst.MainSwitchBtnWidth, JoyConst.MainSwitchBtnWidth, JoyConst.MainSwitchBtnHeigh - 4);
                DrawSelectBtn(rectLAN, XmlUI.DxDeviceYellow);
            }
            #endregion
            #region Setting
            if (selectBtn == UIType.Setting)
            {
                rectSetting = new RectangleF(2, JoyConst.MainSwitchBtnHeigh * 3 + 2 + JoyConst.MainSwitchBtnWidth, JoyConst.MainSwitchBtnExWidth, JoyConst.MainSwitchBtnHeigh - 4);
                DrawSelectExBtn(selectBtn, rectSetting, XmlUI.DxDevicePurple);
            }
            else
            {
                rectSetting = new RectangleF(2, JoyConst.MainSwitchBtnHeigh * 3 + 2 + JoyConst.MainSwitchBtnWidth, JoyConst.MainSwitchBtnWidth, JoyConst.MainSwitchBtnHeigh - 4);
                DrawSelectBtn(rectSetting, XmlUI.DxDevicePurple);
            }
            #endregion
        }
        public void DxRenderMedium()
        {
            if (Hide) return;
        }
        public void DxRenderLow()
        {
            if (Hide) return;
        }
        #endregion
        #region MouseEvent
        public void JoyMouseDownEvent(MouseEventArgs e)
        {
            if (Hide) return;
            if (PublicData.InSide(e.X, e.Y, rectPluginControl))
            {
                PublicData.ui_Type = UIType.PluginControl;
            }
            if (PublicData.InSide(e.X, e.Y, rectJoyControl))
            {
                PublicData.ui_Type = UIType.JoyControl;
            }
            if (PublicData.InSide(e.X, e.Y, rectLAN))
            {
                PublicData.ui_Type = UIType.LAN;
            }
            if (PublicData.InSide(e.X, e.Y, rectSetting))
            {
                PublicData.ui_Type = UIType.Setting;
            }
        }
        public void JoyMouseMoveEvent(MouseEventArgs e)
        {
            if (Hide) return;
            selectBtn = UIType.None;
            if (PublicData.InSide(e.X, e.Y, rectPluginControl))
            {
                selectBtn = UIType.PluginControl;
            }
            if (PublicData.InSide(e.X, e.Y, rectJoyControl))
            {
                selectBtn = UIType.JoyControl;
            }
            if (PublicData.InSide(e.X, e.Y, rectLAN))
            {
                selectBtn = UIType.LAN;
            }
            if (PublicData.InSide(e.X, e.Y, rectSetting))
            {
                selectBtn = UIType.Setting;
            }
        }
        public void JoyMouseMoveWheel(MouseEventArgs e)
        {
            if (Hide) return;
        }
        public void JoyMouseUpEvent(MouseEventArgs e)
        {
            if (Hide) return;
        }
        #endregion
    }
}
