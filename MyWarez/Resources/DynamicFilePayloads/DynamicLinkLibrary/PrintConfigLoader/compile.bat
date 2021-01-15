call "C:\Program Files (x86)\Microsoft Visual Studio\2019\Community\VC\Auxiliary\Build\VCVARSALL.bat" x64 -vcvars_ver=14.16
devenv PrintConfigLoader.sln /Rebuild "Release|x64"
copy x64\Release\PrintConfigLoader.dll .