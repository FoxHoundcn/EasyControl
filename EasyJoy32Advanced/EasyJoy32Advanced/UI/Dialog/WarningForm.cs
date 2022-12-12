using SharpDX;
using System;
using System.Windows.Forms;

namespace EasyControl
{
    public class WarningForm : iControl
    {
        public bool NodeLinkMode { get; private set; }
        public iControl Parent { get { return null; } set { } }

        public RectangleF DrawRect { get { return new RectangleF(0, 0, Dx2D.Instance.Width, Dx2D.Instance.Height); } }

        public RectangleF Rect { set { } }
        public Vector2 Offset { get { return new Vector2(0, 0); } set { } }
        public bool Hide { get { return warningUI.Hide; } set { } }

        public int Index { get { return -1; } }

        public string Name { get { return "WarningForm"; } }

        public string UIKey { set { } }

        public string PluginID { get { return ""; } set { } }
        //----------------------------------------------------------------------------------
        LayoutControl warningUI;
        uiTextLable warningInfo;
        //----------------------------------------------------------------------------------
        public static readonly WarningForm Instance = new WarningForm();
        private WarningForm()
        {
        }
        public void Init()
        {
            warningUI = XmlUI.Instance.CreateUI(System.Environment.CurrentDirectory + @"\Xml\Dialog\Warning.xml");
            warningUI.Hide = true;
            warningInfo = XmlUI.Instance.GetTextLable("WarningInfo");
            uiButton warningBtn = XmlUI.Instance.GetButton("WarningButton");
            warningBtn.LeftButtonClick += OnWarningClick;
            //----
        }
        private void OnWarningClick(object sender, EventArgs e)
        {
            warningUI.Hide = true;
        }
        public void OpenUI(string info, bool loc = true)
        {
            warningInfo.bLocalization = loc;
            warningInfo.Text = info;
            warningUI.Hide = false;
        }
        public void Dx2DResize()
        {
            warningUI.Dx2DResize();
        }
        public void DxRenderLogic()
        {
            if (!warningUI.Hide)
            {
                warningUI.Rect = new RectangleF(0, 0, Dx2D.Instance.Width, Dx2D.Instance.Height);
                warningUI.DxRenderLogic();
            }
        }
        public void DxRenderHigh()
        {
            if (!warningUI.Hide)
                warningUI.DxRenderMedium();
        }
        public void DxRenderMedium()
        {
        }
        public void DxRenderLow()
        {
        }
        public void JoyMouseMoveEvent(MouseEventArgs e)
        {
            if (!warningUI.Hide)
            {
                warningUI.JoyMouseMoveEvent(e);
                return;
            }
        }
        public void JoyMouseDownEvent(MouseEventArgs e)
        {
            if (!warningUI.Hide)
            {
                warningUI.JoyMouseDownEvent(e);
                return;
            }
        }
        public void JoyMouseUpEvent(MouseEventArgs e)
        {
            if (!warningUI.Hide)
            {
                warningUI.JoyMouseUpEvent(e);
                return;
            }
        }
        public void JoyMouseMoveWheel(MouseEventArgs e)
        {
            if (!warningUI.Hide)
            {
                warningUI.JoyMouseMoveWheel(e);
                return;
            }
        }
    }
}
