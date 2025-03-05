#!/bin/bash
# Usage: bash macnotocheck.sh <notarization UUID>

if [ "$#" -ne 1 ]; then
  echo "Usage: $0 <notarization UUID>"
  exit 1
fi

xcrun notarytool info $1 --keychain-profile "DEV"

#cd /Users/dev/src/Chameleon/publish/osx
#scp -s Chameleon.zip srv-cugb14aj1k6c738lm0kg@ssh.ohio.render.com:/local/storage/Chameleon.zip
#scp -s Chameleon.zip srv-cv46cfpu0jms73eja6c0@ssh.ohio.render.com:/local/storage/Chameleon.zip

## 1. First, verify the executable inside the app bundle
#codesign --verify --verbose Chameleon.app/Contents/MacOS/Chameleon
## Expected output should show no errors and indicate proper signing
#
## 2. Verify the entire app bundle
#codesign --verify --verbose Chameleon.app
## Should also show no errors if properly signed
#
## 3. Check detailed code signing information
#codesign -d --verbose=4 Chameleon.app
#
## 4. Verify bundle requirements
#codesign --verify --verbose=4 --strict Chameleon.app
#
## 5. Check if the app meets hardened runtime requirements
#codesign --display --entitlements :- Chameleon.app
#
## 6. Verify the notarization submission status
#xcrun notarytool info Chameleon.zip --keychain-profile "DEV"
#
## 7. Check notarization log for detailed errors
#xcrun notarytool log [submission-id] --keychain-profile "DEV"

# Common reasons for rejection:
# - Missing hardened runtime
# - Unsigned executables or libraries
# - Invalid entitlements
# - Missing secure timestamp
# - Invalid code signing certificate
# - Missing or invalid Info.plist
# - Malware detection
# - Invalid bundle identifier
# - Missing provisioning profile (if required)
