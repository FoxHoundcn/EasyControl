using SharpDX;
using System;
using System.Windows.Forms;

namespace EasyControl
{
    public class uiPanel : iControl
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
        //--------------------------------------------------------------------------------
        private RectangleF fillRect = new RectangleF();
        //----
        public Color4 ForeColor { get; set; }
        public bool bFill { get; set; }
        public bool bClick { get; set; }
        private bool _mouseEnter = false;
        public bool mouseEnter
        {
            get { return _mouseEnter; }
            private set
            {
                if (_mouseEnter == value)
                    return;
                _mouseEnter = value;
                if (_mouseEnter)
                {
                    JoyIndexChangeArgs args = new JoyIndexChangeArgs(PluginID, UIKey, Index);
                    MouseEnterEvent(this, args);
                }
                else
                {
                    JoyIndexChangeArgs args = new JoyIndexChangeArgs(PluginID, UIKey, Index);
                    MouseLeaveEvent(this, args);
                }
            }
        }
        public bool mouseDown { get; private set; } = false;
        RectangleF drawRect = new RectangleF();
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
        #region 事件
        public EventHandler MouseLeftDownEvent;
        public EventHandler MouseRightDownEvent;
        public EventHandler MouseLeftClickEvent;
        public EventHandler MouseRightClickEvent;
        public EventHandler MouseEnterEvent;
        public EventHandler MouseLeaveEvent;
        #endregion
        //////////////////////////////////////////////////////////////////////////////////
        public uiPanel(Color4 _color, bool _fill, bool _click)
        {
            Index = maxIndex;
            maxIndex++;
            Name = "jPanel" + Index;
            ForeColor = _color;
            bFill = _fill;
            bClick = _click;
            //-------------------------------------------
            Dx2DResize();
            MouseLeftDownEvent += doNothing;
            MouseRightDownEvent += doNothing;
            MouseLeftClickEvent += doNothing;
            MouseRightClickEvent += doNothing;
            MouseEnterEvent += doNothing;
            MouseLeaveEvent += doNothing;
        }
        #region 鼠标
        public void JoyMouseMoveEvent(MouseEventArgs e)
        {
            if (Hide)
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
            if (Hide)
            {
                return;
            }
            if (e.X >= Rect.X + Offset.X && e.X < Rect.X + Offset.X + Rect.Width &&
                e.Y >= Rect.Y + Offset.Y && e.Y < Rect.Y + Offset.Y + Rect.Height)
            {
                mouseDown = true;
                switch (e.Button)
                {
                    case MouseButtons.Left:
                        TriggerLeftButtonDown();
                        break;
                    case MouseButtons.Right:
                        TriggerRightButtonDown();
                        break;
                }
            }
        }
        public void JoyMouseUpEvent(MouseEventArgs e)
        {
            if (Hide)
            {
                mouseDown = false;
                return;
            }
            if (mouseDown && bClick)
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
            if (Hide) return;
        }
        public void TriggerLeftButtonDown()
        {
            JoyIndexChangeArgs args = new JoyIndexChangeArgs(PluginID, UIKey, Index);
            MouseLeftDownEvent(this, args);
        }
        public void TriggerRightButtonDown()
        {
            JoyIndexChangeArgs args = new JoyIndexChangeArgs(PluginID, UIKey, Index);
            MouseRightDownEvent(this, args);
        }
        public void TriggerLeftButtonClick()
        {
            JoyIndexChangeArgs args = new JoyIndexChangeArgs(PluginID, UIKey, Index);
            MouseLeftClickEvent(this, args);
        }
        public void TriggerRightButtonClick()
        {
            JoyIndexChangeArgs args = new JoyIndexChangeArgs(PluginID, UIKey, Index);
            MouseRightClickEvent(this, args);
        }
        #endregion
        public void Dx2DResize()
        {
        }

        public void DxRenderLogic()
        {
            if (Hide) return;
            edgeWidth = (int)(Math.Min(Rect.Height, Rect.Width) * 0.05f);
            fillRect = PublicData.GetActualRange(Rect, edgeWidth * 2);
            drawRect = PublicData.GetActualRange(Rect, edgeWidth);
        }

        public void DxRenderHigh()
        {
            if (Hide) return;
        }

        public void DxRenderMedium()
        {
            if (Hide) return;
            if (bFill)
            {
                Dx2D.Instance.RenderTarget2D.FillRectangle(fillRect, Dx2D.Instance.GetSolidColorBrush(ForeColor));
            }
            else
            {
                Dx2D.Instance.RenderTarget2D.DrawRectangle(fillRect, Dx2D.Instance.GetSolidColorBrush(ForeColor), edgeWidth * 2);
            }
            if (mouseEnter && bClick)
            {
                Dx2D.Instance.RenderTarget2D.DrawRectangle(drawRect, Dx2D.Instance.GetSolidColorBrush(XmlUI.DxUIClickColor), edgeWidth * 2);
            }
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
