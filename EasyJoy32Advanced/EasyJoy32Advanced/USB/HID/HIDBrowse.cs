using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Text;

namespace FoxH.HID
{
    public class HIDBrowse
    {
        /* browse all HID class devices */
        public static List<HIDInfo> Browse()
        {
            try
            {
                /* hid device class guid */
                Guid gHid;
                /* list of device information */
                List<HIDInfo> info = new List<HIDInfo>();

                /* obtain hid guid */
                HIDSystem.HidD_GetHidGuid(out gHid);
                /* get list of present hid devices */
                var hInfoSet = HIDSystem.SetupDiGetClassDevs(ref gHid, null, IntPtr.Zero,
                               HIDSystem.DIGCF_DEVICEINTERFACE | HIDSystem.DIGCF_PRESENT);

                /* allocate mem for interface descriptor */
                var iface = new HIDSystem.DeviceInterfaceData();
                /* set size field */
                iface.Size = Marshal.SizeOf(iface);
                /* interface index */
                uint index = 0;

                /* iterate through all interfaces */
                while (HIDSystem.SetupDiEnumDeviceInterfaces(hInfoSet, 0, ref gHid,
                        index, ref iface))
                {

                    /* vid and pid */
                    short vid, pid;

                    /* get device path */
                    var path = GetPath(hInfoSet, ref iface);

                    /* open device */
                    var handle = Open(path);
                    /* device is opened? */
                    if (handle != HIDSystem.INVALID_HANDLE_VALUE)
                    {
                        /* get device manufacturer string */
                        var man = GetManufacturer(handle);
                        /* get product string */
                        var prod = GetProduct(handle);
                        /* get serial number */
                        var serial = GetSerialNumber(handle);
                        /* get vid and pid */
                        GetVidPid(handle, out vid, out pid);

                        /* build up a new element */
                        HIDInfo i = new HIDInfo(prod, serial, man, path, vid, pid);
                        /* add to list */
                        info.Add(i);

                        /* close */
                        Close(handle);
                    }

                    /* next, please */
                    index++;
                }

                /* clean up */
                if (HIDSystem.SetupDiDestroyDeviceInfoList(hInfoSet) == false)
                {
                    /* fail! */
                    throw new Win32Exception();
                }

                /* return list */
                return info;
            }
            catch (Exception ex)
            {
                throw new Win32Exception(ex.ToString());
            }
        }

        /* open device */
        private static IntPtr Open(string path)
        {
            try
            {
                /* opens hid device file */
                return HIDSystem.CreateFile(path,
                                            HIDSystem.GENERIC_READ | HIDSystem.GENERIC_WRITE,
                                            HIDSystem.FILE_SHARE_READ | HIDSystem.FILE_SHARE_WRITE,
                                            IntPtr.Zero, HIDSystem.OPEN_EXISTING, HIDSystem.FILE_FLAG_OVERLAPPED,
                                            IntPtr.Zero);
            }
            catch (Exception ex)
            {
                throw new Win32Exception(ex.ToString());
            }
        }

        /* close device */
        private static void Close(IntPtr handle)
        {
            try
            {
                /* try to close handle */
                if (HIDSystem.CloseHandle(handle) == false)
                {
                    /* fail! */
                    throw new Win32Exception();
                }
            }
            catch (Exception ex)
            {
                throw new Win32Exception(ex.ToString());
            }
        }

        /* get device path */
        private static string GetPath(IntPtr hInfoSet,
                                      ref HIDSystem.DeviceInterfaceData iface)
        {
            try
            {
                /* detailed interface information */
                var detIface = new HIDSystem.DeviceInterfaceDetailData();
                /* required size */
                uint reqSize = (uint)Marshal.SizeOf(detIface);

                /* set size. The cbSize member always contains the size of the
                 * fixed part of the data structure, not a size reflecting the
                 * variable-length string at the end. */
                /* now stay with me and look at that x64/x86 maddness! */
                detIface.Size = Marshal.SizeOf(typeof(IntPtr)) == 8 ? 8 : 5;

                /* get device path */
                bool status = HIDSystem.SetupDiGetDeviceInterfaceDetail(hInfoSet,
                              ref iface, ref detIface, reqSize, ref reqSize, IntPtr.Zero);

                /* whops */
                if (!status)
                {
                    /* fail! */
                    throw new Win32Exception();
                }

                /* return device path */
                return detIface.DevicePath;
            }
            catch (Exception ex)
            {
                throw new Win32Exception(ex.ToString());
            }
        }

        /* get device manufacturer string */
        private static string GetManufacturer(IntPtr handle)
        {
            try
            {
                /* buffer */
                var s = new StringBuilder(256);
                /* returned string */
                string rc = String.Empty;

                /* get string */
                if (HIDSystem.HidD_GetManufacturerString(handle, s, s.Capacity))
                {
                    rc = s.ToString();
                }

                /* report string */
                return rc;
            }
            catch (Exception ex)
            {
                throw new Win32Exception(ex.ToString());
            }
        }

        /* get device product string */
        private static string GetProduct(IntPtr handle)
        {
            try
            {
                /* buffer */
                var s = new StringBuilder(256);
                /* returned string */
                string rc = String.Empty;

                /* get string */
                if (HIDSystem.HidD_GetProductString(handle, s, s.Capacity))
                {
                    rc = s.ToString();
                }

                /* report string */
                return rc;
            }
            catch (Exception ex)
            {
                throw new Win32Exception(ex.ToString());
            }
        }

        /* get device product string */
        private static string GetSerialNumber(IntPtr handle)
        {
            try
            {
                /* buffer */
                var s = new StringBuilder(256);
                /* returned string */
                string rc = String.Empty;

                /* get string */
                if (HIDSystem.HidD_GetSerialNumberString(handle, s, s.Capacity))
                {
                    rc = s.ToString();
                }

                /* report string */
                return rc;
            }
            catch (Exception ex)
            {
                throw new Win32Exception(ex.ToString());
            }
        }

        /* get vid and pid */
        private static void GetVidPid(IntPtr handle, out short Vid, out short Pid)
        {
            try
            {
                /* attributes structure */
                var attr = new HIDSystem.HiddAttributtes();
                /* set size */
                attr.Size = Marshal.SizeOf(attr);

                /* get attributes */
                if (HIDSystem.HidD_GetAttributes(handle, ref attr) == false)
                {
                    Vid = short.MaxValue;
                    Pid = short.MaxValue;
                    return;
                    /* fail! */
                    //throw new Win32Exception();
                }

                /* update vid and pid */
                Vid = attr.VendorID;
                Pid = attr.ProductID;
            }
            catch (Exception ex)
            {
                throw new Win32Exception(ex.ToString());
            }
        }
    }
}
