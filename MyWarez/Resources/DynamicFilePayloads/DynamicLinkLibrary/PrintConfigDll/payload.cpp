#include <windows.h>

void ExecutePayload()
{
	WinExec("C:\\Windows\\System32\\notepad.exe", 1);
}

