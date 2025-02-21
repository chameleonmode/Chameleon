#!/bin/bash
# cd /Users/dev/src/Chameleon/scripts
# bash macbuild.sh 

dotnet publish /Users/dev/src/Chameleon/Chameleon.Avalonia.Desktop/Chameleon.Avalonia.Desktop.csproj \
  -r osx-x64 -c Release -f net8.0 \
  --self-contained true \
  -p:DebugType=None \
  -p:DebugSymbols=false \
  -p:PublishSingleFile=true \
  -p:PublishReadyToRun=true \
  -p:IncludeNativeLibrariesForSelfExtract=false
