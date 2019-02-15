if "%windir%"=="" goto compile
if not exist MEGSV.LIB implib -c MEGSV.LIB %windir%\SYSTEM\MEGSV.DLL
:compile
bcc32 GSVDEMO.C MEGSV.LIB
