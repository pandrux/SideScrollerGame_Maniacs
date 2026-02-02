@echo off
setlocal

:: Check for input file
if "%~1"=="" (
    echo Usage: %~0 input.mp4
    exit /b
)

set INPUT=%~1
set BASENAME=%~n1

:: Create output folder
mkdir "%BASENAME%_frames" >nul 2>&1

:: Extract frames and apply chroma key (0x31f217)
C:\ffmpeg-8.0.1-essentials_build\bin\ffmpeg -i "%INPUT%" -vf "colorkey=0x31f217:0.3:0.1" "%BASENAME%_frames\frame_%%04d.png"

echo Done. Frames saved in "%BASENAME%_frames"
endlocal