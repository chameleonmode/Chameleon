#!/bin/bash
# Usage cd /Users/dev/src/Chameleon/scripts or C:\repos\Chameleon\Chameleon.Avalonia.Desktop
# bash windowsbuild.sh

dotnet publish /Users/dev/src/Chameleon/Chameleon.Avalonia.Desktop/Chameleon.Avalonia.Desktop.csproj \
  -r win-x64 -c Release -f net8.0 -o "/Users/dev/src/Chameleon/build/windows" \
  --self-contained true \
  -p:DebugType=None \
  -p:DebugSymbols=false \
  -p:PublishSingleFile=true \
  -p:PublishReadyToRun=true \
  -p:IncludeNativeLibrariesForSelfExtract=true 

cd /Users/dev/src/Chameleon/build/windows
rm -rf Chameleon.7z
rm -rf Resources/.playwright

mkdir -p "Resources/.playwright/node/win32_x64/"
cp -a /Users/dev/src/Chameleon/Chameleon.Avalonia.Desktop/bin/Release/net8.0/win-x64/.playwright/node/LICENSE "Resources/.playwright/node/LICENSE"
cp -a /Users/dev/src/Chameleon/Chameleon.Avalonia.Desktop/bin/Release/net8.0/win-x64/.playwright/node/win32_x64/. "Resources/.playwright/node/win32_x64"

mkdir -p "Resources/.playwright/package"
cp -a /Users/dev/src/Chameleon/Chameleon.Avalonia.Desktop/bin/Release/net8.0/win-x64/.playwright/package/. "Resources/.playwright/package"

mkdir -p "Resources/.playwright/scripts"
cp -a /Users/dev/src/chameleon-playwright/dist/. "Resources/.playwright/scripts"
cp -a /Users/dev/src/chameleon-playwright/node_modules/. "Resources/.playwright/scripts/node_modules"
cp -a /Users/dev/src/chameleon-playwright/package.json "Resources/.playwright/scripts/package.json"

rm -rf .playwright
rm -rf playwright.ps1
7z a Chameleon.7z && 7z d Chameleon.7z -r '*.DS_Store'
if [ "$#" -ne 1 ]; then
  echo "Usage: $0 <upload bool>"
  exit 1
fi
scp -s Chameleon.7z srv-cugb14aj1k6c738lm0kg@ssh.ohio.render.com:/local/storage/Chameleon.7z
