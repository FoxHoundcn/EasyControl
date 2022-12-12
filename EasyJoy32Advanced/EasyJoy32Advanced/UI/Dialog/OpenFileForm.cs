using SharpDX;
using System;
using System.IO;
using System.Windows.Forms;

namespace EasyControl
{
    public class OpenFileForm : iControl
    {
        public bool NodeLinkMode { get; private set; }
        public iControl Parent { get { return null; } set { } }

        public RectangleF DrawRect { get { return new RectangleF(0, 0, Dx2D.Instance.Width, Dx2D.Instance.Height); } }

        public RectangleF Rect { set { } }
        public Vector2 Offset { get { return new Vector2(0, 0); } set { } }
        public bool Hide { get { return openFileUI.Hide; } set { } }

        public int Index { get { return -1; } }

        public string Name { get { return "OpenFileForm"; } }

        public string UIKey { set { } }

        public string PluginID { get { return ""; } set { } }
        //----------------------------------------------------------------------------------
        LayoutControl openFileUI;
        uiTextLable openFileTip;
        LayoutControl openFileList;
        uiTextLable openFileError;
        private int SelectOpenFileIndex = -1;
        private int OpenFileCount = 0;
        private float lcOpenFileHeight = 0;
        private OpenFileType openFileType = OpenFileType.Error;
        private string[] fileList;
        //----------------------------------------------------------------------------------
        public static readonly OpenFileForm Instance = new OpenFileForm();
        private OpenFileForm()
        {
        }
        public void Init()
        {
            openFileUI = XmlUI.Instance.CreateUI(System.Environment.CurrentDirectory + @"\Xml\Dialog\OpenFile.xml");
            openFileUI.Hide = true;
            openFileTip = XmlUI.Instance.GetTextLable("OpenFileTip");
            openFileList = XmlUI.Instance.GetLayoutControl("OpenFileListLC");
            uiButton openFileButton = XmlUI.Instance.GetButton("OpenFileButton");
            openFileButton.LeftButtonClick += OnOpenFile;
            openFileError = XmlUI.Instance.GetTextLable("OpenFileError");
            uiButton openFileCancel = XmlUI.Instance.GetButton("OpenFileCancel");
            openFileCancel.LeftButtonClick += OnCancelOpenFile;
        }
        private void OnOpenFile(object sender, EventArgs e)
        {
            if (SelectOpenFileIndex == -1)
            {
                openFileError.Text = "SeleteOpenFile";
                return;
            }
            if (SelectOpenFileIndex >= fileList.Length)
            {
                openFileError.Text = "SeleteOpenFile";
                SelectOpenFileIndex = -1;
                return;
            }
            switch (openFileType)
            {
                case OpenFileType.EasyJoy:
                    JoyObject currentObj = PublicData.GetCurrentSelectJoyObject();
                    if (currentObj != null)
                    {
                        if (currentObj.Load(fileList[SelectOpenFileIndex]))
                        {
                            openFileUI.Hide = true;
                            WarningForm.Instance.OpenUI("LoadSuccess");
                            return;
                        }
                    }
                    break;
                case OpenFileType.EasyControl:
                    if (NodeLinkControl.Instance.LoadAs(fileList[SelectOpenFileIndex]))
                    {
                        openFileUI.Hide = true;
                        WarningForm.Instance.OpenUI("LoadSuccess");
                        return;
                    }
                    break;
            }
        }
        private void OnCancelOpenFile(object sender, EventArgs e)
        {
            Close();
        }
        private void CreateFileList(string path, string fileType)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            else
            {
                fileList = Directory.GetFiles(path);
                OpenFileCount = fileList.Length;
                for (int i = 0; i < fileList.Length; i++)
                {
                    if (Path.GetExtension(fileList[i]).Equals(fileType))
                    {
                        string fileName = Path.GetFileNameWithoutExtension(fileList[i]);
                        //==============================================================
                        LayoutControl newType = null;
                        if (i < openFileList.controlList.Count)
                        {
                            newType = openFileList.controlList[i];
                        }
                        else
                        {
                            newType = XmlUI.Instance.CreateUI(System.Environment.CurrentDirectory + @"\Xml\Dialog\OpenFileItem.xml", "OpenFile" + i);
                            openFileList.AddObject(newType);
                        }
                        newType.Hide = false;
                        LayoutControl lcDevice = XmlUI.Instance.GetLayoutControl("OpenFile" + i + "OpenFileItemLC");
                        lcOpenFileHeight = lcDevice.maxHeight;
                        uiButton btn = XmlUI.Instance.GetButton("OpenFile" + i + "OpenFileItemIndex");
                        if (btn != null)
                        {
                            btn.Index = i;
                            btn.Name = fileName;
                            btn.LeftButtonClick += OnOpenFileClick;
                        }
                        uiPanel pan = XmlUI.Instance.GetPanel("OpenFile" + i + "OpenFileItemColor");
                        pan.ForeColor = XmlUI.DxDeviceGreen;
                    }
                }
                if (openFileList.controlList.Count > fileList.Length)
                {
                    for (int i = fileList.Length; i < openFileList.controlList.Count; i++)
                    {
                        openFileList.controlList[i].Hide = true;
                    }
                }
            }
        }
        public void OpenEasyControl()
        {
            openFileTip.Text = "OpenEasyControlTip";
            openFileError.Text = "SeleteOpenFile";
            SelectOpenFileIndex = -1;
            openFileType = OpenFileType.EasyControl;
            #region 文件列表
            string econtrolPath = System.Environment.CurrentDirectory + @"\Econtrol";
            CreateFileList(econtrolPath, ".xml");
            #endregion
            openFileUI.Hide = false;
        }
        #region OpenEasyJoy
        public void OpenEasyJoy()
        {
            openFileTip.Text = "OpenEasyJoyTip";
            openFileError.Text = "SeleteOpenFile";
            SelectOpenFileIndex = -1;
            openFileType = OpenFileType.EasyJoy;
            #region 文件列表
            string ejoyPath = System.Environment.CurrentDirectory + @"\Ejoy";
            CreateFileList(ejoyPath, ".xml");
            #endregion
            openFileUI.Hide = false;
        }
        private void OnOpenFileClick(object sender, EventArgs e)
        {
            JoyIndexChangeArgs args = (JoyIndexChangeArgs)e;
            SelectOpenFileIndex = args.Index;
            openFileError.Text = "";
            for (int i = 0; i < openFileList.controlList.Count; i++)
            {
                uiButton btn = XmlUI.Instance.GetButton("OpenFile" + i + "OpenFileItemIndex");
                if (btn != null)
                {
                    if (i == SelectOpenFileIndex)
                        btn.SelectOn = true;
                    else
                        btn.SelectOn = false;
                }
            }
        }
        #endregion
        #region Close
        public void Close()
        {
            openFileTip.Text = "";
            openFileError.Text = "";
            SelectOpenFileIndex = -1;
            openFileType = OpenFileType.Error;
            openFileUI.Hide = true;
        }
        #endregion
        public void Dx2DResize()
        {
            openFileUI.Dx2DResize();
        }
        public void DxRenderLogic()
        {
            if (!openFileUI.Hide)
            {
                openFileList.Rect = new RectangleF(0, 0, 384, lcOpenFileHeight * OpenFileCount);
                openFileUI.Rect = new RectangleF(0, 0, Dx2D.Instance.Width, Dx2D.Instance.Height);
                openFileUI.DxRenderLogic();
            }
        }
        public void DxRenderHigh()
        {
            if (!openFileUI.Hide)
                openFileUI.DxRenderMedium();
        }
        public void DxRenderMedium()
        {
        }
        public void DxRenderLow()
        {
        }
        public void JoyMouseMoveEvent(MouseEventArgs e)
        {
            if (!openFileUI.Hide)
            {
                openFileUI.JoyMouseMoveEvent(e);
                return;
            }
        }
        public void JoyMouseDownEvent(MouseEventArgs e)
        {
            if (!openFileUI.Hide)
            {
                openFileUI.JoyMouseDownEvent(e);
                return;
            }
        }
        public void JoyMouseUpEvent(MouseEventArgs e)
        {
            if (!openFileUI.Hide)
            {
                openFileUI.JoyMouseUpEvent(e);
                return;
            }
        }
        public void JoyMouseMoveWheel(MouseEventArgs e)
        {
            if (!openFileUI.Hide)
            {
                openFileUI.JoyMouseMoveWheel(e);
                return;
            }
        }
    }
}
