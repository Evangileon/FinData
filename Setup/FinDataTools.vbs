'����һ������VBS
dim x
dim fxj
set fxj = createobject("findata.fxjdata")
msgbox "����汾" & fxj.version
fxj.ShowFxjConverter()
'fxj.ShowFxjReader()
'set fxj = nothing
