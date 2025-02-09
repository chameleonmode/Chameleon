#cleanup folders
rm -rf "App/AppName.app/Contents/MacOS/" 
rm -rf "App/AppName.app/Contents/CodeResources" 
rm -rf "App/AppName.app/Contents/_CodeSignature" 
rm -rf "App/AppName.app/Contents/embedded.provisionprofile" 
mkdir -p "App/AppName.app/Contents/Frameworks/"
mkdir -p "App/AppName.app/Contents/MacOS/"

#Build app
dotnet publish ../../ProjectFolder/AppName.csproj -c release -f net5.0 -r osx-x64 --self-contained true -p:PublishSingleFile=true

#Move app
cd ..
cd ..
cp -R -f ProjectFolder/bin/release/net5.0/osx-x64/publish/* "build/osx/App/AppName.app/Contents/MacOS/"
cd "build/osx/"

APP_ENTITLEMENTS="AppEntitlements.entitlements"
APP_SIGNING_IDENTITY="3rd Party Mac Developer Application: [***]"
INSTALLER_SIGNING_IDENTITY="3rd Party Mac Developer Installer: [***]"
APP_NAME="App/AppName.app"

#<here is moving your app resources to Resources folder using relative symlinks>

#<here is moving your .dylib files to Frameworks folder using relative symlinks>

echo "[INFO] Switch provisionprofile to AppStore"
\cp -R -f AppNameAppStore.provisionprofile "App/AppName.app/Contents/embedded.provisionprofile"

echo "[INFO] Fix libuv.dylib architectures"
lipo -remove i386 "App/AppName.app/Contents/Frameworks/libuv.dylib" "App/AppName.app/Contents/Frameworks/libuv.dylib"

find "$APP_NAME/Contents/Frameworks/"|while read fname; do
    if [[ -f $fname ]]; then
        echo "[INFO] Signing $fname"
        codesign --force --sign "$APP_SIGNING_IDENTITY" "$fname"
    fi
done

echo "[INFO] Signing app executable"
codesign --force --entitlements "$FILE_ENTITLEMENTS" --sign "$APP_SIGNING_IDENTITY" "App/AppName.app/Contents/MacOS/AppName"

echo "[INFO] Signing app bundle"
codesign --force --entitlements "$APP_ENTITLEMENTS" --sign "$APP_SIGNING_IDENTITY" "$APP_NAME"

echo "[INFO] Creating AppName.pkg"
productbuild --component App/AppName.app /Applications --sign "$INSTALLER_SIGNING_IDENTITY" AppName.pkg