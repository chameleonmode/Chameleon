cd /Users/dev/src/Chameleon/scripts
bash macnoto.sh
cd /Users/dev/src/Chameleon/scripts
bash macpload.sh
echo "[Info] (mac/status):\n\t bash macstatus.sh <notarization UUID>"
echo "bash macstatus.sh" | pbcopy

cd /Users/dev/src/Chameleon/scripts
bash winbuild.sh
cd /Users/dev/src/Chameleon/scripts
bash winpload.sh
cd /Users/dev/src/Chameleon/scripts
bash macship.sh