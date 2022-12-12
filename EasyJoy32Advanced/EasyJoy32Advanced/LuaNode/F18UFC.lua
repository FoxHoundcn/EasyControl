function Name() return 'F18_UFC_Text' end

function Info() return '处理F18的UFC文字' end

function PortList()
    local portTable = {
        ['PortList'] = {
            {['name'] = 'String', ['type'] = 'In', ['valueType'] = 'String'},
            {['name'] = 'Select', ['type'] = 'Out', ['valueType'] = 'String'},
            {['name'] = 'Text', ['type'] = 'Out', ['valueType'] = 'String'}
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
    local len = string.len(portTable['PortList'][1])
    if (len > 0) then
        local select = string.sub(portTable['PortList'][1], 1, 1)
        if (select == '>') then
            portTable['PortList'][2] = ':'
            portTable['PortList'][3] = string.sub(portTable['PortList'][1], 2,
                                                  len)
        else
            portTable['PortList'][2] = ''
            portTable['PortList'][3] = portTable['PortList'][1]
        end
    end
    return portTable
end
