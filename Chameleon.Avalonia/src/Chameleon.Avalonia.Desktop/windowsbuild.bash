cd C:\repos\Chameleon\Chameleon.Avalonia\src\Chameleon.Avalonia.Desktop
dotnet publish -r win-x64 -c Release -f net8.0 --self-contained true -p:DebugType=None -p:DebugSymbols=false -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true -p:PublishReadyToRun=true -o "obj/outwin" 
scp -s Chameleon.7z srv-cugb14aj1k6c738lm0kg@ssh.ohio.render.com:/local/storage/Chameleon.7z
scp -s Chameleon.7z srv-cugvv9lds78s73b7j7pg@ssh.ohio.render.com:/local/storage/Chameleon.7z
 srv-cugvv9lds78s73b7j7pg@ssh.ohio.render.com
dotnet publish -r win-x64 -c Release -f net8.0 --self-contained true -p:DebugType=None -p:DebugSymbols=false -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=false -p:PublishReadyToRun=false -o "obj/outwin2"
dotnet publish -r win-x64 -c Release -f net8.0 -o "obj/outwinfull"
dotnet publish -r win-x64 -c Release -f net8.0 --self-contained true -p:DebugType=None -p:DebugSymbols=false -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=false -p:PublishReadyToRun=true -o "obj/outwin3"