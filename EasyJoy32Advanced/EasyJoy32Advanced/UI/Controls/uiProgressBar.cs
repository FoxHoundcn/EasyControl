using SharpDX;
using SharpDX.DirectWrite;
using System;
using System.Windows.Forms;

namespace EasyControl
{
    public class uiProgressBar : iControl
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
        private RectangleF barRect = new RectangleF();
        private RectangleF drawRect = new RectangleF();
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
        //----
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
        public Color4 TextColor { get; set; }
        public float FontRatio = 1f;
        public TextAlignment textAlignment { get; set; } = TextAlignment.Center;
        public ParagraphAlignment pargraphAlignment { get; set; } = ParagraphAlignment.Center;
        public FontWeight fontWeight { get; set; } = FontWeight.Normal;
        public FontStyle fontStyle { get; set; } = FontStyle.Normal;
        public bool AutoSize = false;
        public float Percentage = 1f;
        //////////////////////////////////////////////////////////////////////////////////
        public uiProgressBar(Color4 _color, Color4 _back, Color4 _text, float _percentage)
        {
            Index = maxIndex;
            maxIndex++;
            Name = "jProgressBar" + Index;
            ForeColor = _color;
            BackColor = _back;
            TextColor = _text;
            Rect = new RectangleF();
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
            drawText = Dx2D.Instance.GetDrawText(((int)(Percentage * 100f)).ToString(),
                textRect, ref tf, textAlignment, pargraphAlignment, textRect.Height, fontWeight, fontStyle);
        }

        public void DxRenderLogic()
        {
            if (Hide) return;
            if (AutoSize)
            {
                Dx2D.Instance.GetTextFormat(ref tf, textRect.Height, textAlignment, pargraphAlignment, textRect.Height, fontWeight, fontStyle);
                System.Drawing.SizeF size = Dx2D.Instance.MeasureString(((int)(Percentage * 100f)).ToString(), tf);
                Rect = new RectangleF(Rect.X, Rect.Y, size.Width + size.Height, size.Height * 1.4f);
            }
            edgeWidth = (int)(Math.Min(Rect.Height, Rect.Width) * 0.05f);
            drawRect = PublicData.GetActualRange(Rect, edgeWidth * 2);
            textRect = PublicData.GetActualRange(drawRect, edgeWidth * 2, (int)(drawRect.Height * (1f - FontRatio) / 2f + edgeWidth * 2));
            barRect = PublicData.GetActualRange(new RectangleF(drawRect.X, drawRect.Y, drawRect.Width * Percentage, drawRect.Height), edgeWidth * 2);
        }

        public void DxRenderHigh()
        {
            if (Hide) return;
        }

        public void DxRenderMedium()
        {
            if (Hide) return;
            Dx2D.Instance.RenderTarget2D.FillRectangle(drawRect, Dx2D.Instance.GetSolidColorBrush(BackColor));
            Dx2D.Instance.RenderTarget2D.DrawRectangle(drawRect, Dx2D.Instance.GetSolidColorBrush(XmlUI.DxUIClickColor), edgeWidth);
            if (Percentage > 0f)
            {
                Dx2D.Instance.RenderTarget2D.FillRectangle(barRect, Dx2D.Instance.GetSolidColorBrush(ForeColor));
                Dx2D.Instance.RenderTarget2D.DrawText(drawText + " %", tf, textRect, Dx2D.Instance.GetSolidColorBrush(TextColor));
            }
        }

        public void DxRenderLow()
        {
            if (Hide) return;
        }
    }
}
