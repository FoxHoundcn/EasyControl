using SharpDX;
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Xml;

namespace EasyControl
{
    public class XmlUI
    {
        static Dictionary<string, LayoutControl> layoutControlList = new Dictionary<string, LayoutControl>();
        static Dictionary<string, SplitControl> splitControlList = new Dictionary<string, SplitControl>();
        static Dictionary<string, ViewControl> viewControlList = new Dictionary<string, ViewControl>();
        //----
        static Dictionary<string, uiHatSetting> hatList = new Dictionary<string, uiHatSetting>();
        //----
        static Dictionary<string, uiButton> buttonList = new Dictionary<string, uiButton>();
        static Dictionary<string, uiDataCurve> dataCurveList = new Dictionary<string, uiDataCurve>();
        static Dictionary<string, uiImage> imageList = new Dictionary<string, uiImage>();
        static Dictionary<string, uiKeyBoard> keyBoardList = new Dictionary<string, uiKeyBoard>();
        static Dictionary<string, uiOLED> oledList = new Dictionary<string, uiOLED>();
        static Dictionary<string, uiPanel> panelList = new Dictionary<string, uiPanel>();
        static Dictionary<string, uiPlaceholder> placeList = new Dictionary<string, uiPlaceholder>();
        static Dictionary<string, uiProgressBar> progressBarList = new Dictionary<string, uiProgressBar>();
        static Dictionary<string, uiSwitchButton> switchBtnList = new Dictionary<string, uiSwitchButton>();
        static Dictionary<string, uiTextEditor> textEditorList = new Dictionary<string, uiTextEditor>();
        static Dictionary<string, uiTextLable> textLableList = new Dictionary<string, uiTextLable>();
        static Dictionary<string, uiTrackBar> trackBarList = new Dictionary<string, uiTrackBar>();
        //----
        #region DxColor
        public static Color4 DxBackColor { get; private set; } = PublicData.GetDxColor(46, 49, 56);
        public static Color4 DxUIBackColor { get; private set; } = PublicData.GetDxColor(82, 88, 99);
        public static Color4 DxUIClickColor { get; private set; } = PublicData.GetDxColor(123, 126, 132);
        public static Color4 DxTextColor { get; private set; } = PublicData.GetDxColor(255, 255, 255);

        public static Color4 DxDeviceBlue { get; private set; } = PublicData.GetDxColor(42, 161, 211);
        public static Color4 DxDeviceGreen { get; private set; } = PublicData.GetDxColor(138, 183, 27);
        public static Color4 DxDeviceRed { get; private set; } = PublicData.GetDxColor(238, 78, 16);
        public static Color4 DxDeviceYellow { get; private set; } = PublicData.GetDxColor(240, 176, 23);
        public static Color4 DxDevicePurple { get; private set; } = PublicData.GetDxColor(191, 88, 203);

        public static Color4 DxDefaultColor { get; private set; } = PublicData.GetDxColor(Color.LightGray.R, Color.LightGray.G, Color.LightGray.B);
        public static Color4 DxSelectColor { get; private set; } = PublicData.GetDxColor(Color.Violet.R, Color.Violet.G, Color.Violet.B);
        public static Color4 DxWarningColor { get; private set; } = PublicData.GetDxColor(Color.Yellow.R, Color.Yellow.G, Color.Yellow.B);
        public static Color4 DxUsedColor { get; private set; } = PublicData.GetDxColor(Color.LightPink.R, Color.LightPink.G, Color.LightPink.B);
        public static Color4 DxApplyColor { get; private set; } = PublicData.GetDxColor(Color.LightGreen.R, Color.LightGreen.G, Color.LightGreen.B);
        #endregion
        #region 事件
        public EventHandler ButtonLeftClick;
        public EventHandler ButtonRightClick;
        public EventHandler SwitchButtonChange;
        public EventHandler TextEditorChange;
        public EventHandler TrackBarChange;
        #endregion
        //===================================================================
        public static readonly XmlUI Instance = new XmlUI();
        public void Init()
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(System.Environment.CurrentDirectory + @"\Xml\Color.xml");
            #region Color
            XmlNode rootNode = xmlDoc.SelectSingleNode("Color");
            Color4 _DxBackColor;
            if (GetAttribute(rootNode, "DxBackColor", out _DxBackColor))
            {
                DxBackColor = _DxBackColor;
            }
            Color4 _DxUIBackColor;
            if (GetAttribute(rootNode, "DxUIBackColor", out _DxUIBackColor))
            {
                DxUIBackColor = _DxUIBackColor;
            }
            Color4 _DxUIClickColor;
            if (GetAttribute(rootNode, "DxUIClickColor", out _DxUIClickColor))
            {
                DxUIClickColor = _DxUIClickColor;
            }
            Color4 _DxTextColor;
            if (GetAttribute(rootNode, "DxTextColor", out _DxTextColor))
            {
                DxTextColor = _DxTextColor;
            }
            //-----------------------------------------------------------------------------------------
            Color4 _DxDeviceBlue;
            if (GetAttribute(rootNode, "DxDeviceBlue", out _DxDeviceBlue))
            {
                DxDeviceBlue = _DxDeviceBlue;
            }
            Color4 _DxDeviceGreen;
            if (GetAttribute(rootNode, "DxDeviceGreen", out _DxDeviceGreen))
            {
                DxDeviceGreen = _DxDeviceGreen;
            }
            Color4 _DxDeviceRed;
            if (GetAttribute(rootNode, "DxDeviceRed", out _DxDeviceRed))
            {
                DxDeviceRed = _DxDeviceRed;
            }
            Color4 _DxDeviceYellow;
            if (GetAttribute(rootNode, "DxDeviceYellow", out _DxDeviceYellow))
            {
                DxDeviceYellow = _DxDeviceYellow;
            }
            Color4 _DxDevicePurple;
            if (GetAttribute(rootNode, "DxDevicePurple", out _DxDevicePurple))
            {
                DxDevicePurple = _DxDevicePurple;
            }
            //-----------------------------------------------------------------------------------------
            Color4 _DxDefaultColor;
            if (GetAttribute(rootNode, "DxDefaultColor", out _DxDefaultColor))
            {
                DxDefaultColor = _DxDefaultColor;
            }
            Color4 _DxSelectColor;
            if (GetAttribute(rootNode, "DxSelectColor", out _DxSelectColor))
            {
                DxSelectColor = _DxSelectColor;
            }
            Color4 _DxWarningColor;
            if (GetAttribute(rootNode, "DxWarningColor", out _DxWarningColor))
            {
                DxWarningColor = _DxWarningColor;
            }
            Color4 _DxUsedColor;
            if (GetAttribute(rootNode, "DxUsedColor", out _DxUsedColor))
            {
                DxUsedColor = _DxUsedColor;
            }
            Color4 _DxApplyColor;
            if (GetAttribute(rootNode, "DxApplyColor", out _DxApplyColor))
            {
                DxApplyColor = _DxApplyColor;
            }
            #endregion
            //-----------------------------------------------------------------------------------------
            #region 事件       
            ButtonLeftClick += doNothing;
            ButtonRightClick += doNothing;
            SwitchButtonChange += doNothing;
            TextEditorChange += doNothing;
            TrackBarChange += doNothing;
            #endregion
        }
        public LayoutControl CreateUI(string path, string Prefix = "")
        {
            try
            {
                LayoutControl mainLC = new LayoutControl(OrientationType.Vertical);
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(path);
                XmlNode rootNode = xmlDoc.SelectSingleNode("UI");
                if (rootNode != null && rootNode.HasChildNodes)
                {
                    BuildUI(mainLC, rootNode.ChildNodes, Prefix, -1);
                }
                mainLC.Dx2DResize();
                return mainLC;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + ex.StackTrace);
                return null;
            }
        }
        #region 获取控件
        public LayoutControl GetLayoutControl(string ID)
        {
            if (layoutControlList.ContainsKey(ID))
                return layoutControlList[ID];
            return null;
        }
        public SplitControl GetSplitControl(string ID)
        {
            if (splitControlList.ContainsKey(ID))
                return splitControlList[ID];
            return null;
        }
        public ViewControl GetViewControl(string ID)
        {
            if (viewControlList.ContainsKey(ID))
                return viewControlList[ID];
            return null;
        }
        public uiButton GetButton(string ID)
        {
            if (buttonList.ContainsKey(ID))
                return buttonList[ID];
            return null;
        }
        public uiDataCurve GetDataCurve(string ID)
        {
            if (dataCurveList.ContainsKey(ID))
                return dataCurveList[ID];
            return null;
        }
        public uiKeyBoard GetKeyBoard(string ID)
        {
            if (keyBoardList.ContainsKey(ID))
                return keyBoardList[ID];
            return null;
        }
        public uiOLED GetOLED(string ID)
        {
            if (oledList.ContainsKey(ID))
                return oledList[ID];
            return null;
        }
        public uiPanel GetPanel(string ID)
        {
            if (panelList.ContainsKey(ID))
                return panelList[ID];
            return null;
        }
        public uiPlaceholder GetPlaceholder(string ID)
        {
            if (placeList.ContainsKey(ID))
                return placeList[ID];
            return null;
        }
        public uiProgressBar GetProgressBar(string ID)
        {
            if (progressBarList.ContainsKey(ID))
                return progressBarList[ID];
            return null;
        }
        public uiSwitchButton GetSwitchButton(string ID)
        {
            if (switchBtnList.ContainsKey(ID))
                return switchBtnList[ID];
            return null;
        }
        public uiTextEditor GetTextEditor(string ID)
        {
            if (textEditorList.ContainsKey(ID))
                return textEditorList[ID];
            return null;
        }
        public uiTextLable GetTextLable(string ID)
        {
            if (textLableList.ContainsKey(ID))
                return textLableList[ID];
            return null;
        }
        public uiTrackBar GetTrackBar(string ID)
        {
            if (trackBarList.ContainsKey(ID))
                return trackBarList[ID];
            return null;
        }
        public uiHatSetting GetHat(string ID)
        {
            if (hatList.ContainsKey(ID))
                return hatList[ID];
            return null;
        }
        public uiImage GetImage(string ID)
        {
            if (imageList.ContainsKey(ID))
                return imageList[ID];
            return null;
        }
        #endregion
        private void BuildUI(LayoutControl parent, XmlNodeList firstLevelNodeList, string Prefix = "", int index = -1)
        {
            foreach (XmlNode node in firstLevelNodeList)
            {
                string nodeName = node.Name;
                switch (nodeName)
                {
                    case "LayoutControl":
                        #region LayoutControl
                        LayoutControl lcNew = null;
                        #region -->OrientationType
                        string layoutType = "";
                        if (GetAttribute(node, "_OrientationType", out layoutType))
                        {
                            switch (layoutType)
                            {
                                case "Vertical":
                                    lcNew = new LayoutControl(OrientationType.Vertical);
                                    break;
                                case "Horizontal":
                                    lcNew = new LayoutControl(OrientationType.Horizontal);
                                    break;
                                default:
                                    lcNew = new LayoutControl();
                                    break;
                            }
                        }
                        else
                        {
                            throw new Exception("LayoutControl Parameter : \"_OrientationType\" ERROR !!!");
                        }
                        #endregion
                        #region Child
                        if (lcNew != null && node.HasChildNodes)
                        {
                            BuildUI(lcNew, node.ChildNodes, Prefix, index);
                        }
                        #endregion
                        #region **ID**
                        string layoutID = "";
                        if (GetAttribute(node, "ID", out layoutID))
                        {
                            layoutID = Prefix + layoutID;
                            if (index != -1)
                                layoutID += index;
                            layoutControlList.Add(layoutID, lcNew);
                            lcNew.UIKey = layoutID;
                        }
                        #endregion
                        SetLayoutControl(node, lcNew);
                        parent.AddObject(lcNew);
                        #endregion
                        break;
                    case "SplitControl":
                        #region SplitControl
                        #region -->OrientationType
                        string _spType = "";
                        if (GetAttribute(node, "_OrientationType", out _spType))
                        {
                            OrientationType splitType = OrientationType.Object;
                            if (!Enum.TryParse(_spType, out splitType))
                            {
                                splitType = OrientationType.Object;
                            }
                            //left & right LC
                            XmlNode leftSplit = node.SelectSingleNode("LeftSplit");
                            int leftMin = 0;
                            XmlNode rightSplit = node.SelectSingleNode("RightSplit");
                            int rightMin = 0;
                            if (leftSplit != null && GetAttribute(leftSplit, "Min", out leftMin) &&
                                rightSplit != null && GetAttribute(rightSplit, "Min", out rightMin))
                            {
                                #region Child
                                LayoutControl lcLeft = new LayoutControl(OrientationType.Vertical);
                                if (leftSplit.HasChildNodes)
                                {
                                    BuildUI(lcLeft, leftSplit.ChildNodes, Prefix, -1);
                                }
                                LayoutControl lcRight = new LayoutControl(OrientationType.Vertical);
                                if (rightSplit.HasChildNodes)
                                {
                                    BuildUI(lcRight, rightSplit.ChildNodes, Prefix, -1);
                                }
                                #endregion
                                SplitControl spNew = new SplitControl(splitType, lcLeft, lcRight, leftMin, rightMin);
                                #region **ID**
                                string splitID = "";
                                if (GetAttribute(node, "ID", out splitID))
                                {
                                    splitID = Prefix + splitID;
                                    if (index != -1)
                                        splitID += index;
                                    splitControlList.Add(splitID, spNew);
                                    spNew.UIKey = splitID;
                                }
                                #region LeftID
                                string leftID = "";
                                if (GetAttribute(leftSplit, "ID", out leftID))
                                {
                                    leftID = Prefix + leftID;
                                    if (index != -1)
                                        leftID += index;
                                    layoutControlList.Add(leftID, lcLeft);
                                }
                                #endregion
                                #region RightID
                                string rightID = "";
                                if (GetAttribute(rightSplit, "ID", out rightID))
                                {
                                    rightID = Prefix + rightID;
                                    if (index != -1)
                                        rightID += index;
                                    layoutControlList.Add(rightID, lcRight);
                                }
                                #endregion
                                #endregion
                                LayoutControl lcSplit = new LayoutControl(spNew);
                                SetLayoutControl(node, lcSplit);
                                parent.AddObject(lcSplit);
                            }
                        }
                        else
                        {
                            throw new Exception("SplitControl Parameter : \"_OrientationType\" ERROR !!!");
                        }
                        #endregion
                        #endregion
                        break;
                    case "ViewControl":
                        #region ViewControl
                        #region LayoutType
                        string viewLayout = "Medium";
                        if (!GetAttribute(node, "LayoutType", out viewLayout))
                        {
                            viewLayout = "Medium";
                        }
                        LayoutType viewType = LayoutType.Medium;
                        if (!Enum.TryParse(viewLayout, out viewType))
                        {
                            viewType = LayoutType.Medium;
                        }
                        #endregion
                        LayoutControl lcChild = new LayoutControl(OrientationType.Vertical);
                        ViewControl vcNew = new ViewControl(lcChild, viewType);
                        #region Child
                        if (node.HasChildNodes)
                            BuildUI(lcChild, node.ChildNodes, Prefix, -1);
                        #endregion
                        #region **ID**                         
                        string viewID = "";
                        if (GetAttribute(node, "ID", out viewID))
                        {
                            viewID = Prefix + viewID;
                            if (index != -1)
                                viewID += index;
                            viewControlList.Add(viewID, vcNew);
                            vcNew.UIKey = viewID;
                            layoutControlList.Add(viewID + "LC", lcChild);
                            lcChild.UIKey = viewID + "LC";
                        }
                        #endregion
                        #region VerticalBar
                        bool _VerticalBar = false;
                        if (GetAttribute(node, "VerticalBar", out _VerticalBar))
                        {
                            vcNew.bVerticalBar = _VerticalBar;
                        }
                        #endregion
                        #region HorizontalBar
                        bool _HorizontalBar = false;
                        if (GetAttribute(node, "HorizontalBar", out _HorizontalBar))
                        {
                            vcNew.bHorizontalBar = _HorizontalBar;
                        }
                        #endregion
                        LayoutControl lcView = new LayoutControl(vcNew);
                        SetLayoutControl(node, lcChild);
                        parent.AddObject(lcView);
                        #endregion
                        break;
                    #region===================================================
                    #endregion
                    case "Matrix":
                        #region Matrix
                        if (node.HasChildNodes)
                        {
                            int VerticalCount = 1;
                            int HorizontalCount = 1;
                            GetAttribute(node, "VerticalCount", out VerticalCount);
                            if (VerticalCount < 1)
                                VerticalCount = 1;
                            GetAttribute(node, "HorizontalCount", out HorizontalCount);
                            if (HorizontalCount < 1)
                                HorizontalCount = 1;
                            int maxCount = -1;
                            GetAttribute(node, "MaxCount", out maxCount);
                            LayoutControl lcV = new LayoutControl(OrientationType.Vertical);
                            for (int v = 0; v < VerticalCount; v++)
                            {
                                LayoutControl lcH = new LayoutControl(OrientationType.Horizontal);
                                for (int h = 0; h < HorizontalCount; h++)
                                {
                                    int currentIndex = v * HorizontalCount + h;
                                    if (maxCount <= 0 || currentIndex < maxCount)
                                        BuildUI(lcH, node.ChildNodes, Prefix, currentIndex);
                                    else
                                        lcH.AddObject(new LayoutControl());
                                }
                                lcV.AddObject(lcH);
                            }
                            parent.AddObject(lcV);
                        }
                        #endregion
                        break;
                    #region===================================================
                    #endregion
                    case "Hat":
                        #region Hat
                        int hatIndex = 0;
                        #region -->Index
                        if (GetAttribute(node, "_Index", out hatIndex))
                        {
                            uiHatSetting hatSet = new uiHatSetting(hatIndex);
                            #region **ID**
                            string hatID = "";
                            if (GetAttribute(node, "ID", out hatID))
                            {
                                hatID = Prefix + hatID;
                                if (index != -1)
                                    hatID += index;
                                hatList.Add(hatID, hatSet);
                                hatSet.UIKey = hatID;
                            }
                            #endregion
                            LayoutControl lcHat = new LayoutControl(hatSet);
                            SetLayoutControl(node, lcHat);
                            parent.AddObject(lcHat);
                        }
                        else
                        {
                            throw new Exception("Hat Parameter : \"_Index\" ERROR !!!");
                        }
                        #endregion
                        #endregion
                        break;
                    case "Button":
                        #region Button
                        string btnText = "";
                        #region -->Text
                        if (GetAttribute(node, "_Text", out btnText))
                        {
                            #region jBtnType
                            string btnType = "";
                            jBtnType _type = jBtnType.Normal;
                            if (GetAttribute(node, "Type", out btnType))
                            {
                                if (!Enum.TryParse(btnType, out _type))
                                {
                                    _type = jBtnType.Normal;
                                }
                            }
                            #endregion
                            if (index != -1)
                                btnText += index + 1;
                            uiButton btnNew = new uiButton(index, btnText, _type);
                            #region **ID**
                            string buttonID = "";
                            if (GetAttribute(node, "ID", out buttonID))
                            {
                                buttonID = Prefix + buttonID;
                                if (index != -1)
                                    buttonID += index;
                                buttonList.Add(buttonID, btnNew);
                                btnNew.UIKey = buttonID;
                                btnNew.LeftButtonClick += OnButtonLeftClick;
                                btnNew.RightButtonClick += OnButtonRightClick;
                            }
                            #endregion
                            #region AlwaysOn
                            bool _AlwaysOn = false;
                            if (GetAttribute(node, "AlwaysOn", out _AlwaysOn))
                            {
                                btnNew.AlwaysOn = _AlwaysOn;
                            }
                            #endregion
                            #region AlwaysOn
                            bool _SelectOn = false;
                            if (GetAttribute(node, "SelectOn", out _SelectOn))
                            {
                                btnNew.SelectOn = _SelectOn;
                            }
                            #endregion
                            #region Color
                            Color4 _color;
                            if (GetAttribute(node, "Color", out _color))
                            {
                                btnNew.ForeColor = _color;
                            }
                            #endregion
                            #region BackColor
                            Color4 _BackColor;
                            if (GetAttribute(node, "BackColor", out _BackColor))
                            {
                                btnNew.BackColor = _BackColor;
                            }
                            #endregion
                            #region TextAlignment
                            string _TextAlignment;
                            if (GetAttribute(node, "TextAlignment", out _TextAlignment))
                            {
                                SharpDX.DirectWrite.TextAlignment ta;
                                if (Enum.TryParse(_TextAlignment, out ta))
                                {
                                    btnNew.textAlignment = ta;
                                }
                            }
                            #endregion
                            #region FontRatio
                            float _FontRatio;
                            if (GetAttribute(node, "FontRatio", out _FontRatio))
                            {
                                btnNew.FontRatio = _FontRatio;
                            }
                            #endregion
                            LayoutControl lcButton = new LayoutControl(btnNew);
                            SetLayoutControl(node, lcButton);
                            parent.AddObject(lcButton);
                        }
                        else
                        {
                            throw new Exception("Button Parameter : \"_Text\" ERROR !!!");
                        }
                        #endregion
                        #endregion
                        break;
                    case "ImageButton":
                        #region ImageButton
                        string btnImgText = "";
                        string btnImgPath = "";
                        string btnImgFormat = "";
                        #region -->Text
                        if (GetAttribute(node, "_Text", out btnImgText) &&
                            GetAttribute(node, "_Image", out btnImgPath) &&
                            GetAttribute(node, "_ImageFormat", out btnImgFormat))
                        {
                            #region jBtnType
                            string btnType = "";
                            jBtnType _type = jBtnType.Normal;
                            if (GetAttribute(node, "Type", out btnType))
                            {
                                if (!Enum.TryParse(btnType, out _type))
                                {
                                    _type = jBtnType.Normal;
                                }
                            }
                            #endregion
                            #region Image
                            uiImage image = new uiImage(btnImgPath, btnImgFormat, false);
                            #region Opacity
                            float _Opacity;
                            if (GetAttribute(node, "Opacity", out _Opacity))
                            {
                                image.Opacity = _Opacity;
                            }
                            #endregion
                            #region AspectRatio
                            float _AspectRatio = 0;
                            if (GetAttribute(node, "AspectRatio", out _AspectRatio))
                            {
                                image.AspectRatio = _AspectRatio;
                            }
                            #endregion
                            #region Offset
                            float _Offset = 0.5f;
                            if (GetAttribute(node, "Offset", out _Offset))
                            {
                                image.ImageOffset = _Offset;
                            }
                            #endregion
                            #endregion
                            if (index != -1)
                                btnImgText += index + 1;
                            uiButton btnNew = new uiButton(index, btnImgText, image);
                            #region **ID**
                            string buttonID = "";
                            if (GetAttribute(node, "ID", out buttonID))
                            {
                                buttonID = Prefix + buttonID;
                                if (index != -1)
                                    buttonID += index;
                                buttonList.Add(buttonID, btnNew);
                                btnNew.UIKey = buttonID;
                            }
                            #endregion
                            #region AlwaysOn
                            bool _AlwaysOn = false;
                            if (GetAttribute(node, "AlwaysOn", out _AlwaysOn))
                            {
                                btnNew.AlwaysOn = _AlwaysOn;
                            }
                            #endregion
                            #region AlwaysOn
                            bool _SelectOn = false;
                            if (GetAttribute(node, "SelectOn", out _SelectOn))
                            {
                                btnNew.SelectOn = _SelectOn;
                            }
                            #endregion
                            #region Color
                            Color4 _color;
                            if (GetAttribute(node, "Color", out _color))
                            {
                                btnNew.ForeColor = _color;
                            }
                            #endregion
                            #region BackColor
                            Color4 _BackColor;
                            if (GetAttribute(node, "BackColor", out _BackColor))
                            {
                                btnNew.BackColor = _BackColor;
                            }
                            #endregion
                            #region TextAlignment
                            string _TextAlignment;
                            if (GetAttribute(node, "TextAlignment", out _TextAlignment))
                            {
                                SharpDX.DirectWrite.TextAlignment ta;
                                if (Enum.TryParse(_TextAlignment, out ta))
                                {
                                    btnNew.textAlignment = ta;
                                }
                            }
                            #endregion
                            #region FontRatio
                            float _FontRatio;
                            if (GetAttribute(node, "FontRatio", out _FontRatio))
                            {
                                btnNew.FontRatio = _FontRatio;
                            }
                            #endregion
                            LayoutControl lcButton = new LayoutControl(btnNew);
                            SetLayoutControl(node, lcButton);
                            parent.AddObject(lcButton);
                        }
                        else
                        {
                            throw new Exception("Button Parameter : \"_Text\" ERROR !!!");
                        }
                        #endregion
                        #endregion
                        break;
                    case "DataCurve":
                        #region DataCurve
                        uiDataCurve dcNew = new uiDataCurve();
                        #region **ID**
                        string dataCurveID = "";
                        if (GetAttribute(node, "ID", out dataCurveID))
                        {
                            dataCurveID = Prefix + dataCurveID;
                            if (index != -1)
                                dataCurveID += index;
                            dataCurveList.Add(dataCurveID, dcNew);
                            dcNew.UIKey = dataCurveID;
                        }
                        #endregion
                        LayoutControl lcDC = new LayoutControl(dcNew);
                        SetLayoutControl(node, lcDC);
                        parent.AddObject(lcDC);
                        #endregion
                        break;
                    case "Image":
                        #region Image
                        string path = "";
                        string _ImageFormat;
                        #region -->Path, ImageFormat
                        if (GetAttribute(node, "_Path", out path) &&
                            GetAttribute(node, "_ImageFormat", out _ImageFormat))
                        {
                            bool click = false;
                            if (!GetAttribute(node, "Click", out click))
                            {
                                click = false;
                            }
                            uiImage imageNew = new uiImage(path, _ImageFormat, click);
                            #region **ID**
                            string imageID = "";
                            if (GetAttribute(node, "ID", out imageID))
                            {
                                imageID = Prefix + imageID;
                                if (index != -1)
                                    imageID += index;
                                imageList.Add(imageID, imageNew);
                                imageNew.UIKey = imageID;
                            }
                            #endregion
                            #region Opacity
                            float _Opacity;
                            if (GetAttribute(node, "Opacity", out _Opacity))
                            {
                                imageNew.Opacity = _Opacity;
                            }
                            #endregion
                            #region AspectRatio
                            float _AspectRatio = 0;
                            if (GetAttribute(node, "AspectRatio", out _AspectRatio))
                            {
                                imageNew.AspectRatio = _AspectRatio;
                            }
                            #endregion
                            #region Offset
                            float _Offset = 0.5f;
                            if (GetAttribute(node, "Offset", out _Offset))
                            {
                                imageNew.ImageOffset = _Offset;
                            }
                            #endregion
                            LayoutControl lcImage = new LayoutControl(imageNew);
                            SetLayoutControl(node, lcImage);
                            parent.AddObject(lcImage);
                        }
                        else
                        {
                            throw new Exception("Image Parameter : \"_Path _ImageFormat\" ERROR !!!");
                        }
                        #endregion
                        #endregion
                        break;
                    case "KeyBoard":
                        #region KeyBoard
                        uiKeyBoard kbNew = new uiKeyBoard();
                        #region **ID**
                        string keyBoardID = "";
                        if (GetAttribute(node, "ID", out keyBoardID))
                        {
                            keyBoardID = Prefix + keyBoardID;
                            if (index != -1)
                                keyBoardID += index;
                            keyBoardList.Add(keyBoardID, kbNew);
                            kbNew.UIKey = keyBoardID;
                        }
                        #endregion
                        LayoutControl lcKB = new LayoutControl(kbNew);
                        SetLayoutControl(node, lcKB);
                        parent.AddObject(lcKB);
                        #endregion
                        break;
                    case "OLED":
                        #region DataCurve
                        uiOLED oledNew = new uiOLED();
                        #region **ID**
                        string oledID = "";
                        if (GetAttribute(node, "ID", out oledID))
                        {
                            oledID = Prefix + oledID;
                            if (index != -1)
                                oledID += index;
                            oledList.Add(oledID, oledNew);
                            oledNew.UIKey = oledID;
                        }
                        #endregion
                        LayoutControl lcOLED = new LayoutControl(oledNew);
                        SetLayoutControl(node, lcOLED);
                        parent.AddObject(lcOLED);
                        #endregion
                        break;
                    case "Panel":
                        #region Panel
                        Color4 pColor;
                        #region -->Color
                        if (GetAttribute(node, "_Color", out pColor))
                        {
                            bool fill = false;
                            if (!GetAttribute(node, "Fill", out fill))
                            {
                                fill = false;
                            }
                            bool click = false;
                            if (!GetAttribute(node, "Click", out click))
                            {
                                click = false;
                            }
                            uiPanel newPanel = new uiPanel(pColor, fill, click);
                            #region **ID**
                            string panelID = "";
                            if (GetAttribute(node, "ID", out panelID))
                            {
                                panelID = Prefix + panelID;
                                if (index != -1)
                                    panelID += index;
                                panelList.Add(panelID, newPanel);
                                newPanel.UIKey = panelID;
                            }
                            #endregion
                            LayoutControl lcPnael = new LayoutControl(newPanel);
                            SetLayoutControl(node, lcPnael);
                            parent.AddObject(lcPnael);
                        }
                        else
                        {
                            throw new Exception("Panel Parameter : \"_Color\" ERROR !!!");
                        }
                        #endregion
                        #endregion
                        break;
                    case "Placeholder":
                        #region Placeholder
                        uiPlaceholder newPlace = new uiPlaceholder();
                        #region **ID**
                        string placeholderID = "";
                        if (GetAttribute(node, "ID", out placeholderID))
                        {
                            placeholderID = Prefix + placeholderID;
                            if (index != -1)
                                placeholderID += index;
                            placeList.Add(placeholderID, newPlace);
                            newPlace.UIKey = placeholderID;
                        }
                        #endregion
                        LayoutControl lcPlace = new LayoutControl(newPlace);
                        SetLayoutControl(node, lcPlace);
                        parent.AddObject(lcPlace);
                        #endregion
                        break;
                    case "Port":
                        #region Port
                        //do it
                        #endregion
                        break;
                    case "ProgressBar":
                        #region ProgressBar
                        Color4 pbColor = XmlUI.DxDeviceGreen;
                        Color4 pbBackColor = XmlUI.DxUIBackColor;
                        Color4 pbTextColor = XmlUI.DxTextColor;
                        if (GetAttribute(node, "Color", out pbColor) &&
                            GetAttribute(node, "BackColor", out pbBackColor) &&
                            GetAttribute(node, "TextColor", out pbTextColor))
                        {
                            uiProgressBar pbNew = new uiProgressBar(pbColor, pbBackColor, pbTextColor, 0f);
                            #region **ID**
                            string progressBarID = "";
                            if (GetAttribute(node, "ID", out progressBarID))
                            {
                                progressBarID = Prefix + progressBarID;
                                if (index != -1)
                                    progressBarID += index;
                                progressBarList.Add(progressBarID, pbNew);
                                pbNew.UIKey = progressBarID;
                            }
                            #endregion
                            LayoutControl lcTL = new LayoutControl(pbNew);
                            SetLayoutControl(node, lcTL);
                            parent.AddObject(lcTL);
                        }
                        else
                        {
                            throw new Exception("ProgressBar Parameter : \"Color, BackColor\" ERROR !!!");
                        }
                        #endregion
                        break;
                    case "SwitchButton":
                        #region SwitchButton
                        string sbText = "";
                        bool sbLoc = true;
                        #region -->Text
                        if (GetAttribute(node, "_Text", out sbText))
                        {
                            bool sbStyle = false;
                            if (!GetAttribute(node, "RadioStyle", out sbStyle))
                            {
                                sbStyle = false;
                            }
                            if (!GetAttribute(node, "Localization", out sbLoc))
                            {
                                sbLoc = true;
                            }
                            uiSwitchButton sbNew = new uiSwitchButton(sbText, sbStyle, sbLoc);
                            #region **ID**
                            string switchButtonID = "";
                            if (GetAttribute(node, "ID", out switchButtonID))
                            {
                                switchButtonID = Prefix + switchButtonID;
                                if (index != -1)
                                    switchButtonID += index;
                                switchBtnList.Add(switchButtonID, sbNew);
                                sbNew.UIKey = switchButtonID;
                                sbNew.ValueChange += OnSwitchButtonChange;
                            }
                            #endregion
                            LayoutControl lcSB = new LayoutControl(sbNew);
                            SetLayoutControl(node, lcSB);
                            parent.AddObject(lcSB);
                        }
                        else
                        {
                            throw new Exception("SwitchButton Parameter : \"_Text\" ERROR !!!");
                        }
                        #endregion
                        #endregion
                        break;
                    case "TextEditor":
                        #region TextEditor
                        string teText = "";
                        #region -->Text
                        if (GetAttribute(node, "_Text", out teText))
                        {
                            uiTextEditor teNew = new uiTextEditor(teText);
                            #region **ID**
                            string textEditorID = "";
                            if (GetAttribute(node, "ID", out textEditorID))
                            {
                                textEditorID = Prefix + textEditorID;
                                if (index != -1)
                                    textEditorID += index;
                                textEditorList.Add(textEditorID, teNew);
                                teNew.UIKey = textEditorID;
                                teNew.TextChange += OnTextEditorChange;
                            }
                            #endregion
                            #region Password
                            bool _Password = false;
                            if (GetAttribute(node, "Password", out _Password))
                            {
                                teNew.password = _Password;
                            }
                            #endregion
                            LayoutControl lcTE = new LayoutControl(teNew);
                            SetLayoutControl(node, lcTE);
                            parent.AddObject(lcTE);
                        }
                        else
                        {
                            throw new Exception("TextEditor Parameter : \"_Text\" ERROR !!!");
                        }
                        #endregion
                        #endregion
                        break;
                    case "TextLable":
                        #region TextLable
                        string tlText = "";
                        #region -->Text
                        if (GetAttribute(node, "_Text", out tlText))
                        {
                            Color4 tlColor = XmlUI.DxTextColor;
                            Color4 backColor = XmlUI.DxUIBackColor;
                            float maxHeight = JoyConst.MaxFontSize;
                            bool Frame, Loc = true;
                            if (!GetAttribute(node, "Color", out tlColor))
                            {
                                tlColor = XmlUI.DxTextColor;
                            }
                            if (!GetAttribute(node, "BackColor", out backColor))
                            {
                                backColor = XmlUI.DxUIBackColor;
                            }
                            if (!GetAttribute(node, "MaxHeight", out maxHeight))
                            {
                                maxHeight = JoyConst.MaxFontSize;
                            }
                            if (!GetAttribute(node, "Frame", out Frame))
                            {
                                Frame = true;
                            }
                            if (!GetAttribute(node, "Localization", out Loc))
                            {
                                Loc = true;
                            }
                            uiTextLable tlNew = new uiTextLable(tlText, tlColor, backColor, maxHeight, Frame, Loc);
                            #region **ID**
                            string textLableID = "";
                            if (GetAttribute(node, "ID", out textLableID))
                            {
                                textLableID = Prefix + textLableID;
                                if (index != -1)
                                    textLableID += index;
                                textLableList.Add(textLableID, tlNew);
                                tlNew.UIKey = textLableID;
                            }
                            #endregion
                            #region TextAlignment
                            string _TextAlignment;
                            if (GetAttribute(node, "TextAlignment", out _TextAlignment))
                            {
                                SharpDX.DirectWrite.TextAlignment ta;
                                if (Enum.TryParse(_TextAlignment, out ta))
                                {
                                    tlNew.textAlignment = ta;
                                }
                            }
                            #endregion
                            #region ParagraphAlignment
                            string _ParagraphAlignment;
                            if (GetAttribute(node, "ParagraphAlignment", out _ParagraphAlignment))
                            {
                                SharpDX.DirectWrite.ParagraphAlignment pa;
                                if (Enum.TryParse(_ParagraphAlignment, out pa))
                                {
                                    tlNew.pargraphAlignment = pa;
                                }
                            }
                            #endregion
                            #region FontWeight
                            string _FontWeight;
                            if (GetAttribute(node, "FontWeight", out _FontWeight))
                            {
                                SharpDX.DirectWrite.FontWeight fw;
                                if (Enum.TryParse(_FontWeight, out fw))
                                {
                                    tlNew.fontWeight = fw;
                                }
                            }
                            #endregion
                            #region FontStyle
                            string _FontStyle;
                            if (GetAttribute(node, "FontStyle", out _FontStyle))
                            {
                                SharpDX.DirectWrite.FontStyle fs;
                                if (Enum.TryParse(_FontStyle, out fs))
                                {
                                    tlNew.fontStyle = fs;
                                }
                            }
                            #endregion
                            #region FontRatio
                            float _FontRatio;
                            if (GetAttribute(node, "FontRatio", out _FontRatio))
                            {
                                tlNew.FontRatio = _FontRatio;
                            }
                            #endregion
                            LayoutControl lcTL = new LayoutControl(tlNew);
                            SetLayoutControl(node, lcTL);
                            parent.AddObject(lcTL);
                        }
                        else
                        {
                            throw new Exception("TextLable Parameter : \"_Text\" ERROR !!!");
                        }
                        #endregion
                        #endregion
                        break;
                    case "TrackBar":
                        #region TrackBar
                        int tbMin, tbMax = 0;
                        #region -->Min Max
                        if (GetAttribute(node, "_Min", out tbMin) &&
                            GetAttribute(node, "_Max", out tbMax))
                        {
                            uiTrackBar tbNew = new uiTrackBar(tbMin, tbMax);
                            #region **ID**
                            string trackBarID = "";
                            if (GetAttribute(node, "ID", out trackBarID))
                            {
                                trackBarID = Prefix + trackBarID;
                                if (index != -1)
                                    trackBarID += index;
                                trackBarList.Add(trackBarID, tbNew);
                                tbNew.UIKey = trackBarID;
                                tbNew.ValueChange += OnTrackBarChange;
                            }
                            #endregion
                            LayoutControl lcTB = new LayoutControl(tbNew);
                            SetLayoutControl(node, lcTB);
                            parent.AddObject(lcTB);
                        }
                        else
                        {
                            throw new Exception("TrackBar Parameter : \"_Min _Max\" ERROR !!!");
                        }
                        #endregion
                        #endregion
                        break;
                }
            }
        }
        #region UI事件
        private void OnButtonLeftClick(object sender, EventArgs e)
        {
            ButtonLeftClick(null, e);
        }
        private void OnButtonRightClick(object sender, EventArgs e)
        {
            ButtonRightClick(null, e);
        }
        private void OnSwitchButtonChange(object sender, EventArgs e)
        {
            SwitchButtonChange(null, e);
        }
        private void OnTextEditorChange(object sender, EventArgs e)
        {
            TextEditorChange(null, e);
        }
        private void OnTrackBarChange(object sender, EventArgs e)
        {
            TrackBarChange(null, e);
        }
        //
        private void doNothing(object sender, EventArgs e)
        {
        }
        #endregion
        private void SetLayoutControl(XmlNode node, LayoutControl lcNew)
        {
            #region Hide
            bool _Hide;
            if (GetAttribute(node, "LayoutHide", out _Hide))
            {
                lcNew.Hide = _Hide;
            }
            #endregion
            #region Rect
            RectangleF _Rect;
            if (GetAttribute(node, "LayoutRect", out _Rect))
            {
                lcNew.Rect = _Rect;
            }
            #endregion
            #region Placeholder
            float _Placeholder = 1;
            if (GetAttribute(node, "LayoutPlaceholder", out _Placeholder))
            {
                lcNew.Placeholder = _Placeholder;
            }
            #endregion
            #region minPlaceholder
            float _minPlaceholder = 0;
            if (GetAttribute(node, "LayoutMinPlaceholder", out _minPlaceholder))
            {
                lcNew.minPlaceholder = _minPlaceholder;
            }
            #endregion
            #region AspectRatio
            float _AspectRatio = 0;
            if (GetAttribute(node, "LayoutAspectRatio", out _AspectRatio))
            {
                lcNew.AspectRatio = _AspectRatio;
            }
            #endregion
            #region maxWidth
            float _maxWidth = 0;
            if (GetAttribute(node, "LayoutMaxWidth", out _maxWidth))
            {
                lcNew.maxWidth = _maxWidth;
            }
            #endregion
            #region maxHeight
            float _maxHeight = 0;
            if (GetAttribute(node, "LayoutMaxHeight", out _maxHeight))
            {
                lcNew.maxHeight = _maxHeight;
            }
            #endregion
            #region Info
            string _Info = "";
            if (GetAttribute(node, "LayoutInfo", out _Info))
            {
                lcNew.Info = _Info;
            }
            #endregion
            #region BackColorLow
            Color4 colorLow;
            if (GetAttribute(node, "LayoutBackColorLow", out colorLow))
            {
                lcNew.BackColorLow = colorLow;
            }
            #endregion
            #region BackColorMedium
            Color4 colorMedium;
            if (GetAttribute(node, "LayoutBackColorMedium", out colorMedium))
            {
                lcNew.BackColorMedium = colorMedium;
            }
            #endregion
            #region BackColorHigh
            Color4 colorHigh;
            if (GetAttribute(node, "LayoutBackColorHigh", out colorHigh))
            {
                lcNew.BackColorHigh = colorHigh;
            }
            #endregion
        }
        #region 获取参数
        public bool GetAttribute<TEnum>(XmlNode node, string name, out TEnum value) where TEnum : struct
        {
            if (node == null)
            {
                value = default(TEnum);
                return false;
            }
            foreach (XmlAttribute attri in node.Attributes)
            {
                if (name.Equals(attri.Name))
                {
                    TEnum t = default(TEnum);
                    if (Enum.TryParse<TEnum>(attri.Value, out t))
                    {
                        value = t;
                        return true;
                    }
                }
            }
            value = default(TEnum);
            return false;
        }
        public bool GetAttribute(XmlNode node, string name, out byte value)
        {
            if (node == null)
            {
                value = 0;
                return false;
            }
            foreach (XmlAttribute attri in node.Attributes)
            {
                if (name.Equals(attri.Name))
                {
                    byte _temp = 0;
                    if (byte.TryParse(attri.Value, out _temp))
                    {
                        value = _temp;
                        return true;
                    }
                }
            }
            value = 0;
            return false;
        }
        public bool GetAttribute(XmlNode node, string name, out UInt16 value)
        {
            if (node == null)
            {
                value = 0;
                return false;
            }
            foreach (XmlAttribute attri in node.Attributes)
            {
                if (name.Equals(attri.Name))
                {
                    string attriValue = attri.Value.Replace(" ", "");
                    UInt16 _temp = 0;
                    if (UInt16.TryParse(attriValue, out _temp))
                    {
                        value = _temp;
                        return true;
                    }
                }
            }
            value = 0;
            return false;
        }
        public bool GetAttribute(XmlNode node, string name, out int value)
        {
            if (node == null)
            {
                value = 0;
                return false;
            }
            foreach (XmlAttribute attri in node.Attributes)
            {
                if (name.Equals(attri.Name))
                {
                    string attriValue = attri.Value.Replace(" ", "");
                    int _temp = 0;
                    if (int.TryParse(attriValue, out _temp))
                    {
                        value = _temp;
                        return true;
                    }
                }
            }
            value = -1;
            return false;
        }
        public bool GetAttribute(XmlNode node, string name, out float value)
        {
            if (node == null)
            {
                value = 0f;
                return false;
            }
            foreach (XmlAttribute attri in node.Attributes)
            {
                if (name.Equals(attri.Name))
                {
                    string attriValue = attri.Value.Replace(" ", "");
                    float _temp = 0f;
                    if (float.TryParse(attriValue, out _temp))
                    {
                        value = _temp;
                        return true;
                    }
                }
            }
            value = 0f;
            return false;
        }
        public bool GetAttribute(XmlNode node, string name, out string value)
        {
            if (node == null)
            {
                value = "";
                return false;
            }
            foreach (XmlAttribute attri in node.Attributes)
            {
                if (name.Equals(attri.Name))
                {
                    value = attri.Value;
                    return true;
                }
            }
            value = "";
            return false;
        }
        public bool GetAttribute(XmlNode node, string name, out bool value)
        {
            if (node == null)
            {
                value = false;
                return false;
            }
            foreach (XmlAttribute attri in node.Attributes)
            {
                if (name.Equals(attri.Name))
                {
                    string attriValue = attri.Value.Replace(" ", "");
                    if (!bool.TryParse(attriValue, out value))
                    {
                        value = true;
                        return false;
                    }
                    return true;
                }
            }
            value = false;
            return false;
        }
        public bool GetAttribute(XmlNode node, string name, out Color4 value)
        {
            if (node == null)
            {
                value = XmlUI.DxTextColor;
                return false;
            }
            foreach (XmlAttribute attri in node.Attributes)
            {
                if (name.Equals(attri.Name))
                {
                    string attriValue = attri.Value.Replace(" ", "");
                    string[] floatList = attriValue.Split(',');
                    if (floatList.Length == 4)
                    {
                        float r, g, b, a = 0f;
                        if (float.TryParse(floatList[0], out r) &&
                            float.TryParse(floatList[1], out g) &&
                            float.TryParse(floatList[2], out b) &&
                            float.TryParse(floatList[3], out a))
                        {
                            value = new Color4(r, g, b, a);
                            return true;
                        }
                    }
                }
            }
            value = new Color4();
            return false;
        }
        public bool GetAttribute(XmlNode node, string name, out RectangleF value)
        {
            if (node == null)
            {
                value = new RectangleF();
                return false;
            }
            foreach (XmlAttribute attri in node.Attributes)
            {
                if (name.Equals(attri.Name))
                {
                    string attriValue = attri.Value.Replace(" ", "");
                    string[] floatList = attriValue.Split(',');
                    if (floatList.Length == 4)
                    {
                        float x, y, w, h = 0f;
                        if (float.TryParse(floatList[0], out x) &&
                            float.TryParse(floatList[1], out y) &&
                            float.TryParse(floatList[2], out w) &&
                            float.TryParse(floatList[3], out h))
                        {
                            value = new RectangleF(x, y, w, h);
                            return true;
                        }
                    }
                }
            }
            value = new RectangleF();
            return false;
        }
        #endregion
    }
}
