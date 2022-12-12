using ControllorPlugin;
using SharpDX;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;

namespace EasyControl
{
    public class uiNode : iControl
    {
        private const float AddWidth = 16f;
        private const float AddHeight = 8f;
        //------------------------------------------------------------
        public bool NodeLinkMode { get { return true; } }
        private iControl _Parent = null;
        public iControl Parent
        {
            get { return _Parent; }
            set
            {
                _Parent = value;
                for (int i = 0; i < portList.Count; i++)
                {
                    portList[i].Parent = value;
                }
                for (int i = 0; i < debugTextList.Count; i++)
                {
                    debugTextList[i].Parent = value;
                }
            }
        }
        public RectangleF DrawRect { get { return Rect; } }
        public RectangleF Rect { private get; set; }
        private Vector2 _offset = new Vector2();
        public Vector2 Offset
        {
            get
            {
                return _offset;
            }
            set
            {
                _offset = value;
            }
        }
        public bool Hide { get; set; } = false;
        public int Index { get; private set; }
        public string Name { get { if (parentNode != null) return parentNode.Name; else return ""; } }
        public string PluginID { get; set; }
        public string UIKey { set; private get; }
        //---------------------------------------------------------------------------------
        public Vector2 sourceOffset = new Vector2();
        public bool mouseEnter { get; private set; } = false;
        public bool mouseDown { get; private set; } = false;
        public int sourceX = 0;
        public int sourceY = 0;
        System.Drawing.SizeF fontSize;
        public List<uiPort> portList { private set; get; } = new List<uiPort>();
        uiTextLable nameText;
        LayoutControl lcMain = new LayoutControl(OrientationType.Vertical);
        List<LayoutControl> lcTextList = new List<LayoutControl>();
        //List<uiTextLable> debugTextList = new List<uiTextLable>();
        List<uiTextEditor> debugTextList = new List<uiTextEditor>();
        public Node parentNode { get; private set; } = null;
        public InterfacePlugin parentInterfacePlugin { get; private set; } = null;
        private bool bClose;
        /////////////////////////////////////////////////////////////////////////////////////
        public uiNode(string _ID, int _index, InterfacePlugin _ip, Node _node, bool close)
        {
            Index = _index;
            PluginID = _ID;
            if (_node == null)
                throw new Win32Exception("New uiNode error !!! Node is null.");
            parentNode = _node;
            if (_ip == null)
                throw new Win32Exception("New uiNode error !!! InterfacePlugin is null.");
            parentInterfacePlugin = _ip;
            LayoutControl layoutName = new LayoutControl(OrientationType.Horizontal);
            nameText = new uiTextLable(Name, XmlUI.DxTextColor, XmlUI.DxUIBackColor, JoyConst.MaxFontSize, true, false);
            nameText.textAlignment = SharpDX.DirectWrite.TextAlignment.Center;
            nameText.FontRatio = 0.8f;
            uiImage btnClose = new uiImage("close.png", "PNG", true);
            LayoutControl layoutClose = new LayoutControl(OrientationType.Horizontal);
            layoutClose.AspectRatio = 1f;
            layoutClose.AddObject(new LayoutControl(btnClose));
            layoutName.AddObject(new LayoutControl(nameText));
            bClose = close;
            if (bClose)
                layoutName.AddObject(layoutClose);
            lcMain.AddObject(layoutName);
            Rect = new RectangleF();
            Hide = false;
            if (parentNode.NodePortList == null || parentNode.NodePortList.Count <= 0)
                throw new Win32Exception("New uiNode error !!! NodePortList is null.");
            for (int i = 0; i < parentNode.NodePortList.Count; i++)
            {
                Color4 color = XmlUI.DxTextColor;
                switch (parentNode.NodePortList[i].Type)
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
                LayoutControl lcPortH = new LayoutControl(OrientationType.Horizontal);
                uiTextLable textLable = new uiTextLable(parentNode.NodePortList[i].Name, color, XmlUI.DxUIBackColor, JoyConst.MaxFontSize, true, false);
                uiPort port = new uiPort(PluginID, Index, i, parentNode.NodePortList[i]);
                port.Parent = this;
                port.parentNode = this;
                portList.Add(port);
                LayoutControl lcPort = new LayoutControl(port);
                lcPort.AspectRatio = 1f;
                switch (parentNode.NodePortList[i].IO)
                {
                    case PortType.In:
                        lcPortH.AddObject(lcPort);
                        textLable.FontRatio = 0.8f;
                        textLable.textAlignment = SharpDX.DirectWrite.TextAlignment.Leading;
                        lcTextList.Add(new LayoutControl(textLable));
                        lcPortH.AddObject(lcTextList[i]);
                        break;
                    case PortType.Out:
                        textLable.FontRatio = 0.8f;
                        textLable.textAlignment = SharpDX.DirectWrite.TextAlignment.Trailing;
                        lcTextList.Add(new LayoutControl(textLable));
                        lcPortH.AddObject(lcTextList[i]);
                        lcPortH.AddObject(lcPort);
                        break;
                }
                LayoutControl lcPortV = new LayoutControl(OrientationType.Vertical);
                lcPortV.AddObject(lcPortH);
                uiTextEditor debugText = new uiTextEditor("", true);
                debugText.Parent = this;
                debugText.Index = i;
                debugText.TextChange += OnDebugTextChange;
                //uiTextLable debugText = new uiTextLable(parentNode.NodePortList[i].Name, color, XmlUI.DxUIBackColor, JoyConst.MaxFontSize, true, false);
                debugTextList.Add(debugText);
                lcPortV.AddObject(new LayoutControl(debugText, 0.8f));
                lcMain.AddObject(lcPortV);
            }
            //-------------------------------------------
        }
        private void OnDebugTextChange(object sender, EventArgs e)
        {
            uiTextEditor te = (uiTextEditor)sender;
            int index = te.Index;
            string txt = te.Text;
            switch (parentNode.NodePortList[index].Type)
            {
                case PortValue.Int64:
                    int valueInt;
                    if (int.TryParse(txt, out valueInt))
                    {
                        portList[index].nodePort.ValueInt64 = valueInt;
                    }
                    break;
                case PortValue.Double:
                    float valueFloat;
                    if (float.TryParse(txt, out valueFloat))
                    {
                        portList[index].nodePort.ValueDouble = valueFloat;
                    }
                    break;
                case PortValue.String:
                    portList[index].nodePort.ValueString = txt;
                    break;
            }
        }
        public void Load(SaveNode node)
        {
            _offset.X = node.sourceOffsetX;
            _offset.Y = node.sourceOffsetY;
            if (node.portList.Count == portList.Count)
            {
                for (int i = 0; i < portList.Count; i++)
                {
                    portList[i].Load(node.portList[i]);
                }
            }
        }
        public void Dx2DResize()
        {
            if (Hide) return;
            float maxWidth = 0;
            float maxHeight = 0;
            string name = Name.Replace('\n', ' ');
            nameText.Text = name;
            fontSize = Dx2D.Instance.MeasureString(name, Dx2D.nodeTextFormat);
            if (fontSize.Width + AddWidth > maxWidth)
            {
                maxWidth = fontSize.Width + AddWidth;
                maxHeight += fontSize.Height + AddHeight;
            }
            for (int i = 0; i < parentNode.NodePortList.Count; i++)
            {
                System.Drawing.SizeF portSize = Dx2D.Instance.MeasureString(parentNode.NodePortList[i].Name, Dx2D.nodeTextFormat);
                if (portSize.Width + AddWidth > maxWidth)
                    maxWidth = portSize.Width + AddWidth;
                maxHeight += portSize.Height + AddHeight;
            }
            maxWidth += fontSize.Height * 2f;
            if (maxWidth > JoyConst.MaxNodeWidth)
                maxWidth = JoyConst.MaxNodeWidth;
            if (maxWidth < JoyConst.MaxNodeWidth / 2)
                maxWidth = JoyConst.MaxNodeWidth / 2;
            Rect = new RectangleF(0, 0, maxWidth, PublicData.PluginDebug ? maxHeight * 1.8f : maxHeight);
            for (int i = 0; i < lcTextList.Count; i++)
            {
                lcTextList[i].Placeholder = Rect.Width / fontSize.Height - 1f;
            }
            lcMain.Dx2DResize();
        }
        public void portValueChange(int portIndex)
        {
            parentInterfacePlugin.ValueChangeEvent(Index, portIndex);
        }
        #region 渲染
        public void DxRenderLogic()
        {
            if (Hide) return;
            for (int i = 0; i < parentNode.NodePortList.Count; i++)
            {
                if (!PublicData.PluginDebug)
                {
                    debugTextList[i].Hide = true;
                }
                else
                {
                    debugTextList[i].Hide = false;
                    uiPort port = portList[i];
                    switch (port.nodePort.Type)
                    {
                        case PortValue.Int64:
                            if (!debugTextList[i].Text.Equals(port.nodePort.ValueInt64.ToString()))
                                debugTextList[i].Text = port.nodePort.ValueInt64.ToString();
                            break;
                        case PortValue.Double:
                            if (!debugTextList[i].Text.Equals(port.nodePort.ValueDouble.ToString("f4")))
                                debugTextList[i].Text = port.nodePort.ValueDouble.ToString("f4");
                            break;
                        case PortValue.String:
                            if (!debugTextList[i].Text.Equals(port.nodePort.ValueString))
                                debugTextList[i].Text = port.nodePort.ValueString;
                            break;
                    }
                    //switch (port.nodePort.IO)
                    //{
                    //    case PortType.In:
                    //        debugTextList[i].textAlignment = SharpDX.DirectWrite.TextAlignment.Leading;
                    //        break;
                    //    case PortType.Out:
                    //        debugTextList[i].textAlignment = SharpDX.DirectWrite.TextAlignment.Trailing;
                    //        break;
                    //}
                }
            }
            lcMain.Rect = new RectangleF(Offset.X, Offset.Y, Rect.Width, Rect.Height);
            lcMain.DxRenderLogic();
        }
        public void DxRenderLow()
        {
            if (Hide) return;
            lcMain.DxRenderLow();
        }
        public void DxRenderMedium()
        {
            if (Hide) return;
            if (mouseEnter)
                Dx2D.Instance.RenderTarget2D.DrawRectangle(new RectangleF(Rect.X + Offset.X, Rect.Y + Offset.Y, Rect.Width, Rect.Height), Dx2D.Instance.GetSolidColorBrush(XmlUI.DxUIClickColor), 2f);
            else
                Dx2D.Instance.RenderTarget2D.DrawRectangle(new RectangleF(Rect.X + Offset.X, Rect.Y + Offset.Y, Rect.Width, Rect.Height), Dx2D.Instance.GetSolidColorBrush(XmlUI.DxUIBackColor), 2f);
            lcMain.DxRenderMedium();
        }
        public void DxRenderHigh()
        {
            if (Hide) return;
            lcMain.DxRenderHigh();
        }
        #endregion
        #region 鼠标
        public void SetPosition(int x, int y)
        {
            _offset = new Vector2(sourceOffset.X - (sourceX - x) / NodeLinkControl.Instance.ScalingValue,
                sourceOffset.Y - (sourceY - y) / NodeLinkControl.Instance.ScalingValue);
        }
        public void JoyMouseMoveEvent(MouseEventArgs e)
        {
            if (Hide)
            {
                mouseEnter = false;
                return;
            }
            if (e.X >= (Rect.X + Offset.X) * NodeLinkControl.Instance.ScalingValue + Parent.Offset.X &&
                e.X < (Rect.X + Offset.X + Rect.Width) * NodeLinkControl.Instance.ScalingValue + Parent.Offset.X &&
                e.Y >= (Rect.Y + Offset.Y) * NodeLinkControl.Instance.ScalingValue + Parent.Offset.Y &&
                e.Y < (Rect.Y + Offset.Y + Rect.Height) * NodeLinkControl.Instance.ScalingValue + Parent.Offset.Y)
            {
                mouseEnter = true;
            }
            else
            {
                mouseEnter = false;
            }
            if (mouseDown)
            {
                _offset = new Vector2(sourceOffset.X - (sourceX - e.X) / NodeLinkControl.Instance.ScalingValue,
                    sourceOffset.Y - (sourceY - e.Y) / NodeLinkControl.Instance.ScalingValue);
            }
            lcMain.JoyMouseMoveEvent(e);
        }
        public void JoyMouseDownEvent(MouseEventArgs e)
        {
            if (Hide)
            {
                return;
            }
            float topTextHeight = PublicData.PluginDebug ? (fontSize.Height + AddHeight) * 1.8f : fontSize.Height + AddHeight;
            if (bClose &&
                e.X >= (Rect.X + Offset.X + Rect.Width - topTextHeight) * NodeLinkControl.Instance.ScalingValue + Parent.Offset.X &&
                e.X < (Rect.X + Offset.X + Rect.Width) * NodeLinkControl.Instance.ScalingValue + Parent.Offset.X &&
                e.Y >= (Rect.Y + Offset.Y) * NodeLinkControl.Instance.ScalingValue + Parent.Offset.Y &&
                e.Y < (Rect.Y + Offset.Y + topTextHeight) * NodeLinkControl.Instance.ScalingValue + Parent.Offset.Y &&
                e.Button == MouseButtons.Right)
            {
                parentNode.Open = false;
                parentInterfacePlugin.NodeCloseEvent(Index);
            }
            if (e.X >= (Rect.X + Offset.X) * NodeLinkControl.Instance.ScalingValue + Parent.Offset.X &&
                e.X < (Rect.X + Offset.X + Rect.Width) * NodeLinkControl.Instance.ScalingValue + Parent.Offset.X &&
                e.Y >= (Rect.Y + Offset.Y) * NodeLinkControl.Instance.ScalingValue + Parent.Offset.Y &&
                e.Y < (Rect.Y + Offset.Y + topTextHeight) * NodeLinkControl.Instance.ScalingValue + Parent.Offset.Y &&
               PublicData.MoveNode == null && e.Button == MouseButtons.Left)
            {
                sourceOffset = _offset;
                sourceX = e.X;
                sourceY = e.Y;
                mouseDown = true;
                PublicData.MoveNode = this;
            }
            lcMain.JoyMouseDownEvent(e);
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
            PublicData.MoveNode = null;
            lcMain.JoyMouseUpEvent(e);
        }
        public void JoyMouseMoveWheel(MouseEventArgs e)
        {
            lcMain.JoyMouseMoveWheel(e);
        }
        #endregion
    }
}
