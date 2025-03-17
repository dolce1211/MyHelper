
``` csharp
using Dapper;
using Ez.DBA;
using MethodDecorator.Fody.Interfaces;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Constructor | AttributeTargets.Assembly | AttributeTargets.Module)]
internal class QueryPublishAttribute : Attribute, IMethodDecorator
{

    private DateTime _startTime;
    private QueryRecord? _queryRecord = default!;    
    public void Init(object instance, MethodBase method, object[] args)
    {
        //EDI2Helperに送信するクエリ情報を取得
        _queryRecord = QueryRecordFactory.CreateQueryRecord(args);
    }

    public void OnEntry()
    {
        _startTime = DateTime.Now;
    }

    public void OnException(Exception exception)
    {
    }

    public void OnExit()
    {
        //クエリ実行時間を計測
        var elapsedMilliseconds = (DateTime.Now - _startTime).TotalMilliseconds;

        //クエリ情報をEDI2Helperに送信する
        MessageSender.SendMessageTo("EDI2Helper",_queryRecord, elapsedMilliseconds);

    }
}

internal record QueryRecord(string Query,
                            string ParamString,
                            CallerMethodInfo? readerwriterMethod, //FSZDataReaderかFSZDataWriterのサブクラスの情報
                            CallerMethodInfo? pageCallerMethod // Blazorページ内の呼び元情報
                            )
{
    public string CreateMessage(double elapsedMilliseconds) => 
        $"{Query}#{ParamString}" +
        $"#{readerwriterMethod?.Method??""}#{readerwriterMethod?.FilePath??""}#{readerwriterMethod?.LineNumber}" +
        $"#{pageCallerMethod?.Method??""}#{pageCallerMethod?.FilePath??""}#{pageCallerMethod?.LineNumber}" +
        $"#{Convert.ToInt64(elapsedMilliseconds)}";

}

internal record CallerMethodInfo(string Method, string FilePath, int LineNumber);




internal static class QueryRecordFactory
{
    public static QueryRecord? CreateQueryRecord(object[] args)
    {
        //Item1:FSZDataReaderかFSZDataWriterのサブクラスの情報
        //Item2:Blazorページ内の呼び元情報
        var callerInfoTuple = GetCallerInfo();
        if (args.Length < 1)
            return null;

        string query = args[0] as string ?? "";

        string paramString = "";
        if (args.Length >= 2)
        {
            paramString = TryCreateParamString((object)args[1]);
        }
        return new QueryRecord(query, paramString, callerInfoTuple?.Item1,callerInfoTuple?.Item2);
    }

    private static (CallerMethodInfo, CallerMethodInfo)? GetCallerInfo()
    {
        CallerMethodInfo? readerwriterMethod = null;
        CallerMethodInfo? pageCallerMethod = null;

        var stackTrace = new StackTrace(true);
        if (stackTrace.FrameCount > 1)
        {
            var st = 4;//呼び出し元のフレームを取得
            for (int i = st; i < 99; i++)
            {
                if (stackTrace.FrameCount < i)
                    break;
                var frame = stackTrace.GetFrame(i); 
                if (frame != null)
                {
                    var method = frame.GetMethod();
                    string filePath = frame.GetFileName()??"";
                    var lineNumber = frame.GetFileLineNumber();
                    string methodName = method?.Name ?? "";

                    if (methodName=="MoveNext")
                    {
                        //asyncメソッドかTaskと思われる
                        methodName = ExtractOriginalMethodName(method?.DeclaringType?.FullName);
                    }
                    
                    if (i== st)
                    {
                        // 4:FSZDataReaderかFSZDataWriterのサブクラス
                        readerwriterMethod = new CallerMethodInfo(methodName, filePath, lineNumber);
                    }
                    else if (filePath.ToLower().Contains(@"\EzEDIPages\EzEDIPages".ToLower()))
                    {
                        //Blazorページ内の呼び元
                        pageCallerMethod = new CallerMethodInfo(methodName, filePath, lineNumber);
                    }
                }
                if (readerwriterMethod != null && pageCallerMethod != null)
                    break;
            }            
        }
        if(readerwriterMethod==null)
            readerwriterMethod = new CallerMethodInfo("", "", 0);
        if(pageCallerMethod == null)
            pageCallerMethod = new CallerMethodInfo("", "", 0);
        return (readerwriterMethod, pageCallerMethod);
    }
    private static string ExtractOriginalMethodName(string? declaringTypeFullName)
    {
        if (string.IsNullOrEmpty( declaringTypeFullName))
            return "";
        var match = System.Text.RegularExpressions.Regex.Match(declaringTypeFullName, @"<(.+)>d__\d+");
        return match.Success ? match.Groups[1].Value : declaringTypeFullName;
    }
    /// <summary>
    /// パラメタ情報を文字列に変換する。パラメタを使ってる時と使ってない時の基準が分からん…
    /// </summary>
    /// <param name="param"></param>
    /// <returns></returns>
    private static string TryCreateParamString(object param)
    {
        if (param == null)
            return "";
        if (param is DBParams dbp)
        {
            return string.Join("\r\n", dbp.GetParams().Select(x => $":{x.Name} = {x.Value}"));
        }
        if (param is DynamicParameters dp)
        {
            var paramList = new List<string>();
            foreach (var paramName in dp.ParameterNames)
            {
                var paramValue = dp.Get<object>(paramName);
                paramList.Add($":{paramName} = {paramValue}");
            }
            return string.Join("\r\n", paramList);
        }
        if (param is Dictionary<string, object> dic)
        {
            var sqlParam = new DynamicParameters(dic);
            return TryCreateParamString(sqlParam);
        }
        if (IsAnonymousType(param.GetType()))
        {
            var dicc = param.GetType().GetProperties().ToDictionary(p => p.Name, p => p.GetValue(param));
            var sqlParam = new DynamicParameters(dicc);
            return TryCreateParamString(sqlParam);
        }
        return "";
    }

    private static bool IsAnonymousType(Type type)
    {
        return type.IsGenericType
            && (type.Attributes & TypeAttributes.NotPublic) == TypeAttributes.NotPublic
            && (type.Name.Contains("AnonymousType") || type.Name.Contains("匿名型"));
    }
}

internal static class MessageSender
{
    private const int WM_COPYDATA = 0x004A;
    private const int WM_USER = 0x0400;
    private const int WM_CUSTOM_MESSAGE = WM_USER + 1;

    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    private static extern IntPtr FindWindow(string? lpClassName, string lpWindowName);

    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    private static extern bool SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    private static extern bool PostMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);
    [StructLayout(LayoutKind.Sequential)]
    public struct COPYDATASTRUCT
    {
        public IntPtr dwData;
        public int cbData;
        public IntPtr lpData;
    }

    /// <summary>
    /// 対象ウィンドウにメッセージを送信する
    /// </summary>
    /// <param name="windowName"></param>
    /// <param name="queryRecord"></param>
    /// <param name="elapsedMilliseconds"></param>
    /// <returns></returns>
    public static bool SendMessageTo(string windowName, QueryRecord? queryRecord, double elapsedMilliseconds)
    {
        if (queryRecord == null)
            return false;

        // 受け側のウィンドウタイトルを探す
        IntPtr hWnd = FindWindow(null, windowName);

        if (hWnd != IntPtr.Zero)
        {
            string message = queryRecord.CreateMessage(elapsedMilliseconds);
            int byteCount = (message.Length + 1) * 2; // Unicode文字列のバイト数を計算

            IntPtr lpData = Marshal.StringToHGlobalUni(message);
            COPYDATASTRUCT cds = new COPYDATASTRUCT
            {
                dwData = IntPtr.Zero,
                cbData = byteCount,
                lpData = lpData
            };

            IntPtr pCds = Marshal.AllocHGlobal(Marshal.SizeOf(cds));
            Marshal.StructureToPtr(cds, pCds, false);

            SendMessage(hWnd, WM_COPYDATA, IntPtr.Zero, pCds);

            Marshal.FreeHGlobal(lpData);
            Marshal.FreeHGlobal(pCds);
        }
        return true;
    }
}
```