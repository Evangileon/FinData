'这是一个测试VBS
dim x
dim fxj
set fxj = createobject("findata.fxjdata")
msgbox "组件版本" & fxj.version
fxj.ShowFxjConverter()
'fxj.ShowFxjReader()
'set fxj = nothing
