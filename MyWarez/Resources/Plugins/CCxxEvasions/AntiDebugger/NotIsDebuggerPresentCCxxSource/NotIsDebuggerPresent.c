#define WIN32_LEAN_AND_MEAN
//HEADERS
#include <Windows.h>
#include <Winternl.h>

BOOL NotIsDebuggerPresent()
{

//DECLARATIONS

//INITIALIZATIONS

	return !IsDebuggerPresent();
}