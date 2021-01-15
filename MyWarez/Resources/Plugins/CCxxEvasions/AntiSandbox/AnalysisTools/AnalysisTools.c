#include <Windows.h>
#include <Winternl.h>
#include <TlHelp32.h>
#include <Shlwapi.h>
#include <tchar.h>

DWORD GetProcessIdFromName(LPCTSTR szProcessName)
{
	PROCESSENTRY32 pe32;
	HANDLE hSnapshot = NULL;
	SecureZeroMemory(&pe32, sizeof(PROCESSENTRY32));

	// We want a snapshot of processes
	hSnapshot = CreateToolhelp32Snapshot(TH32CS_SNAPPROCESS, 0);

	// Check for a valid handle, in this case we need to check for
	// INVALID_HANDLE_VALUE instead of NULL
	if (hSnapshot == INVALID_HANDLE_VALUE) {
		//print_last_error(_T("CreateToolhelp32Snapshot"));
		return 0;
	}

	// Now we can enumerate the running process, also 
	// we can't forget to set the PROCESSENTRY32.dwSize member
	// otherwise the following functions will fail
	pe32.dwSize = sizeof(PROCESSENTRY32);

	if (Process32First(hSnapshot, &pe32) == FALSE)
	{
		// Cleanup the mess
		//print_last_error(_T("Process32First"));
		CloseHandle(hSnapshot);
		return 0;
	}

	// Do our first comparison
	if (StrCmpI(pe32.szExeFile, szProcessName) == 0)
	{
		// Cleanup the mess
		CloseHandle(hSnapshot);
		return pe32.th32ProcessID;
	}

	// Most likely it won't match on the first try so 
	// we loop through the rest of the entries until
	// we find the matching entry or not one at all
	while (Process32Next(hSnapshot, &pe32))
	{
		if (StrCmpI(pe32.szExeFile, szProcessName) == 0)
		{
			// Cleanup the mess
			CloseHandle(hSnapshot);
			return pe32.th32ProcessID;
		}
	}

	// If we made it this far there wasn't a match, so we'll return 0
	// _tprintf(_T("\n-> Process %s is not running on this system ..."), szProcessName);

	CloseHandle(hSnapshot);
	return 0;
}

BOOL analysis_tools_process()
{
	TCHAR* szProcesses[] = {
		_T("ollydbg.exe"),			// OllyDebug debugger
		_T("ProcessHacker.exe"),	// Process Hacker
		_T("tcpview.exe"),			// Part of Sysinternals Suite
		_T("autoruns.exe"),			// Part of Sysinternals Suite
		_T("autorunsc.exe"),		// Part of Sysinternals Suite
		_T("filemon.exe"),			// Part of Sysinternals Suite
		_T("procmon.exe"),			// Part of Sysinternals Suite
		_T("procexp.exe"),			// Part of Sysinternals Suite
		_T("regmon.exe"),			// Part of Sysinternals Suite
		_T("procexp.exe"),			// Part of Sysinternals Suite
		_T("idaq.exe"),				// IDA Pro Interactive Disassembler
		_T("idaq64.exe"),			// IDA Pro Interactive Disassembler
		_T("ImmunityDebugger.exe"), // ImmunityDebugger
		_T("Wireshark.exe"),		// Wireshark packet sniffer
		_T("dumpcap.exe"),			// Network traffic dump tool
		_T("HookExplorer.exe"),		// Find various types of runtime hooks
		_T("ImportREC.exe"),		// Import Reconstructor
		_T("PETools.exe"),			// PE Tool
		_T("LordPE.exe"),			// LordPE
		_T("dumpcap.exe"),			// Network traffic dump tool
		_T("SysInspector.exe"),		// ESET SysInspector
		_T("proc_analyzer.exe"),	// Part of SysAnalyzer iDefense
		_T("sysAnalyzer.exe"),		// Part of SysAnalyzer iDefense
		_T("sniff_hit.exe"),		// Part of SysAnalyzer iDefense
		_T("windbg.exe"),			// Microsoft WinDbg
		_T("joeboxcontrol.exe"),	// Part of Joe Sandbox
		_T("joeboxserver.exe"),		// Part of Joe Sandbox
		_T("sandboxie.exe"),			// Part of Sysinternals Suite
	};

	WORD iLength = sizeof(szProcesses) / sizeof(szProcesses[0]);
	for (int i = 0; i < iLength; i++)
	{
		TCHAR msg[256] = _T("");
		//_stprintf_s(msg, sizeof(msg) / sizeof(TCHAR), _T("Checking process of malware analysis tool: %s: "), szProcesses[i]);
		if (GetProcessIdFromName(szProcesses[i]))
		{
			//print_results(TRUE, msg);
			return TRUE;
		}
		//else
			//print_results(FALSE, msg);
	}

	return FALSE;
}


BOOL AnalysisTools()
{
	return !analysis_tools_process();
}