#if defined(_WIN64)
void AlignRSP(void);
#else
void BeginExecutePayload(void);
#endif

void Begin(void)
{
#if defined(_WIN64)
	// Call the ASM stub that will guarantee 16-byte stack alignment.
	// The stub will then call the ExecutePayload.
	AlignRSP();
#else
	BeginExecutePayload();
#endif
}
