using System;
using System.Drawing;
using System.Drawing.Text;
using System.IO;
using System.Windows.Forms;

namespace EasyControl
{
    public class FontTypeSelect : iUiLogic
    {
        //---------------------------------------------------------------------------------------
        LayoutControl selectTypeControl;
        LayoutControl fontTypeLC;
        float lcHeight = 0;
        int allFontType = 0;
        ////////////////////////////////////////////////////////////////////////////////////
        public static readonly FontTypeSelect Instance = new FontTypeSelect();
        private FontTypeSelect()
        {
        }
        public void Init()
        {
            allFontType = 0;
            selectTypeControl = XmlUI.Instance.GetLayoutControl("SelectTypeControl");
            fontTypeLC = XmlUI.Instance.GetLayoutControl("vcFontTypeLC");
            string fontPath = System.Environment.CurrentDirectory + @"\Fonts";
            if (Directory.Exists(fontPath))
            {
                string[] fileList = Directory.GetFiles(fontPath);
                for (int i = 0; i < fileList.Length; i++)
                {
                    if (Path.GetExtension(fileList[i]).Equals(".ecf"))
                    {
                        string fileName = Path.GetFileNameWithoutExtension(fileList[i]);
                        byte[] fileData = File.ReadAllBytes(fileList[i]);
                        eFont ft = new eFont();
                        ft.FontWidth = fileData[0];
                        ft.FontHeight = fileData[1];
                        ft.StartChar = fileData[2];
                        ft.EndChar = fileData[3];
                        for (int dataIndex = 0; dataIndex < fileData.Length - 4; dataIndex++)
                        {
                            ft.SetFontByte(dataIndex, fileData[4 + dataIndex]);
                        }
                        PublicData.fontTypeList.Add(ft);
                        //==============================================================
                        LayoutControl newType = XmlUI.Instance.CreateUI(System.Environment.CurrentDirectory + @"\Xml\FontType.xml", fileName);
                        fontTypeLC.AddObject(newType);
                        LayoutControl lcDevice = XmlUI.Instance.GetLayoutControl(fileName + "FontTypeLC");
                        lcHeight = lcDevice.maxHeight;
                        uiButton btn = XmlUI.Instance.GetButton(fileName + "FontTypeIndex");
                        if (btn != null)
                        {
                            btn.Index = allFontType;
                            btn.Name = fileName;
                            btn.LeftButtonClick += OnTypeChangeClick;
                        }
                        uiPanel pan = XmlUI.Instance.GetPanel(fileName + "FontModeColor");
                        pan.ForeColor = XmlUI.DxDeviceGreen;
                        allFontType++;
                    }
                }
            }
            InstalledFontCollection allFont = new InstalledFontCollection();
            FontFamily[] allFontFamilies = allFont.Families;
            for (int i = 0; i < allFontFamilies.Length; i++)
            {
                string name = allFontFamilies[i].Name;
                if (!name.Equals(""))
                {
                    LayoutControl newType = XmlUI.Instance.CreateUI(System.Environment.CurrentDirectory + @"\Xml\FontType.xml", name);
                    fontTypeLC.AddObject(newType);
                    LayoutControl lcDevice = XmlUI.Instance.GetLayoutControl(name + "FontTypeLC");
                    lcHeight = lcDevice.maxHeight;
                    uiButton btn = XmlUI.Instance.GetButton(name + "FontTypeIndex");
                    if (btn != null)
                    {
                        btn.Index = allFontType;
                        btn.Name = name;
                        btn.LeftButtonClick += OnFamiliesChangeClick;
                    }
                    uiPanel pan = XmlUI.Instance.GetPanel(name + "FontModeColor");
                    pan.ForeColor = XmlUI.DxDeviceBlue;
                    allFontType++;
                }
            }
        }
        public Bitmap GetCharBMP(string str, string font, int w, int h)
        {
            StringFormat sf = new StringFormat(); // 设置格式
            sf.Alignment = StringAlignment.Center;
            sf.LineAlignment = StringAlignment.Near;
            Bitmap bmp = new Bitmap(w, h); // 新建位图变量
            Graphics g = Graphics.FromImage(bmp);
            float fontW = w < h ? w : h;
            g.DrawString(str, new Font(font, fontW * PublicData.fontScale), Brushes.Black,
                new RectangleF(PublicData.fontOffsetX, PublicData.fontOffsetY, w + (-PublicData.fontOffsetX * 2), h + (-PublicData.fontOffsetY * 2)), sf); // 向图像变量输出字符
            return bmp;
        }
        private void OnFamiliesChangeClick(object sender, EventArgs e)
        {
            uiButton btn = sender as uiButton;
            if (btn != null)
            {
                JoyObject joyObj = PublicData.GetCurrentSelectJoyObject();
                if (joyObj != null)
                {
                    eFont currentFont = joyObj.GetCurrentFont();
                    if (currentFont != null)
                    {
                        string name = btn.Name;
                        for (char i = ' '; i <= '~'; i++)
                        {
                            int height = currentFont.FontHeight * 8;
                            Bitmap bmp;
                            switch (FontLibraryControl.Instance.rotateType)
                            {
                                case RotateType.Rotate0:
                                case RotateType.Rotate180:
                                    bmp = GetCharBMP(i.ToString(), name, currentFont.FontWidth, height);
                                    for (int w = 0; w < bmp.Width; w++)
                                    {
                                        byte temp = 0;
                                        for (int h = 0; h < bmp.Height; h++)
                                        {
                                            if (bmp.GetPixel(w, h) == Color.FromArgb(0, 0, 0))// 以下几行根据点阵格式计算它的十六进制并写入
                                                temp += (byte)(0x01 << (h % 8));
                                            if (h % 8 == 7)
                                            {
                                                joyObj.SetFontLibrary(currentFont, (i - ' '), currentFont.FontWidth * (h / 8) + w, temp);
                                                temp = 0;
                                            }
                                        }
                                    }
                                    break;
                                case RotateType.Rotate90:
                                case RotateType.Rotate270:
                                    bmp = GetCharBMP(i.ToString(), name, height, currentFont.FontWidth);
                                    for (int h = 0; h < bmp.Height; h++)
                                    {
                                        byte temp = 0;
                                        for (int w = 0; w < bmp.Width; w++)
                                        {
                                            if (bmp.GetPixel(w, h) == Color.FromArgb(0, 0, 0))// 以下几行根据点阵格式计算它的十六进制并写入
                                                temp += (byte)(0x01 << (w % 8));
                                            if (w % 8 == 7)
                                            {
                                                joyObj.SetFontLibrary(currentFont, (i - ' '), currentFont.FontWidth * (w / 8) + h, temp);
                                                temp = 0;
                                            }
                                        }
                                    }
                                    break;
                            }
                        }
                    }
                }
            }
        }
        private void OnTypeChangeClick(object sender, EventArgs e)
        {
            JoyIndexChangeArgs args = (JoyIndexChangeArgs)e;
            JoyObject joyObj = PublicData.GetCurrentSelectJoyObject();
            if (joyObj != null)
            {
                if (args.Index >= 0 && args.Index < PublicData.fontTypeList.Count)
                {
                    eFont ft = PublicData.fontTypeList[args.Index];
                    eFont currentFont = joyObj.GetCurrentFont();
                    if (currentFont != null && ft != null)
                    {
                        bool apply = true;
                        if (currentFont.FontWidth != ft.FontWidth || currentFont.FontHeight != ft.FontHeight)
                        {
                            if (MessageBox.Show(Localization.Instance.GetLS("FontTypeConfirm"), Localization.Instance.GetLS("Warning"), MessageBoxButtons.OKCancel) != DialogResult.OK)
                            {
                                apply = false;
                            }
                        }
                        else
                        {
                            if (MessageBox.Show(Localization.Instance.GetLS("FontApplyConfirm"), Localization.Instance.GetLS("Warning"), MessageBoxButtons.OKCancel) != DialogResult.OK)
                            {
                                apply = false;
                            }
                        }
                        if (apply)
                        {
                            joyObj.SetFontLibraryType(currentFont, ft);
                            WarningForm.Instance.OpenUI("LoadSuccess");
                        }
                    }
                }
                else
                {

                }
            }
        }
        public void DxRenderLogic()
        {
            JoyObject joyObj = PublicData.GetCurrentSelectJoyObject();
            if (joyObj != null)
            {
                if (joyObj.TypeSwitchControl == TypeSwitch.FontLibrarySwitch)
                {
                    float height = lcHeight * allFontType;
                    if (height < fontTypeLC.DrawRect.Height)
                        fontTypeLC.Rect = new SharpDX.RectangleF(0f, 0f, selectTypeControl.DrawRect.Width, height);
                    else
                        fontTypeLC.Rect = new SharpDX.RectangleF(0f, 0f, selectTypeControl.DrawRect.Width - ViewControl.sliderWidth, height);
                }
            }
        }
    }
}
