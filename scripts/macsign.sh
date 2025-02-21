#!/bin/bash
# Usage cd /Users/dev/src/Chameleon/scripts
# bash macsign.sh


SLN_DIR=/Users/dev/src/Chameleon
CSPROJ=Chameleon.Avalonia.Desktop
BUILD_DIR=$SLN_DIR/build/osx

APP_NAME=Chameleon.app
APP_SIGNING_IDENTITY="Developer ID Application: Simon Dadia (5K732WRGK2)"
APP_ENTITLEMENTS_FILE=$SLN_DIR/$CSPROJ/chameleonApp.entitlements
APP_PROVISIONINGPROFILE=$SLN_DIR/$CSPROJ/chameleonmodes.provisionprofile 

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