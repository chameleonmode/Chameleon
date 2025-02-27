cd /Users/dev/src/Chameleon/publish/osx
codesign --verify --verbose Chameleon.app/Contents/MacOS/Chameleon
codesign --verify --verbose Chameleon.app
ditto -c -k --sequesterRsrc --keepParent Chameleon.app Chameleon.zip
xcrun notarytool submit Chameleon.zip --keychain-profile "DEV"