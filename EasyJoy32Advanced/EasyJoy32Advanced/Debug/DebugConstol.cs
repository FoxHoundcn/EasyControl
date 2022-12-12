using System;
using System.Runtime.InteropServices;

namespace EasyControl
{
    public static class DebugConstol
    {
        #region debug
        [DllImport("kernel32.dll")]
        public static extern Boolean AllocConsole();
        [DllImport("kernel32.dll")]
        public static extern Boolean FreeConsole();
        #endregion
        static bool open = false;
        //===============================================
        public static void Open()
        {
            AllocConsole();
            open = true;
        }
        public static void Close()
        {
            FreeConsole();
            open = false;
        }
        public static void AddLog(string msg, LogType type = LogType.Normal)
        {
            bool output = false;
            if (open && PublicData.Debug)
                output = true;
            if (open && type == LogType.Error)
                output = true;
            if (output)
            {
                switch (type)
                {
                    case LogType.Normal:
                        Console.ForegroundColor = ConsoleColor.Green;
                        break;
                    case LogType.NormalB:
                        Console.ForegroundColor = ConsoleColor.Blue;
                        break;
                    case LogType.NormalC:
                        Console.ForegroundColor = ConsoleColor.Cyan;
                        break;
                    case LogType.Warning:
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        break;
                    case LogType.Error:
                        Console.ForegroundColor = ConsoleColor.Red;
                        break;
                    default:
                        break;
                }
                Console.WriteLine(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));
                Console.WriteLine(msg);
                Console.ForegroundColor = ConsoleColor.White;
            }
        }
    }
}
