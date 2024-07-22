#!/bin/bash cbwt-aygq-qiup-udlps
#Build app
dotnet publish -r osx-x64 -c Release -f net8.0 --self-contained true -p:DebugType=None -p:DebugSymbols=false -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true -p:PublishReadyToRun=true 
bash pacamac.sh
ditto -c -k --sequesterRsrc --keepParent App/Chameleon.app App/Chameleon.zip
xcrun notarytool submit App/Chameleon.zip --keychain-profile "DEV"
xcrun notarytool info ... --keychain-profile "DEV"
xcrun stapler staple App/Chameleon.app
xcrun stapler validate App/Chameleon.app
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
xcrun notarytool info d9d02511-23bc-45dc-a94f-6a0b7099c75b --keychain-profile "DEV"
xcrun notarytool info 18383cc0-305b-4d85-bc5c-92957a92c2e9 --keychain-profile "DEV"
xcrun notarytool info 362529bd-4bbd-4b39-ac03-d07193a554aa --keychain-profile "DEV"
xcrun notarytool info 192f5a1d-c23f-4731-b33b-32afd871b7c7 --keychain-profile "DEV"
xcrun notarytool log 192f5a1d-c23f-4731-b33b-32afd871b7c7 --keychain-profile "DEV"
xcrun notarytool info 526aca37-1d03-455c-86a1-df9d0bfd1e29 --keychain-profile "DEV"
xcrun notarytool log 526aca37-1d03-455c-86a1-df9d0bfd1e29 --keychain-profile "DEV"
xcrun notarytool info d583a9d8-525a-4ac2-a118-4a3e471e1075 --keychain-profile "DEV"
xcrun notarytool log d583a9d8-525a-4ac2-a118-4a3e471e1075 --keychain-profile "DEV"
xcrun notarytool info a5679336-0f1c-45dc-adb6-24f0f4514407 --keychain-profile "DEV"