using ControllorPlugin;
using SharpDX;
using SharpDX.Direct2D1;
using System.Collections.Generic;
using System.Windows.Forms;

namespace EasyControl
{
    public class uiPort : iControl
    {
        public bool NodeLinkMode { get { return true; } }
        public iControl Parent { get; set; }
        public RectangleF DrawRect { get { return Rect; } }
        public RectangleF Rect { private get; set; }
        public Vector2 Offset { get; set; }
        public bool Hide { get; set; } = false;
        public int Index { get; private set; }
        public string Name { get; set; }
        public string PluginID { get; set; }
        public string UIKey { set; private get; }
        //--------------------------------------------------------------------------------
        public string Text { get { return Name; } set { Name = value; } }
        public bool mouseEnter { get; private set; } = false;
        public bool mouseDown { get; private set; } = false;
        public Ellipse _portRect = new Ellipse();
        public Ellipse portRect { get { return _portRect; } private set { _portRect = value; } }
        public NodePort nodePort { private set; get; }//IO方向; 
        public uiNode parentNode;
        public int NodeIndex { get; private set; }
        public Dictionary<string, NodePortLink> portLinkList = new Dictionary<string, NodePortLink>();
        //////////////////////////////////////////////////////////////////////////////////
        public uiPort(string _key, int _nodeIndex, int _index, NodePort _port)
        {
            Index = _index;
            PluginID = _key;
            NodeIndex = _nodeIndex;
            Name = "jPort" + Index;
            nodePort = _port;
            Rect = new RectangleF();
        }
        public void Load(SavePort port)
        {
            portLinkList.Clear();
            foreach (var item in port.portLinkList)
            {
                if (portLinkList.ContainsKey(item.Key))
                {
                    portLinkList[item.Key] = item.Value;
                }
                else
                {
                    portLinkList.Add(item.Key, item.Value);
                }
            }
        }
        public bool InSide(MouseEventArgs e)
        {
            if (e.X >= (Rect.X + Offset.X) * NodeLinkControl.Instance.ScalingValue + Parent.Offset.X &&
                e.X < (Rect.X + Offset.X + Rect.Width) * NodeLinkControl.Instance.ScalingValue + Parent.Offset.X &&
                e.Y >= (Rect.Y + Offset.Y) * NodeLinkControl.Instance.ScalingValue + Parent.Offset.Y &&
                e.Y < (Rect.Y + Offset.Y + Rect.Height) * NodeLinkControl.Instance.ScalingValue + Parent.Offset.Y)
            {
                return true;
            }
            return false;
        }
        public void ClearNodePortLink()
        {
            portLinkList.Clear();
        }
        public bool AddNodePortLink(NodePortLink newOne)
        {
            if (!portLinkList.ContainsKey(newOne.Key))
            {
                portLinkList.Add(newOne.Key, newOne);
                return true;
            }
            return false;
        }
        #region 鼠标
        public void JoyMouseMoveEvent(MouseEventArgs e)
        {
            if (Hide)
            {
                mouseEnter = false;
                return;
            }
            if (InSide(e))
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
            if (InSide(e))
            {
                switch (e.Button)
                {
                    case MouseButtons.Left:
                        mouseDown = true;
                        break;
                    case MouseButtons.Right:
                        ClearNodePortLink();
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
            if (mouseDown)
            {
                mouseDown = false;
            }
        }

        public void JoyMouseMoveWheel(MouseEventArgs e)
        {

        }
        #endregion

        public void Dx2DResize()
        {
        }
        public void DxRenderLogic()
        {
            if (Hide) return;
            float width = Rect.Width > Rect.Height ? Rect.Height : Rect.Width;
            _portRect.Point.X = Rect.X + Rect.Width / 2;
            _portRect.Point.Y = Rect.Y + Rect.Height / 2;
            _portRect.RadiusX = width * 0.3f;
            _portRect.RadiusY = width * 0.3f;
        }

        public void DxRenderHigh()
        {
            if (Hide) return;
        }

        public void DxRenderMedium()
        {
            if (Hide) return;
            if (mouseEnter)
            {
                Dx2D.Instance.RenderTarget2D.FillEllipse(portRect, Dx2D.Instance.GetSolidColorBrush(XmlUI.DxUIBackColor));
            }
            Color4 color = XmlUI.DxDeviceYellow;
            switch (nodePort.Type)
            {
                case PortValue.Int64:
                    color = XmlUI.DxDeviceBlue;
                    break;
                case PortValue.Double:
                    color = XmlUI.DxDeviceGreen;
                    break;
                case PortValue.String:
                    color = XmlUI.DxDeviceRed;
                    break;
            }
            Dx2D.Instance.RenderTarget2D.DrawEllipse(portRect, Dx2D.Instance.GetSolidColorBrush(color), portRect.RadiusX * 0.3f);
            switch (nodePort.IO)
            {
                case PortType.In:
                    Dx2D.Instance.RenderTarget2D.DrawLine(new Vector2(portRect.Point.X + portRect.RadiusX, portRect.Point.Y),
                        new Vector2(portRect.Point.X + portRect.RadiusX / 3f * 5f, portRect.Point.Y), Dx2D.Instance.GetSolidColorBrush(color), portRect.RadiusX * 0.3f);
                    break;
                case PortType.Out:
                    Dx2D.Instance.RenderTarget2D.DrawLine(new Vector2(portRect.Point.X - portRect.RadiusX, portRect.Point.Y),
                        new Vector2(portRect.Point.X - portRect.RadiusX / 3f * 5f, portRect.Point.Y), Dx2D.Instance.GetSolidColorBrush(color), portRect.RadiusX * 0.3f);
                    break;
            }
            if (mouseDown)
            {
                Ellipse eps = new Ellipse(portRect.Point, portRect.RadiusX * 0.5f, portRect.RadiusX * 0.5f);
                Dx2D.Instance.RenderTarget2D.FillEllipse(eps, Dx2D.Instance.GetSolidColorBrush(color));
            }
        }

        public void DxRenderLow()
        {
            if (Hide) return;
        }
    }
}
