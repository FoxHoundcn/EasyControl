function Name() return 'Char2String' end

function Info() return '字符拼字符串' end

function PortList()
    local portTable = {
        ['PortList'] = {
            {['name'] = 'String', ['type'] = 'Out', ['valueType'] = 'String'},
            {['name'] = 'Char1', ['type'] = 'In', ['valueType'] = 'String'},
            {['name'] = 'Char2', ['type'] = 'In', ['valueType'] = 'String'},
            {['name'] = 'Char3', ['type'] = 'In', ['valueType'] = 'String'},
            {['name'] = 'Char4', ['type'] = 'In', ['valueType'] = 'String'},
            {['name'] = 'Char5', ['type'] = 'In', ['valueType'] = 'String'},
            {['name'] = 'Char6', ['type'] = 'In', ['valueType'] = 'String'},
            {['name'] = 'Char7', ['type'] = 'In', ['valueType'] = 'String'},
            {['name'] = 'Char8', ['type'] = 'In', ['valueType'] = 'String'},
            {['name'] = 'Char9', ['type'] = 'In', ['valueType'] = 'String'},
            {['name'] = 'Char10', ['type'] = 'In', ['valueType'] = 'String'},
            {['name'] = 'Char11', ['type'] = 'In', ['valueType'] = 'String'},
            {['name'] = 'Char12', ['type'] = 'In', ['valueType'] = 'String'},
            {['name'] = 'Char13', ['type'] = 'In', ['valueType'] = 'String'},
            {['name'] = 'Char14', ['type'] = 'In', ['valueType'] = 'String'},
            {['name'] = 'Char15', ['type'] = 'In', ['valueType'] = 'String'},
            {['name'] = 'Char16', ['type'] = 'In', ['valueType'] = 'String'}
        }
    }
    return portTable
end

function Update(portTable)
    --    ["PortList"] = {
    --        0.333333343,
    --        0.0,
    --        0.0
    --    }
    portTable['PortList'][1] = portTable['PortList'][2] ..
                                   portTable['PortList'][3] ..
                                   portTable['PortList'][4] ..
                                   portTable['PortList'][5] ..
                                   portTable['PortList'][6] ..
                                   portTable['PortList'][7] ..
                                   portTable['PortList'][8] ..
                                   portTable['PortList'][9] ..
                                   portTable['PortList'][10] ..
                                   portTable['PortList'][11] ..
                                   portTable['PortList'][12] ..
                                   portTable['PortList'][13] ..
                                   portTable['PortList'][14] ..
                                   portTable['PortList'][15] ..
                                   portTable['PortList'][16] ..
                                   portTable['PortList'][17]
    return portTable
end
