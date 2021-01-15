#include <winsock2.h>
#include <Windows.h>

// TODO: Copy more signatures



// Useful for typing Win32 API functions here so that you can right click and peek at function definitions
static void scratch()
{
}


#pragma region Ntdll.dll

// Ntdll.dll!NtTerminateProcess
typedef NTSTATUS(NTAPI* FuncNtTerminateProcess) (
	_In_opt_ HANDLE ProcessHandle,
	_In_ NTSTATUS ExitStatus
	);

// Ntdll.dll!NtAllocateVirtualMemory
typedef NTSTATUS(NTAPI* FuncNtAllocateVirtualMemory) (
	_In_ HANDLE ProcessHandle,
	_Inout_ _At_(*BaseAddress, _Readable_bytes_(*RegionSize) _Writable_bytes_(*RegionSize) _Post_readable_byte_size_(*RegionSize)) PVOID* BaseAddress,
	_In_ ULONG_PTR ZeroBits,
	_Inout_ PSIZE_T RegionSize,
	_In_ ULONG AllocationType,
	_In_ ULONG Protect
);

// Ntdll.dll!NtReadVirtualMemory
typedef NTSTATUS(NTAPI* FuncNtReadVirtualMemory) (
	_In_ HANDLE ProcessHandle,
	_In_opt_ PVOID BaseAddress,
	_Out_writes_bytes_(BufferSize) PVOID Buffer,
	_In_ SIZE_T BufferSize,
	_Out_opt_ PSIZE_T NumberOfBytesRead
);

// Ntdll.dll!NtWriteVirtualMemory
typedef NTSTATUS(NTAPI* FuncNtWriteVirtualMemory) (
	_In_ HANDLE ProcessHandle,
	_In_opt_ PVOID BaseAddress,
	_In_reads_bytes_(BufferSize) PVOID Buffer,
	_In_ SIZE_T BufferSize,
	_Out_opt_ PSIZE_T NumberOfBytesWritten
);

// Ntdll.dll!NtUnmapViewOfSection
typedef NTSTATUS(NTAPI* FuncNtUnmapViewOfSection) (
	_In_ HANDLE ProcessHandle,
	_In_opt_ PVOID BaseAddress
);

// Ntdll.dll!NtResumeThread
typedef NTSTATUS(NTAPI* FuncNtResumeThread) (
	_In_ HANDLE ThreadHandle,
	_Out_opt_ PULONG PreviousSuspendCount
);

// Ntdll.dll!NtGetContextThread
typedef NTSTATUS(NTAPI* FuncNtGetContextThread) (
	_In_ HANDLE ThreadHandle,
	_Inout_ PCONTEXT ThreadContext
);

// Ntdll.dll!NtSetContextThread
typedef NTSTATUS(NTAPI* FuncNtSetContextThread) (
	_In_ HANDLE ThreadHandle,
	_In_ PCONTEXT ThreadContext
);

// Ntdll.dll!NtClose
typedef NTSTATUS(NTAPI* FuncNtClose) (
	_In_ _Post_ptr_invalid_ HANDLE Handle
);

#pragma endregion


#pragma region Kernel32.dll

// Kernel32.dll!LoadLibraryA
typedef HMODULE(WINAPI* FuncLoadLibraryA) (
	_In_ LPCSTR lpLibFileName
);

// Kernel32.dll!CreateProcessA
typedef BOOL(WINAPI* FuncCreateProcessA) (
	_In_opt_ LPCSTR lpApplicationName,
	_Inout_opt_ LPSTR lpCommandLine,
	_In_opt_ LPSECURITY_ATTRIBUTES lpProcessAttributes,
	_In_opt_ LPSECURITY_ATTRIBUTES lpThreadAttributes,
	_In_ BOOL bInheritHandles,
	_In_ DWORD dwCreationFlags,
	_In_opt_ LPVOID lpEnvironment,
	_In_opt_ LPCSTR lpCurrentDirectory,
	_In_ LPSTARTUPINFOA lpStartupInfo,
	_Out_ LPPROCESS_INFORMATION lpProcessInformation
);

// Kernel32.dll!WriteFile
typedef BOOL(WINAPI* FuncWriteFile) (
	_In_ HANDLE hFile,
	_In_reads_bytes_opt_(nNumberOfBytesToWrite) LPCVOID lpBuffer,
	_In_ DWORD nNumberOfBytesToWrite,
	_Out_opt_ LPDWORD lpNumberOfBytesWritten,
	_Inout_opt_ LPOVERLAPPED lpOverlapped
);

// Kernel32.dll!CreateFileW
typedef HANDLE(WINAPI* FuncCreateFileW) (
	_In_ LPCWSTR lpFileName,
	_In_ DWORD dwDesiredAccess,
	_In_ DWORD dwShareMode,
	_In_opt_ LPSECURITY_ATTRIBUTES lpSecurityAttributes,
	_In_ DWORD dwCreationDisposition,
	_In_ DWORD dwFlagsAndAttributes,
	_In_opt_ HANDLE hTemplateFile
);

// Kernel32.dll!CloseHandle
typedef BOOL(WINAPI* FuncCloseHandle) (
	_In_ _Post_ptr_invalid_ HANDLE hObject
);

// Kernel32.dll!CreateMutexExW
typedef HANDLE(WINAPI* FuncCreateMutexExW) (
	_In_opt_ LPSECURITY_ATTRIBUTES lpMutexAttributes,
	_In_opt_ LPCWSTR lpName,
	_In_ DWORD dwFlags,
	_In_ DWORD dwDesiredAccess
);

// Kernel32.dll!GetLastError
typedef DWORD(WINAPI* FuncGetLastError) (
	VOID
);

// Kernel32.dll!IsDebuggerPresent
typedef BOOL(WINAPI* FuncIsDebuggerPresent) (
	VOID
);

// Kernel32.dll!CheckRemoteDebuggerPresent
typedef BOOL(WINAPI* FuncCheckRemoteDebuggerPresent) (
	_In_ HANDLE hProcess,
	_Out_ PBOOL pbDebuggerPresent
);

// Kernel32.dll!GetCurrentProcess
typedef HANDLE(WINAPI* FuncGetCurrentProcess) (
	VOID
);

// Kernel32.dll!CreateThread
typedef HANDLE(WINAPI* FuncCreateThread) (
	_In_opt_ LPSECURITY_ATTRIBUTES lpThreadAttributes,
	_In_ SIZE_T dwStackSize,
	_In_ LPTHREAD_START_ROUTINE lpStartAddress,
	_In_opt_ __drv_aliasesMem LPVOID lpParameter,
	_In_ DWORD dwCreationFlags,
	_Out_opt_ LPDWORD lpThreadId
);

// Kernel32.dll!WaitForSingleObject
typedef DWORD(WINAPI* FuncWaitForSingleObject) (
	_In_ HANDLE hHandle,
	_In_ DWORD dwMilliseconds
);

#pragma endregion

// Ws2_32.dll

#pragma region Ws2_32.dll

// Ws2_32.dll!WSAStartup
typedef int(PASCAL FAR* FuncWSAStartup) (
	_In_ WORD wVersionRequired,
	_Out_ LPWSADATA lpWSAData
	);

// Ws2_32.dll!WSACleanup
typedef int(PASCAL FAR* FuncWSACleanup) (
	void
	);

// Ws2_32.dll!WSASocketW
typedef SOCKET(WSAAPI* FuncWSASocketW) (
	_In_ int af,
	_In_ int type,
	_In_ int protocol,
	_In_opt_ LPWSAPROTOCOL_INFOW lpProtocolInfo,
	_In_ GROUP g,
	_In_ DWORD dwFlags
	);

// Ws2_32.dll!getaddrinfo
typedef INT(WSAAPI* Funcgetaddrinfo) (
	_In_opt_        PCSTR               pNodeName,
	_In_opt_        PCSTR               pServiceName,
	_In_opt_        const ADDRINFOA* pHints,
	_Outptr_        PADDRINFOA* ppResult
	);

// Ws2_32.dll!connect
typedef int(WSAAPI* Funcconnect) (
	_In_ SOCKET s,
	_In_reads_bytes_(namelen) const struct sockaddr FAR* name,
	_In_ int namelen
	);

#pragma endregion
