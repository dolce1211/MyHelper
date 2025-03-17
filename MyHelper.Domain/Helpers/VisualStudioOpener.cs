using System;
using EnvDTE;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Windows.Forms;
using System.Runtime.Versioning;
using System.Security;

namespace MyHelper.Domain.Helpers
{

    public class VisualStudioOpener
    {
        /// <summary>
        /// 引数filePathで指定した.csファイルの指定行数にジャンプします。
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="lineNumber"></param>
        public static bool OpenFileAndGoToLine(string filePath, int lineNumber, string solutionName = "")
        {
            DTE? DTE = GetVisualStudioInstance();
            if (DTE == null)
            {
                MessageBox.Show("Visual Studioがインストールされているか確認してください");
                return false;
            }

            try
            {
                nint hwnd = new nint(DTE.MainWindow.HWnd);
                hwnd.BringWindowToFront();
            }
            catch (Exception)
            {
                //気まぐれにエラーが出ることがあるので握りつぶしておく
            }



            for (int index = 1; index <= 3; index++)
            {
                try
                {
                    DTE.ItemOperations.OpenFile(filePath);
                    TextSelection selection = (TextSelection)DTE.ActiveDocument.Selection;
                    selection.GotoLine(lineNumber, true);
                    break;
                }
                catch (Exception)
                {
                    //気まぐれに失敗するので三回までリトライ
                    System.Threading.Thread.Sleep(100);
                }
            }
            return true;
        }

        private static DTE? GetVisualStudioInstance()
        {
            try
            {
                // Marshal.GetActiveObjectで実行中のVisual Studioにアタッチ
                return (DTE)Marshal2.GetActiveObject("VisualStudio.DTE");
            }
            catch (COMException)
            {
                //たぶんVSがインストールされてない
                return null;
            }
        }
    }


    /// <summary>
    /// Martial.GetActiveObjectは.netFrameworkでは有効だが.NET Coreでは使えないため、代替処理を行う
    /// https://stackoverflow.com/questions/58010510/no-definition-found-for-getactiveobject-from-system-runtime-interopservices-mars
    /// </summary>
    internal static class Marshal2
    {
        internal const string OLEAUT32 = "oleaut32.dll";
        internal const string OLE32 = "ole32.dll";

        [SecurityCritical]  // auto-generated_required
        public static object GetActiveObject(string progID)
        {
            object? obj = null;
            Guid clsid;

            // Call CLSIDFromProgIDEx first then fall back on CLSIDFromProgID if
            // CLSIDFromProgIDEx doesn't exist.
            try
            {
                CLSIDFromProgIDEx(progID, out clsid);
            }
            //            catch
            catch (Exception)
            {
                CLSIDFromProgID(progID, out clsid);
            }

            GetActiveObject(ref clsid, nint.Zero, out obj);
            return obj;
        }

        //[DllImport(Microsoft.Win32.Win32Native.OLE32, PreserveSig = false)]
        [DllImport(OLE32, PreserveSig = false)]
        [ResourceExposure(ResourceScope.None)]
        [SuppressUnmanagedCodeSecurity]
        [SecurityCritical]  // auto-generated
        private static extern void CLSIDFromProgIDEx([MarshalAs(UnmanagedType.LPWStr)] string progId, out Guid clsid);

        //[DllImport(Microsoft.Win32.Win32Native.OLE32, PreserveSig = false)]
        [DllImport(OLE32, PreserveSig = false)]
        [ResourceExposure(ResourceScope.None)]
        [SuppressUnmanagedCodeSecurity]
        [SecurityCritical]  // auto-generated
        private static extern void CLSIDFromProgID([MarshalAs(UnmanagedType.LPWStr)] string progId, out Guid clsid);

        //[DllImport(Microsoft.Win32.Win32Native.OLEAUT32, PreserveSig = false)]
        [DllImport(OLEAUT32, PreserveSig = false)]
        [ResourceExposure(ResourceScope.None)]
        [SuppressUnmanagedCodeSecurity]
        [SecurityCritical]  // auto-generated
        private static extern void GetActiveObject(ref Guid rclsid, nint reserved, [MarshalAs(UnmanagedType.Interface)] out object ppunk);

    }
}