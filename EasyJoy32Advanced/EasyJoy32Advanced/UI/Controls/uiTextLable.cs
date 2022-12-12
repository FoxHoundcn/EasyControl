using SharpDX;
using SharpDX.DirectWrite;
using System;
using System.Windows.Forms;

namespace EasyControl
{
    public class uiTextLable : iControl
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
        private RectangleF drawRect = new RectangleF();
        private bool _bFrame = false;
        public bool bFrame
        {
            get { return _bFrame; }
            set
            {
                _bFrame = value;
            }
        }
        public bool bLocalization { get; set; } = true;
        private RectangleF textRect;
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
        float maxHeight;
        //----
        public string Text { get { return Name; } set { Name = value; } }
        public Color4 ForeColor { get; set; }
        Color4 _backColor;
        public Color4 BackColor
        {
            get { return _backColor; }
            set
            {
                _backColor = value;
            }
        }
        public float FontRatio = 1f;
        public TextAlignment textAlignment { get; set; } = TextAlignment.Leading;
        public ParagraphAlignment pargraphAlignment { get; set; } = ParagraphAlignment.Center;
        public FontWeight fontWeight { get; set; } = FontWeight.Normal;
        public FontStyle fontStyle { get; set; } = FontStyle.Normal;
        public bool AutoSize = false;
        //////////////////////////////////////////////////////////////////////////////////
        public uiTextLable(string _text, Color4 _color, Color4 _back, float _maxHeight, bool _frame, bool _loc)
        {
            Index = maxIndex;
            maxIndex++;
            Name = _text;
            ForeColor = _color;
            BackColor = _back;
            maxHeight = _maxHeight;
            Rect = new RectangleF();
            bFrame = _frame;
            bLocalization = _loc;
            //-------------------------------------------
            Dx2DResize();
        }
        #region 鼠标
        public void JoyMouseMoveEvent(MouseEventArgs e)
        {
            if (Hide) return;
        }
        public void JoyMouseDownEvent(MouseEventArgs e)
        {
            if (Hide) return;
        }
        public void JoyMouseUpEvent(MouseEventArgs e)
        {
            if (Hide) return;
        }
        public void JoyMouseMoveWheel(MouseEventArgs e)
        {
            if (Hide) return;
        }
        #endregion
        public void Dx2DResize()
        {
            if (Hide) return;
            drawText = Dx2D.Instance.GetDrawText(bLocalization ? Localization.Instance.GetLS(Name) : Name,
                textRect, ref tf, textAlignment, pargraphAlignment, maxHeight, fontWeight, fontStyle);
        }

        public void DxRenderLogic()
        {
            if (Hide) return;
            if (AutoSize)
            {
                Dx2D.Instance.GetTextFormat(ref tf, maxHeight, textAlignment, pargraphAlignment, maxHeight, fontWeight, fontStyle);
                System.Drawing.SizeF size = Dx2D.Instance.MeasureString(bLocalization ? Localization.Instance.GetLS(Name) : Name, tf);
                Rect = new RectangleF(Rect.X, Rect.Y, size.Width + size.Height, size.Height * 1.4f);
            }
            edgeWidth = (int)(Math.Min(Rect.Height, Rect.Width) * 0.05f);
            drawRect = PublicData.GetActualRange(Rect, edgeWidth * 2);
            textRect = PublicData.GetActualRange(drawRect, edgeWidth * 2, (int)(drawRect.Height * (1f - FontRatio) / 2f + edgeWidth * 2));
        }

        public void DxRenderHigh()
        {
            if (Hide) return;
        }

        public void DxRenderMedium()
        {
            if (Hide) return;
            if (bFrame)
            {
                Dx2D.Instance.RenderTarget2D.FillRectangle(drawRect, Dx2D.Instance.GetSolidColorBrush(BackColor));
                Dx2D.Instance.RenderTarget2D.DrawRectangle(drawRect, Dx2D.Instance.GetSolidColorBrush(XmlUI.DxUIClickColor), edgeWidth);
            }
            Dx2D.Instance.RenderTarget2D.DrawText(drawText, tf, textRect, Dx2D.Instance.GetSolidColorBrush(ForeColor));
        }

        public void DxRenderLow()
        {
            if (Hide) return;
        }
    }
}
