#!/bin/bash
# This script converts an SVG file into a multi-size ICO file using Inkscape and ImageMagick.
# Make sure you have Inkscape and ImageMagick (ImageMagick 7+) installed:
#   brew install inkscape imagemagick

# Hardcoded file names (modify these if necessary)
input_svg="logo-symbol.svg"
output_ico="logo-symbol.ico"

# Define the sizes (in pixels) you want in your .ico file.
sizes=(16 32 48 64 128 256)

# Create a temporary directory for PNG files.
tmp_dir=$(mktemp -d)

echo "Converting $input_svg to PNGs in sizes: ${sizes[*]}"

# Loop through each size, converting the SVG to a PNG at that size.
for size in "${sizes[@]}"; do
  png_file="$tmp_dir/icon-${size}.png"
  echo "Exporting ${size}x${size} PNG to $png_file"
  inkscape "$input_svg" --export-type=png --export-width="$size" --export-height="$size" -o "$png_file"
done

echo "Combining PNGs into $output_ico"
# Use ImageMagick's 'magick' command to combine all the PNGs into one .ico file.
magick "$tmp_dir"/icon-*.png "$output_ico"

# Clean up temporary files.
rm -r "$tmp_dir"

echo "Created $output_ico from $input_svg"
