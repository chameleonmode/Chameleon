#!/bin/bash
# cd /Users/dev/src/Chameleon/scripts
# bash deploy.sh 

bash macnoto.sh
bash macpload.sh
echo "[Info] (mac/status):\n\t bash macstatus.sh <notarization UUID>"
echo "bash macstatus.sh" | pbcopy

bash winbuild.sh
bash winpload.sh
bash macship.sh