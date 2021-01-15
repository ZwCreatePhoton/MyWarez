#include <Windows.h>

BOOL SleepLoop()
{
	for (int i = 0; i < 1000; i++)
	{
		Sleep(60);
	}
	return TRUE;
}