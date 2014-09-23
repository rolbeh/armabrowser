
REM http://msdn.microsoft.com/de-de/library/ms788718(v=vs.110).aspx
copy LocBaml.exe bin\Debug /Y
cd bin\Debug

LocBaml.exe /generate de-DE\ArmaServerBrowser.resources.dll  /trans:..\..\ArmaServerBrowser.resources.en-US.CSV /out: en-US /cul:en-US

delete LocBaml.exe
cd ..\..\