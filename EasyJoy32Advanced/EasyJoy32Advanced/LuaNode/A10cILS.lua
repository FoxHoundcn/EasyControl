function Name() return 'A10cILS' end

function Info() return 'A10-C ILS' end

function PortList()
    local portTable = {
        ['PortList'] = {
            {['name'] = 'ILS In', ['type'] = 'In', ['valueType'] = 'String'},
            {['name'] = 'ILS Out', ['type'] = 'Out', ['valueType'] = 'String'}
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
    if (#strLable == 2) then
        local khz = 0.10
        if (strLable[2] == "0.0") then
            khz = .10
        elseif (strLable[2] == "0.1") then
            khz = .15
        elseif (strLable[2] == "0.2") then
            khz = .30
        elseif (strLable[2] == "0.3") then
            khz = .35
        elseif (strLable[2] == "0.4") then
            khz = .50
        elseif (strLable[2] == "0.5") then
            khz = .55
        elseif (strLable[2] == "0.6") then
            khz = .70
        elseif (strLable[2] == "0.7") then
            khz = .75
        elseif (strLable[2] == "0.8") then
            khz = .90
        elseif (strLable[2] == "0.9") then
            khz = .95
        end
        portTable['PortList'][2] = string.format("%.2f", 108 +
                                                     tonumber(strLable[1]) * 10 +
                                                     khz)
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
