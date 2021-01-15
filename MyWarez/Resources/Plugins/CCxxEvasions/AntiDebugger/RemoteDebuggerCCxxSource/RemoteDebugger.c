#define WIN32_LEAN_AND_MEAN
//HEADERS
#include <Windows.h>
#include <Winternl.h>

BOOL CheckRemoteDebuggerPresentAPI()
{
//DECLARATIONS

	BOOL bIsDbgPresent = FALSE;

//INITIALIZATIONS

	CheckRemoteDebuggerPresent(GetCurrentProcess(), &bIsDbgPresent);
	return bIsDbgPresent;
}

BOOL RemoteDebugger()
{
	return !CheckRemoteDebuggerPresentAPI();
}