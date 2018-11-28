using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Sekonix_pop
{
    public partial class fmLogin : Form
    {
        public bool bLogin = false;

        public fmLogin()
        {
            InitializeComponent();
        }

        private void btn_cancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btn_login_Click(object sender, EventArgs e)
        {
            bLogin = true;
            this.Close();
        }

        private void fmLogin_Load(object sender, EventArgs e)
        {
            //로그인창 위치잡기
            Rectangle recScreen = Screen.PrimaryScreen.Bounds;

            Point pt = new Point((recScreen.Width / 2) - (this.Size.Width / 2), (recScreen.Height / 2) - (this.Size.Height / 2));

            this.Location = pt;
        }
    }
}
