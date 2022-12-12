using SharpDX;
using System;
using System.Windows.Forms;

namespace EasyControl
{
    public class V3xUpdateForm : iControl
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
        uiButton v35Btn, v31Btn, vkbBtn, vNrfBtn, cancelBtn;
        bool Check = false;
        UpdateObject updateObj = null;
        //----------------------------------------------------------------------------------
        public static readonly V3xUpdateForm Instance = new V3xUpdateForm();
        private V3xUpdateForm()
        {
        }
        public void Init()
        {
            updateUI = XmlUI.Instance.CreateUI(System.Environment.CurrentDirectory + @"\Xml\Dialog\V3xUpdate.xml");
            updateUI.Hide = true;
            updateInfo = XmlUI.Instance.GetTextLable("V3xUpdateInfo");
            v35Btn = XmlUI.Instance.GetButton("Ver35");
            v35Btn.LeftButtonClick += OnVer35Click;
            v31Btn = XmlUI.Instance.GetButton("Ver31");
            v31Btn.LeftButtonClick += OnVer31Click;
            vkbBtn = XmlUI.Instance.GetButton("VerKB");
            vkbBtn.LeftButtonClick += OnKeyBoardClick;
            vNrfBtn = XmlUI.Instance.GetButton("VerNRF");
            vNrfBtn.LeftButtonClick += OnNRF24Click;
            cancelBtn = XmlUI.Instance.GetButton("VerCancel");
            cancelBtn.LeftButtonClick += OnCancelClick;
            //----
        }
        private void OnCancelClick(object sender, EventArgs e)
        {
            //updateObj.RunUpdate = false;
            updateUI.Hide = true;
        }
        private void OnVer35Click(object sender, EventArgs e)
        {
            if (Check && updateObj != null)
            {
                updateObj.SelectVersion(V3xFirmware.v35);
                updateObj.StartUpdate();
                updateUI.Hide = true;
            }
        }
        private void OnVer31Click(object sender, EventArgs e)
        {
            if (Check && updateObj != null)
            {
                updateObj.SelectVersion(V3xFirmware.v31);
                updateObj.StartUpdate();
                updateUI.Hide = true;
            }
        }
        private void OnKeyBoardClick(object sender, EventArgs e)
        {
            if (Check && updateObj != null)
            {
                updateObj.SelectVersion(V3xFirmware.vKB);
                updateObj.StartUpdate();
                updateUI.Hide = true;
            }
        }
        private void OnNRF24Click(object sender, EventArgs e)
        {
            if (Check && updateObj != null)
            {
                updateObj.SelectVersion(V3xFirmware.vNRF);
                updateObj.StartUpdate();
                updateUI.Hide = true;
            }
        }
        public void SelectUpdate(UpdateObject obj)
        {
            updateInfo.bLocalization = false;
            updateInfo.Text = obj.Name + Localization.Instance.GetLS("SelectVersion");
            Check = true;
            updateObj = obj;
            updateObj.RunUpdate = true;
            updateUI.Hide = false;
        }
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
