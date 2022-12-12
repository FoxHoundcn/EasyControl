function Name() return 'StringSwitch' end

function Info() return '字符串切换' end

function PortList()
    local portTable = {
        ['PortList'] = {
            {['name'] = 'Index', ['type'] = 'In', ['valueType'] = 'Int'},
            {['name'] = 'String1', ['type'] = 'In', ['valueType'] = 'String'},
            {['name'] = 'String2', ['type'] = 'In', ['valueType'] = 'String'},
            {['name'] = 'String3', ['type'] = 'In', ['valueType'] = 'String'},
            {['name'] = 'String4', ['type'] = 'In', ['valueType'] = 'String'},
            {['name'] = 'StringOut', ['type'] = 'Out', ['valueType'] = 'String'}
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
    if (portTable['PortList'][1] < 4) then -- 0-3
        portTable['PortList'][6] = portTable['PortList'][2 +
                                       portTable['PortList'][1]]
    else
        portTable['PortList'][6] = ''
    end
    return portTable
end
