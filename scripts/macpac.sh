#!/bin/bash
# Usage cd /Users/dev/src/Chameleon/scripts
# bash macpac.sh

APP_NAME=Chameleon.app

SLN_DIR=/Users/dev/src/Chameleon
CSPROJ=Chameleon.Avalonia.Desktop
BUILD_DIR=$SLN_DIR/build/osx

#cleanup folders
cd $BUILD_DIR
rm -rf Chameleon.zip
rm -rf $APP_NAME
mkdir -p $APP_NAME
mkdir -p "$APP_NAME/Contents/Frameworks/"
mkdir -p "$APP_NAME/Contents/MacOS/"
mkdir -p "$APP_NAME/Contents/Resources"
mkdir -p "$APP_NAME/Contents/Resources/BrowserExtensions"

#Move app
cd $SLN_DIR
rm -rf $CSPROJ/bin/release/net8.0/osx-x64/publish/playwright.ps1
cp -R -f $CSPROJ/bin/release/net8.0/osx-x64/publish/* "$BUILD_DIR/$APP_NAME/Contents/MacOS/"
cp -R -f $CSPROJ/bin/release/net8.0/osx-x64/publish/.playwright/. "$BUILD_DIR/$APP_NAME/Contents/Resources/.playwright"
cd $BUILD_DIR

#<here is moving your app resources to Resources folder using relative symlinks>
cp $SLN_DIR/$CSPROJ/Info.plist $APP_NAME/Contents/Info.plist
cp $SLN_DIR/$CSPROJ/logo-symbol.icns $APP_NAME/Contents/Resources/logo-symbol.icns
cp -a $SLN_DIR/resources/BrowserExtensions/. $APP_NAME/Contents/Resources/BrowserExtensions
mkdir -p $APP_NAME/Contents/Resources/.playwright/scripts/
cp -a /Users/dev/src/chameleon-playwright/dist/. $APP_NAME/Contents/Resources/.playwright/scripts/dist
cp -a /Users/dev/src/chameleon-playwright/node_modules/. $APP_NAME/Contents/Resources/.playwright/scripts/node_modules
cp -a /Users/dev/src/chameleon-playwright/package.json $APP_NAME/Contents/Resources/.playwright/scripts/package.json

#<here is moving your .dylib files to Frameworks folder using relative symlinks>
find "$APP_NAME/Contents/MacOS" -name '*.dylib' | while read fname; do
    if [[ -f $fname ]]; then
        mv $fname "$APP_NAME/Contents/Frameworks/"
    fi
done

cd $APP_NAME/Contents/MacOS
ln -s ../Resources/.playwright .playwright
for dylib in ../Frameworks/*.dylib; do
    ln -s "../Frameworks/$(basename "$dylib")" "$(basename "$dylib")"
done

echo "[INFO] done"