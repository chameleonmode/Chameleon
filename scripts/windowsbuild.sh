#!/bin/bash
# Usage cd /Users/dev/Projects/Chameleon/scripts or C:\repos\Chameleon\Chameleon.Avalonia.Desktop
# bash pacamac.sh
dotnet publish -r win-x64 -c Release -f net8.0 --self-contained true -p:DebugType=None -p:DebugSymbols=false -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true -p:PublishReadyToRun=true -o "obj/outwin" 
cd obj/outwin
7z a Chameleon.7z
7z d Chameleon.7z -r "*.DS_Store"
7z l Chameleon.7z
scp -s Chameleon.7z srv-cugb14aj1k6c738lm0kg@ssh.ohio.render.com:/local/storage/Chameleon.7z
#dotnet publish -r win-x64 -c Release -f net8.0 --self-contained true -p:DebugType=None -p:DebugSymbols=false -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=false -p:PublishReadyToRun=false -o "obj/outwin2"
#dotnet publish -r win-x64 -c Release -f net8.0 -o "obj/outwinfull"
#dotnet publish -r win-x64 -c Release -f net8.0 --self-contained true -p:DebugType=None -p:DebugSymbols=false -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=false -p:PublishReadyToRun=true -o "obj/outwin3"