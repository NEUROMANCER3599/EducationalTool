using UnityEngine;
using System;
using System.Runtime.InteropServices;

public class AlwaysOnTopWindow : MonoBehaviour
{
    // ตัวแปรสำหรับฟังก์ชัน Win32 API
    private static readonly IntPtr HWND_TOPMOST = new IntPtr(-1);
    private const uint SWP_NOSIZE = 0x0001;
    private const uint SWP_NOMOVE = 0x0002;
    private const uint TOPMOST_FLAGS = SWP_NOMOVE | SWP_NOSIZE;

    [DllImport("user32.dll")]
    private static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

    [DllImport("user32.dll")]
    private static extern IntPtr GetActiveWindow();

    void Start()
    {
        // ตรวจสอบว่าเป็น Windows Standalone Build
#if UNITY_STANDALONE_WIN
        SetWindowToTopMost();
#endif
    }

    private void SetWindowToTopMost()
    {
        // 1. รับ Handle ของหน้าต่างเกมปัจจุบัน
        IntPtr currentWindowHandle = GetActiveWindow();

        // 2. ตั้งค่าให้หน้าต่างเป็น "Always On Top"
        // โดยการเรียกใช้ SetWindowPos
        SetWindowPos(currentWindowHandle, HWND_TOPMOST, 
            0, 0, 0, 0, 
            TOPMOST_FLAGS);

        Debug.Log("Window set to Always On Top.");
    }
}