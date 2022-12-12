namespace EasyControl
{
    public class UI_NodeLink : iUiLogic
    {
        public LayoutControl mainLayout { get; private set; }
        //------------------------------------------------------------------------------------------------------
        public static readonly UI_NodeLink Instance = new UI_NodeLink();
        private UI_NodeLink()
        {
        }
        //============================================================
        public void DxRenderLogic()
        {
        }

        public void Init()
        {
            mainLayout = XmlUI.Instance.CreateUI(System.Environment.CurrentDirectory + @"\Xml\UI\UI_NodeLink.xml");
        }
    }
}
