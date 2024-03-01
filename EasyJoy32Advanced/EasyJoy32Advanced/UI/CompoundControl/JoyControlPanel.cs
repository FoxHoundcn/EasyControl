using System;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace EasyControl
{
    public class JoyControlPanel : iUiLogic
    {
        //---------------------------------------------------------------------------------------
        JoyObject currentObj = null;
        #region Joy属性
        uiTextEditor joyNameTE;
        uiTextEditor joyVIDTE;
        uiTextEditor joyPIDTE;
        uiTextLable joyVersion;
        uiTextLable joyMcuID;
        uiTextLable joyKeyTE;
        uiButton joyCopyBtn;
        uiButton joyPasteBtn;
        #endregion
        #region 外设数量
        uiTrackBar maxAdcTB;
        uiTrackBar maxHallTB;
        uiTrackBar maxPwmTB;
        uiTrackBar maxPinTB;
        #endregion
        //////////////////////////////////////////////////////////////////////////////////////////
        public static readonly JoyControlPanel Instance = new JoyControlPanel();
        private JoyControlPanel()
        {
        }
        public void Init()
        {
            #region Joy属性
            joyNameTE = XmlUI.Instance.GetTextEditor("JoyNameTE");
            joyVIDTE = XmlUI.Instance.GetTextEditor("JoyVIDTE");
            joyPIDTE = XmlUI.Instance.GetTextEditor("JoyPIDTE");
            joyNameTE.TextChange += OnJoyNameTextChange;
            joyVIDTE.TextChange += OnJoyVIDTextChange;
            joyPIDTE.TextChange += OnJoyPIDTextChange;
            joyVersion = XmlUI.Instance.GetTextLable("EjoyVersion");
            joyMcuID = XmlUI.Instance.GetTextLable("ShowMucID");
            joyKeyTE = XmlUI.Instance.GetTextLable("ShowKey");
            joyCopyBtn = XmlUI.Instance.GetButton("CopyBtn");
            joyCopyBtn.LeftButtonClick += OnCopyBtnClick;
            joyPasteBtn = XmlUI.Instance.GetButton("PasteBtn");
            joyPasteBtn.LeftButtonClick += OnPasteBtnClick;
            #endregion
            #region 外设数量
            maxAdcTB = XmlUI.Instance.GetTrackBar("maxAdcTB");
            maxAdcTB.ValueChange += OnJoyMaxAdcChange;
            maxHallTB = XmlUI.Instance.GetTrackBar("maxHallTB");
            maxHallTB.ValueChange += OnJoyMaxHallChange;
            maxPwmTB = XmlUI.Instance.GetTrackBar("maxPwmTB");
            maxPwmTB.ValueChange += OnJoyMaxPwmChange;
            maxPinTB = XmlUI.Instance.GetTrackBar("maxPinTB");
            maxPinTB.ValueChange += OnJoyMaxPinChange;
            #endregion
        }
        #region joy
        private void OnJoyNameTextChange(object sender, EventArgs e)
        {
            if (currentObj != null)
            {
                currentObj.SetUsbName(joyNameTE.Text);
            }
        }
        private void OnJoyVIDTextChange(object sender, EventArgs e)
        {
            string text = joyVIDTE.Text.Length >= 4 ? joyVIDTE.Text.Substring(0, 4) : joyVIDTE.Text;
            int value = int.Parse(text, System.Globalization.NumberStyles.HexNumber);
            if (currentObj != null)
            {
                currentObj.SetVID((ushort)value);
            }
        }
        private void OnJoyPIDTextChange(object sender, EventArgs e)
        {
            string text = joyPIDTE.Text.Length >= 4 ? joyPIDTE.Text.Substring(0, 4) : joyPIDTE.Text;
            int value = int.Parse(text, System.Globalization.NumberStyles.HexNumber);
            if (currentObj != null)
            {
                currentObj.SetPID((ushort)value);
            }
        }
        private void OnCopyBtnClick(object sender, EventArgs e)
        {
            Clipboard.SetDataObject(joyMcuID.Text);
        }
        private void OnPasteBtnClick(object sender, EventArgs e)
        {
            IDataObject iData = Clipboard.GetDataObject();
            string key = "";
            if (iData.GetDataPresent(DataFormats.Text))
            {
                key = (string)iData.GetData(DataFormats.Text);
            }
            if (currentObj != null)
            {
                if (currentObj.SetKeyText(key))
                {
                    joyKeyTE.Text = key;
                }
                else
                {
                    WarningForm.Instance.OpenUI("LicenseFormat");
                }
            }
        }
        private void OnJoyMaxAdcChange(object sender, EventArgs e)
        {
            currentObj.joyMaxADC = (byte)maxAdcTB.Value;
        }
        private void OnJoyMaxHallChange(object sender, EventArgs e)
        {
            currentObj.joyMaxHall = (byte)maxHallTB.Value;
        }
        private void OnJoyMaxPwmChange(object sender, EventArgs e)
        {
            currentObj.joyMaxPWM = (byte)maxPwmTB.Value;
        }
        private void OnJoyMaxPinChange(object sender, EventArgs e)
        {
            currentObj.joyMaxPin = (byte)maxPinTB.Value;
        }
        #endregion
        public void DxRenderLogic()
        {
            currentObj = PublicData.GetCurrentSelectJoyObject();
            if (currentObj != null)
            {
                if (joyNameTE.Text != currentObj.usbName)
                    joyNameTE.Text = currentObj.usbName;
                if (joyVIDTE.Text != currentObj.VID.ToString("x4").ToUpper())
                    joyVIDTE.Text = currentObj.VID.ToString("x4").ToUpper();
                if (joyPIDTE.Text != currentObj.PID.ToString("x4").ToUpper())
                    joyPIDTE.Text = currentObj.PID.ToString("x4").ToUpper();
                if (joyVersion.Text != currentObj.version1 + "." + currentObj.version2 + "." + currentObj.version3)
                    joyVersion.Text = currentObj.version1 + "." + currentObj.version2 + "." + currentObj.version3;
                if (joyMcuID.Text != currentObj.McuID)
                    joyMcuID.Text = currentObj.McuID;
                //----------------------------------------------------------
                if (maxAdcTB.Value != currentObj.joyMaxADC)
                    maxAdcTB.Value = currentObj.joyMaxADC;
                if (maxHallTB.Value != currentObj.joyMaxHall)
                    maxHallTB.Value = currentObj.joyMaxHall;
                if (maxPwmTB.Value != currentObj.joyMaxPWM)
                    maxPwmTB.Value = currentObj.joyMaxPWM;
                if (maxPinTB.Value != currentObj.joyMaxPin)
                    maxPinTB.Value = currentObj.joyMaxPin;
            }
        }
    }
}
