using SharpDX;
using SharpDX.Direct2D1;
using System;
using System.Windows.Forms;

namespace EasyControl
{
    public class uiKeyBoard : iControl
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
        //--------------------------------------------------------------------------------
        public byte Fun = 0;
        public byte Code = 255;
        //----
        private KeyBoardData kb;
        private float keyWidth;
        private float keyHeight;
        private byte moveKey;
        public bool mouseEnter { get; private set; } = false;
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
        public EventHandler KeyboardClick;
        #endregion
        ///////////////////////////////////////////////////////////////////////////////////
        public uiKeyBoard()
        {
            Index = maxIndex;
            maxIndex++;
            Name = "KeyBoard";
            Rect = new RectangleF();
            kb = new KeyBoardData();
            Dx2DResize();
            KeyboardClick += OnButtonClick;
        }
        public void Dx2DResize()
        {
            if (Hide) return;
            edgeWidth = (int)(Math.Min(Rect.Height, Rect.Width) * 0.05f / KeyBoardData.maxHeightCount);
            keyWidth = (Rect.Width - edgeWidth * 3) / KeyBoardData.maxWidthCount;
            keyHeight = (Rect.Height - edgeWidth * 3) / KeyBoardData.maxHeightCount;
            for (int i = 0; i < kb.btnList.Count; i++)
            {
                Key key = kb.btnList[i];
                float width = keyWidth;
                float height = keyHeight;
                switch (key.size)
                {
                    case keySize.x1:
                        break;
                    case keySize.x125:
                        width *= 1.25f;
                        break;
                    case keySize.x15:
                        width *= 1.5f;
                        break;
                    case keySize.x2:
                        width *= 2f;
                        break;
                    case keySize.x2H:
                        height *= 2f;
                        break;
                    case keySize.x25:
                        width *= 2.5f;
                        break;
                    case keySize.x55:
                        width *= 5.5f;
                        break;
                    default:
                        WarningForm.Instance.OpenUI("Key Size ERROR : " + key.size.ToString(), false);
                        break;
                }
                key.OuterFrame.X = Rect.X + (int)(keyWidth * key.x + 1) + edgeWidth * 2;
                key.OuterFrame.Y = Rect.Y + (int)(keyHeight * key.y + 1) + edgeWidth * 2;
                key.OuterFrame.Width = width - edgeWidth * 2;
                key.OuterFrame.Height = height - edgeWidth * 2;
                key.InnerFrame = PublicData.GetActualRange(key.OuterFrame, edgeWidth * 2);
                key.TextFrame.X = key.InnerFrame.X;
                key.TextFrame.Y = key.InnerFrame.Y + (key.InnerFrame.Height - keyHeight / key.sizeScale) / 2f;
                key.TextFrame.Width = key.InnerFrame.Width;
                key.TextFrame.Height = keyHeight / key.sizeScale;
                key.drawText = Dx2D.Instance.GetDrawText(key.ShowName, key.TextFrame, ref key.tf, SharpDX.DirectWrite.TextAlignment.Center);
            }
        }
        #region 鼠标
        public void JoyMouseMoveEvent(MouseEventArgs e)
        {
            if (Hide)
            {
                return;
            }
            moveKey = GetSelectHex(e.X, e.Y);
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
            moveKey = GetSelectHex(e.X, e.Y);
            if (moveKey == 255)
                return;
            switch (moveKey)
            {
                case 0xff:
                    break;
                case 0xE0:
                    if ((Fun & 0x01) == 0x01)
                    {
                        Fun ^= 0x01;
                    }
                    else
                    {
                        Fun |= 0x01;
                    }
                    break;
                case 0xE1:
                    if ((Fun & 0x02) == 0x02)
                    {
                        Fun ^= 0x02;
                    }
                    else
                    {
                        Fun |= 0x02;
                    }
                    break;
                case 0xE2:
                    if ((Fun & 0x04) == 0x04)
                    {
                        Fun ^= 0x04;
                    }
                    else
                    {
                        Fun |= 0x04;
                    }
                    break;
                case 0xE3:
                    if ((Fun & 0x08) == 0x08)
                    {
                        Fun ^= 0x08;
                    }
                    else
                    {
                        Fun |= 0x08;
                    }
                    break;
                case 0xE4:
                    if ((Fun & 0x10) == 0x10)
                    {
                        Fun ^= 0x10;
                    }
                    else
                    {
                        Fun |= 0x10;
                    }
                    break;
                case 0xE5:
                    if ((Fun & 0x20) == 0x20)
                    {
                        Fun ^= 0x20;
                    }
                    else
                    {
                        Fun |= 0x20;
                    }
                    break;
                case 0xE6:
                    if ((Fun & 0x40) == 0x40)
                    {
                        Fun ^= 0x40;
                    }
                    else
                    {
                        Fun |= 0x40;
                    }
                    break;
                case 0xE7:
                    if ((Fun & 0x80) == 0x80)
                    {
                        Fun ^= 0x80;
                    }
                    else
                    {
                        Fun |= 0x80;
                    }
                    break;
                default:
                    Code = moveKey;
                    break;
            }
            JoyIndexChangeArgs args = new JoyIndexChangeArgs(PluginID, UIKey, Index);
            KeyboardClick(null, args);
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
        public byte GetSelectHex(int x, int y)
        {
            for (int i = 0; i < kb.btnList.Count; i++)
            {
                Key key = kb.btnList[i];
                float width = keyWidth;
                float height = keyHeight;
                switch (key.size)
                {
                    case keySize.x1:
                        break;
                    case keySize.x125:
                        width *= 1.25f;
                        break;
                    case keySize.x15:
                        width *= 1.5f;
                        break;
                    case keySize.x2:
                        width *= 2f;
                        break;
                    case keySize.x2H:
                        height *= 2f;
                        break;
                    case keySize.x25:
                        width *= 2.5f;
                        break;
                    case keySize.x55:
                        width *= 5.5f;
                        break;
                    default:
                        WarningForm.Instance.OpenUI("Key Size ERROR : " + key.size.ToString(), false);
                        break;
                }
                //----------------------------------------------------------------------
                float RectX = Rect.X + Offset.X + (int)(keyWidth * key.x + 1);
                float RectY = Rect.Y + Offset.Y + (int)(keyHeight * key.y + 1);
                if (x >= RectX && x <= RectX + width &&
                    y >= RectY && y <= RectY + height)
                {
                    return key.Hex;
                }
            }
            return 255;
        }
        public void DxRenderLogic()
        {
            if (Hide) return;
        }

        public void DxRenderHigh()
        {
            if (Hide) return;
        }

        public void DxRenderLow()
        {
            if (Hide) return;
        }

        public void DxRenderMedium()
        {
            if (Hide) return;
            if (mouseEnter)
                Dx2D.Instance.RenderTarget2D.DrawRectangle(PublicData.GetActualRange(Rect, edgeWidth),
                    Dx2D.Instance.GetSolidColorBrush(XmlUI.DxUIClickColor), edgeWidth);
            else
                Dx2D.Instance.RenderTarget2D.DrawRectangle(PublicData.GetActualRange(Rect, edgeWidth),
                    Dx2D.Instance.GetSolidColorBrush(XmlUI.DxUIBackColor), edgeWidth);
            #region 按键
            for (int i = 0; i < kb.btnList.Count; i++)
            {
                Key key = kb.btnList[i];
                float width = keyWidth;
                float height = keyHeight;
                switch (key.size)
                {
                    case keySize.x1:
                        break;
                    case keySize.x125:
                        width *= 1.25f;
                        break;
                    case keySize.x15:
                        width *= 1.5f;
                        break;
                    case keySize.x2:
                        width *= 2f;
                        break;
                    case keySize.x2H:
                        height *= 2f;
                        break;
                    case keySize.x25:
                        width *= 2.5f;
                        break;
                    case keySize.x55:
                        width *= 5.5f;
                        break;
                    default:
                        WarningForm.Instance.OpenUI("Key Size ERROR : " + key.size.ToString(), false);
                        break;
                }
                //----------------------------------------------------------------------
                SolidColorBrush solidColorBrush = Dx2D.Instance.GetSolidColorBrush(XmlUI.DxUIClickColor);
                if (moveKey == key.Hex)
                    solidColorBrush.Color = XmlUI.DxUIClickColor;
                else
                    solidColorBrush.Color = XmlUI.DxUIBackColor;
                Dx2D.Instance.RenderTarget2D.DrawRectangle(key.OuterFrame, solidColorBrush, edgeWidth);
                solidColorBrush.Color = XmlUI.DxApplyColor;
                #region 选中
                if ((Fun & 0x01) == 0x01 && key.Hex == 0xE0)
                {
                    Dx2D.Instance.RenderTarget2D.FillRectangle(key.InnerFrame, solidColorBrush);
                    solidColorBrush.Color = XmlUI.DxBackColor;
                }
                else if ((Fun & 0x02) == 0x02 && key.Hex == 0xE1)
                {
                    Dx2D.Instance.RenderTarget2D.FillRectangle(key.InnerFrame, solidColorBrush);
                    solidColorBrush.Color = XmlUI.DxBackColor;
                }
                else if ((Fun & 0x04) == 0x04 && key.Hex == 0xE2)
                {
                    Dx2D.Instance.RenderTarget2D.FillRectangle(key.InnerFrame, solidColorBrush);
                    solidColorBrush.Color = XmlUI.DxBackColor;
                }
                else if ((Fun & 0x08) == 0x08 && key.Hex == 0xE3)
                {
                    Dx2D.Instance.RenderTarget2D.FillRectangle(key.InnerFrame, solidColorBrush);
                    solidColorBrush.Color = XmlUI.DxBackColor;
                }
                else if ((Fun & 0x10) == 0x10 && key.Hex == 0xE4)
                {
                    Dx2D.Instance.RenderTarget2D.FillRectangle(key.InnerFrame, solidColorBrush);
                    solidColorBrush.Color = XmlUI.DxBackColor;
                }
                else if ((Fun & 0x20) == 0x20 && key.Hex == 0xE5)
                {
                    Dx2D.Instance.RenderTarget2D.FillRectangle(key.InnerFrame, solidColorBrush);
                    solidColorBrush.Color = XmlUI.DxBackColor;
                }
                else if ((Fun & 0x40) == 0x40 && key.Hex == 0xE6)
                {
                    Dx2D.Instance.RenderTarget2D.FillRectangle(key.InnerFrame, solidColorBrush);
                    solidColorBrush.Color = XmlUI.DxBackColor;
                }
                else if ((Fun & 0x80) == 0x80 && key.Hex == 0xE7)
                {
                    Dx2D.Instance.RenderTarget2D.FillRectangle(key.InnerFrame, solidColorBrush);
                    solidColorBrush.Color = XmlUI.DxBackColor;
                }
                else if (Code == key.Hex)
                {
                    Dx2D.Instance.RenderTarget2D.FillRectangle(key.InnerFrame, solidColorBrush);
                    solidColorBrush.Color = XmlUI.DxBackColor;
                }
                else
                {
                    solidColorBrush.Color = XmlUI.DxDefaultColor;
                }
                Dx2D.Instance.RenderTarget2D.DrawText(key.drawText, key.tf, key.TextFrame, solidColorBrush);
                #endregion
            }
            #endregion
        }
        private void OnButtonClick(object sender, EventArgs e)
        {

        }
    }
}
