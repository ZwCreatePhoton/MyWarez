//HEADERS
#define WIN32_LEAN_AND_MEAN
#include <Windows.h>

struct FileResource {
	unsigned long size;
	unsigned char* data;
};

struct FileResource* GetFileResource(struct FileResource* const destination);
wchar_t* GetTargetPathW(wchar_t* const destination);

void WriteFileResource(void)
{
//DECLARATIONS

	HANDLE hFile;
	DWORD dwBytesToWrite;
	DWORD dwBytesWritten;
	wchar_t lpFileName[1024];
	LPVOID lpBuffer;
	unsigned char data[99999];
	struct FileResource fileResource;

//INITIALIZATIONS

	fileResource.size = 0;
	fileResource.data = data;


	GetTargetPathW(lpFileName);
	GetFileResource(&fileResource);
	dwBytesToWrite = (DWORD)(fileResource.size);
	lpBuffer = (LPVOID)(fileResource.data);

	hFile = CreateFileW(
		lpFileName,				// path of file to write to
		GENERIC_WRITE,          // open for writing
		0,                      // do not share
		NULL,                   // default security
		CREATE_ALWAYS,          // create new file only
		FILE_ATTRIBUTE_NORMAL,  // normal file
		NULL);                  // no attr. template
	WriteFile(
		hFile,					// open file handle
		lpBuffer,				// start of data to write
		dwBytesToWrite,			// number of bytes to write
		&dwBytesWritten,		// number of bytes that were written
		NULL);					// no overlapped structure
	CloseHandle(hFile);
}