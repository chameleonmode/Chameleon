#!/bin/bash
# cd /Users/dev/src/Chameleon/scripts
# bash macbuild.sh 

rm -rf /Users/dev/src/Chameleon/build/osx
mkdir -p /Users/dev/src/Chameleon/build/osx

dotnet publish /Users/dev/src/Chameleon/Chameleon.Desktop/Chameleon.Avalonia.Desktop.csproj \
  --self-contained true -r osx-x64 -c Release -f net8.0 -o "/Users/dev/src/Chameleon/build/osx" \
  -p:DebugType=None \
  -p:DebugSymbols=false \
  -p:PublishSingleFile=true \
  -p:PublishReadyToRun=true \
  -p:IncludeNativeLibrariesForSelfExtract=false
