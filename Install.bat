cd %~dp0
mkdir output
cd output
copy ..\HostFileReplace\bin\Release\*.* .
copy ..\Monitor\bin\Release\*.* .

del *.vshost.*
del *.pdb

HostFileReplace.exe install
HostFileReplaceMonitor.exe install

HostFileReplace run
HostFileReplaceMonitor run
pause