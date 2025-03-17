using MyHelper;
using MyHelper.Domain;
using System.Diagnostics;
using System.Runtime.InteropServices;
using MyHelper.Domain.Repositories;
using MyHelper.Infrastructure.Repositories;
using MyHelper.Infrastructure.MyHelperDbContext;
using System.Reflection;


namespace MyHelper
{
    internal static class Program
    {
        private static Mutex mutex = new Mutex(true, "{B1AFC9A1-8C3D-4D3A-9B1A-5C5A5A5A5A5A}");

        [DllImport("user32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern bool ShowWindowAsync(IntPtr hWnd, int nCmdShow);

        [DllImport("user32.dll")]
        private static extern bool IsIconic(IntPtr hWnd);

        private const int SW_RESTORE = 9;

        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            if (mutex.WaitOne(TimeSpan.Zero, true))
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                
                ApplicationConfiguration.Initialize();
                Application.Run(new frmMain());
                
                mutex.ReleaseMutex();
            }
            else
            {
                //多重起動させない    
                Process currentProcess = Process.GetCurrentProcess();
                Process? existingProcess = Process.GetProcessesByName(currentProcess.ProcessName)
                    .FirstOrDefault(p => p.Id != currentProcess.Id);

                if (existingProcess != null)
                {
                    //既に起動していたならば、そのウィンドウをアクティブにする
                    IntPtr hWnd = existingProcess.MainWindowHandle;
                    if (IsIconic(hWnd))
                    {
                        ShowWindowAsync(hWnd, SW_RESTORE);
                    }
                    SetForegroundWindow(hWnd);
                }
            }

        }

    }
}