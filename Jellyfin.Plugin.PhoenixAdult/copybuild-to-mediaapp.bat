setlocal enableextensions enabledelayedexpansion
echo off

cls

for /f "delims== tokens=1,2" %%G in (build.txt) do set %%G=%%H

set ASSEMBLYNAME=%ASSEMBLYNAME:"=%
set ASSEMBLYVERSION=%ASSEMBLYVERSION:"=%
set CONFIGURATION=%CONFIGURATION:"=%
set OUTDIR=%OUTDIR:"=%
set PROJECTDIR=%PROJECTDIR:"=%
set SOLUTIONDIR=%SOLUTIONDIR:"=%
set SOLUTIONNAME=%SOLUTIONNAME:"=%
set TARGETFILENAME=%TARGETFILENAME:"=%
set TARGETPATH=%TARGETPATH:"=%

echo ASSEMBLYNAME 	-%ASSEMBLYNAME%-
echo ASSEMBYVERSION -%ASSEMBLYVERSION%-
echo CONFIGURATION 	-%CONFIGURATION%-
echo OUTDIR 		-%OUTDIR%-
echo PROJECTDIR   	-%PROJECTDIR%-
echo SOLUTIONDIR  	-%SOLUTIONDIR%-
echo SOLUTIONNAME 	-%SOLUTIONNAME%-
echo TARGETFILENAME -%TARGETFILENAME%-
echo TARGETPATH   	-%TARGETPATH%-

if %CONFIGURATION%==Debug (
	echo "Jellyfin - Debug configuration"
	
	del /F "%SOLUTIONDIR%..\jellyfin_10.9.11-amd64\jellyfin\jellyfin-data\plugins\%ASSEMBLYNAME%_%ASSEMBLYVERSION%\%TARGETFILENAME%"
	mkdir "%SOLUTIONDIR%..\jellyfin_10.9.11-amd64\jellyfin\jellyfin-data\plugins\%ASSEMBLYNAME%_%ASSEMBLYVERSION%"
	copy /Y "%TARGETPATH%" "%SOLUTIONDIR%..\jellyfin_10.9.11-amd64\jellyfin\jellyfin-data\plugins\%ASSEMBLYNAME%_%ASSEMBLYVERSION%\"
	copy /Y "%OUTDIR%%SOLUTIONNAME%.pdb" "%SOLUTIONDIR%..\jellyfin_10.9.11-amd64\jellyfin\jellyfin-data\plugins\%ASSEMBLYNAME%_%ASSEMBLYVERSION%\"
	goto end:
)

if %CONFIGURATION%==Release (
	echo "Jellyfin - Release configuration"
	
 	del /F "%SOLUTIONDIR%..\jellyfin_10.9.11-amd64\jellyfin\jellyfin-data\plugins\%ASSEMBLYNAME%_%ASSEMBLYVERSION\%TARGETFILENAME%"
	mkdir "%SOLUTIONDIR%..\jellyfin_10.9.11-amd64\jellyfin\jellyfin-data\plugins\%ASSEMBLYNAME%_%ASSEMBLYVERSION%"
	copy /Y "%TARGETPATH%" "%SOLUTIONDIR%..\jellyfin_10.9.11-amd64\jellyfin\jellyfin-data\plugins\%ASSEMBLYNAME%_%ASSEMBLYVERSION%\"
	copy /Y "%OUTDIR%%SOLUTIONNAME%.pdb" "%SOLUTIONDIR%..\jellyfin_10.9.11-amd64\jellyfin\jellyfin-data\plugins\%ASSEMBLYNAME%_%ASSEMBLYVERSION%\"
	goto end:
)

echo "ERROR: No configuration run"
echo "Configuration: "
echo %CONFIGURATION%

:end

echo "Finished"

SET /P M=Do you want to start the media server (y/n) press ENTER:
IF %M%==Y GOTO RUNMEDIA
IF %M%==y GOTO RUNMEDIA
IF %M%==n GOTO EOF
IF %M%==n GOTO EOF

:RUNMEDIA
"%SOLUTIONDIR%..\jellyfin_10.9.11-amd64\jellyfin\jellyfin.exe" --datadir="%SOLUTIONDIR%..\jellyfin_10.9.11-amd64\jellyfin\jellyfin-data"

:EOF

pause

echo on
endlocal