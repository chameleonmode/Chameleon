#!/bin/bash
# Usage cd /Users/dev/src/Chameleon/scripts
# bash winpload.sh

cd /Users/dev/src/Chameleon/build/windows
7z a Chameleon.7z && 7z d Chameleon.7z -r '*.DS_Store'
scp -s Chameleon.7z srv-cugb14aj1k6c738lm0kg@ssh.ohio.render.com:/local/storage/Chameleon.7z