function Name() return 'A10cVHF' end

function Info() return 'A10-C VHF' end

function PortList()
    local portTable = {
        ['PortList'] = {
            {['name'] = 'VHF In', ['type'] = 'In', ['valueType'] = 'String'},
            {['name'] = 'VHF Out', ['type'] = 'Out', ['valueType'] = 'String'}
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
    local strLable = split(portTable['PortList'][1], "-")
    if (#strLable == 4) then
        portTable['PortList'][2] = string.format("%.3f",
                                                 tonumber(strLable[1]) * 200 +
                                                     tonumber(strLable[2]) * 10 +
                                                     tonumber(strLable[3]) +
                                                     tonumber(strLable[4]) / 10)
    end
    return portTable
end
-- Custom Function
function split(str, reps)
    local resultStrList = {}
    string.gsub(str, '[^' .. reps .. ']+',
                function(w) table.insert(resultStrList, w) end)
    return resultStrList
end
