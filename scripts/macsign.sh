#!/bin/bash
# Usage cd /Users/dev/src/Chameleon/scripts
# bash macsign.sh

APP_NAME=Chameleon.app
APP_SIGNING_IDENTITY="Developer ID Application: Simon Dadia (5K732WRGK2)"
CSPROJ_DIR=/Users/dev/src/Chameleon/Chameleon.Desktop
PUBLISH_DIR=/Users/dev/src/Chameleon/publish/osx

# Move .playwright folder from MacOS to Resources if needed
if [ -d "$PUBLISH_DIR/$APP_NAME/Contents/MacOS/.playwright" ]; then
    echo "[INFO] Moving .playwright from MacOS to Resources"
    mv "$PUBLISH_DIR/$APP_NAME/Contents/MacOS/.playwright" "$PUBLISH_DIR/$APP_NAME/Contents/Resources/"
fi

# Remove any existing symlink (if present) in MacOS
if [ -L "$PUBLISH_DIR/$APP_NAME/Contents/MacOS/.playwright" ]; then
    rm "$PUBLISH_DIR/$APP_NAME/Contents/MacOS/.playwright"
fi

# Create a symlink in MacOS pointing to the new Resources location
echo "[INFO] Creating symlink for .playwright in MacOS folder"
ln -s "../Resources/.playwright" "$PUBLISH_DIR/$APP_NAME/Contents/MacOS/.playwright"

# [Perform all your signing operations here...]
echo "[INFO] Signing app bundle - dylib files"
find "$PUBLISH_DIR/$APP_NAME/Contents/MacOS" -name '*.dylib' | while read fname; do
    if [[ -f $fname ]]; then
        echo "[INFO] Signing $fname"
        codesign --force --sign "$APP_SIGNING_IDENTITY" "$fname"
    fi
done

eecho "[INFO] Signing fsevents module"
codesign --force --timestamp --options runtime --entitlements "$CSPROJ_DIR/chameleonApp.entitlements" --sign "$APP_SIGNING_IDENTITY" "$PUBLISH_DIR/$APP_NAME/Contents/Resources/scripts/node_modules/fsevents/fsevents.node"

echo "[INFO] Switching provision profile to AppStore"
cp -R -f "$CSPROJ_DIR/chameleonmodes.provisionprofile" "$PUBLISH_DIR/$APP_NAME/Contents/embedded.provisionprofile"

echo "[INFO] Signing app executable"
codesign --force --timestamp --options runtime --entitlements "$CSPROJ_DIR/chameleonApp.entitlements" --sign "$APP_SIGNING_IDENTITY" "$PUBLISH_DIR/$APP_NAME/Contents/MacOS/Chameleon"

echo "[INFO] Signing app bundle"
codesign --force --timestamp --options runtime --entitlements "$CSPROJ_DIR/chameleonApp.entitlements" --sign "$APP_SIGNING_IDENTITY" "$PUBLISH_DIR/$APP_NAME"

echo "[INFO] Verifying signed app bundle"
codesign --verify --deep --strict "$PUBLISH_DIR/$APP_NAME"

echo "[INFO] done"