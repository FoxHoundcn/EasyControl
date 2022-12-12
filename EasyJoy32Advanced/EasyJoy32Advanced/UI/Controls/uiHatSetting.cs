using SharpDX;
using SharpDX.Direct2D1;
using SharpDX.DirectWrite;
using System;
using System.Windows.Forms;

namespace EasyControl
{
    public class uiHatSetting : iControl
    {
        public iControl Parent { get; set; }
        public bool Hide { get; set; } = false;
        public int Index { get; private set; }
        public string Name { get; set; }
        public Vector2 _offset = new Vector2();
        public Vector2 Offset
        {
            get { return _offset; }
            set
            {
                _offset = value;
                lcInput.Offset = value;
            }
        }
        public RectangleF DrawRect { get { return Rect; } }
        public RectangleF Rect { private get; set; }
        public string PluginID { get; set; }
        public string UIKey { set; private get; }
        public bool NodeLinkMode { get; private set; }
        //--------------------------------------------------------------------------------
        public Color4 backColor;
        public bool mouseEnter { get; private set; } = false;
        private const int maxCount = 4;
        private int CurrentSelectIndex = -1;
        LayoutControl lcInput = new LayoutControl(OrientationType.Horizontal);
        uiButton btn0;
        uiButton btn1;
        uiButton btn2;
        uiButton btn3;
        //--------------------------------------------------------------------------------
        float ellipseRadius;
        RectangleF rr = new RectangleF();
        Ellipse em = new Ellipse();
        #region 8向三角
        Vector2 u1 = new Vector2();
        Vector2 u2 = new Vector2();
        Vector2 u3 = new Vector2();
        Vector2 ul1 = new Vector2();
        Vector2 ul2 = new Vector2();
        Vector2 ul3 = new Vector2();
        Vector2 l1 = new Vector2();
        Vector2 l2 = new Vector2();
        Vector2 l3 = new Vector2();
        Vector2 ld1 = new Vector2();
        Vector2 ld2 = new Vector2();
        Vector2 ld3 = new Vector2();
        Vector2 d1 = new Vector2();
        Vector2 d2 = new Vector2();
        Vector2 d3 = new Vector2();
        Vector2 dr1 = new Vector2();
        Vector2 dr2 = new Vector2();
        Vector2 dr3 = new Vector2();
        Vector2 r1 = new Vector2();
        Vector2 r2 = new Vector2();
        Vector2 r3 = new Vector2();
        Vector2 ru1 = new Vector2();
        Vector2 ru2 = new Vector2();
        Vector2 ru3 = new Vector2();
        #endregion
        Ellipse e0 = new Ellipse();
        Ellipse e1 = new Ellipse();
        Ellipse e2 = new Ellipse();
        Ellipse e3 = new Ellipse();
        RectangleF hat0 = new RectangleF();
        RectangleF hat1 = new RectangleF();
        RectangleF hat2 = new RectangleF();
        RectangleF hat3 = new RectangleF();
        TextFormat tf;
        const int ActualRange = 2;
        ///////////////////////////////////////////////////////////////////////////////////
        public uiHatSetting(int index)
        {
            Index = index;
            Name = "HatSetting" + index;
            backColor = XmlUI.DxUIBackColor;
            btn0 = new uiButton(0, "1", jBtnType.NoLocalization);
            btn1 = new uiButton(1, "2", jBtnType.NoLocalization);
            btn2 = new uiButton(2, "3", jBtnType.NoLocalization);
            btn3 = new uiButton(3, "4", jBtnType.NoLocalization);
            btn0.LeftButtonClick += OnBtnClick;
            btn1.LeftButtonClick += OnBtnClick;
            btn2.LeftButtonClick += OnBtnClick;
            btn3.LeftButtonClick += OnBtnClick;
            lcInput.AddObject(new LayoutControl(btn0));
            lcInput.AddObject(new LayoutControl(btn1));
            lcInput.AddObject(new LayoutControl(btn2));
            lcInput.AddObject(new LayoutControl(btn3));
            lcInput.Parent = this;
            Dx2DResize();
        }
        public void Dx2DResize()
        {
            if (Hide) return;
            Dx2D.Instance.GetTextFormat(ref tf, hat0.Height - ActualRange * 2, TextAlignment.Center);
            lcInput.Dx2DResize();
        }
        public void JoyMouseMoveEvent(MouseEventArgs e)
        {
            if (Hide)
            {
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
            lcInput.JoyMouseMoveEvent(e);
        }
        public void JoyMouseDownEvent(MouseEventArgs e)
        {
            if (Hide) return;
            float xBtn;
            float yBtn;
            float widthBtn = Rect.Width * 0.06f;
            //---------up
            xBtn = Offset.X + em.Point.X;
            yBtn = Offset.Y + em.Point.Y - (em.RadiusY - e0.RadiusY - ellipseRadius * 0.025f);
            if (e.X > xBtn - widthBtn && e.X < xBtn + widthBtn &&
                e.Y > yBtn - widthBtn && e.Y < yBtn + widthBtn)
            {
                CurrentSelectIndex = 0;
                return;
            }
            //---------left
            xBtn = Offset.X + em.Point.X - (em.RadiusX - e1.RadiusX - ellipseRadius * 0.025f);
            yBtn = Offset.Y + em.Point.Y;
            if (e.X > xBtn - widthBtn && e.X < xBtn + widthBtn &&
                e.Y > yBtn - widthBtn && e.Y < yBtn + widthBtn)
            {
                CurrentSelectIndex = 1;
                return;
            }
            //---------down
            xBtn = Offset.X + em.Point.X;
            yBtn = Offset.Y + em.Point.Y + (em.RadiusY - e2.RadiusY - ellipseRadius * 0.025f);
            if (e.X > xBtn - widthBtn && e.X < xBtn + widthBtn &&
                e.Y > yBtn - widthBtn && e.Y < yBtn + widthBtn)
            {
                CurrentSelectIndex = 2;
                return;
            }
            //---------right
            xBtn = Offset.X + em.Point.X + (em.RadiusX - e3.RadiusX - ellipseRadius * 0.025f);
            yBtn = Offset.Y + em.Point.Y;
            if (e.X > xBtn - widthBtn && e.X < xBtn + widthBtn &&
                e.Y > yBtn - widthBtn && e.Y < yBtn + widthBtn)
            {
                CurrentSelectIndex = 3;
                return;
            }
            lcInput.JoyMouseDownEvent(e);
        }
        public void JoyMouseUpEvent(MouseEventArgs e)
        {
            if (Hide) return;
            lcInput.JoyMouseUpEvent(e);
        }
        public void JoyMouseMoveWheel(MouseEventArgs e)
        {
            if (Hide) return;
            lcInput.JoyMouseMoveWheel(e);
        }

        private void OnBtnClick(object sender, EventArgs e)
        {
            JoyIndexChangeArgs args = (JoyIndexChangeArgs)e;
            JoyObject currentObj = PublicData.GetCurrentSelectJoyObject();
            if (currentObj != null)
            {
                Hat hat = currentObj.GetHat(Index);
                if (hat != null)
                {
                    if (CurrentSelectIndex >= 0 && CurrentSelectIndex < JoyConst.MaxHat && hat.InputIndex[CurrentSelectIndex] != args.Index)
                    {
                        int indexChange = -1;
                        for (int change = 0; change < maxCount; change++)
                        {
                            if (hat.InputIndex[change] == args.Index)
                                indexChange = change;
                        }
                        if (indexChange >= 0 && indexChange < JoyConst.MaxHat)
                        {
                            hat.InputIndex[indexChange] = hat.InputIndex[CurrentSelectIndex];
                            hat.InputIndex[CurrentSelectIndex] = (byte)args.Index;
                        }
                        return;
                    }
                }
            }
        }
        public void DxRenderLogic()
        {
            if (Hide) return;
            ellipseRadius = Math.Min(Rect.Height, Rect.Width);
            #region 背景
            rr = PublicData.GetActualRange(Rect, ActualRange);
            #endregion
            #region 苦力帽
            em.Point.X = Rect.X + Rect.Width * 0.5f;
            em.Point.Y = Rect.Y + Rect.Height * 0.35f + ActualRange;
            em.RadiusX = ellipseRadius * 0.25f - ActualRange * 2;
            em.RadiusY = ellipseRadius * 0.25f - ActualRange * 2;
            //------------------------------------------------
            u1.X = em.Point.X;
            u1.Y = em.Point.Y - em.RadiusX - ellipseRadius * 0.1f;
            //------------------------------------------------
            u2.X = em.Point.X + ellipseRadius * 0.07f;
            u2.Y = em.Point.Y - em.RadiusX - ellipseRadius * 0.025f;
            //------------------------------------------------
            u3.X = em.Point.X - ellipseRadius * 0.07f;
            u3.Y = em.Point.Y - em.RadiusX - ellipseRadius * 0.025f;
            //------------------------------------------------
            ul1.X = em.Point.X - (em.RadiusX + ellipseRadius * 0.1f) * 0.7071f;
            ul1.Y = em.Point.Y - (em.RadiusX + ellipseRadius * 0.1f) * 0.7071f;
            //------------------------------------------------
            ul2.X = em.Point.X - (em.RadiusX + ellipseRadius * 0.1f) * 0.7071f + ellipseRadius * 0.11f;
            ul2.Y = em.Point.Y - (em.RadiusX + ellipseRadius * 0.1f) * 0.7071f;
            //------------------------------------------------
            ul3.X = em.Point.X - (em.RadiusX + ellipseRadius * 0.1f) * 0.7071f;
            ul3.Y = em.Point.Y - (em.RadiusX + ellipseRadius * 0.1f) * 0.7071f + ellipseRadius * 0.11f;
            //------------------------------------------------
            l1.X = em.Point.X - em.RadiusX - ellipseRadius * 0.1f;
            l1.Y = em.Point.Y;
            //------------------------------------------------
            l2.X = em.Point.X - em.RadiusX - ellipseRadius * 0.025f;
            l2.Y = em.Point.Y + ellipseRadius * 0.07f;
            //------------------------------------------------
            l3.X = em.Point.X - em.RadiusX - ellipseRadius * 0.025f;
            l3.Y = em.Point.Y - ellipseRadius * 0.07f;
            //------------------------------------------------
            ld1.X = em.Point.X - (em.RadiusX + ellipseRadius * 0.1f) * 0.7071f;
            ld1.Y = em.Point.Y + (em.RadiusX + ellipseRadius * 0.1f) * 0.7071f;
            //------------------------------------------------
            ld2.X = em.Point.X - (em.RadiusX + ellipseRadius * 0.1f) * 0.7071f;
            ld2.Y = em.Point.Y + (em.RadiusX + ellipseRadius * 0.1f) * 0.7071f - ellipseRadius * 0.11f;
            //------------------------------------------------
            ld3.X = em.Point.X - (em.RadiusX + ellipseRadius * 0.1f) * 0.7071f + ellipseRadius * 0.11f;
            ld3.Y = em.Point.Y + (em.RadiusX + ellipseRadius * 0.1f) * 0.7071f;
            //------------------------------------------------
            d1.X = em.Point.X;
            d1.Y = em.Point.Y + em.RadiusX + ellipseRadius * 0.1f;
            //------------------------------------------------
            d2.X = em.Point.X + ellipseRadius * 0.07f;
            d2.Y = em.Point.Y + em.RadiusX + ellipseRadius * 0.025f;
            //------------------------------------------------
            d3.X = em.Point.X - ellipseRadius * 0.07f;
            d3.Y = em.Point.Y + em.RadiusX + ellipseRadius * 0.025f;
            //------------------------------------------------
            dr1.X = em.Point.X + (em.RadiusX + ellipseRadius * 0.1f) * 0.7071f;
            dr1.Y = em.Point.Y + (em.RadiusX + ellipseRadius * 0.1f) * 0.7071f;
            //------------------------------------------------
            dr2.X = em.Point.X + (em.RadiusX + ellipseRadius * 0.1f) * 0.7071f;
            dr2.Y = em.Point.Y + (em.RadiusX + ellipseRadius * 0.1f) * 0.7071f - ellipseRadius * 0.11f;
            //------------------------------------------------
            dr3.X = em.Point.X + (em.RadiusX + ellipseRadius * 0.1f) * 0.7071f - ellipseRadius * 0.11f;
            dr3.Y = em.Point.Y + (em.RadiusX + ellipseRadius * 0.1f) * 0.7071f;
            //------------------------------------------------
            r1.X = em.Point.X + em.RadiusX + ellipseRadius * 0.1f;
            r1.Y = em.Point.Y;
            //------------------------------------------------
            r2.X = em.Point.X + em.RadiusX + ellipseRadius * 0.025f;
            r2.Y = em.Point.Y + ellipseRadius * 0.07f;
            //------------------------------------------------
            r3.X = em.Point.X + em.RadiusX + ellipseRadius * 0.025f;
            r3.Y = em.Point.Y - ellipseRadius * 0.07f;
            //------------------------------------------------
            ru1.X = em.Point.X + (em.RadiusX + ellipseRadius * 0.1f) * 0.7071f;
            ru1.Y = em.Point.Y - (em.RadiusX + ellipseRadius * 0.1f) * 0.7071f;
            //------------------------------------------------
            ru2.X = em.Point.X + (em.RadiusX + ellipseRadius * 0.1f) * 0.7071f;
            ru2.Y = em.Point.Y - (em.RadiusX + ellipseRadius * 0.1f) * 0.7071f + ellipseRadius * 0.11f;
            //------------------------------------------------
            ru3.X = em.Point.X + (em.RadiusX + ellipseRadius * 0.1f) * 0.7071f - ellipseRadius * 0.11f;
            ru3.Y = em.Point.Y - (em.RadiusX + ellipseRadius * 0.1f) * 0.7071f;
            #endregion
            #region 属性框
            e0.RadiusX = Rect.Width * 0.06f;
            e0.RadiusY = Rect.Width * 0.06f;
            e0.Point.X = em.Point.X;
            e0.Point.Y = em.Point.Y - (em.RadiusY - e0.RadiusY - ellipseRadius * 0.025f);
            //------------------------------------------------
            hat0.X = e0.Point.X - e0.RadiusX;
            hat0.Y = e0.Point.Y - e0.RadiusY;
            hat0.Width = e0.RadiusX * 2;
            hat0.Height = e0.RadiusY * 2;
            //------------------------------------------------
            e1.RadiusX = Rect.Width * 0.06f;
            e1.RadiusY = Rect.Width * 0.06f;
            e1.Point.X = em.Point.X - (em.RadiusX - e1.RadiusX - ellipseRadius * 0.025f);
            e1.Point.Y = em.Point.Y;
            //------------------------------------------------
            hat1.X = e1.Point.X - e1.RadiusX;
            hat1.Y = e1.Point.Y - e1.RadiusY;
            hat1.Width = e1.RadiusX * 2;
            hat1.Height = e1.RadiusY * 2;
            //------------------------------------------------
            e2.RadiusX = Rect.Width * 0.06f;
            e2.RadiusY = Rect.Width * 0.06f;
            e2.Point.X = em.Point.X;
            e2.Point.Y = em.Point.Y + (em.RadiusY - e2.RadiusY - ellipseRadius * 0.025f);
            //------------------------------------------------
            hat2.X = e2.Point.X - e2.RadiusX;
            hat2.Y = e2.Point.Y - e2.RadiusY;
            hat2.Width = e2.RadiusX * 2;
            hat2.Height = e2.RadiusY * 2;
            //------------------------------------------------
            e3.RadiusX = Rect.Width * 0.06f;
            e3.RadiusY = Rect.Width * 0.06f;
            e3.Point.X = em.Point.X + (em.RadiusX - e3.RadiusX - ellipseRadius * 0.025f);
            e3.Point.Y = em.Point.Y;
            //------------------------------------------------
            hat3.X = e3.Point.X - e3.RadiusX;
            hat3.Y = e3.Point.Y - e3.RadiusY;
            hat3.Width = e3.RadiusX * 2;
            hat3.Height = e3.RadiusY * 2;
            #endregion
            lcInput.Rect = new RectangleF(Rect.X + ActualRange * 2, Rect.Y + Rect.Height * 0.7f + ActualRange * 2,
                Rect.Width - ActualRange * 4, Rect.Height * 0.3f - ActualRange * 4);
            JoyObject currentObj = PublicData.GetCurrentSelectJoyObject();
            if (currentObj != null)
            {
                Hat hat = currentObj.GetHat(Index);
                if (hat != null)
                {
                    if (!hat.State[0])
                        btn0.BackColor = XmlUI.DxDeviceGreen;
                    else
                        btn0.BackColor = XmlUI.DxUIBackColor;
                    if (!hat.State[1])
                        btn1.BackColor = XmlUI.DxDeviceGreen;
                    else
                        btn1.BackColor = XmlUI.DxUIBackColor;
                    if (!hat.State[2])
                        btn2.BackColor = XmlUI.DxDeviceGreen;
                    else
                        btn2.BackColor = XmlUI.DxUIBackColor;
                    if (!hat.State[3])
                        btn3.BackColor = XmlUI.DxDeviceGreen;
                    else
                        btn3.BackColor = XmlUI.DxUIBackColor;
                }
            }
            lcInput.DxRenderLogic();
        }

        public void DxRenderHigh()
        {
            if (Hide) return;
            lcInput.DxRenderHigh();
        }

        public void DxRenderMedium()
        {
            if (Hide) return;
            #region 背景
            SolidColorBrush solidColorBrush = Dx2D.Instance.GetSolidColorBrush(XmlUI.DxUIClickColor);
            if (mouseEnter)
                solidColorBrush.Color = XmlUI.DxUIClickColor;
            else
                solidColorBrush.Color = XmlUI.DxUIBackColor;
            Dx2D.Instance.RenderTarget2D.DrawRectangle(rr, solidColorBrush, ActualRange);
            #endregion
            #region 苦力帽
            solidColorBrush.Color = backColor;
            Dx2D.Instance.RenderTarget2D.DrawEllipse(em, solidColorBrush);
            //--------------------------------------------------------------------------Up
            #region up
            PathGeometry pgDataUp = new PathGeometry(Dx2D.Instance.Factory2D);
            GeometrySink gsDataUp = pgDataUp.Open();
            gsDataUp.BeginFigure(u1, FigureBegin.Filled);
            gsDataUp.AddLine(u2);
            gsDataUp.AddLine(u3);
            gsDataUp.EndFigure(FigureEnd.Closed);
            gsDataUp.Close();
            Dx2D.Instance.RenderTarget2D.FillGeometry(pgDataUp, solidColorBrush);
            pgDataUp.Dispose();
            gsDataUp.Dispose();
            #endregion
            //--------------------------------------------------------------------------UpLeft
            #region UpLeft
            PathGeometry pgDataUpLeft = new PathGeometry(Dx2D.Instance.Factory2D);
            GeometrySink gsDataUpLeft = pgDataUpLeft.Open();
            gsDataUpLeft.BeginFigure(ul1, FigureBegin.Filled);
            gsDataUpLeft.AddLine(ul2);
            gsDataUpLeft.AddLine(ul3);
            gsDataUpLeft.EndFigure(FigureEnd.Closed);
            gsDataUpLeft.Close();
            Dx2D.Instance.RenderTarget2D.FillGeometry(pgDataUpLeft, solidColorBrush);
            pgDataUpLeft.Dispose();
            gsDataUpLeft.Dispose();
            #endregion
            //--------------------------------------------------------------------------Left
            #region Left
            PathGeometry pgDataLeft = new PathGeometry(Dx2D.Instance.Factory2D);
            GeometrySink gsDataLeft = pgDataLeft.Open();
            gsDataLeft.BeginFigure(l1, FigureBegin.Filled);
            gsDataLeft.AddLine(l2);
            gsDataLeft.AddLine(l3);
            gsDataLeft.EndFigure(FigureEnd.Closed);
            gsDataLeft.Close();
            Dx2D.Instance.RenderTarget2D.FillGeometry(pgDataLeft, solidColorBrush);
            pgDataLeft.Dispose();
            gsDataLeft.Dispose();
            #endregion
            //--------------------------------------------------------------------------LeftDown
            #region LeftDown
            PathGeometry pgDataLeftDown = new PathGeometry(Dx2D.Instance.Factory2D);
            GeometrySink gsDataLeftDown = pgDataLeftDown.Open();
            gsDataLeftDown.BeginFigure(ld1, FigureBegin.Filled);
            gsDataLeftDown.AddLine(ld2);
            gsDataLeftDown.AddLine(ld3);
            gsDataLeftDown.EndFigure(FigureEnd.Closed);
            gsDataLeftDown.Close();
            Dx2D.Instance.RenderTarget2D.FillGeometry(pgDataLeftDown, solidColorBrush);
            pgDataLeftDown.Dispose();
            gsDataLeftDown.Dispose();
            #endregion
            //--------------------------------------------------------------------------Down
            #region Down
            PathGeometry pgDataDown = new PathGeometry(Dx2D.Instance.Factory2D);
            GeometrySink gsDataDown = pgDataDown.Open();
            gsDataDown.BeginFigure(d1, FigureBegin.Filled);
            gsDataDown.AddLine(d2);
            gsDataDown.AddLine(d3);
            gsDataDown.EndFigure(FigureEnd.Closed);
            gsDataDown.Close();
            Dx2D.Instance.RenderTarget2D.FillGeometry(pgDataDown, solidColorBrush);
            pgDataDown.Dispose();
            gsDataDown.Dispose();
            #endregion
            //--------------------------------------------------------------------------DownRight
            #region DownRight
            PathGeometry pgDataDownRight = new PathGeometry(Dx2D.Instance.Factory2D);
            GeometrySink gsDataDownRight = pgDataDownRight.Open();
            gsDataDownRight.BeginFigure(dr1, FigureBegin.Filled);
            gsDataDownRight.AddLine(dr2);
            gsDataDownRight.AddLine(dr3);
            gsDataDownRight.EndFigure(FigureEnd.Closed);
            gsDataDownRight.Close();
            Dx2D.Instance.RenderTarget2D.FillGeometry(pgDataDownRight, solidColorBrush);
            pgDataDownRight.Dispose();
            gsDataDownRight.Dispose();
            #endregion
            //--------------------------------------------------------------------------Right
            #region Right
            PathGeometry pgDataRight = new PathGeometry(Dx2D.Instance.Factory2D);
            GeometrySink gsDataRight = pgDataRight.Open();
            gsDataRight.BeginFigure(r1, FigureBegin.Filled);
            gsDataRight.AddLine(r2);
            gsDataRight.AddLine(r3);
            gsDataRight.EndFigure(FigureEnd.Closed);
            gsDataRight.Close();
            Dx2D.Instance.RenderTarget2D.FillGeometry(pgDataRight, solidColorBrush);
            pgDataRight.Dispose();
            gsDataRight.Dispose();
            #endregion
            //--------------------------------------------------------------------------RightUp
            #region RightUp
            PathGeometry pgDataRightUp = new PathGeometry(Dx2D.Instance.Factory2D);
            GeometrySink gsDataRightUp = pgDataRightUp.Open();
            gsDataRightUp.BeginFigure(ru1, FigureBegin.Filled);
            gsDataRightUp.AddLine(ru2);
            gsDataRightUp.AddLine(ru3);
            gsDataRightUp.EndFigure(FigureEnd.Closed);
            gsDataRightUp.Close();
            Dx2D.Instance.RenderTarget2D.FillGeometry(pgDataRightUp, solidColorBrush);
            pgDataRightUp.Dispose();
            gsDataRightUp.Dispose();
            #endregion
            //---------------------------------------------------------------------------
            #endregion
            #region 属性框
            JoyObject currentObj = PublicData.GetCurrentSelectJoyObject();
            if (currentObj != null)
            {
                Hat hat = currentObj.GetHat(Index);
                if (hat == null)
                    return;
                //--------------------------------------
                if (CurrentSelectIndex == 0)
                    solidColorBrush.Color = XmlUI.DxApplyColor;
                else
                    solidColorBrush.Color = XmlUI.DxDefaultColor;
                Dx2D.Instance.RenderTarget2D.FillEllipse(e0, solidColorBrush);
                solidColorBrush.Color = XmlUI.DxBackColor;
                Dx2D.Instance.RenderTarget2D.DrawText((hat.InputIndex[0] + 1).ToString(), tf, hat0, solidColorBrush);
                //--------------------------------------
                if (CurrentSelectIndex == 1)
                    solidColorBrush.Color = XmlUI.DxApplyColor;
                else
                    solidColorBrush.Color = XmlUI.DxDefaultColor;
                Dx2D.Instance.RenderTarget2D.FillEllipse(e1, solidColorBrush);
                solidColorBrush.Color = XmlUI.DxBackColor;
                Dx2D.Instance.RenderTarget2D.DrawText((hat.InputIndex[1] + 1).ToString(), tf, hat1, solidColorBrush);
                //--------------------------------------
                if (CurrentSelectIndex == 2)
                    solidColorBrush.Color = XmlUI.DxApplyColor;
                else
                    solidColorBrush.Color = XmlUI.DxDefaultColor;
                Dx2D.Instance.RenderTarget2D.FillEllipse(e2, solidColorBrush);
                solidColorBrush.Color = XmlUI.DxBackColor;
                Dx2D.Instance.RenderTarget2D.DrawText((hat.InputIndex[2] + 1).ToString(), tf, hat2, solidColorBrush);
                //--------------------------------------
                if (CurrentSelectIndex == 3)
                    solidColorBrush.Color = XmlUI.DxApplyColor;
                else
                    solidColorBrush.Color = XmlUI.DxDefaultColor;
                Dx2D.Instance.RenderTarget2D.FillEllipse(e3, solidColorBrush);
                solidColorBrush.Color = XmlUI.DxBackColor;
                Dx2D.Instance.RenderTarget2D.DrawText((hat.InputIndex[3] + 1).ToString(), tf, hat3, solidColorBrush);
                //--------------------------------------
            }
            #endregion
            lcInput.DxRenderMedium();
        }

        public void DxRenderLow()
        {
            if (Hide) return;
            lcInput.DxRenderLow();
        }
    }
}
