#include <Windows.h>

BOOL MouseCursor()
{
	POINT point_initial;
	POINT point_final;
	BOOL mouse_moved;
	point_initial = { 0 };
	point_final = { 0 };
	GetCursorPos(&point_initial);
	Sleep(10 * 1000);
	point_final = { 0 };
	GetCursorPos(&point_final);
	mouse_moved = point_initial.x != point_final.x || point_initial.y != point_final.y;
	return mouse_moved;
}