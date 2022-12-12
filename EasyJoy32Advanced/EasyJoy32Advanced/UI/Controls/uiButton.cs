using SharpDX;
using SharpDX.DirectWrite;
using System;
using System.Windows.Forms;

namespace EasyControl
{
    public class uiButton : iControl
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
        public bool Enable = true;
        public bool mouseEnter { get; private set; } = false;
        public bool mouseDown { get; private set; } = false;
        private jBtnType btnType;
        private string drawText = "";
        private TextFormat tf;
        #region 事件
        public EventHandler LeftButtonClick;
        public EventHandler RightButtonClick;
        #endregion
        //----
        public bool AlwaysOn { get; set; } = false;
        public bool SelectOn { get; set; } = false;
        public Color4 TextColor { get; set; } = XmlUI.DxTextColor;
        public Color4 ForeColor { get; set; }
        Color4 _backColor;
        public Color4 BackColor
        {
            get { return _backColor; }
            set
            {
                _backColor = value;
                _backColor.Alpha = 0.7f;
            }
        }
        TextAlignment _textAlignment = TextAlignment.Center;
        public TextAlignment textAlignment
        {
            set
            {
                _textAlignment = value;
            }
            get { return _textAlignment; }
        }
        public float FontRatio = 1f;
        private uiImage _Image = null;
        public uiImage Image
        {
            get { return _Image; }
            private set
            {
                if (btnType == jBtnType.Image)
                    _Image = value;
            }
        }
        //---------------------------------------------------------------------------------
        RectangleF drawRect = new RectangleF();
        RectangleF rectInside = new RectangleF();
        RectangleF rectDown = new RectangleF();
        RectangleF rectColor = new RectangleF();
        RectangleF rectText = new RectangleF();
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
        /////////////////////////////////////////////////////////////////////////////////////
        public uiButton(int _Index, string _Name, jBtnType _type)
        {
            Index = _Index;
            Name = _Name;
            Rect = new RectangleF();
            //-------------------------------------------
            ForeColor = XmlUI.DxUIBackColor;
            _backColor = XmlUI.DxUIBackColor;
            if (_type == jBtnType.Image)
            {
                throw new Exception("uiButton Type Error !!!");
            }
            btnType = _type;
            //--------------------------------------------------------------------------------------------
            Dx2DResize();
            LeftButtonClick += doNothing;
            RightButtonClick += doNothing;
        }
        public uiButton(int _Index, string _Name, uiImage _image)
        {
            Index = _Index;
            Name = _Name;
            Rect = new RectangleF();
            //-------------------------------------------
            ForeColor = XmlUI.DxUIBackColor;
            _backColor = XmlUI.DxUIBackColor;
            btnType = jBtnType.Image;
            Image = _image;
            //--------------------------------------------------------------------------------------------
            Dx2DResize();
            LeftButtonClick += doNothing;
            RightButtonClick += doNothing;
        }
        public void Dx2DResize()
        {
            if (Hide) return;
            string name = "";
            switch (btnType)
            {
                case jBtnType.NoLocalization:
                    name = Name;
                    break;
                default:
                    name = Localization.Instance.GetLS(Name);
                    break;
            }
            drawText = Dx2D.Instance.GetDrawText(name, rectText, ref tf, textAlignment);
        }
        #region 渲染
        public void DxRenderLogic()
        {
            if (Hide) return;
            edgeWidth = (int)(Math.Min(Rect.Height, Rect.Width) * 0.05f);
            drawRect = PublicData.GetActualRange(Rect, edgeWidth);
            rectInside = PublicData.GetActualRange(drawRect, edgeWidth * 3);
            rectDown = PublicData.GetActualRange(drawRect, edgeWidth);
            int interval = (int)(rectInside.Width > rectInside.Height ? rectInside.Height * 0.05f : rectInside.Width * 0.05f);
            if (interval < 1)
                interval = 1;
            rectColor.X = rectInside.X + interval * 2;
            rectColor.Y = rectInside.Y + interval * 2;
            rectColor.Width = interval * 4;
            rectColor.Height = rectInside.Height - interval * 4;
            int width = (int)(drawRect.Width > drawRect.Height ? drawRect.Height : drawRect.Width);
            if (AlwaysOn)
            {
                rectText.X = rectInside.X + interval * 7;
                rectText.Width = rectInside.Width - interval * 8;
            }
            else
            {
                rectText.X = rectInside.X + interval * 2;
                rectText.Width = rectInside.Width - interval * 3;
            }
            int heightTemp = (int)(rectInside.Height * (1f - FontRatio) / 2f + interval * 2);
            rectText.Y = rectInside.Y + heightTemp;
            rectText.Height = rectInside.Height - heightTemp * 2;
            if (btnType == jBtnType.Image && _Image != null)
            {
                _Image.Rect = rectText;
                _Image.DxRenderLogic();
            }
        }
        public void DxRenderLow()
        {
            if (Hide) return;
        }
        public void DxRenderMedium()
        {
            if (Hide) return;
            if (mouseEnter)
            {
                if (SelectOn)
                    Dx2D.Instance.RenderTarget2D.DrawRectangle(drawRect, Dx2D.Instance.GetSolidColorBrush(XmlUI.DxDeviceBlue), edgeWidth * 2);
                else
                    Dx2D.Instance.RenderTarget2D.DrawRectangle(drawRect, Dx2D.Instance.GetSolidColorBrush(XmlUI.DxUIClickColor), edgeWidth);
            }
            else
            {
                if (SelectOn)
                    Dx2D.Instance.RenderTarget2D.DrawRectangle(drawRect, Dx2D.Instance.GetSolidColorBrush(XmlUI.DxDeviceBlue), edgeWidth * 2);
                else
                    Dx2D.Instance.RenderTarget2D.DrawRectangle(drawRect, Dx2D.Instance.GetSolidColorBrush(XmlUI.DxUIBackColor), edgeWidth);
            }
            #region 主体
            Dx2D.Instance.RenderTarget2D.FillRectangle(rectInside, Dx2D.Instance.GetSolidColorBrush(XmlUI.DxUIBackColor));
            Dx2D.Instance.RenderTarget2D.DrawRectangle(rectInside, Dx2D.Instance.GetSolidColorBrush(BackColor), edgeWidth * 3);
            #endregion
            #region 点击
            if (mouseDown)
                Dx2D.Instance.RenderTarget2D.DrawRectangle(rectDown, Dx2D.Instance.GetSolidColorBrush(XmlUI.DxUIClickColor), edgeWidth * 2);
            #endregion
            #region 左边条
            if (AlwaysOn)
                Dx2D.Instance.RenderTarget2D.FillRectangle(rectColor, Dx2D.Instance.GetSolidColorBrush(ForeColor));
            #endregion
            #region Name
            if (btnType != jBtnType.Image)
            {
                Dx2D.Instance.RenderTarget2D.DrawText(drawText, tf, rectText, Dx2D.Instance.GetSolidColorBrush(TextColor));
            }
            else
            {
                _Image.DxRenderMedium();
            }
            #endregion
            #region  Enable
            if (!Enable)
            {
                Dx2D.Instance.RenderTarget2D.DrawRectangle(PublicData.GetActualRange(rectInside, edgeWidth),
                    Dx2D.Instance.GetSolidColorBrush(XmlUI.DxDeviceRed), edgeWidth * 2);
                Dx2D.Instance.RenderTarget2D.DrawLine(new Vector2(rectInside.X + edgeWidth * 2, rectInside.Y + edgeWidth * 2),
                    new Vector2(rectInside.X + rectInside.Width - edgeWidth * 2, rectInside.Y + rectInside.Height - edgeWidth * 2),
                    Dx2D.Instance.GetSolidColorBrush(XmlUI.DxDeviceRed), edgeWidth * 2);
                Dx2D.Instance.RenderTarget2D.DrawLine(new Vector2(rectInside.X + rectInside.Width - edgeWidth * 2, rectInside.Y + edgeWidth * 2),
                    new Vector2(rectInside.X + edgeWidth * 2, rectInside.Y + rectInside.Height - edgeWidth * 2),
                    Dx2D.Instance.GetSolidColorBrush(XmlUI.DxDeviceRed), edgeWidth * 2);
            }
            #endregion
        }
        public void DxRenderHigh() { if (Hide) return; }
        #endregion
        #region 鼠标
        public void JoyMouseMoveEvent(MouseEventArgs e)
        {
            if (Hide || !Enable)
            {
                mouseEnter = false;
                return;
            }
            if (e.X >= Rect.X + Offset.X && e.X < Rect.X + Offset.X + Rect.Width &&
                e.Y >= Rect.Y + Offset.Y && e.Y < Rect.Y + Offset.Y + Rect.Height)
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
            if (Hide || !Enable)
            {
                return;
            }
            if (e.X >= Rect.X + Offset.X && e.X < Rect.X + Offset.X + Rect.Width &&
                e.Y >= Rect.Y + Offset.Y && e.Y < Rect.Y + Offset.Y + Rect.Height)
            {
                mouseDown = true;
            }
        }
        public void JoyMouseUpEvent(MouseEventArgs e)
        {
            if (Hide || !Enable)
            {
                mouseDown = false;
                return;
            }
            if (mouseDown)
            {
                mouseDown = false;
                switch (e.Button)
                {
                    case MouseButtons.Left:
                        TriggerLeftButtonClick();
                        break;
                    case MouseButtons.Right:
                        TriggerRightButtonClick();
                        break;
                }
            }
        }
        public void JoyMouseMoveWheel(MouseEventArgs e)
        {
            if (Hide || !Enable) return;
        }
        public void TriggerLeftButtonClick()
        {
            JoyIndexChangeArgs args = new JoyIndexChangeArgs(PluginID, UIKey, Index);
            LeftButtonClick(this, args);
        }
        public void TriggerRightButtonClick()
        {
            JoyIndexChangeArgs args = new JoyIndexChangeArgs(PluginID, UIKey, Index);
            RightButtonClick(this, args);
        }
        #endregion
        private void doNothing(object sender, EventArgs e)
        {

        }
    }
}
