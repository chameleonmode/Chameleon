#!/bin/bash 
# Usage cd /Users/dev/src/Chameleon/scripts
# bash macnotorize.sh

#Build app
#dotnet publish .csproj -c release -f net8.0 -r osx-x64 --self-contained true -p:PublishSingleFile=true
dotnet publish /Users/dev/src/Chameleon/Chameleon.Avalonia.Desktop/Chameleon.Avalonia.Desktop.csproj \
 -r osx-x64 -c Release -f net8.0 --self-contained true \
 -p:DebugType=None \
 -p:DebugSymbols=false \
 -p:PublishSingleFile=true \
 -p:PublishReadyToRun=true \
 -p:IncludeNativeLibrariesForSelfExtract=false
bash pacamac.sh
cd /Users/dev/src/Chameleon/build/osx
codesign --verify --verbose Chameleon.app/Contents/MacOS/Chameleon
codesign --verify --verbose Chameleon.app
ditto -c -k --sequesterRsrc --keepParent Chameleon.app Chameleon.zip
xcrun notarytool submit Chameleon.zip --keychain-profile "DEV"
