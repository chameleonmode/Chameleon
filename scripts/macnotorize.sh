#!/bin/bash 
# Navigate to the script directory
# cd /Users/dev/src/Chameleon/scripts
# Usage: bash macnotorize.sh <true|false>
# Run the script with an argument (e.g., "false" to proceed with the build)
# bash macnotorize.sh false

#Build app
if [ "$#" -ne 1 ]; then
  echo "No argument provided, defaulting to 'true'."
  ARG="true"
else
  ARG="$1"
fi

if [ "$ARG" == "true" ]; then
  #dotnet publish .csproj -c release -f net8.0 -r osx-x64 --self-contained true -p:PublishSingleFile=true
  dotnet publish \
    /Users/dev/src/Chameleon/Chameleon.Avalonia.Desktop/Chameleon.Avalonia.Desktop.csproj \
    -r osx-x64 \
    -c Release \
    -f net8.0 \
    --self-contained true \
    -p:DebugType=None \
    -p:DebugSymbols=false \
    -p:PublishSingleFile=true \
    -p:PublishReadyToRun=true \
    -p:IncludeNativeLibrariesForSelfExtract=false
fi
bash pacamac.sh
cd /Users/dev/src/Chameleon/build/osx
codesign --verify --verbose Chameleon.app/Contents/MacOS/Chameleon
codesign --verify --verbose Chameleon.app
ditto -c -k --sequesterRsrc --keepParent Chameleon.app Chameleon.zip
xcrun notarytool submit Chameleon.zip --keychain-profile "DEV"
