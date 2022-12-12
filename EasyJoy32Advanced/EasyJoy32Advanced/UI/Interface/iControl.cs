using SharpDX;
using System.Windows.Forms;

namespace EasyControl
{
    public interface iControl
    {
        iControl Parent { get; set; }
        RectangleF DrawRect { get; }//显示范围
        RectangleF Rect { set; }//显示范围
        Vector2 Offset { get; set; }//偏移
        bool Hide { get; set; }//隐藏，用于上级界面隐藏
        int Index { get; }//序号
        string Name { get; }//名称
        string UIKey { set; }//xml识别UI的key
        string PluginID { get; }//插件ID信息
        bool NodeLinkMode { get; }//是否在连线视图中
        //-------------------------------------------------
        void Dx2DResize();
        //-------------------------------------------------
        void DxRenderLogic();
        void DxRenderHigh();
        void DxRenderMedium();
        void DxRenderLow();
        //-------------------------------------------------
        void JoyMouseDownEvent(MouseEventArgs e);
        void JoyMouseUpEvent(MouseEventArgs e);
        void JoyMouseMoveEvent(MouseEventArgs e);
        void JoyMouseMoveWheel(MouseEventArgs e);
    }
}
