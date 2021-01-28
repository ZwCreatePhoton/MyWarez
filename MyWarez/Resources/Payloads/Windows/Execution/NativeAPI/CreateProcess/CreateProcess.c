//HEADERS
#define WIN32_LEAN_AND_MEAN
#include <windows.h>

VOID CreateProcessFunction(VOID)
{
//DECLARATIONS

	STARTUPINFOA StartupInfo;
	PROCESS_INFORMATION ProcessInformation;
	char cmdline[] = {'n','o','t','e','p','a','d',0};

//INITIALIZATIONS

	SecureZeroMemory(&StartupInfo, sizeof(StartupInfo));
	SecureZeroMemory(&ProcessInformation, sizeof(ProcessInformation));
	CreateProcessA(0, cmdline, 0, 0, TRUE, 0, 0, 0, &StartupInfo, &ProcessInformation);
}