#include <Windows.h>

BOOL AcceleratedSleep()
{
	DWORD time_initial;
	DWORD time_final;
	DWORD time_elapsed;
	time_initial = GetTickCount();
	Sleep(60000);
	time_final = GetTickCount();
	time_elapsed = time_final - time_initial;
	return time_elapsed > (60000 - 1000);
}