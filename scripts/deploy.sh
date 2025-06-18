#!/bin/bash
# cd /Users/dev/src/Chameleon/scripts
# bash deploy.sh 

bash macnoto.sh
bash macpload.sh
echo "[Info] (mac/status): bash macstatus.sh "
echo "bash macstatus.sh " | pbcopy

bash winbuild.sh
echo "[Info] (mac/status):\n\t bash macship.sh"
echo "bash macship.sh" | pbcopy
bash winpload.sh