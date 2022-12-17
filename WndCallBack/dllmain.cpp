// dllmain.cpp : 定义 DLL 应用程序的入口点。
#include "pch.h"
#define WM_USER_RESIZE 1025

extern "C" __declspec(dllexport) LRESULT ResizeWndCallBack(int nCode, INT_PTR wParam, INT_PTR lParam)
{
    tagCWPRETSTRUCT* wndStruct = (tagCWPRETSTRUCT*)lParam;
    if (wndStruct->message == WM_SIZE)
    {
        HWND commandReceiverHwnd = FindWindowExA(NULL, NULL, NULL, "MediaPlayerCommandReceiver");
        if (commandReceiverHwnd != NULL)
        {
            MessageBoxA(NULL, "Test", "Test", MB_OK);
            PostMessageA(commandReceiverHwnd, WM_USER_RESIZE, 0, 0);
        }
    }
    return 0;
}

BOOL APIENTRY DllMain( HMODULE hModule,
                       DWORD  ul_reason_for_call,
                       LPVOID lpReserved
                     )
{
    switch (ul_reason_for_call)
    {
    case DLL_PROCESS_ATTACH:
    case DLL_THREAD_ATTACH:
    case DLL_THREAD_DETACH:
    case DLL_PROCESS_DETACH:
        break;
    }
    return TRUE;
}

