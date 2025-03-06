#!/bin/bash
# cd /Users/dev/src/Chameleon/scripts
# bash macnoto.sh 1

echo "[INFO] Building macOS app"
cd /Users/dev/src/Chameleon/scripts
bash macbuild.sh 

echo "[INFO] Packaging macOS app"
cd /Users/dev/src/Chameleon/scripts
bash macpac.sh

echo "[INFO] Signing macOS app"
cd /Users/dev/src/Chameleon/scripts
bash macsign.sh

if [ "$#" -ne 1 ]; then
  echo "Usage: $0 <upload bool>"
  exit 1
fi
echo "[INFO] Uploading macOS app"
cd /Users/dev/src/Chameleon/scripts
bash macpload.sh
