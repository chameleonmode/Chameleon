#!/bin/bash
# Usage cd /Users/dev/src/Chameleon/scripts
# bash macpac.sh

APP_NAME=Chameleon.app
SLN_DIR=/Users/dev/src/Chameleon
CSPROJ_DIR=/Users/dev/src/Chameleon/Chameleon.Desktop
BUILD_DIR=/Users/dev/src/Chameleon/build/osx
PUBLISH_DIR=/Users/dev/src/Chameleon/publish/osx
#
rm -rf $PUBLISH_DIR/$APP_NAME.zip
rm -rf $PUBLISH_DIR/$APP_NAME
mkdir -p $PUBLISH_DIR/$APP_NAME
mkdir -p $PUBLISH_DIR/$APP_NAME/Contents/
mkdir -p $PUBLISH_DIR/$APP_NAME/Contents/MacOS/
mkdir -p $PUBLISH_DIR/$APP_NAME/Contents/Resources/
mkdir -p $PUBLISH_DIR/$APP_NAME/Contents/Resources/BrowserExtensions/
mkdir -p $PUBLISH_DIR/$APP_NAME/Contents/Resources/scripts/

#Move app
rm -rf $BUILD_DIR/playwright.ps1
cp -R -f $BUILD_DIR/. $PUBLISH_DIR/$APP_NAME/Contents/MacOS

#<here is moving your app resources to Resources folder using relative symlinks>
cp $CSPROJ_DIR/Info.plist $PUBLISH_DIR/$APP_NAME/Contents/Info.plist
cp $CSPROJ_DIR/logo-symbol.icns $PUBLISH_DIR/$APP_NAME/Contents/Resources/logo-symbol.icns
cp $SLN_DIR/resources/example.js $PUBLISH_DIR/$APP_NAME/Contents/Resources/example.js
cp -a $SLN_DIR/resources/BrowserExtensions/. $PUBLISH_DIR/$APP_NAME/Contents/Resources/BrowserExtensions
cp -a /Users/dev/src/chameleon-playwright/dist/. $PUBLISH_DIR/$APP_NAME/Contents/Resources/scripts/dist

echo "[INFO] done"