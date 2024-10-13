setlocal enableextensions enabledelayedexpansion
echo off

cls

echo Assembyname: 		%1
echo Assemblyversion:	%2
echo Configuration:		%3
echo OutDir:			%4
echo ProjectDir: 		%5
echo SolutionDir: 		%6
echo SolutionName: 		%7
echo TargetFileName: 	%8
echo TargetPath: 		%9

echo "Outputing the parameters to a build file - %4\build.txt"

echo ASSEMBLYNAME=%1>%5\build.txt
echo ASSEMBLYVERSION=%2>>%5\build.txt
echo CONFIGURATION=%3>>%5\build.txt
echo OUTDIR=%4>>%5\build.txt
echo PROJECTDIR=%5>>%5\build.txt
echo SOLUTIONDIR=%6>>%5\build.txt
echo SOLUTIONNAME=%7>>%5\build.txt
echo TARGETFILENAME=%8>>%5\build.txt
echo TARGETPATH=%9>>%5\build.txt

echo on
endlocal