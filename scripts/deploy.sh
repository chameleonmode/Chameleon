#!/bin/bash
# cd /Users/dev/src/Chameleon/scripts
# bash deploy.sh 

bash macnoto.sh
UPLOAD_OUTPUT=$(bash macpload.sh)
NOTARIZATION_ID=$(echo "$UPLOAD_OUTPUT" | grep "id:" | awk '{print $2}')
echo "[Info] (mac/status): bash macstatus.sh $NOTARIZATION_ID"
echo "bash macstatus.sh $NOTARIZATION_ID" | pbcopy

bash winbuild.sh
echo "[Info] (mac/status):\n\t bash macship.sh"
echo "bash macship.sh" | pbcopy
bash winpload.sh