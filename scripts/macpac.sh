#!/bin/bash
# Usage cd /Users/dev/src/Chameleon/scripts
# bash macpac.sh

APP_NAME=Chameleon.app
SLN_DIR=/Users/dev/src/Chameleon
CSPROJ_DIR=/Users/dev/src/Chameleon/Chameleon.Desktop
BUILD_DIR=/Users/dev/src/Chameleon/build/osx
PUBLISH_DIR=/Users/dev/src/Chameleon/publish/osx

# Prepare 
rm -rf $PUBLISH_DIR/$APP_NAME.zip
rm -rf $PUBLISH_DIR/$APP_NAME
mkdir -p $PUBLISH_DIR/$APP_NAME
mkdir -p $PUBLISH_DIR/$APP_NAME/Contents/
mkdir -p $PUBLISH_DIR/$APP_NAME/Contents/MacOS/
mkdir -p $PUBLISH_DIR/$APP_NAME/Contents/Resources/

# Move 
rm -rf $BUILD_DIR/playwright.ps1
cp -R -f $BUILD_DIR/. $PUBLISH_DIR/$APP_NAME/Contents/MacOS

# Resources 
cp $CSPROJ_DIR/Info.plist $PUBLISH_DIR/$APP_NAME/Contents/Info.plist
cp $CSPROJ_DIR/logo-symbol.icns $PUBLISH_DIR/$APP_NAME/Contents/Resources/logo-symbol.icns
cp -a $SLN_DIR/Resources/. $PUBLISH_DIR/$APP_NAME/Contents/Resources
# cp -a /Users/dev/src/chameleon-playwright/dist/. $PUBLISH_DIR/$APP_NAME/Contents/Resources/scripts/dist

echo "[INFO] done"