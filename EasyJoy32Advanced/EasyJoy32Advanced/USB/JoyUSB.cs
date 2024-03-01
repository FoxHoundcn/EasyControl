using FoxH.HID;
using System;
using System.Collections.Generic;

namespace EasyControl
{
    public class JoyUSB
    {
        public string selectName = "";
        //-------------------------------------------------------------
        public static Dictionary<string, JoyObject> eJoyList { get; private set; }
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////
        #region Init
        public static readonly JoyUSB Instance = new JoyUSB();
        private JoyUSB()
        {
        }
        public void Init()
        {
            eJoyList = new Dictionary<string, JoyObject>();
        }
        #endregion
        public JoyObject GetJoyObjectAtIndex(int index)
        {
            List<JoyObject> joyList = GetJoyList();
            if (joyList.Count > 0 && index >= 0 && index < joyList.Count)
                return joyList[index];
            return null;
        }
        public List<JoyObject> GetJoyList()
        {
            List<JoyObject> templist = new List<JoyObject>();
            foreach (KeyValuePair<string, JoyObject> data in eJoyList)
            {
                templist.Add(data.Value);
            }
            templist.Sort();
            return templist;
        }
        public void CheckRefresh()
        {
            try
            {
                List<HIDInfo> devs = HIDBrowse.Browse();
                if (devs.Count != PublicData.devCount)
                {
                    Refresh();
                }
            }
            catch (Exception ex)
            {
                DebugConstol.AddLog("ERROR : " + ex.ToString(), LogType.Error);
            }
        }
        public bool Refresh()
        {
            try
            {
                DebugConstol.AddLog("List of USB HID devices:", LogType.NormalC);
                selectName = "";
                List<HIDInfo> devs = HIDBrowse.Browse();
                /* display VID and PID for every device found */
                foreach (var dev in devs)
                {
                    if (dev.SerialNumber.Length == 31)
                    {
                        string ejoy = dev.SerialNumber.Substring(0, 4);
                        string ver = dev.SerialNumber.Substring(4, 2);
                        string index = dev.SerialNumber.Substring(6, 1);
                        string key = dev.SerialNumber.Substring(7);
                        DebugConstol.AddLog("SerialNumber : " + ejoy + " - " + index + " - " + key);
                        if (ejoy.Equals("EJoy"))
                        {
                            if (!eJoyList.ContainsKey(key) && eJoyList.Count < JoyConst.MaxJoyObject)
                            {
                                eJoyList.Add(key, new JoyObject(eJoyList.Count, key, dev.Product, ver));
                            }
                            if (eJoyList[key] != null && !eJoyList[key].CheckJoy())
                            {
                                eJoyList[key].OpenJoy(dev);
                            }
                        }
                    }
                    else
                    {
                        bool check = true;
                        if (dev.SerialNumber.Length == 32)
                        {
                            string ejoyUpdate = dev.SerialNumber.Substring(0, 4);
                            string verUpdate = dev.SerialNumber.Substring(4, 3);
                            string indexUpdate = dev.SerialNumber.Substring(7, 1);
                            string keyUpdate = dev.SerialNumber.Substring(8);
                            if (ejoyUpdate.Equals("EJoy"))
                            {
                                check = false;
                            }
                        }
                        if (check && Localization.Instance.GetCheckUSB())
                        {
                            string key = dev.Vid.ToString() + "|" + dev.Pid.ToString();
                            int count = 0;
                            foreach (var devTemp in devs)
                            {
                                string keyTemp = devTemp.Vid.ToString() + "|" + devTemp.Pid.ToString();
                                if (keyTemp.Equals(key))
                                {
                                    count++;
                                }
                            }
                            if (!eJoyList.ContainsKey(key) && eJoyList.Count < JoyConst.MaxJoyObject)
                            {
                                switch (count)
                                {
                                    case 2:
                                        eJoyList.Add(key, new JoyObject(eJoyList.Count, key, dev.Product, "KB"));
                                        break;
                                    case 3:
                                        eJoyList.Add(key, new JoyObject(eJoyList.Count, key, dev.Product, "JOY"));
                                        break;
                                    default:
                                        eJoyList.Add(key, new JoyObject(eJoyList.Count, key, dev.Product, "unknown"));
                                        break;
                                }
                            }
                            if (eJoyList[key] != null && !eJoyList[key].CheckJoy())
                            {
                                eJoyList[key].OpenJoy(dev);
                            }
                        }
                    }
                }
                //-------------------------------------------------------------------------------
                List<string> errorList = new List<string>();
                foreach (KeyValuePair<string, JoyObject> data in eJoyList)
                {
                    if (data.Value.CheckJoy() && data.Value.loop)
                    {
                        DebugConstol.AddLog("SerialNumber : " + data.Key);
                    }
                    else
                    {
                        DebugConstol.AddLog("ERROR : " + data.Key, LogType.Error);
                        errorList.Add(data.Key);
                    }
                }
                for (int i = 0; i < errorList.Count; i++)
                {
                    eJoyList.Remove(errorList[i]);
                }
                DebugConstol.AddLog("EJoy count : " + eJoyList.Keys.Count, LogType.NormalC);
                //---------------------------------------------------------------------------------------------------------
                foreach (KeyValuePair<string, JoyObject> data in eJoyList)
                {
                    data.Value.Open = true;
                }
                PublicData.devCount = devs.Count;
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
