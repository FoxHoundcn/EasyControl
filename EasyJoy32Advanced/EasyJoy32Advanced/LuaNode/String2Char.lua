function Name() return 'String2Char' end

function Info() return '字符串转字符' end

function PortList()
    local portTable = {
        ['PortList'] = {
            {['name'] = 'String', ['type'] = 'In', ['valueType'] = 'String'},
            {['name'] = 'Align', ['type'] = 'In', ['valueType'] = 'Int'},
            {['name'] = 'Char1', ['type'] = 'Out', ['valueType'] = 'String'},
            {['name'] = 'Char2', ['type'] = 'Out', ['valueType'] = 'String'},
            {['name'] = 'Char3', ['type'] = 'Out', ['valueType'] = 'String'},
            {['name'] = 'Char4', ['type'] = 'Out', ['valueType'] = 'String'},
            {['name'] = 'Char5', ['type'] = 'Out', ['valueType'] = 'String'},
            {['name'] = 'Char6', ['type'] = 'Out', ['valueType'] = 'String'},
            {['name'] = 'Char7', ['type'] = 'Out', ['valueType'] = 'String'},
            {['name'] = 'Char8', ['type'] = 'Out', ['valueType'] = 'String'},
            {['name'] = 'Char9', ['type'] = 'Out', ['valueType'] = 'String'},
            {['name'] = 'Char10', ['type'] = 'Out', ['valueType'] = 'String'},
            {['name'] = 'Char11', ['type'] = 'Out', ['valueType'] = 'String'},
            {['name'] = 'Char12', ['type'] = 'Out', ['valueType'] = 'String'},
            {['name'] = 'Char13', ['type'] = 'Out', ['valueType'] = 'String'},
            {['name'] = 'Char14', ['type'] = 'Out', ['valueType'] = 'String'},
            {['name'] = 'Char15', ['type'] = 'Out', ['valueType'] = 'String'},
            {['name'] = 'Char16', ['type'] = 'Out', ['valueType'] = 'String'}
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
        if (portTable['PortList'][2] == 1) then
            for i = 1, 16 do
                if (i <= len) then
                    portTable['PortList'][2 + i] =
                        string.sub(portTable['PortList'][1], i, i)
                else
                    portTable['PortList'][2 + i] = ''
                end
            end
        else
            for i = 1, 16 do
                if (i <= len) then
                    portTable['PortList'][19 - i] =
                        string.sub(portTable['PortList'][1], len + 1 - i,
                                   len + 1 - i)
                else
                    portTable['PortList'][19 - i] = ''
                end
            end
        end
    end
    return portTable
end
