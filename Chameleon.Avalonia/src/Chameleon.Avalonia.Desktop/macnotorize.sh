#!/bin/bash
ditto -c -k --sequesterRsrc --keepParent Chameleon.app Chameleon.zip
xcrun notarytool --notarize-app -f Chameleon.zip --primary-bundle-id com.Chameleon001 -u dev@chameleonmode.com -p "noje-ehjv-yhgo-bfbg"
% xcrun notarytool store-credentials DEV --apple-id dev@chameleonmode.com 
