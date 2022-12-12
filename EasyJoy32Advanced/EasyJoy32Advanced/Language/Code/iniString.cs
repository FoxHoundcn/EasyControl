using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace EasyControl
{
    public class iniString
    {
        public string Path;
        private string Section;
        [DllImport("kernel32")]
        private static extern long WritePrivateProfileString(string section, string key, string val, string filePath);
        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder retVal, int size, string filePath);

        public iniString(string _section, string path)
        {
            Path = path;
            Section = _section;
        }

        public void IniWriteValue(string Key, string Value)
        {
            WritePrivateProfileString(Section, Key, Value, Path);
        }

        public bool IniReadValue(string Key, out string Value)
        {
            StringBuilder temp = new StringBuilder(500);
            int i = GetPrivateProfileString(Section, Key, "", temp, 500, Path);
            Value = temp.ToString();
            return i > 0;
        }

        public bool ExistINIFile(string path)
        {
            string pathSave = Environment.CurrentDirectory + path;
            FileStream fs = File.Open(pathSave, FileMode.OpenOrCreate);
            if (fs != null)
                return true;
            else
                return false;
        }
    }
}
