using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace EasyControl
{
    public class ReportManager
    {
        public JoyObject obj { get; private set; } = null;
        public const int reportID = 3;
        public const int checkID = 2;
        List<Report> reportList = new List<Report>();
        object lockObj = new object();
        bool syncUIType = false;
        //------------------------------------------------------------------
        public ReportManager(JoyObject _obj)
        {
            obj = _obj ?? throw new Win32Exception("New ReportManager Error !!!");
        }

        public void ReSet()
        {
            lock (lockObj)
                reportList.Clear();
            obj.SetLinkMode(LinkMode.Error);
        }

        public bool AddReport(Report report)
        {
            lock (lockObj)
            {
                if (report.Type == ReportType.ReBoot || report.Type == ReportType.Update)
                {
                    reportList.Clear();
                }
                reportList.Add(report);
            }
            return true;
        }

        public int GetCount()
        {
            return reportList.Count;
        }
        public bool InCommunication()
        {
            lock (lockObj)
                if (reportList.Count > 0)
                {
                    for (int i = 0; i < reportList.Count; i++)
                    {
                        if (reportList[i].Type != ReportType.CustomSync &&
                            reportList[i].Type != ReportType.DeviceSync &&
                            reportList[i].Type != ReportType.SyncProperty)
                        {
                            return true;
                        }
                    }
                }
            return false;
        }
        public Report GetReport()
        {
            if (reportList.Count > 0)
            {
                return reportList[0];
            }
            if (obj.Ready)
            {
                if (PublicData.ui_Type == UIType.NodeLink)
                {
                    if (!syncUIType)
                    {
                        syncUIType = true;
                        Report newSyncProperty = new Report(ReportType.SyncProperty);
                        lock (lockObj)
                            reportList.Add(newSyncProperty);
                    }
                    Report newReport = new Report(ReportType.CustomSync);
                    lock (lockObj)
                        reportList.Add(newReport);
                }
                else
                {
                    syncUIType = false;
                    Report newSyncProperty = new Report(ReportType.SyncProperty);
                    lock (lockObj)
                        reportList.Add(newSyncProperty);
                    Report newCustomData = new Report(ReportType.CustomSync);
                    lock (lockObj)
                        reportList.Add(newCustomData);
                }
            }
            else
            {
                Report newSyncProperty = new Report(ReportType.LinkTest);
                lock (lockObj)
                    reportList.Add(newSyncProperty);
            }
            return reportList[0];
        }
        public bool CheckReport(JoyObject obj, byte[] report, out string message)
        {
            obj.SetLinkMode(LinkMode.OnLine);
            #region 列表为空
            if (reportList.Count <= 0)
            {
                message = "报告列表为空";
                return true;
            }
            #endregion
            if (report[0] != checkID)
            {
                message = "数据端口错误！";
                return false;
            }
            Report usbReport = new Report(report);
            if (report[1] == JoyConst.ProtocolVer1 &&
                report[2] == JoyConst.ProtocolVer2)
            {
                if (usbReport.Type == reportList[0].Type &&                //类型匹配
                usbReport.Index == reportList[0].Index &&               //序号正确
                usbReport.maxIndex == reportList[0].maxIndex &&        //序号长度正确
                usbReport.ECC == reportList[0].ECC)                               //校验正确
                {
                    if (usbReport.Successful)                                                      //数据处理正确
                    {
                        #region 数据验证
                        //do it
                        switch (reportList[0].Type)
                        {
                            case ReportType.LinkTest:
                                #region 删除到连线图中
                                NodeLinkControl.Instance.DeletePluginNode(obj);
                                #endregion
                                break;
                            case ReportType.ReBoot:
                                //nothing
                                break;
                            case ReportType.Update:
                                //nothing
                                break;
                            case ReportType.LicenseKey:
                                #region LicenseKey
                                obj.SetLicenseUID(usbReport.data[0] == 1 ? true : false);
                                if (obj.LicenseUID)
                                {
                                    WarningForm.Instance.OpenUI("LicenseSuccess");
                                }
                                else
                                {
                                    WarningForm.Instance.OpenUI("LicenseFail");
                                }
                                #endregion
                                break;
                            case ReportType.LicenseInfo:
                                #region LicenseInfo
                                obj.version1 = usbReport.data[1];
                                obj.version2 = usbReport.data[2];
                                obj.version3 = usbReport.data[3];
                                byte[] mcuIDbyte = new byte[12];
                                for (int i = 0; i < 12; i++)
                                {
                                    mcuIDbyte[i] = usbReport.data[4 + i];
                                }
                                obj.SetMcuID(PublicData.ByteArray2String(mcuIDbyte, false));
                                byte[] hardwareVersion = new byte[2];
                                hardwareVersion[0] = usbReport.data[16];
                                hardwareVersion[1] = usbReport.data[17];
                                obj.SetHardwareVersion(System.Text.Encoding.Default.GetString(hardwareVersion, 0, 2));
                                //放到最后，用于自动注册（需要前置信息）
                                obj.SetLicenseUID(usbReport.data[0] == 1 ? true : false);
                                #endregion
                                break;
                            case ReportType.DeviceData:
                                //nothing
                                break;
                            case ReportType.DeviceInfo:
                                #region DriverInfo
                                if (usbReport.Index >= 0 && usbReport.Index < JoyConst.MaxDevice / JoyConst.DeviceInfoCount)
                                {
                                    int reportSpace = 13;
                                    for (int i = 0; i < JoyConst.DeviceInfoCount; i++)
                                    {
                                        JoyDevice dev = obj.GetJoyDevice(usbReport.Index * JoyConst.DeviceInfoCount + i);
                                        if (dev != null)
                                        {
                                            dev.SyncData(usbReport, reportSpace * i);
                                        }
                                    }
                                }
                                #endregion
                                break;
                            case ReportType.CustomData:
                                //nothing
                                break;
                            case ReportType.CustomInfo:
                                #region CustomInfo
                                if (usbReport.Index >= 0 && usbReport.Index < JoyConst.MaxCustom)
                                {
                                    JoyCustom cus = obj.GetJoyCustom(usbReport.Index);
                                    if (cus != null)
                                    {
                                        cus.SyncData(usbReport);
                                    }
                                }
                                #endregion
                                break;
                            case ReportType.ButtonData:
                                //nothing
                                break;
                            case ReportType.ButtonInfo:
                                #region ButtonInfo
                                if (usbReport.Index >= 0 && usbReport.Index < JoyConst.MaxButton / JoyConst.ButtonInfoCount)
                                {
                                    int reportSpace = 4;
                                    for (int i = 0; i < JoyConst.ButtonInfoCount; i++)
                                    {
                                        Button btn = obj.GetButton(usbReport.Index * JoyConst.ButtonInfoCount + i);
                                        if (btn != null)
                                        {
                                            btn.Fun = usbReport.data[reportSpace * i];
                                            btn.Code = usbReport.data[reportSpace * i + 1];
                                            btn.FunFN = usbReport.data[reportSpace * i + 2];
                                            btn.CodeFN = usbReport.data[reportSpace * i + 3];
                                        }
                                    }
                                }
                                #endregion
                                break;
                            case ReportType.HatData:
                                //nothing
                                break;
                            case ReportType.HatInfo:
                                #region HatInfo
                                if (usbReport.Index >= 0 && usbReport.Index < JoyConst.MaxHat / JoyConst.HatInfoCount)
                                {
                                    int reportSpace = 4;
                                    for (int i = 0; i < JoyConst.MaxHat; i++)
                                    {
                                        Hat hat = obj.GetHat(usbReport.Index * JoyConst.MaxHat + i);
                                        if (hat != null)
                                        {
                                            hat.InputIndex[0] = usbReport.data[reportSpace * i];
                                            hat.InputIndex[1] = usbReport.data[reportSpace * i + 1];
                                            hat.InputIndex[2] = usbReport.data[reportSpace * i + 2];
                                            hat.InputIndex[3] = usbReport.data[reportSpace * i + 3];
                                        }
                                    }
                                }
                                #endregion
                                break;
                            case ReportType.AdcData:
                                //nothing
                                break;
                            case ReportType.AdcInfo:
                                #region AdcInfo
                                if (usbReport.Index >= 0 && usbReport.Index < JoyConst.MaxADC / JoyConst.AdcInfoCount)
                                {
                                    int reportSpace = 3;
                                    for (int i = 0; i < JoyConst.AdcInfoCount; i++)
                                    {
                                        Adc adc = obj.GetAdc(usbReport.Index * JoyConst.AdcInfoCount + i);
                                        if (adc != null)
                                        {
                                            adc.maxCC = usbReport.data[reportSpace * i];
                                            adc.maxCY = usbReport.data[reportSpace * i + 1];
                                            adc.maxPC = usbReport.data[reportSpace * i + 2];
                                        }
                                    }
                                }
                                #endregion
                                break;
                            case ReportType.FormatData:
                                //nothing
                                break;
                            case ReportType.FormatInfo:
                                #region FormatInfo
                                if (usbReport.Index >= 0 && usbReport.Index < JoyConst.MaxFormat / JoyConst.FormatInfoCount)
                                {
                                    int reportSpace = 13;
                                    for (int i = 0; i < JoyConst.FormatInfoCount; i++)
                                    {
                                        Format format = obj.GetFormat(usbReport.Index * JoyConst.FormatInfoCount + i);
                                        if (format != null)
                                        {
                                            format.Reverse = usbReport.data[reportSpace * i] == 0 ? false : true;
                                            format.Calibration = usbReport.data[reportSpace * i + 1] == 0 ? false : true;
                                            format.AutoRange = usbReport.data[reportSpace * i + 2] == 0 ? false : true;
                                            format.Shift = usbReport.data[reportSpace * i + 3];
                                            format.minDzone = usbReport.data[reportSpace * i + 4];
                                            format.midDzone = usbReport.data[reportSpace * i + 5];
                                            format.maxDzone = usbReport.data[reportSpace * i + 6];
                                            format.minValue = (usbReport.data[reportSpace * i + 7] << 8) + usbReport.data[reportSpace * i + 8];
                                            format.midValue = (usbReport.data[reportSpace * i + 9] << 8) + usbReport.data[reportSpace * i + 10];
                                            format.maxValue = (usbReport.data[reportSpace * i + 11] << 8) + usbReport.data[reportSpace * i + 12];
                                        }
                                    }
                                }
                                #endregion
                                break;
                            case ReportType.LedData:
                                //nothing
                                break;
                            case ReportType.LedInfo:
                                #region LedInfo
                                if (usbReport.Index >= 0 && usbReport.Index < JoyConst.MaxLed / JoyConst.LedInfoCount)
                                {
                                    int reportSpace;
                                    switch (obj.HardwareVersion)
                                    {
                                        case "KB":
                                            reportSpace = 11;
                                            for (int i = 0; i < JoyConst.LedInfoCount; i++)
                                            {
                                                Led led = obj.GetLed(usbReport.Index * JoyConst.LedInfoCount + i);
                                                ColorInfo color = obj.GetColorInfo(usbReport.Index * JoyConst.LedInfoCount + i);
                                                if (led != null && color != null)
                                                {
                                                    //led.Always = usbReport.data[reportSpace * i] == 0 ? false : true;
                                                    led.ControlType = (LedControlType)usbReport.data[reportSpace * i + 1];
                                                    led.R = usbReport.data[reportSpace * i + 2];
                                                    led.G = usbReport.data[reportSpace * i + 3];
                                                    led.B = usbReport.data[reportSpace * i + 4];
                                                    led.OpenR = usbReport.data[reportSpace * i + 5];
                                                    led.OpenG = usbReport.data[reportSpace * i + 6];
                                                    led.OpenB = usbReport.data[reportSpace * i + 7];
                                                    //----
                                                    color.ClickMode = (ColorInfoType)usbReport.data[reportSpace * i + 8];
                                                    color.FnMode = (ColorInfoType)usbReport.data[reportSpace * i + 9];
                                                    color.Offset = (UInt16)((usbReport.data[reportSpace * i + 10] << 8) + usbReport.data[reportSpace * i + 11]);
                                                }
                                            }
                                            break;
                                        default:
                                            reportSpace = 8;
                                            for (int i = 0; i < JoyConst.LedInfoCount; i++)
                                            {
                                                Led led = obj.GetLed(usbReport.Index * JoyConst.LedInfoCount + i);
                                                if (led != null)
                                                {
                                                    //led.Always = usbReport.data[reportSpace * i] == 0 ? false : true;
                                                    led.ControlType = (LedControlType)usbReport.data[reportSpace * i + 1];
                                                    led.R = usbReport.data[reportSpace * i + 2];
                                                    led.G = usbReport.data[reportSpace * i + 3];
                                                    led.B = usbReport.data[reportSpace * i + 4];
                                                    led.OpenR = usbReport.data[reportSpace * i + 5];
                                                    led.OpenG = usbReport.data[reportSpace * i + 6];
                                                    led.OpenB = usbReport.data[reportSpace * i + 7];
                                                }
                                            }
                                            break;
                                    }
                                }
                                #endregion
                                break;
                            case ReportType.SaveUsbData:
                                #region LicenseKey
                                obj.SetLicenseUID(usbReport.data[0] == 1 ? true : false);
                                if (obj.LicenseUID)
                                {
                                    WarningForm.Instance.OpenUI("SaveSuccess");
                                }
                                else
                                {
                                    WarningForm.Instance.OpenUI("SaveFail");
                                }
                                #endregion
                                break;
                            case ReportType.GetUsbInfo:
                                #region GetUsbInfo
                                byte[] nameList = new byte[JoyConst.MaxUsbName];
                                int maxIndex = 0;
                                for (int i = 0; i < JoyConst.MaxUsbName; i++)
                                {
                                    if (usbReport.data[i] != System.Convert.ToByte('\0'))
                                    {
                                        nameList[i] = usbReport.data[i];
                                    }
                                    else
                                    {
                                        maxIndex = i;
                                        break;
                                    }
                                }
                                obj.SetUsbName(System.Text.Encoding.Default.GetString(nameList, 0, maxIndex));
                                obj.SetMaxOutJoystick(usbReport.data[JoyConst.MaxUsbName]);
                                obj.SetMaxOutAxis(usbReport.data[JoyConst.MaxUsbName + 1]);
                                obj.SetMaxOutHat(usbReport.data[JoyConst.MaxUsbName + 2]);
                                obj.SetVID((ushort)(usbReport.data[JoyConst.MaxUsbName + 3] + (usbReport.data[JoyConst.MaxUsbName + 4] << 8)));
                                obj.SetPID((ushort)(usbReport.data[JoyConst.MaxUsbName + 5] + (usbReport.data[JoyConst.MaxUsbName + 6] << 8)));
                                obj.SetColorOrder(usbReport.data[JoyConst.MaxUsbName + 7]);
                                obj.SetBackLightBrightness(usbReport.data[JoyConst.MaxUsbName + 8]);
                                obj.SetHC165(usbReport.data[JoyConst.MaxUsbName + 9] != 0);
                                obj.axisID[0] = usbReport.data[JoyConst.MaxUsbName + 10];
                                obj.axisID[1] = usbReport.data[JoyConst.MaxUsbName + 11];
                                obj.axisID[2] = usbReport.data[JoyConst.MaxUsbName + 12];
                                obj.axisID[3] = usbReport.data[JoyConst.MaxUsbName + 13];
                                obj.axisID[4] = usbReport.data[JoyConst.MaxUsbName + 14];
                                obj.axisID[5] = usbReport.data[JoyConst.MaxUsbName + 15];
                                obj.axisID[6] = usbReport.data[JoyConst.MaxUsbName + 16];
                                obj.axisID[7] = usbReport.data[JoyConst.MaxUsbName + 17];
                                obj.joyMaxADC = usbReport.data[JoyConst.MaxUsbName + 18];
                                obj.joyMaxHall = usbReport.data[JoyConst.MaxUsbName + 19];
                                obj.joyMaxPWM = usbReport.data[JoyConst.MaxUsbName + 20];
                                obj.joyMaxPin = usbReport.data[JoyConst.MaxUsbName + 21];
                                obj.SetUsbPower(usbReport.data[JoyConst.MaxUsbName + 22] != 0);
                                //KB
                                obj.SetIdleColor((ColorInfoType)usbReport.data[JoyConst.MaxUsbName + 22]);
                                obj.SetDynamicSpeed(usbReport.data[JoyConst.MaxUsbName + 23]);
                                #endregion
                                #region 添加到连线图中
                                NodeLinkControl.Instance.AddNewPluginNode(obj, false);
                                #endregion
                                #region 设备准备完毕
                                obj.Ready = true;
                                #endregion
                                break;
                            case ReportType.SyncProperty:
                                switch ((TypeSwitch)usbReport.data[0])
                                {
                                    case TypeSwitch.DeviceSwitch:
                                        //pin
                                        int pinBtye = JoyConst.MaxPin / 8;
                                        for (int i = 0; i < pinBtye; i++)
                                        {
                                            obj.pinValue[i] = usbReport.data[i + 1];
                                        }
                                        //format
                                        obj.inFormatValue = usbReport.data[pinBtye + 1] +
                                            (usbReport.data[pinBtye + 2] << 8) +
                                            (usbReport.data[pinBtye + 3] << 16) +
                                            (usbReport.data[pinBtye + 4] << 24);
                                        obj.inFormatMax = usbReport.data[pinBtye + 5] +
                                            (usbReport.data[pinBtye + 6] << 8) +
                                            (usbReport.data[pinBtye + 7] << 16) +
                                            (usbReport.data[pinBtye + 8] << 24);
                                        obj.outFormatValue = usbReport.data[pinBtye + 9] +
                                            (usbReport.data[pinBtye + 10] << 8) +
                                            (usbReport.data[pinBtye + 11] << 16) +
                                            (usbReport.data[pinBtye + 12] << 24);
                                        obj.outFormatMax = usbReport.data[pinBtye + 13] +
                                            (usbReport.data[pinBtye + 14] << 8) +
                                            (usbReport.data[pinBtye + 15] << 16) +
                                            (usbReport.data[pinBtye + 16] << 24);
                                        //hat
                                        obj.SetHatState(0, usbReport.data[pinBtye + 17]);
                                        obj.SetHatState(1, (byte)(usbReport.data[pinBtye + 17] >> 4));
                                        obj.SetHatState(2, usbReport.data[pinBtye + 18]);
                                        obj.SetHatState(3, (byte)(usbReport.data[pinBtye + 18] >> 4));
                                        //
                                        obj.SetColorOrder(usbReport.data[pinBtye + 19]);
                                        obj.SetBackLightBrightness(usbReport.data[pinBtye + 20]);
                                        obj.SetHC165(usbReport.data[pinBtye + 21] != 0);
                                        obj.SetChangePin(usbReport.data[pinBtye + 22]);
                                        obj.SetChangeFormat(usbReport.data[pinBtye + 23]);
                                        //KB
                                        obj.SetIdleColor((ColorInfoType)usbReport.data[pinBtye + 24]);
                                        obj.SetDynamicSpeed(usbReport.data[pinBtye + 25]);
                                        break;
                                    case TypeSwitch.CustomSwitch:
                                        //do it
                                        break;
                                    case TypeSwitch.FontLibrarySwitch:
                                        //do it
                                        break;
                                }
                                break;
                            case ReportType.DeviceSync:
                            case ReportType.CustomSync:
                                for (int i = 0; i < JoyConst.MaxSoftData; i++)
                                {
                                    obj.softDataList[i] = usbReport.data[i];
                                }
                                break;
                            //----------------------------------------------
                            case ReportType.GetFont0:
                                obj.GetSyncFont(JoyConst.MaxSyncFontCount * usbReport.Index, usbReport.data);
                                break;
                            case ReportType.GetFont1:
                                obj.GetSyncFont(JoyConst.MaxSyncFontCount * usbReport.Index + JoyConst.MaxSyncFontCount * 256, usbReport.data);
                                break;
                            case ReportType.GetFont2:
                                obj.GetSyncFont(JoyConst.MaxSyncFontCount * usbReport.Index + JoyConst.MaxSyncFontCount * 256 * 2, usbReport.data);
                                break;
                            case ReportType.GetFont3:
                                obj.GetSyncFont(JoyConst.MaxSyncFontCount * usbReport.Index + JoyConst.MaxSyncFontCount * 256 * 3, usbReport.data);
                                break;
                            case ReportType.GetFont4:
                                obj.GetSyncFont(JoyConst.MaxSyncFontCount * usbReport.Index + JoyConst.MaxSyncFontCount * 256 * 4, usbReport.data);
                                break;
                            case ReportType.GetFont5:
                                obj.GetSyncFont(JoyConst.MaxSyncFontCount * usbReport.Index + JoyConst.MaxSyncFontCount * 256 * 5, usbReport.data);
                                break;
                            case ReportType.GetFont6:
                                obj.GetSyncFont(JoyConst.MaxSyncFontCount * usbReport.Index + JoyConst.MaxSyncFontCount * 256 * 6, usbReport.data);
                                break;
                            case ReportType.GetFont7:
                                obj.GetSyncFont(JoyConst.MaxSyncFontCount * usbReport.Index + JoyConst.MaxSyncFontCount * 256 * 7, usbReport.data);
                                break;
                            case ReportType.GetFontOver:
                                obj.GetSyncFontLib();
                                break;
                            //----------------------------------------------
                            case ReportType.ClearFont://<--
                            case ReportType.SyncFont0:
                            case ReportType.SyncFont1:
                            case ReportType.SyncFont2:
                            case ReportType.SyncFont3:
                            case ReportType.SyncFont4:
                            case ReportType.SyncFont5:
                            case ReportType.SyncFont6:
                            case ReportType.SyncFont7:
                            case ReportType.SaveFont:
                            case ReportType.OledClear://-->
                            case ReportType.KeyBoardSync:
                                //nothing
                                break;
                            default:
                                message = "此类型未处理 ：" + usbReport.Type.ToString();
                                return false;
                        }
                        #endregion
                        if (reportList[0].IsOver())//是否已经完成
                        {
                            lock (lockObj)
                                reportList.RemoveAt(0);
                            message = "\n================▲    " + usbReport.Type.ToString() + " - Over    ▲================";
                        }
                        else
                        {
                            reportList[0].SetNext();    //下一条数据
                            message = "";
                        }
                        return true;
                    }
                    else
                    {
                        message = "数据处理不正确";
                        return false;
                    }
                }
                else
                {
                    message = "数据校验失败";
                    return false;
                }
            }
            else
            {
                message = obj.usbName + " - " + Localization.Instance.GetLS("ErrorVersion");
                if (!obj.errorVersion)
                {
                    string protocol = "";
                    if (report[2] == 0)
                    {
                        protocol += report[1] + "." + report[3];
                    }
                    else
                    {
                        protocol += report[1] + "." + report[2];
                    }
                    UpdateForm.Instance.CheckUpdate(obj, protocol);
                }
                return false;
            }
        }
    }
}
