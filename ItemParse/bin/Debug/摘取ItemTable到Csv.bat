@echo off
set lniPath=E:\War3\ToolSet\w3x2lni_zhCN_v2.5.2\w2l
set dataPath=D:\GithubWork\YoooTool\ItemParse\bin\Debug
set mapName=ZombieWorld_Work
rem %lniPath% help lni
%lniPath% lni %dataPath%\%mapName%.w3x
itemparse -pick item.ini InfoPicked.csv
pause