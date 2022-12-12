using SharpDX;
using SharpDX.DirectWrite;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace EasyControl
{
    public class uiTextEditor : iControl
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
        private RectangleF drawRect = new RectangleF();
        public string Text
        {
            get { return textEdit.ToString(); }
            set { textEdit = new StringBuilder(value); }
        }
        private StringBuilder textEdit = new StringBuilder();
        #region 事件
        public EventHandler TextChange;
        #endregion
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
        public bool password = false;
        int _currentIndex;
        public int CurrentIndex
        {
            get
            {
                if (_currentIndex < 0)
                    _currentIndex = 0;
                if (_currentIndex >= textEdit.Length)
                    _currentIndex = textEdit.Length;
                return _currentIndex;
            }
            set
            {
                _currentIndex = value;
                if (_currentIndex < 0)
                    _currentIndex = 0;
                if (_currentIndex >= textEdit.Length)
                    _currentIndex = textEdit.Length;
            }
        }
        private System.Drawing.SizeF indexSize;
        private List<System.Drawing.SizeF> sizeList = new List<System.Drawing.SizeF>();
        //////////////////////////////////////////////////////////////////////////////////
        public uiTextEditor(string _text, bool nodeLink = false)
        {
            Index = maxIndex;
            maxIndex++;
            Text = _text;
            Rect = new RectangleF();
            NodeLinkMode = nodeLink;
            //-------------------------------------------
            Dx2DResize();
            TextChange += doNothing;
        }
        public void Dx2DResize()
        {
            if (Hide) return;
            Dx2D.Instance.GetTextFormat(ref tf, PublicData.GetActualRange(drawRect, edgeWidth * 2).Height);
            sizeList.Clear();
            if (textEdit.Length > 0)
            {
                for (int i = 1; i <= textEdit.Length; i++)
                {
                    sizeList.Add(Dx2D.Instance.MeasureString(textEdit.ToString().Substring(0, i), tf));
                }
                indexSize = Dx2D.Instance.MeasureString(textEdit.ToString().Substring(0, textEdit.Length - CurrentIndex), tf);
            }
            else
            {
                indexSize = new System.Drawing.SizeF(0, 0);
            }
        }
        #region 文字处理
        public void AppendText(char text)
        {
            if (PublicData.currentTextEdit == this)
            {
                if (textEdit.Length <= 0)
                {
                    textEdit.Append(text);
                }
                else
                {
                    int index = textEdit.Length;
                    textEdit.Insert(index - CurrentIndex, text);
                    //textEdit.Append(text);
                }
                TriggerTextChange();
            }
        }
        public void AppendText(string text)
        {
            if (PublicData.currentTextEdit == this)
            {
                if (textEdit.Length <= 0)
                {
                    textEdit.Append(text);
                }
                else
                {
                    int index = textEdit.Length;
                    textEdit.Insert(index - CurrentIndex, text);
                }
                TriggerTextChange();
            }
        }
        public void RemoveText()
        {
            if (PublicData.currentTextEdit == this)
            {
                if (textEdit.Length > 0)
                {
                    int index = textEdit.Length - CurrentIndex - 1;
                    if (index >= 0 && index < textEdit.Length)
                    {
                        textEdit.Remove(index, 1);
                        TriggerTextChange();
                    }
                }
            }
        }
        public void DeleteText()
        {
            if (PublicData.currentTextEdit == this)
            {
                if (textEdit.Length > 0)
                {
                    int index = textEdit.Length - CurrentIndex;
                    if (index >= 0 && index < textEdit.Length)
                    {
                        textEdit.Remove(index, 1);
                        if (textEdit.Length - CurrentIndex > 0)
                            CurrentIndex--;
                        TriggerTextChange();
                    }
                }
            }
        }
        public void TriggerTextChange()
        {
            JoyIndexChangeArgs args = new JoyIndexChangeArgs(PluginID, UIKey, Index);
            TextChange(this, args);
        }
        #endregion
        #region 鼠标操作
        public void JoyMouseMoveEvent(MouseEventArgs e)
        {
            if (Hide) return;
        }
        public void JoyMouseDownEvent(MouseEventArgs e)
        {
            if (Hide)
            {
                return;
            }
            bool select = false;
            float clickWidth = 0f;
            if (NodeLinkMode)
            {
                if (e.X >= (Rect.X + Offset.X) * NodeLinkControl.Instance.ScalingValue + Parent.Offset.X &&
                    e.X < (Rect.X + Offset.X + Rect.Width) * NodeLinkControl.Instance.ScalingValue + Parent.Offset.X &&
                    e.Y >= (Rect.Y + Offset.Y) * NodeLinkControl.Instance.ScalingValue + Parent.Offset.Y &&
                    e.Y < (Rect.Y + Offset.Y + Rect.Height) * NodeLinkControl.Instance.ScalingValue + Parent.Offset.Y)
                {
                    clickWidth = Rect.Width - ((Rect.X + Offset.X + Rect.Width) * NodeLinkControl.Instance.ScalingValue + Parent.Offset.X - e.X);
                    select = true;
                }
            }
            else
            {
                if (e.X >= Rect.X + Offset.X && e.X < Rect.X + Offset.X + Rect.Width &&
                    e.Y >= Rect.Y + Offset.Y && e.Y < Rect.Y + Offset.Y + Rect.Height)
                {
                    clickWidth = Rect.Width - (Rect.X + Offset.X + Rect.Width - e.X);
                    select = true;
                }
            }
            if (select)
            {
                if (sizeList.Count > 0)
                {
                    if (clickWidth < sizeList[0].Width)
                    {
                        CurrentIndex = textEdit.Length;
                    }
                    else if (clickWidth > sizeList[sizeList.Count - 1].Width)
                    {
                        CurrentIndex = 0;
                    }
                    else
                    {
                        for (int i = 1; i < sizeList.Count; i++)
                        {
                            if (clickWidth >= sizeList[i - 1].Width && clickWidth <= sizeList[i].Width)
                            {
                                CurrentIndex = textEdit.Length - i;
                            }
                        }
                    }
                }
                PublicData.currentTextEdit = this;
            }
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
        public void DxRenderLogic()
        {
            if (Hide) return;
            edgeWidth = (int)(Math.Min(Rect.Height, Rect.Width) * 0.05f);
            drawRect = PublicData.GetActualRange(Rect, edgeWidth * 2);
        }

        public void DxRenderHigh()
        {
            if (Hide) return;
        }

        public void DxRenderMedium()
        {
            if (Hide) return;
            if (PublicData.currentTextEdit == this)
                Dx2D.Instance.RenderTarget2D.DrawRectangle(drawRect, Dx2D.Instance.GetSolidColorBrush(XmlUI.DxUIClickColor), edgeWidth);
            else
                Dx2D.Instance.RenderTarget2D.DrawRectangle(drawRect, Dx2D.Instance.GetSolidColorBrush(XmlUI.DxUIBackColor), edgeWidth);
            string text = textEdit.ToString();
            if (password)
            {
                string temp = "";
                for (int i = 0; i < text.Length; i++)
                {
                    temp += "*";
                }
                text = temp;
            }
            Dx2D.Instance.RenderTarget2D.DrawText(text, tf,
                PublicData.GetActualRange(drawRect, edgeWidth * 2), Dx2D.Instance.GetSolidColorBrush(XmlUI.DxTextColor));
            if (PublicData.currentTextEdit == this)
                Dx2D.Instance.RenderTarget2D.DrawLine(new Vector2(drawRect.X + indexSize.Width + edgeWidth * 2, drawRect.Y),
                    new Vector2(drawRect.X + indexSize.Width + edgeWidth * 2, drawRect.Y + drawRect.Height), Dx2D.Instance.GetSolidColorBrush(XmlUI.DxTextColor));
        }

        public void DxRenderLow()
        {
            if (Hide) return;
        }
        public void doNothing(object sender, EventArgs e)
        {

        }
    }
}
