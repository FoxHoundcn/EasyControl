using SharpDX;
using System;
using System.Windows.Forms;

namespace EasyControl
{
    public class UpdateForm : iControl
    {
        public bool NodeLinkMode { get; private set; }
        public iControl Parent { get { return null; } set { } }

        public RectangleF DrawRect { get { return new RectangleF(0, 0, Dx2D.Instance.Width, Dx2D.Instance.Height); } }

        public RectangleF Rect { set { } }
        public Vector2 Offset { get { return new Vector2(0, 0); } set { } }
        public bool Hide { get { return updateUI.Hide; } set { } }

        public int Index { get { return -1; } }

        public string Name { get { return "UpdateForm"; } }

        public string UIKey { set { } }

        public string PluginID { get { return ""; } set { } }
        //----------------------------------------------------------------------------------
        LayoutControl updateUI;
        uiTextLable updateInfo;
        uiProgressBar updatePB;
        uiButton cancelBtn, updateBtn;
        bool Over = false;
        bool Check = false;
        JoyObject updateObj = null;
        //----------------------------------------------------------------------------------
        public static readonly UpdateForm Instance = new UpdateForm();
        private UpdateForm()
        {
        }
        public void Init()
        {
            updateUI = XmlUI.Instance.CreateUI(System.Environment.CurrentDirectory + @"\Xml\Dialog\Update.xml");
            updateUI.Hide = true;
            updateInfo = XmlUI.Instance.GetTextLable("UpdateInfo");
            cancelBtn = XmlUI.Instance.GetButton("CancelButton");
            cancelBtn.LeftButtonClick += OnCancelClick;
            updateBtn = XmlUI.Instance.GetButton("UpdateButton");
            updateBtn.LeftButtonClick += OnUpdateClick;
            updatePB = XmlUI.Instance.GetProgressBar("UpdatePB");
            //----
        }
        private void OnCancelClick(object sender, EventArgs e)
        {
            updateUI.Hide = true;
        }
        private void OnUpdateClick(object sender, EventArgs e)
        {
            if (Check && updateObj != null)
            {
                updateObj.AddReport(new Report(ReportType.Update));
                updateUI.Hide = true;
            }
            if (Over)
                updateUI.Hide = true;
        }
        public void CheckUpdate(JoyObject obj, string protocol)
        {
            string checkVersion = Localization.Instance.GetLS("ProtocolVersion") + ":" + protocol + "    " +
                Localization.Instance.GetLS("SoftwareVersion") + ":" + Localization.Instance.GetLS("ErrorSoftwareVersion");
            string version;
            if (NetMQServer.GetProtocolVersion(protocol, out version))
            {
                checkVersion = Localization.Instance.GetLS("ProtocolVersion") + ":" + protocol + "    " +
                   Localization.Instance.GetLS("SoftwareVersion") + ":" + version;
            }
            updateInfo.bLocalization = false;
            updateInfo.Text = obj.usbName + "\n" + Localization.Instance.GetLS("NeedUpdate") + "\n" + checkVersion;
            updateUI.Hide = false;
            Over = false;
            updatePB.Percentage = 0f;
            cancelBtn.Hide = false;
            updateBtn.Name = "OK";
            Check = true;
            updateObj = obj;
            obj.errorVersion = true;
        }
        public void OpenUI(string info, bool loc = true)
        {
            updateInfo.bLocalization = loc;
            updateInfo.Text = info;
            updateUI.Hide = false;
            Over = false;
            updatePB.Percentage = 0f;
            cancelBtn.Hide = true;
            updateBtn.Name = "Update";
            Check = false;
        }
        public void UpdateProgress(float progress)
        {
            updatePB.Percentage = progress;
        }
        #region Close
        public void Close()
        {
            Over = true;
            updateBtn.Name = "OK";
        }
        #endregion
        public void Dx2DResize()
        {
            updateUI.Dx2DResize();
        }
        public void DxRenderLogic()
        {
            if (!updateUI.Hide)
            {
                updateUI.Rect = new RectangleF(0, 0, Dx2D.Instance.Width, Dx2D.Instance.Height);
                updateUI.DxRenderLogic();
            }
        }
        public void DxRenderHigh()
        {
            if (!updateUI.Hide)
                updateUI.DxRenderMedium();
        }
        public void DxRenderMedium()
        {
        }
        public void DxRenderLow()
        {
        }
        public void JoyMouseMoveEvent(MouseEventArgs e)
        {
            if (!updateUI.Hide)
            {
                updateUI.JoyMouseMoveEvent(e);
                return;
            }
        }
        public void JoyMouseDownEvent(MouseEventArgs e)
        {
            if (!updateUI.Hide)
            {
                updateUI.JoyMouseDownEvent(e);
                return;
            }
        }
        public void JoyMouseUpEvent(MouseEventArgs e)
        {
            if (!updateUI.Hide)
            {
                updateUI.JoyMouseUpEvent(e);
                return;
            }
        }
        public void JoyMouseMoveWheel(MouseEventArgs e)
        {
            if (!updateUI.Hide)
            {
                updateUI.JoyMouseMoveWheel(e);
                return;
            }
        }
    }
}
