#define WIN32_LEAN_AND_MEAN
//HEADERS
#include <Windows.h>

void ExecutePayload(void);

void MutexSingleton(void)
{
    // Only run at most 1 instance of the payload

//DECLARATIONS

    HANDLE m_hMutex;
    wchar_t mutexName[] = {L'G',L'l',L'o',L'b',L'a',L'l',L'\\',L'M',L'u',L't',L'e',L'x',L'S',L'i',L'n',L'g',L'l',L'e',L't',L'o',L'n',0};

//INITIALIZATIONS

    m_hMutex = CreateMutexExW(NULL, mutexName, 0, SYNCHRONIZE);
    if (!m_hMutex)
    {
        ; // can't access
    }
    else if (GetLastError() == ERROR_ALREADY_EXISTS)
    {
        ; // already running on this machine";
    }
    else
    {
        // no instance is running yet
        ExecutePayload();
    }
    CloseHandle(m_hMutex);
}