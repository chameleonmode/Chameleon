#!/bin/bash cbwt-aygq-qiup-udlps
ditto -c -k --sequesterRsrc --keepParent App/Chameleon.app App/Chameleon.zip
xcrun notarytool submit App/Chameleon.zip --keychain-profile "DEV"
xcrun notarytool info ... --keychain-profile "DEV"
xcrun stapler staple App/Chameleon.app
#xcrun altool --validate-app -f App/Chameleon.app -t macos -u dev@chameleonmode.com -p noje-ehjv-yhgo-bfbg
#xcrun notarytool --notarize-app -f App/Chameleon.zip --keychain-profile "DEV" --primary-bundle-id com.chameleon.mode001 -u dev@chameleonmode.com -p "noje-ehjv-yhgo-bfbg"
#xcrun notarytool submit App/Chameleon.zip     
#xcrun notarytool store-credentials DEV --apple-id dev@chameleonmode.com 
xcrun notarytool info db05f1e1-4ccf-4ca2-b147-ec8dd4546891 --keychain-profile "DEV"
xcrun notarytool info 22af54b7-6b72-4bed-b920-fddb0c4e318d --keychain-profile "DEV"
xcrun notarytool info 6c718125-b6e1-413f-a97a-fca6ab90509e --keychain-profile "DEV"
xcrun notarytool info f827c675-55e9-45b6-ba30-ef278e9fc8fe --keychain-profile "DEV"
xcrun notarytool info 421a1b35-40a9-4745-b64a-8f68ecfcf43c --keychain-profile "DEV"
xcrun notarytool info ad12574c-e0f5-46ca-b1b2-dbd08cbb759d --keychain-profile "DEV"
xcrun notarytool info a1430797-3224-43d6-a5d4-aa7ffd62cf3b --keychain-profile "DEV"
