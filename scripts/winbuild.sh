#!/bin/bash
# Usage cd /Users/dev/src/Chameleon/scripts
# bash winbuild.sh 1

echo "[INFO] Building"
dotnet publish /Users/dev/src/Chameleon/Chameleon.Desktop/Chameleon.Desktop.csproj \
  -r win-x64 -c Release -f net8.0 -o /Users/dev/src/Chameleon/build/windows \
  --self-contained true \
  -p:DebugType=None \
  -p:DebugSymbols=false \
  -p:PublishSingleFile=true \
  -p:PublishReadyToRun=true \
  -p:IncludeNativeLibrariesForSelfExtract=true 

echo "[INFO] Packaging"
cd /Users/dev/src/Chameleon/build/windows
rm -rf Chameleon.7z
rm -rf Resources/scripts
rm -rf .playwright
rm -rf playwright.ps1
cp -a /Users/dev/src/Chameleon/Chameleon.Desktop/bin/Release/net8.0/win-x64/.playwright/. .playwright
rm -rf .playwright/node/darwin-x64
7z a Chameleon.7z && 7z d Chameleon.7z -r '*.DS_Store'

rm /Users/dev/src/Chameleon/publish/win/Chameleon.7z
cp Chameleon.7z /Users/dev/src/Chameleon/publish/win

# rm /Users/dev/Library/CloudStorage/OneDrive-EagleFusion/current/Chameleon.7z
# cp Chameleon.7z /Users/dev/Library/CloudStorage/OneDrive-EagleFusion/current