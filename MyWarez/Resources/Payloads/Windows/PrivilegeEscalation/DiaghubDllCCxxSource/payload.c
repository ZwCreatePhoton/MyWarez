#include <windows.h>

void ExecutePayload(void)
{
	WinExec("C:\\Windows\\System32\\notepad.exe", 1);
}

