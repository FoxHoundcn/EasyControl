using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace EasyControl
{
    public class UI_Login : iUiLogic
    {
        public LayoutControl mainLayout { get; private set; }
        //----
        //------------------------------------------------------------------------------------------------------
        public static readonly UI_Login Instance = new UI_Login();
        private UI_Login()
        {
        }
        //============================================================
        public void DxRenderLogic()
        {
        }
        public void Init()
        {
            mainLayout = XmlUI.Instance.CreateUI(System.Environment.CurrentDirectory + @"\Xml\UI\UI_Login.xml");
        }
        public void LoginReady()
        {
            PublicData.ui_Type = UIType.NodeLink;
            #region 成功登陆
            NodeLinkControl.Instance.Load();
            UI_PluginControl.Instance.Init();
            JoyObjectSelect.Instance.RefreshClick();
            NodeLinkControl.Instance.Load();
            #endregion
        }
    }
}
