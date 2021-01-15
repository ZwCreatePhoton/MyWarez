#include <Windows.h>
#include <Winternl.h>

static char* SANDBOX_WINDOW_NAMES[] = {
    "TEST",
    "ANALYZE",
    "SANDBOX",
    "MALWARE",
    "VIRUS"
};

static size_t SANDBOX_WINDOW_NAMES_SIZE = 5;

static int sandbox_window_count = 0;

static size_t strlength(const char* str)
{
    size_t len = 0;
    while (str[len]) len++;
    return len;
}

// modifies in place and returns
static char* upper(char* s) {
    size_t l = strlength(s);
    for (size_t i = 0; i < l; ++i) {
        if (s[i] >= 'a' && s[i] <= 'z') s[i] -= 32;
    }
    return s;
}

static int isSubstring(char* w1, char* w2)
{
    int i = 0;
    int j = 0;

    while (w1[i] != '\0') {
        if (w1[i] == w2[j])
        {
            int init = i;
            while (w1[i] == w2[j] && w2[j] != '\0')
            {
                j++;
                i++;
            }
            if (w2[j] == '\0') {
                return 1;
            }
            j = 0;
        }
        i++;
    }
    return 0;
}

static BOOL CALLBACK enumWindowCallback(HWND hWnd, LPARAM lparam)
{
    char classname[MAX_PATH] = { 0 };
    int chars_copied = GetClassNameA(hWnd, classname, MAX_PATH);

    for (int i = 0; i < SANDBOX_WINDOW_NAMES_SIZE; i++)
    {
        if (isSubstring(SANDBOX_WINDOW_NAMES[i], upper(classname)))
        {
            sandbox_window_count++;
            break;
        }
    }
    return TRUE;
}

BOOL EnumWindowsFunction()
{
    EnumWindows(enumWindowCallback, 0);
    return sandbox_window_count == 0;
}