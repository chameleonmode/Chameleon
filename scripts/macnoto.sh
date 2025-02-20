#!/bin/bash
# cd /Users/dev/src/Chameleon/scripts
# bash macno.sh 

echo "[INFO] Building macOS app"
cd /Users/dev/src/Chameleon/scripts
bash buildamac.sh 

echo "[INFO] Packaging macOS app"
cd /Users/dev/src/Chameleon/scripts
bash pacamac.sh

echo "[INFO] Signing macOS app"
cd /Users/dev/src/Chameleon/scripts
bash signamac.sh

if [ "$#" -ne 1 ]; then
  echo "Usage: $0 <upload bool>"
  exit 1
fi

cd /Users/dev/src/Chameleon/build/osx
codesign --verify --verbose Chameleon.app/Contents/MacOS/Chameleon
codesign --verify --verbose Chameleon.app
ditto -c -k --sequesterRsrc --keepParent Chameleon.app Chameleon.zip
xcrun notarytool submit Chameleon.zip --keychain-profile "DEV"
