using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyControl
{
    public class KeyBoardData
    {
        public const float maxWidthCount = 22f;
        public const float maxHeightCount = 7f;
        //---------------------------------------
        public List<Key> btnList = new List<Key>();
        //---------------------------------------
        public KeyBoardData()
        {
            //==============================================
            btnList.Add(new Key(0f, 0f, keySize.x1, "Esc"));
            //----------
            btnList.Add(new Key(1.5f, 0f, keySize.x1, "F1"));
            btnList.Add(new Key(2.5f, 0f, keySize.x1, "F2"));
            btnList.Add(new Key(3.5f, 0f, keySize.x1, "F3"));
            btnList.Add(new Key(4.5f, 0f, keySize.x1, "F4"));
            btnList.Add(new Key(6f, 0f, keySize.x1, "F5"));
            btnList.Add(new Key(7f, 0f, keySize.x1, "F6"));
            btnList.Add(new Key(8f, 0f, keySize.x1, "F7"));
            btnList.Add(new Key(9f, 0f, keySize.x1, "F8"));
            btnList.Add(new Key(10.5f, 0f, keySize.x1, "F9"));
            btnList.Add(new Key(11.5f, 0f, keySize.x1, "F10"));
            btnList.Add(new Key(12.5f, 0f, keySize.x1, "F11"));
            btnList.Add(new Key(13.5f, 0f, keySize.x1, "F12"));
            //------------------------------------------------------------------------------
            btnList.Add(new Key(0f, 1f, keySize.x1, "`"));
            btnList.Add(new Key(1f, 1f, keySize.x1, "1"));
            btnList.Add(new Key(2f, 1f, keySize.x1, "2"));
            btnList.Add(new Key(3f, 1f, keySize.x1, "3"));
            btnList.Add(new Key(4f, 1f, keySize.x1, "4"));
            btnList.Add(new Key(5f, 1f, keySize.x1, "5"));
            btnList.Add(new Key(6f, 1f, keySize.x1, "6"));
            btnList.Add(new Key(7f, 1f, keySize.x1, "7"));
            btnList.Add(new Key(8f, 1f, keySize.x1, "8"));
            btnList.Add(new Key(9f, 1f, keySize.x1, "9"));
            btnList.Add(new Key(10f, 1f, keySize.x1, "0"));
            btnList.Add(new Key(11f, 1f, keySize.x1, "-"));
            btnList.Add(new Key(12f, 1f, keySize.x1, "="));
            btnList.Add(new Key(13f, 1f, keySize.x15, "Back"));
            //------------------------------------------------------------------------------
            btnList.Add(new Key(0f, 2f, keySize.x125, "Tab"));
            btnList.Add(new Key(1.25f, 2f, keySize.x1, "Q"));
            btnList.Add(new Key(2.25f, 2f, keySize.x1, "W"));
            btnList.Add(new Key(3.25f, 2f, keySize.x1, "E"));
            btnList.Add(new Key(4.25f, 2f, keySize.x1, "R"));
            btnList.Add(new Key(5.25f, 2f, keySize.x1, "T"));
            btnList.Add(new Key(6.25f, 2f, keySize.x1, "Y"));
            btnList.Add(new Key(7.25f, 2f, keySize.x1, "U"));
            btnList.Add(new Key(8.25f, 2f, keySize.x1, "I"));
            btnList.Add(new Key(9.25f, 2f, keySize.x1, "O"));
            btnList.Add(new Key(10.25f, 2f, keySize.x1, "P"));
            btnList.Add(new Key(11.25f, 2f, keySize.x1, "["));
            btnList.Add(new Key(12.25f, 2f, keySize.x1, "]"));
            btnList.Add(new Key(13.25f, 2f, keySize.x125, "\\"));
            //------------------------------------------------------------------------------
            btnList.Add(new Key(0f, 3f, keySize.x15, "Cap"));
            btnList.Add(new Key(1.5f, 3f, keySize.x1, "A"));
            btnList.Add(new Key(2.5f, 3f, keySize.x1, "S"));
            btnList.Add(new Key(3.5f, 3f, keySize.x1, "D"));
            btnList.Add(new Key(4.5f, 3f, keySize.x1, "F"));
            btnList.Add(new Key(5.5f, 3f, keySize.x1, "G"));
            btnList.Add(new Key(6.5f, 3f, keySize.x1, "H"));
            btnList.Add(new Key(7.5f, 3f, keySize.x1, "J"));
            btnList.Add(new Key(8.5f, 3f, keySize.x1, "K"));
            btnList.Add(new Key(9.5f, 3f, keySize.x1, "L"));
            btnList.Add(new Key(10.5f, 3f, keySize.x1, ";"));
            btnList.Add(new Key(11.5f, 3f, keySize.x1, "'"));
            btnList.Add(new Key(12.5f, 3f, keySize.x2, "Enter"));
            //------------------------------------------------------------------------------
            btnList.Add(new Key(0f, 4f, keySize.x2, "L Shift", "Shift"));
            btnList.Add(new Key(2f, 4f, keySize.x1, "Z"));
            btnList.Add(new Key(3f, 4f, keySize.x1, "X"));
            btnList.Add(new Key(4f, 4f, keySize.x1, "C"));
            btnList.Add(new Key(5f, 4f, keySize.x1, "V"));
            btnList.Add(new Key(6f, 4f, keySize.x1, "B"));
            btnList.Add(new Key(7f, 4f, keySize.x1, "N"));
            btnList.Add(new Key(8f, 4f, keySize.x1, "M"));
            btnList.Add(new Key(9f, 4f, keySize.x1, ","));
            btnList.Add(new Key(10f, 4f, keySize.x1, "."));
            btnList.Add(new Key(11f, 4f, keySize.x1, "/"));
            btnList.Add(new Key(12f, 4f, keySize.x25, "R Shift", "Shift"));
            //------------------------------------------------------------------------------
            btnList.Add(new Key(0f, 5f, keySize.x15, "L Ctrl", "Ctrl"));
            btnList.Add(new Key(1.5f, 5f, keySize.x15, "L Win", "Win"));
            btnList.Add(new Key(3f, 5f, keySize.x15, "L Alt", "Alt"));
            btnList.Add(new Key(4.5f, 5f, keySize.x55, "Spacebar"));
            btnList.Add(new Key(10f, 5f, keySize.x15, "R Alt", "Alt"));
            btnList.Add(new Key(11.5f, 5f, keySize.x15, "R Win", "Win"));
            btnList.Add(new Key(13f, 5f, keySize.x15, "R Ctrl", "Ctrl"));
            //==============================================
            btnList.Add(new Key(14.75f, 0f, keySize.x25, "Print Screen"));
            //btnList.Add(new Key(17.25f, 0f, keySize.x25, "FN Switch"));
            btnList.Add(new Key(19.75f, 0f, keySize.x2, "Pause"));
            //------------------------------------------------------------------------------
            btnList.Add(new Key(14.75f, 1f, keySize.x15, "Insert", 3f));
            btnList.Add(new Key(16.25f, 1f, keySize.x15, "Home", 3f));
            btnList.Add(new Key(16.25f, 2f, keySize.x15, "Page Up", "Page Up", 3f));
            //------------------------------------------------------------------------------
            btnList.Add(new Key(14.75f, 2f, keySize.x15, "Delete", 3f));
            btnList.Add(new Key(14.75f, 3f, keySize.x15, "End", 3f));
            btnList.Add(new Key(16.25f, 3f, keySize.x15, "Page Down", "Page Down", 3f));
            //------------------------------------------------------------------------------
            btnList.Add(new Key(15.75f, 4f, keySize.x1, "↑"));
            //------------------------------------------------------------------------------
            btnList.Add(new Key(14.75f, 5f, keySize.x1, "←"));
            btnList.Add(new Key(15.75f, 5f, keySize.x1, "↓"));
            btnList.Add(new Key(16.75f, 5f, keySize.x1, "→"));
            //==============================================
            btnList.Add(new Key(18f, 1f, keySize.x1, "Num Lock", "NL"));
            btnList.Add(new Key(19f, 1f, keySize.x1, "KP /", "/"));
            btnList.Add(new Key(20f, 1f, keySize.x1, "KP *", "*"));
            btnList.Add(new Key(21f, 1f, keySize.x1, "KP -", "-"));
            //------------------------------------------------------------------------------
            btnList.Add(new Key(18f, 2f, keySize.x1, "KP 7", "7"));
            btnList.Add(new Key(19f, 2f, keySize.x1, "KP 8", "8"));
            btnList.Add(new Key(20f, 2f, keySize.x1, "KP 9", "9"));
            btnList.Add(new Key(21f, 2f, keySize.x2H, "KP +", "+"));
            //------------------------------------------------------------------------------
            btnList.Add(new Key(18f, 3f, keySize.x1, "KP 4", "4"));
            btnList.Add(new Key(19f, 3f, keySize.x1, "KP 5", "5"));
            btnList.Add(new Key(20f, 3f, keySize.x1, "KP 6", "6"));
            //------------------------------------------------------------------------------
            btnList.Add(new Key(18f, 4f, keySize.x1, "KP 1", "1"));
            btnList.Add(new Key(19f, 4f, keySize.x1, "KP 2", "2"));
            btnList.Add(new Key(20f, 4f, keySize.x1, "KP 3", "3"));
            btnList.Add(new Key(21f, 4f, keySize.x2H, "KP Enter", "┘"));
            //------------------------------------------------------------------------------
            btnList.Add(new Key(18f, 5f, keySize.x2, "KP 0", "0"));
            btnList.Add(new Key(20f, 5f, keySize.x1, "KP .", "."));
            //==============================================
            btnList.Add(new Key(0f, 6f, keySize.x1, "Fn", "Fn"));
            btnList.Add(new Key(1.25f, 6f, keySize.x1, "Mute", "♪x"));
            btnList.Add(new Key(2.25f, 6f, keySize.x1, "Volume Up", "♪▲"));
            btnList.Add(new Key(3.25f, 6f, keySize.x1, "Volume Down", "♪▼"));
            btnList.Add(new Key(4.25f, 6f, keySize.x1, "Play", ">>"));
            btnList.Add(new Key(5.25f, 6f, keySize.x1, "Stop", "■"));
            btnList.Add(new Key(6.25f, 6f, keySize.x1, "Previous Track", "|<"));
            btnList.Add(new Key(7.25f, 6f, keySize.x1, "Next Track", ">|"));
            btnList.Add(new Key(8.5f, 6f, keySize.x1, "Mail", "@"));
            btnList.Add(new Key(9.5f, 6f, keySize.x1, "Calculator", "⅓"));
            btnList.Add(new Key(10.75f, 6f, keySize.x15, "Web Search", "Search", 3f));
            btnList.Add(new Key(12.25f, 6f, keySize.x15, "Web Home", "Home", 3f));
            btnList.Add(new Key(13.75f, 6f, keySize.x15, "Web Favorites", "Favorites", 3f));
            btnList.Add(new Key(15.25f, 6f, keySize.x15, "Web Refresh", "Refresh", 3f));
            btnList.Add(new Key(16.75f, 6f, keySize.x15, "Web Stop", "Stop", 3f));
            btnList.Add(new Key(18.25f, 6f, keySize.x15, "Web Forward", "Forward", 3f));
            btnList.Add(new Key(19.75f, 6f, keySize.x15, "Web Back", "Back", 3f));
        }
    }
}
