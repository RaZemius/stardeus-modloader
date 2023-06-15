echo off

REM remove unnecessary assemblies
DEL /Q .\Libraries\*.*

REM remove obj cache
DEL /Q .\.vscode\obj\*.*

REM build dll
dotnet build .vscode
move .\Libraries\ModCore.dll "C:\Program Files (x86)\Steam\steamapps\common\Stardeus\Stardeus_Data\StreamingAssets\Mods\detourcore"
move .\ModInfo.json "C:\Program Files (x86)\Steam\steamapps\common\Stardeus\Stardeus_Data\StreamingAssets\Mods\detourcore"
