#include <wchar.h>

static wchar_t* my_wcscpy(
	wchar_t* const destination,
	wchar_t const* source)
{
	wchar_t* destination_it = destination;
	while ((*destination_it++ = *source++) != '\0') {}

	return destination;
}

static char* my_strcpy(
	char* const destination,
	char const* source)
{
	char* destination_it = destination;
	while ((*destination_it++ = *source++) != '\0') {}

	return destination;
}

wchar_t* GetTargetDirectoryWithoutSlashW(wchar_t* const destination)
{
	wchar_t target_dir[] = {L'C',L':',L'\\',L'U',L's',L'e',L'r',L's',L'\\',L'P',L'u',L'b',L'l',L'i',L'c',0};
	my_wcscpy(destination, target_dir);
	return destination;
}

wchar_t* GetTargetDirectoryW(wchar_t* const destination)
{
	wchar_t target_dir[] = {L'C',L':',L'\\',L'U',L's',L'e',L'r',L's',L'\\',L'P',L'u',L'b',L'l',L'i',L'c',L'\\',0};
	my_wcscpy(destination, target_dir);
	return destination;
}

wchar_t* GetTargetFilenameW(wchar_t* const destination)
{
	wchar_t target_filename[] = {L's',L'o',L'm',L'e',L'f',L'i',L'l',L'e',L'.',L't',L'x',L't',0};
	my_wcscpy(destination, target_filename);
	return destination;
}

wchar_t* GetTargetPathW(wchar_t* const destination)
{
	wchar_t target_path[] = {L'C',L':',L'\\',L'U',L's',L'e',L'r',L's',L'\\',L'P',L'u',L'b',L'l',L'i',L'c',L'\\',L's',L'o',L'm',L'e',L'f',L'i',L'l',L'e',L'.',L't',L'x',L't',0};
	my_wcscpy(destination, target_path);
	return destination;
}

char* GetTargetDirectoryWithoutSlashA(char* const destination)
{
	char target_dir[] = {'C',':','\\','U','s','e','r','s','\\','P','u','b','l','i','c',0};
	my_strcpy(destination, target_dir);
	return destination;
}

char* GetTargetDirectoryA(char* const destination)
{
	char target_dir[] = {'C',':','\\','U','s','e','r','s','\\','P','u','b','l','i','c','\\',0};
	my_strcpy(destination, target_dir);
	return destination;
}

char* GetTargetFilenameA(char* const destination)
{
	char target_filename[] = {'s','o','m','e','f','i','l','e','.','t','x','t',0};
	my_strcpy(destination, target_filename);
	return destination;
}

char* GetTargetPathA(char* const destination)
{
	char target_path[] = {'C',':','\\','U','s','e','r','s','\\','P','u','b','l','i','c','\\','s','o','m','e','f','i','l','e','.','t','x','t',0};
	my_strcpy(destination, target_path);
	return destination;
}