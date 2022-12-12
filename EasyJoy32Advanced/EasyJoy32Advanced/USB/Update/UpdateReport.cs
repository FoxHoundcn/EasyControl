using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace EasyControl
{
    public enum UpdateReportType
    {
        LinkTest = 0,
        ReBoot,
        //--------------
        OpenFlash,
        SyncFlash,
        LockFlash,
    }
    public class UpdateReport
    {
        public const byte ProtocolVer1 = 1;
        public const byte ProtocolVer2 = 0;
        public const byte ProtocolVer3 = 0;
        public const int UpdateMaxUsbReport = 64;
        public const int UpdateReportDataStart = 10;
        public const int UpdateCount = 52;
        public const int UpdateMaxUsbData = UpdateMaxUsbReport - UpdateReportDataStart - 1;
        public UpdateReportType Type { get; private set; }
        public UInt16 Index { get; private set; }
        public UInt16 maxIndex { get; private set; }
        public bool Successful { get; private set; }
        public byte ECC { get; private set; }
        public static byte eccIndex = 1;

        public UpdateReport(byte[] report)
        {
            if (report.Length != UpdateMaxUsbReport)
                throw new Win32Exception("New Report whit UsbReport Error !!!");
            Type = (UpdateReportType)report[4];
            Index = (UInt16)(report[5] + (report[6] << 8));
            maxIndex = (UInt16)(report[7] + (report[8] << 8));
            Successful = report[9] != 0 ? true : false;
            ECC = report[UpdateReportDataStart + UpdateMaxUsbData];
        }
        public UpdateReport(UpdateReportType _type, V3xFirmware ver)
        {
            Type = _type;
            Successful = false;
            switch (_type)
            {
                case UpdateReportType.LinkTest:
                    Index = 0;
                    maxIndex = 0;
                    break;
                case UpdateReportType.ReBoot:
                    Index = 0;
                    maxIndex = 0;
                    break;
                //--------------
                case UpdateReportType.OpenFlash:
                    Index = 0;
                    switch (ver)
                    {
                        case V3xFirmware.v31:
                            maxIndex = (ushort)(((PublicData.BinLengthV3x - PublicData.BinStartV3x) / PublicData.PageLengthV3x) - 1);
                            break;
                        case V3xFirmware.v35:
                            maxIndex = (ushort)(((PublicData.BinLengthV3x - PublicData.BinStartV3x) / PublicData.PageLengthV3x) - 1);
                            break;
                        case V3xFirmware.vKB:
                            maxIndex = (ushort)(((PublicData.BinLengthV3x - PublicData.BinStartV3x) / PublicData.PageLengthV3x) - 1);
                            break;
                        case V3xFirmware.vNRF:
                            maxIndex = (ushort)(((PublicData.BinLengthV3x - PublicData.BinStartV3x) / PublicData.PageLengthV3x) - 1);
                            break;
                        case V3xFirmware.v4b:
                            maxIndex = (ushort)(((PublicData.BinLengthV4b - PublicData.BinStartV4b) / PublicData.PageLengthV4b) - 1);
                            break;
                    }
                    break;
                case UpdateReportType.SyncFlash:
                    Index = 0;
                    switch (ver)
                    {
                        case V3xFirmware.v31:
                            maxIndex = (UInt16)(PublicData.UpdateBinArrayV31.Length / UpdateCount + 1);
                            break;
                        case V3xFirmware.v35:
                            maxIndex = (UInt16)(PublicData.UpdateBinArrayV35.Length / UpdateCount + 1);
                            break;
                        case V3xFirmware.vKB:
                            maxIndex = (UInt16)(PublicData.UpdateBinArrayVKB.Length / UpdateCount + 1);
                            break;
                        case V3xFirmware.vNRF:
                            maxIndex = (UInt16)(PublicData.UpdateBinArrayVNRF.Length / UpdateCount + 1);
                            break;
                        case V3xFirmware.v4b:
                            maxIndex = (UInt16)(PublicData.UpdateBinArrayV4b.Length / UpdateCount + 1);
                            break;
                    }
                    break;
                case UpdateReportType.LockFlash:
                    Index = 0;
                    maxIndex = 0;
                    break;
                default:
                    MessageBox.Show("Error new Report Type !!!" + _type.ToString());
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

        public bool GetReport(UpdateObject obj, out byte[] report)
        {
            report = new byte[UpdateMaxUsbReport];
            if (obj == null)
                return false;
            report[0] = ReportManager.reportID;
            report[1] = UpdateReport.ProtocolVer1;
            report[2] = UpdateReport.ProtocolVer2;
            report[3] = UpdateReport.ProtocolVer3;
            report[4] = (byte)Type;
            report[5] = (byte)Index;
            report[6] = (byte)(Index >> 8);
            report[7] = (byte)maxIndex;
            report[8] = (byte)(maxIndex >> 8);
            report[9] = (byte)(Successful ? 1 : 0);//Successful
            #region 数据内容
            switch (Type)
            {
                //do it
                case UpdateReportType.LinkTest:
                    #region LinkTest
                    //do it
                    #endregion
                    break;
                case UpdateReportType.ReBoot:
                    //nothing
                    break;
                //--------------
                case UpdateReportType.OpenFlash:
                    break;
                case UpdateReportType.SyncFlash:
                    for (int i = 0; i < UpdateCount; i++)
                    {
                        int index = Index * UpdateCount + i;
                        switch (obj.FirmwareVersion)
                        {
                            case V3xFirmware.v31:
                                if (index < PublicData.UpdateBinArrayV31.Length)
                                    report[UpdateReportDataStart + i] = PublicData.UpdateBinArrayV31[Index * UpdateCount + i];
                                else
                                    report[UpdateReportDataStart + i] = 0xff;
                                break;
                            case V3xFirmware.v35:
                                if (index < PublicData.UpdateBinArrayV35.Length)
                                    report[UpdateReportDataStart + i] = PublicData.UpdateBinArrayV35[Index * UpdateCount + i];
                                else
                                    report[UpdateReportDataStart + i] = 0xff;
                                break;
                            case V3xFirmware.vKB:
                                if (index < PublicData.UpdateBinArrayVKB.Length)
                                    report[UpdateReportDataStart + i] = PublicData.UpdateBinArrayVKB[Index * UpdateCount + i];
                                else
                                    report[UpdateReportDataStart + i] = 0xff;
                                break;
                            case V3xFirmware.vNRF:
                                if (index < PublicData.UpdateBinArrayVNRF.Length)
                                    report[UpdateReportDataStart + i] = PublicData.UpdateBinArrayVNRF[Index * UpdateCount + i];
                                else
                                    report[UpdateReportDataStart + i] = 0xff;
                                break;
                            case V3xFirmware.v4b:
                                if (index < PublicData.UpdateBinArrayV4b.Length)
                                    report[UpdateReportDataStart + i] = PublicData.UpdateBinArrayV4b[Index * UpdateCount + i];
                                else
                                    report[UpdateReportDataStart + i] = 0xff;
                                break;
                        }
                    }
                    break;
                case UpdateReportType.LockFlash:
                    break;
                default:
                    //MainUI.Instance.OpenWarningUI("Error get Report Type !!!" + Type.ToString(), false);
                    break;
            }
            #endregion
            report[UpdateReportDataStart + UpdateMaxUsbData] = ECC;
            return true;
        }
    }
}
