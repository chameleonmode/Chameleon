#!/bin/sh -x

set -e

SIZES="
16,16x16
32,16x16@2x
32,32x32
64,32x32@2x
128,128x128
256,128x128@2x
256,256x256
512,256x256@2x
512,512x512
1024,512x512@2x
"

#FILES=/Users/dev/Chameleon/Chameleon/Chameleon.Avalonia/src/Chameleon.Avalonia.Common/Assets/*
#for SVG in $FILES
#do
    SVG=logo-symbol.svg
echo "Processing the $SVG file..."
  # take action on each file. $f store current file name
    BASE=$(basename "$SVG" | sed 's/\.[^\.]*$//')
    ICONSET="$BASE.iconset"
    mkdir -p "$ICONSET"
    qlmanage -t -s 1024 -o "$ICONSET" "$SVG"
    mv "$ICONSET"/$BASE.svg.png "$ICONSET"/icon_512x512@2x.png
	color=$( convert "$ICONSET"/icon_512x512@2x.png -format "%[pixel:p{0,0}]" info:- )
    convert "$ICONSET"/icon_512x512@2x.png -alpha off -bordercolor $color -border 1 \
    \( +clone -fuzz 30% -fill none -floodfill +0+0 $color \
       -alpha extract -geometry 200% -blur 0x0.5 \
       -morphology erode square:1 -geometry 50% \) \
    -compose CopyOpacity -composite -shave 1 "$ICONSET"/icon_512x512@2x.png
	#convert "$ICONSET"/icon_512x512@2x.png -transparent white "$ICONSET"/icon_512x512@2x.png
    #blah
	for PARAMS in $SIZES; do
		SIZE=$(echo $PARAMS | cut -d, -f1)
		LABEL=$(echo $PARAMS | cut -d, -f2)
		sips -z $SIZE $SIZE "$ICONSET"/icon_512x512@2x.png --out "$ICONSET"/icon_$LABEL.png
	done
	cp "$ICONSET/icon_16x16@2x.png" "$ICONSET/icon_32x32.png"
	cp "$ICONSET/icon_128x128@2x.png" "$ICONSET/icon_256x256.png"
	cp "$ICONSET/icon_256x256@2x.png" "$ICONSET/icon_512x512.png"
	iconutil -c icns "$ICONSET"
	#rm -rf "$ICONSET"
#done

