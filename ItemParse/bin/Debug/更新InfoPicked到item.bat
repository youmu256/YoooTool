@echo off
set lniPath=D:\MyData\MyFiles\w3x2lni_zhCN_v2.5.2\w2l
set dataPath=D:\VS2015Work\YoooTool\ItemParse\bin\Debug
set mapName=DeadNight
rem %lniPath% help lni
%lniPath% lni %dataPath%\%mapName%.w3x
rem 拷贝item.ini到当前目录
xcopy %dataPath%\%mapName%\table\item.ini %cd% /y
itemparse -pick
itemparse -modify
xcopy %cd%\item.ini %dataPath%\%mapName%\table\item.ini /y
%lniPath% obj %dataPath%\%mapName% %dataPath%\%mapName%_update.w3x
pause