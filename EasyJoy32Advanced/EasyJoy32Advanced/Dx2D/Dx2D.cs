using ControllorPlugin;
using SharpDX;
using SharpDX.Direct2D1;
using SharpDX.DirectWrite;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Windows.Forms;

namespace EasyControl
{
    public class Dx2D
    {
        #region fps
        public static int showFps = 0;
        private int msTims = 0;
        private int fps = 0;
        private int reSizeTims = 0;
        private long nextTime = 0;
        private int refreshTime = 0;
        #endregion
        #region D2D变量
        //笔刷----------------------------------------------------------------------------------------
        private SolidColorBrush solidColorBrush;
        //D2D----------------------------------------------------------------------------------------
        private PixelFormat P;
        private HwndRenderTargetProperties H;
        private RenderTargetProperties R;
        public SharpDX.DirectWrite.Factory FactoryDWrite;
        public SharpDX.Direct2D1.Factory Factory2D;
        public WindowRenderTarget RenderTarget2D = null;
        public static TextFormat nodeTextFormat = null;
        //
        PathGeometry PG;
        GeometrySink GS;
        BezierSegment BS = new BezierSegment();
        StrokeStyle strokeStyle;
        StrokeStyleProperties strokeProperties;
        int dashOffset = 0;                      //虚线当前偏移量;
        #endregion
        #region 窗口变量
        public iControl render { get; set; } = null;
        public int Width { get; private set; }
        public int Height { get; private set; }
        #endregion
        /////////////////////////////////////////////////////////////////////////////////////////////////
        public static readonly Dx2D Instance = new Dx2D();
        private Dx2D()
        {
        }
        public void Init(MainForm main, Panel panelD2D)
        {
            #region 初始化Dx2D
            P = new PixelFormat(SharpDX.DXGI.Format.B8G8R8A8_UNorm, AlphaMode.Ignore);
            Factory2D = new SharpDX.Direct2D1.Factory();
            H.Hwnd = panelD2D.Handle;
            H.PixelSize = new Size2(panelD2D.Width, panelD2D.Height);
            H.PresentOptions = PresentOptions.None;
            R = new RenderTargetProperties(RenderTargetType.Hardware,
                P, 0, 0, RenderTargetUsage.None, FeatureLevel.Level_DEFAULT);
            RenderTarget2D = new WindowRenderTarget(Factory2D, R, H);
            FactoryDWrite = new SharpDX.DirectWrite.Factory();
            #endregion
            #region 字体
            RenderTarget2D.TextAntialiasMode = TextAntialiasMode.Cleartype;
            #endregion
            #region 笔刷
            solidColorBrush = new SolidColorBrush(RenderTarget2D, Color.Black);
            #endregion
            #region 高宽
            Width = panelD2D.Width;
            Height = panelD2D.Height;
            #endregion
            #region 数据
            GetTextFormat(ref nodeTextFormat, JoyConst.FontSize, TextAlignment.Trailing);
            strokeProperties = new StrokeStyleProperties();
            strokeProperties.StartCap = CapStyle.Round;
            strokeProperties.EndCap = CapStyle.Round;
            strokeProperties.DashCap = CapStyle.Round;
            strokeProperties.DashStyle = DashStyle.Dash;
            #endregion
            JoyEvent.Instance.Dx2DInit(null, null);
            SharpDX.Windows.RenderLoop.Run(main, () =>
            {
                Render();
            });
        }
        #region Render
        private void Render()
        {
            try
            {
                Thread.Sleep(1);
#if !DEBUG
                if (nextTime < DateTime.Now.Ticks)
                {
                    nextTime = DateTime.Now.Ticks + 150000;
#endif
                if (DateTime.Now.Millisecond - reSizeTims > 200 || DateTime.Now.Millisecond - reSizeTims < 0)
                {
                    Process A = Process.GetCurrentProcess();
                    A.MaxWorkingSet = Process.GetCurrentProcess().MaxWorkingSet;
                    A.Dispose();
                    //----
                    reSizeTims = DateTime.Now.Millisecond;
                    render.Dx2DResize();
                }
                if (msTims <= DateTime.Now.Millisecond)
                {
                    fps++;
                    msTims = DateTime.Now.Millisecond;
                }
                else
                {
                    #region 刷新USB
                    refreshTime++;
                    if (refreshTime > 1)
                    {
                        JoyUSB.Instance.CheckRefresh();
                        refreshTime = 0;
                    }
                    #endregion
                    #region 自动保存
                    PublicData.autoSaveTime++;
                    #endregion
                    #region 验证登录状态
                    //暂时取消登录验证
                    //PublicData.checkLoginTime++;       
                    #endregion
                    msTims = 0;
                    showFps = fps;
                    fps = 0;
                    PublicData.publicColorCount += 10;
                    List<JoyObject> joyObjList = JoyUSB.Instance.GetJoyList();
                    for (int i = 0; i < joyObjList.Count; i++)
                    {
                        joyObjList[i].currentColorCount = PublicData.publicColorCount;
                        joyObjList[i].msTims = 0;
                        joyObjList[i].showFps = joyObjList[i].fps;
                        joyObjList[i].fps = 0;
                    }
                    List<JoyObject> joyClientList = TCPServer.Instance.GetJoyClientList();
                    for (int i = 0; i < joyClientList.Count; i++)
                    {
                        joyClientList[i].currentColorCount = PublicData.publicColorCount;
                        joyClientList[i].msTims = 0;
                        joyClientList[i].showFps = joyClientList[i].fps;
                        joyClientList[i].fps = 0;
                    }
                }
                RenderTarget2D.BeginDraw();
                RenderTarget2D.Clear(XmlUI.DxBackColor);
                #region 渲染事件
                if (render != null)
                {
                    dashOffset++;//更新绘制线段偏移;
                    render.DxRenderLogic();
                    render.DxRenderLow();
                    render.DxRenderMedium();
                    render.DxRenderHigh();
                }
                #endregion
                Color4 color = XmlUI.DxDeviceRed;
                if (showFps > 50)
                {
                    color = XmlUI.DxDeviceGreen;
                }
                else if (showFps > 30)
                {
                    color = XmlUI.DxDeviceYellow;
                }
                //------------------------------------------------------------------
                RenderTarget2D.EndDraw();
#if !DEBUG
                }
#endif
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + ex.StackTrace);
                Application.Exit();
            }
        }
        #endregion
        public void Resize(int width, int height)
        {
            Width = width;
            Height = height;
            if (RenderTarget2D != null)
                RenderTarget2D.Resize(new Size2(Width, Height));
            if (render != null)
            {
                render.Dx2DResize();
            }
        }
        public void DrawBezier(Vector2 start, Vector2 end, PortType dir, Color4 color, bool dash)
        {
            PG = new PathGeometry(Factory2D);
            GS = PG.Open();
            GS.BeginFigure(start, FigureBegin.Hollow);
            switch (dir)
            {
                case PortType.In:
                    BS.Point1 = new Vector2(start.X + 50f, start.Y);         //偏移一下
                    BS.Point2 = new Vector2(end.X - 50f, end.Y);          //偏移一下
                    break;
                case PortType.Out:
                    BS.Point1 = new Vector2(start.X - 50f, start.Y);         //偏移一下
                    BS.Point2 = new Vector2(end.X + 50f, end.Y);          //偏移一下
                    break;
            }
            BS.Point3 = end;
            GS.AddBezier(BS);
            GS.EndFigure(FigureEnd.Open);
            GS.Close();
            if (dashOffset > 120)
            {
                dashOffset = 0;
            }
            strokeProperties.DashOffset = dashOffset / 3f;
            strokeStyle = new StrokeStyle(Factory2D, strokeProperties);
            if (dash)
            {
                RenderTarget2D.DrawGeometry(PG, GetSolidColorBrush(color), 3f, strokeStyle);
            }
            else
            {
                RenderTarget2D.DrawGeometry(PG, GetSolidColorBrush(color), 3f);
            }
            GS.Dispose();
            PG.Dispose();
        }
        public Bitmap LoadBitmap(string path, System.Drawing.Imaging.ImageFormat imageFormat)
        {
            SharpDX.WIC.ImagingFactory _wicFactory = new SharpDX.WIC.ImagingFactory();
            SharpDX.WIC.BitmapDecoder decoder = new SharpDX.WIC.BitmapDecoder(
                    _wicFactory,
                    path,
                    SharpDX.WIC.DecodeOptions.CacheOnDemand);

            var formatConverter = new SharpDX.WIC.FormatConverter(_wicFactory);
            formatConverter.Initialize(
                    decoder.GetFrame(0),
                    SharpDX.WIC.PixelFormat.Format32bppPBGRA,
                    SharpDX.WIC.BitmapDitherType.DualSpiral8x8,
                    null,
                    0.0,
                    SharpDX.WIC.BitmapPaletteType.Custom);
            Bitmap bmp = Bitmap.FromWicBitmap(RenderTarget2D, formatConverter);
            return bmp;
        }
        public void GetTextFormat(ref TextFormat textFormat, float fontSize,
            TextAlignment ta = TextAlignment.Leading, ParagraphAlignment pa = ParagraphAlignment.Center, float maxTextHeight = 32f,
            FontWeight fontWeight = FontWeight.Normal, FontStyle fontStyle = FontStyle.Normal)
        {
            if (textFormat != null)
            {
                textFormat.Dispose();
                textFormat = null;
            }
            if (fontSize < JoyConst.MinFontSize)
                fontSize = JoyConst.MinFontSize;
            if (fontSize > JoyConst.MaxFontSize)
                fontSize = JoyConst.MaxFontSize;
            textFormat = new TextFormat(FactoryDWrite, JoyConst.FontType, fontWeight, fontStyle, fontSize);
            textFormat.TextAlignment = ta;
            textFormat.ParagraphAlignment = pa;
        }
        public System.Drawing.SizeF MeasureString(string Message, TextFormat textFormat)
        {
            TextLayout layout =
                new TextLayout(FactoryDWrite, Message, textFormat, 10000f, textFormat.FontSize);
            float width = layout.Metrics.Width;
            float height = layout.Metrics.Height;
            layout.Dispose();
            return new System.Drawing.SizeF(width, height);
        }

        public string GetDrawText(string text, RectangleF rect, ref TextFormat textFormat,
        TextAlignment ta = TextAlignment.Leading, ParagraphAlignment pa = ParagraphAlignment.Center, float maxTextHeight = 32f,
          FontWeight fontWeight = FontWeight.Normal, FontStyle fontStyle = FontStyle.Normal)
        {
            float edgeWidth = rect.Height * 0.1f;
            if (edgeWidth < 2)
                edgeWidth = 2;
            float fontHeight = rect.Height - edgeWidth;
            if (fontHeight < JoyConst.MinFontSize)
                fontHeight = JoyConst.MinFontSize;
            if (fontHeight > maxTextHeight)
                fontHeight = maxTextHeight;
            GetTextFormat(ref textFormat, fontHeight, ta, pa, fontHeight, fontWeight, fontStyle);
            string newText = text;//.Replace("\n", "");
            System.Drawing.SizeF tempSize = MeasureString("...", textFormat);
            bool loop = true;
            bool indentation = false;
            System.Drawing.SizeF textSize = MeasureString(newText, textFormat);
            if (textSize.Width > rect.Width)
            {
                while (loop)
                {
                    textSize = MeasureString(newText, textFormat);
                    if (textSize.Width + tempSize.Width > rect.Width)
                    {
                        if (newText.Length > 1)
                        {
                            newText = newText.Substring(0, newText.Length - 1);
                            if (newText[newText.Length - 1] == ' ')
                                newText = newText.Substring(0, newText.Length - 1);
                            indentation = true;
                        }
                        else
                            loop = false;
                    }
                    else
                    {
                        loop = false;
                    }
                }
            }
            if (indentation)
                newText += "...";
            return newText;
        }

        public SolidColorBrush GetSolidColorBrush(float r, float g, float b, float a)
        {
            solidColorBrush.Color = new Color4(r, g, b, a);
            return solidColorBrush;
        }

        public SolidColorBrush GetSolidColorBrush(Color4 color)
        {
            solidColorBrush.Color = color;
            return solidColorBrush;
        }

        public SolidColorBrush GetSolidColorBrush(Color4 color, float alpha = 1f)
        {
            solidColorBrush.Color = new Color4(color.Red, color.Green, color.Blue, alpha);
            return solidColorBrush;
        }
    }
}
