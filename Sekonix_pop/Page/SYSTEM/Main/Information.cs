using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Sekonix_pop.Page.SYS.Main
{
    public partial class Information : Form
    {

        DataCommon dc = new DataCommon();
        DataTable dt = new DataTable();

        public Information()
        {
            InitializeComponent();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Information_Load(object sender, EventArgs e)
        {
            txtID.Text= Sekonix_pop.Para.USER.ID;
            txtName.Text = Sekonix_pop.Para.USER.NAME;

            //수정창 위치잡기
            Rectangle recScreen = Screen.PrimaryScreen.Bounds;

            Point pt = new Point((recScreen.Width / 2) - (this.Size.Width / 2), (recScreen.Height / 2) - (this.Size.Height / 2));

            this.Location = pt;

            this.txtPW.Select();
             
        }

        private void button1_Click(object sender, EventArgs e)
        {
            String sQuery = "";
            int non_count = 0;
            //패스워드 수정
            //##################################################################################################            

            try
            {
                sQuery = " UPDATE SYS_EMP SET ";
                sQuery += "       NAME  = '" + txtName.Text.Trim() + "',  ";
                sQuery += "       PW    = '" + txtPW.Text.Trim() + "'  ";
                sQuery += " WHERE ID    = '" + txtID.Text.Trim() + "'  ";
                dc = new DataCommon();
                non_count = dc.execNonQuery(sQuery);

                if (non_count != 0)
                {
                    MessageBox.Show("저장 되었습니다. 재로그인시 적용됩니다.");                     
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }

        }
    }
}
