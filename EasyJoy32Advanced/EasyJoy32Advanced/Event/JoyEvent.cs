using System;
using System.Windows.Forms;

namespace EasyControl
{
    public class JoyEvent
    {
        //-------------------------------------------------------------
        public EventHandler Dx2DInit;
        //-------------------------------------------------------------
        public EventHandler CutClick;
        public EventHandler CopyClick;
        public EventHandler PasteClick;
        ///////////////////////////////////////////////////////////////////////////////////////////
        public static readonly JoyEvent Instance = new JoyEvent();
        private JoyEvent()
        {
            //-------------------------------------------------------------
            Dx2DInit += DoNothing;
            //-------------------------------------------------------------
            CutClick += DoNothing;
            CopyClick += DoNothing;
            PasteClick += DoNothing;
        }

        private void DoNothing(object sender, MouseEventArgs e)
        {
        }
        private void DoNothing(object sender, EventArgs e)
        {
        }
    }
}
