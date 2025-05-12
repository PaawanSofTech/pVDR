using System;
using System.Runtime.InteropServices;
using System.Windows;

namespace P2P_VDR_App
{
    public partial class App : Application
    {
        [DllImport("kernel32.dll")]
        public static extern bool AllocConsole();

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            AllocConsole(); // This creates a console window for logging
        }
    }
}
