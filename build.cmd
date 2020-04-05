@echo off

set appVersion=%1


if "%appVersion%" == "" (
    echo parametr is not specofied
    exit -1
)


if exist "src" (
  cd src
  dotnet build BackClient -c Release --output "%appVersion%"/BackClient/bin
  dotnet build BackendApi -c Release --output "%appVersion%"/BackendApi/bin
  dotnet build JobLogger -c Release --output "%appVersion%"/JobLogger/bin
  dotnet build TextRankCalc -c Release --output "%appVersion%"/TextRankCalc/bin
  mkdir "%appVersion%"/config
  
) else (
      echo src not found
       exit -1
)



copy %cd%\BackClient\config\appsettings.json %cd%\"%appVersion%"\config\appsettings.json

cd  %appVersion%


(echo. @echo off
echo start %cd%\BackClient\bin\BackClient.exe
echo start %cd%\BackendApi\bin\BackendApi.exe
echo start %cd%\JobLogger\bin\JobLogger.exe
echo start %cd%\TextRankCalc\bin\TextRankCalc.exe
)>start.cmd

(echo. @echo off
echo taskkill /IM BackClient.exe
echo taskkill /IM BackendApi.exe
echo taskkill /IM JobLogger.exe
echo taskkill /IM TextRankCalc.exe
)>stop.cmd



echo %appVersion%