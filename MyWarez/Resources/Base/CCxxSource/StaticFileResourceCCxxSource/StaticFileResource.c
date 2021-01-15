#include <stddef.h>

struct FileResource {
	unsigned long size;
	unsigned char* data;
};

static void* my_memcpy(void* dest, const void* src, size_t len)
{
	char* d = dest;
	const char* s = src;
	while (len--)
		*d++ = *s++;
	return dest;
}

struct FileResource* GetFileResource(struct FileResource* const destination)
{
	unsigned char data[] = {1,3,3,7,1,3,3,7,1,3,3,7,1,3,3,7};
	destination->size = 16;
	my_memcpy(destination->data, data, 16);
	return destination;
}
