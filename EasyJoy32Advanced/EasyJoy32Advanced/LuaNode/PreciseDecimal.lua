function Name() return 'PreciseDecimal' end

function Info() return '精准小数' end

function PortList()
    local portTable = {
        ['PortList'] = {
            {['name'] = 'InString', ['type'] = 'In', ['valueType'] = 'Float'},
            {['name'] = 'Digits', ['type'] = 'In', ['valueType'] = 'Int'},
            {['name'] = 'OutString', ['type'] = 'Out', ['valueType'] = 'String'},
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
	local nNum = portTable['PortList'][1]
	local n = portTable['PortList'][2]
    n = n or 0;
    n = math.floor(n)
    if n < 0 then
        n = 0;
    end
    local nDecimal = 10 ^ n
    local nTemp = math.floor(nNum * nDecimal);
    local nRet = nTemp / nDecimal;
	portTable['PortList'][3] = tostring(nRet)
    return portTable
end
