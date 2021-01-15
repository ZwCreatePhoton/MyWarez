call "C:\Program Files (x86)\Microsoft Visual Studio\2019\Community\VC\Auxiliary\Build\VCVARSALL.bat" x86 -vcvars_ver=14.16
devenv PrintConfigLoader.sln /Rebuild "Release|x86"
copy Release\PrintConfigLoader.dll .