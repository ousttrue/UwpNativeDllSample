# UwpNativeDllSample
UWP sample project that using native dll by VS2017.

# zlib build 

```
> set CMAKE="C:\Program Files (x86)\Microsoft Visual Studio\2017\Community\Common7\IDE\CommonExtensions\Microsoft\CMake\CMake\bin\cmake.exe"
> mkdir zlib-1.2.11\build
> cd zlib-1.2.11\build
zlib-1.2.11\build> %CMAKE% -G "Visual Studio 15 2017" -DCMAKE_SYSTEM_NAME=WindowsStore -DCMAKE_SYSTEM_VERSION=10.0 -DCMAKE_SYSTEM_PROCESSOR=x86 -DCMAKE_INSTALL_PREFIX=..\..\install .. 
zlib-1.2.11\build> set MSBUILD="C:\Program Files (x86)\Microsoft Visual Studio\2017\Community\MSBuild\15.0\Bin\MSBuild.exe"
zlib-1.2.11\build> %MSBUILD% Install.vcxproj /p:Configuration=Release
```

# libpng build

fix abort

* https://github.com/Microsoft/vcpkg/blob/master/docs/example-3-patch-libpng.md

```
> set CMAKE="C:\Program Files (x86)\Microsoft Visual Studio\2017\Community\Common7\IDE\CommonExtensions\Microsoft\CMake\CMake\bin\cmake.exe"
> mkdir libpng-1.6.29\build
> cd libpng-1.6.29\build
libpng-1.6.29\build> %CMAKE% -G "Visual Studio 15 2017" -DCMAKE_SYSTEM_NAME=WindowsStore -DCMAKE_SYSTEM_VERSION=10.0 -DCMAKE_SYSTEM_PROCESSOR=x86 -DZLIB_ROOT=..\..install -DCMAKE_INSTALL_PREFIX=..\..\install .. 
libpng-1.6.29\build> set MSBUILD="C:\Program Files (x86)\Microsoft Visual Studio\2017\Community\MSBuild\15.0\Bin\MSBuild.exe"
libpng-1.6.29\build> %MSBUILD% Install.vcxproj /p:Configuration=Release
```

