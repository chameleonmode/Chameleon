#!/bin/bash
# Usage cd /Users/dev/src/Chameleon/scripts
# bash pacamac.sh

APP_NAME=Chameleon.app
APP_SIGNING_IDENTITY="Developer ID Application: Simon Dadia (5K732WRGK2)"

SLN_DIR=/Users/dev/src/Chameleon
CSPROJ=Chameleon.Avalonia.Desktop
BUILD_DIR=$SLN_DIR/build/osx

APP_ENTITLEMENTS_FILE=$SLN_DIR/$CSPROJ/chameleonApp.entitlements
APP_PROVISIONINGPROFILE=$SLN_DIR/$CSPROJ/chameleonmodes.provisionprofile 

#cleanup folders
cd $BUILD_DIR
rm -rf "$APP_NAME/Contents/MacOS/" 
rm -rf "$APP_NAME/Contents/CodeResources" 
rm -rf "$APP_NAME/Contents/_CodeSignature"
rm -rf "$APP_NAME/Contents/Resources/"  
rm -rf "$APP_NAME/Contents/embedded.provisionprofile" 
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
cp -a $SLN_DIR/resources/.playwright/scripts/. $APP_NAME/Contents/Resources/.playwright/scripts

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
cd $BUILD_DIR

echo "[INFO] Switch provisionprofile to AppStore"
\cp -R -f $APP_PROVISIONINGPROFILE $APP_NAME/Contents/embedded.provisionprofile

find "$APP_NAME/Contents/Frameworks"|while read fname; do
    if [[ -f $fname ]]; then
        echo "[INFO] Signing $fname"
        codesign --force --sign "$APP_SIGNING_IDENTITY" "$fname"
    fi
done

echo "[INFO] Signing app executable"
codesign --force --timestamp --options=runtime --entitlements "$APP_ENTITLEMENTS_FILE" --sign "$APP_SIGNING_IDENTITY" "$APP_NAME/Contents/MacOS/Chameleon"

echo "[INFO] Signing app bundle"
codesign --force --timestamp --options=runtime --entitlements "$APP_ENTITLEMENTS_FILE" --sign "$APP_SIGNING_IDENTITY" "$APP_NAME"

#echo "[INFO] Creating Chameleon.pkg"
#productbuild --component App/Chameleon.app /Applications --sign "$INSTALLER_SIGNING_IDENTITY" Chameleon.pkg

echo "[INFO] done"