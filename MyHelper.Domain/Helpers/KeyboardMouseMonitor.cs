using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MyHelper.Domain.Helpers;
public class KeyboardMouseMonitor
{
    /// <summary>
    /// Ctrl+Cが0.5秒以内に二回押されたら発火するイベント
    /// </summary>
    public event EventHandler? ControlCDoublePressed;

    /// <summary>
    /// 左右のマウスボタンが両方押されたら発火するイベント
    /// </summary>
    public event EventHandler? BothMouseButtonClicked;

    /// <summary>
    /// 左右のCtrlキーが両方押されたら発火するイベント
    /// </summary>
    public event EventHandler? BothControlKeyPressed;

    // Windows API の定義
    private delegate IntPtr HookProc(int nCode, IntPtr wParam, IntPtr lParam);

    [DllImport("user32.dll", SetLastError = true)]
    private static extern IntPtr SetWindowsHookEx(int idHook, HookProc lpfn, IntPtr hMod, uint dwThreadId);

    [DllImport("user32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool UnhookWindowsHookEx(IntPtr hhk);

    [DllImport("user32.dll", SetLastError = true)]
    private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

    [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern IntPtr GetModuleHandle(string lpModuleName);

    // マウスフックの定数
    private const int WH_MOUSE_LL = 14;

    // マウス押下チェック用
    private const int WM_LBUTTONDOWN = 0x0201;

    private const int WM_LBUTTONUP = 0x0202;
    private const int WM_RBUTTONDOWN = 0x0204;
    private const int WM_RBUTTONUP = 0x0205;

    // マウスフックのハンドル
    private IntPtr _mouseHook = IntPtr.Zero;

    // キーボードフックの定数
    private const int WH_KEYBOARD_LL = 13;

    // キーボード押下チェック用
    private const int WM_KEYDOWN = 0x0100;

    private const int WM_KEYUP = 0x0101;

    // キーボードフックのハンドル
    private IntPtr _keyboardHook = IntPtr.Zero;

    private HookProc hookProcDelegate;

    private bool _hookKeyboard = false;
    private bool _hookMouse = false;

    public KeyboardMouseMonitor()
    {
        hookProcDelegate = HookCallback;
    }

    /// <summary>
    /// フックの設定
    /// </summary>
    /// <param name="hookKeyboard"></param>
    /// <param name="hookMouse"></param>
    public void SetHook(bool hookKeyboard, bool hookMouse)
    {
        IntPtr hModule = GetModuleHandle(Process.GetCurrentProcess()!.MainModule!.ModuleName);

        Unhook();
        if (hookKeyboard)
        {
            _keyboardHook = SetWindowsHookEx(WH_KEYBOARD_LL, hookProcDelegate, hModule, 0);
        }
        if (hookMouse)
        {
            _mouseHook = SetWindowsHookEx(WH_MOUSE_LL, hookProcDelegate, hModule, 0);
        }

        _hookKeyboard = hookKeyboard;
        _hookMouse = hookMouse;
    }

    /// <summary>
    /// 一時的にフックを解除したものを再開させる
    /// </summary>
    public void ReSetHook()
    {
        SetHook(_hookKeyboard, _hookMouse);
    }

    /// <summary>
    /// フックの解除
    /// </summary>
    public void Unhook()
    {
        if (_keyboardHook != IntPtr.Zero)
        {
            UnhookWindowsHookEx(_keyboardHook);
            _keyboardHook = IntPtr.Zero;
        }

        if (_mouseHook != IntPtr.Zero)
        {
            UnhookWindowsHookEx(_mouseHook);
            _mouseHook = IntPtr.Zero;
        }
    }

    private bool _leftCtrlPressed = false;
    private bool _rightCtrlPressed = false;

    // フックのコールバック関数
    private IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
    {
        if (nCode >= 0)
        {
            switch (wParam.ToInt32())
            {
                case WM_KEYDOWN:
                    //キーボード押下
                    int vkCode = Marshal.ReadInt32(lParam);

                    // Ctrl + C の組み合わせを監視
                    if (Control.ModifierKeys == Keys.Control && vkCode == (int)Keys.C)
                    {
                        HandleCtrlCKeyPress();
                    }

                    // 左右のCtrlキーを追跡
                    if (vkCode == (int)Keys.LControlKey)
                    {
                        _leftCtrlPressed = true;
                    }
                    if (vkCode == (int)Keys.RControlKey)
                    {
                        _rightCtrlPressed = true;
                    }

                    // 左右のCtrlキーが同時に押されたかを確認
                    if (_leftCtrlPressed && _rightCtrlPressed)
                    {
                        // 左右のCtrlキーが同時に押されたときの処理
                        HandleBothCtrlKeysPress();
                    }
                    break;

                case WM_KEYUP:
                    // キーが離された場合、Ctrlキーの状態を更新
                    vkCode = Marshal.ReadInt32(lParam);

                    if (vkCode == (int)Keys.LControlKey)
                    {
                        _leftCtrlPressed = false;
                    }
                    if (vkCode == (int)Keys.RControlKey)
                    {
                        _rightCtrlPressed = false;
                    }
                    break;

                case WM_LBUTTONDOWN:
                case WM_RBUTTONDOWN:
                case WM_LBUTTONUP:
                case WM_RBUTTONUP:
                    Task.Run(() => HandleMouseClick(wParam));
                    break;
            }
        }
        return CallNextHookEx(_keyboardHook, nCode, wParam, lParam);
    }

    // Ctrl + C の組み合わせが押された場合の処理
    private DateTime _lastKeyPressTime = DateTime.MinValue;

    private int _keyPressCount = 0;

    /// <summary>
    /// ctrl+Cが押された
    /// </summary>
    private void HandleCtrlCKeyPress()
    {
        DateTime now = DateTime.Now;

        // 0.5秒以内のキー押下をカウント
        if ((now - _lastKeyPressTime).TotalSeconds <= 0.5)
        {
            _keyPressCount += 1;
        }
        else
        {
            _keyPressCount = 1;
        }

        _lastKeyPressTime = now;

        // 2回以上押された場合の処理を発火
        if (_keyPressCount == 2)
        {
            _keyPressCount = 0;
            ControlCDoublePressed?.Invoke(this, EventArgs.Empty);
        }
    }

    /// <summary>
    /// 左右のctrlキーが同時押しされた
    /// </summary>
    private void HandleBothCtrlKeysPress()
    {
        BothControlKeyPressed?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>
    /// マウスボタンを追跡するためのフラグ
    /// </summary>
    private bool _leftButtonPressed = false;

    private bool _rightButtonPressed = false;

    private void HandleMouseClick(IntPtr wParam)
    {
        //マウス押下
        // 左ボタンが押された場合
        if (wParam == (IntPtr)WM_LBUTTONDOWN)
        {
            _leftButtonPressed = true;
        }
        else if (wParam == (IntPtr)WM_LBUTTONUP)
        {
            _leftButtonPressed = false;
        }

        // 右ボタンが押された場合
        if (wParam == (IntPtr)WM_RBUTTONDOWN)
        {
            _rightButtonPressed = true;
        }
        else if (wParam == (IntPtr)WM_RBUTTONUP)
        {
            _rightButtonPressed = false;
        }
        if (_leftButtonPressed && _rightButtonPressed)
        {
            _leftButtonPressed = false;
            _rightButtonPressed = false;
            BothMouseButtonClicked?.Invoke(this, EventArgs.Empty);
        }
    }
}