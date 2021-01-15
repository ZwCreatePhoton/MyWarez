#include <Windows.h>

void ExecutePayload(void);

int WINAPI WinMain(
    HINSTANCE hInstance,
    HINSTANCE hPrevInstance,
    LPSTR    lpCmdLine,
    int       cmdShow)
{
    ExecutePayload();
    ExitThread(0);
}