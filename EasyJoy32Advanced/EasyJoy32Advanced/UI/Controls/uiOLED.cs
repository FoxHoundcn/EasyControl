using SharpDX;
using SharpDX.Direct2D1;
using SharpDX.DirectWrite;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace EasyControl
{
    public class FontRegion
    {
        public int Index;
        public RectangleF Rect;
        public FontRegion(int _index, RectangleF _rect)
        {
            Index = _index;
            Rect = _rect;
        }
    }
    public class uiOLED : iControl
    {
        static int maxIndex = 0;
        //--------------------------------------------------------------
        public iControl Parent { get; set; }
        public RectangleF DrawRect { get { return Rect; } }
        public RectangleF Rect { private get; set; }
        public Vector2 Offset { get; set; }
        public bool Hide { get; set; } = false;
        public int Index { get; set; }
        public string Name { get; set; }
        public string PluginID { get; set; }
        public string UIKey { set; private get; }
        public bool NodeLinkMode { get; private set; }
        //---------------------------------------------------------------------------------
        const float fontWidth = 16;
        const float fontHeight = 16;
        private TextFormat tf;
        RectangleF drawRect = new RectangleF();
        RectangleF rectInside = new RectangleF();
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
        private LayerParameters lp = new LayerParameters();
        private Layer lay;
        List<RectangleF> oledList = new List<RectangleF>();
        List<FontRegion> regionList = new List<FontRegion>();
        List<Color4> colorList = new List<Color4>()
        {
            XmlUI.DxDeviceGreen,
            XmlUI.DxDeviceBlue,
            XmlUI.DxDeviceYellow,
            XmlUI.DxDevicePurple
        };
        /////////////////////////////////////////////////////////////////////////////////////
        public uiOLED()
        {
            Index = maxIndex;
            maxIndex++;
            Rect = new RectangleF();
            //--------------------------------------------------------------------------------------------
            Dx2DResize();
        }
        private RectangleF GetOledRect(float w, float h, int count, bool rotate, out float fontScale)
        {
            RectangleF rect = new RectangleF();
            float width, height;
            if (rotate)
            {
                width = h;
                height = w;
            }
            else
            {
                width = w;
                height = h;
            }
            float scale = width * count / height;
            if (rectInside.Width / rectInside.Height > scale)
            {
                rect.X = 0f;
                rect.Y = 0f;
                if (rotate)
                {
                    rect.Width = rectInside.Height * scale;
                    rect.Height = rectInside.Height;
                }
                else
                {
                    rect.Width = rectInside.Height * scale;
                    rect.Height = rectInside.Height;
                }
            }
            else
            {
                rect.X = 0f;
                rect.Y = 0f;
                if (rotate)
                {
                    rect.Width = rectInside.Width / scale;
                    rect.Height = rectInside.Width;
                }
                else
                {
                    rect.Width = rectInside.Width;
                    rect.Height = rectInside.Width / scale;
                }
            }
            rect.Width /= count;
            if (rotate)
                fontScale = rect.Height / h;
            else
                fontScale = rect.Width / w;
            return rect;
        }
        public void Update(JoyObject currentObj)
        {
            float fontScale = 0f;
            JoyCustom custom = currentObj.GetCurrentJoyCustom();
            if (custom != null)
            {
                lock (oledList)
                {
                    oledList.Clear();
                    switch (custom.Type)
                    {
                        case CustomType.OLED_70_40_SSD1306://0.42
                        case CustomType.OLED_70_40_SSD1306x2:
                        case CustomType.OLED_70_40_SSD1306x3:
                        case CustomType.OLED_70_40_SSD1306x4:
                        //----
                        case CustomType.OLED_48_64_SSD1306://0.71
                        case CustomType.OLED_48_64_SSD1306x2:
                        case CustomType.OLED_48_64_SSD1306x3:
                        case CustomType.OLED_48_64_SSD1306x4:
                        //----
                        case CustomType.OLED_64_32_SSD1306://0.49
                        case CustomType.OLED_64_32_SSD1306x2:
                        case CustomType.OLED_64_32_SSD1306x3:
                        case CustomType.OLED_64_32_SSD1306x4:
                        //----
                        case CustomType.OLED_64_48_SSD1306://0.66
                        case CustomType.OLED_64_48_SSD1306x2:
                        case CustomType.OLED_64_48_SSD1306x3:
                        case CustomType.OLED_64_48_SSD1306x4:
                        //----
                        case CustomType.OLED_96_16_SSD1306://0.86
                        case CustomType.OLED_96_16_SSD1306x2:
                        case CustomType.OLED_96_16_SSD1306x3:
                        case CustomType.OLED_96_16_SSD1306x4:
                        //----
                        case CustomType.OLED_128_32_SSD1306://0.91
                        case CustomType.OLED_128_32_SSD1306x2:
                        case CustomType.OLED_128_32_SSD1306x3:
                        case CustomType.OLED_128_32_SSD1306x4:
                        //----
                        case CustomType.OLED_128_64_SSD1306://0.96, 1.09
                        case CustomType.OLED_128_64_SSD1306x2:
                        case CustomType.OLED_128_64_SSD1306x3:
                        case CustomType.OLED_128_64_SSD1306x4:
                        //----
                        case CustomType.OLED_128_64_SH1106://1.3
                        case CustomType.OLED_128_64_SH1106x2:
                        case CustomType.OLED_128_64_SH1106x3:
                        case CustomType.OLED_128_64_SH1106x4:
                        //----
                        case CustomType.OLED_128_88_SH1107://0.73
                        case CustomType.OLED_128_88_SH1107x2:
                        case CustomType.OLED_128_88_SH1107x3:
                        case CustomType.OLED_128_88_SH1107x4:
                        //----
                        case CustomType.OLED_256_64_SSD1322://3.12
                        case CustomType.OLED_256_64_SSD1322x2:
                        case CustomType.OLED_256_64_SSD1322x3:
                        case CustomType.OLED_256_64_SSD1322x4:
                            string[] enumStr = custom.Type.ToString().Split('_');
                            string[] contStr = enumStr[3].Split('x');
                            float x = float.Parse(enumStr[1]);
                            float y = float.Parse(enumStr[2]);
                            switch (contStr.Length)
                            {
                                case 1:
                                    RectangleF scaleRect1 = GetOledRect(x, y, 1, (custom.rotateType != RotateType.Rotate0 && custom.rotateType != RotateType.Rotate180), out fontScale);
                                    oledList.Add(new RectangleF(rectInside.X, rectInside.Y, scaleRect1.Width, scaleRect1.Height));
                                    break;
                                case 2:
                                    int oledCount = int.Parse(contStr[1]);
                                    for (int oled = 0; oled < oledCount; oled++)
                                    {
                                        RectangleF scaleRectX = GetOledRect(x, y, oledCount, (custom.rotateType != RotateType.Rotate0 && custom.rotateType != RotateType.Rotate180), out fontScale);
                                        oledList.Add(new RectangleF(rectInside.X + scaleRectX.Width * oled, rectInside.Y, scaleRectX.Width, scaleRectX.Height));
                                    }
                                    break;
                            }
                            break;
                    }
                }
                lock (regionList)
                {
                    regionList.Clear();
                    for (int i = 0; i < custom.dataCount; i++)
                    {
                        RectangleF rect = new RectangleF();
                        int oledIndex = 0;
                        switch (custom.Type)
                        {
                            case CustomType.OLED_70_40_SSD1306://0.42
                            case CustomType.OLED_48_64_SSD1306://0.71
                            case CustomType.OLED_64_32_SSD1306://0.49
                            case CustomType.OLED_64_48_SSD1306://0.66
                            case CustomType.OLED_96_16_SSD1306://0.86
                            case CustomType.OLED_128_32_SSD1306://0.91
                            case CustomType.OLED_128_64_SSD1306://0.96, 1.09
                            case CustomType.OLED_128_64_SH1106://1.3
                            case CustomType.OLED_128_88_SH1107://0.73
                            case CustomType.OLED_256_64_SSD1322://3.12
                                oledIndex = 0;
                                rect = oledList[oledIndex];
                                break;
                            case CustomType.OLED_70_40_SSD1306x2:
                            case CustomType.OLED_48_64_SSD1306x2:
                            case CustomType.OLED_64_32_SSD1306x2:
                            case CustomType.OLED_64_48_SSD1306x2:
                            case CustomType.OLED_96_16_SSD1306x2:
                            case CustomType.OLED_128_32_SSD1306x2:
                            case CustomType.OLED_128_64_SSD1306x2:
                            case CustomType.OLED_128_64_SH1106x2:
                            case CustomType.OLED_128_88_SH1107x2:
                            case CustomType.OLED_256_64_SSD1322x2:
                                oledIndex = i % 2;
                                rect = oledList[oledIndex];
                                break;
                            case CustomType.OLED_70_40_SSD1306x3:
                            case CustomType.OLED_48_64_SSD1306x3:
                            case CustomType.OLED_64_32_SSD1306x3:
                            case CustomType.OLED_64_48_SSD1306x3:
                            case CustomType.OLED_96_16_SSD1306x3:
                            case CustomType.OLED_128_32_SSD1306x3:
                            case CustomType.OLED_128_64_SSD1306x3:
                            case CustomType.OLED_128_64_SH1106x3:
                            case CustomType.OLED_128_88_SH1107x3:
                            case CustomType.OLED_256_64_SSD1322x3:
                                oledIndex = i % 3;
                                rect = oledList[oledIndex];
                                break;
                            case CustomType.OLED_70_40_SSD1306x4:
                            case CustomType.OLED_48_64_SSD1306x4:
                            case CustomType.OLED_64_32_SSD1306x4:
                            case CustomType.OLED_64_48_SSD1306x4:
                            case CustomType.OLED_96_16_SSD1306x4:
                            case CustomType.OLED_128_32_SSD1306x4:
                            case CustomType.OLED_128_64_SSD1306x4:
                            case CustomType.OLED_128_64_SH1106x4:
                            case CustomType.OLED_128_88_SH1107x4:
                            case CustomType.OLED_256_64_SSD1322x4:
                                oledIndex = i % 4;
                                rect = oledList[oledIndex];
                                break;
                        }
                        eFont font = currentObj.GetFont(custom.FontSetList[i].LibIndex);
                        RectangleF drawRect = new RectangleF(rect.X + custom.FontSetList[i].X * fontScale,
                            rect.Y + custom.FontSetList[i].Y * 8 * fontScale,
                            font.FontWidth * fontScale * custom.FontSetList[i].Count,
                            font.FontHeight * 8 * fontScale
                            );
                        regionList.Add(new FontRegion(i, drawRect));
                    }
                }
            }
        }

        public void Dx2DResize()
        {
            if (Hide) return;
            string drawText = Dx2D.Instance.GetDrawText("9", new RectangleF(0, 0, fontWidth, fontHeight), ref tf, TextAlignment.Center);
        }

        public void DxRenderLogic()
        {
            if (Hide) return;
            edgeWidth = (int)(Math.Min(Rect.Height, Rect.Width) * 0.02f);
            drawRect = PublicData.GetActualRange(Rect, edgeWidth);
            rectInside = PublicData.GetActualRange(drawRect, edgeWidth * 2);
        }

        public void DxRenderHigh()
        {
            if (Hide) return;
        }

        public void DxRenderMedium()
        {
            if (Hide) return;
            lp.ContentBounds = Rect;
            lp.LayerOptions = LayerOptions.InitializeForCleartype;
            lp.Opacity = 1f;
            lay = new Layer(Dx2D.Instance.RenderTarget2D);
            Dx2D.Instance.RenderTarget2D.PushLayer(ref lp, lay);
            #region 绘制
            Dx2D.Instance.RenderTarget2D.DrawRectangle(drawRect, Dx2D.Instance.GetSolidColorBrush(XmlUI.DxUIClickColor), edgeWidth);
            lock (oledList)
            {
                for (int i = 0; i < oledList.Count; i++)
                {
                    Dx2D.Instance.RenderTarget2D.DrawRectangle(oledList[i], Dx2D.Instance.GetSolidColorBrush(colorList[i]), 3f);
                }
            }
            lock (regionList)
            {
                for (int i = 0; i < regionList.Count; i++)
                {
                    Dx2D.Instance.RenderTarget2D.DrawRectangle(regionList[i].Rect, Dx2D.Instance.GetSolidColorBrush(XmlUI.DxDeviceRed), 1f);
                    Dx2D.Instance.RenderTarget2D.DrawText(regionList[i].Index.ToString(), tf,
                        new RectangleF(regionList[i].Rect.X, regionList[i].Rect.Y, fontWidth, fontHeight), Dx2D.Instance.GetSolidColorBrush(XmlUI.DxDeviceRed));
                }
            }
            #endregion
            Dx2D.Instance.RenderTarget2D.PopLayer();
            lay.Dispose();
        }

        public void DxRenderLow()
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

        public void JoyMouseMoveEvent(MouseEventArgs e)
        {
            if (Hide) return;
        }

        public void JoyMouseMoveWheel(MouseEventArgs e)
        {
            if (Hide) return;
        }
    }
}
