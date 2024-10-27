#cleanup folders
rm -rf "App/Chameleon.app/Contents/MacOS/" 
rm -rf "App/Chameleon.app/Contents/Resources/" 
rm -rf "App/Chameleon.app/Contents/CodeResources" 
rm -rf "App/Chameleon.app/Contents/_CodeSignature" 
rm -rf "App/Chameleon.app/Contents/embedded.provisionprofile" 
mkdir -p "App/Chameleon.app/Contents/Frameworks/"
mkdir -p "App/Chameleon.app/Contents/MacOS/"
mkdir -p "App/Chameleon.app/Contents/Resources"
mkdir -p "App/Chameleon.app/Contents/Resources/BrowserExtensions"

APP_ENTITLEMENTS="chameleonApp.entitlements"
APP_SIGNING_IDENTITY="Developer ID Application: Simon Dadia (5K732WRGK2)"
INSTALLER_SIGNING_IDENTITY="3rd Party Mac Developer Installer: Simon Dadia (5K732WRGK2)"
APP_NAME="App/Chameleon.app"
PUBLISH_EXTENSTIONS="bin/Debug/Resources/BrowserExtensions/."
PUBLISH_PLAYWRIGHT="bin/release/net8.0/osx-x64/publish/.playwright/."
INFO_PLIST="Info.plist"
ICON_FILE="logo-symbol.icns"

#Move app
cp -a bin/release/net8.0/osx-x64/publish/Chameleon $APP_NAME/Contents/MacOS/

#Move app dependencies
cp -a bin/release/net8.0/osx-x64/publish/libAvaloniaNative.dylib $APP_NAME/Contents/MacOS/
cp -a bin/release/net8.0/osx-x64/publish/libHarfBuzzSharp.dylib $APP_NAME/Contents/MacOS/
cp -a bin/release/net8.0/osx-x64/publish/libSkiaSharp.dylib $APP_NAME/Contents/MacOS/

cp "$INFO_PLIST" "$APP_NAME/Contents/Info.plist"

#<here is moving your app resources to Resources folder using relative symlinks>
cp -a bin/release/net8.0/osx-x64/publish/playwright.ps1 $APP_NAME/Contents/Resources/
cp "$ICON_FILE" "$APP_NAME/Contents/Resources/$ICON_FILE"
cp -a "$PUBLISH_EXTENSTIONS" "$APP_NAME/Contents/Resources/BrowserExtensions"
cp -a $PUBLISH_PLAYWRIGHT $APP_NAME/Contents/Resources/.playwright
chflags nohidden /Users/dev/Projects/Chameleon/Chameleon.Avalonia/src/Chameleon.Avalonia.Desktop/App/Chameleon.app/Contents/Resources/.playwright

#<here is moving your .dylib files to Frameworks folder using relative symlinks>
find "$APP_NAME/Contents/MacOS" -name '*.dylib' | while read fname; do
    if [[ -f $fname ]]; then
        mv $fname "$APP_NAME/Contents/Frameworks/"
    fi
done

cd App/Chameleon.app/Contents/MacOS/
for dylib in ../Frameworks/*.dylib; do
    ln -s "../Frameworks/$(basename "$dylib")" "$(basename "$dylib")"
done
ln -s ../Resources/.playwright .playwright

cd "/Users/dev/Projects/Chameleon/Chameleon.Avalonia/src/Chameleon.Avalonia.Desktop"

#echo "[INFO] Switch provisionprofile to AppStore"
cp -R -f Chameleonmodes.provisionprofile "App/Chameleon.app/Contents/embedded.provisionprofile"

#echo "[INFO] Fix libuv.dylib architectures"
#lipo -remove i386 "App/Chameleon.app/Contents/MacOS/libuv.dylib" -output "App/Chameleon.app/Contents/MacOS/libuv.dylib"

find "$APP_NAME/Contents/Frameworks"|while read fname; do
    if [[ -f $fname ]]; then
        #echo "[INFO] Signing $fname"
        codesign --force --sign "$APP_SIGNING_IDENTITY" "$fname"
    fi
done

#echo "[INFO] Signing app executable"
codesign --force --timestamp --options=runtime --entitlements "$APP_ENTITLEMENTS" --sign "$APP_SIGNING_IDENTITY" "App/Chameleon.app/Contents/MacOS/Chameleon"

#echo "[INFO] Signing app bundle"
codesign --force --timestamp --options=runtime --entitlements "$APP_ENTITLEMENTS" --sign "$APP_SIGNING_IDENTITY" "$APP_NAME"

#echo "[INFO] Creating Chameleon.pkg"
#productbuild --component App/Chameleon.app /Applications --sign "$INSTALLER_SIGNING_IDENTITY" Chameleon.pkg

echo "[INFO] done"