using SharpDX;
using SharpDX.Direct2D1;
using System;
using System.Windows.Forms;

namespace EasyControl
{
    public class uiImage : iControl
    {
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
        public float Opacity = 1f;
        public float AspectRatio = -1f;
        public float ImageOffset = 0.5f;
        Bitmap bitmap;
        public bool bClick { get; set; }
        public bool mouseEnter { get; private set; } = false;
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
        public EventHandler LeftButtonClick;
        public EventHandler RightButtonClick;
        #endregion
        //////////////////////////////////////////////////////////////////////////////////
        public uiImage(string _path, string _format, bool _click)
        {
            System.Drawing.Imaging.ImageFormat iformat;
            switch (_format.ToLower())
            {
                case "bmp":
                    iformat = System.Drawing.Imaging.ImageFormat.Bmp;
                    break;
                case "jpg":
                    iformat = System.Drawing.Imaging.ImageFormat.Jpeg;
                    break;
                case "png":
                    iformat = System.Drawing.Imaging.ImageFormat.Png;
                    break;
                default:
                    throw new Exception("Image Parameter : \"_ImageFormat Type\" ERROR !!!");
            }
            bitmap = Dx2D.Instance.LoadBitmap(System.Environment.CurrentDirectory + @"\Image\" + _path, iformat);
            bClick = _click;
            Dx2DResize();
            LeftButtonClick += doNothing;
            RightButtonClick += doNothing;
        }
        public void ChangeBitmap(string _path, string _format)
        {
            System.Drawing.Imaging.ImageFormat iformat;
            switch (_format.ToLower())
            {
                case "bmp":
                    iformat = System.Drawing.Imaging.ImageFormat.Bmp;
                    break;
                case "jpg":
                    iformat = System.Drawing.Imaging.ImageFormat.Jpeg;
                    break;
                case "png":
                    iformat = System.Drawing.Imaging.ImageFormat.Png;
                    break;
                default:
                    throw new Exception("Image Parameter : \"_ImageFormat Type\" ERROR !!!");
            }
            bitmap = Dx2D.Instance.LoadBitmap(System.Environment.CurrentDirectory + @"\Image\" + _path, iformat);
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
        public void Dx2DResize()
        {
        }
        public void DxRenderLogic()
        {
            if (Hide) return;
            edgeWidth = (int)(Math.Min(Rect.Height, Rect.Width) * 0.05f);
            drawRect = PublicData.GetActualRange(Rect, edgeWidth);
        }

        public void DxRenderHigh()
        {
            if (Hide) return;
        }

        public void DxRenderMedium()
        {
            if (Hide) return;
            RectangleF rect = new RectangleF();
            if (ImageOffset < 0f)
                ImageOffset = 0f;
            if (ImageOffset > 1f)
                ImageOffset = 1f;
            if (Rect.Width / Rect.Height > AspectRatio)
            {
                rect.Height = Rect.Height;
                rect.Width = Rect.Height * AspectRatio;
                rect.X = Rect.X + (Rect.Width - rect.Width) * ImageOffset;
                rect.Y = Rect.Y;
            }
            else
            {
                rect.Width = Rect.Width;
                rect.Height = Rect.Height / AspectRatio * (Rect.Width / Rect.Height);
                rect.X = Rect.X;
                rect.Y = Rect.Y + (Rect.Height - rect.Height) * ImageOffset;
            }
            Dx2D.Instance.RenderTarget2D.DrawBitmap(bitmap, rect, Opacity, BitmapInterpolationMode.Linear);
            if (mouseEnter && bClick)
            {
                Dx2D.Instance.RenderTarget2D.DrawRectangle(drawRect, Dx2D.Instance.GetSolidColorBrush(XmlUI.DxUIClickColor), edgeWidth);
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
