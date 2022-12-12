using SharpDX;
using System.Windows.Forms;

namespace EasyControl
{
    public class uiPlaceholder : iControl
    {
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
        /////////////////////////////////////////////////////////////////////////////////////

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
        }
    }
}
