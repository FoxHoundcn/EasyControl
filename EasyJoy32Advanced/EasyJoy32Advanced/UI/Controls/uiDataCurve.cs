using SharpDX;
using SharpDX.Direct2D1;
using SharpDX.DirectWrite;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace EasyControl
{
    public class uiDataCurve : iControl
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
        //-------------------------------------------
        public int inFormatMax = 4095;
        public int outFormatMax = 4095;
        public bool mouseEnter { get; private set; } = false;
        private float mouseValueY = 0f;
        private const int maxPoint = 200;
        private List<int> inData;
        private List<int> outData;
        private float topOff { get { return Rect.Height * 0.1f; } }
        private bool bDrawData = true;
        PathGeometry pgInLineData = null;
        PathGeometry pgInData = null;
        PathGeometry pgOutLineSource = null;
        PathGeometry pgOutSource = null;
        //-------------------------------------------
        RectangleF rrT = new RectangleF();
        RectangleF rrB = new RectangleF();
        Vector2 lineL1 = new Vector2();
        Vector2 lineL2 = new Vector2();
        Vector2 lineR1 = new Vector2();
        Vector2 lineR2 = new Vector2();
        Vector2 lineT1 = new Vector2();
        Vector2 lineT2 = new Vector2();
        Vector2 lineM1 = new Vector2();
        Vector2 lineM2 = new Vector2();
        Vector2 lineB1 = new Vector2();
        Vector2 lineB2 = new Vector2();
        //-----------------------------------------------------
        RectangleF rrInF = new RectangleF();
        RectangleF rrInT = new RectangleF();
        RectangleF rrInD = new RectangleF();
        RectangleF rrOutF = new RectangleF();
        RectangleF rrOutT = new RectangleF();
        RectangleF rrOutD = new RectangleF();
        //-----------------------------------------------------
        string drawName;
        TextFormat tfName;
        TextFormat tfInOutL;
        TextFormat tfInOutR;
        const int ActualRange = 2;
        //////////////////////////////////////////////////////////////////////////////
        public uiDataCurve()
        {
            Index = maxIndex;
            maxIndex++;
            Name = "DataCurve";
            Rect = new RectangleF();
            //-------------------------------------------
            inData = new List<int>();
            outData = new List<int>();
            for (int i = 0; i < maxPoint; i++)
            {
                double inAngle = (double)inFormatMax / maxPoint * i / inFormatMax * 360f;
                double outAngle = (double)outFormatMax / maxPoint * i / outFormatMax * 360f;
                AddData((int)(Math.Sin(inAngle * Math.PI / 360f) * inFormatMax / 2 + inFormatMax / 2),
                    (int)(Math.Cos(outAngle * Math.PI / 360f) * outFormatMax / 2 + outFormatMax / 2));
            }
            Dx2DResize();
        }
        public void Dx2DResize()
        {
            if (Hide) return;
            drawName = Dx2D.Instance.GetDrawText(Localization.Instance.GetLS(Name), rrT, ref tfName, TextAlignment.Center);
            Dx2D.Instance.GetTextFormat(ref tfInOutR, Rect.Height * 0.06f, TextAlignment.Trailing);
            Dx2D.Instance.GetTextFormat(ref tfInOutL, Rect.Height * 0.06f, TextAlignment.Leading);
        }
        public void JoyMouseMoveEvent(MouseEventArgs e)
        {
            if (Hide)
            {
                mouseEnter = false;
                return;
            }
            if (e.X >= Offset.X + Rect.X + Rect.Height * 0.1f && e.X < Offset.X + Rect.X + Rect.Width - Rect.Height * 0.1f &&
                e.Y >= Offset.Y + Rect.Y + topOff && e.Y < Offset.Y + Rect.Y + Rect.Height * 0.9f)
            {
                mouseValueY = (e.Y - Offset.Y - Rect.Y - topOff) / (Rect.Height * 0.9f - topOff);
                mouseEnter = true;
            }
            else
            {
                mouseEnter = false;
            }
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
        public void AddData(int _in, int _out)
        {
            inData.Add(_in);
            if (inData.Count > maxPoint)
            {
                inData.RemoveAt(0);
            }
            outData.Add(_out);
            if (outData.Count > maxPoint)
            {
                outData.RemoveAt(0);
            }
            bDrawData = true;
        }
        public int GetLateInValue()
        {
            return inData[inData.Count - 1];
        }
        public void DxRenderLogic()
        {
            if (Hide) return;
            rrT.X = Rect.X;
            rrT.Y = Rect.Y + ActualRange;
            rrT.Width = Rect.Width;
            rrT.Height = topOff - ActualRange * 2;
            //------------------------------------------------------------
            rrB = PublicData.GetActualRange(Rect, ActualRange);
            //------------------------------------------------------------
            #region 底框
            lineL1.X = Rect.X + Rect.Height * 0.1f;
            lineL1.Y = Rect.Y + topOff - Rect.Height * 0.05f;
            lineL2.X = Rect.X + Rect.Height * 0.1f;
            lineL2.Y = Rect.Y + Rect.Height * 0.95f;
            //------------------------------------------------------------
            lineR1.X = Rect.X + Rect.Width - Rect.Height * 0.1f;
            lineR1.Y = Rect.Y + topOff - Rect.Height * 0.05f;
            lineR2.X = Rect.X + Rect.Width - Rect.Height * 0.1f;
            lineR2.Y = Rect.Y + Rect.Height * 0.95f;
            //------------------------------------------------------------
            lineT1.X = Rect.X + Rect.Height * 0.05f;
            lineT1.Y = Rect.Y + topOff;
            lineT2.X = Rect.X + Rect.Width - Rect.Height * 0.05f;
            lineT2.Y = Rect.Y + topOff;
            //------------------------------------------------------------
            lineM1.X = Rect.X + Rect.Height * 0.05f;
            lineM1.Y = Rect.Y + topOff + (Rect.Height - topOff - Rect.Height * 0.1f) / 2;
            lineM2.X = Rect.X + Rect.Width - Rect.Height * 0.05f;
            lineM2.Y = Rect.Y + topOff + (Rect.Height - topOff - Rect.Height * 0.1f) / 2;
            //------------------------------------------------------------
            lineB1.X = Rect.X + Rect.Height * 0.05f;
            lineB1.Y = Rect.Y + Rect.Height * 0.9f;
            lineB2.X = Rect.X + Rect.Width - Rect.Height * 0.05f;
            lineB2.Y = Rect.Y + Rect.Height * 0.9f;
            #endregion
            #region 文字描述
            rrInF.X = Rect.X + Rect.Width / 14f;
            rrInF.Y = Rect.Y + Rect.Height * 0.92f;
            rrInF.Width = Rect.Height * 0.06f;
            rrInF.Height = Rect.Height * 0.06f;
            //-------------------------------------------------------------
            rrInT.X = Rect.X + Rect.Width / 14f * 2f;
            rrInT.Y = Rect.Y + Rect.Height * 0.92f;
            rrInT.Width = Rect.Width / 7f;
            rrInT.Height = Rect.Height * 0.06f;
            //-------------------------------------------------------------
            rrInD.X = Rect.X + Rect.Width / 14f * 3f;
            rrInD.Y = Rect.Y + Rect.Height * 0.92f;
            rrInD.Width = Rect.Width / 7f * 1.75f;
            rrInD.Height = Rect.Height * 0.06f;
            //-------------------------------------------------------------
            rrOutF.X = Rect.X + Rect.Width / 14f * 7f;
            rrOutF.Y = Rect.Y + Rect.Height * 0.92f;
            rrOutF.Width = Rect.Height * 0.06f;
            rrOutF.Height = Rect.Height * 0.06f;
            //-------------------------------------------------------------
            rrOutT.X = Rect.X + Rect.Width / 14f * 8f;
            rrOutT.Y = Rect.Y + Rect.Height * 0.92f;
            rrOutT.Width = Rect.Width / 7f;
            rrOutT.Height = Rect.Height * 0.06f;
            //-------------------------------------------------------------
            rrOutD.X = Rect.X + Rect.Width / 14f * 9f;
            rrOutD.Y = Rect.Y + Rect.Height * 0.92f;
            rrOutD.Width = Rect.Width / 7f * 1.75f;
            rrOutD.Height = Rect.Height * 0.06f;
            #endregion
            if (bDrawData)
            {
                #region init
                if (pgInLineData == null)
                {
                    pgInLineData = new PathGeometry(Dx2D.Instance.Factory2D);
                }
                else
                {
                    pgInLineData.Dispose();
                    pgInLineData = new PathGeometry(Dx2D.Instance.Factory2D);
                }
                if (pgInData == null)
                {
                    pgInData = new PathGeometry(Dx2D.Instance.Factory2D);
                }
                else
                {
                    pgInData.Dispose();
                    pgInData = new PathGeometry(Dx2D.Instance.Factory2D);
                }
                if (pgOutLineSource == null)
                {
                    pgOutLineSource = new PathGeometry(Dx2D.Instance.Factory2D);
                }
                else
                {
                    pgOutLineSource.Dispose();
                    pgOutLineSource = new PathGeometry(Dx2D.Instance.Factory2D);
                }
                if (pgOutSource == null)
                {
                    pgOutSource = new PathGeometry(Dx2D.Instance.Factory2D);
                }
                else
                {
                    pgOutSource.Dispose();
                    pgOutSource = new PathGeometry(Dx2D.Instance.Factory2D);
                }
                #endregion
                #region 绘制数据
                float dataWidth = Rect.Width - Rect.Height * 0.2f;
                float dataHeight = Rect.Height - topOff - Rect.Height * 0.1f;

                float x1, y1, x2 = 0f, y2 = 0f;
                GeometrySink gsInLineData = pgInLineData.Open();
                for (int i = 0; i < inData.Count; i++)
                {
                    x1 = Rect.X + Rect.Height * 0.1f + dataWidth / inData.Count * i;
                    y1 = Rect.Y + Rect.Height * 0.9f - dataHeight * (inData[i] / (float)inFormatMax);
                    if (y1 < Rect.Y + topOff)
                        y1 = Rect.Y + topOff;
                    if (y1 > Rect.Y + Rect.Height * 0.9f)
                        y1 = Rect.Y + Rect.Height * 0.9f;
                    if (i == 0)
                    {
                        gsInLineData.BeginFigure(new Vector2(x1, y1), FigureBegin.Filled);
                    }
                    else
                    {
                        gsInLineData.AddLine(new Vector2(x1, y1));
                    }
                }
                x1 = Rect.X + Rect.Height * 0.1f + dataWidth;
                y1 = Rect.Y + Rect.Height * 0.9f - dataHeight * (inData[inData.Count - 1] / (float)inFormatMax);
                if (y1 < Rect.Y + topOff)
                    y1 = Rect.Y + topOff;
                gsInLineData.AddLine(new Vector2(Rect.X + Rect.Height * 0.1f + dataWidth, y1));
                x2 = x1;
                y2 = y1;
                gsInLineData.EndFigure(FigureEnd.Open);
                gsInLineData.Close();
                gsInLineData.Dispose();
                //-------------------------------------------------------------------------------------------------------
                GeometrySink gsInData = pgInData.Open();
                gsInData.BeginFigure(new Vector2(x2, y2), FigureBegin.Filled);
                gsInData.AddLine(new Vector2(x2 + 10f, y2 - 5f));
                gsInData.AddLine(new Vector2(x2 + 10f, y2 + 5f));
                gsInData.AddLine(new Vector2(x2, y2));
                gsInData.EndFigure(FigureEnd.Closed);
                gsInData.Close();
                gsInData.Dispose();
                #endregion
                #region 绘制元数据
                float sourceWidth = Rect.Width - Rect.Height * 0.2f;
                float sourceHeight = Rect.Height - topOff - Rect.Height * 0.1f;

                GeometrySink gsOutLineSource = pgOutLineSource.Open();
                for (int i = 0; i < outData.Count; i++)
                {
                    x1 = Rect.X + Rect.Height * 0.1f + sourceWidth / outData.Count * i;
                    y1 = Rect.Y + Rect.Height * 0.9f - sourceHeight * (outData[i] / (float)outFormatMax);
                    if (y1 < Rect.Y + topOff)
                        y1 = Rect.Y + topOff;
                    if (y1 > Rect.Y + Rect.Height * 0.9f)
                        y1 = Rect.Y + Rect.Height * 0.9f;
                    if (i == 0)
                    {
                        gsOutLineSource.BeginFigure(new Vector2(x1, y1), FigureBegin.Filled);
                    }
                    else
                    {
                        gsOutLineSource.AddLine(new Vector2(x1, y1));
                    }
                }
                x1 = Rect.X + Rect.Height * 0.1f + sourceWidth;
                y1 = Rect.Y + Rect.Height * 0.9f - sourceHeight * (outData[outData.Count - 1] / (float)outFormatMax);
                if (y1 < Rect.Y + topOff)
                    y1 = Rect.Y + topOff;
                gsOutLineSource.AddLine(new Vector2(Rect.X + Rect.Height * 0.1f + sourceWidth, y1));
                x2 = x1;
                y2 = y1;
                gsOutLineSource.EndFigure(FigureEnd.Open);
                gsOutLineSource.Close();
                gsOutLineSource.Dispose();
                GeometrySink gsOutSource = pgOutSource.Open();
                gsOutSource.BeginFigure(new Vector2(x2, y2), FigureBegin.Filled);
                gsOutSource.AddLine(new Vector2(x2, y2 - 5f));
                gsOutSource.AddLine(new Vector2(x2 + 10f, y2));
                gsOutSource.AddLine(new Vector2(x2, y2 + 5f));
                gsOutSource.EndFigure(FigureEnd.Closed);
                gsOutSource.Close();
                gsOutSource.Dispose();
                #endregion
                bDrawData = false;
            }
        }

        public void DxRenderHigh()
        {
            if (Hide) return;
        }

        public void DxRenderMedium()
        {
            if (Hide) return;
            SolidColorBrush solidColorBrush = Dx2D.Instance.GetSolidColorBrush(XmlUI.DxUIBackColor);
            #region 背景
            if (mouseEnter)
                solidColorBrush.Color = XmlUI.DxUIClickColor;
            else
                solidColorBrush.Color = XmlUI.DxUIBackColor;
            Dx2D.Instance.RenderTarget2D.DrawRectangle(rrB, solidColorBrush, ActualRange);
            #endregion
            #region 底框
            solidColorBrush.Color = XmlUI.DxTextColor;
            Dx2D.Instance.RenderTarget2D.DrawLine(lineL1, lineL2, solidColorBrush);//竖，前
            Dx2D.Instance.RenderTarget2D.DrawLine(lineR1, lineR2, solidColorBrush);//竖，后
            Dx2D.Instance.RenderTarget2D.DrawLine(lineT1, lineT2, solidColorBrush);//横，顶
            Dx2D.Instance.RenderTarget2D.DrawLine(lineM1, lineM2, solidColorBrush);//横，中
            Dx2D.Instance.RenderTarget2D.DrawLine(lineB1, lineB2, solidColorBrush);//横，底
            #endregion
            #region 绘制数据
            solidColorBrush.Color = Color.Red;
            if (pgInLineData != null)
                Dx2D.Instance.RenderTarget2D.DrawGeometry(pgInLineData, solidColorBrush, ActualRange);
            if (pgInData != null)
                Dx2D.Instance.RenderTarget2D.FillGeometry(pgInData, solidColorBrush);
            #endregion
            #region 绘制元数据
            solidColorBrush.Color = Color.Green;
            if (pgOutLineSource != null)
                Dx2D.Instance.RenderTarget2D.DrawGeometry(pgOutLineSource, solidColorBrush, ActualRange);
            if (pgOutSource != null)
                Dx2D.Instance.RenderTarget2D.FillGeometry(pgOutSource, solidColorBrush);
            #endregion
            #region 文字描述
            solidColorBrush.Color = Color.Red;
            Dx2D.Instance.RenderTarget2D.FillRectangle(rrInF, solidColorBrush);
            Dx2D.Instance.RenderTarget2D.DrawText(Localization.Instance.GetLS("Input"), tfInOutL, rrInT, solidColorBrush);
            Dx2D.Instance.RenderTarget2D.DrawText(inData[inData.Count - 1].ToString() + " / " + inFormatMax.ToString(), tfInOutR, rrInD, solidColorBrush);
            //----------------------------------------------------
            solidColorBrush.Color = Color.Green;
            Dx2D.Instance.RenderTarget2D.FillRectangle(rrOutF, solidColorBrush);
            Dx2D.Instance.RenderTarget2D.DrawText(Localization.Instance.GetLS("Output"), tfInOutL, rrOutT, solidColorBrush);
            Dx2D.Instance.RenderTarget2D.DrawText(outData[outData.Count - 1].ToString() + " / " + outFormatMax.ToString(), tfInOutR, rrOutD, solidColorBrush);
            #endregion
            Vector2 line1a = new Vector2();
            Vector2 line1b = new Vector2();
            RectangleF rect = new RectangleF();
            #region 最大值，中间值，最小值
            JoyObject currentObj = PublicData.GetCurrentSelectJoyObject();
            if (currentObj != null)
            {
                JoyDevice currentDev = currentObj.GetCurrentJoyDevice();
                if (currentDev != null)
                {
                    Format format = currentObj.GetSelectFormat();
                    if (format != null)
                    {
                        float viewHeight = Rect.Height - topOff - Rect.Height * 0.1f;
                        float min = 1f - format.minValue / (float)inFormatMax;
                        float mid = 1f - format.midValue / (float)inFormatMax;
                        float max = 1f - format.maxValue / (float)inFormatMax;
                        float minDZ = (mid - min) * 0.01f;
                        float maxDZ = (max - mid) * 0.01f;
                        //---------------------------------------------------------------------------------------------------------------------------minValue
                        line1a.X = Rect.X + Rect.Height * 0.05f;
                        line1a.Y = Rect.Y + topOff + viewHeight * min;
                        line1b.X = Rect.X + Rect.Width - Rect.Height * 0.1f;
                        line1b.Y = Rect.Y + topOff + viewHeight * min;
                        solidColorBrush.Color = Color.LightGreen;
                        Dx2D.Instance.RenderTarget2D.DrawLine(line1a, line1b, solidColorBrush);
                        PathGeometry pgMin = new PathGeometry(Dx2D.Instance.Factory2D);
                        GeometrySink gsMin = pgMin.Open();
                        gsMin.BeginFigure(new Vector2(line1a.X, line1a.Y), FigureBegin.Filled);
                        gsMin.AddLine(new Vector2(line1a.X, line1a.Y - 5f));
                        gsMin.AddLine(new Vector2(line1a.X + 15f, line1a.Y));
                        gsMin.AddLine(new Vector2(line1a.X, line1a.Y + 5f));
                        gsMin.EndFigure(FigureEnd.Closed);
                        gsMin.Close();
                        Dx2D.Instance.RenderTarget2D.FillGeometry(pgMin, solidColorBrush);
                        pgMin.Dispose();
                        gsMin.Dispose();
                        //----------------------------------------------------------------------------------------minDZ
                        rect.X = Rect.X + Rect.Height * 0.1f;
                        rect.Y = Rect.Y + topOff + viewHeight * min;
                        rect.Width = Rect.Width - Rect.Height * 0.2f;
                        rect.Height = viewHeight * minDZ * format.minDzone;
                        solidColorBrush.Opacity = 0.25f;
                        Dx2D.Instance.RenderTarget2D.FillRectangle(rect, solidColorBrush);
                        solidColorBrush.Opacity = 1f;
                        //---------------------------------------------------------------------------------------------------------------------------midValue
                        line1a.X = Rect.X + Rect.Height * 0.05f;
                        line1a.Y = Rect.Y + topOff + viewHeight * mid;
                        line1b.X = Rect.X + Rect.Width - Rect.Height * 0.1f;
                        line1b.Y = Rect.Y + topOff + viewHeight * mid;
                        solidColorBrush.Color = Color.LightPink;
                        Dx2D.Instance.RenderTarget2D.DrawLine(line1a, line1b, solidColorBrush);
                        PathGeometry pgMid = new PathGeometry(Dx2D.Instance.Factory2D);
                        GeometrySink gsMid = pgMid.Open();
                        gsMid.BeginFigure(new Vector2(line1a.X, line1a.Y), FigureBegin.Filled);
                        gsMid.AddLine(new Vector2(line1a.X, line1a.Y - 5f));
                        gsMid.AddLine(new Vector2(line1a.X + 15f, line1a.Y));
                        gsMid.AddLine(new Vector2(line1a.X, line1a.Y + 5f));
                        gsMid.EndFigure(FigureEnd.Closed);
                        gsMid.Close();
                        Dx2D.Instance.RenderTarget2D.FillGeometry(pgMid, solidColorBrush);
                        pgMid.Dispose();
                        gsMid.Dispose();
                        //----------------------------------------------------------------------------------------midDZ
                        rect.X = Rect.X + Rect.Height * 0.1f;
                        rect.Y = Rect.Y + topOff + viewHeight * mid - viewHeight * minDZ * format.midDzone;
                        rect.Width = Rect.Width - Rect.Height * 0.2f;
                        rect.Height = (viewHeight * minDZ * format.midDzone) + (viewHeight * maxDZ * format.midDzone);
                        solidColorBrush.Opacity = 0.25f;
                        Dx2D.Instance.RenderTarget2D.FillRectangle(rect, solidColorBrush);
                        solidColorBrush.Opacity = 1f;
                        //---------------------------------------------------------------------------------------------------------------------------maxValue
                        line1a.X = Rect.X + Rect.Height * 0.05f;
                        line1a.Y = Rect.Y + topOff + viewHeight * max;
                        line1b.X = Rect.X + Rect.Width - Rect.Height * 0.1f;
                        line1b.Y = Rect.Y + topOff + viewHeight * max;
                        solidColorBrush.Color = Color.LightBlue;
                        Dx2D.Instance.RenderTarget2D.DrawLine(line1a, line1b, solidColorBrush);
                        PathGeometry pgMax = new PathGeometry(Dx2D.Instance.Factory2D);
                        GeometrySink gsMax = pgMax.Open();
                        gsMax.BeginFigure(new Vector2(line1a.X, line1a.Y), FigureBegin.Filled);
                        gsMax.AddLine(new Vector2(line1a.X, line1a.Y - 5f));
                        gsMax.AddLine(new Vector2(line1a.X + 15f, line1a.Y));
                        gsMax.AddLine(new Vector2(line1a.X, line1a.Y + 5f));
                        gsMax.EndFigure(FigureEnd.Closed);
                        gsMax.Close();
                        Dx2D.Instance.RenderTarget2D.FillGeometry(pgMax, solidColorBrush);
                        pgMax.Dispose();
                        gsMax.Dispose();
                        //----------------------------------------------------------------------------------------maxDZ
                        rect.X = Rect.X + Rect.Height * 0.1f;
                        rect.Y = Rect.Y + topOff + viewHeight * max - viewHeight * maxDZ * format.maxDzone;
                        rect.Width = Rect.Width - Rect.Height * 0.2f;
                        rect.Height = viewHeight * maxDZ * format.maxDzone;
                        solidColorBrush.Opacity = 0.25f;
                        Dx2D.Instance.RenderTarget2D.FillRectangle(rect, solidColorBrush);
                        solidColorBrush.Opacity = 1f;
                        //---------------------------------------------------------------------------------------------------------------------------
                    }
                }
            }
            #endregion
            #region 数据线
            if (mouseEnter)
            {
                solidColorBrush.Color = XmlUI.DxTextColor;
                line1a.X = Rect.X + Rect.Height * 0.05f;
                line1a.Y = Rect.Y + topOff + mouseValueY * (Rect.Height * 0.9f - topOff);
                line1b.X = Rect.X + Rect.Width - Rect.Height * 0.05f;
                line1b.Y = Rect.Y + topOff + mouseValueY * (Rect.Height * 0.9f - topOff);
                Dx2D.Instance.RenderTarget2D.DrawLine(line1a, line1b, solidColorBrush);
                rect.Height = Rect.Height * 0.06f;
                rect.Width = rect.Height * 5f;
                rect.X = Rect.X + Rect.Height * 0.1f + ActualRange;
                rect.Y = Rect.Y + topOff + mouseValueY * (Rect.Height * 0.9f - topOff) - rect.Height;
                solidColorBrush.Color = Color.Red;
                Dx2D.Instance.RenderTarget2D.DrawText(((int)((1 - mouseValueY) * inFormatMax)).ToString(), tfInOutR, rect, solidColorBrush);
                rect.Y = Rect.Y + topOff + mouseValueY * (Rect.Height * 0.9f - topOff);
                solidColorBrush.Color = Color.Green;
                Dx2D.Instance.RenderTarget2D.DrawText(((int)((1 - mouseValueY) * outFormatMax)).ToString(), tfInOutR, rect, solidColorBrush);
            }
            #endregion
        }

        public void DxRenderLow()
        {
            if (Hide) return;
        }
    }
}
