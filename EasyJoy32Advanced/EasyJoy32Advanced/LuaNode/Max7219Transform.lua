﻿function Name() return 'Max7219Transform' end

function Info() return 'Max7219数码管转换' end

function PortList()
    local portTable = {
        ['PortList'] = {
            {['name'] = 'In', ['type'] = 'In', ['valueType'] = 'String'},
            {['name'] = 'Out', ['type'] = 'Out', ['valueType'] = 'String'}
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
    local DigitalTubeByte = {
        0x7e, 0x30, 0x6d, 0x79, 0x33, 0x5b, 0x5f, 0x70, 0x7f, 0x7b, 0x00, ----
        0xfe, 0xb0, 0xed, 0xf9, 0xb3, 0xdb, 0xdf, 0xf0, 0xff, 0xfb, 0x80
    }
    local len = string.len(portTable['PortList'][1])
    if (len > 0) then
        local outputStr = ''
        local currentChar
        local byteTable = {}
        local newLength = 1
        for i = 1, len do
            currentChar = string.sub(portTable['PortList'][1], i, i)
            if (currentChar == '.') then
                if (newLength > 1) then
                    byteTable[newLength - 1] = byteTable[newLength - 1] + 11
                end
            elseif (currentChar == '0') then
                byteTable[newLength] = 1
                newLength = newLength + 1
            elseif (currentChar == '1') then
                byteTable[newLength] = 2
                newLength = newLength + 1
            elseif (currentChar == '2') then
                byteTable[newLength] = 3
                newLength = newLength + 1
            elseif (currentChar == '3') then
                byteTable[newLength] = 4
                newLength = newLength + 1
            elseif (currentChar == '4') then
                byteTable[newLength] = 5
                newLength = newLength + 1
            elseif (currentChar == '5') then
                byteTable[newLength] = 6
                newLength = newLength + 1
            elseif (currentChar == '6') then
                byteTable[newLength] = 7
                newLength = newLength + 1
            elseif (currentChar == '7') then
                byteTable[newLength] = 8
                newLength = newLength + 1
            elseif (currentChar == '8') then
                byteTable[newLength] = 9
                newLength = newLength + 1
            elseif (currentChar == '9') then
                byteTable[newLength] = 10
                newLength = newLength + 1
            elseif (currentChar == ' ') then
                byteTable[newLength] = 11
                newLength = newLength + 1
            end
        end
        for i = 1, 8 do
            if (i <= #byteTable) then
                if (#byteTable >= 8 and i ~= 1) then
                    outputStr = outputStr .. ','
                end
                if (#byteTable < 8) then
                    outputStr = outputStr .. ','
                end
                outputStr = outputStr .. DigitalTubeByte[byteTable[i]]
            else
                outputStr = DigitalTubeByte[11] .. outputStr
                if (i < 8) then outputStr = ',' .. outputStr end
            end
        end
        portTable['PortList'][2] = outputStr
    end
    return portTable
end
