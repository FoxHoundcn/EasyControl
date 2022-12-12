function Name() return 'FloatPlus' end

function Info() return '两个浮点数相加' end

function PortList()
    local portTable = {
        ['PortList'] = {
            {['name'] = 'A', ['type'] = 'In', ['valueType'] = 'Float'},
            {['name'] = 'B', ['type'] = 'In', ['valueType'] = 'Float'},
            {['name'] = 'Out', ['type'] = 'Out', ['valueType'] = 'Float'}
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
    portTable['PortList'][3] = portTable['PortList'][1] +
                                   portTable['PortList'][2]
    return portTable
end
