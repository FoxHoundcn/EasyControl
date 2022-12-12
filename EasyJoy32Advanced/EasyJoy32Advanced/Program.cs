using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EasyControl
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            bool runone;
            System.Threading.Mutex run = new System.Threading.Mutex(true, "EasyControl", out runone);
            if (runone)
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                new MainForm();
            }
            else
            {
                MessageBox.Show("EasyControl is running.");
            }
        }
    }
}
