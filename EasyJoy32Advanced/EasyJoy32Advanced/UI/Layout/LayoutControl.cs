using SharpDX;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace EasyControl
{
    public class LayoutControl : iControl
    {
        static int maxIndex = 0;
        //--------------------------------------------------------------
        public bool NodeLinkMode { get; private set; }
        public iControl Parent { get; set; }
        public bool Hide { get; set; } = false;
        public int Index { get; private set; }
        public string Name { get; set; }
        public RectangleF DrawRect { get; private set; }
        private RectangleF _rect;
        public RectangleF Rect
        {
            private get { return _rect; }
            set
            {
                float x = value.X;
                if (float.IsNaN(x) || float.IsInfinity(x))
                    x = 1;
                float y = value.Y;
                if (float.IsNaN(y) || float.IsInfinity(y))
                    y = 1;
                float w = value.Width;
                if (float.IsNaN(w) || float.IsInfinity(w))
                    w = 1;
                float h = value.Height;
                if (float.IsNaN(h) || float.IsInfinity(h))
                    h = 1;
                _rect = new RectangleF(x, y, w, h);
            }
        }
        public Vector2 _offset = new Vector2();
        public Vector2 Offset
        {
            get { return _offset; }
            set
            {
                _offset = value;
                switch (oriType)
                {
                    case OrientationType.Horizontal:
                    case OrientationType.Vertical:
                        for (int i = 0; i < controlList.Count; i++)
                        {
                            if (controlList[i] != null)
                                controlList[i].Offset = value;
                        }
                        break;
                    case OrientationType.Object:
                        if (uiObject != null)
                            uiObject.Offset = value;
                        break;
                    default:
                        break;
                }
            }
        }
        public string PluginID { get; set; }
        public string UIKey { set; private get; }
        //---------------------------------------------------------------------------------
        public OrientationType oriType { get; private set; }
        public List<LayoutControl> controlList { get; private set; } = new List<LayoutControl>();
        private iControl _uiObject = null;
        public iControl uiObject
        {
            get { return _uiObject; }
            private set
            {
                _uiObject = value;
                _uiObject.Parent = this;
            }
        }
        //----
        private float _Placeholder = 1f;
        public float Placeholder
        {
            get { return _Placeholder; }
            set
            {
                if (float.IsNaN(value) || float.IsInfinity(value))
                    _Placeholder = 1f;
                else
                    _Placeholder = value;
            }
        }
        public float minPlaceholder = -1;
        public float AspectRatio = -1f;
        public float maxWidth = -1f;
        public float maxHeight = -1f;
        public float maxPlaceholder { private set; get; } = 0;
        public string Info = "";
        public bool InfoLocalization = true;
        public Color4 BackColorLow = new Color4(0, 0, 0, 0);
        public Color4 BackColorMedium = new Color4(0, 0, 0, 0);
        public Color4 BackColorHigh = new Color4(0, 0, 0, 0);
        /////////////////////////////////////////////////////////////////////////
        public LayoutControl(float _placeholder = 1)
        {
            Index = maxIndex;
            Name = "LayoutControl" + maxIndex;
            maxIndex++;
            oriType = OrientationType.Object;
            if (_placeholder < 0.1f)
                Placeholder = 0.1f;
            else
                Placeholder = _placeholder;
            uiObject = new uiPlaceholder();
            Rect = new RectangleF();
        }
        public LayoutControl(OrientationType _oriType, float _placeholder = 1)
        {
            Index = maxIndex;
            Name = "LayoutControl" + maxIndex;
            maxIndex++;
            if (_oriType == OrientationType.Object)
                throw new Exception("new LayoutControl oriType Error !!!");
            oriType = _oriType;
            if (_placeholder < 0.1f)
                Placeholder = 0.1f;
            else
                Placeholder = _placeholder;
            Rect = new RectangleF();
        }
        public LayoutControl(iControl _object, float _placeholder = 1)
        {
            Index = maxIndex;
            Name = "LayoutControl" + maxIndex;
            maxIndex++;
            oriType = OrientationType.Object;
            if (_placeholder < 0.1f)
                Placeholder = 0.1f;
            else
                Placeholder = _placeholder;
            uiObject = _object;
            Rect = new RectangleF();
        }
        public void AddObject(LayoutControl obj)
        {
            controlList.Add(obj);
            obj.Parent = this;
        }
        private bool SetRect(LayoutControl lc)
        {
            switch (lc.oriType)
            {
                case OrientationType.Object:
                    if (lc.uiObject != null && !lc.uiObject.Hide)
                        return true;
                    break;
                case OrientationType.Vertical:
                case OrientationType.Horizontal:
                    if (!lc.Hide)
                        return true;
                    break;
            }
            return false;
        }
        #region 鼠标
        public void JoyMouseDownEvent(MouseEventArgs e)
        {
            if (Hide)
                return;
            switch (oriType)
            {
                case OrientationType.Horizontal:
                case OrientationType.Vertical:
                    if (controlList.Count >= 1)
                    {
                        List<LayoutControl> noSizeList = new List<LayoutControl>();
                        List<LayoutControl> allSizeList = new List<LayoutControl>();
                        float currentMaxPlaceholder = maxPlaceholder;
                        for (int i = 0; i < controlList.Count; i++)
                        {
                            LayoutControl obj = controlList[i];
                            if (obj != null)
                            {
                                obj.JoyMouseDownEvent(e);
                            }
                        }
                    }
                    break;
                case OrientationType.Object:
                    if (uiObject != null)
                    {
                        uiObject.JoyMouseDownEvent(e);
                    }
                    break;
            }
        }
        public bool InSide(int X, int Y)
        {
            if (X >= Rect.X + Offset.X && X < Rect.X + Offset.X + Rect.Width &&
                Y >= Rect.Y + Offset.Y && Y < Rect.Y + Offset.Y + Rect.Height)
            {
                return true;
            }
            return false;
        }
        public void JoyMouseMoveEvent(MouseEventArgs e)
        {
            if (Hide) return;
            if (InSide(e.X, e.Y))
            {
                if (!MainUI.Instance.InfoList.ContainsKey(this))
                {
                    MainUI.Instance.InfoList.Add(this, InfoLocalization ? Localization.Instance.GetLS(Info) : Info);
                }
            }
            switch (oriType)
            {
                case OrientationType.Horizontal:
                case OrientationType.Vertical:
                    if (controlList.Count >= 1)
                    {
                        for (int i = 0; i < controlList.Count; i++)
                        {
                            LayoutControl obj = controlList[i];
                            if (obj != null)
                            {
                                obj.JoyMouseMoveEvent(e);
                            }
                        }
                    }
                    break;
                case OrientationType.Object:
                    if (uiObject != null)
                    {
                        uiObject.JoyMouseMoveEvent(e);
                    }
                    break;
            }
        }
        public void JoyMouseUpEvent(MouseEventArgs e)
        {
            if (Hide)
                return;
            switch (oriType)
            {
                case OrientationType.Horizontal:
                case OrientationType.Vertical:
                    if (controlList.Count >= 1)
                    {
                        for (int i = 0; i < controlList.Count; i++)
                        {
                            LayoutControl obj = controlList[i];
                            if (obj != null)
                            {
                                obj.JoyMouseUpEvent(e);
                            }
                        }
                    }
                    break;
                case OrientationType.Object:
                    if (uiObject != null)
                    {
                        uiObject.JoyMouseUpEvent(e);
                    }
                    break;
            }
        }
        public void JoyMouseMoveWheel(MouseEventArgs e)
        {
            if (Hide)
                return;
            switch (oriType)
            {
                case OrientationType.Horizontal:
                case OrientationType.Vertical:
                    if (controlList.Count >= 1)
                    {
                        for (int i = 0; i < controlList.Count; i++)
                        {
                            LayoutControl obj = controlList[i];
                            if (obj != null)
                            {
                                obj.JoyMouseMoveWheel(e);
                            }
                        }
                    }
                    break;
                case OrientationType.Object:
                    if (uiObject != null)
                    {
                        uiObject.JoyMouseMoveWheel(e);
                    }
                    break;
            }
        }
        #endregion
        public void Dx2DResize()
        {
            if (Hide) return;
            switch (oriType)
            {
                case OrientationType.Horizontal:
                case OrientationType.Vertical:
                    if (controlList.Count >= 1)
                    {
                        for (int i = 0; i < controlList.Count; i++)
                        {
                            LayoutControl obj = controlList[i];
                            if (obj != null)
                            {
                                obj.Dx2DResize();
                            }
                        }
                    }
                    break;
                case OrientationType.Object:
                    if (uiObject != null)
                    {
                        uiObject.Dx2DResize();
                    }
                    break;
            }
        }
        public void DxRenderLogic()
        {
            if (Hide) return;
            maxPlaceholder = 0;
            for (int i = 0; i < controlList.Count; i++)
            {
                switch (controlList[i].oriType)
                {
                    case OrientationType.Object:
                        if (controlList[i].uiObject != null && !controlList[i].uiObject.Hide)
                            maxPlaceholder += controlList[i].Placeholder;
                        break;
                    case OrientationType.Vertical:
                    case OrientationType.Horizontal:
                        if (!controlList[i].Hide)
                            maxPlaceholder += controlList[i].Placeholder;
                        break;
                }
            }
            if (maxPlaceholder <= 0)
                maxPlaceholder = 1;
            if (minPlaceholder > 0 && maxPlaceholder < minPlaceholder)
                maxPlaceholder = minPlaceholder;
            float x = Rect.X;
            float y = Rect.Y;
            float width = Rect.Width;
            float height = Rect.Height;
            switch (oriType)
            {
                case OrientationType.Horizontal://横
                    if (controlList.Count >= 1)
                    {
                        List<LayoutControl> noSizeList = new List<LayoutControl>();
                        List<LayoutControl> allSizeList = new List<LayoutControl>();
                        float currentMaxPlaceholder = maxPlaceholder;
                        for (int i = 0; i < controlList.Count; i++)
                        {
                            LayoutControl obj = controlList[i];
                            if (obj != null)
                            {
                                if (SetRect(obj))
                                {
                                    bool setRect = false;
                                    float objWidth = Rect.Width / maxPlaceholder * obj.Placeholder;
                                    if (obj.AspectRatio >= 0f)
                                    {
                                        float arWidth = height * obj.AspectRatio;
                                        objWidth = arWidth < objWidth ? arWidth : objWidth;
                                        setRect = true;
                                    }
                                    if (obj.maxWidth > 0f && obj.maxWidth < objWidth)
                                    {
                                        objWidth = obj.maxWidth;
                                        setRect = true;
                                    }
                                    if (setRect)
                                    {
                                        width -= objWidth;
                                        currentMaxPlaceholder -= obj.Placeholder;
                                        obj.Rect = new RectangleF(0, 0, objWidth, height);
                                    }
                                    else
                                    {
                                        noSizeList.Add(obj);
                                    }
                                    allSizeList.Add(obj);
                                }
                            }
                        }
                        for (int i = 0; i < noSizeList.Count; i++)
                        {
                            LayoutControl obj = noSizeList[i];
                            float objWidth = width / currentMaxPlaceholder * obj.Placeholder;
                            width -= objWidth;
                            currentMaxPlaceholder -= obj.Placeholder;
                            obj.Rect = new RectangleF(0, 0, objWidth, height);
                        }
                        for (int i = 0; i < allSizeList.Count; i++)
                        {
                            LayoutControl obj = allSizeList[i];
                            obj.Rect = new RectangleF(x, y, obj.Rect.Width, obj.Rect.Height);
                            obj.DxRenderLogic();
                            if (!(allSizeList[i].oriType == OrientationType.Object && allSizeList[i].uiObject != null && allSizeList[i].uiObject.Hide))
                                x += obj.Rect.Width;
                        }
                        DrawRect = new RectangleF(Rect.X, Rect.Y, Rect.Width, Rect.Height);
                    }
                    break;
                case OrientationType.Vertical://竖
                    if (controlList.Count >= 1)
                    {
                        List<LayoutControl> noSizeList = new List<LayoutControl>();
                        List<LayoutControl> allSizeList = new List<LayoutControl>();
                        float currentMaxPlaceholder = maxPlaceholder;
                        for (int i = 0; i < controlList.Count; i++)
                        {
                            LayoutControl obj = controlList[i];
                            if (obj != null)
                            {
                                if (SetRect(obj))
                                {
                                    bool setRect = false;
                                    float objHeight = Rect.Height / maxPlaceholder * obj.Placeholder;
                                    if (obj.AspectRatio >= 0f)
                                    {
                                        float arHeight = width * obj.AspectRatio;
                                        objHeight = arHeight < objHeight ? arHeight : objHeight;
                                        setRect = true;
                                    }
                                    if (obj.maxHeight > 0f && obj.maxHeight < objHeight)
                                    {
                                        objHeight = obj.maxHeight;
                                        setRect = true;
                                    }
                                    if (setRect)
                                    {
                                        height -= objHeight;
                                        currentMaxPlaceholder -= obj.Placeholder;
                                        obj.Rect = new RectangleF(0, 0, width, objHeight);
                                    }
                                    else
                                    {
                                        noSizeList.Add(obj);
                                    }
                                    allSizeList.Add(obj);
                                }
                            }
                        }
                        for (int i = 0; i < noSizeList.Count; i++)
                        {
                            LayoutControl obj = noSizeList[i];
                            if (obj != null)
                            {
                                float objHeight = height / currentMaxPlaceholder * obj.Placeholder;
                                height -= objHeight;
                                currentMaxPlaceholder -= obj.Placeholder;
                                obj.Rect = new RectangleF(0, 0, width, objHeight);
                            }
                        }
                        for (int i = 0; i < allSizeList.Count; i++)
                        {
                            LayoutControl obj = allSizeList[i];
                            obj.Rect = new RectangleF(x, y, obj.Rect.Width, obj.Rect.Height);
                            obj.DxRenderLogic();
                            if (!(allSizeList[i].oriType == OrientationType.Object && allSizeList[i].uiObject != null && allSizeList[i].uiObject.Hide))
                                y += obj.Rect.Height;
                        }
                        DrawRect = new RectangleF(Rect.X, Rect.Y, Rect.Width, Rect.Height);
                    }
                    break;
                case OrientationType.Object:
                    if (uiObject != null)
                    {
                        uiObject.Rect = Rect;
                        DrawRect = uiObject.DrawRect;
                        uiObject.DxRenderLogic();
                    }
                    break;
                default:
                    throw new Exception("LayoutControl OrientationType Error !!!");
            }
        }

        public void DxRenderHigh()
        {
            if (Hide) return;
            if (BackColorHigh.Alpha > 0f)
            {
                Dx2D.Instance.RenderTarget2D.FillRectangle(DrawRect, Dx2D.Instance.GetSolidColorBrush(BackColorHigh));
            }
            for (int i = 0; i < controlList.Count; i++)
            {
                if (controlList[i] != null)
                    controlList[i].DxRenderHigh();
            }
            if (uiObject != null)
                uiObject.DxRenderHigh();
        }

        public void DxRenderMedium()
        {
            if (Hide) return;
            if (BackColorMedium.Alpha > 0f)
            {
                Dx2D.Instance.RenderTarget2D.FillRectangle(DrawRect, Dx2D.Instance.GetSolidColorBrush(BackColorMedium));
            }
            for (int i = 0; i < controlList.Count; i++)
            {
                if (controlList[i] != null)
                    controlList[i].DxRenderMedium();
            }
            if (uiObject != null)
                uiObject.DxRenderMedium();
        }

        public void DxRenderLow()
        {
#if DEBUG
            //Dx2D.Instance.RenderTarget2D.DrawRectangle(Rect, Dx2D.Instance.GetSolidColorBrush(1f, 1f, 0f, 0.2f));
            //Dx2D.Instance.RenderTarget2D.DrawRectangle(DrawRect, Dx2D.Instance.GetSolidColorBrush(1f, 0f, 0f, 0.2f));
#endif
            if (Hide) return;
            if (BackColorLow.Alpha > 0f)
            {
                Dx2D.Instance.RenderTarget2D.FillRectangle(DrawRect, Dx2D.Instance.GetSolidColorBrush(BackColorLow));
            }
            for (int i = 0; i < controlList.Count; i++)
            {
                if (controlList[i] != null)
                    controlList[i].DxRenderLow();
            }
            if (uiObject != null)
                uiObject.DxRenderLow();
        }
    }
}
