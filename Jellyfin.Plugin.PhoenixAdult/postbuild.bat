setlocal enableextensions enabledelayedexpansion
echo off

cls

echo Assembyname: 		%1
echo Assembyversion:	%2

echo Configuration:		%3
echo OutDir:			%4
echo ProjectDir: 		%5
echo SolutionDir: 		%6
echo SolutionName: 		%7
echo TargetFileName: 	%8
echo TargetPath: 		%9

if %3%=="Debug" (
	echo "Jellyfin - Debug configuration"
	
	del /F %6..\jellyfin_10.9.11-amd64\jellyfin\jellyfin-data\plugins\%1_%2\%8
	mkdir %6..\jellyfin_10.9.11-amd64\jellyfin\jellyfin-data\plugins\%1_%2
	copy /Y %9 %6..\jellyfin_10.9.11-amd64\jellyfin\jellyfin-data\plugins\%1_%2\
	copy /Y %4%7.pdb %6..\jellyfin_10.9.11-amd64\jellyfin\jellyfin-data\plugins\%1_%2\
	goto end:
)

if %3%=="Release" (
	echo "Jellyfin - Release configuration"
	
 	del /F %6..\jellyfin_10.9.11-amd64\jellyfin\jellyfin-data\plugins\%1_%2\%8
	mkdir %6..\jellyfin_10.9.11-amd64\jellyfin\jellyfin-data\plugins\%1_%2
	copy /Y %9 %6..\jellyfin_10.9.11-amd64\jellyfin\jellyfin-data\plugins\%1_%2\
	copy /Y %4%7.pdb %6..\jellyfin_10.9.11-amd64\jellyfin\jellyfin-data\plugins\%1_%2\
	goto end:
)

if %3%=="Debug.Emby" (
	echo "Emby - Debug.Emby configuration"
 
	del /F %6..\embyserver-win-x64-4.8.10.0\programdata\plugins\%8
	copy /Y %9 %6..\embyserver-win-x64-4.8.10.0\programdata\plugins\
	copy /Y %4%7.pdb %6..\embyserver-win-x64-4.8.10.0\programdata\plugins\
	goto end:
)

if %3%=="Release.Emby" (
	echo "Emby - Release.Emby configuration"
	
	del /F %6..\embyserver-win-x64-4.8.10.0\programdata\plugins\%8
	copy /Y %9 %6..\embyserver-win-x64-4.8.10.0\programdata\plugins\
	copy /Y %4%7.pdb %6..\embyserver-win-x64-4.8.10.0\programdata\plugins\
	goto end:
)

echo "No configuration run"
echo "Configuration: **%3**"

:end

echo on
endlocal