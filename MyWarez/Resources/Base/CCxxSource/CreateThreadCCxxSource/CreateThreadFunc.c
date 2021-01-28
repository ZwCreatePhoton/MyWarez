//HEADERS
#define WIN32_LEAN_AND_MEAN
#include <Windows.h>

void ThreadFunction(void);

void CreateThreadFunc(void)
{
//DECLARATIONS

//INITIALIZATIONS

    CloseHandle(
        CreateThread(NULL, 0, (LPTHREAD_START_ROUTINE)ThreadFunction, NULL, 0, NULL)
    );
}