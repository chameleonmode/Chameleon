#!/bin/bash cbwt-aygq-qiup-udlps
#Build app
#dotnet publish -r osx-x64 -c Release -f net8.0 --self-contained true -p:DebugType=None -p:DebugSymbols=false -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true -p:PublishReadyToRun=true 
bash pacamac.sh
codesign --verify --verbose App/Chameleon.app/Contents/MacOS/Chameleon
ditto -c -k --sequesterRsrc --keepParent App/Chameleon.app App/Chameleon.zip
#xcrun notarytool submit App/Chameleon.zip --keychain-profile "DEV"
#2294894b-882d-410c-9dd7-5163a1d0bae8
#xcrun notarytool info ... --keychain-profile "DEV"
#xcrun notarytool info 2294894b-882d-410c-9dd7-5163a1d0bae8 --keychain-profile "DEV"
#xcrun notarytool info d7c7cf3d-b84d-4643-b7a0-d9e099be269d --keychain-profile "DEV"
#xcrun stapler staple App/Chameleon.app
#xcrun stapler validate App/Chameleon.app
##xcrun altool --validate-app -f App/Chameleon.app -t macos -u dev@chameleonmode.com -p noje-ehjv-yhgo-bfbg
##xcrun notarytool --notarize-app -f App/Chameleon.zip --keychain-profile "DEV" --primary-bundle-id com.chameleon.mode001 -u dev@chameleonmode.com -p "noje-ehjv-yhgo-bfbg"
##xcrun notarytool submit App/Chameleon.zip     
##xcrun notarytool store-credentials DEV --apple-id dev@chameleonmode.com 
#xcrun notarytool info db05f1e1-4ccf-4ca2-b147-ec8dd4546891 --keychain-profile "DEV"
#xcrun notarytool info 22af54b7-6b72-4bed-b920-fddb0c4e318d --keychain-profile "DEV"
#xcrun notarytool info 6c718125-b6e1-413f-a97a-fca6ab90509e --keychain-profile "DEV"
#xcrun notarytool info f827c675-55e9-45b6-ba30-ef278e9fc8fe --keychain-profile "DEV"
#xcrun notarytool info 421a1b35-40a9-4745-b64a-8f68ecfcf43c --keychain-profile "DEV"
#xcrun notarytool info ad12574c-e0f5-46ca-b1b2-dbd08cbb759d --keychain-profile "DEV"
#xcrun notarytool info a1430797-3224-43d6-a5d4-aa7ffd62cf3b --keychain-profile "DEV"
#xcrun notarytool info d9d02511-23bc-45dc-a94f-6a0b7099c75b --keychain-profile "DEV"
#xcrun notarytool info 18383cc0-305b-4d85-bc5c-92957a92c2e9 --keychain-profile "DEV"
#xcrun notarytool info 362529bd-4bbd-4b39-ac03-d07193a554aa --keychain-profile "DEV"
#xcrun notarytool info 192f5a1d-c23f-4731-b33b-32afd871b7c7 --keychain-profile "DEV"
#xcrun notarytool log 192f5a1d-c23f-4731-b33b-32afd871b7c7 --keychain-profile "DEV"
#xcrun notarytool info 526aca37-1d03-455c-86a1-df9d0bfd1e29 --keychain-profile "DEV"
#xcrun notarytool log 526aca37-1d03-455c-86a1-df9d0bfd1e29 --keychain-profile "DEV"
#xcrun notarytool info d583a9d8-525a-4ac2-a118-4a3e471e1075 --keychain-profile "DEV"
#xcrun notarytool log d583a9d8-525a-4ac2-a118-4a3e471e1075 --keychain-profile "DEV"
#xcrun notarytool info a5679336-0f1c-45dc-adb6-24f0f4514407 --keychain-profile "DEV"
#xcrun notarytool info 90e7f4f7-afa9-4fe9-9142-78ad9520e303 --keychain-profile "DEV"
#xcrun notarytool log 90e7f4f7-afa9-4fe9-9142-78ad9520e303 --keychain-profile "DEV"
#xcrun notarytool info 59ae4bbe-48c3-4652-a23a-3d1a4ada1b03 --keychain-profile "DEV"
#xcrun notarytool info 9d773d3d-a3dc-447f-8dbe-2cb390446648 --keychain-profile "DEV"
#xcrun notarytool info 7830c155-31a6-4470-8f52-5be2eaa8f5ba --keychain-profile "DEV"
#xcrun notarytool info 6a65044d-3b4d-421c-a58b-017aee6dc218 --keychain-profile "DEV"
#xcrun notarytool info e8a759a3-d655-483e-86cd-90d2691d1149 --keychain-profile "DEV"
#xcrun notarytool info 4a04b8b8-075f-4f78-a8cc-01bfcaa53ff5 --keychain-profile "DEV"
#xcrun notarytool info 5a2ffaa5-7b55-4e69-8fa1-ec819c43d742 --keychain-profile "DEV"
#xcrun notarytool info e52bb440-ea5b-48d7-bb68-5b028f0e611f --keychain-profile "DEV"
#xcrun notarytool info 4ce8cf52-7486-474d-8905-0304b5dd259c --keychain-profile "DEV"
#xcrun notarytool info 316925d2-9dd2-496b-b7d9-27141f2cbb8b --keychain-profile "DEV"
#xcrun notarytool info 14d2ce9f-f149-4f9a-be38-726a9b4fc6b6 --keychain-profile "DEV"
#xcrun notarytool info 1ecc6143-1ad0-4e0a-85f2-cc749cc7cd23 --keychain-profile "DEV"
#xcrun notarytool info ac25d2f4-f0c1-4b52-be9a-80cd95335031 --keychain-profile "DEV"
#xcrun notarytool info 64ded6eb-6efd-4bc0-85e7-90f35a6f6309 --keychain-profile "DEV"