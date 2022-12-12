using System;
using System.Management;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace EasyControl
{
    public partial class MainForm : Form
    {
        #region 输入法
        private const int WM_IME_SETCONTEXT = 0x0281;
        private const int WM_IME_CHAR = 0x0286;
        private const int WM_CHAR = 0x0102;
        private const int WM_IME_COMPOSITION = 0x010F;
        private const int GCS_COMPSTR = 0x0008;
        [DllImport("Imm32.dll")]
        public static extern IntPtr ImmGetContext(IntPtr hWnd);
        [DllImport("Imm32.dll")]
        public static extern IntPtr ImmAssociateContext(IntPtr hWnd, IntPtr hIMC);
        [DllImport("imm32.dll")]
        static extern int ImmGetCompositionString(IntPtr hIMC, int dwIndex, StringBuilder lpBuf, int dwBufLen);
        [DllImport("imm32.dll")]
        public static extern bool ImmSetCompositionWindow(IntPtr hIMC, ref COMPOSITIONFORM lpCompForm);
        private int GCS_RESULTSTR = 0x0800;
        private const int HC_ACTION = 0;
        private const int PM_REMOVE = 0x0001;
        #endregion
        /////////////////////////////////////////////////////////////////////////////////////////////////////
        public MainForm()
        {
            InitializeComponent();
            this.panelDx2D.MouseWheel += new MouseEventHandler(panelDx2D_MouseWheel);
            Text = "Easy Control ver" + JoyConst.version1 + "." + JoyConst.version2 + "." + JoyConst.version3;
            string serverVer = "";
            if (NetMQServer.GetControlVersion(out serverVer))
            {
                if (!serverVer.Equals(""))
                {
                    Text += " (" + serverVer + ")";
                    PublicData.LastVersion = ServerState.OldVersion;
                }
                else
                {
                    Text += " ( Last version )";
                    PublicData.LastVersion = ServerState.LastVersion;
                }
            }
            else
            {
                Text += " ( OffLine )";
                PublicData.LastVersion = ServerState.Offline;

                //MessageBox.Show("Server Error !");
                //System.Environment.Exit(System.Environment.ExitCode);
            }
            try
            {
                #region Debug窗口
                PublicData.Debug = Localization.Instance.GetDebug();
#if DEBUG
                PublicData.Debug = true;
#endif
                if (PublicData.Debug)
                    DebugConstol.Open();
                else
                    DebugConstol.Close();
                #endregion
                #region Init 注意顺序
                JoyEvent.Instance.Dx2DInit += OnDx2DInit;
                //-------------------------------------------------------------------------------------------------
                Dx2D.Instance.Init(this, panelDx2D);                                                   //初始化dx2d，必须放最后
                #endregion
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + ex.StackTrace);
            }
        }
        private void OnDx2DInit(object sender, EventArgs e)
        {
            JoyUSB.Instance.Init();//USB
            //-------------------------------------------------------------------------------------------------
            MainUI.Instance.Init();                                                                         //主UI
            Dx2D.Instance.render = MainUI.Instance;
        }
        private void MainForm_Load(object sender, EventArgs e)
        {
            PublicData.m_hImc = ImmGetContext(this.Handle);
        }
        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            NodeLinkControl.Instance.Save();
            //窗体关闭原因为单击"关闭"按钮或Alt+F4  
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;           //取消关闭操作 表现为不关闭窗体  
                this.Hide();               //隐藏窗体  
            }
        }
        #region 输入法
        protected override void DefWndProc(ref Message m)
        {
            if (m.Msg == WM_IME_SETCONTEXT && m.WParam.ToInt32() == 1)
            {
                ImmAssociateContext(this.Handle, PublicData.m_hImc);
            }
            foreach (InterfacePlugin ip in UI_PluginControl.pluginList.Values)
            {
                DefWndProcHandler handler = new DefWndProcHandler(ip.DefWndProc);
                handler.BeginInvoke(m.Msg, null, null);
                //ip.DefWndProc(m.Msg);//插件事件处理
            }
            switch (m.Msg)
            {
                case WM_IME_COMPOSITION:
                    if (PublicData.currentTextEdit != null)
                    {
                        COMPOSITIONFORM cf = new COMPOSITIONFORM();
                        cf.dwStyle = 2;
                        cf.ptCurrentPos.X = (int)(PublicData.currentTextEdit.DrawRect.X + PublicData.currentTextEdit.Offset.X);
                        cf.ptCurrentPos.Y = (int)(PublicData.currentTextEdit.DrawRect.Y + PublicData.currentTextEdit.Offset.Y +
                            PublicData.currentTextEdit.DrawRect.Height);
                        bool setcom = ImmSetCompositionWindow(PublicData.m_hImc, ref cf);
                    }
                    break;
                case WM_CHAR:
                    KeyEventArgs e = new KeyEventArgs(((Keys)((int)((long)m.WParam))) | ModifierKeys);
                    char a = (char)e.KeyData; //英文
                    if (!a.Equals('\b') && !a.Equals('\t') && !a.Equals('\r') && PublicData.currentTextEdit != null)
                        PublicData.currentTextEdit.AppendText(a);
                    break;
                case WM_IME_CHAR:
                    if (m.WParam.ToInt32() == PM_REMOVE) //如果不做这个判断.会打印出重复的中文
                    {
                        StringBuilder str = new StringBuilder();
                        int size = ImmGetCompositionString(PublicData.m_hImc, GCS_COMPSTR, null, 0);
                        size += sizeof(Char);
                        ImmGetCompositionString(PublicData.m_hImc, GCS_RESULTSTR, str, size);
                        if (!str.Equals('\b') && !str.Equals('\t') && PublicData.currentTextEdit != null)
                            PublicData.currentTextEdit.AppendText(str.ToString());
                        WarningForm.Instance.OpenUI(str.ToString(), false);
                    }
                    break;
            }
            base.DefWndProc(ref m);
        }
        private void MainForm_KeyDown(object sender, KeyEventArgs e)
        {
            //退格处理
            switch (e.KeyCode)
            {
                case Keys.Back:
                    PublicData.currentTextEdit.RemoveText();
                    break;
                case Keys.Delete:
                    PublicData.currentTextEdit.DeleteText();
                    break;
                case Keys.Left:
                    PublicData.currentTextEdit.CurrentIndex++;
                    break;
                case Keys.Right:
                    PublicData.currentTextEdit.CurrentIndex--;
                    break;
                case Keys.X:
                    if ((Control.ModifierKeys & Keys.Control) == Keys.Control)
                    {
                        JoyEvent.Instance.CutClick(null, null);
                    }
                    break;
                case Keys.C:
                    if ((Control.ModifierKeys & Keys.Control) == Keys.Control)
                    {
                        JoyEvent.Instance.CopyClick(null, null);
                    }
                    break;
                case Keys.V:
                    if ((Control.ModifierKeys & Keys.Control) == Keys.Control)
                    {
                        JoyEvent.Instance.PasteClick(null, null);
                    }
                    break;
            }
        }
        #endregion
        #region 界面缩放
        private void MainForm_Resize(object sender, EventArgs e)
        {
        }
        private void panelDx2D_Resize(object sender, EventArgs e)
        {
            Dx2D.Instance.Resize(panelDx2D.Width, panelDx2D.Height);
        }
        #endregion
        #region 鼠标操作
        private void panelDx2D_MouseDown(object sender, MouseEventArgs e)
        {
            PublicData.MouseX = e.X;
            PublicData.MouseY = e.Y;
            MainUI.Instance.JoyMouseDownEvent(e);
            if (e.Button == MouseButtons.Middle)
            {
                PublicData.MoveFrom = true;
                PublicData.MoveFromX = MousePosition.X;
                PublicData.MoveFromY = MousePosition.Y;
            }
        }

        private void panelDx2D_MouseUp(object sender, MouseEventArgs e)
        {
            PublicData.MouseX = e.X;
            PublicData.MouseY = e.Y;
            MainUI.Instance.JoyMouseUpEvent(e);
            if (e.Button == MouseButtons.Middle)
            {
                PublicData.MoveFromX = 0;
                PublicData.MoveFromY = 0;
                PublicData.MoveFrom = false;
            }
        }

        private void panelDx2D_MouseMove(object sender, MouseEventArgs e)
        {
            PublicData.MouseX = e.X;
            PublicData.MouseY = e.Y;
            MainUI.Instance.JoyMouseMoveEvent(e);
            if (PublicData.MoveFrom)
            {
                this.Left += MousePosition.X - PublicData.MoveFromX;
                this.Top += MousePosition.Y - PublicData.MoveFromY;
                PublicData.MoveFromX = MousePosition.X;
                PublicData.MoveFromY = MousePosition.Y;
            }
        }
        private void panelDx2D_MouseWheel(object sender, MouseEventArgs e)
        {
            PublicData.MouseX = e.X;
            PublicData.MouseY = e.Y;
            MainUI.Instance.JoyMouseMoveWheel(e);
        }
        #endregion

        private void panelDx2D_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            //if (this.FormBorderStyle == FormBorderStyle.None)
            //{
            //    this.FormBorderStyle = FormBorderStyle.Sizable;
            //}
            //else
            //{
            //    this.FormBorderStyle = FormBorderStyle.None;
            //    this.WindowState = FormWindowState.Maximized;
            //}
        }

        private void 显示ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Show();                                //窗体显示  
            this.WindowState = FormWindowState.Normal;  //窗体状态默认大小  
            this.Activate();                            //激活窗体给予焦点
        }

        private void notifyIcon1_DoubleClick(object sender, EventArgs e)
        {
            this.Show();                                //窗体显示  
            this.WindowState = FormWindowState.Normal;  //窗体状态默认大小  
            this.Activate();                            //激活窗体给予焦点
        }

        private void 退出ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //点击"是(YES)"退出程序  
            if (MessageBox.Show("确定要退出程序?", "安全提示",
                        System.Windows.Forms.MessageBoxButtons.YesNo,
                        System.Windows.Forms.MessageBoxIcon.Warning)
                == System.Windows.Forms.DialogResult.Yes)
            {
                notifyIcon1.Visible = false;   //设置图标不可见  
                TCPServer.Instance.StopServer();
                this.Close();                  //关闭窗体  
                this.Dispose();                //释放资源  
                Application.Exit();            //关闭应用程序窗体  
            }
        }
    }
}
