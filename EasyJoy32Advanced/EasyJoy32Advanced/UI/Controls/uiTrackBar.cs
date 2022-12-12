using SharpDX;
using SharpDX.Direct2D1;
using SharpDX.DirectWrite;
using System;
using System.Windows.Forms;

namespace EasyControl
{
    public class uiTrackBar : iControl
    {
        static int maxIndex = 0;
        //--------------------------------------------------------------
        public iControl Parent { get; set; }
        public RectangleF DrawRect { get { return Rect; } }
        public RectangleF Rect { private get; set; }
        public Vector2 Offset { get; set; }
        public bool Hide { get; set; } = false;
        public int Index { get; private set; }
        public string Name { get; set; }
        public string PluginID { get; set; }
        public string UIKey { set; private get; }
        public bool NodeLinkMode { get; private set; }
        //-------------------------------------------------------------------------------
        public bool mouseEnter { get; private set; } = false;
        public bool mouseDown { get; private set; } = false;
        #region 事件
        public int MinValue { get; set; }
        public int MaxValue { get; set; }
        private float _value;
        public int Value
        {
            get { return (int)_value; }
            set
            {
                if (_value != value)
                {
                    _value = value;
                    if (_value < MinValue)
                        _value = MinValue;
                    if (_value > MaxValue)
                        _value = MaxValue;
                    JoyIndexChangeArgs args = new JoyIndexChangeArgs(PluginID, UIKey, Index);
                    ValueChange(null, args);
                }
            }
        }
        public EventHandler ValueChange;
        #endregion
        RoundedRectangle rectTrack = new RoundedRectangle();
        RoundedRectangle rectBar = new RoundedRectangle();
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
        public uiTrackBar(int _minValue, int _maxValue)
        {
            ValueChange += doNothing;
            Index = maxIndex;
            maxIndex++;
            MinValue = _minValue;
            MaxValue = _maxValue;
            if (MinValue >= MaxValue)
                throw new Exception("jTrackBar min >= max Error !!!");
            Value = MinValue;
            Rect = new RectangleF();
            //-------------------------------------------
            Dx2DResize();
        }
        public void Dx2DResize()
        {
            if (Hide) return;
            Dx2D.Instance.GetTextFormat(ref tf, rectTrack.Rect.Height, TextAlignment.Center);
        }
        #region 鼠标
        public void JoyMouseMoveEvent(MouseEventArgs e)
        {
            if (Hide)
            {
                mouseEnter = false;
                return;
            }
            if (e.X >= rectBar.Rect.X + Offset.X && e.X < rectBar.Rect.X + Offset.X + rectBar.Rect.Width &&
                e.Y >= rectBar.Rect.Y + Offset.Y && e.Y < rectBar.Rect.Y + Offset.Y + rectBar.Rect.Height)
            {
                mouseEnter = true;
            }
            else
            {
                mouseEnter = false;
            }
            if (mouseDown)
            {
                if (e.X < DrawRect.X + Offset.X - DrawRect.Height || e.X >= DrawRect.X + Offset.X + DrawRect.Width + DrawRect.Height ||
                e.Y < DrawRect.Y + Offset.Y || e.Y >= DrawRect.Y + Offset.Y + DrawRect.Height)
                {
                    mouseDown = false;
                    return;
                }
                float valueWidth = Rect.Width - Rect.Height;
                float valueCurrent = e.X - DrawRect.X - Offset.X - Rect.Height / 2;
                if (valueCurrent < 0f)
                    valueCurrent = 0f;
                if (valueCurrent > valueWidth)
                    valueCurrent = valueWidth;
                _value = valueCurrent / valueWidth * (MaxValue - MinValue) + MinValue;
                TriggerValueChange();
            }
        }
        public void TriggerValueChange()
        {
            JoyIndexChangeArgs args = new JoyIndexChangeArgs(PluginID, UIKey, Index);
            ValueChange(this, args);
        }
        public void JoyMouseDownEvent(MouseEventArgs e)
        {
            if (Hide)
            {
                return;
            }
            if (e.X >= DrawRect.X + Offset.X && e.X < DrawRect.X + Offset.X + DrawRect.Width &&
                e.Y >= DrawRect.Y + Offset.Y && e.Y < DrawRect.Y + Offset.Y + DrawRect.Height)
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
            }
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
            rectTrack.Rect.X = Rect.X + Rect.Height / 4f;
            rectTrack.Rect.Y = Rect.Y + Rect.Height / 2f - Rect.Height / 6f;
            rectTrack.Rect.Width = Rect.Width - Rect.Height / 2f;
            rectTrack.Rect.Height = Rect.Height / 3f;
            rectTrack.RadiusX = Rect.Height / 6f;
            rectTrack.RadiusY = Rect.Height / 6f;
        }

        public void DxRenderHigh()
        {
            if (Hide) return;
        }
        private RoundedRectangle GetBarRoundedRect()
        {
            if (_value < MinValue)
                _value = MinValue;
            if (_value > MaxValue)
                _value = MaxValue;
            float valueStart = Rect.Height;
            float valueWidth = Rect.Width - Rect.Height * 2;
            float valueCurrent = (_value - MinValue) / (MaxValue - MinValue) * valueWidth;
            RoundedRectangle rectBar = new RoundedRectangle();
            rectBar.Rect.X = Rect.X + valueStart + valueCurrent - Rect.Height * 0.75f;
            rectBar.Rect.Y = Rect.Y + Rect.Height / 10f + edgeWidth;
            rectBar.Rect.Width = Rect.Height * 1.5f;
            rectBar.Rect.Height = Rect.Height / 5f * 4f - edgeWidth * 2;
            rectBar.RadiusX = Rect.Height / 6f;
            rectBar.RadiusY = Rect.Height / 6f;
            return rectBar;
        }
        public void DxRenderMedium()
        {
            if (Hide) return;
            #region 滑动条
            Dx2D.Instance.RenderTarget2D.FillRoundedRectangle(rectTrack, Dx2D.Instance.GetSolidColorBrush(XmlUI.DxUIBackColor));
            #endregion
            #region 滑动块
            rectBar = GetBarRoundedRect();
            Dx2D.Instance.RenderTarget2D.FillRoundedRectangle(rectBar, Dx2D.Instance.GetSolidColorBrush(XmlUI.DxTextColor));
            if (mouseEnter)
                Dx2D.Instance.RenderTarget2D.DrawRoundedRectangle(rectBar, Dx2D.Instance.GetSolidColorBrush(XmlUI.DxUIClickColor), edgeWidth * 2);
            else
                Dx2D.Instance.RenderTarget2D.DrawRoundedRectangle(rectBar, Dx2D.Instance.GetSolidColorBrush(XmlUI.DxUIBackColor), edgeWidth * 2);
            Dx2D.Instance.RenderTarget2D.DrawText(Value.ToString(), tf, rectBar.Rect, Dx2D.Instance.GetSolidColorBrush(XmlUI.DxUIBackColor));
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
