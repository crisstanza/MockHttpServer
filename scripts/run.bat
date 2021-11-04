@echo off
cls
setlocal

cd ..
.\MockHttpServer.exe -port 8910 -pongPath C:\Users\cneves\source\repos\MockHttpServer\MockHttpServer\html

endlocal
