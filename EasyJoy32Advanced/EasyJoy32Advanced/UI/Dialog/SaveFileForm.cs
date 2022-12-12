using SharpDX;
using System;
using System.Windows.Forms;

namespace EasyControl
{
    public class SaveFileForm : iControl
    {
        public bool NodeLinkMode { get; private set; }
        public iControl Parent { get { return null; } set { } }

        public RectangleF DrawRect { get { return new RectangleF(0, 0, Dx2D.Instance.Width, Dx2D.Instance.Height); } }

        public RectangleF Rect { set { } }
        public Vector2 Offset { get { return new Vector2(0, 0); } set { } }
        public bool Hide { get { return saveFileUI.Hide; } set { } }

        public int Index { get { return -1; } }

        public string Name { get { return "SaveFileForm"; } }

        public string UIKey { set { } }

        public string PluginID { get { return ""; } set { } }
        //----------------------------------------------------------------------------------
        LayoutControl saveFileUI;
        uiTextLable saveFileTip;
        uiTextEditor saveFileName;
        uiTextLable saveFileError;
        private SaveFileType saveFileType = SaveFileType.Error;
        //----------------------------------------------------------------------------------
        public static readonly SaveFileForm Instance = new SaveFileForm();
        private SaveFileForm()
        {
        }
        public void Init()
        {
            saveFileUI = XmlUI.Instance.CreateUI(System.Environment.CurrentDirectory + @"\Xml\Dialog\SaveFile.xml");
            saveFileUI.Hide = true;
            saveFileTip = XmlUI.Instance.GetTextLable("SaveFileTip");
            saveFileName = XmlUI.Instance.GetTextEditor("SaveFileName");
            uiButton saveFileButton = XmlUI.Instance.GetButton("SaveFileButton");
            saveFileButton.LeftButtonClick += OnSaveFile;
            saveFileError = XmlUI.Instance.GetTextLable("SaveFileError");
            uiButton saveFileCancel = XmlUI.Instance.GetButton("SaveFileCancel");
            saveFileCancel.LeftButtonClick += OnCancelSaveFile;
        }
        #region OpenSave
        public void SaveEasyControl()
        {
            saveFileTip.Text = "SaveEasyControlTip";
            saveFileName.Text = "";
            saveFileError.Text = "InputFileName";
            saveFileType = SaveFileType.EasyControl;
            saveFileUI.Hide = false;
        }
        public void SaveEasyJoy()
        {
            saveFileTip.Text = "SaveEasyJoyTip";
            saveFileName.Text = "";
            saveFileError.Text = "InputFileName";
            saveFileType = SaveFileType.EasyJoy;
            saveFileUI.Hide = false;
        }
        public void SaveFontLib()
        {
            saveFileTip.Text = "SaveFontLibTip";
            saveFileName.Text = "";
            saveFileError.Text = "InputFileName";
            saveFileType = SaveFileType.FontLib;
            saveFileUI.Hide = false;
        }
        private void OnSaveFile(object sender, EventArgs e)
        {
            string logMsg;
            JoyObject currentObj = PublicData.GetCurrentSelectJoyObject();
            switch (saveFileType)
            {
                case SaveFileType.FontLib:
                    #region FontLib
                    if (currentObj != null)
                    {
                        if (!PublicData.SaveFont(true, currentObj, saveFileName.Text, out logMsg))
                        {
                            saveFileError.Text = logMsg;
                        }
                        else
                        {
                            saveFileUI.Hide = true;
                            WarningForm.Instance.OpenUI("SaveSuccess");
                            return;
                        }
                    }
                    else
                    {
                        DebugConstol.AddLog("OnSaveFontLib : Easy Joy Error !!!", LogType.Error);
                        saveFileUI.Hide = true;
                    }
                    return;
                #endregion
                case SaveFileType.EasyJoy:
                    #region EasyJoy
                    if (currentObj != null)
                    {
                        if (!PublicData.SaveJoy(true, currentObj, saveFileName.Text, out logMsg))
                        {
                            saveFileError.Text = logMsg;
                        }
                        else
                        {
                            saveFileUI.Hide = true;
                            WarningForm.Instance.OpenUI("SaveSuccess");
                            return;
                        }
                    }
                    else
                    {
                        DebugConstol.AddLog("OnSaveFontLib : Easy Joy Error !!!", LogType.Error);
                        saveFileUI.Hide = true;
                    }
                    return;
                #endregion
                case SaveFileType.EasyControl:
                    #region EasyControl
                    if (!PublicData.SaveControl(true, saveFileName.Text, out logMsg))
                    {
                        saveFileError.Text = logMsg;
                    }
                    else
                    {
                        saveFileUI.Hide = true;
                        WarningForm.Instance.OpenUI("SaveSuccess");
                        return;
                    }
                    return;
                    #endregion
            }
            DebugConstol.AddLog("OnSaveFontLib : Type Error !!!", LogType.Error);
            saveFileUI.Hide = true;
        }
        private void OnCancelSaveFile(object sender, EventArgs e)
        {
            Close();
        }
        #endregion
        #region Close
        public void Close()
        {
            saveFileTip.Text = "";
            saveFileName.Text = "";
            saveFileError.Text = "";
            saveFileType = SaveFileType.Error;
            saveFileUI.Hide = true;
        }
        #endregion
        public void Dx2DResize()
        {
            saveFileUI.Dx2DResize();
        }
        public void DxRenderLogic()
        {
            if (!saveFileUI.Hide)
            {
                saveFileUI.Rect = new RectangleF(0, 0, Dx2D.Instance.Width, Dx2D.Instance.Height);
                saveFileUI.DxRenderLogic();
            }
        }
        public void DxRenderHigh()
        {
            if (!saveFileUI.Hide)
                saveFileUI.DxRenderMedium();
        }
        public void DxRenderMedium()
        {
        }
        public void DxRenderLow()
        {
        }
        public void JoyMouseMoveEvent(MouseEventArgs e)
        {
            if (!saveFileUI.Hide)
            {
                saveFileUI.JoyMouseMoveEvent(e);
                return;
            }
        }
        public void JoyMouseDownEvent(MouseEventArgs e)
        {
            if (!saveFileUI.Hide)
            {
                saveFileUI.JoyMouseDownEvent(e);
                return;
            }
        }
        public void JoyMouseUpEvent(MouseEventArgs e)
        {
            if (!saveFileUI.Hide)
            {
                saveFileUI.JoyMouseUpEvent(e);
                return;
            }
        }
        public void JoyMouseMoveWheel(MouseEventArgs e)
        {
            if (!saveFileUI.Hide)
            {
                saveFileUI.JoyMouseMoveWheel(e);
                return;
            }
        }
    }
}
