using System.Collections.Generic;
using System.ComponentModel;

namespace EasyControl
{
    public class UpdateReportManager
    {
        public UpdateObject obj { get; private set; } = null;
        public const int reportID = 3;
        public const int checkID = 2;
        List<UpdateReport> reportList = new List<UpdateReport>();
        //------------------------------------------------------------------
        public UpdateReportManager(UpdateObject _obj)
        {
            if (_obj == null)
                throw new Win32Exception("New ReportManager Error !!!");
            obj = _obj;
        }

        public void ReSet()
        {
            reportList.Clear();
        }

        public bool AddReport(UpdateReport report)
        {
            reportList.Add(report);
            return true;
        }

        public int GetCount()
        {
            return reportList.Count;
        }

        public UpdateReport GetReport()
        {
            if (reportList.Count > 0)
            {
                return reportList[0];
            }
            UpdateReport newSyncProperty = new UpdateReport(UpdateReportType.LinkTest, obj.FirmwareVersion);
            reportList.Add(newSyncProperty);
            return reportList[0];
        }

        public bool CheckReport(UpdateObject obj, byte[] report, out string message)
        {
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
            UpdateReport usbReport = new UpdateReport(report);
            if (report[1] == UpdateReport.ProtocolVer1 &&
                report[2] == UpdateReport.ProtocolVer2 &&
                report[3] == UpdateReport.ProtocolVer3)
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
                            case UpdateReportType.LinkTest:
                                break;
                            case UpdateReportType.ReBoot:
                                //nothing
                                break;
                            //--------------
                            case UpdateReportType.OpenFlash:
                                UpdateForm.Instance.OpenUI(obj.Key + "\n" + Localization.Instance.GetLS("FlashErase"), false);
                                UpdateForm.Instance.UpdateProgress((float)usbReport.Index / usbReport.maxIndex);
                                //DebugConstol.AddLog("====OpenFlash====", LogType.NormalB);
                                break;
                            case UpdateReportType.SyncFlash:
                                UpdateForm.Instance.OpenUI(obj.Key + "\n" + Localization.Instance.GetLS("FlashProgram"), false);
                                UpdateForm.Instance.UpdateProgress((float)usbReport.Index / usbReport.maxIndex);
                                //DebugConstol.AddLog("====SyncFlash====", LogType.NormalB);
                                break;
                            case UpdateReportType.LockFlash:
                                UpdateForm.Instance.Close();
                                //DebugConstol.AddLog("====LockFlash====", LogType.NormalB);
                                break;
                            default:
                                message = "此类型未处理 ：" + usbReport.Type.ToString();
                                return false;
                        }
                        #endregion
                        if (reportList[0].IsOver())//是否已经完成
                        {
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
                //message = obj.usbName + " - " + Localization.Instance.GetLS("ErrorVersion");
                //MainUI.Instance.OpenWarningUI(message, false);
                message = "";
                obj.Open = false;
                return false;
            }
        }
    }
}
