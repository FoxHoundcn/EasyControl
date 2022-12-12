using SharpDX;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace EasyControl
{
    public class SplitControl : iControl
    {
        static int maxIndex = 0;
        //---------------------------------------------------------
        public bool NodeLinkMode { get; private set; }
        public iControl Parent { get; set; }
        public bool Hide { get; set; } = false;
        public int Index { get; private set; }
        public string Name { get; set; }
        public RectangleF DrawRect { get { return Rect; } }
        public RectangleF Rect { private get; set; }
        public Vector2 _offset = new Vector2();
        public Vector2 Offset
        {
            get { return _offset; }
            set
            {
                _offset = value;
                for (int i = 0; i < controlList.Count; i++)
                {
                    controlList[i].Offset = value;
                }
            }
        }
        public string PluginID { get; set; }
        public string UIKey { set; private get; }
        //---------------------------------------------------------------------------------
        public OrientationType oriType { get; private set; }
        public List<LayoutControl> controlList { get; private set; } = new List<LayoutControl>();
        public List<float> minList = new List<float>();
        public List<float> sizeList = new List<float>();
        private bool move = false;
        private const int splitWidth = 4;
        /////////////////////////////////////////////////////////////////////////
        public SplitControl(OrientationType _oriType, LayoutControl _controlList0, LayoutControl _controlList1, float _min0, float _min1)
        {
            Index = maxIndex;
            Name = "SplitControl" + maxIndex;
            maxIndex++;
            oriType = _oriType;
            if (_oriType == OrientationType.Object)
                throw new Exception("new LayoutControl oriType Error !!!");
            controlList.Add(_controlList0);
            _controlList0.Parent = this;
            controlList.Add(_controlList1);
            _controlList1.Parent = this;
            minList.Add(_min0);
            minList.Add(_min1);
            sizeList.Add(_min0);
            sizeList.Add(_min1);
        }
        #region 鼠标操作
        public void JoyMouseDownEvent(MouseEventArgs e)
        {
            if (Hide) return;
            RectangleF rectSplit = new RectangleF();
            switch (oriType)
            {
                case OrientationType.Horizontal:
                    rectSplit.X = Rect.X + Offset.X + controlList[0].DrawRect.Width;
                    rectSplit.Y = Rect.Y + Offset.Y;
                    rectSplit.Width = 4;
                    rectSplit.Height = Rect.Height;
                    break;
                case OrientationType.Vertical:
                    rectSplit.X = Rect.X + Offset.X;
                    rectSplit.Y = Rect.Y + Offset.Y + controlList[0].DrawRect.Height;
                    rectSplit.Width = Rect.Width;
                    rectSplit.Height = 4;
                    break;
                case OrientationType.Object:
                    throw new Exception("SplitControl OrientationType No Object !!!");
                default:
                    throw new Exception("SplitControl OrientationType Error !!!");
            }
            if (e.X >= rectSplit.X && e.X < rectSplit.X + rectSplit.Width &&
                e.Y >= rectSplit.Y && e.Y < rectSplit.Y + rectSplit.Height)
            {
                move = true;
            }
            controlList[0].JoyMouseDownEvent(e);
            controlList[1].JoyMouseDownEvent(e);
        }
        public void JoyMouseUpEvent(MouseEventArgs e)
        {
            if (Hide) return;
            move = false;
            controlList[0].JoyMouseUpEvent(e);
            controlList[1].JoyMouseUpEvent(e);
        }
        public void JoyMouseMoveEvent(MouseEventArgs e)
        {
            if (Hide) return;
            if (move)
            {
                float width = Rect.Width;
                float height = Rect.Height;
                float minSize = 0;
                for (int i = 0; i < minList.Count; i++)
                {
                    minSize += minList[i];
                }
                switch (oriType)
                {
                    case OrientationType.Horizontal:
                        sizeList[0] = PublicData.MouseX - Rect.X - Offset.X - (splitWidth / 2);
                        width -= splitWidth;
                        float scaleH = width / minSize;         //显示比例
                        if (scaleH > 1f)
                            scaleH = 1f;
                        if (sizeList[0] < minList[0] * scaleH)
                        {
                            sizeList[0] = minList[0] * scaleH;
                        }
                        break;
                    case OrientationType.Vertical:
                        sizeList[0] = PublicData.MouseY - Rect.Y - Offset.Y - (splitWidth / 2);
                        height -= splitWidth;
                        float scaleV = height / minSize;         //显示比例
                        if (scaleV > 1f)
                            scaleV = 1f;
                        if (sizeList[0] < minList[0] * scaleV)
                        {
                            sizeList[0] = minList[0] * scaleV;
                        }
                        break;
                    case OrientationType.Object:
                        throw new Exception("SplitControl OrientationType No Object !!!");
                    default:
                        throw new Exception("SplitControl OrientationType Error !!!");
                }
            }
            controlList[0].JoyMouseMoveEvent(e);
            controlList[1].JoyMouseMoveEvent(e);
        }
        public void JoyMouseMoveWheel(MouseEventArgs e)
        {
            if (Hide) return;
            controlList[0].JoyMouseMoveWheel(e);
            controlList[1].JoyMouseMoveWheel(e);
        }
        #endregion
        public void Dx2DResize()
        {
            if (Hide) return;
            controlList[0].Dx2DResize();
            controlList[1].Dx2DResize();
        }
        public void DxRenderLogic()
        {
            if (Hide) return;
            float x = Rect.X;
            float y = Rect.Y;
            float width = Rect.Width;
            float height = Rect.Height;
            float minSize = 0;
            float currentSize = 0;
            for (int i = 0; i < minList.Count; i++)
            {
                minSize += minList[i];
                currentSize += sizeList[i];
            }
            switch (oriType)
            {
                case OrientationType.Horizontal:
                    width -= splitWidth;
                    float scaleH = width / minSize;         //显示比例
                    if (scaleH > 1f)
                        scaleH = 1f;
                    if (currentSize > width)
                    {
                        float sizeHcut = currentSize - width;
                        if (sizeList[1] - minList[1] * scaleH > sizeHcut)
                        {
                            sizeList[1] -= sizeHcut;
                        }
                        else
                        {
                            sizeList[1] = minList[1] * scaleH;
                            sizeHcut -= sizeList[1] - minList[1] * scaleH;
                            sizeList[0] -= sizeHcut;
                        }
                    }
                    else
                    {
                        sizeList[1] += width - currentSize;
                    }
                    if (sizeList[0] < minList[0] * scaleH)
                    {
                        float set0 = minList[0] * scaleH - sizeList[0];
                        sizeList[0] += set0;
                        sizeList[1] -= set0;
                    }
                    else if (sizeList[1] < minList[1] * scaleH)
                    {
                        float set1 = minList[1] * scaleH - sizeList[1];
                        sizeList[0] -= set1;
                        sizeList[1] += set1;
                    }
                    controlList[0].Rect = new RectangleF(x, y, sizeList[0], height);
                    controlList[0].DxRenderLogic();
                    controlList[1].Rect = new RectangleF(x + sizeList[0] + splitWidth, y, sizeList[1], height);
                    controlList[1].DxRenderLogic();
                    break;
                case OrientationType.Vertical:
                    height -= splitWidth;
                    float scaleV = height / minSize;         //显示比例
                    if (scaleV > 1f)
                        scaleV = 1f;
                    if (currentSize > height)
                    {
                        float sizeVcut = currentSize - height;
                        if (sizeList[1] - minList[1] * scaleV > sizeVcut)
                        {
                            sizeList[1] -= sizeVcut;
                        }
                        else
                        {
                            sizeList[1] = minList[1] * scaleV;
                            sizeVcut -= sizeList[1] - minList[1] * scaleV;
                            sizeList[0] -= sizeVcut;
                        }
                    }
                    else
                    {
                        sizeList[1] += height - currentSize;
                    }
                    if (sizeList[0] < minList[0] * scaleV)
                    {
                        float set0 = minList[0] * scaleV - sizeList[0];
                        sizeList[0] += set0;
                        sizeList[1] -= set0;
                    }
                    else if (sizeList[1] < minList[1] * scaleV)
                    {
                        float set1 = minList[1] * scaleV - sizeList[1];
                        sizeList[0] -= set1;
                        sizeList[1] += set1;
                    }
                    controlList[0].Rect = new RectangleF(x, y, width, sizeList[0]);
                    controlList[0].DxRenderLogic();
                    controlList[1].Rect = new RectangleF(x, y + sizeList[0] + splitWidth, width, sizeList[1]);
                    controlList[1].DxRenderLogic();
                    break;
                case OrientationType.Object:
                    throw new Exception("SplitControl OrientationType No Object !!!");
                default:
                    throw new Exception("SplitControl OrientationType Error !!!");
            }
        }

        public void DxRenderHigh()
        {
            if (Hide) return;
            controlList[0].DxRenderHigh();
            controlList[1].DxRenderHigh();
        }

        public void DxRenderMedium()
        {
            if (Hide) return;
            RectangleF rectSplit = new RectangleF();
            bool Hline = true;
            switch (oriType)
            {
                case OrientationType.Horizontal:
                    rectSplit.X = Rect.X + controlList[0].DrawRect.Width;
                    rectSplit.Y = Rect.Y;
                    rectSplit.Width = splitWidth;
                    rectSplit.Height = Rect.Height;
                    Hline = true;
                    break;
                case OrientationType.Vertical:
                    rectSplit.X = Rect.X;
                    rectSplit.Y = Rect.Y + controlList[0].DrawRect.Height;
                    rectSplit.Width = Rect.Width;
                    rectSplit.Height = splitWidth;
                    Hline = false;
                    break;
                case OrientationType.Object:
                    throw new Exception("SplitControl OrientationType No Object !!!");
                default:
                    throw new Exception("SplitControl OrientationType Error !!!");
            }
            Dx2D.Instance.RenderTarget2D.FillRectangle(rectSplit, Dx2D.Instance.GetSolidColorBrush(XmlUI.DxDeviceYellow));
            if (Hline)
                Dx2D.Instance.RenderTarget2D.DrawLine(new Vector2(rectSplit.X + (splitWidth / 2), rectSplit.Y + (splitWidth / 2)),
                    new Vector2(rectSplit.X + (splitWidth / 2), rectSplit.Y + rectSplit.Height - (splitWidth / 2)), Dx2D.Instance.GetSolidColorBrush(Color.Black));
            else
                Dx2D.Instance.RenderTarget2D.DrawLine(new Vector2(rectSplit.X + (splitWidth / 2), rectSplit.Y + (splitWidth / 2)),
                    new Vector2(rectSplit.X + rectSplit.Width - (splitWidth / 2), rectSplit.Y + (splitWidth / 2)), Dx2D.Instance.GetSolidColorBrush(Color.Black));
            controlList[0].DxRenderMedium();
            controlList[1].DxRenderMedium();
        }

        public void DxRenderLow()
        {
            if (Hide) return;
            controlList[0].DxRenderLow();
            controlList[1].DxRenderLow();
        }
    }
}
