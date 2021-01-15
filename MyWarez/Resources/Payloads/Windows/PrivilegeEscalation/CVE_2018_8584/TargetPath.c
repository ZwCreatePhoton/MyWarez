#include <string>


wchar_t* GetTargetDirectoryWithoutSlashW(void)
{
	wchar_t* target_dir = (wchar_t*)L"C:\\Windows\\System32";
	return target_dir;
}

wchar_t* GetTargetDirectoryW(void)
{
	wchar_t target_dir[1000] = L"";
	wcscat(target_dir, GetTargetDirectoryWithoutSlashW());
	wcscat(target_dir, L"\\");
	return target_dir;
}

wchar_t* GetTargetFilenameW(void)
{
	wchar_t* target_filename = (wchar_t*)L"License.rtf";
	return target_filename;
}

wchar_t* GetTargetPathW(void)
{
	wchar_t target_path[1000] = L"";
	wcscat(target_path, GetTargetDirectoryW());
	wcscat(target_path, GetTargetFilenameW());
	return target_path;
}