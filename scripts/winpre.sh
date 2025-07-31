#!/bin/bash
# https://chameleon-ws.onrender.com/app/download/pre?ext=7z
# Usage cd /Users/dev/src/Chameleon/scripts
# bash winpre.sh

bash winbuild.sh

cd /Users/dev/src/Chameleon/publish/win
scp -s Chameleon.7z srv-cugb14aj1k6c738lm0kg@ssh.ohio.render.com:/local/storage/pre/Chameleon.7z