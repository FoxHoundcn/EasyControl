using SharpDX;
using System;
using System.Collections.Generic;

namespace EasyControl
{
    public class DeviceControlControl : iUiLogic
    {
        //---------------------------------------------------------------------------------------
        JoyObject currentObj = null;
        JoyDevice currentDev = null;
        #region Pin
        LayoutControl pinControl;
        uiSwitchButton sbHC165;
        uiSwitchButton sbCD4021;
        uiSwitchButton sbInReversal;
        #endregion
        #region Power
        LayoutControl powerControl;
        uiSwitchButton sbUsbPower;
        #endregion
        #region Band
        LayoutControl bandControl;
        uiTextLable bandTip;
        uiTrackBar bandTB;
        #endregion
        #region ADC
        LayoutControl adcControl;
        uiTrackBar adcCCTB;
        uiTrackBar adcCYTB;
        uiTrackBar adcPCTB;
        #endregion
        #region TLE5010
        LayoutControl hellControl;
        #endregion
        #region Button
        LayoutControl buttonControl;
        LayoutControl encodeControl;
        uiSwitchButton sbStep;
        uiSwitchButton sbTwoStep;
        uiSwitchButton sbFourStep;
        uiSwitchButton sbJoy;
        uiSwitchButton sbKey;
        uiSwitchButton sbOutTurn;
        LayoutControl OutTurnSwitchLC;
        //----keyBoard
        LayoutControl keyBoardControl;
        uiKeyBoard keyBoard;
        //----keyBoardFN
        LayoutControl keyFNControl;
        uiKeyBoard keyFN;
        #endregion
        #region Pulse
        LayoutControl pulseControl;
        uiTrackBar pulseTB;
        #endregion
        #region MouseSpeed
        LayoutControl mouseSpeedControl;
        uiTrackBar mouseSpeedTB;
        #endregion
        #region Percentage
        LayoutControl percentageControl;
        uiTrackBar percentageTB;
        #endregion
        #region Axis
        LayoutControl axisControl;
        LayoutControl axisOutControl;
        uiDataCurve axisDC;
        uiSwitchButton sbAxisReverse;
        uiSwitchButton sbAxisAutoRange;
        uiSwitchButton sbAxisCalibration;
        LayoutControl calibrationControl;
        LayoutControl axisSwitchControl;
        uiTrackBar shiftBit;
        //----Max,DZone
        uiTextEditor maxValueTE;
        uiTrackBar maxValueTB;
        uiTrackBar maxDZoneTB;
        //----Mid,DZone
        uiTextEditor midValueTE;
        uiTrackBar midValueTB;
        uiTrackBar midDZoneTB;
        //----Min,DZone
        uiTextEditor minValueTE;
        uiTrackBar minValueTB;
        uiTrackBar minDZoneTB;
        #endregion
        #region Hat
        LayoutControl hatControl;
        #endregion
        #region Led
        LayoutControl ledControl;
        LayoutControl ledSwitchControl;
        LayoutControl ledControlTypeAControl;
        LayoutControl ledControlTypeBControl;
        LayoutControl ledOrderControl;
        LayoutControl ledLightControl;
        LayoutControl ledLinkControl;
        LayoutControl ledMatrixControl;
        uiTrackBar backLightTB;
        uiTrackBar ledLinkTB;
        uiSwitchButton sbNoneType;
        uiSwitchButton sbAlwaysType;
        uiSwitchButton sbControlType;
        uiSwitchButton sbFnType;
        uiSwitchButton sbCapsType;
        uiSwitchButton sbNumType;
        uiSwitchButton sbLedOn;
        uiPanel colorPanel;
        uiTrackBar redTB;
        uiTrackBar greenTB;
        uiTrackBar blueTB;
        uiPanel openColorPanel;
        uiTrackBar openRedTB;
        uiTrackBar openGreenTB;
        uiTrackBar openBlueTB;
        #endregion
        #region ColorInfo
        LayoutControl colorInfoControl;
        uiSwitchButton sbStandbyModeNone;
        uiSwitchButton sbStandbyModeRainbow;
        uiSwitchButton sbStandbyModeDynamic;
        uiSwitchButton sbStandbyModeRGBWave;
        uiSwitchButton sbStandbyModeCustom;
        uiSwitchButton sbStandbyModeClick;
        uiSwitchButton sbClickModeNone;
        uiSwitchButton sbClickModeRainbow;
        uiSwitchButton sbClickModeDynamic;
        uiSwitchButton sbClickModeRGBWave;
        uiSwitchButton sbClickModeCustom;
        uiSwitchButton sbClickModeClick;
        uiSwitchButton sbFnModeNone;
        uiSwitchButton sbFnModeRainbow;
        uiSwitchButton sbFnModeDynamic;
        uiSwitchButton sbFnModeRGBWave;
        uiSwitchButton sbFnModeCustom;
        uiSwitchButton sbFnModeClick;
        uiTrackBar colorSpeedTB;
        uiTrackBar colorOffsetTB;
        uiTextEditor colorOffsetTE;
        #endregion
        #region PWM
        LayoutControl pwmControl;
        uiSwitchButton sbPwmReversal;
        #endregion
        #region SoftData
        LayoutControl softDataControl;
        #endregion
        //////////////////////////////////////////////////////////////////////////////////////////
        public static readonly DeviceControlControl Instance = new DeviceControlControl();
        private DeviceControlControl()
        {
        }
        public void Init()
        {
            #region Input----
            #region Pin
            pinControl = XmlUI.Instance.GetLayoutControl("PinControl");
            sbHC165 = XmlUI.Instance.GetSwitchButton("hc165Switch");
            sbCD4021 = XmlUI.Instance.GetSwitchButton("cd4021Switch");
            sbHC165.ValueChange = hc165Switch;
            sbCD4021.ValueChange = cd4021Switch;
            for (int i = 0; i < JoyConst.MaxPin; i++)
            {
                uiButton btn = XmlUI.Instance.GetButton("PinButton" + i);
                btn.Index = i;
                btn.LeftButtonClick += OnPinClick;
            }
            sbInReversal = XmlUI.Instance.GetSwitchButton("ReversalSwitch");
            sbInReversal.ValueChange += ReversalSwitch;
            sbLedOn = XmlUI.Instance.GetSwitchButton("LedOnSwitch");
            sbLedOn.ValueChange += LedOnSwitch;
            #endregion
            #region Power
            powerControl = XmlUI.Instance.GetLayoutControl("PowerControl");
            sbUsbPower = XmlUI.Instance.GetSwitchButton("usbPowerSwitch");
            sbUsbPower.ValueChange = usbPowerSwitch;
            #endregion
            #region Band
            bandControl = XmlUI.Instance.GetLayoutControl("BandControl");
            bandTip = XmlUI.Instance.GetTextLable("BandTip");
            bandTB = XmlUI.Instance.GetTrackBar("BandTB");
            bandTB.ValueChange += bandCountChange;
            #endregion
            #region ADC
            adcControl = XmlUI.Instance.GetLayoutControl("AdcControl");
            for (int i = 0; i < JoyConst.MaxADC; i++)
            {
                uiButton btn = XmlUI.Instance.GetButton("AdcButton" + i);
                btn.Index = i;
                btn.LeftButtonClick += OnAdcLeftClick;
            }
            adcCCTB = XmlUI.Instance.GetTrackBar("AdcCCTB");
            adcCYTB = XmlUI.Instance.GetTrackBar("AdcCYTB");
            adcPCTB = XmlUI.Instance.GetTrackBar("AdcPCTB");
            adcCCTB.ValueChange += adcCChange;
            adcCYTB.ValueChange += adcCYhange;
            adcPCTB.ValueChange += adcPChange;
            #endregion
            #region TLE5010
            hellControl = XmlUI.Instance.GetLayoutControl("HellControl");
            for (int i = 0; i < JoyConst.MaxHall; i++)
            {
                uiButton btn = XmlUI.Instance.GetButton("HellButton" + i);
                btn.Index = i;
                btn.LeftButtonClick += OnHellLeftClick;
            }
            #endregion
            #endregion
            #region Output----
            #region Button
            buttonControl = XmlUI.Instance.GetLayoutControl("ButtonControl");
            for (int i = 0; i < JoyConst.MaxButton; i++)
            {
                uiButton btn = XmlUI.Instance.GetButton("ButtonButton" + i);
                btn.Index = i;
                btn.RightButtonClick += OnButtonClick;
            }
            encodeControl = XmlUI.Instance.GetLayoutControl("EncodeTypeControl");
            sbStep = XmlUI.Instance.GetSwitchButton("Step");
            sbStep.ValueChange = EncodeStepSwitch;
            sbTwoStep = XmlUI.Instance.GetSwitchButton("TwoStep");
            sbTwoStep.ValueChange = EncodeTwoStepSwitch;
            sbFourStep = XmlUI.Instance.GetSwitchButton("FourStep");
            sbFourStep.ValueChange = EncodeFourStepSwitch;
            sbJoy = XmlUI.Instance.GetSwitchButton("JoyStickSwitch");
            sbJoy.ValueChange = JoyStickSwitch;
            sbKey = XmlUI.Instance.GetSwitchButton("KeyBoardSwitch");
            sbKey.ValueChange = KeyBoardSwitch;
            sbOutTurn = XmlUI.Instance.GetSwitchButton("OutTurnSwitch");
            sbOutTurn.ValueChange = OutTurnSwitch;
            OutTurnSwitchLC = XmlUI.Instance.GetLayoutControl("OutTurnSwitchLC");
            //----KeyBoard
            keyBoardControl = XmlUI.Instance.GetLayoutControl("KeyBoardControl");
            keyBoard = XmlUI.Instance.GetKeyBoard("KeyBoard");
            keyBoard.KeyboardClick += OnKeyboardClick;
            //----KeyBoardFN
            keyFNControl = XmlUI.Instance.GetLayoutControl("KeyFNControl");
            keyFN = XmlUI.Instance.GetKeyBoard("KeyFN");
            keyFN.KeyboardClick += OnKeyFNClick;
            #endregion
            #region Pulse
            pulseControl = XmlUI.Instance.GetLayoutControl("PulseControl");
            pulseTB = XmlUI.Instance.GetTrackBar("PulseTB");
            pulseTB.ValueChange += pulseCountChange;
            #endregion
            #region mouseSpeed
            mouseSpeedControl = XmlUI.Instance.GetLayoutControl("MouseSpeedControl");
            mouseSpeedTB = XmlUI.Instance.GetTrackBar("MouseSpeedTB");
            mouseSpeedTB.ValueChange += mouseSpeedCountChange;
            #endregion
            #region mouseSpeed
            percentageControl = XmlUI.Instance.GetLayoutControl("PercentageControl");
            percentageTB = XmlUI.Instance.GetTrackBar("PercentageTB");
            percentageTB.ValueChange += percentageCountChange;
            #endregion
            #region Axis
            axisControl = XmlUI.Instance.GetLayoutControl("AxisControl");
            axisOutControl = XmlUI.Instance.GetLayoutControl("AxisOutControl");
            for (int i = 0; i < JoyConst.MaxFormat; i++)
            {
                uiButton btn = XmlUI.Instance.GetButton("FormatButton" + i);
                btn.Index = i;
                btn.LeftButtonClick += OnChangeFormatClick;
                btn.RightButtonClick += OnFormatClick;
            }
            axisDC = XmlUI.Instance.GetDataCurve("AxisDataCurve");
            for (int i = 0; i < JoyConst.MaxAxis; i++)
            {
                uiButton btn = XmlUI.Instance.GetButton("AxisButton" + i);
                btn.Index = i;
                btn.RightButtonClick += OnAxisClick;
            }
            sbAxisReverse = XmlUI.Instance.GetSwitchButton("AxisReverseSwitch");
            sbAxisCalibration = XmlUI.Instance.GetSwitchButton("AxisCalibrationSwitch");
            sbAxisAutoRange = XmlUI.Instance.GetSwitchButton("AxisAutoRangeSwitch");
            sbAxisReverse.ValueChange = AxisReverseSwitch;
            sbAxisCalibration.ValueChange = AxisCalibrationSwitch;
            sbAxisAutoRange.ValueChange = AxisAutoRangeSwitch;
            calibrationControl = XmlUI.Instance.GetLayoutControl("CalibrationControl");
            axisSwitchControl = XmlUI.Instance.GetLayoutControl("AxisSwitchControl");
            //----Shift
            shiftBit = XmlUI.Instance.GetTrackBar("ShiftTB");
            shiftBit.ValueChange += OnShiftValueChange;
            //----Max
            uiButton maxValueBtn = XmlUI.Instance.GetButton("AxisMaxValue");
            maxValueBtn.LeftButtonClick += OnMaxValueClick;
            maxValueTE = XmlUI.Instance.GetTextEditor("AxisMaxTE");
            maxValueTE.TextChange += OnMaxValueTextChange;
            maxValueTB = XmlUI.Instance.GetTrackBar("AxisMaxTB");
            maxValueTB.ValueChange += OnMaxValueValueChange;
            maxDZoneTB = XmlUI.Instance.GetTrackBar("AxisMaxZoneTB");
            maxDZoneTB.ValueChange += OnMaxDZoneValueChange;
            //----Mid
            uiButton midValueBtn = XmlUI.Instance.GetButton("AxisMidValue");
            midValueBtn.LeftButtonClick += OnMidValueClick;
            midValueTE = XmlUI.Instance.GetTextEditor("AxisMidTE");
            midValueTE.TextChange += OnMidValueTextChange;
            midValueTB = XmlUI.Instance.GetTrackBar("AxisMidTB");
            midValueTB.ValueChange += OnMidValueValueChange;
            midDZoneTB = XmlUI.Instance.GetTrackBar("AxisMidZoneTB");
            midDZoneTB.ValueChange += OnMidDZoneValueChange;
            //----Min
            uiButton minValueBtn = XmlUI.Instance.GetButton("AxisMinValue");
            minValueBtn.LeftButtonClick += OnMinValueClick;
            minValueTE = XmlUI.Instance.GetTextEditor("AxisMinTE");
            minValueTE.TextChange += OnMinValueTextChange;
            minValueTB = XmlUI.Instance.GetTrackBar("AxisMinTB");
            minValueTB.ValueChange += OnMinValueValueChange;
            minDZoneTB = XmlUI.Instance.GetTrackBar("AxisMinZoneTB");
            minDZoneTB.ValueChange += OnMinDZoneValueChange;
            //----AxisID
            for (int i = 0; i < JoyConst.MaxAxis; i++)
            {
                uiButton btn = XmlUI.Instance.GetButton("AxisID" + i);
                btn.Index = i;
                btn.LeftButtonClick += OnAxisIDClick;
            }
            #endregion
            #region Hat
            hatControl = XmlUI.Instance.GetLayoutControl("HatControl");
            #endregion
            #region Led
            ledControl = XmlUI.Instance.GetLayoutControl("LedControl");
            ledSwitchControl = XmlUI.Instance.GetLayoutControl("LedSwitchControl");
            ledControlTypeAControl = XmlUI.Instance.GetLayoutControl("LedControlTypeAControl");
            ledControlTypeBControl = XmlUI.Instance.GetLayoutControl("LedControlTypeBControl");
            ledOrderControl = XmlUI.Instance.GetLayoutControl("LedOrderControl");
            ledLightControl = XmlUI.Instance.GetLayoutControl("LedLightControl");
            backLightTB = XmlUI.Instance.GetTrackBar("LedLightTB");
            backLightTB.ValueChange += BackLightChange;
            for (int i = 0; i < JoyConst.MaxLedOrder; i++)
            {
                uiButton btn = XmlUI.Instance.GetButton("LedOrder" + i);
                btn.Index = i;
                btn.LeftButtonClick += OnLedOrderClick;
            }
            ledLinkControl = XmlUI.Instance.GetLayoutControl("LedLinkControl");
            ledLinkTB = XmlUI.Instance.GetTrackBar("LedLinkTB");
            ledLinkTB.ValueChange += ledLinkCountChange;
            ledMatrixControl = XmlUI.Instance.GetLayoutControl("LedMatrixControl");
            for (int i = 0; i < JoyConst.MaxLed; i++)
            {
                uiButton btn = XmlUI.Instance.GetButton("LedButton" + i);
                btn.Index = i;
                btn.LeftButtonClick += OnLedLeftClick;
                btn.RightButtonClick += OnLedRightClick;
            }
            sbNoneType = XmlUI.Instance.GetSwitchButton("LedNoneSwitch");
            sbNoneType.ValueChange = LedNoneSwitch;
            sbAlwaysType = XmlUI.Instance.GetSwitchButton("LedAlwaysSwitch");
            sbAlwaysType.ValueChange = LedAlwaysSwitch;
            sbControlType = XmlUI.Instance.GetSwitchButton("LedControlSwitch");
            sbControlType.ValueChange += LedControlSwitch;
            sbFnType = XmlUI.Instance.GetSwitchButton("LedFNSwitch");
            sbFnType.ValueChange += LedFnSwitch;
            sbCapsType = XmlUI.Instance.GetSwitchButton("LedCapsSwitch");
            sbCapsType.ValueChange += LedCapsSwitch;
            sbNumType = XmlUI.Instance.GetSwitchButton("LedNumSwitch");
            sbNumType.ValueChange += LedNumSwitch;
            colorPanel = XmlUI.Instance.GetPanel("LedColorSelect");
            redTB = XmlUI.Instance.GetTrackBar("LedColorR");
            greenTB = XmlUI.Instance.GetTrackBar("LedColorG");
            blueTB = XmlUI.Instance.GetTrackBar("LedColorB");
            redTB.ValueChange += OnRedValueChange;
            greenTB.ValueChange += OnGreenValueChange;
            blueTB.ValueChange += OnBlueValueChange;
            openColorPanel = XmlUI.Instance.GetPanel("OpenColorSelect");
            openRedTB = XmlUI.Instance.GetTrackBar("OpenColorR");
            openGreenTB = XmlUI.Instance.GetTrackBar("OpenColorG");
            openBlueTB = XmlUI.Instance.GetTrackBar("OpenColorB");
            openRedTB.ValueChange += OnOpenRedValueChange;
            openGreenTB.ValueChange += OnOpenGreenValueChange;
            openBlueTB.ValueChange += OnOpenBlueValueChange;
            #endregion
            #region ColorInfo
            colorInfoControl = XmlUI.Instance.GetLayoutControl("ColorInfoControl");
            //----
            sbStandbyModeNone = XmlUI.Instance.GetSwitchButton("StandbyModeNone");
            sbStandbyModeNone.ValueChange += OnStandbyModeNone;
            sbStandbyModeRainbow = XmlUI.Instance.GetSwitchButton("StandbyModeRainbow");
            sbStandbyModeRainbow.ValueChange += OnStandbyModeRainbow;
            sbStandbyModeDynamic = XmlUI.Instance.GetSwitchButton("StandbyModeDynamic");
            sbStandbyModeDynamic.ValueChange += OnStandbyModeDynamic;
            sbStandbyModeRGBWave = XmlUI.Instance.GetSwitchButton("StandbyModeRGBWave");
            sbStandbyModeRGBWave.ValueChange += OnStandbyModeRGBWave;
            sbStandbyModeCustom = XmlUI.Instance.GetSwitchButton("StandbyModeCustom");
            sbStandbyModeCustom.ValueChange += OnStandbyModeCustom;
            sbStandbyModeClick = XmlUI.Instance.GetSwitchButton("StandbyModeClick");
            sbStandbyModeClick.ValueChange += OnStandbyModeClick;
            //----
            sbClickModeNone = XmlUI.Instance.GetSwitchButton("ClickModeNone");
            sbClickModeNone.ValueChange += OnClickModeNone;
            sbClickModeRainbow = XmlUI.Instance.GetSwitchButton("ClickModeRainbow");
            sbClickModeRainbow.ValueChange += OnClickModeRainbow;
            sbClickModeDynamic = XmlUI.Instance.GetSwitchButton("ClickModeDynamic");
            sbClickModeDynamic.ValueChange += OnClickModeDynamic;
            sbClickModeRGBWave = XmlUI.Instance.GetSwitchButton("ClickModeRGBWave");
            sbClickModeRGBWave.ValueChange += OnClickModeRGBWave;
            sbClickModeCustom = XmlUI.Instance.GetSwitchButton("ClickModeCustom");
            sbClickModeCustom.ValueChange += OnClickModeCustom;
            sbClickModeClick = XmlUI.Instance.GetSwitchButton("ClickModeClick");
            sbClickModeClick.ValueChange += OnClickModeClick;
            //----
            sbFnModeNone = XmlUI.Instance.GetSwitchButton("FnModeNone");
            sbFnModeNone.ValueChange += OnFnModeNone;
            sbFnModeRainbow = XmlUI.Instance.GetSwitchButton("FnModeRainbow");
            sbFnModeRainbow.ValueChange += OnFnModeRainbow;
            sbFnModeDynamic = XmlUI.Instance.GetSwitchButton("FnModeDynamic");
            sbFnModeDynamic.ValueChange += OnFnModeDynamic;
            sbFnModeRGBWave = XmlUI.Instance.GetSwitchButton("FnModeRGBWave");
            sbFnModeRGBWave.ValueChange += OnFnModeRGBWave;
            sbFnModeCustom = XmlUI.Instance.GetSwitchButton("FnModeCustom");
            sbFnModeCustom.ValueChange += OnFnModeCustom;
            sbFnModeClick = XmlUI.Instance.GetSwitchButton("FnModeClick");
            sbFnModeClick.ValueChange += OnFnModeClick;
            //----
            colorSpeedTB = XmlUI.Instance.GetTrackBar("ColorSpeedTB");
            colorSpeedTB.ValueChange += OnColorSpeedChange;
            colorOffsetTB = XmlUI.Instance.GetTrackBar("ColorOffsetTB");
            colorOffsetTB.ValueChange += OnColorOffsetChange;
            colorOffsetTE = XmlUI.Instance.GetTextEditor("ColorOffsetTE");
            colorOffsetTE.TextChange += OnColorOffsetEditor;
            #endregion
            #region PWM
            pwmControl = XmlUI.Instance.GetLayoutControl("PwmControl");
            for (int i = 0; i < JoyConst.MaxPWM; i++)
            {
                uiButton btn = XmlUI.Instance.GetButton("PwmButton" + i);
                btn.Index = i;
                btn.LeftButtonClick += OnPwmClick;
            }
            sbPwmReversal = XmlUI.Instance.GetSwitchButton("PwmTurnSwitch");
            sbPwmReversal.ValueChange += PwmReversal;
            #endregion
            #region SoftData
            softDataControl = XmlUI.Instance.GetLayoutControl("SoftDataControl");
            for (int i = 0; i < JoyConst.MaxSoftData; i++)
            {
                uiButton btn = XmlUI.Instance.GetButton("SoftDataButton" + i);
                btn.Index = i;
                btn.LeftButtonClick += OnSoftDataClick;
            }
            #endregion
            #endregion
            //---------------------------------------------------------------------------------------
        }
        #region 控件操作
        #region pin
        private void hc165Switch(object sender, EventArgs e)
        {
            if (currentObj != null)
            {
                currentObj.SetHC165(true);
            }
        }
        private void cd4021Switch(object sender, EventArgs e)
        {
            if (currentObj != null)
            {
                currentObj.SetHC165(false);
            }
        }
        private void usbPowerSwitch(object sender, EventArgs e)
        {
            if (currentObj != null)
            {
                currentObj.SetUsbPower(sbUsbPower.bSwitchOn);
            }
        }
        private void OnPinClick(object sender, EventArgs e)
        {
            JoyIndexChangeArgs args = (JoyIndexChangeArgs)e;
            if (currentObj != null && currentDev != null)
            {
                currentDev.inPort = (byte)args.Index;
            }
        }
        private void ReversalSwitch(object sender, EventArgs e)
        {
            if (currentObj != null && currentDev != null)
            {
                currentDev.inInversion = sbInReversal.bSwitchOn;
            }
        }
        private void LedOnSwitch(object sender, EventArgs e)
        {
            if (currentObj != null && currentDev != null)
            {
                currentDev.outLed = sbLedOn.bSwitchOn;
            }
        }
        #endregion
        #region button
        private void OnButtonClick(object sender, EventArgs e)
        {
            JoyIndexChangeArgs args = (JoyIndexChangeArgs)e;
            if (currentObj != null)
            {
                currentObj.SelectButton = args.Index;
            }
        }
        private void OutTurnSwitch(object sender, EventArgs e)
        {
            if (currentObj != null && currentDev != null)
            {
                currentDev.outInversion = sbOutTurn.bSwitchOn;
            }
        }
        private void EncodeStepSwitch(object sender, EventArgs e)
        {
            if (currentObj != null && currentDev != null)
            {
                currentDev.encodeType = EncodeType.Step;
            }
        }
        private void EncodeTwoStepSwitch(object sender, EventArgs e)
        {
            if (currentObj != null && currentDev != null)
            {
                currentDev.encodeType = EncodeType.TwoStep;
            }
        }
        private void EncodeFourStepSwitch(object sender, EventArgs e)
        {
            if (currentObj != null && currentDev != null)
            {
                currentDev.encodeType = EncodeType.FourStep;
            }
        }
        private void JoyStickSwitch(object sender, EventArgs e)
        {
            if (currentObj != null && currentDev != null)
            {
                switch (currentObj.HardwareVersion)
                {
                    case "KB":
                        currentDev.outType = OutputType.Keyboard;
                        break;
                    default:
                        currentDev.outType = OutputType.Joystick;
                        break;
                }
            }
        }
        private void KeyBoardSwitch(object sender, EventArgs e)
        {
            if (currentObj != null && currentDev != null)
            {
                currentDev.outType = OutputType.Keyboard;
            }
        }
        private void OnKeyboardClick(object sender, EventArgs e)
        {
            if (currentObj != null)
            {
                Button button = currentObj.GetButton(currentObj.SelectButton);
                if (button != null)
                {
                    if (button.Code != keyBoard.Code)
                        button.Code = keyBoard.Code;
                    else
                        button.Code = 0;
                    if (button.Code == 0xFE ||//FN
                        button.Code == 0x53 ||//CapsLock
                        button.Code == 0x39)//NumLock
                    {
                        button.Fun = 0;
                        button.CodeFN = button.Code;
                        button.FunFN = 0;
                    }
                    else
                    {
                        if (button.CodeFN == 0xFE ||//FN
                            button.CodeFN == 0x53 ||//CapsLock
                            button.CodeFN == 0x39)//NumLock
                            button.CodeFN = 0;
                        button.Fun = keyBoard.Fun;
                    }
                }
            }
        }
        private void OnKeyFNClick(object sender, EventArgs e)
        {
            if (currentObj != null)
            {
                Button button = currentObj.GetButton(currentObj.SelectButton);
                if (button != null)
                {
                    if (button.CodeFN != keyFN.Code)
                        button.CodeFN = keyFN.Code;
                    else
                        button.CodeFN = 0;
                    if (button.CodeFN == 0xFE ||//FN
                        button.CodeFN == 0x53 ||//CapsLock
                        button.CodeFN == 0x39)//NumLock
                    {
                        button.FunFN = 0;
                        button.Code = button.CodeFN;
                        button.Fun = 0;
                    }
                    else
                    {
                        if (button.Code == 0xFE ||//FN
                            button.Code == 0x53 ||//CapsLock
                            button.Code == 0x39)//NumLock
                            button.Code = 0;
                        button.FunFN = keyFN.Fun;
                    }
                }
            }
        }
        #endregion
        #region band
        private void bandCountChange(object sender, EventArgs e)
        {
            if (currentObj != null && currentDev != null)
            {
                switch (currentDev.Type)
                {
                    case DeviceType.SB_SoftSwitch:
                    case DeviceType.SB_ModeSwitch:
                        currentDev.ledCount = (byte)bandTB.Value;
                        break;
                    case DeviceType.MB_MultiModeEncode:
                        currentDev.outCount = (byte)(bandTB.Value * 2);
                        break;
                    default:
                        currentDev.outCount = (byte)bandTB.Value;
                        break;
                }
            }
        }
        #endregion
        #region adc
        private void OnAdcLeftClick(object sender, EventArgs e)
        {
            JoyIndexChangeArgs args = (JoyIndexChangeArgs)e;
            if (currentObj != null && currentDev != null)
            {
                currentDev.inPort = (byte)args.Index;
            }
        }
        private void adcCChange(object sender, EventArgs e)
        {
            if (currentObj != null && currentDev != null)
            {
                Adc adc = currentObj.GetAdc(currentDev.inPort);
                if (adc != null)
                {
                    adc.maxCC = (byte)adcCCTB.Value;
                    if (adc.maxCC > JoyConst.MaxAdcCC)
                        adc.maxCC = JoyConst.MaxAdcCC;
                }
            }
        }
        private void adcCYhange(object sender, EventArgs e)
        {
            if (currentObj != null && currentDev != null)
            {
                Adc adc = currentObj.GetAdc(currentDev.inPort);
                if (adc != null)
                {
                    adc.maxCY = (byte)adcCYTB.Value;
                    if (adc.maxCY > JoyConst.MaxAdcCY)
                        adc.maxCY = JoyConst.MaxAdcCY;
                }
            }
        }
        private void adcPChange(object sender, EventArgs e)
        {
            if (currentObj != null && currentDev != null)
            {
                Adc adc = currentObj.GetAdc(currentDev.inPort);
                if (adc != null)
                {
                    adc.maxPC = (byte)adcPCTB.Value;
                    if (adc.maxPC > JoyConst.MaxAdcPC)
                        adc.maxPC = JoyConst.MaxAdcPC;
                }
            }
        }
        #endregion
        #region axis
        private void OnFormatClick(object sender, EventArgs e)
        {
            JoyIndexChangeArgs args = (JoyIndexChangeArgs)e;
            if (currentObj != null)
            {
                currentObj.SelectFormatIn = args.Index;
            }
        }
        private void OnChangeFormatClick(object sender, EventArgs e)
        {
            JoyIndexChangeArgs args = (JoyIndexChangeArgs)e;
            if (currentObj != null && currentDev != null)
            {
                if (currentDev.portInType == InPortType.FormatOut)
                    currentDev.inPort = (byte)args.Index;
            }
        }
        private void OnAxisClick(object sender, EventArgs e)
        {
            JoyIndexChangeArgs args = (JoyIndexChangeArgs)e;
            if (currentObj != null)
            {
                currentObj.SelectAxis = args.Index;
            }
        }
        private void OnAxisIDClick(object sender, EventArgs e)
        {
            JoyIndexChangeArgs args = (JoyIndexChangeArgs)e;
            if (currentObj != null && currentDev != null)
            {
                currentObj.SetAxisID(currentObj.SelectAxis, args.Index);
            }
        }
        private void AxisReverseSwitch(object sender, EventArgs e)
        {
            if (currentObj != null && currentDev != null)
            {
                Format format = currentObj.GetSelectFormat();
                if (format != null)
                {
                    format.Reverse = sbAxisReverse.bSwitchOn;
                }
            }
        }
        private void AxisCalibrationSwitch(object sender, EventArgs e)
        {
            if (currentObj != null && currentDev != null)
            {
                Format format = currentObj.GetSelectFormat();
                if (format != null)
                {
                    format.Calibration = sbAxisCalibration.bSwitchOn;
                }
            }
        }
        private void AxisAutoRangeSwitch(object sender, EventArgs e)
        {
            if (currentObj != null && currentDev != null)
            {
                Format format = currentObj.GetSelectFormat();
                if (format != null)
                {
                    format.AutoRange = sbAxisAutoRange.bSwitchOn;
                }
            }
        }
        private void OnShiftValueChange(object sender, EventArgs e)
        {
            if (currentObj != null && currentDev != null)
            {
                Format format = currentObj.GetSelectFormat();
                if (format != null)
                {
                    format.Shift = (byte)shiftBit.Value;
                }
            }
        }
        #region formatMax
        private void OnMaxValueClick(object sender, EventArgs e)
        {
            if (currentObj != null && currentDev != null)
            {
                Format format = currentObj.GetSelectFormat();
                if (format != null)
                {
                    format.maxValue = axisDC.GetLateInValue();
                }
            }
        }
        private void OnMaxValueTextChange(object sender, EventArgs e)
        {
            int value = 0;
            int.TryParse(maxValueTE.Text, out value);
            if (currentObj != null && currentDev != null)
            {
                Format format = currentObj.GetSelectFormat();
                if (format != null)
                {
                    format.maxValue = value;
                    if (maxValueTB.Value != value)
                        maxValueTB.Value = value;
                }
            }
        }
        private void OnMaxValueValueChange(object sender, EventArgs e)
        {
            if (currentObj != null && currentDev != null)
            {
                Format format = currentObj.GetSelectFormat();
                if (format != null)
                {
                    format.maxValue = maxValueTB.Value;
                    maxValueTE.Text = maxValueTB.Value.ToString();
                }
            }
        }
        private void OnMaxDZoneValueChange(object sender, EventArgs e)
        {
            if (currentObj != null && currentDev != null)
            {
                Format format = currentObj.GetSelectFormat();
                if (format != null)
                {
                    format.maxDzone = (byte)maxDZoneTB.Value;
                }
            }
        }
        #endregion
        #region formatMid
        private void OnMidValueClick(object sender, EventArgs e)
        {
            if (currentObj != null && currentDev != null)
            {
                Format format = currentObj.GetSelectFormat();
                if (format != null)
                {
                    format.midValue = axisDC.GetLateInValue();
                }
            }
        }
        private void OnMidValueTextChange(object sender, EventArgs e)
        {
            int value = 0;
            int.TryParse(midValueTE.Text, out value);
            if (currentObj != null && currentDev != null)
            {
                Format format = currentObj.GetSelectFormat();
                if (format != null)
                {
                    format.midValue = value;
                    if (midValueTB.Value != value)
                        midValueTB.Value = value;
                }
            }
        }
        private void OnMidValueValueChange(object sender, EventArgs e)
        {
            if (currentObj != null && currentDev != null)
            {
                Format format = currentObj.GetSelectFormat();
                if (format != null)
                {
                    format.midValue = midValueTB.Value;
                    midValueTE.Text = midValueTB.Value.ToString();
                }
            }
        }
        private void OnMidDZoneValueChange(object sender, EventArgs e)
        {
            if (currentObj != null && currentDev != null)
            {
                Format format = currentObj.GetSelectFormat();
                if (format != null)
                {
                    format.midDzone = (byte)midDZoneTB.Value;
                }
            }
        }
        #endregion
        #region formatMin
        private void OnMinValueClick(object sender, EventArgs e)
        {
            if (currentObj != null && currentDev != null)
            {
                Format format = currentObj.GetSelectFormat();
                if (format != null)
                {
                    format.minValue = axisDC.GetLateInValue();
                }
            }
        }
        private void OnMinValueTextChange(object sender, EventArgs e)
        {
            int value = 0;
            int.TryParse(minValueTE.Text, out value);
            if (currentObj != null && currentDev != null)
            {
                Format format = currentObj.GetSelectFormat();
                if (format != null)
                {
                    format.minValue = value;
                    if (minValueTB.Value != value)
                        minValueTB.Value = value;
                }
            }
        }
        private void OnMinValueValueChange(object sender, EventArgs e)
        {
            if (currentObj != null && currentDev != null)
            {
                Format format = currentObj.GetSelectFormat();
                if (format != null)
                {
                    format.minValue = minValueTB.Value;
                    minValueTE.Text = minValueTB.Value.ToString();
                }
            }
        }
        private void OnMinDZoneValueChange(object sender, EventArgs e)
        {
            if (currentObj != null && currentDev != null)
            {
                Format format = currentObj.GetSelectFormat();
                if (format != null)
                {
                    format.minDzone = (byte)minDZoneTB.Value;
                }
            }
        }
        #endregion
        #endregion
        #region hell
        private void OnHellLeftClick(object sender, EventArgs e)
        {
            JoyIndexChangeArgs args = (JoyIndexChangeArgs)e;
            if (currentObj != null && currentDev != null)
            {
                currentDev.inPort = (byte)args.Index;
            }
        }
        #endregion
        #region pulse
        private void pulseCountChange(object sender, EventArgs e)
        {
            if (currentObj != null && currentDev != null)
            {
                currentDev.pulseCount = (byte)pulseTB.Value;
            }
        }
        #endregion
        #region mouseSpeed
        private void mouseSpeedCountChange(object sender, EventArgs e)
        {
            if (currentObj != null && currentDev != null)
            {
                currentDev.pulseCount = (byte)mouseSpeedTB.Value;
            }
        }
        #endregion
        #region mouseSpeed
        private void percentageCountChange(object sender, EventArgs e)
        {
            if (currentObj != null && currentDev != null)
            {
                currentDev.pulseCount = (byte)percentageTB.Value;
            }
        }
        #endregion
        #region led
        private void BackLightChange(object sender, EventArgs e)
        {
            if (currentObj != null)
            {
                currentObj.SetBackLightBrightness((byte)backLightTB.Value);
            }
        }
        private void OnLedOrderClick(object sender, EventArgs e)
        {
            JoyIndexChangeArgs args = (JoyIndexChangeArgs)e;
            if (currentObj != null)
            {
                currentObj.SetColorOrder((byte)args.Index);
            }
        }
        private void ledLinkCountChange(object sender, EventArgs e)
        {
            if (currentObj != null && currentDev != null)
            {
                currentDev.ledCount = (byte)ledLinkTB.Value;
            }
        }
        private void OnLedLeftClick(object sender, EventArgs e)
        {
            JoyIndexChangeArgs args = (JoyIndexChangeArgs)e;
            if (currentObj != null)
            {
                currentDev.ledPort = (byte)args.Index;
            }
        }
        private void OnLedRightClick(object sender, EventArgs e)
        {
            JoyIndexChangeArgs args = (JoyIndexChangeArgs)e;
            if (currentObj != null && currentDev != null)
            {
                currentObj.SelectLed = args.Index;
            }
        }
        private void LedNoneSwitch(object sender, EventArgs e)
        {
            if (currentObj != null)
            {
                Led led = currentObj.GetLed(currentObj.SelectLed);
                if (led != null)
                {
                    led.ControlType = LedControlType.LedNone;
                }
            }
        }
        private void LedAlwaysSwitch(object sender, EventArgs e)
        {
            if (currentObj != null)
            {
                Led led = currentObj.GetLed(currentObj.SelectLed);
                if (led != null)
                {
                    led.ControlType = LedControlType.LedAlways;
                }
            }
        }
        private void LedControlSwitch(object sender, EventArgs e)
        {
            if (currentObj != null)
            {
                Led led = currentObj.GetLed(currentObj.SelectLed);
                if (led != null)
                {
                    led.ControlType = LedControlType.LedControl;
                }
            }
        }
        private void LedFnSwitch(object sender, EventArgs e)
        {
            if (currentObj != null)
            {
                Led led = currentObj.GetLed(currentObj.SelectLed);
                if (led != null)
                {
                    led.ControlType = LedControlType.LedFN;
                }
            }
        }
        private void LedCapsSwitch(object sender, EventArgs e)
        {
            if (currentObj != null)
            {
                Led led = currentObj.GetLed(currentObj.SelectLed);
                if (led != null)
                {
                    led.ControlType = LedControlType.LedCapsLock;
                }
            }
        }
        private void LedNumSwitch(object sender, EventArgs e)
        {
            if (currentObj != null)
            {
                Led led = currentObj.GetLed(currentObj.SelectLed);
                if (led != null)
                {
                    led.ControlType = LedControlType.LedNumLock;
                }
            }
        }
        private void OnRedValueChange(object sender, EventArgs e)
        {
            colorPanel.ForeColor = new Color(redTB.Value / 255f, greenTB.Value / 255f, blueTB.Value / 255f);
            if (currentObj != null)
            {
                Led led = currentObj.GetLed(currentObj.SelectLed);
                if (led != null)
                {
                    led.R = (byte)redTB.Value;
                }
            }
        }
        private void OnGreenValueChange(object sender, EventArgs e)
        {
            colorPanel.ForeColor = new Color(redTB.Value / 255f, greenTB.Value / 255f, blueTB.Value / 255f);
            if (currentObj != null)
            {
                Led led = currentObj.GetLed(currentObj.SelectLed);
                if (led != null)
                {
                    led.G = (byte)greenTB.Value;
                }
            }
        }
        private void OnBlueValueChange(object sender, EventArgs e)
        {
            colorPanel.ForeColor = new Color(redTB.Value / 255f, greenTB.Value / 255f, blueTB.Value / 255f);
            if (currentObj != null)
            {
                Led led = currentObj.GetLed(currentObj.SelectLed);
                if (led != null)
                {
                    led.B = (byte)blueTB.Value;
                }
            }
        }
        //Open======================================================================
        private void OnOpenRedValueChange(object sender, EventArgs e)
        {
            openColorPanel.ForeColor = new Color(openRedTB.Value / 255f, openGreenTB.Value / 255f, openBlueTB.Value / 255f);
            if (currentObj != null)
            {
                Led led = currentObj.GetLed(currentObj.SelectLed);
                if (led != null)
                {
                    led.OpenR = (byte)openRedTB.Value;
                }
            }
        }
        private void OnOpenGreenValueChange(object sender, EventArgs e)
        {
            openColorPanel.ForeColor = new Color(openRedTB.Value / 255f, openGreenTB.Value / 255f, openBlueTB.Value / 255f);
            if (currentObj != null)
            {
                Led led = currentObj.GetLed(currentObj.SelectLed);
                if (led != null)
                {
                    led.OpenG = (byte)openGreenTB.Value;
                }
            }
        }
        private void OnOpenBlueValueChange(object sender, EventArgs e)
        {
            openColorPanel.ForeColor = new Color(openRedTB.Value / 255f, openGreenTB.Value / 255f, openBlueTB.Value / 255f);
            if (currentObj != null)
            {
                Led led = currentObj.GetLed(currentObj.SelectLed);
                if (led != null)
                {
                    led.OpenB = (byte)openBlueTB.Value;
                }
            }
        }
        #endregion
        #region ColorInfo
        private void OnStandbyModeNone(object sender, EventArgs e)
        {
            if (currentObj != null)
            {
                currentObj.SetIdleColor(ColorInfoType.ColorNone);
            }
        }
        private void OnStandbyModeRainbow(object sender, EventArgs e)
        {
            if (currentObj != null)
            {
                currentObj.SetIdleColor(ColorInfoType.ColorRainbow);
            }
        }
        private void OnStandbyModeDynamic(object sender, EventArgs e)
        {
            if (currentObj != null)
            {
                currentObj.SetIdleColor(ColorInfoType.ColorDynamicRainbow);
            }
        }
        private void OnStandbyModeRGBWave(object sender, EventArgs e)
        {
            if (currentObj != null)
            {
                currentObj.SetIdleColor(ColorInfoType.ColorRGBWave);
            }
        }
        private void OnStandbyModeCustom(object sender, EventArgs e)
        {
            if (currentObj != null)
            {
                currentObj.SetIdleColor(ColorInfoType.ColorCustom);
            }
        }
        private void OnStandbyModeClick(object sender, EventArgs e)
        {
            if (currentObj != null)
            {
                currentObj.SetIdleColor(ColorInfoType.ColorClick);
            }
        }
        private void OnClickModeNone(object sender, EventArgs e)
        {
            if (currentObj != null)
            {
                ColorInfo info = currentObj.GetColorInfo(currentObj.SelectLed);
                if (info != null)
                    info.ClickMode = ColorInfoType.ColorNone;
            }
        }
        private void OnClickModeRainbow(object sender, EventArgs e)
        {
            if (currentObj != null)
            {
                ColorInfo info = currentObj.GetColorInfo(currentObj.SelectLed);
                if (info != null)
                    info.ClickMode = ColorInfoType.ColorRainbow;
            }
        }
        private void OnClickModeDynamic(object sender, EventArgs e)
        {
            if (currentObj != null)
            {
                ColorInfo info = currentObj.GetColorInfo(currentObj.SelectLed);
                if (info != null)
                    info.ClickMode = ColorInfoType.ColorDynamicRainbow;
            }
        }
        private void OnClickModeRGBWave(object sender, EventArgs e)
        {
            if (currentObj != null)
            {
                ColorInfo info = currentObj.GetColorInfo(currentObj.SelectLed);
                if (info != null)
                    info.ClickMode = ColorInfoType.ColorRGBWave;
            }
        }
        private void OnClickModeCustom(object sender, EventArgs e)
        {
            if (currentObj != null)
            {
                ColorInfo info = currentObj.GetColorInfo(currentObj.SelectLed);
                if (info != null)
                    info.ClickMode = ColorInfoType.ColorCustom;
            }
        }
        private void OnClickModeClick(object sender, EventArgs e)
        {
            if (currentObj != null)
            {
                ColorInfo info = currentObj.GetColorInfo(currentObj.SelectLed);
                if (info != null)
                    info.ClickMode = ColorInfoType.ColorClick;
            }
        }
        private void OnFnModeNone(object sender, EventArgs e)
        {
            if (currentObj != null)
            {
                ColorInfo info = currentObj.GetColorInfo(currentObj.SelectLed);
                if (info != null)
                    info.FnMode = ColorInfoType.ColorNone;
            }
        }
        private void OnFnModeRainbow(object sender, EventArgs e)
        {
            if (currentObj != null)
            {
                ColorInfo info = currentObj.GetColorInfo(currentObj.SelectLed);
                if (info != null)
                    info.FnMode = ColorInfoType.ColorRainbow;
            }
        }
        private void OnFnModeDynamic(object sender, EventArgs e)
        {
            if (currentObj != null)
            {
                ColorInfo info = currentObj.GetColorInfo(currentObj.SelectLed);
                if (info != null)
                    info.FnMode = ColorInfoType.ColorDynamicRainbow;
            }
        }
        private void OnFnModeRGBWave(object sender, EventArgs e)
        {
            if (currentObj != null)
            {
                ColorInfo info = currentObj.GetColorInfo(currentObj.SelectLed);
                if (info != null)
                    info.FnMode = ColorInfoType.ColorRGBWave;
            }
        }
        private void OnFnModeCustom(object sender, EventArgs e)
        {
            if (currentObj != null)
            {
                ColorInfo info = currentObj.GetColorInfo(currentObj.SelectLed);
                if (info != null)
                    info.FnMode = ColorInfoType.ColorCustom;
            }
        }
        private void OnFnModeClick(object sender, EventArgs e)
        {
            if (currentObj != null)
            {
                ColorInfo info = currentObj.GetColorInfo(currentObj.SelectLed);
                if (info != null)
                    info.FnMode = ColorInfoType.ColorClick;
            }
        }
        private void OnColorSpeedChange(object sender, EventArgs e)
        {
            if (currentObj != null)
            {
                currentObj.SetDynamicSpeed((byte)colorSpeedTB.Value);
            }
        }
        private void OnColorOffsetChange(object sender, EventArgs e)
        {
            if (currentObj != null)
            {
                ColorInfo info = currentObj.GetColorInfo(currentObj.SelectLed);
                if (info != null)
                {
                    if (info.Offset != colorOffsetTB.Value)
                    {
                        info.Offset = (UInt16)colorOffsetTB.Value;
                        colorOffsetTE.Text = info.Offset.ToString();
                    }
                }
            }
        }
        private void OnColorOffsetEditor(object sender, EventArgs e)
        {
            if (currentObj != null)
            {
                ColorInfo info = currentObj.GetColorInfo(currentObj.SelectLed);
                if (info != null)
                {
                    int value = 0;
                    int.TryParse(colorOffsetTE.Text, out value);
                    info.Offset = (UInt16)value;
                    if (colorOffsetTB.Value != value)
                        colorOffsetTB.Value = value;
                }
            }
        }
        #endregion
        #region pwm
        private void OnPwmClick(object sender, EventArgs e)
        {
            JoyIndexChangeArgs args = (JoyIndexChangeArgs)e;
            if (currentObj != null && currentDev != null)
            {
                currentDev.outPort = (byte)args.Index;
            }
        }
        private void PwmReversal(object sender, EventArgs e)
        {
            if (currentObj != null && currentDev != null)
            {
                currentDev.outInversion = sbPwmReversal.bSwitchOn;
            }
        }
        #endregion
        #region SoftData
        private void OnSoftDataClick(object sender, EventArgs e)
        {

        }
        #endregion
        #endregion
        #region 设置输入UI
        public void SetPinHide(bool _pin)
        {
            pinControl.Hide = _pin;
            if (!_pin)
            {
                if (currentObj != null && currentDev != null)
                {
                    PublicData.SetButtonList("PinButton", currentObj.GetInputUsed(JoyConst.MaxPin, InPortType.Pin));
                    PublicData.SetButtonEnable("PinButton", currentObj.pinUsed);
                    List<PortShowType> clickState = currentObj.GetPinClick();
                    PublicData.SetPanelSwitch("PinClick", clickState);
                    PublicData.SetButtonSelect("PinButton", clickState);
                    sbInReversal.bSwitchOn = currentDev.inInversion;
                    if (currentObj.HC165)
                    {
                        sbHC165.bSwitchOn = true;
                        sbCD4021.bSwitchOn = false;
                    }
                    else
                    {
                        sbHC165.bSwitchOn = false;
                        sbCD4021.bSwitchOn = true;
                    }
                    if (currentObj.version1 == 3 &&
                        currentObj.version2 == 1)//V3.1版本
                    {
                        sbHC165.Name = "DirectInput";
                        sbCD4021.Name = "MatrixInput";
                    }
                    else
                    {
                        sbHC165.Name = "HC165";
                        sbCD4021.Name = "CD4021";
                    }
                }
            }
        }
        public void SetPowerHide(bool _power)
        {
            powerControl.Hide = _power;
            if (!_power)
            {
                if (currentObj != null && currentDev != null)
                {
                    sbUsbPower.bSwitchOn = currentObj.USBPower;
                }
            }
        }
        public void SetBandHide(bool _band)
        {
            bandControl.Hide = _band;
            if (!_band)
            {
                if (currentObj != null && currentDev != null)
                {
                    switch (currentDev.Type)
                    {
                        case DeviceType.SB_SoftSwitch:
                        case DeviceType.SB_ModeSwitch:
                        case DeviceType.MB_SoftBand:
                        case DeviceType.MB_BandModeSwitch:
                            if (bandTB.Value != currentDev.ledCount)
                            {
                                bandTB.Value = currentDev.ledCount;
                            }
                            break;
                        case DeviceType.MB_MultiModeEncode:
                            if (bandTB.Value != currentDev.outCount / 2)
                            {
                                bandTB.Value = currentDev.outCount / 2;
                            }
                            break;
                        default:
                            if (bandTB.Value != currentDev.outCount)
                            {
                                bandTB.Value = currentDev.outCount;
                            }
                            break;
                    }
                    switch (currentDev.Type)
                    {
                        case DeviceType.SB_SoftSwitch:
                            bandTip.Text = "SwitchCountTL";
                            break;
                        case DeviceType.SB_ModeSwitch:
                        case DeviceType.SB_MultiMode:
                        case DeviceType.MB_MultiModeEncode:
                            bandTip.Text = "ModeCountTL";
                            break;
                        case DeviceType.F_Trigger:
                            bandTip.Text = "TriggerCountTL";
                            break;
                        default:
                            bandTip.Text = "BandTL";
                            break;
                    }
                }
            }
        }
        public void SetAdcHide(bool _adc)
        {
            adcControl.Hide = _adc;
            if (!_adc)
            {
                if (currentObj != null && currentDev != null)
                {
                    PublicData.SetButtonList("AdcButton", currentObj.GetInputUsed(JoyConst.MaxADC, InPortType.ADC));
                    PublicData.SetButtonEnable("AdcButton", currentObj.adcUsed);
                    Adc adc = currentObj.GetAdc(currentDev.inPort);
                    if (adc != null)
                    {
                        if (adcCCTB.Value != adc.maxCC)
                            adcCCTB.Value = adc.maxCC;
                        if (adcCYTB.Value != adc.maxCY)
                            adcCYTB.Value = adc.maxCY;
                        if (adcPCTB.Value != adc.maxPC)
                            adcPCTB.Value = adc.maxPC;
                    }
                }
            }
        }
        public void SetHellHide(bool _hell)
        {
            hellControl.Hide = _hell;
            if (!_hell)
            {
                if (currentObj != null && currentDev != null)
                {
                    PublicData.SetButtonList("HellButton", currentObj.GetInputUsed(JoyConst.MaxHall, InPortType.Hall));
                    PublicData.SetButtonEnable("HellButton", currentObj.hallUsed);
                }
            }
        }
        public void SetPulseHide(bool _pulse)
        {
            pulseControl.Hide = _pulse;
            if (!_pulse)
            {
                if (currentObj != null && currentDev != null)
                {
                    if (pulseTB.Value != currentDev.pulseCount)
                    {
                        pulseTB.Value = currentDev.pulseCount;
                    }
                }
            }
        }
        public void SetMouseSpeedHide(bool _mouseSpeed)
        {
            mouseSpeedControl.Hide = _mouseSpeed;
            if (!_mouseSpeed)
            {
                if (currentObj != null && currentDev != null)
                {
                    if (mouseSpeedTB.Value != currentDev.pulseCount)
                    {
                        mouseSpeedTB.Value = currentDev.pulseCount;
                    }
                }
            }
        }
        public void SetPercentageHide(bool _percentage)
        {
            percentageControl.Hide = _percentage;
            if (!_percentage)
            {
                if (currentObj != null && currentDev != null)
                {
                    if (percentageTB.Value != currentDev.pulseCount)
                    {
                        percentageTB.Value = currentDev.pulseCount;
                    }
                }
            }
        }
        #endregion
        #region 设置输出UI
        public void SetButtonHide(bool _button)
        {
            buttonControl.Hide = _button;
            keyBoardControl.Hide = true;
            keyFNControl.Hide = true;
            if (!_button)
            {
                if (currentObj != null && currentDev != null)
                {
                    float AspectRatio = 10f;
                    PublicData.SetButtonList("ButtonButton", currentObj.GetButtonUsed(currentDev.outType));
                    PublicData.SetPanelUsed("ButtonJoyS", currentObj.GetButtonUsed(OutputType.Joystick), XmlUI.DxDeviceGreen);
                    PublicData.SetPanelUsed("ButtonKeyB", currentObj.GetButtonUsed(OutputType.Keyboard), XmlUI.DxDeviceBlue);
                    switch (currentDev.Type)
                    {
                        case DeviceType.MB_Switch2:
                        case DeviceType.MB_Switch3:
                        case DeviceType.MB_Band:
                        case DeviceType.MB_EncodeBand:
                        case DeviceType.MB_Switch2_Pulse:
                        case DeviceType.MB_Switch3_Pulse:
                        case DeviceType.MB_Band_Pulse:
                        case DeviceType.MB_EncodeBand_Pulse:
                        case DeviceType.MB_Encode:
                            AspectRatio += 1f;
                            OutTurnSwitchLC.Hide = false;
                            sbOutTurn.bSwitchOn = currentDev.outInversion;
                            break;
                        default:
                            OutTurnSwitchLC.Hide = true;
                            break;
                    }
                    sbJoy.bSwitchOn = false;
                    sbKey.bSwitchOn = false;
                    switch (currentDev.outType)
                    {
                        case OutputType.Joystick:
                            sbJoy.bSwitchOn = true;
                            keyBoardControl.Hide = true;
                            keyFNControl.Hide = true;
                            break;
                        case OutputType.Keyboard:
                            sbKey.bSwitchOn = true;
                            keyBoardControl.Hide = false;
                            keyFNControl.Hide = false;
                            break;
                        default:
                            break;
                    }
                    if (!keyBoardControl.Hide)
                    {
                        AspectRatio += 14f;
                    }
                    buttonControl.AspectRatio = AspectRatio / 22f;
                    Button button = currentObj.GetButton(currentObj.SelectButton);
                    if (button != null)
                    {
                        keyBoard.Code = button.Code;
                        keyBoard.Fun = button.Fun;
                        keyFN.Code = button.CodeFN;
                        keyFN.Fun = button.FunFN;
                    }
                }
            }
        }
        public void SetEncodeTypeHide(bool _encodeType)
        {
            encodeControl.Hide = _encodeType;
            if (!_encodeType)
            {
                if (currentObj != null && currentDev != null)
                {
                    switch (currentDev.encodeType)
                    {
                        case EncodeType.Step:
                            sbStep.bSwitchOn = true;
                            sbTwoStep.bSwitchOn = false;
                            sbFourStep.bSwitchOn = false;
                            break;
                        case EncodeType.TwoStep:
                            sbStep.bSwitchOn = false;
                            sbTwoStep.bSwitchOn = true;
                            sbFourStep.bSwitchOn = false;
                            break;
                        case EncodeType.FourStep:
                            sbStep.bSwitchOn = false;
                            sbTwoStep.bSwitchOn = false;
                            sbFourStep.bSwitchOn = true;
                            break;
                    }
                }
            }
        }
        public void SetFormatHide(bool _format, bool _input)
        {
            axisControl.Hide = _format;
            calibrationControl.Hide = true;
            axisSwitchControl.Hide = true;
            if (currentObj != null && currentDev != null)
            {
                if (_input)
                    PublicData.SetButtonList("FormatButton", currentObj.GetInputUsed(JoyConst.MaxFormat, InPortType.FormatOut));
                else
                    PublicData.SetButtonList("FormatButton", currentObj.GetOutputUsed(JoyConst.MaxFormat, OutPortType.FormatIn));
                Format format = currentObj.GetSelectFormat();
                if (format != null)
                {
                    axisSwitchControl.Hide = false;
                    sbAxisReverse.bSwitchOn = format.Reverse;
                    sbAxisCalibration.bSwitchOn = format.Calibration;
                    calibrationControl.Hide = !format.Calibration;
                    sbAxisAutoRange.Hide = !format.Calibration;
                    sbAxisAutoRange.bSwitchOn = format.AutoRange;
                    if (calibrationControl.Hide)
                    {
                        axisControl.AspectRatio = 15f / 22f;
                    }
                    else
                    {
                        axisControl.AspectRatio = 19f / 22f;
                    }
                    switch (currentObj.GetInFormatDataType())
                    {
                        case InPortType.ADC:
                            maxValueTB.MaxValue = JoyConst.MaxAdcValue;
                            midValueTB.MaxValue = JoyConst.MaxAdcValue;
                            minValueTB.MaxValue = JoyConst.MaxAdcValue;
                            break;
                        case InPortType.Hall:
                            switch (currentDev.Type)
                            {
                                case DeviceType.H_TLE5010:
                                    maxValueTB.MaxValue = JoyConst.MaxTLE5010Value;
                                    midValueTB.MaxValue = JoyConst.MaxTLE5010Value;
                                    minValueTB.MaxValue = JoyConst.MaxTLE5010Value;
                                    break;
                                case DeviceType.H_MLX90316:
                                    maxValueTB.MaxValue = JoyConst.MaxMLX90316Value;
                                    midValueTB.MaxValue = JoyConst.MaxMLX90316Value;
                                    minValueTB.MaxValue = JoyConst.MaxMLX90316Value;
                                    break;
                                case DeviceType.H_MLX90333:
                                    maxValueTB.MaxValue = JoyConst.MaxMLX90333Value;
                                    midValueTB.MaxValue = JoyConst.MaxMLX90333Value;
                                    minValueTB.MaxValue = JoyConst.MaxMLX90333Value;
                                    break;
                                case DeviceType.H_MLX90363:
                                    maxValueTB.MaxValue = JoyConst.MaxMLX90363Value;
                                    midValueTB.MaxValue = JoyConst.MaxMLX90363Value;
                                    minValueTB.MaxValue = JoyConst.MaxMLX90363Value;
                                    break;
                                case DeviceType.H_MLX90393:
                                    maxValueTB.MaxValue = JoyConst.MaxMLX90393Value;
                                    midValueTB.MaxValue = JoyConst.MaxMLX90393Value;
                                    minValueTB.MaxValue = JoyConst.MaxMLX90393Value;
                                    break;
                                case DeviceType.H_N35P112:
                                    maxValueTB.MaxValue = JoyConst.MaxN35P112Value;
                                    midValueTB.MaxValue = JoyConst.MaxN35P112Value;
                                    minValueTB.MaxValue = JoyConst.MaxN35P112Value;
                                    break;
                                case DeviceType.H_HX711:
                                case DeviceType.H_HX717:
                                    maxValueTB.MaxValue = JoyConst.MaxHX711Value;
                                    midValueTB.MaxValue = JoyConst.MaxHX711Value;
                                    minValueTB.MaxValue = JoyConst.MaxHX711Value;
                                    break;
                            }
                            break;
                    }
                    if (shiftBit.Value != format.Shift)
                        shiftBit.Value = format.Shift;
                    if (maxValueTB.Value != format.maxValue)
                        maxValueTB.Value = format.maxValue;
                    if (midValueTB.Value != format.midValue)
                        midValueTB.Value = format.midValue;
                    if (minValueTB.Value != format.minValue)
                        minValueTB.Value = format.minValue;
                    maxValueTE.Text = format.maxValue.ToString();
                    midValueTE.Text = format.midValue.ToString();
                    minValueTE.Text = format.minValue.ToString();
                    if (maxDZoneTB.Value != format.maxDzone)
                        maxDZoneTB.Value = format.maxDzone;
                    if (midDZoneTB.Value != format.midDzone)
                        midDZoneTB.Value = format.midDzone;
                    if (minDZoneTB.Value != format.minDzone)
                        minDZoneTB.Value = format.minDzone;
                }
            }
        }
        public void SetAxisHide(bool _axis)
        {
            axisOutControl.Hide = _axis;
            if (currentObj != null && currentDev != null)
            {
                PublicData.SetButtonList("AxisButton", currentObj.GetOutputUsed(JoyConst.MaxAxis, OutPortType.Axis));
                PublicData.SetButtonList("AxisID", currentObj.GetAxisID());
            }
        }
        public void SetHatHide(bool _hat)
        {
            hatControl.Hide = _hat;
            if (currentObj != null && currentDev != null)
            {
                List<PortShowType> portList = currentObj.GetOutputUsed(JoyConst.MaxHat, OutPortType.Hat);
                for (int i = 0; i < JoyConst.MaxHat; i++)
                {
                    uiHatSetting hat = XmlUI.Instance.GetHat(i + "Hat");
                    if ((currentDev.Type == DeviceType.MB_Hat || currentDev.Type == DeviceType.F_Hat) && currentDev.outPort == i)
                    {
                        hat.Hide = false;
                    }
                    else
                    {
                        hat.Hide = true;
                    }
                    if (hat != null)
                    {
                        switch (portList[i])
                        {
                            case PortShowType.None:
                                hat.backColor = XmlUI.DxUIBackColor;
                                break;
                            case PortShowType.Used:
                                hat.backColor = XmlUI.DxUsedColor;
                                break;
                            case PortShowType.Apply:
                                hat.backColor = XmlUI.DxApplyColor;
                                break;
                            case PortShowType.Error:
                                hat.backColor = XmlUI.DxWarningColor;
                                break;
                            case PortShowType.UsedError:
                                hat.backColor = XmlUI.DxSelectColor;
                                break;
                            default:
                                WarningForm.Instance.OpenUI("SetHatHide PortShowType Error !!!", false);
                                break;
                        }
                    }
                }
            }
        }
        private void SetColorInfoMode(uiSwitchButton _none, uiSwitchButton _rainbow, uiSwitchButton _dynamic, uiSwitchButton _wave, uiSwitchButton _custom, uiSwitchButton _click, ColorInfoType type)
        {
            switch (type)
            {
                case ColorInfoType.ColorNone:
                    _none.bSwitchOn = true;
                    _rainbow.bSwitchOn = false;
                    _dynamic.bSwitchOn = false;
                    _wave.bSwitchOn = false;
                    _custom.bSwitchOn = false;
                    _click.bSwitchOn = false;
                    break;
                case ColorInfoType.ColorRainbow:
                    _none.bSwitchOn = false;
                    _rainbow.bSwitchOn = true;
                    _dynamic.bSwitchOn = false;
                    _wave.bSwitchOn = false;
                    _custom.bSwitchOn = false;
                    _click.bSwitchOn = false;
                    break;
                case ColorInfoType.ColorDynamicRainbow:
                    _none.bSwitchOn = false;
                    _rainbow.bSwitchOn = false;
                    _dynamic.bSwitchOn = true;
                    _wave.bSwitchOn = false;
                    _custom.bSwitchOn = false;
                    _click.bSwitchOn = false;
                    break;
                case ColorInfoType.ColorRGBWave:
                    _none.bSwitchOn = false;
                    _rainbow.bSwitchOn = false;
                    _dynamic.bSwitchOn = false;
                    _wave.bSwitchOn = true;
                    _custom.bSwitchOn = false;
                    _click.bSwitchOn = false;
                    break;
                case ColorInfoType.ColorCustom:
                    _none.bSwitchOn = false;
                    _rainbow.bSwitchOn = false;
                    _dynamic.bSwitchOn = false;
                    _wave.bSwitchOn = false;
                    _custom.bSwitchOn = true;
                    _click.bSwitchOn = false;
                    break;
                case ColorInfoType.ColorClick:
                    _none.bSwitchOn = false;
                    _rainbow.bSwitchOn = false;
                    _dynamic.bSwitchOn = false;
                    _wave.bSwitchOn = false;
                    _custom.bSwitchOn = false;
                    _click.bSwitchOn = true;
                    break;
            }
        }
        private float SetLedUIHide(LedUItype type)
        {
            ledControl.Hide = false;
            ledSwitchControl.Hide = false;
            ledControlTypeAControl.Hide = false;
            ledControlTypeBControl.Hide = false;
            ledOrderControl.Hide = false;
            ledLightControl.Hide = false;
            ledLinkControl.Hide = false;
            ledMatrixControl.Hide = false;
            switch (type)
            {
                case LedUItype.AllHide:
                    ledControl.Hide = true;
                    break;
                case LedUItype.NoneDevice:
                    ledSwitchControl.Hide = true;
                    ledLinkControl.Hide = true;
                    return 16f;
                case LedUItype.LedClose:
                    ledOrderControl.Hide = true;
                    ledLightControl.Hide = true;
                    ledLinkControl.Hide = true;
                    ledMatrixControl.Hide = true;
                    ledControlTypeAControl.Hide = true;
                    ledControlTypeBControl.Hide = true;
                    return 2f;
                case LedUItype.LedLink:
                    ledLinkControl.Hide = true;
                    return 17f;
                case LedUItype.LedCount:
                    return 18f;
                case LedUItype.OnlyBrightness:
                    ledSwitchControl.Hide = true;
                    ledOrderControl.Hide = true;
                    ledLinkControl.Hide = true;
                    ledMatrixControl.Hide = true;
                    ledControlTypeAControl.Hide = true;
                    ledControlTypeBControl.Hide = true;
                    return 2f;
            }
            return 0f;
        }
        public void SetLedHide()
        {
            float ledAspectRatio = SetLedUIHide(LedUItype.AllHide);
            if (currentObj != null && currentDev != null)
            {
                if (currentDev.Type == DeviceType.None)
                {
                    ledAspectRatio = SetLedUIHide(LedUItype.NoneDevice);
                }
                else
                {
                    switch (currentDev.portLedType)
                    {
                        case LedPortType.Led:
                            if (currentDev.outLed)
                                ledAspectRatio = SetLedUIHide(LedUItype.LedLink);
                            else
                                ledAspectRatio = SetLedUIHide(LedUItype.LedClose);
                            break;
                        case LedPortType.Brightness:
                            if (currentDev.outLed)
                                ledAspectRatio = SetLedUIHide(LedUItype.LedCount);
                            else
                                ledAspectRatio = SetLedUIHide(LedUItype.LedClose);
                            break;
                        case LedPortType.SoftBrightness:
                            ledAspectRatio = SetLedUIHide(LedUItype.OnlyBrightness);
                            break;
                    }
                }
                ledControl.AspectRatio = ledAspectRatio / 22f;
                sbLedOn.bSwitchOn = currentDev.outLed;
                PublicData.SetButtonList("LedOrder", currentObj.GetLedOrder());
                PublicData.SetButtonList("LedButton", currentObj.GetLedUsed());
                PublicData.SetButtonColor("LedButton", currentObj.GetLedBrightnessUsed());
                PublicData.SetPanelColor("LedColor", currentObj.GetLedColor());
                PublicData.SetPanelColor("OpenColor", currentObj.GetLedOpenColor());
                PublicData.SetPanelSwitch("AlwaysOnOpen", currentObj.GetLedOpen());
                if (backLightTB.Value != currentObj.BackLightBrightness)
                    backLightTB.Value = currentObj.BackLightBrightness;
                if (ledLinkTB.Value != currentDev.ledCount)
                    ledLinkTB.Value = currentDev.ledCount;
                Led led = currentObj.GetLed(currentObj.SelectLed);
                if (led != null)
                {
                    switch (led.ControlType)
                    {
                        case LedControlType.LedNone:
                            sbNoneType.bSwitchOn = true;
                            sbAlwaysType.bSwitchOn = false;
                            sbControlType.bSwitchOn = false;
                            sbFnType.bSwitchOn = false;
                            sbCapsType.bSwitchOn = false;
                            sbNumType.bSwitchOn = false;
                            break;
                        case LedControlType.LedAlways:
                            sbNoneType.bSwitchOn = false;
                            sbAlwaysType.bSwitchOn = true;
                            sbControlType.bSwitchOn = false;
                            sbFnType.bSwitchOn = false;
                            sbCapsType.bSwitchOn = false;
                            sbNumType.bSwitchOn = false;
                            break;
                        case LedControlType.LedControl:
                            sbNoneType.bSwitchOn = false;
                            sbAlwaysType.bSwitchOn = false;
                            sbControlType.bSwitchOn = true;
                            sbFnType.bSwitchOn = false;
                            sbCapsType.bSwitchOn = false;
                            sbNumType.bSwitchOn = false;
                            break;
                        case LedControlType.LedFN:
                            sbNoneType.bSwitchOn = false;
                            sbAlwaysType.bSwitchOn = false;
                            sbControlType.bSwitchOn = false;
                            sbFnType.bSwitchOn = true;
                            sbCapsType.bSwitchOn = false;
                            sbNumType.bSwitchOn = false;
                            break;
                        case LedControlType.LedCapsLock:
                            sbNoneType.bSwitchOn = false;
                            sbAlwaysType.bSwitchOn = false;
                            sbControlType.bSwitchOn = false;
                            sbFnType.bSwitchOn = false;
                            sbCapsType.bSwitchOn = true;
                            sbNumType.bSwitchOn = false;
                            break;
                        case LedControlType.LedNumLock:
                            sbNoneType.bSwitchOn = false;
                            sbAlwaysType.bSwitchOn = false;
                            sbControlType.bSwitchOn = false;
                            sbFnType.bSwitchOn = false;
                            sbCapsType.bSwitchOn = false;
                            sbNumType.bSwitchOn = true;
                            break;
                    }
                    if (redTB.Value != led.R)
                        redTB.Value = led.R;
                    if (greenTB.Value != led.G)
                        greenTB.Value = led.G;
                    if (blueTB.Value != led.B)
                        blueTB.Value = led.B;
                    //Open
                    if (openRedTB.Value != led.OpenR)
                        openRedTB.Value = led.OpenR;
                    if (openGreenTB.Value != led.OpenG)
                        openGreenTB.Value = led.OpenG;
                    if (openBlueTB.Value != led.OpenB)
                        openBlueTB.Value = led.OpenB;
                }
                if (currentDev.outLed)
                {
                    if (currentObj.HardwareVersion == "KB")
                    {
                        sbControlType.Name = "LedAlwaysOff";
                        colorInfoControl.Hide = false;
                        SetColorInfoMode(sbStandbyModeNone, sbStandbyModeRainbow, sbStandbyModeDynamic, sbStandbyModeRGBWave, sbStandbyModeCustom, sbStandbyModeClick, currentObj.IdleColor);
                        ColorInfo info = currentObj.GetColorInfo(currentObj.SelectLed);
                        SetColorInfoMode(sbClickModeNone, sbClickModeRainbow, sbClickModeDynamic, sbClickModeRGBWave, sbClickModeCustom, sbClickModeClick, info.ClickMode);
                        SetColorInfoMode(sbFnModeNone, sbFnModeRainbow, sbFnModeDynamic, sbFnModeRGBWave, sbFnModeCustom, sbFnModeClick, info.FnMode);
                        if (colorSpeedTB.Value != currentObj.DynamicSpeed)
                            colorSpeedTB.Value = currentObj.DynamicSpeed;
                        if (colorOffsetTB.Value != info.Offset)
                            colorOffsetTB.Value = info.Offset;
                        colorOffsetTE.Text = info.Offset.ToString();
                    }
                    else
                    {
                        sbControlType.Name = "LedControlSwitch";
                    }
                }
                else
                {
                    colorInfoControl.Hide = true;
                }
            }
        }
        public void SetPwnHide(bool _pwm)
        {
            pwmControl.Hide = _pwm;
            if (currentObj != null && currentDev != null)
            {
                PublicData.SetButtonList("PwmButton", currentObj.GetOutputUsed(JoyConst.MaxPWM, OutPortType.PWM));
                PublicData.SetButtonEnable("PwmButton", currentObj.pwmUsed);
                sbPwmReversal.bSwitchOn = currentDev.outInversion;
            }
        }
        public void SetSoftDataHide(bool _softData)
        {
            softDataControl.Hide = _softData;
            if (currentObj != null && currentDev != null)
            {
                PublicData.SetButtonList("SoftDataButton", currentObj.GetDataOutUsed());
            }
        }
        #endregion
        private void SetAllHide()
        {
            SetPinHide(true);
            SetBandHide(true);
            SetAdcHide(true);
            SetHellHide(true);
            SetButtonHide(true);
            SetEncodeTypeHide(true);
            SetPulseHide(true);
            SetMouseSpeedHide(true);
            SetPercentageHide(true);
            SetFormatHide(true, true);
            SetAxisHide(true);
            SetHatHide(true);
            SetLedHide();
            SetPwnHide(true);
            SetSoftDataHide(true);
            if (currentObj != null)
            {
                SetPowerHide(!currentObj.NeedPowerControl);
                PublicData.SetButtonSelect("FormatButton", currentObj.SelectFormatIn, JoyConst.MaxFormat);
                if (currentDev != null && currentDev.outType == OutputType.Keyboard)
                {
                    PublicData.SetButtonSelect("ButtonButton", currentObj.SelectButton, JoyConst.MaxButton);
                }
                else
                {
                    PublicData.SetButtonSelect("ButtonButton", -1, currentObj.SelectButton);
                }
                PublicData.SetButtonSelect("AxisButton", currentObj.SelectAxis, JoyConst.MaxAxis);
                PublicData.SetButtonSelect("LedButton", currentObj.SelectLed, JoyConst.MaxLed);
            }
        }
        public void DxRenderLogic()
        {
            currentObj = PublicData.GetCurrentSelectJoyObject();
            #region 处理数据显示
            if (currentObj == null)
            {
                SetAllHide();
            }
            else
            {
                #region device
                currentDev = currentObj.GetCurrentJoyDevice();
                if (currentDev == null)
                {
                    SetAllHide();
                }
                else
                {
                    SetAllHide();
                    switch (currentDev.Type)
                    {
                        case DeviceType.None:
                            break;
                        //=========================================
                        case DeviceType.SB_Normal: //按钮输出------------------------------------------------
                        case DeviceType.SB_Lock: //带锁按钮--------------------------------------------------
                        case DeviceType.SB_RKJX://RKJX系列8方向按下---------------------------------------
                            SetPinHide(false);
                            SetButtonHide(false);
                            break;
                        case DeviceType.SB_OnPulse: //开脉冲-------------------------------------------------
                        case DeviceType.SB_OffPulse: //关脉冲------------------------------------------------
                        case DeviceType.SB_AllPulse: //全脉冲------------------------------------------------
                        case DeviceType.SB_Turbo: //连击-----------------------------------------------------
                            SetPinHide(false);
                            SetButtonHide(false);
                            SetPulseHide(false);
                            break;
                        case DeviceType.SB_Soft: //按钮输入--------------------------------------------------
                        case DeviceType.SB_ModeClick://按钮模式触发----------------------------------------
                            SetPinHide(false);
                            SetSoftDataHide(false);
                            break;
                        case DeviceType.SB_SoftSwitch: //按钮切换输入----------------------------------------
                        case DeviceType.SB_ModeSwitch: //按钮模式切换----------------------------------------
                            SetPinHide(false);
                            SetSoftDataHide(false);
                            SetBandHide(false);
                            break;
                        case DeviceType.SB_MultiMode: //按钮多模式-------------------------------------------
                            SetPinHide(false);
                            SetBandHide(false);
                            SetButtonHide(false);
                            break;
                        case DeviceType.SB_CombinedAxisSwitch://轴融合比例开关-----------------------------
                        case DeviceType.SB_MouseLeft: //鼠标左键---------------------------------------------
                        case DeviceType.SB_MouseRight: //鼠标右键--------------------------------------------
                        case DeviceType.SB_MouseMiddle: //鼠标中键-------------------------------------------
                            SetPinHide(false);
                            break;
                        //====================================================================
                        case DeviceType.MB_Switch2: //2档钮子开关--------------------------------------------
                            SetPinHide(false);
                            SetButtonHide(false);
                            break;
                        case DeviceType.MB_Switch2_Pulse: //2档钮子脉冲开关----------------------------------
                            SetPinHide(false);
                            SetButtonHide(false);
                            SetPulseHide(false);
                            break;
                        case DeviceType.MB_SoftSwitch2: //2档钮子开关输入------------------------------------
                        case DeviceType.MB_Switch2ModeSwitch: //2档钮子开关-------------------------------
                            SetPinHide(false);
                            SetSoftDataHide(false);
                            break;
                        case DeviceType.MB_Switch3: //3档钮子开关--------------------------------------------
                            SetPinHide(false);
                            SetButtonHide(false);
                            break;
                        case DeviceType.MB_Switch3_Pulse: //3档钮子脉冲开关----------------------------------
                            SetPinHide(false);
                            SetButtonHide(false);
                            SetPulseHide(false);
                            break;
                        case DeviceType.MB_SoftSwitch3: //3档钮子开关输入------------------------------------
                        case DeviceType.MB_Switch3ModeSwitch: //3档钮子开关-------------------------------
                            SetPinHide(false);
                            SetSoftDataHide(false);
                            break;
                        case DeviceType.MB_Band: //波段开关--------------------------------------------------
                            SetPinHide(false);
                            SetBandHide(false);
                            SetButtonHide(false);
                            break;
                        case DeviceType.MB_Band_Pulse: //波段脉冲开关----------------------------------------
                            SetPinHide(false);
                            SetBandHide(false);
                            SetButtonHide(false);
                            SetPulseHide(false);
                            break;
                        case DeviceType.MB_SoftBand: //波段开关输入------------------------------------------
                            SetPinHide(false);
                            SetSoftDataHide(false);
                            SetBandHide(false);
                            break;
                        case DeviceType.MB_BandModeSwitch: //波段模式切换------------------------------------
                            SetPinHide(false);
                            SetSoftDataHide(false);
                            SetBandHide(false);
                            break;
                        case DeviceType.MB_Encode: //编码器--------------------------------------------------
                            SetPinHide(false);
                            SetButtonHide(false);
                            SetEncodeTypeHide(false);
                            SetPulseHide(false);
                            break;
                        case DeviceType.MB_SoftEncode: //编码器输入------------------------------------------
                            SetPinHide(false);
                            SetEncodeTypeHide(false);
                            SetSoftDataHide(false);
                            break;
                        case DeviceType.MB_MultiModeEncode: //编码器多模式-----------------------------------
                            SetPinHide(false);
                            SetBandHide(false);
                            SetButtonHide(false);
                            SetEncodeTypeHide(false);
                            SetPulseHide(false);
                            break;
                        case DeviceType.MB_EncodeBand: //编码转波段开关--------------------------------------
                            SetPinHide(false);
                            SetBandHide(false);
                            SetButtonHide(false);
                            SetEncodeTypeHide(false);
                            break;
                        case DeviceType.MB_EncodeBand_Pulse: //编码转波段脉冲开关----------------------------
                            SetPinHide(false);
                            SetBandHide(false);
                            SetButtonHide(false);
                            SetEncodeTypeHide(false);
                            SetPulseHide(false);
                            break;
                        case DeviceType.MB_EncodeMouseX: //编码转鼠标横向------------------------------------
                        case DeviceType.MB_EncodeMouseY: //编码转鼠标纵向------------------------------------
                        case DeviceType.MB_EncodeMouseWheel: //编码转鼠标滚轮--------------------------------
                            SetPinHide(false);
                            SetEncodeTypeHide(false);
                            SetMouseSpeedHide(false);
                            break;
                        case DeviceType.MB_Hat: //苦力帽-----------------------------------------------------
                            SetPinHide(false);
                            SetHatHide(false);
                            break;
                        case DeviceType.MB_EncodeCombinedAxis://编码器调节轴融合比例------------------------
                            SetPinHide(false);
                            SetEncodeTypeHide(false);
                            break;
                        //====================================================================
                        case DeviceType.A_ADC: //模数转换轴数据----------------------------------------------
                            SetAdcHide(false);
                            SetFormatHide(false, false);
                            break;
                        case DeviceType.H_TLE5010: //TLE5010轴数据-------------------------------------------
                        case DeviceType.H_MLX90316: //MLX90316轴数据-----------------------------------------
                        case DeviceType.H_MLX90333: //MLX90333轴数据-----------------------------------------
                        case DeviceType.H_MLX90363: //MLX90363轴数据-----------------------------------------
                        case DeviceType.H_MLX90393: //MLX90393轴数据-----------------------------------------
                        case DeviceType.H_N35P112: //N35P112轴数据-------------------------------------------
                        case DeviceType.H_HX711://HX177轴数据--------------------------------------------------
                        case DeviceType.H_HX717://HX717轴数据--------------------------------------------------
                            SetHellHide(false);
                            SetFormatHide(false, false);
                            break;
                        //====================================================================
                        case DeviceType.F_Normal: //轴输出---------------------------------------------------
                        case DeviceType.F_CombinedAxis://融合轴---------------------------------------------
                            SetFormatHide(false, true);
                            SetAxisHide(false);
                            break;
                        case DeviceType.F_ButtonMin: //轴带按键----------------------------------------------
                        case DeviceType.F_ButtonMax: //轴带按键----------------------------------------------
                            SetFormatHide(false, true);
                            SetButtonHide(false);
                            SetPercentageHide(false);
                            break;
                        case DeviceType.F_LedBrightness: //轴背光控制----------------------------------------
                            SetFormatHide(false, true);
                            SetLedHide();
                            break;
                        case DeviceType.F_Band: //轴转波段开关-----------------------------------------------
                            SetFormatHide(false, true);
                            SetBandHide(false);
                            SetButtonHide(false);
                            break;
                        case DeviceType.F_Trigger: //轴转多段扳机--------------------------------------------
                            SetFormatHide(false, true);
                            SetBandHide(false);
                            SetButtonHide(false);
                            break;
                        case DeviceType.F_Hat: //轴转苦力帽--------------------------------------------------
                            SetFormatHide(false, true);
                            SetHatHide(false);
                            break;
                        case DeviceType.F_Soft: //轴输入-----------------------------------------------------
                            SetFormatHide(false, true);
                            SetSoftDataHide(false);
                            break;
                        case DeviceType.F_MouseX: //轴转鼠标横向---------------------------------------------
                            SetFormatHide(false, true);
                            SetMouseSpeedHide(false);
                            break;
                        case DeviceType.F_MouseY: //轴转鼠标纵向---------------------------------------------
                            SetFormatHide(false, true);
                            SetMouseSpeedHide(false);
                            break;
                        case DeviceType.F_PWM: //占空比------------------------------------------------------
                            SetFormatHide(false, true);
                            SetPwnHide(false);
                            break;
                        case DeviceType.Brightness_PWM: //软件背光占空比-------------------------------------------
                        case DeviceType.Soft_PWM: //软件数据占空比-------------------------------------------
                            SetPwnHide(false);
                            break;
                        case DeviceType.Soft_Button:
                            SetButtonHide(false);
                            break;
                        case DeviceType.Soft_Axis:
                            SetAxisHide(false);
                            break;
                        case DeviceType.Led_Only:
                            SetLedHide();
                            break;
                        default:
                            currentDev.Type = DeviceType.None;
                            WarningForm.Instance.OpenUI("InputControlPanel - DxRenderLogic - dev.Type : Error !!!", false);
                            break;
                    }
                }
                #endregion
                #region format
                axisDC.inFormatMax = currentObj.inFormatMax;
                axisDC.outFormatMax = currentObj.outFormatMax;
                axisDC.AddData(currentObj.inFormatValue, currentObj.outFormatValue);
                #endregion
            }
            #endregion
        }
    }
}
