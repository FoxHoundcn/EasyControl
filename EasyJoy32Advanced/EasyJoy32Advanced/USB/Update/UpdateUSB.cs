using FoxH.HID;
using System;
using System.Collections.Generic;

namespace EasyControl
{
    public class UpdateUSB
    {
        //-------------------------------------------------------------
        public static UpdateObject updateJoy = null;
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////
        #region Init
        public static readonly UpdateUSB Instance = new UpdateUSB();
        private UpdateUSB()
        {
        }
        public void Init()
        {
        }
        #endregion
        public bool Refresh()
        {
            try
            {
                if (updateJoy != null && !updateJoy.loop)
                    updateJoy = null;
                List<HIDInfo> devs = HIDBrowse.Browse();
                /* display VID and PID for every device found */
                foreach (var dev in devs)
                {
                    if (dev.SerialNumber.Length == 32)
                    {
                        string ejoy = dev.SerialNumber.Substring(0, 4);
                        string ver = dev.SerialNumber.Substring(4, 3);
                        string index = dev.SerialNumber.Substring(7, 1);
                        string key = dev.SerialNumber.Substring(8);
                        if (ejoy.Equals("EJoy") &&
                            (ver.Equals("3xU") || ver.Equals("35U") || ver.Equals("41U")))
                        {
                            if (updateJoy == null)
                                updateJoy = new UpdateObject(ver, key, dev.Product);
                            if (updateJoy != null && !updateJoy.CheckJoy() && !updateJoy.RunUpdate)
                            {
                                updateJoy.OpenJoy(dev);
                            }
                        }
                    }
                }
                //-------------------------------------------------------------------------------
                if (updateJoy != null && updateJoy.CheckJoy() && !updateJoy.RunUpdate && updateJoy.loop)
                {
                    switch (updateJoy.Version)
                    {
                        case "3xU":
                        case "35U":
                            updateJoy.Open = true;
                            V3xUpdateForm.Instance.SelectUpdate(updateJoy);
                            break;
                        case "41U":
                            updateJoy.Open = true;
                            updateJoy.RunUpdate = true;
                            updateJoy.SelectVersion(V3xFirmware.v4b);
                            updateJoy.StartUpdate();
                            break;
                    }
                }
                if (Localization.Instance.GetHideWindows())
                {
                    if (Dx2D.Instance.mainCache != null)
                        Dx2D.Instance.mainCache.Hide();
                }
                return true;
            }
            catch (Exception ex)
            {
                DebugConstol.AddLog("ERROR : " + ex.ToString(), LogType.Error);
                return false;
            }
        }
    }
}
