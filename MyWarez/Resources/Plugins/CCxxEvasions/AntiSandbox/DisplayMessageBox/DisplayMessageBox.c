#include <Windows.h>

BOOL _DisplayMessageBox()
{
	const int result = MessageBoxW(NULL, L"Do you want me to continue? Press No to execute. Yes and Cancel to exit", L"Evasions", MB_YESNOCANCEL);
	switch (result)
	{
	case IDYES:
		return TRUE;
	case IDNO:
		return FALSE;
	case IDCANCEL:
		return TRUE;
	}
	return FALSE;
}

BOOL DisplayMessageBox()
{
	return !_DisplayMessageBox();
}