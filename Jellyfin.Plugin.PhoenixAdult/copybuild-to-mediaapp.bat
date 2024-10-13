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
	
	echo del /F "%SOLUTIONDIR%..\jellyfin_10.9.11-amd64\jellyfin\jellyfin-data\plugins\%ASSEMBLYNAME%_%ASSEMBLYVERSION%\%TARGETFILENAME%"
	del /F "%SOLUTIONDIR%..\jellyfin_10.9.11-amd64\jellyfin\jellyfin-data\plugins\%ASSEMBLYNAME%_%ASSEMBLYVERSION%\%TARGETFILENAME%"
	
	echo mkdir "%SOLUTIONDIR%..\jellyfin_10.9.11-amd64\jellyfin\jellyfin-data\plugins\%ASSEMBLYNAME%_%ASSEMBLYVERSION%"
	mkdir "%SOLUTIONDIR%..\jellyfin_10.9.11-amd64\jellyfin\jellyfin-data\plugins\%ASSEMBLYNAME%_%ASSEMBLYVERSION%"
	
	echo copy /Y "%TARGETPATH%" "%SOLUTIONDIR%..\jellyfin_10.9.11-amd64\jellyfin\jellyfin-data\plugins\%ASSEMBLYNAME%_%ASSEMBLYVERSION%\"
	copy /Y "%TARGETPATH%" "%SOLUTIONDIR%..\jellyfin_10.9.11-amd64\jellyfin\jellyfin-data\plugins\%ASSEMBLYNAME%_%ASSEMBLYVERSION%\"
	
	echo copy /Y "%OUTDIR%%SOLUTIONNAME%.pdb" "%SOLUTIONDIR%..\jellyfin_10.9.11-amd64\jellyfin\jellyfin-data\plugins\%ASSEMBLYNAME%_%ASSEMBLYVERSION%\"
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

if %CONFIGURATION%==Debug.Emby (
	echo "Emby - Debug.Emby configuration"
 
	del /F "%SOLUTIONDIR%..\embyserver-win-x64-4.8.10.0\programdata\plugins\%TARGETFILENAME%"
	copy /Y "%TARGETPATH%" "%SOLUTIONDIR%..\embyserver-win-x64-4.8.10.0\programdata\plugins\"
	copy /Y "%OUTDIR%%SOLUTIONNAME%.pdb" "%SOLUTIONDIR%..\embyserver-win-x64-4.8.10.0\programdata\plugins\"
	goto end:
)

if %CONFIGURATION%==Release.Emby (
	echo "Emby - Release.Emby configuration"
	
	del /F "%SOLUTIONDIR%..\embyserver-win-x64-4.8.10.0\programdata\plugins\%TARGETFILENAME%"
	copy /Y "%TARGETPATH%" "%SOLUTIONDIR%..\embyserver-win-x64-4.8.10.0\programdata\plugins\"
	copy /Y "%OUTDIR%%SOLUTIONNAME%.pdb" "%SOLUTIONDIR%..\embyserver-win-x64-4.8.10.0\programdata\plugins\"
	goto end:
)

echo "ERROR: No configuration run"
echo "Configuration: "
echo %CONFIGURATION%

:end

pause

echo "Finished"

echo on
endlocal