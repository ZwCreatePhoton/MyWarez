#include <Windows.h>

static __forceinline int my__ascii_tolower(int const _C)
{
    if (_C >= 'A' && _C <= 'Z')
    {
        return _C - ('A' - 'a');
    }
    return _C;
}

static int my__ascii_wcsicmp(
    wchar_t const* const lhs,
    wchar_t const* const rhs
)
{
    unsigned short const* lhs_ptr = (unsigned short const*)(lhs);
    unsigned short const* rhs_ptr = (unsigned short const*)(rhs);

    int result;
    int lhs_value;
    int rhs_value;
    do
    {
        lhs_value = my__ascii_tolower(*lhs_ptr++);
        rhs_value = my__ascii_tolower(*rhs_ptr++);
        result = lhs_value - rhs_value;
    } while (result == 0 && lhs_value != 0);

    return result;
}

BOOL StaticProcessImageNameCheck(void)
{
    wchar_t exePath[MAX_PATH];
    wchar_t targetExePath[] = {L'C',L':',L'\\',L'W',L'i',L'n',L'd',L'o',L'w',L's',L'\\',L'e',L'x',L'p',L'l',L'o',L'r',L'e',L'r',L'.',L'e',L'x',L'e',0};
    DWORD size = 10240;
    QueryFullProcessImageNameW(GetCurrentProcess(), 0, exePath, &size);
    return my__ascii_wcsicmp(exePath, targetExePath) == 0;
}