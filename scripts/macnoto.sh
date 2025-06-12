#!/bin/bash
# cd /Users/dev/src/Chameleon/scripts
# bash macnoto.sh 1

echo "[INFO] Building"
cd /Users/dev/src/Chameleon/scripts
bash macbuild.sh 

echo "[INFO] Packaging"
cd /Users/dev/src/Chameleon/scripts
bash macpac.sh

echo "[INFO] Signing"
cd /Users/dev/src/Chameleon/scripts
bash macsign.sh
