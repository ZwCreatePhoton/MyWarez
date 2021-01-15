call "C:\Program Files (x86)\Microsoft Visual Studio\2019\Community\VC\Auxiliary\Build\VCVARSALL.bat" x64 -vcvars_ver=14.16
devenv CVE_2019_1069.sln /Rebuild "Release|x64"
copy x64\Release\CVE_2019_1069.dll .