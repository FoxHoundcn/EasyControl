using SharpDX;
using SharpDX.DirectWrite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyControl
{
    public enum keySize
    {
        x1,
        x125,
        x15,
        x2,
        x2H,
        x25,
        x55
    }

    public class keyID
    {
        private byte Hex;
        private string Name;
        public keyID(byte _Hex, string _Name)
        {
            Hex = _Hex;
            Name = _Name;
        }
        public byte getHex()
        {
            return Hex;
        }
        public string getName()
        {
            return Name;
        }
    }

    public class Key
    {
        public float x;
        public float y;
        public keySize size;
        //------------------------------------------
        public byte State { get; set; }
        //---------------------------
        public byte Hex;
        public string Name;
        public string ShowName;
        public float sizeScale;
        public RectangleF OuterFrame = new RectangleF();
        public RectangleF InnerFrame = new RectangleF();
        public RectangleF TextFrame = new RectangleF();
        public TextFormat tf;
        public string drawText;
        //---------------------------
        #region keyList
        private static List<keyID> _keyList = new List<keyID>() {
        new keyID(0x04, "A"),
            new keyID(0x05, "B"),
            new keyID(0x06, "C"),
            new keyID(0x07, "D"),
            new keyID(0x08, "E"),
            new keyID(0x09, "F"),
            new keyID(0x0A, "G"),
            new keyID(0x0B, "H"),
            new keyID(0x0C, "I"),
            new keyID(0x0D, "J"),
            new keyID(0x0E, "K"),
            new keyID(0x0F, "L"),
            new keyID(0x10, "M"),
            new keyID(0x11, "N"),
            new keyID(0x12, "O"),
            new keyID(0x13, "P"),
            new keyID(0x14, "Q"),
            new keyID(0x15, "R"),
            new keyID(0x16, "S"),
            new keyID(0x17, "T"),
            new keyID(0x18, "U"),
            new keyID(0x19, "V"),
            new keyID(0x1A, "W"),
            new keyID(0x1B, "X"),
            new keyID(0x1C, "Y"),
            new keyID(0x1D, "Z"),
            new keyID(0x1E, "1"),
            new keyID(0x1F, "2"),
            new keyID(0x20, "3"),
            new keyID(0x21, "4"),
            new keyID(0x22, "5"),
            new keyID(0x23, "6"),
            new keyID(0x24, "7"),
            new keyID(0x25, "8"),
            new keyID(0x26, "9"),
            new keyID(0x27, "0"),
            new keyID(0x28, "Enter"),
            new keyID(0x29, "Esc"),
            new keyID(0x2A, "Back"),
            new keyID(0x2B, "Tab"),
            new keyID(0x2C, "Spacebar"),
            new keyID(0x2D, "-"),
            new keyID(0x2E, "="),
            new keyID(0x2F, "["),
            new keyID(0x30, "]"),
            new keyID(0x31, "\\"),
            //
            new keyID(0x33, ";"),
            new keyID(0x34, "'"),
            new keyID(0x35, "`"),
            new keyID(0x36, ","),
            new keyID(0x37, "."),
            new keyID(0x38, "/"),
            new keyID(0x39, "Cap"),
            new keyID(0x3A, "F1"),
            new keyID(0x3B, "F2"),
            new keyID(0x3C, "F3"),
            new keyID(0x3D, "F4"),
            new keyID(0x3E, "F5"),
            new keyID(0x3F, "F6"),
            new keyID(0x40, "F7"),
            new keyID(0x41, "F8"),
            new keyID(0x42, "F9"),
            new keyID(0x43, "F10"),
            new keyID(0x44, "F11"),
            new keyID(0x45, "F12"),
            new keyID(0x46, "Print Screen"),
            //new keyID(0x47, "FN Switch"),
            new keyID(0x48, "Pause"),
            new keyID(0x49, "Insert"),
            new keyID(0x4A, "Home"),
            new keyID(0x4B, "Page Up"),
            new keyID(0x4C, "Delete"),
            new keyID(0x4D, "End"),
            new keyID(0x4E, "Page Down"),
            new keyID(0x4F, "→"),
            new keyID(0x50, "←"),
            new keyID(0x51, "↓"),
            new keyID(0x52, "↑"),
            new keyID(0x53, "Num Lock"),
            new keyID(0x54, "KP /"),
            new keyID(0x55, "KP *"),
            new keyID(0x56, "KP -"),
            new keyID(0x57, "KP +"),
            new keyID(0x58, "KP Enter"),
            new keyID(0x59, "KP 1"),
            new keyID(0x5A, "KP 2"),
            new keyID(0x5B, "KP 3"),
            new keyID(0x5C, "KP 4"),
            new keyID(0x5D, "KP 5"),
            new keyID(0x5E, "KP 6"),
            new keyID(0x5F, "KP 7"),
            new keyID(0x60, "KP 8"),
            new keyID(0x61, "KP 9"),
            new keyID(0x62, "KP 0"),
            new keyID(0x63, "KP ."),
            //----------------------------------
            new keyID(0xC0, "Mute"),
            new keyID(0xC1, "Volume Up"),
            new keyID(0xC2, "Volume Down"),
            new keyID(0xC3, "Play"),
            new keyID(0xC4, "Stop"),
            new keyID(0xC5, "Previous Track"),
            new keyID(0xC6, "Next Track"),
            new keyID(0xC7, "Mail"),
            new keyID(0xC8, "Calculator"),
            new keyID(0xC9, "Web Search"),
            new keyID(0xCA, "Web Home"),
            new keyID(0xCB, "Web Favorites"),
            new keyID(0xCC, "Web Refresh"),
            new keyID(0xCD, "Web Stop"),
            new keyID(0xCE, "Web Forward"),
            new keyID(0xCF, "Web Back"),
            //----------------------------------
            new keyID(0xE0, "L Ctrl"),
            new keyID(0xE1, "L Shift"),
            new keyID(0xE2, "L Alt"),
            new keyID(0xE3, "L Win"),
            new keyID(0xE4, "R Ctrl"),
            new keyID(0xE5, "R Shift"),
            new keyID(0xE6, "R Alt"),
            new keyID(0xE7, "R Win"),
            //----------------------------------
            new keyID(0xFE, "Fn"),
            new keyID(0xFF, "[  ]"),
    };
        #endregion
        public Key(float _x, float _y, keySize _size, string _Name, float _sizeScale = 2.2f)
        {
            x = _x;
            y = _y;
            size = _size;
            Name = _Name;
            ShowName = _Name;
            Hex = getKey(_Name).getHex();
            sizeScale = _sizeScale;
        }
        public Key(float _x, float _y, keySize _size, string _Name, string _ShowName, float _sizeScale = 2.2f)
        {
            x = _x;
            y = _y;
            size = _size;
            Name = _Name;
            ShowName = _ShowName;
            Hex = getKey(_Name).getHex();
            sizeScale = _sizeScale;
        }

        public static keyID getKey(string name)
        {
            foreach (keyID kID in _keyList)
            {
                if (kID.getName().Equals(name))
                {
                    return kID;
                }
            }
            return null;
        }

        public static keyID getKey(byte _Hex)
        {
            foreach (keyID kID in _keyList)
            {
                if (kID.getHex() == _Hex)
                {
                    return kID;
                }
            }
            return null;
        }
    }
}
