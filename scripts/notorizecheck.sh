#!/bin/bash
# Usage: bash notorizecheck.sh <notarization UUID>

if [ "$#" -ne 1 ]; then
  echo "Usage: $0 <notarization UUID>"
  exit 1
fi

xcrun notarytool info $1 --keychain-profile "DEV"

#cd /Users/dev/src/Chameleon/build/osx
#scp -s Chameleon.zip srv-cugb14aj1k6c738lm0kg@ssh.ohio.render.com:/local/storage/Chameleon.zip
