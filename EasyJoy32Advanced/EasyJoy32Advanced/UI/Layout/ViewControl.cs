using SharpDX;
using SharpDX.Direct2D1;
using System.Windows.Forms;

namespace EasyControl
{
    public class ViewControl : iControl
    {
        static int maxIndex = 0;
        public const float sliderWidth = 10f;
        //----------------------------------------------------------------------
        public bool NodeLinkMode { get; private set; }
        public iControl Parent { get; set; }
        public bool Hide { get; set; } = false;
        public int Index { get; private set; }
        public string Name { get; set; }
        public RectangleF DrawRect { get { return Rect; } }
        public RectangleF Rect { private get; set; }
        private Vector2 sourceOffset = new Vector2();
        private Vector2 _offset = new Vector2();
        public Vector2 Offset
        {
            get
            {
                Vector2 offset = new Vector2(Rect.X + _offset.X, Rect.Y + _offset.Y);
                return offset;
            }
            set
            {
                _offset = value;
            }
        }
        public string PluginID { get; set; }
        public string UIKey { set; private get; }
        //---------------------------------------------------------------------------------
        public LayoutType layoutType { get; private set; }
        public iControl control { get; private set; } = null;
        private bool moveV = false;
        private bool moveH = false;
        private int offsetX = 0;
        private int offsetY = 0;
        private LayerParameters lp = new LayerParameters();
        private Layer lay;
        private float barWidth;
        private float barHeight;
        private RectangleF rectVbar;
        private RectangleF rectHbar;
        //----
        public bool bVerticalBar = true;
        public bool bHorizontalBar = true;
        public bool hSlider { get; private set; } = false;
        public bool vSlider { get; private set; } = false;
        /////////////////////////////////////////////////////////////////////////
        public ViewControl(iControl _control, LayoutType _layoutType)
        {
            Index = maxIndex;
            Name = "ViewControl" + maxIndex;
            maxIndex++;
            control = _control;
            _control.Parent = this;
            layoutType = _layoutType;
        }
        #region 鼠标操作
        public void JoyMouseDownEvent(MouseEventArgs e)
        {
            if (Hide) return;
            if (e.X >= rectVbar.X && e.X < rectVbar.X + rectVbar.Width &&
                e.Y >= rectVbar.Y && e.Y < rectVbar.Y + rectVbar.Height && bVerticalBar)
            {
                sourceOffset = _offset;
                offsetX = e.X;
                moveV = true;
            }
            else
            {
                moveV = false;
            }

            if (e.X >= rectHbar.X && e.X < rectHbar.X + rectHbar.Width &&
                e.Y >= rectHbar.Y && e.Y < rectHbar.Y + rectHbar.Height && bHorizontalBar)
            {
                sourceOffset = _offset;
                offsetY = e.Y;
                moveH = true;
            }
            else
            {
                moveH = false;
            }

            if (e.X >= DrawRect.X && e.X < DrawRect.X + DrawRect.Width &&
                e.Y >= DrawRect.Y && e.Y < DrawRect.Y + DrawRect.Height)
            {
                control.JoyMouseDownEvent(e);
            }
        }
        public void JoyMouseUpEvent(MouseEventArgs e)
        {
            if (Hide) return;
            moveV = false;
            moveH = false;
            control.JoyMouseUpEvent(e);
        }
        public void JoyMouseMoveEvent(MouseEventArgs e)
        {
            if (Hide) return;
            if (moveV)
            {
                _offset.X = sourceOffset.X * (Rect.Width - barWidth - sliderWidth) / (control.DrawRect.Width - Rect.Width + sliderWidth) - (e.X - offsetX);
                _offset.X = _offset.X * (control.DrawRect.Width - Rect.Width + sliderWidth) / (Rect.Width - barWidth - sliderWidth);
                if (_offset.X < -(control.DrawRect.Width - Rect.Width + sliderWidth))
                    _offset.X = -(control.DrawRect.Width - Rect.Width + sliderWidth);
                if (_offset.X > 0)
                    _offset.X = 0;
            }
            if (moveH)
            {
                _offset.Y = sourceOffset.Y * (Rect.Height - barHeight - sliderWidth) / (control.DrawRect.Height - Rect.Height + sliderWidth) - (e.Y - offsetY);
                _offset.Y = _offset.Y * (control.DrawRect.Height - Rect.Height + sliderWidth) / (Rect.Height - barHeight - sliderWidth);
                if (_offset.Y < -(control.DrawRect.Height - Rect.Height + sliderWidth))
                    _offset.Y = -(control.DrawRect.Height - Rect.Height + sliderWidth);
                if (_offset.Y > 0)
                    _offset.Y = 0;
            }
            if (e.X >= DrawRect.X && e.X < DrawRect.X + DrawRect.Width &&
                e.Y >= DrawRect.Y && e.Y < DrawRect.Y + DrawRect.Height)
            {
                control.JoyMouseMoveEvent(e);
            }
        }
        public void JoyMouseMoveWheel(MouseEventArgs e)
        {
            if (Hide) return;
            if (e.X >= Rect.X && e.X < Rect.X + Rect.Width &&
                e.Y >= Rect.Y && e.Y < Rect.Y + Rect.Height - (hSlider ? sliderWidth : 0))
            {
                _offset.Y += e.Delta / (Rect.Height - (hSlider ? sliderWidth : 0)) * 500f;
            }
            if (e.X >= DrawRect.X && e.X < DrawRect.X + DrawRect.Width &&
                e.Y >= DrawRect.Y && e.Y < DrawRect.Y + DrawRect.Height)
            {
                control.JoyMouseMoveWheel(e);
            }
        }
        #endregion
        public void Dx2DResize()
        {
            if (Hide) return;
            control.Dx2DResize();
        }
        public void DxRenderLogic()
        {
            if (Hide) return;
            control.Offset = Offset;
            control.DxRenderLogic();
            //control.Rect = new RectangleF(0, 0, control.DrawRect.Width, control.DrawRect.Height);
            if (_offset.X + control.DrawRect.Width < Rect.Width - sliderWidth && Rect.Width < control.DrawRect.Width)
                _offset.X = Rect.Width - control.DrawRect.Width;
            if (_offset.X > 0)
                _offset.X = 0;
            if (Rect.Width > control.DrawRect.Width)
                _offset.X = 0;
            if (_offset.Y + control.DrawRect.Height < Rect.Height - (hSlider ? sliderWidth : 0) && Rect.Height < control.DrawRect.Height)
                _offset.Y = Rect.Height - control.DrawRect.Height - (hSlider ? sliderWidth : 0);
            if (_offset.Y > 0)
                _offset.Y = 0;
            if (Rect.Height > control.DrawRect.Height)
                _offset.Y = 0;
        }

        private void DrawSlider()
        {
            vSlider = false;
            hSlider = false;
            if (Rect.Width < control.DrawRect.Width && bVerticalBar)
            {
                vSlider = true;
            }
            if (Rect.Height < control.DrawRect.Height && bHorizontalBar)
            {
                hSlider = true;
            }
            if (hSlider && !vSlider && Rect.Width - sliderWidth < control.DrawRect.Width && bVerticalBar)
            {
                vSlider = true;
            }
            if (vSlider && !hSlider && Rect.Height - sliderWidth < control.DrawRect.Height && bHorizontalBar)
            {
                hSlider = true;
            }
            if (vSlider)
            {
                RectangleF rectV = new RectangleF(Rect.X, Rect.Y + Rect.Height - sliderWidth, Rect.Width - sliderWidth, sliderWidth);
                Dx2D.Instance.RenderTarget2D.FillRectangle(rectV, Dx2D.Instance.GetSolidColorBrush(XmlUI.DxUIBackColor));
                Dx2D.Instance.RenderTarget2D.DrawRectangle(rectV, Dx2D.Instance.GetSolidColorBrush(XmlUI.DxUIClickColor));
                RectangleF rectFill = new RectangleF(Rect.X + Rect.Width - sliderWidth, Rect.Y + Rect.Height - sliderWidth, sliderWidth, sliderWidth);
                Dx2D.Instance.RenderTarget2D.FillRectangle(rectFill, Dx2D.Instance.GetSolidColorBrush(XmlUI.DxBackColor));
                //bar-----------------------------------
                barWidth = (1f + ((Rect.Width - sliderWidth) / control.DrawRect.Width * 3f)) * sliderWidth;
                float barX = -_offset.X * (Rect.Width - barWidth - sliderWidth) / (control.DrawRect.Width - Rect.Width + sliderWidth);
                rectVbar = new RectangleF(Rect.X + barX + 1, Rect.Y + Rect.Height - sliderWidth + 1, barWidth - 2, sliderWidth - 2);
                Dx2D.Instance.RenderTarget2D.FillRectangle(rectVbar, Dx2D.Instance.GetSolidColorBrush(XmlUI.DxDeviceYellow));
                Dx2D.Instance.RenderTarget2D.DrawRectangle(rectVbar, Dx2D.Instance.GetSolidColorBrush(XmlUI.DxBackColor));
            }
            if (hSlider)
            {
                RectangleF rectH = new RectangleF(Rect.X + Rect.Width - sliderWidth, Rect.Y, sliderWidth, Rect.Height - sliderWidth);
                Dx2D.Instance.RenderTarget2D.FillRectangle(rectH, Dx2D.Instance.GetSolidColorBrush(XmlUI.DxUIBackColor));
                Dx2D.Instance.RenderTarget2D.DrawRectangle(rectH, Dx2D.Instance.GetSolidColorBrush(XmlUI.DxUIClickColor));
                RectangleF rectFill = new RectangleF(Rect.X + Rect.Width - sliderWidth, Rect.Y + Rect.Height - sliderWidth, sliderWidth, sliderWidth);
                Dx2D.Instance.RenderTarget2D.FillRectangle(rectFill, Dx2D.Instance.GetSolidColorBrush(XmlUI.DxBackColor));
                //bar-----------------------------------
                barHeight = (1f + ((Rect.Height - sliderWidth) / control.DrawRect.Height * 3f)) * sliderWidth;
                float barY = -_offset.Y * (Rect.Height - barHeight - sliderWidth) / (control.DrawRect.Height - Rect.Height + sliderWidth);
                rectHbar = new RectangleF(Rect.X + Rect.Width - sliderWidth + 1, Rect.Y + barY + 1, sliderWidth - 2, barHeight - 2);
                Dx2D.Instance.RenderTarget2D.FillRectangle(rectHbar, Dx2D.Instance.GetSolidColorBrush(XmlUI.DxDeviceYellow));
                Dx2D.Instance.RenderTarget2D.DrawRectangle(rectHbar, Dx2D.Instance.GetSolidColorBrush(XmlUI.DxBackColor));
            }
        }

        public void DxRenderHigh()
        {
            if (Hide) return;
            if (layoutType == LayoutType.High)
            {
                lp.ContentBounds = Rect;
                lp.LayerOptions = LayerOptions.InitializeForCleartype;
                lp.Opacity = 1f;
                lay = new Layer(Dx2D.Instance.RenderTarget2D);
                Dx2D.Instance.RenderTarget2D.PushLayer(ref lp, lay);
                Matrix3x2 translate = Matrix3x2.Translation(Offset);
                Dx2D.Instance.RenderTarget2D.Transform = translate;
                control.DxRenderLow();
                control.DxRenderMedium();
                control.DxRenderHigh();
                Dx2D.Instance.RenderTarget2D.Transform = Matrix3x2.Identity;
                Dx2D.Instance.RenderTarget2D.PopLayer();
                lay.Dispose();
                DrawSlider();
            }
        }

        public void DxRenderMedium()
        {
            if (Hide) return;
            if (layoutType == LayoutType.Medium)
            {
                lp.ContentBounds = Rect;
                lp.LayerOptions = LayerOptions.InitializeForCleartype;
                lp.Opacity = 1f;
                lay = new Layer(Dx2D.Instance.RenderTarget2D);
                Dx2D.Instance.RenderTarget2D.PushLayer(ref lp, lay);
                Matrix3x2 translate = Matrix3x2.Translation(Offset);
                Dx2D.Instance.RenderTarget2D.Transform = translate;
                control.DxRenderLow();
                control.DxRenderMedium();
                control.DxRenderHigh();
                Dx2D.Instance.RenderTarget2D.Transform = Matrix3x2.Identity;
                Dx2D.Instance.RenderTarget2D.PopLayer();
                lay.Dispose();
                DrawSlider();
            }
        }

        public void DxRenderLow()
        {
            if (Hide) return;
            if (layoutType == LayoutType.Low)
            {
                lp.ContentBounds = Rect;
                lp.LayerOptions = LayerOptions.InitializeForCleartype;
                lp.Opacity = 1f;
                lay = new Layer(Dx2D.Instance.RenderTarget2D);
                Dx2D.Instance.RenderTarget2D.PushLayer(ref lp, lay);
                Matrix3x2 translate = Matrix3x2.Translation(Offset);
                Dx2D.Instance.RenderTarget2D.Transform = translate;
                control.DxRenderLow();
                control.DxRenderMedium();
                control.DxRenderHigh();
                Dx2D.Instance.RenderTarget2D.Transform = Matrix3x2.Identity;
                Dx2D.Instance.RenderTarget2D.PopLayer();
                lay.Dispose();
                DrawSlider();
            }
        }
    }
}
