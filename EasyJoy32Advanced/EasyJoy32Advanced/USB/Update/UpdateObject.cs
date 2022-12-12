//#define usbDebug
using FoxH.HID;
using System;
using System.ComponentModel;
using System.Threading;

namespace EasyControl
{
    public class UpdateObject : IDisposable, IComparable<UpdateObject>
    {
        private Thread threadJoy = null;
        //------------------------------------------------------------------------
        public int Index { get; private set; }
        public string Name { get; private set; }
        public string Key { get; private set; }
        public string Version { get; private set; }
        #region 设备读写
        public bool loop = true;
        public bool InReady = false;
        public bool OutReady = false;

        HIDDev devIn = new HIDDev();
        HIDDev devOut = new HIDDev();

        public V3xFirmware FirmwareVersion { get; private set; }
        public bool RunUpdate = false;
        #endregion
        #region Open
        private UpdateReportManager reportMgr;
        public int reportCount { get { return reportMgr.GetCount(); } }
        public void AddReport(UpdateReport report)
        {
            reportMgr.AddReport(report);
        }
        private bool _open = false;
        public bool Open
        {
            get { return _open; }
            set
            {
                try
                {
                    if (value != _open)
                    {
                        if (value)
                        {
                            if (CheckJoy() && PublicData.BinReady)
                            {
                                _open = true;
                                loop = true;
                                #region Usb报文处理
                                threadJoy = new Thread(MessageLink);
                                threadJoy.IsBackground = true;
                                threadJoy.Start();
                                #endregion
                                #region 初始化报文
                                reportMgr.ReSet();
                                reportMgr.AddReport(new UpdateReport(UpdateReportType.LinkTest, FirmwareVersion));
                                #endregion
                                return;
                            }
                            else
                            {
                                _open = false;
                            }
                        }
                    }
                    if (!_open)
                        loop = false;
                }
                catch (Exception ex)
                {
                    DebugConstol.AddLog(Index + " - " + Name + " - ERROR : " + ex.ToString(), LogType.Error);
                    loop = false;
                }
            }
        }
        #endregion
        public UpdateObject(string _ver, string _key, string _name)
        {
            reportMgr = new UpdateReportManager(this);
            Index = 1;
            Version = _ver;
            Key = _key;
            Name = _name;
            PublicData.ReadBinFile(_ver);
        }
        public void SelectVersion(V3xFirmware ver)
        {
            FirmwareVersion = ver;
        }
        public void StartUpdate()
        {
            reportMgr.AddReport(new UpdateReport(UpdateReportType.OpenFlash, FirmwareVersion));
            reportMgr.AddReport(new UpdateReport(UpdateReportType.SyncFlash, FirmwareVersion));
            reportMgr.AddReport(new UpdateReport(UpdateReportType.LockFlash, FirmwareVersion));
            reportMgr.AddReport(new UpdateReport(UpdateReportType.ReBoot, FirmwareVersion));
        }
        #region MessageLink
        private void MessageLink()
        {
            while (loop)
            {
                //Thread.Sleep(1);
                try
                {
                    //-------------------------------------------------------------------------------
                    if (_open)
                    {
                        #region 通信
                        byte[] report;
                        UpdateReport _report = reportMgr.GetReport();
                        if (!_report.GetReport(this, out report))
                        {
                            throw new Win32Exception("GetReport Error !!!");
                        }
                        if (Send(report))
                        {
                            if (_report.Type == UpdateReportType.ReBoot)
                            {
                                reportMgr.ReSet();
                            }
#if usbDebug
                            string reportSend = "";
                            string sendType = "";
                            for (int i = 0; i < JoyConst.MaxUsbReport; i++)
                            {
                                switch (i)
                                {
                                    case 0://ID
                                        reportSend += "<<";
                                        break;
                                    case 1://ver1
                                        reportSend += "[";
                                        reportSend += report[i].ToString() + ".";
                                        break;
                                    case 2://ver2
                                        reportSend += report[i].ToString() + ".";
                                        break;
                                    case 3://ver3
                                        reportSend += report[i].ToString();
                                        reportSend += "], ";
                                        break;
                                    case 4://type
                                        sendType = ((UpdateReportType)report[i]).ToString();
                                        break;
                                    case 5://Index
                                        sendType += "(";
                                        sendType += report[i].ToString();
                                        sendType += " - ";
                                        break;
                                    case 6://maxIndex
                                        sendType += report[i].ToString();
                                        sendType += "), ";
                                        break;
                                    case 7://Successful
                                        switch (report[i])
                                        {
                                            case 0:
                                                reportSend += "▼";
                                                break;
                                            case 1:
                                                reportSend += "▲";
                                                break;
                                            default:
                                                reportSend += "X";
                                                break;
                                        }
                                        break;
                                    case 8://data0
                                        reportSend += " -->";
                                        reportSend += report[i].ToString("X2") + ", ";
                                        break;
                                    default:
                                        reportSend += report[i].ToString("X2") + ", ";
                                        break;
                                }
                            }
                            DebugConstol.AddLog(Index + " - " + Name + " - TX : " + reportSend + " - " + sendType, LogType.NormalB);
#endif
                        }
                        byte[] receive;
                        if (Receive(out receive))
                        {
                            if (receive != null && receive[0] == UpdateReportManager.checkID)
                            {
#if usbDebug
                                string reportReceive = "";
                                string receiveType = "";
                                for (int i = 0; i < receive.Length; i++)
                                {
                                    switch (i)
                                    {
                                        case 0://ID
                                            reportReceive += ">>";
                                            break;
                                        case 1://ver1
                                            reportReceive += "[";
                                            reportReceive += receive[i].ToString() + ".";
                                            break;
                                        case 2://ver2
                                            reportReceive += receive[i].ToString() + ".";
                                            break;
                                        case 3://ver3
                                            reportReceive += receive[i].ToString();
                                            reportReceive += "], ";
                                            break;
                                        case 4://type
                                            receiveType = ((UpdateReportType)receive[i]).ToString();
                                            break;
                                        case 5://Index
                                            receiveType += "(";
                                            receiveType += receive[i].ToString();
                                            receiveType += " - ";
                                            break;
                                        case 6://maxIndex
                                            receiveType += receive[i].ToString();
                                            receiveType += "), ";
                                            break;
                                        case 7://Successful
                                            switch (receive[i])
                                            {
                                                case 0:
                                                    reportReceive += "▼";
                                                    break;
                                                case 1:
                                                    reportReceive += "▲";
                                                    break;
                                                default:
                                                    reportReceive += "X";
                                                    break;
                                            }
                                            break;
                                        case 8://data0
                                            reportReceive += " -->";
                                            reportReceive += receive[i].ToString("X2") + ", ";
                                            break;
                                        default:
                                            reportReceive += receive[i].ToString("X2") + ", ";
                                            break;
                                    }
                                }
#endif
                                string msg = "";
                                if (reportMgr.CheckReport(this, receive, out msg))
                                {
#if usbDebug
                                    DebugConstol.AddLog(Index + " - " + Name + " - RX : " + reportReceive + " - " + receiveType + " : " + msg);
                                }
                                else
                                {
                                    DebugConstol.AddLog(Index + " - " + Name + " - ERROR : " + msg, LogType.Error);
                                    DebugConstol.AddLog(Index + " - " + Name + " - ERROR : " + reportReceive + " - " + receiveType, LogType.Error);
#endif
                                }
                            }
                        }
                        #endregion
                    }
                    //-------------------------------------------------------------------------------
                }
                catch (Exception ex)
                {
                    DebugConstol.AddLog(Index + " - " + Name + " - ERROR : " + ex.ToString(), LogType.Error);
                    loop = false;
                }
            }
            Dispose();
        }
        private bool Receive(out byte[] report)
        {
            try
            {
                report = new byte[devIn.GetInputReportByteLength()];
                if (Open)
                {
                    devIn.Read(report);
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                throw new Win32Exception(ex.ToString());
            }
        }

        private bool Send(byte[] report)
        {
            try
            {
                if (Open && report.Length == devOut.GetOutputReportByteLength())
                {
                    devOut.Write(report);
                }
                else
                {
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                throw new Win32Exception(ex.ToString());
            }
        }
        public bool CheckJoy()
        {
            return InReady & OutReady;
        }
        public void OpenJoy(HIDInfo dev)
        {
            HIDDev tempDev = new HIDDev();
            try
            {
                if (dev != null)
                {
                    if (tempDev.Open(dev))
                    {
                        if (tempDev.GetInputReportByteLength() == 64)//In
                        {
                            devIn = tempDev;
                            InReady = true;
                        }
                        if (tempDev.GetOutputReportByteLength() == 64)//Out
                        {
                            devOut = tempDev;
                            OutReady = true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Win32Exception(ex.ToString());
            }
        }
        #endregion
        #region 排序，ToString
        public override string ToString()
        {
            return Name.ToString();
        }

        public int CompareTo(UpdateObject obj)
        {
            if (obj == null)
                return 1;
            if (obj.Index > Index)
                return 0;
            else
                return 1;
        }
        #endregion
        public void Dispose()
        {
            try
            {
                _open = false;
                if (devIn != null)
                {
                    devIn.Close();
                }
                if (devOut != null)
                {
                    devOut.Close();
                }
                GC.SuppressFinalize(this);
                threadJoy.Abort();
                threadJoy.Join();
            }
            catch (Exception ex)
            {
                DebugConstol.AddLog(ex.ToString(), LogType.Error);
            }
        }
    }
}
