using System;
using System.IO;
using System.Collections.Generic;
using Microsoft.Win32.SafeHandles;
using System.ComponentModel;
using System.Text;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace FoxH.HID
{
    public class HIDDev : IDisposable
    {

        /* device handle */
        private IntPtr handle;
        /* stream */
        private FileStream _fileStream;

        private HIDSystem.HIDP_CAPS myHIDP_CAPS;

        /* stream */
        public FileStream fileStream
        {
            get
            {
                return _fileStream;
            }
            /* do not expose this setter */
            internal set
            {
                _fileStream = value;
            }
        }

        /* dispose */
        public void Dispose()
        {
            try
            {
                /* deal with file stream */
                if (_fileStream != null)
                {
                    /* close stream */
                    _fileStream.Close();
                    /* get rid of object */
                    _fileStream = null;
                }

                /* close handle */
                HIDSystem.CloseHandle(handle);
            }
            catch (Exception ex)
            {
                throw new Win32Exception(ex.ToString());
            }
        }

        /* open hid device */
        public bool Open(HIDInfo dev)
        {
            try
            {
                /* safe file handle */
                SafeFileHandle shandle;

                Process tool = new Process();
                tool.StartInfo.FileName = System.Environment.CurrentDirectory + @"\USB\handle.exe";
                tool.StartInfo.Arguments = dev.Path + " /accepteula -c -y";
                tool.StartInfo.UseShellExecute = false;
                tool.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                tool.StartInfo.CreateNoWindow = true;
                tool.StartInfo.RedirectStandardOutput = true;
                tool.Start();
                tool.WaitForExit();
                //string outputTool = tool.StandardOutput.ReadToEnd();

                //string matchPattern = @"(?<=\s+pid:\s+)\b(\d+)\b(?=\s+)";
                //foreach (Match match in Regex.Matches(outputTool, matchPattern))
                //{
                //    Process.GetProcessById(int.Parse(match.Value)).Kill();
                //}
                /* opens hid device file */
                handle = HIDSystem.CreateFile(dev.Path,
                                              HIDSystem.GENERIC_READ | HIDSystem.GENERIC_WRITE,
                                              HIDSystem.FILE_SHARE_READ | HIDSystem.FILE_SHARE_WRITE,
                                              IntPtr.Zero, HIDSystem.OPEN_EXISTING, HIDSystem.FILE_FLAG_OVERLAPPED,
                                              IntPtr.Zero);

                /* whops */
                if (handle == HIDSystem.INVALID_HANDLE_VALUE)
                {
                    return false;
                }

                /* build up safe file handle */
                shandle = new SafeFileHandle(handle, false);

                IntPtr myPtrToPreparsedData = IntPtr.Zero;
                bool result1 = HIDSystem.HidD_GetPreparsedData(handle, ref myPtrToPreparsedData);
                if (result1)
                {
                    myHIDP_CAPS = new HIDSystem.HIDP_CAPS();
                    int result2 = HIDSystem.HidP_GetCaps(myPtrToPreparsedData, ref myHIDP_CAPS);
                    if (result2 != HIDSystem.HIDP_STATUS_SUCCESS)
                    {
                        return false;
                    }
                }

                /* prepare stream - async */
                _fileStream = new FileStream(shandle, FileAccess.ReadWrite,
                                             64, true);

                /* report status */
                return true;
            }
            catch (Exception ex)
            {
                throw new Win32Exception(ex.ToString());
            }
        }

        public int GetInputReportByteLength()
        {
            return myHIDP_CAPS.InputReportByteLength;
        }

        public int GetOutputReportByteLength()
        {
            return myHIDP_CAPS.OutputReportByteLength;
        }

        public int GetFeatureReportByteLength()
        {
            return myHIDP_CAPS.FeatureReportByteLength;
        }

        /* close hid device */
        public void Close()
        {
            try
            {
                /* deal with file stream */
                if (_fileStream != null)
                {
                    /* close stream */
                    _fileStream.Close();
                    /* get rid of object */
                    _fileStream = null;
                }

                /* close handle */
                HIDSystem.CloseHandle(handle);
            }
            catch (Exception ex)
            {
                throw new Win32Exception(ex.ToString());
            }
        }

        /* write record */
        public void Write(byte[] data)
        {
            try
            {
                //var s = new StringBuilder(data.Length);
                //s.Append(data);
                //if (HIDSystem.HidD_SetFeature(handle, s, s.Capacity))
                //    s = null;
                if (_fileStream.CanWrite && data.Length == myHIDP_CAPS.OutputReportByteLength)
                {
                    /* write some bytes */
                    _fileStream.Write(data, 0, data.Length);
                    /* flush! */
                    _fileStream.Flush();
                }
            }
            catch (Exception ex)
            {
                throw new Win32Exception(ex.ToString());
            }
        }

        /* read record */
        public void Read(byte[] data)
        {
            try
            {
                //var s = new StringBuilder(data.Length);
                //if (HIDSystem.HidD_GetFeature(handle, s, s.Capacity))
                //    data = Encoding.UTF8.GetBytes(s.ToString());
                if (_fileStream.CanRead && data.Length == myHIDP_CAPS.InputReportByteLength)
                {
                    /* get number of bytes */
                    int n = 0, bytes = data.Length;

                    /* read buffer */
                    while (n != bytes)
                    {
                        /* read data */
                        int rc = _fileStream.Read(data, n, bytes - n);
                        /* update pointers */
                        n += rc;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Win32Exception(ex.ToString());
            }
        }
    }
}
