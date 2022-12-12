//#define fontDebug
using System;
using System.Text;
using System.Windows.Forms;

namespace EasyControl
{
    public class FontLibraryControl : iUiLogic
    {
        uiButton btnSetFont;
        uiButton btnGetFont;
        uiTextLable tlSizeValue;
        uiTrackBar tbMaxWidth;
        uiTrackBar tbMaxHeight;
        uiButton btnUp;
        uiButton btnLeft;
        uiButton btnRight;
        uiButton btnDown;
        uiSwitchButton sbRed;
        uiSwitchButton sbGreen;
        uiSwitchButton sbBlue;
        uiTrackBar tbFontScale;
        uiTextEditor teFontX, teFontY;
        uiButton btnClean;
        uiButton btnOutput;
        uiTrackBar tbCharStart, tbCharEnd;
        uiSwitchButton rotate0, rotate90, rotate180, rotate270;
        //---------------------------------------------------------------------------------------
        JoyObject currentObj = null;
        public RotateType rotateType = RotateType.Rotate0;
        private bool changeMatrixSwitch = false;
        private bool changeMatrixState = false;

        private byte[] copyFontList;
        /////////////////////////////////////////////////////////////////////////////////////////
        public static readonly FontLibraryControl Instance = new FontLibraryControl();
        private FontLibraryControl()
        {
        }
        public void Init()
        {
            btnSetFont = XmlUI.Instance.GetButton("JoySetFont");
            btnSetFont.LeftButtonClick += OnSetFontClick;
            btnGetFont = XmlUI.Instance.GetButton("JoyGetFont");
            btnGetFont.LeftButtonClick += OnGetFontClick;
            tlSizeValue = XmlUI.Instance.GetTextLable("FontSizeValue");
            tbMaxWidth = XmlUI.Instance.GetTrackBar("FontWidthTB");
            if (tbMaxWidth != null)
            {
                tbMaxWidth.ValueChange += OnWidthChange;
            }
            tbMaxHeight = XmlUI.Instance.GetTrackBar("FontHeighTB");
            if (tbMaxHeight != null)
            {
                tbMaxHeight.ValueChange += OnHeightChange;
            }
            btnUp = XmlUI.Instance.GetButton("FontUpButton");
            btnUp.LeftButtonClick += OnUpClick;
            btnLeft = XmlUI.Instance.GetButton("FontLeftButton");
            btnLeft.LeftButtonClick += OnLeftClick;
            btnRight = XmlUI.Instance.GetButton("FontRightButton");
            btnRight.LeftButtonClick += OnRightClick;
            btnDown = XmlUI.Instance.GetButton("FontDownButton");
            btnDown.LeftButtonClick += OnDownClick;
            sbRed = XmlUI.Instance.GetSwitchButton("fontRedSwitch");
            sbRed.ValueChange += OnRedSwitch;
            sbRed.bSwitchOn = true;
            tbFontScale = XmlUI.Instance.GetTrackBar("FontScaleTB");
            tbFontScale.ValueChange += OnFontScaleChange;
            tbFontScale.Value = 10;
            teFontX = XmlUI.Instance.GetTextEditor("FontOffsetXTE");
            teFontX.TextChange += OnFontXChange;
            teFontY = XmlUI.Instance.GetTextEditor("FontOffsetYTE");
            teFontY.TextChange += OnFontYChange;
            sbGreen = XmlUI.Instance.GetSwitchButton("fontGreenSwitch");
            sbGreen.ValueChange += OnGreenSwitch;
            sbBlue = XmlUI.Instance.GetSwitchButton("fontBlueSwitch");
            sbBlue.ValueChange += OnBlueSwitch;
            btnClean = XmlUI.Instance.GetButton("FontCleanButton");
            btnClean.LeftButtonClick += OnCleanClick;
            btnOutput = XmlUI.Instance.GetButton("FontOutputButton");
            btnOutput.LeftButtonClick += OnOutputClick;
            for (int i = 0; i < JoyConst.MaxFontCount; i++)
            {
                uiButton btn = XmlUI.Instance.GetButton("FontIndexButton" + i);
                if (btn != null)
                {
                    byte stringBtye = System.Convert.ToByte(' ');
                    byte[] byteList = new byte[2] { (byte)(stringBtye + i), System.Convert.ToByte('\0') };
                    btn.Name = Encoding.Default.GetString(byteList);
                    btn.Index = i;
                    btn.RightButtonClick += OnFontIndexButtonClick;
                }
            }
            tbCharStart = XmlUI.Instance.GetTrackBar("FontStartTB");
            tbCharStart.ValueChange += OnCharStartChange;
            tbCharEnd = XmlUI.Instance.GetTrackBar("FontEndTB");
            tbCharEnd.ValueChange += OnCharEndChange;
            rotate0 = XmlUI.Instance.GetSwitchButton("FontRotate0");
            rotate0.ValueChange += OnRorate0Click;
            rotate90 = XmlUI.Instance.GetSwitchButton("FontRotate90");
            rotate90.ValueChange += OnRorate90Click;
            rotate180 = XmlUI.Instance.GetSwitchButton("FontRotate180");
            rotate180.ValueChange += OnRorate180Click;
            rotate270 = XmlUI.Instance.GetSwitchButton("FontRotate270");
            rotate270.ValueChange += OnRorate270Click;
            for (int i = 0; i < JoyConst.MaxFontWidth * JoyConst.MaxFontHeight * 8; i++)
            {
                uiPanel btn = XmlUI.Instance.GetPanel("FontMatrixButton" + i);
                if (btn != null)
                {
                    btn.Index = i;
                    btn.MouseLeftDownEvent += OnMatrixButtonDown;
                    btn.MouseLeftClickEvent += OnMatrixButtonClick;
                    btn.MouseEnterEvent += OnMatrixButtonEnter;
                }
            }
            JoyEvent.Instance.CutClick += OnCutClick;
            JoyEvent.Instance.CopyClick += OnCopyClick;
            JoyEvent.Instance.PasteClick += OnPasteClick;
        }
        #region x,c,v
        private void CopyCurrentFont()
        {
            currentObj = PublicData.GetCurrentSelectJoyObject();
            if (currentObj != null)
            {
                eFont font = currentObj.GetCurrentFont();
                if (font != null)
                {
                    int index = font.SelectFontIndex;
                    copyFontList = new byte[font.FontSize];
                    for (int i = 0; i < font.FontSize; i++)
                    {
                        byte charData;
                        if (currentObj.GetFontLibrary(font, index, i, out charData))
                            copyFontList[i] = charData;
                    }
                }
            }
        }
        private void OnCutClick(object sender, EventArgs e)
        {
            CopyCurrentFont();
            OnCleanClick(null, null);
        }
        private void OnCopyClick(object sender, EventArgs e)
        {
            CopyCurrentFont();
        }
        private void OnPasteClick(object sender, EventArgs e)
        {
            currentObj = PublicData.GetCurrentSelectJoyObject();
            if (currentObj != null)
            {
                eFont font = currentObj.GetCurrentFont();
                if (font != null)
                {
                    int index = font.SelectFontIndex;
                    int widthByte = font.FontWidth;
                    int heightByte = font.FontHeight;
                    int fontSize = widthByte * heightByte;
                    if (copyFontList == null || copyFontList.Length != fontSize)
                    {
                        return;
                    }
                    else
                    {
                        for (int i = 0; i < fontSize; i++)
                        {
                            currentObj.SetFontLibrary(font, index, i, copyFontList[i]);
                        }
                    }
                }
            }
        }
        #endregion
        #region Save&Load
        private void OnSetFontClick(object sender, EventArgs e)
        {
            JoyObject joyObj = PublicData.GetCurrentSelectJoyObject();
            if (joyObj != null)
            {
                string logMsg;
                PublicData.SaveFont(false, joyObj, "eFont_" + joyObj.McuID + "_" + DateTime.Now.ToString("yyyy_MM_dd_HH_mm_ssffff"), out logMsg);
                joyObj.AddReport(new Report(ReportType.ClearFont));
                joyObj.AddReport(new Report(ReportType.SyncFont0));
                joyObj.AddReport(new Report(ReportType.SyncFont1));
                joyObj.AddReport(new Report(ReportType.SyncFont2));
                joyObj.AddReport(new Report(ReportType.SyncFont3));
                joyObj.AddReport(new Report(ReportType.SyncFont4));
                joyObj.AddReport(new Report(ReportType.SyncFont5));
                joyObj.AddReport(new Report(ReportType.SyncFont6));
                joyObj.AddReport(new Report(ReportType.SyncFont7));
                joyObj.AddReport(new Report(ReportType.SaveFont));
            }
        }
        private void OnGetFontClick(object sender, EventArgs e)
        {
            JoyObject joyObj = PublicData.GetCurrentSelectJoyObject();
            if (joyObj != null)
            {
                joyObj.AddReport(new Report(ReportType.GetFont0));
                joyObj.AddReport(new Report(ReportType.GetFont1));
                joyObj.AddReport(new Report(ReportType.GetFont2));
                joyObj.AddReport(new Report(ReportType.GetFont3));
                joyObj.AddReport(new Report(ReportType.GetFont4));
                joyObj.AddReport(new Report(ReportType.GetFont5));
                joyObj.AddReport(new Report(ReportType.GetFont6));
                joyObj.AddReport(new Report(ReportType.GetFont7));
                joyObj.AddReport(new Report(ReportType.GetFontOver));
            }
        }
        #endregion
        #region 上下左右
        private void FontToUp()
        {
            if (currentObj != null)
            {
                eFont font = currentObj.GetCurrentFont();
                if (font != null)
                {
                    int index = font.SelectFontIndex;
                    for (int x = 0; x < font.FontWidth; x++)
                    {
                        for (int y = 0; y < font.FontHeight; y++)
                        {
                            if (y < font.FontHeight - 1)
                            {
                                byte data, data1;
                                if (currentObj.GetFontLibrary(font, index, font.FontWidth * y + x, out data) &&
                                currentObj.GetFontLibrary(font, index, font.FontWidth * (y + 1) + x, out data1))
                                {
                                    int temp = data + (data1 << 8);
                                    temp = temp >> 1;
                                    currentObj.SetFontLibrary(font, index, font.FontWidth * y + x, (byte)(temp));
                                }
                            }
                            else
                            {
                                byte data;
                                if (currentObj.GetFontLibrary(font, index, font.FontWidth * y + x, out data))
                                {
                                    currentObj.SetFontLibrary(font, index, font.FontWidth * y + x, (byte)(data >> 1));
                                }
                            }
                        }
                    }
                }
            }
        }
        private void OnUpClick(object sender, EventArgs e)
        {
            switch (rotateType)
            {
                case RotateType.Rotate0:
                case RotateType.Rotate180:
                    FontToUp();
                    break;
                case RotateType.Rotate90:
                case RotateType.Rotate270:
                    FontToRight();
                    break;
            }
        }
        private void FontToDown()
        {
            if (currentObj != null)
            {
                eFont font = currentObj.GetCurrentFont();
                if (font != null)
                {
                    int index = font.SelectFontIndex;
                    for (int x = 0; x < font.FontWidth; x++)
                    {
                        for (int y = font.FontHeight - 1; y >= 0; y--)
                        {
                            if (y > 0)
                            {
                                byte data, data1;
                                if (currentObj.GetFontLibrary(font, index, font.FontWidth * y + x, out data) &&
                                currentObj.GetFontLibrary(font, index, font.FontWidth * (y - 1) + x, out data1))
                                {
                                    int temp = (data << 8) + data1;
                                    temp = temp << 1;
                                    currentObj.SetFontLibrary(font, index, font.FontWidth * y + x, (byte)(temp >> 8));
                                }
                            }
                            else
                            {
                                byte data;
                                if (currentObj.GetFontLibrary(font, index, font.FontWidth * y + x, out data))
                                {
                                    currentObj.SetFontLibrary(font, index, font.FontWidth * y + x, (byte)(data << 1));
                                }
                            }
                        }
                    }
                }
            }
        }
        private void OnDownClick(object sender, EventArgs e)
        {
            switch (rotateType)
            {
                case RotateType.Rotate0:
                case RotateType.Rotate180:
                    FontToDown();
                    break;
                case RotateType.Rotate90:
                case RotateType.Rotate270:
                    FontToLeft();
                    break;
            }
        }
        private void FontToLeft()
        {
            if (currentObj != null)
            {
                eFont font = currentObj.GetCurrentFont();
                if (font != null)
                {
                    int index = font.SelectFontIndex;
                    for (int x = 0; x < font.FontWidth; x++)
                    {
                        for (int y = 0; y < font.FontHeight * 8; y++)
                        {
                            int yByte = y / 8;
                            int yBit = y % 8;
                            if (x < font.FontWidth - 1)
                            {
                                byte data;
                                if (currentObj.GetFontLibrary(font, index, font.FontWidth * yByte + x + 1, out data))
                                {
                                    currentObj.SetFontLibrary(font, index, font.FontWidth * yByte + x, data);
                                }
                            }
                            else
                            {
                                currentObj.SetFontLibrary(font, index, font.FontWidth * yByte + x, 0);
                            }
                        }
                    }
                }
            }
        }
        private void OnLeftClick(object sender, EventArgs e)
        {
            switch (rotateType)
            {
                case RotateType.Rotate0:
                case RotateType.Rotate180:
                    FontToLeft();
                    break;
                case RotateType.Rotate90:
                case RotateType.Rotate270:
                    FontToUp();
                    break;
            }
        }
        private void FontToRight()
        {
            if (currentObj != null)
            {
                eFont font = currentObj.GetCurrentFont();
                if (font != null)
                {
                    int index = font.SelectFontIndex;
                    for (int x = font.FontWidth - 1; x >= 0; x--)
                    {
                        for (int y = 0; y < font.FontHeight * 8; y++)
                        {
                            int yByte = y / 8;
                            int yBit = y % 8;
                            if (x > 0)
                            {
                                byte data;
                                if (currentObj.GetFontLibrary(font, index, font.FontWidth * yByte + x - 1, out data))
                                {
                                    currentObj.SetFontLibrary(font, index, font.FontWidth * yByte + x, data);
                                }
                            }
                            else
                            {
                                currentObj.SetFontLibrary(font, index, font.FontWidth * yByte + x, 0);
                            }
                        }
                    }
                }
            }
        }
        private void OnRightClick(object sender, EventArgs e)
        {
            switch (rotateType)
            {
                case RotateType.Rotate0:
                case RotateType.Rotate180:
                    FontToRight();
                    break;
                case RotateType.Rotate90:
                case RotateType.Rotate270:
                    FontToDown();
                    break;
            }
        }
        #endregion
        private void OnCleanClick(object sender, EventArgs e)
        {
            if (currentObj != null)
            {
                eFont font = currentObj.GetCurrentFont();
                if (font != null)
                {
                    if (MessageBox.Show(Localization.Instance.GetLS("FontCleanConfirm"), Localization.Instance.GetLS("Warning"), MessageBoxButtons.OKCancel) == DialogResult.OK)
                    {
                        int index = font.SelectFontIndex;
                        for (int x = 0; x < font.FontWidth; x++)
                        {
                            for (int y = 0; y < font.FontHeight * 8; y++)
                            {
                                int yByte = y / 8;
                                int yBit = y % 8;
                                currentObj.SetFontLibrary(font, index, font.FontWidth * yByte + x, 0);
                            }
                        }
                    }
                }
            }
        }
        private void OnOutputClick(object sender, EventArgs e)
        {
            SaveFileForm.Instance.SaveFontLib();
        }
        #region Color
        private void OnRedSwitch(object sender, EventArgs e)
        {
            sbRed.bSwitchOn = true;
            sbGreen.bSwitchOn = false;
            sbBlue.bSwitchOn = false;
            currentObj.FontColor = XmlUI.DxDeviceRed;
        }
        private void OnGreenSwitch(object sender, EventArgs e)
        {
            sbRed.bSwitchOn = false;
            sbGreen.bSwitchOn = true;
            sbBlue.bSwitchOn = false;
            currentObj.FontColor = XmlUI.DxDeviceGreen;
        }
        private void OnBlueSwitch(object sender, EventArgs e)
        {
            sbRed.bSwitchOn = false;
            sbGreen.bSwitchOn = false;
            sbBlue.bSwitchOn = true;
            currentObj.FontColor = XmlUI.DxDeviceBlue;
        }
        #endregion
        private void OnFontScaleChange(object sender, EventArgs e)
        {
            PublicData.fontScale = tbFontScale.Value / 10f;
        }
        private void OnFontXChange(object sender, EventArgs e)
        {
            int x = 0;
            if (!int.TryParse(teFontX.Text, out x))
            {
                x = 0;
            }
            PublicData.fontOffsetX = x;
        }
        private void OnFontYChange(object sender, EventArgs e)
        {
            int y = 0;
            if (!int.TryParse(teFontY.Text, out y))
            {
                y = 0;
            }
            PublicData.fontOffsetY = y;
        }
        private void OnCharStartChange(object sender, EventArgs e)
        {
            if (currentObj != null)
            {
                eFont font = currentObj.GetCurrentFont();
                font.StartChar = (byte)tbCharStart.Value;
                tlSizeValue.Text = (font.FontWidth * font.FontHeight * font.MaxChar).ToString() + "/" + JoyConst.MaxFontLibLengh.ToString();
            }
        }
        private void OnCharEndChange(object sender, EventArgs e)
        {
            if (currentObj != null)
            {
                eFont font = currentObj.GetCurrentFont();
                font.EndChar = (byte)tbCharEnd.Value;
                tlSizeValue.Text = (font.FontWidth * font.FontHeight * font.MaxChar).ToString() + "/" + JoyConst.MaxFontLibLengh.ToString();
            }
        }
        #region Rorate
        private void OnRorate0Click(object sender, EventArgs e)
        {
            rotateType = RotateType.Rotate0;
        }
        private void OnRorate90Click(object sender, EventArgs e)
        {
            rotateType = RotateType.Rotate90;
        }
        private void OnRorate180Click(object sender, EventArgs e)
        {
            rotateType = RotateType.Rotate180;
        }
        private void OnRorate270Click(object sender, EventArgs e)
        {
            rotateType = RotateType.Rotate270;
        }
        #endregion
        #region Width&Height
        private void OnWidthChange(object sender, EventArgs e)
        {
            if (currentObj != null)
            {
                eFont font = currentObj.GetCurrentFont();
                font.FontWidth = (byte)tbMaxWidth.Value;
                tlSizeValue.Text = font.MaxCharLength.ToString() + "/" + JoyConst.MaxFontLibLengh.ToString();
            }
        }
        private void OnHeightChange(object sender, EventArgs e)
        {
            if (currentObj != null)
            {
                eFont font = currentObj.GetCurrentFont();
                font.FontHeight = (byte)tbMaxHeight.Value;
                tlSizeValue.Text = font.MaxCharLength.ToString() + "/" + JoyConst.MaxFontLibLengh.ToString();
            }
        }
        #endregion
        private void OnFontIndexButtonClick(object sender, EventArgs e)
        {
            JoyIndexChangeArgs args = (JoyIndexChangeArgs)e;
            if (currentObj != null)
            {
                eFont font = currentObj.GetCurrentFont();
                if (font != null)
                {
                    font.SelectFontIndex = args.Index;
                }
            }
        }
        private void OnMatrixButtonEnter(object sender, EventArgs e)
        {
            if (changeMatrixSwitch)
                if (currentObj != null)
                {
                    eFont font = currentObj.GetCurrentFont();
                    if (font != null)
                    {
                        JoyIndexChangeArgs args = (JoyIndexChangeArgs)e;
                        int x = args.Index % JoyConst.MaxFontWidth;
                        int y = args.Index / (JoyConst.MaxFontHeight * 8);
                        int index = font.SelectFontIndex;
                        byte setByte, bitPos;
                        switch (rotateType)
                        {
                            case RotateType.Rotate0:
                            case RotateType.Rotate180:
                                int yByte = y / 8;
                                int yBit = y % 8;
                                if (currentObj.GetFontLibrary(font, index, font.FontWidth * yByte + x, out setByte))
                                {
                                    bitPos = (byte)(0x01 << yBit);
                                    if (changeMatrixState)
                                    {
                                        setByte &= (byte)(~bitPos);
                                    }
                                    else
                                    {
                                        setByte |= bitPos;
                                    }
                                    currentObj.SetFontLibrary(font, index, font.FontWidth * yByte + x, setByte);
                                }
                                break;
                            case RotateType.Rotate90:
                            case RotateType.Rotate270:
                                int xByte = x / 8;
                                int xBit = x % 8;
                                if (currentObj.GetFontLibrary(font, index, font.FontWidth * xByte + y, out setByte))
                                {
                                    bitPos = (byte)(0x01 << xBit);
                                    if (changeMatrixState)
                                    {
                                        setByte &= (byte)(~bitPos);
                                    }
                                    else
                                    {
                                        setByte |= bitPos;
                                    }
                                    currentObj.SetFontLibrary(font, index, font.FontWidth * xByte + y, setByte);
                                }
                                break;
                        }
                    }
                }
        }
        private void OnMatrixButtonDown(object sender, EventArgs e)
        {
            if (currentObj != null)
            {
                eFont font = currentObj.GetCurrentFont();
                if (font != null)
                {
                    JoyIndexChangeArgs args = (JoyIndexChangeArgs)e;
                    int x = args.Index % JoyConst.MaxFontWidth;
                    int y = args.Index / (JoyConst.MaxFontHeight * 8);
                    int index = font.SelectFontIndex;
                    byte setByte, bitPos;
                    switch (rotateType)
                    {
                        case RotateType.Rotate0:
                        case RotateType.Rotate180:
                            int yByte = y / 8;
                            int yBit = y % 8;
                            if (currentObj.GetFontLibrary(font, index, font.FontWidth * yByte + x, out setByte))
                            {
                                bitPos = (byte)(0x01 << yBit);
                                if ((setByte & bitPos) == bitPos)
                                {
                                    setByte ^= bitPos;
                                    changeMatrixSwitch = true;
                                    changeMatrixState = true;
                                }
                                else
                                {
                                    setByte |= bitPos;
                                    changeMatrixSwitch = true;
                                    changeMatrixState = false;
                                }
                                currentObj.SetFontLibrary(font, index, font.FontWidth * yByte + x, setByte);
                            }
                            break;
                        case RotateType.Rotate90:
                        case RotateType.Rotate270:
                            int xByte = x / 8;
                            int xBit = x % 8;
                            if (currentObj.GetFontLibrary(font, index, font.FontWidth * xByte + y, out setByte))
                            {
                                bitPos = (byte)(0x01 << xBit);
                                if ((setByte & bitPos) == bitPos)
                                {
                                    setByte ^= bitPos;
                                    changeMatrixSwitch = true;
                                    changeMatrixState = true;
                                }
                                else
                                {
                                    setByte |= bitPos;
                                    changeMatrixSwitch = true;
                                    changeMatrixState = false;
                                }
                                currentObj.SetFontLibrary(font, index, font.FontWidth * xByte + y, setByte);
                            }
                            break;
                    }
                }
            }
        }
        private void OnMatrixButtonClick(object sender, EventArgs e)
        {
            changeMatrixSwitch = false;
        }
        public void DxRenderLogic()
        {
            currentObj = PublicData.GetCurrentSelectJoyObject();
            if (currentObj != null)
            {
                switch (rotateType)
                {
                    case RotateType.Rotate0:
                        rotate0.bSwitchOn = true;
                        rotate90.bSwitchOn = false;
                        rotate180.bSwitchOn = false;
                        rotate270.bSwitchOn = false;
                        break;
                    case RotateType.Rotate90:
                        rotate0.bSwitchOn = false;
                        rotate90.bSwitchOn = true;
                        rotate180.bSwitchOn = false;
                        rotate270.bSwitchOn = false;
                        break;
                    case RotateType.Rotate180:
                        rotate0.bSwitchOn = false;
                        rotate90.bSwitchOn = false;
                        rotate180.bSwitchOn = true;
                        rotate270.bSwitchOn = false;
                        break;
                    case RotateType.Rotate270:
                        rotate0.bSwitchOn = false;
                        rotate90.bSwitchOn = false;
                        rotate180.bSwitchOn = false;
                        rotate270.bSwitchOn = true;
                        break;
                }
                int maxFontByte = JoyConst.MaxFontLibLengh;
                for (int i = 0; i < JoyConst.MaxFont; i++)
                {
                    eFont font = currentObj.GetFont(i);
                    maxFontByte -= font.MaxCharLength;
                    uiPanel fontColor = XmlUI.Instance.GetPanel(i + "FontColor");
                    if (fontColor != null)
                    {
                        if (maxFontByte < 0)
                        {
                            fontColor.ForeColor = XmlUI.DxDeviceRed;
                        }
                        else
                        {
                            fontColor.ForeColor = XmlUI.DxDeviceGreen;
                        }
                    }
                }
                eFont currentFont = currentObj.GetCurrentFont();
                if (currentFont != null)
                {
                    if (tbMaxWidth.Value != currentFont.FontWidth)
                        tbMaxWidth.Value = currentFont.FontWidth;
                    if (tbMaxHeight.Value != currentFont.FontHeight)
                        tbMaxHeight.Value = currentFont.FontHeight;
                    if (tbCharStart.Value != currentFont.StartChar)
                        tbCharStart.Value = currentFont.StartChar;
                    if (tbCharEnd.Value != currentFont.EndChar)
                        tbCharEnd.Value = currentFont.EndChar;
                    PublicData.SetButtonSelect("FontIndexButton", currentFont.SelectFontIndex, JoyConst.MaxFontCount);
                    PublicData.SetButtonEnable("FontIndexButton", currentFont.GetCharEnable());
                    int index = currentFont.SelectFontIndex;
                    for (int x = 0; x < JoyConst.MaxFontWidth; x++)
                    {
                        for (int y = 0; y < JoyConst.MaxFontHeight * 8; y++)
                        {
                            uiPanel panel = XmlUI.Instance.GetPanel("FontMatrixButton" + (y * JoyConst.MaxFontWidth + x));
                            if (panel != null)
                            {
                                switch (rotateType)
                                {
                                    case RotateType.Rotate0:
                                    case RotateType.Rotate180:
                                        if (x < currentFont.FontWidth && y < currentFont.FontHeight * 8)
                                        {
                                            panel.Hide = false;
                                            int yByte = y / 8;
                                            int yBit = y % 8;
                                            byte setByte;
                                            if (currentObj.GetFontLibrary(currentFont, index, currentFont.FontWidth * yByte + x, out setByte))
                                            {
                                                byte bitPos = (byte)(0x01 << yBit);
                                                if ((setByte & bitPos) == bitPos)
                                                {
                                                    panel.ForeColor = currentObj.FontColor;
                                                }
                                                else
                                                {
                                                    panel.ForeColor = XmlUI.DxUIBackColor;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            panel.Hide = true;
                                        }
                                        break;
                                    case RotateType.Rotate90:
                                    case RotateType.Rotate270:
                                        if (x < currentFont.FontHeight * 8 && y < currentFont.FontWidth)
                                        {
                                            panel.Hide = false;
                                            int xByte = x / 8;
                                            int xBit = x % 8;
                                            byte setByte;
                                            if (currentObj.GetFontLibrary(currentFont, index, currentFont.FontWidth * xByte + y, out setByte))
                                            {
                                                byte bitPos = (byte)(0x01 << xBit);
                                                if ((setByte & bitPos) == bitPos)
                                                {
                                                    panel.ForeColor = currentObj.FontColor;
                                                }
                                                else
                                                {
                                                    panel.ForeColor = XmlUI.DxUIBackColor;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            panel.Hide = true;
                                        }
                                        break;
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
