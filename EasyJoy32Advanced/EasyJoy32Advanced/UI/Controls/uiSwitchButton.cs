using SharpDX;
using SharpDX.Direct2D1;
using SharpDX.DirectWrite;
using System;
using System.Windows.Forms;

namespace EasyControl
{
    public class uiSwitchButton : iControl
    {
        static int maxIndex = 0;
        //--------------------------------------------------------------
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
        //-------------------------------------------------------------------------------
        public bool mouseEnter { get; private set; } = false;
        public bool mouseDown { get; private set; } = false;
        private bool bRadioStyle;
        public bool bLocalization { get; private set; } = true;
        #region 事件
        public bool bSwitchOn = false;
        public EventHandler ValueChange;
        #endregion
        RoundedRectangle rr = new RoundedRectangle();
        RectangleF rrText = new RectangleF();
        string drawText;
        TextFormat tf;
        int _edgeWidth;
        int edgeWidth
        {
            get { return _edgeWidth; }
            set
            {
                _edgeWidth = value;
                if (_edgeWidth < 1)
                    _edgeWidth = 1;
            }
        }
        //////////////////////////////////////////////////////////////////////////////////
        public uiSwitchButton(string _text, bool _style, bool _loc)
        {
            Index = maxIndex;
            maxIndex++;
            Name = _text;
            bRadioStyle = _style;
            Rect = new RectangleF();
            Hide = false;
            bLocalization = _loc;
            //-------------------------------------------
            Dx2DResize();
            ValueChange += doNothing;
        }
        public void Dx2DResize()
        {
            if (Hide) return;
            drawText = Dx2D.Instance.GetDrawText(bLocalization ? Localization.Instance.GetLS(Name) : Name, rrText, ref tf);
        }
        #region 鼠标
        public void JoyMouseMoveEvent(MouseEventArgs e)
        {
            if (Hide)
            {
                mouseEnter = false;
                return;
            }
            if (e.X >= rr.Rect.X + Offset.X && e.X < rr.Rect.X + Offset.X + rr.Rect.Width &&
                e.Y >= rr.Rect.Y + Offset.Y && e.Y < rr.Rect.Y + Offset.Y + rr.Rect.Height)
            {
                mouseEnter = true;
            }
            else
            {
                mouseEnter = false;
            }
        }
        public void JoyMouseDownEvent(MouseEventArgs e)
        {
            if (Hide)
            {
                return;
            }
            if (e.X >= rr.Rect.X + Offset.X && e.X < rr.Rect.X + Offset.X + rr.Rect.Width &&
                e.Y >= rr.Rect.Y + Offset.Y && e.Y < rr.Rect.Y + Offset.Y + rr.Rect.Height)
            {
                mouseDown = true;
            }
        }
        public void JoyMouseUpEvent(MouseEventArgs e)
        {
            if (Hide)
            {
                mouseEnter = false;
                return;
            }
            if (mouseDown)
            {
                mouseDown = false;
                bSwitchOn = !bSwitchOn;
                TriggerValueChange();
            }
        }
        public void TriggerValueChange()
        {
            JoyIndexChangeArgs args = new JoyIndexChangeArgs(PluginID, UIKey, Index);
            ValueChange(this, args);
        }
        public void JoyMouseMoveWheel(MouseEventArgs e)
        {
            if (Hide) return;
        }
        #endregion

        public void DxRenderLogic()
        {
            if (Hide) return;
            edgeWidth = (int)(Math.Min(Rect.Height, Rect.Width) * 0.05f);
            if (bRadioStyle)
            {
                rr.Rect = PublicData.GetActualRange(new RectangleF(Rect.X, Rect.Y, Rect.Height, Rect.Height), edgeWidth);
                rr.RadiusX = rr.Rect.Height / 3f;
                rr.RadiusY = rr.Rect.Height / 3f;
                rrText.X = Rect.X + Rect.Height * 1.2f + edgeWidth * 2;
                rrText.Y = Rect.Y + edgeWidth;
                rrText.Width = Rect.Width - Rect.Height * 1.2f - edgeWidth * 3;
                rrText.Height = Rect.Height - edgeWidth * 2;
            }
            else
            {
                rr.Rect = PublicData.GetActualRange(new RectangleF(Rect.X, Rect.Y, Rect.Height * 2f, Rect.Height), edgeWidth);
                rr.RadiusX = rr.Rect.Height / 3f;
                rr.RadiusY = rr.Rect.Height / 3f;
                rrText.X = Rect.X + Rect.Height * 2.2f + edgeWidth * 2;
                rrText.Y = Rect.Y + edgeWidth;
                rrText.Width = Rect.Width - Rect.Height * 2.2f - edgeWidth * 3;
                rrText.Height = Rect.Height - edgeWidth * 2;
            }
        }

        public void DxRenderHigh()
        {
            if (Hide) return;
        }

        public void DxRenderMedium()
        {
            if (Hide) return;
            SolidColorBrush brush = Dx2D.Instance.GetSolidColorBrush(XmlUI.DxUIBackColor);
            #region 外框
            if (mouseEnter)
            {
                brush.Color = XmlUI.DxUIClickColor;
            }
            else
            {
                brush.Color = XmlUI.DxUIBackColor;
            }
            Dx2D.Instance.RenderTarget2D.FillRoundedRectangle(rr, brush);
            Dx2D.Instance.RenderTarget2D.DrawRoundedRectangle(rr, brush, edgeWidth * 3);
            #endregion
            #region 开关
            if (bSwitchOn)
            {
                brush.Color = XmlUI.DxDeviceGreen;
            }
            else
            {
                brush.Color = XmlUI.DxDeviceRed;
            }
            RoundedRectangle rrS = new RoundedRectangle();
            if (bRadioStyle)
            {
                rrS.Rect = PublicData.GetActualRange(rr.Rect, edgeWidth);
                rrS.RadiusX = rrS.Rect.Height / 3f;
                rrS.RadiusY = rrS.Rect.Height / 3f;
                Dx2D.Instance.RenderTarget2D.FillRoundedRectangle(rrS, brush);
                brush.Color = XmlUI.DxTextColor;
            }
            else
            {
                rrS.Rect = PublicData.GetActualRange(rr.Rect, edgeWidth);
                rrS.RadiusX = rrS.Rect.Height / 3f;
                rrS.RadiusY = rrS.Rect.Height / 3f;
                Dx2D.Instance.RenderTarget2D.FillRoundedRectangle(rrS, brush);
                if (!bSwitchOn)
                {
                    rrS.Rect.X += rrS.Rect.Width / 2f;
                }
                rrS.Rect.Width = rrS.Rect.Width / 2f;
                brush.Color = XmlUI.DxTextColor;
                Dx2D.Instance.RenderTarget2D.FillRoundedRectangle(rrS, brush);
                brush.Color = XmlUI.DxUIClickColor;
                Dx2D.Instance.RenderTarget2D.DrawRoundedRectangle(rrS, brush, edgeWidth);
            }
            if (bSwitchOn)
            {
                Dx2D.Instance.RenderTarget2D.DrawLine(
                    new Vector2(rrS.Rect.X + edgeWidth * 4f, rrS.Rect.Y + rrS.Rect.Height / 2f - edgeWidth),
                    new Vector2(rrS.Rect.X + rrS.Rect.Width / 2f - edgeWidth, rrS.Rect.Y + rrS.Rect.Height - edgeWidth * 4f), brush, 2f);
                Dx2D.Instance.RenderTarget2D.DrawLine(
                    new Vector2(rrS.Rect.X + rrS.Rect.Width - edgeWidth * 4f, rrS.Rect.Y + edgeWidth * 4f),
                    new Vector2(rrS.Rect.X + rrS.Rect.Width / 2f - edgeWidth, rrS.Rect.Y + rrS.Rect.Height - edgeWidth * 4f), brush, 2f);
            }
            else
            {
                Dx2D.Instance.RenderTarget2D.DrawLine(
                    new Vector2(rrS.Rect.X + edgeWidth * 4f, rrS.Rect.Y + edgeWidth * 4f),
                    new Vector2(rrS.Rect.X + rrS.Rect.Width - edgeWidth * 4f, rrS.Rect.Y + rrS.Rect.Height - edgeWidth * 4f), brush, 2f);
                Dx2D.Instance.RenderTarget2D.DrawLine(
                    new Vector2(rrS.Rect.X + edgeWidth * 4f, rrS.Rect.Y + rrS.Rect.Height - edgeWidth * 4f),
                    new Vector2(rrS.Rect.X + rrS.Rect.Width - edgeWidth * 4f, rrS.Rect.Y + edgeWidth * 4f), brush, 2f);
            }
            #endregion
            #region 文字
            brush.Color = XmlUI.DxTextColor;
            Dx2D.Instance.RenderTarget2D.DrawText(drawText, tf, rrText, brush);
            #endregion
        }

        public void DxRenderLow()
        {
            if (Hide) return;
        }
        private void doNothing(object sender, EventArgs e)
        {

        }
    }
}
