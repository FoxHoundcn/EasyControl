using System.ComponentModel;

namespace EasyControl
{
    public class Report
    {
        public ReportType Type { get; private set; }
        public byte Index { get; private set; }
        public byte maxIndex { get; private set; }
        public bool Successful { get; private set; }
        public byte[] data { get; private set; }
        public byte ECC { get; private set; }
        public static byte eccIndex = 1;

        public Report(byte[] report)
        {
            if (report.Length != JoyConst.MaxUsbReport)
                throw new Win32Exception("New Report whit UsbReport Error !!!");
            Type = (ReportType)report[3];
            Index = report[4];
            maxIndex = report[5];
            Successful = report[6] != 0 ? true : false;
            data = new byte[JoyConst.MaxUsbData];
            for (int i = 0; i < JoyConst.MaxUsbData; i++)
            {
                data[i] = report[JoyConst.ReportDataStart + i];
            }
            ECC = report[JoyConst.ReportDataStart + JoyConst.MaxUsbData];
        }
        public Report(ReportType _type)
        {
            Type = _type;
            Successful = false;
            data = new byte[JoyConst.MaxUsbData];
            for (byte i = 0; i < JoyConst.MaxUsbData; i++)
            {
                data[i] = 0;
            }
            switch (_type)
            {
                case ReportType.LinkTest:
                case ReportType.ReBoot:
                case ReportType.Update:
                case ReportType.LicenseKey:
                case ReportType.LicenseInfo:
                    Index = 0;
                    maxIndex = 0;
                    break;
                case ReportType.DeviceData:
                case ReportType.DeviceInfo:
                    Index = 0;
                    maxIndex = JoyConst.MaxDevice / JoyConst.DeviceInfoCount - 1;
                    break;
                case ReportType.CustomData:
                case ReportType.CustomInfo:
                    Index = 0;
                    maxIndex = JoyConst.MaxCustom - 1;
                    break;
                case ReportType.ButtonData:
                case ReportType.ButtonInfo:
                    Index = 0;
                    maxIndex = JoyConst.MaxButton / JoyConst.ButtonInfoCount - 1;
                    break;
                case ReportType.HatData:
                case ReportType.HatInfo:
                    Index = 0;
                    maxIndex = JoyConst.MaxHat / JoyConst.HatInfoCount - 1;
                    break;
                case ReportType.AdcData:
                case ReportType.AdcInfo:
                    Index = 0;
                    maxIndex = JoyConst.MaxADC / JoyConst.AdcInfoCount - 1;
                    break;
                case ReportType.FormatData:
                case ReportType.FormatInfo:
                    Index = 0;
                    maxIndex = JoyConst.MaxFormat / JoyConst.FormatInfoCount - 1;
                    break;
                case ReportType.LedData:
                case ReportType.LedInfo:
                    Index = 0;
                    maxIndex = JoyConst.MaxLed / JoyConst.LedInfoCount - 1;
                    break;
                case ReportType.GetFont0:
                case ReportType.GetFont1:
                case ReportType.GetFont2:
                case ReportType.GetFont3:
                case ReportType.GetFont4:
                case ReportType.GetFont5:
                case ReportType.GetFont6:
                case ReportType.GetFont7:
                    Index = 0;
                    maxIndex = 255;
                    break;
                case ReportType.GetFontOver:
                    Index = 0;
                    maxIndex = 0;
                    break;
                //------------------------------------------
                case ReportType.SyncFont0:
                case ReportType.SyncFont1:
                case ReportType.SyncFont2:
                case ReportType.SyncFont3:
                case ReportType.SyncFont4:
                case ReportType.SyncFont5:
                case ReportType.SyncFont6:
                case ReportType.SyncFont7:
                    Index = 0;
                    maxIndex = 255;
                    break;
                case ReportType.SaveUsbData:
                case ReportType.GetUsbInfo:
                case ReportType.SyncProperty:
                case ReportType.DeviceSync:
                case ReportType.CustomSync:
                case ReportType.ClearFont:
                case ReportType.SaveFont:
                case ReportType.OledClear:
                case ReportType.KeyBoardSync:
                    Index = 0;
                    maxIndex = 0;
                    break;
                default:
                    WarningForm.Instance.OpenUI("Error new Report Type !!!" + _type.ToString(), false);
                    break;
            }
            ECC = eccIndex;
            eccIndex++;
            if (eccIndex >= 255)
            {
                eccIndex = 1;
            }
        }

        public bool IsOver()
        {
            if (Index >= maxIndex)
            {
                return true;
            }
            return false;
        }

        public void SetNext()
        {
            if (Index < maxIndex)
                Index++;
        }

        public bool GetReport(JoyObject obj, out byte[] report)
        {
            report = new byte[JoyConst.MaxUsbReport];
            if (obj == null)
                return false;
            report[0] = ReportManager.reportID;
            report[1] = JoyConst.ProtocolVer1;
            report[2] = JoyConst.ProtocolVer2;
            report[3] = (byte)Type;
            report[4] = Index;
            report[5] = maxIndex;
            report[6] = (byte)(Successful ? 1 : 0);//Successful
            #region 数据内容
            switch (Type)
            {
                //do it
                case ReportType.LinkTest:
                    #region KB
                    switch (obj.HardwareVersion)
                    {
                        case "KB":
                            report[JoyConst.ReportDataStart + 1] = (byte)(obj.keyBoardFN ? 1 : 0);
                            report[JoyConst.ReportDataStart + 2] = (byte)(obj.currentColorCount >> 8);
                            report[JoyConst.ReportDataStart + 3] = (byte)(obj.currentColorCount);
                            break;
                    }
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
                    report[JoyConst.ReportDataStart] = obj.licenseKey[0];
                    report[JoyConst.ReportDataStart + 1] = obj.licenseKey[1];
                    report[JoyConst.ReportDataStart + 2] = obj.licenseKey[2];
                    report[JoyConst.ReportDataStart + 3] = obj.licenseKey[3];
                    report[JoyConst.ReportDataStart + 4] = obj.licenseKey[4];
                    report[JoyConst.ReportDataStart + 5] = obj.licenseKey[5];
                    report[JoyConst.ReportDataStart + 6] = obj.licenseKey[6];
                    report[JoyConst.ReportDataStart + 7] = obj.licenseKey[7];
                    report[JoyConst.ReportDataStart + 8] = obj.licenseKey[8];
                    report[JoyConst.ReportDataStart + 9] = obj.licenseKey[9];
                    report[JoyConst.ReportDataStart + 10] = obj.licenseKey[10];
                    report[JoyConst.ReportDataStart + 11] = obj.licenseKey[11];
                    report[JoyConst.ReportDataStart + 12] = obj.licenseKey[12];
                    report[JoyConst.ReportDataStart + 13] = obj.licenseKey[13];
                    report[JoyConst.ReportDataStart + 14] = obj.licenseKey[14];
                    report[JoyConst.ReportDataStart + 15] = obj.licenseKey[15];
                    #endregion
                    break;
                case ReportType.LicenseInfo:
                    //nothing
                    break;
                case ReportType.DeviceData:
                    #region DriverData
                    if (Index >= 0 && Index < JoyConst.MaxDevice / JoyConst.DeviceInfoCount)
                    {
                        int reportSpace = 13;
                        for (int i = 0; i < JoyConst.DeviceInfoCount; i++)
                        {
                            JoyDevice currentDev = obj.GetJoyDevice(Index * JoyConst.DeviceInfoCount + i);
                            if (currentDev != null)
                            {
                                report[JoyConst.ReportDataStart + i * reportSpace] = (byte)currentDev.Type;
                                report[JoyConst.ReportDataStart + 1 + i * reportSpace] = (byte)(currentDev.inInversion ? 1 : 0);
                                report[JoyConst.ReportDataStart + 2 + i * reportSpace] = (byte)currentDev.outType;
                                report[JoyConst.ReportDataStart + 3 + i * reportSpace] = (byte)(currentDev.outInversion ? 1 : 0);
                                report[JoyConst.ReportDataStart + 4 + i * reportSpace] = (byte)(currentDev.outLed ? 1 : 0);
                                report[JoyConst.ReportDataStart + 5 + i * reportSpace] = (byte)currentDev.encodeType;
                                report[JoyConst.ReportDataStart + 6 + i * reportSpace] = currentDev.inPort;
                                report[JoyConst.ReportDataStart + 7 + i * reportSpace] = currentDev.inEnd;
                                report[JoyConst.ReportDataStart + 8 + i * reportSpace] = currentDev.outPort;
                                report[JoyConst.ReportDataStart + 9 + i * reportSpace] = currentDev.outCount;
                                report[JoyConst.ReportDataStart + 10 + i * reportSpace] = currentDev.pulseCount;
                                report[JoyConst.ReportDataStart + 11 + i * reportSpace] = currentDev.ledPort;
                                report[JoyConst.ReportDataStart + 12 + i * reportSpace] = currentDev.ledCount;
                                report[JoyConst.ReportDataStart + 13 + i * reportSpace] = 0xff;
                            }
                        }
                    }
                    #endregion
                    break;
                case ReportType.DeviceInfo:
                    //nothing
                    break;
                case ReportType.CustomData:
                    #region CustomData
                    if (Index >= 0 && Index < JoyConst.MaxCustom)
                    {
                        JoyCustom currentCus = obj.GetJoyCustom(Index);
                        if (currentCus != null)
                        {
                            report[JoyConst.ReportDataStart] = (byte)currentCus.Type;
                            report[JoyConst.ReportDataStart + 1] = (byte)currentCus.rotateType;
                            report[JoyConst.ReportDataStart + 2] = currentCus.data;
                            report[JoyConst.ReportDataStart + 3] = currentCus.cs;
                            report[JoyConst.ReportDataStart + 4] = currentCus.clk;
                            report[JoyConst.ReportDataStart + 5] = currentCus.dc;
                            report[JoyConst.ReportDataStart + 6] = currentCus.rst;
                            report[JoyConst.ReportDataStart + 7] = currentCus.cs2;
                            report[JoyConst.ReportDataStart + 8] = currentCus.cs3;
                            report[JoyConst.ReportDataStart + 9] = currentCus.cs4;
                            report[JoyConst.ReportDataStart + 10] = currentCus.dataStart;
                            report[JoyConst.ReportDataStart + 11] = currentCus.dataCount;
                            switch (currentCus.Type)
                            {
                                case CustomType.NoneCustom:
                                case CustomType.DT_Max7219:
                                case CustomType.DT_TM1638:
                                case CustomType.DT_HT16K33:
                                //--------------------------
                                case CustomType.OUT_StepperMotor:
                                //--------------------------单点输出
                                case CustomType.OUT_74HC595:
                                case CustomType.OUT_IO:
                                //--------------------------无线模块
                                case CustomType.OUT_NRF24:
                                    break;
                                case CustomType.OUT_W5500:
                                    //MAC
                                    report[JoyConst.ReportDataStart + 12] = obj.MAC[0];
                                    report[JoyConst.ReportDataStart + 13] = obj.MAC[1];
                                    report[JoyConst.ReportDataStart + 14] = obj.MAC[2];
                                    report[JoyConst.ReportDataStart + 15] = obj.MAC[3];
                                    report[JoyConst.ReportDataStart + 16] = obj.MAC[4];
                                    report[JoyConst.ReportDataStart + 17] = obj.MAC[5];
                                    //Local_IP
                                    report[JoyConst.ReportDataStart + 18] = obj.Local_IP[0];
                                    report[JoyConst.ReportDataStart + 19] = obj.Local_IP[1];
                                    report[JoyConst.ReportDataStart + 20] = obj.Local_IP[2];
                                    report[JoyConst.ReportDataStart + 21] = obj.Local_IP[3];
                                    //Local_Port
                                    report[JoyConst.ReportDataStart + 22] = (byte)(obj.Local_Port >> 8);
                                    report[JoyConst.ReportDataStart + 23] = (byte)obj.Local_Port;
                                    //Subnet
                                    report[JoyConst.ReportDataStart + 24] = obj.Subnet[0];
                                    report[JoyConst.ReportDataStart + 25] = obj.Subnet[1];
                                    report[JoyConst.ReportDataStart + 26] = obj.Subnet[2];
                                    report[JoyConst.ReportDataStart + 27] = obj.Subnet[3];
                                    //Gateway
                                    report[JoyConst.ReportDataStart + 28] = obj.Gateway[0];
                                    report[JoyConst.ReportDataStart + 29] = obj.Gateway[1];
                                    report[JoyConst.ReportDataStart + 30] = obj.Gateway[2];
                                    report[JoyConst.ReportDataStart + 31] = obj.Gateway[3];
                                    //DNS
                                    report[JoyConst.ReportDataStart + 32] = obj.DNS[0];
                                    report[JoyConst.ReportDataStart + 33] = obj.DNS[1];
                                    report[JoyConst.ReportDataStart + 34] = obj.DNS[2];
                                    report[JoyConst.ReportDataStart + 35] = obj.DNS[3];
                                    //Remote_IP
                                    report[JoyConst.ReportDataStart + 36] = obj.Remote_IP[0];
                                    report[JoyConst.ReportDataStart + 37] = obj.Remote_IP[1];
                                    report[JoyConst.ReportDataStart + 38] = obj.Remote_IP[2];
                                    report[JoyConst.ReportDataStart + 39] = obj.Remote_IP[3];
                                    //Remote_Port
                                    report[JoyConst.ReportDataStart + 40] = (byte)(obj.Remote_Port >> 8);
                                    report[JoyConst.ReportDataStart + 41] = (byte)obj.Remote_Port;
                                    break;
                                default:
                                    for (int i = 0; i < JoyConst.MaxFontSetCount; i++)
                                    {
                                        report[JoyConst.ReportDataStart + 12 + 4 * i] = currentCus.FontSetList[i].X;
                                        report[JoyConst.ReportDataStart + 12 + 4 * i + 1] = currentCus.FontSetList[i].Y;
                                        report[JoyConst.ReportDataStart + 12 + 4 * i + 2] = currentCus.FontSetList[i].LibIndex;
                                        report[JoyConst.ReportDataStart + 12 + 4 * i + 3] = currentCus.FontSetList[i].Count;
                                    }
                                    break;
                            }
                            report[JoyConst.ReportDataStart + 12 + 4 * JoyConst.MaxFontSetCount] = 0xff;
                        }
                    }
                    #endregion
                    break;
                case ReportType.CustomInfo:
                    //nothing
                    break;
                case ReportType.ButtonData:
                    #region ButtonData
                    if (Index >= 0 && Index < JoyConst.MaxButton / JoyConst.ButtonInfoCount)
                    {
                        int reportSpace = 4;
                        for (int i = 0; i < JoyConst.ButtonInfoCount; i++)
                        {
                            Button btn = obj.GetButton(Index * JoyConst.ButtonInfoCount + i);
                            if (btn != null)
                            {
                                report[JoyConst.ReportDataStart + i * reportSpace] = btn.Fun;
                                report[JoyConst.ReportDataStart + 1 + i * reportSpace] = btn.Code;
                                report[JoyConst.ReportDataStart + 2 + i * reportSpace] = btn.FunFN;
                                report[JoyConst.ReportDataStart + 3 + i * reportSpace] = btn.CodeFN;
                                report[JoyConst.ReportDataStart + 4 + i * reportSpace] = 0xff;
                            }
                        }
                    }
                    #endregion
                    break;
                case ReportType.ButtonInfo:
                    //nothing
                    break;
                case ReportType.HatData:
                    #region HatData
                    if (Index >= 0 && Index < JoyConst.MaxHat / JoyConst.HatInfoCount)
                    {
                        int reportSpace = 4;
                        for (int i = 0; i < JoyConst.HatInfoCount; i++)
                        {
                            Hat hat = obj.GetHat(Index * JoyConst.HatInfoCount + i);
                            if (hat != null)
                            {
                                report[JoyConst.ReportDataStart + i * reportSpace] = hat.InputIndex[0];
                                report[JoyConst.ReportDataStart + 1 + i * reportSpace] = hat.InputIndex[1];
                                report[JoyConst.ReportDataStart + 2 + i * reportSpace] = hat.InputIndex[2];
                                report[JoyConst.ReportDataStart + 3 + i * reportSpace] = hat.InputIndex[3];
                                report[JoyConst.ReportDataStart + 4 + i * reportSpace] = 0xff;
                            }
                        }
                    }
                    #endregion
                    break;
                case ReportType.HatInfo:
                    //nothing
                    break;
                case ReportType.AdcData:
                    #region AdcData
                    if (Index >= 0 && Index < JoyConst.MaxADC / JoyConst.AdcInfoCount)
                    {
                        int reportSpace = 3;
                        for (int i = 0; i < JoyConst.AdcInfoCount; i++)
                        {
                            Adc adc = obj.GetAdc(Index * JoyConst.AdcInfoCount + i);
                            if (adc != null)
                            {
                                report[JoyConst.ReportDataStart + i * reportSpace] = adc.maxCC;
                                report[JoyConst.ReportDataStart + 1 + i * reportSpace] = adc.maxCY;
                                report[JoyConst.ReportDataStart + 2 + i * reportSpace] = adc.maxPC;
                                report[JoyConst.ReportDataStart + 3 + i * reportSpace] = 0xff;
                            }
                        }
                    }
                    #endregion
                    break;
                case ReportType.AdcInfo:
                    //nothing
                    break;
                case ReportType.FormatData:
                    #region FormatData
                    if (Index >= 0 && Index < JoyConst.MaxFormat / JoyConst.FormatInfoCount)
                    {
                        int reportSpace = 13;
                        for (int i = 0; i < JoyConst.FormatInfoCount; i++)
                        {
                            Format format = obj.GetFormat(Index * JoyConst.FormatInfoCount + i);
                            if (format != null)
                            {
                                report[JoyConst.ReportDataStart + i * reportSpace] = (byte)(format.Reverse ? 1 : 0);
                                report[JoyConst.ReportDataStart + 1 + i * reportSpace] = (byte)(format.Calibration ? 1 : 0);
                                report[JoyConst.ReportDataStart + 2 + i * reportSpace] = (byte)(format.AutoRange ? 1 : 0);
                                report[JoyConst.ReportDataStart + 3 + i * reportSpace] = format.Shift;
                                report[JoyConst.ReportDataStart + 4 + i * reportSpace] = format.minDzone;
                                report[JoyConst.ReportDataStart + 5 + i * reportSpace] = format.midDzone;
                                report[JoyConst.ReportDataStart + 6 + i * reportSpace] = format.maxDzone;
                                report[JoyConst.ReportDataStart + 7 + i * reportSpace] = (byte)(format.minValue >> 8);
                                report[JoyConst.ReportDataStart + 8 + i * reportSpace] = (byte)format.minValue;
                                report[JoyConst.ReportDataStart + 9 + i * reportSpace] = (byte)(format.midValue >> 8);
                                report[JoyConst.ReportDataStart + 10 + i * reportSpace] = (byte)format.midValue;
                                report[JoyConst.ReportDataStart + 11 + i * reportSpace] = (byte)(format.maxValue >> 8);
                                report[JoyConst.ReportDataStart + 12 + i * reportSpace] = (byte)format.maxValue;
                                report[JoyConst.ReportDataStart + 13 + i * reportSpace] = 0xff;
                            }
                        }
                    }
                    #endregion
                    break;
                case ReportType.FormatInfo:
                    //nothing
                    break;
                case ReportType.LedData:
                    #region LedData
                    if (Index >= 0 && Index < JoyConst.MaxLed / JoyConst.LedInfoCount)
                    {
                        int reportSpace;
                        switch (obj.HardwareVersion)
                        {
                            case "KB":
                                reportSpace = 11;
                                for (int i = 0; i < JoyConst.LedInfoCount; i++)
                                {
                                    Led led = obj.GetLed(Index * JoyConst.LedInfoCount + i);
                                    ColorInfo color = obj.GetColorInfo(Index * JoyConst.LedInfoCount + i);
                                    if (led != null && color != null)
                                    {
                                        //report[JoyConst.ReportDataStart + i * reportSpace] = (byte)(led.Always ? 1 : 0);
                                        report[JoyConst.ReportDataStart + 1 + i * reportSpace] = (byte)(led.ControlType);
                                        report[JoyConst.ReportDataStart + 2 + i * reportSpace] = led.R;
                                        report[JoyConst.ReportDataStart + 3 + i * reportSpace] = led.G;
                                        report[JoyConst.ReportDataStart + 4 + i * reportSpace] = led.B;
                                        report[JoyConst.ReportDataStart + 5 + i * reportSpace] = led.OpenR;
                                        report[JoyConst.ReportDataStart + 6 + i * reportSpace] = led.OpenG;
                                        report[JoyConst.ReportDataStart + 7 + i * reportSpace] = led.OpenB;
                                        report[JoyConst.ReportDataStart + 8 + i * reportSpace] = (byte)color.ClickMode;
                                        report[JoyConst.ReportDataStart + 9 + i * reportSpace] = (byte)color.FnMode;
                                        report[JoyConst.ReportDataStart + 10 + i * reportSpace] = (byte)(color.Offset >> 8);
                                        report[JoyConst.ReportDataStart + 11 + i * reportSpace] = (byte)color.Offset;
                                        report[JoyConst.ReportDataStart + 12 + i * reportSpace] = 0xff;
                                    }
                                }
                                break;
                            default:
                                reportSpace = 8;
                                for (int i = 0; i < JoyConst.LedInfoCount; i++)
                                {
                                    Led led = obj.GetLed(Index * JoyConst.LedInfoCount + i);
                                    if (led != null)
                                    {
                                        //report[JoyConst.ReportDataStart + i * reportSpace] = (byte)(led.Always ? 1 : 0);
                                        report[JoyConst.ReportDataStart + 1 + i * reportSpace] = (byte)(led.ControlType);
                                        report[JoyConst.ReportDataStart + 2 + i * reportSpace] = led.R;
                                        report[JoyConst.ReportDataStart + 3 + i * reportSpace] = led.G;
                                        report[JoyConst.ReportDataStart + 4 + i * reportSpace] = led.B;
                                        report[JoyConst.ReportDataStart + 5 + i * reportSpace] = led.OpenR;
                                        report[JoyConst.ReportDataStart + 6 + i * reportSpace] = led.OpenG;
                                        report[JoyConst.ReportDataStart + 7 + i * reportSpace] = led.OpenB;
                                        report[JoyConst.ReportDataStart + 8 + i * reportSpace] = 0xff;
                                    }
                                }
                                break;
                        }
                    }
                    #endregion
                    break;
                case ReportType.LedInfo:
                    //nothing
                    break;
                case ReportType.SaveUsbData:
                    #region SetUsbData
                    int maxIndex = 0;
                    byte[] name = System.Text.Encoding.Default.GetBytes(obj.usbName);
                    if (obj.usbName.Length > JoyConst.MaxUsbName - 1)
                    {
                        maxIndex = JoyConst.MaxUsbName - 1;
                    }
                    else
                    {
                        maxIndex = obj.usbName.Length;
                    }
                    for (int i = 0; i < maxIndex; i++)
                    {
                        report[JoyConst.ReportDataStart + i] = name[i];
                    }
                    report[JoyConst.ReportDataStart + maxIndex] = System.Convert.ToByte('\0');
                    report[JoyConst.ReportDataStart + JoyConst.MaxUsbName] = obj.maxOutJoystick;
                    report[JoyConst.ReportDataStart + JoyConst.MaxUsbName + 1] = obj.maxOutAxis;
                    report[JoyConst.ReportDataStart + JoyConst.MaxUsbName + 2] = obj.maxOutHat;
                    report[JoyConst.ReportDataStart + JoyConst.MaxUsbName + 3] = (byte)obj.VID;
                    report[JoyConst.ReportDataStart + JoyConst.MaxUsbName + 4] = (byte)(obj.VID >> 8);
                    report[JoyConst.ReportDataStart + JoyConst.MaxUsbName + 5] = (byte)obj.PID;
                    report[JoyConst.ReportDataStart + JoyConst.MaxUsbName + 6] = (byte)(obj.PID >> 8);
                    report[JoyConst.ReportDataStart + JoyConst.MaxUsbName + 7] = obj.ColorOrder;
                    report[JoyConst.ReportDataStart + JoyConst.MaxUsbName + 8] = obj.BackLightBrightness;
                    report[JoyConst.ReportDataStart + JoyConst.MaxUsbName + 9] = (byte)(obj.HC165 ? 1 : 0);
                    report[JoyConst.ReportDataStart + JoyConst.MaxUsbName + 10] = obj.axisID[0];
                    report[JoyConst.ReportDataStart + JoyConst.MaxUsbName + 11] = obj.axisID[1];
                    report[JoyConst.ReportDataStart + JoyConst.MaxUsbName + 12] = obj.axisID[2];
                    report[JoyConst.ReportDataStart + JoyConst.MaxUsbName + 13] = obj.axisID[3];
                    report[JoyConst.ReportDataStart + JoyConst.MaxUsbName + 14] = obj.axisID[4];
                    report[JoyConst.ReportDataStart + JoyConst.MaxUsbName + 15] = obj.axisID[5];
                    report[JoyConst.ReportDataStart + JoyConst.MaxUsbName + 16] = obj.axisID[6];
                    report[JoyConst.ReportDataStart + JoyConst.MaxUsbName + 17] = obj.axisID[7];
                    report[JoyConst.ReportDataStart + JoyConst.MaxUsbName + 18] = obj.joyMaxADC;
                    report[JoyConst.ReportDataStart + JoyConst.MaxUsbName + 19] = obj.joyMaxHall;
                    report[JoyConst.ReportDataStart + JoyConst.MaxUsbName + 20] = obj.joyMaxPWM;
                    report[JoyConst.ReportDataStart + JoyConst.MaxUsbName + 21] = obj.joyMaxPin;
                    report[JoyConst.ReportDataStart + JoyConst.MaxUsbName + 22] = (byte)obj.IdleColor;
                    report[JoyConst.ReportDataStart + JoyConst.MaxUsbName + 23] = obj.DynamicSpeed;
                    report[JoyConst.ReportDataStart + JoyConst.MaxUsbName + 24] = (byte)(obj.USBPower ? 1 : 0);
                    #endregion
                    break;
                case ReportType.GetUsbInfo:
                    //nothing
                    break;
                case ReportType.SyncProperty:
                    #region 当前选中的属性
                    //do it
                    report[JoyConst.ReportDataStart] = (byte)obj.TypeSwitchControl;
                    if (PublicData.ui_Type == UIType.NodeLink)
                    {
                        report[JoyConst.ReportDataStart] = (byte)TypeSwitch.NoneSwitch;
                    }
                    switch (obj.TypeSwitchControl)
                    {
                        case TypeSwitch.NoneSwitch:
                            break;
                        case TypeSwitch.DeviceSwitch:
                            #region TypeSwitch.Device
                            JoyDevice dev = obj.GetJoyDevice(obj.SelectDevice);
                            if (dev == null)
                            {
                                report[JoyConst.ReportDataStart + 1] = 255;
                            }
                            else
                            {
                                report[JoyConst.ReportDataStart + 1] = (byte)obj.SelectDevice;
                                report[JoyConst.ReportDataStart + 2] = (byte)dev.Type;
                                report[JoyConst.ReportDataStart + 3] = (byte)(dev.inInversion ? 1 : 0);
                                report[JoyConst.ReportDataStart + 4] = (byte)dev.outType;
                                report[JoyConst.ReportDataStart + 5] = (byte)(dev.outInversion ? 1 : 0);
                                report[JoyConst.ReportDataStart + 6] = (byte)(dev.outLed ? 1 : 0);
                                report[JoyConst.ReportDataStart + 7] = (byte)dev.encodeType;
                                report[JoyConst.ReportDataStart + 8] = dev.inPort;
                                report[JoyConst.ReportDataStart + 9] = dev.inEnd;
                                report[JoyConst.ReportDataStart + 10] = dev.outPort;
                                report[JoyConst.ReportDataStart + 11] = dev.outCount;
                                report[JoyConst.ReportDataStart + 12] = dev.pulseCount;
                                report[JoyConst.ReportDataStart + 13] = dev.ledPort;
                                report[JoyConst.ReportDataStart + 14] = dev.ledCount;
                                #region Adc
                                Adc adc = obj.GetAdc(obj.SelectFormatIn);
                                if (adc == null)
                                {
                                    report[JoyConst.ReportDataStart + 15] = 255;
                                }
                                else
                                {
                                    report[JoyConst.ReportDataStart + 15] = dev.inPort;
                                    report[JoyConst.ReportDataStart + 16] = adc.maxCC;
                                    report[JoyConst.ReportDataStart + 17] = adc.maxCY;
                                    report[JoyConst.ReportDataStart + 18] = adc.maxPC;
                                }
                                #endregion
                                #region Format
                                Format format = obj.GetFormat(obj.SelectFormatIn);
                                if (format == null)
                                {
                                    report[JoyConst.ReportDataStart + 19] = 255;
                                }
                                else
                                {
                                    report[JoyConst.ReportDataStart + 19] = (byte)obj.SelectFormatIn;
                                    report[JoyConst.ReportDataStart + 20] = (byte)(format.Reverse ? 1 : 0);
                                    report[JoyConst.ReportDataStart + 21] = (byte)(format.AutoRange ? 1 : 0);
                                    report[JoyConst.ReportDataStart + 22] = (byte)(format.Calibration ? 1 : 0);
                                    report[JoyConst.ReportDataStart + 23] = format.Shift;
                                    report[JoyConst.ReportDataStart + 24] = format.minDzone;
                                    report[JoyConst.ReportDataStart + 25] = format.midDzone;
                                    report[JoyConst.ReportDataStart + 26] = format.maxDzone;
                                    report[JoyConst.ReportDataStart + 27] = (byte)(format.minValue >> 8);
                                    report[JoyConst.ReportDataStart + 28] = (byte)(format.minValue);
                                    report[JoyConst.ReportDataStart + 29] = (byte)(format.midValue >> 8);
                                    report[JoyConst.ReportDataStart + 30] = (byte)(format.midValue);
                                    report[JoyConst.ReportDataStart + 31] = (byte)(format.maxValue >> 8);
                                    report[JoyConst.ReportDataStart + 32] = (byte)(format.maxValue);
                                }
                                #endregion
                                #region Button
                                Button button = obj.GetButton(obj.SelectButton);
                                if (button == null)
                                {
                                    report[JoyConst.ReportDataStart + 33] = 255;
                                }
                                else
                                {
                                    report[JoyConst.ReportDataStart + 33] = (byte)obj.SelectButton;
                                    report[JoyConst.ReportDataStart + 34] = button.Code;
                                    report[JoyConst.ReportDataStart + 35] = button.Fun;
                                    report[JoyConst.ReportDataStart + 36] = button.CodeFN;
                                    report[JoyConst.ReportDataStart + 37] = button.FunFN;
                                }
                                #endregion
                                #region Hat
                                Hat hat = obj.GetHat(obj.SelectHat);
                                if (hat == null)
                                {
                                    report[JoyConst.ReportDataStart + 38] = 255;
                                }
                                else
                                {
                                    report[JoyConst.ReportDataStart + 38] = (byte)obj.SelectHat;
                                    report[JoyConst.ReportDataStart + 39] = hat.InputIndex[0];
                                    report[JoyConst.ReportDataStart + 40] = hat.InputIndex[1];
                                    report[JoyConst.ReportDataStart + 41] = hat.InputIndex[2];
                                    report[JoyConst.ReportDataStart + 42] = hat.InputIndex[3];
                                }
                                #endregion
                                #region Led
                                Led led = obj.GetLed(obj.SelectLed);
                                if (led == null)
                                {
                                    report[JoyConst.ReportDataStart + 43] = 255;
                                }
                                else
                                {
                                    report[JoyConst.ReportDataStart + 43] = (byte)obj.SelectLed;
                                    //report[JoyConst.ReportDataStart + 44] = (byte)(led.Always ? 1 : 0);
                                    report[JoyConst.ReportDataStart + 44] = (byte)(led.ControlType);
                                    report[JoyConst.ReportDataStart + 45] = led.R;
                                    report[JoyConst.ReportDataStart + 46] = led.G;
                                    report[JoyConst.ReportDataStart + 47] = led.B;
                                    report[JoyConst.ReportDataStart + 48] = led.OpenR;
                                    report[JoyConst.ReportDataStart + 49] = led.OpenG;
                                    report[JoyConst.ReportDataStart + 50] = led.OpenB;
                                }
                                #endregion
                                #region ColorInfo
                                switch (obj.HardwareVersion)
                                {
                                    case "KB":
                                        ColorInfo color = obj.GetColorInfo(obj.SelectLed);
                                        if (color != null)
                                        {
                                            report[JoyConst.ReportDataStart + 38] = (byte)color.ClickMode;
                                            report[JoyConst.ReportDataStart + 39] = (byte)color.FnMode;
                                            report[JoyConst.ReportDataStart + 40] = (byte)(color.Offset >> 8);
                                            report[JoyConst.ReportDataStart + 41] = (byte)color.Offset;
                                        }
                                        break;
                                }
                                #endregion
                            }
                            #region 公共属性
                            report[JoyConst.ReportDataStart + 51] = obj.ColorOrder;
                            report[JoyConst.ReportDataStart + 52] = obj.BackLightBrightness;
                            report[JoyConst.ReportDataStart + 53] = (byte)(obj.HC165 ? 1 : 0);
                            report[JoyConst.ReportDataStart + 54] = (byte)(obj.USBPower ? 1 : 0);
                            #endregion
                            #region KB
                            switch (obj.HardwareVersion)
                            {
                                case "KB":
                                    report[JoyConst.ReportDataStart + 31] = (byte)obj.IdleColor;
                                    report[JoyConst.ReportDataStart + 32] = obj.DynamicSpeed;
                                    break;
                            }
                            #endregion
                            break;
                        #endregion
                        case TypeSwitch.CustomSwitch:
                            #region TypeSwitch.Custom
                            JoyCustom cus = obj.GetJoyCustom(obj.SelectCustom);
                            if (cus != null)
                            {
                                report[JoyConst.ReportDataStart + 1] = (byte)obj.SelectCustom;
                                report[JoyConst.ReportDataStart + 2] = (byte)cus.Type;
                                report[JoyConst.ReportDataStart + 3] = (byte)cus.rotateType;
                                report[JoyConst.ReportDataStart + 4] = cus.data;
                                report[JoyConst.ReportDataStart + 5] = cus.cs;
                                report[JoyConst.ReportDataStart + 6] = cus.clk;
                                report[JoyConst.ReportDataStart + 7] = cus.dc;
                                report[JoyConst.ReportDataStart + 8] = cus.rst;
                                report[JoyConst.ReportDataStart + 9] = cus.cs2;
                                report[JoyConst.ReportDataStart + 10] = cus.cs3;
                                report[JoyConst.ReportDataStart + 11] = cus.cs4;
                                report[JoyConst.ReportDataStart + 12] = cus.dataStart;
                                report[JoyConst.ReportDataStart + 13] = cus.dataCount;
                                switch (cus.Type)
                                {
                                    case CustomType.DT_Max7219:
                                    case CustomType.DT_TM1638:
                                    case CustomType.DT_HT16K33:
                                        //nothing
                                        break;
                                    case CustomType.Matrix_Max7219:
                                    //----
                                    case CustomType.OLED_70_40_SSD1306://0.42
                                    case CustomType.OLED_70_40_SSD1306x2:
                                    case CustomType.OLED_70_40_SSD1306x3:
                                    case CustomType.OLED_70_40_SSD1306x4:
                                    //----
                                    case CustomType.OLED_48_64_SSD1306://0.71
                                    case CustomType.OLED_48_64_SSD1306x2:
                                    case CustomType.OLED_48_64_SSD1306x3:
                                    case CustomType.OLED_48_64_SSD1306x4:
                                    //----
                                    case CustomType.OLED_64_32_SSD1306://0.49
                                    case CustomType.OLED_64_32_SSD1306x2:
                                    case CustomType.OLED_64_32_SSD1306x3:
                                    case CustomType.OLED_64_32_SSD1306x4:
                                    //----
                                    case CustomType.OLED_64_48_SSD1306://0.66, 0.71
                                    case CustomType.OLED_64_48_SSD1306x2:
                                    case CustomType.OLED_64_48_SSD1306x3:
                                    case CustomType.OLED_64_48_SSD1306x4:
                                    //----
                                    case CustomType.OLED_96_16_SSD1306://0.86
                                    case CustomType.OLED_96_16_SSD1306x2:
                                    case CustomType.OLED_96_16_SSD1306x3:
                                    case CustomType.OLED_96_16_SSD1306x4:
                                    //----
                                    case CustomType.OLED_128_32_SSD1306://0.91
                                    case CustomType.OLED_128_32_SSD1306x2:
                                    case CustomType.OLED_128_32_SSD1306x3:
                                    case CustomType.OLED_128_32_SSD1306x4:
                                    //----
                                    case CustomType.OLED_128_64_SSD1306://0.96, 1.09
                                    case CustomType.OLED_128_64_SSD1306x2:
                                    case CustomType.OLED_128_64_SSD1306x3:
                                    case CustomType.OLED_128_64_SSD1306x4:
                                    //----
                                    case CustomType.OLED_128_64_SH1106://1.3
                                    case CustomType.OLED_128_64_SH1106x2:
                                    case CustomType.OLED_128_64_SH1106x3:
                                    case CustomType.OLED_128_64_SH1106x4:
                                    //----
                                    case CustomType.OLED_128_88_SH1107://0.73
                                    case CustomType.OLED_128_88_SH1107x2:
                                    case CustomType.OLED_128_88_SH1107x3:
                                    case CustomType.OLED_128_88_SH1107x4:
                                    //----
                                    case CustomType.OLED_256_64_SSD1322://3.12
                                    case CustomType.OLED_256_64_SSD1322x2:
                                    case CustomType.OLED_256_64_SSD1322x3:
                                    case CustomType.OLED_256_64_SSD1322x4:
                                        for (int i = 0; i < JoyConst.MaxFontSetCount; i++)
                                        {
                                            report[JoyConst.ReportDataStart + 14 + 4 * i] = cus.FontSetList[i].X;
                                            report[JoyConst.ReportDataStart + 14 + 4 * i + 1] = cus.FontSetList[i].Y;
                                            report[JoyConst.ReportDataStart + 14 + 4 * i + 2] = cus.FontSetList[i].LibIndex;
                                            report[JoyConst.ReportDataStart + 14 + 4 * i + 3] = cus.FontSetList[i].Count;
                                        }
                                        break;
                                    case CustomType.OUT_StepperMotor:
                                    case CustomType.OUT_74HC595:
                                    case CustomType.OUT_IO:
                                    case CustomType.OUT_NRF24:
                                    case CustomType.OUT_W5500:
                                        //nothing
                                        break;
                                }
                            }
                            else
                            {
                                report[JoyConst.ReportDataStart + 1] = 255;
                            }
                            break;
                        #endregion
                        case TypeSwitch.FontLibrarySwitch:
                            //do it
                            break;
                    }
                    #endregion
                    break;
                case ReportType.DeviceSync:
                    #region 计算LED数据 0 - 15
                    for (int i = 0; i < obj.ledControlReport.Length; i++)
                    {
                        obj.ledControlReport[i] = 0;
                    }
                    for (int i = 0; i < JoyConst.MaxLed; i++)
                    {
                        if (obj.GetLed(i).SoftValue)
                        {
                            int listIndex = i / 8;
                            int hexIndex = i % 8;
                            switch (hexIndex)
                            {
                                case 0:
                                    obj.ledControlReport[listIndex] |= 0x01;
                                    break;
                                case 1:
                                    obj.ledControlReport[listIndex] |= 0x02;
                                    break;
                                case 2:
                                    obj.ledControlReport[listIndex] |= 0x04;
                                    break;
                                case 3:
                                    obj.ledControlReport[listIndex] |= 0x08;
                                    break;
                                case 4:
                                    obj.ledControlReport[listIndex] |= 0x10;
                                    break;
                                case 5:
                                    obj.ledControlReport[listIndex] |= 0x20;
                                    break;
                                case 6:
                                    obj.ledControlReport[listIndex] |= 0x40;
                                    break;
                                case 7:
                                    obj.ledControlReport[listIndex] |= 0x80;
                                    break;
                            }
                        }
                    }
                    for (int i = 0; i < obj.ledControlReport.Length; i++)
                    {
                        report[JoyConst.ReportDataStart + i] = obj.ledControlReport[i];
                    }
                    #endregion
                    #region Pwm数据 16 - 23
                    for (int i = 0; i < JoyConst.MaxPWM; i++)
                    {
                        report[JoyConst.ReportDataStart + obj.ledControlReport.Length + i] = obj.pwmControlReport[i];
                    }
                    #endregion
                    #region Button数据 24 - 39
                    for (int i = 0; i < obj.buttonControlReport.Length; i++)
                    {
                        obj.buttonControlReport[i] = 0;
                    }
                    for (int i = 0; i < JoyConst.MaxButton; i++)
                    {
                        if (obj.GetButton(i).SoftValue)
                        {
                            int listIndex = i / 8;
                            int hexIndex = i % 8;
                            switch (hexIndex)
                            {
                                case 0:
                                    obj.buttonControlReport[listIndex] |= 0x01;
                                    break;
                                case 1:
                                    obj.buttonControlReport[listIndex] |= 0x02;
                                    break;
                                case 2:
                                    obj.buttonControlReport[listIndex] |= 0x04;
                                    break;
                                case 3:
                                    obj.buttonControlReport[listIndex] |= 0x08;
                                    break;
                                case 4:
                                    obj.buttonControlReport[listIndex] |= 0x10;
                                    break;
                                case 5:
                                    obj.buttonControlReport[listIndex] |= 0x20;
                                    break;
                                case 6:
                                    obj.buttonControlReport[listIndex] |= 0x40;
                                    break;
                                case 7:
                                    obj.buttonControlReport[listIndex] |= 0x80;
                                    break;
                            }
                        }
                    }
                    for (int i = 0; i < obj.buttonControlReport.Length; i++)
                    {
                        report[JoyConst.ReportDataStart + obj.ledControlReport.Length + JoyConst.MaxPWM + i] = obj.buttonControlReport[i];
                    }
                    #endregion
                    #region Axis数据 40 - 55
                    for (int i = 0; i < JoyConst.MaxAxis; i++)
                    {
                        report[JoyConst.ReportDataStart + obj.ledControlReport.Length + JoyConst.MaxPWM + obj.buttonControlReport.Length + i * 2] =
                            (byte)obj.axisControlReport[i];
                        report[JoyConst.ReportDataStart + obj.ledControlReport.Length + JoyConst.MaxPWM + obj.buttonControlReport.Length + i * 2 + 1] =
                            (byte)(obj.axisControlReport[i] >> 8);
                    }
                    #endregion
                    break;
                case ReportType.CustomSync:
                    #region DataSend
                    for (int i = 0; i < JoyConst.LowSpeedData; i++)
                    {
                        if (i < JoyConst.HighSpeedData)
                        {
                            report[JoyConst.ReportDataStart + i] = obj.customDataList[i];
                        }
                        if (i < JoyConst.MidSpeedData)
                        {
                            report[JoyConst.ReportDataStart + JoyConst.HighSpeedData + i] =
                                obj.customDataList[JoyConst.HighSpeedData +
                                JoyConst.MidSpeedData * obj.currentMidSpeedData + i];
                        }
                        if (i < JoyConst.LowSpeedData)
                        {
                            report[JoyConst.ReportDataStart + JoyConst.HighSpeedData + JoyConst.MidSpeedData + i] =
                                obj.customDataList[JoyConst.HighSpeedData +
                                JoyConst.MidSpeedData * JoyConst.MidSpeedCount +
                                JoyConst.LowSpeedData * obj.currentLowSpeedData + i];
                        }
                        report[JoyConst.ReportDataStart + JoyConst.MaxCustomReport] = obj.currentMidSpeedData;
                        report[JoyConst.ReportDataStart + JoyConst.MaxCustomReport + 1] = obj.currentLowSpeedData;
                    }
                    obj.currentMidSpeedData++;
                    if (obj.currentMidSpeedData >= JoyConst.MidSpeedCount)
                    {
                        obj.currentMidSpeedData = 0;
                    }
                    obj.currentLowSpeedData++;
                    if (obj.currentLowSpeedData >= JoyConst.LowSpeedCount)
                    {
                        obj.AddReport(new Report(ReportType.DeviceSync));
                        obj.currentLowSpeedData = 0;
                    }
                    #endregion
                    break;
                case ReportType.GetFont0:
                case ReportType.GetFont1:
                case ReportType.GetFont2:
                case ReportType.GetFont3:
                case ReportType.GetFont4:
                case ReportType.GetFont5:
                case ReportType.GetFont6:
                case ReportType.GetFont7:
                case ReportType.GetFontOver:
                    //nothing
                    break;
                case ReportType.ClearFont:
                    #region ClearFont
                    obj.ReSetFontLib();
                    #endregion
                    break;
                case ReportType.SyncFont0:
                    #region SyncFont1
                    obj.SetSyncFont(JoyConst.MaxSyncFontCount * Index, ref report);
                    #endregion
                    break;
                case ReportType.SyncFont1:
                    #region SyncFont1
                    obj.SetSyncFont(JoyConst.MaxSyncFontCount * Index + JoyConst.MaxSyncFontCount * 256, ref report);
                    #endregion
                    break;
                case ReportType.SyncFont2:
                    #region SyncFont2
                    obj.SetSyncFont(JoyConst.MaxSyncFontCount * Index + JoyConst.MaxSyncFontCount * 256 * 2, ref report);
                    #endregion
                    break;
                case ReportType.SyncFont3:
                    #region SyncFont3
                    obj.SetSyncFont(JoyConst.MaxSyncFontCount * Index + JoyConst.MaxSyncFontCount * 256 * 3, ref report);
                    #endregion
                    break;
                case ReportType.SyncFont4:
                    #region SyncFont4
                    obj.SetSyncFont(JoyConst.MaxSyncFontCount * Index + JoyConst.MaxSyncFontCount * 256 * 4, ref report);
                    #endregion
                    break;
                case ReportType.SyncFont5:
                    #region SyncFont5
                    obj.SetSyncFont(JoyConst.MaxSyncFontCount * Index + JoyConst.MaxSyncFontCount * 256 * 5, ref report);
                    #endregion
                    break;
                case ReportType.SyncFont6:
                    #region SyncFont6
                    obj.SetSyncFont(JoyConst.MaxSyncFontCount * Index + JoyConst.MaxSyncFontCount * 256 * 6, ref report);
                    #endregion
                    break;
                case ReportType.SyncFont7:
                    #region SyncFont7
                    obj.SetSyncFont(JoyConst.MaxSyncFontCount * Index + JoyConst.MaxSyncFontCount * 256 * 7, ref report);
                    #endregion
                    break;
                case ReportType.SaveFont:
                    #region SaveFont
                    #endregion
                    break;
                case ReportType.OledClear:
                case ReportType.KeyBoardSync:
                    //nothing
                    break;
                default:
                    WarningForm.Instance.OpenUI("Error get Report Type !!!" + Type.ToString(), false);
                    break;
            }
            #endregion
            report[JoyConst.ReportDataStart + JoyConst.MaxUsbData] = ECC;
            return true;
        }
    }
}
