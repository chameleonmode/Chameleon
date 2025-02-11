#!/bin/bash
# Usage cd /Users/dev/src/Chameleon/scripts or C:\repos\Chameleon\Chameleon.Avalonia.Desktop
# bash windowsbuild.sh
dotnet publish /Users/dev/src/Chameleon/Chameleon.Avalonia.Desktop/Chameleon.Avalonia.Desktop.csproj \
  -r win-x64 -c Release -f net8.0 -o "/Users/dev/src/Chameleon/build/windows" --self-contained true \
  -p:DebugType=None \
  -p:DebugSymbols=false \
  -p:PublishSingleFile=true \
  -p:IncludeNativeLibrariesForSelfExtract=true \
  -p:PublishReadyToRun=true 
  
rm -rf /Users/dev/src/Chameleon/build/windows/playwright.ps1
rm -rf /Users/dev/src/Chameleon/build/windows/.playwright/node/darwin-x64

cd /Users/dev/src/Chameleon/build/windows
7z a Chameleon.7z
7z d Chameleon.7z -r "*.DS_Store"
7z l Chameleon.7z
scp -s Chameleon.7z srv-cugb14aj1k6c738lm0kg@ssh.ohio.render.com:/local/storage/Chameleon.7z
