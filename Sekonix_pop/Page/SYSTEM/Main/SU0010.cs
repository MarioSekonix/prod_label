using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using System.Reflection;

namespace Sekonix_pop
{
    public partial class Main2 : Form
    {
        private int childFormNumber = 0;

        public Main2()
        {
            InitializeComponent();
        }

        private void ShowNewForm(object sender, EventArgs e)
        {
            Form childForm = new Form();
            childForm.MdiParent = this;
            childForm.Text = "창 " + childFormNumber++;
            childForm.Show();
        }

        private void OpenFile(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            openFileDialog.Filter = "텍스트 파일 (*.txt)|*.txt|모든 파일 (*.*)|*.*";
            if (openFileDialog.ShowDialog(this) == DialogResult.OK)
            {
                string FileName = openFileDialog.FileName;
            }
        }

        private void SaveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            saveFileDialog.Filter = "텍스트 파일 (*.txt)|*.txt|모든 파일 (*.*)|*.*";
            if (saveFileDialog.ShowDialog(this) == DialogResult.OK)
            {
                string FileName = saveFileDialog.FileName;
            }
        }

        private void ExitToolsStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void CutToolStripMenuItem_Click(object sender, EventArgs e)
        {
        }

        private void CopyToolStripMenuItem_Click(object sender, EventArgs e)
        {
        }

        private void PasteToolStripMenuItem_Click(object sender, EventArgs e)
        {
        }


        private void CascadeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LayoutMdi(MdiLayout.Cascade);
        }

        private void TileVerticalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LayoutMdi(MdiLayout.TileVertical);
        }

        private void TileHorizontalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LayoutMdi(MdiLayout.TileHorizontal);
        }

        private void ArrangeIconsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LayoutMdi(MdiLayout.ArrangeIcons);
        }

        private void CloseAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (Form childForm in MdiChildren)
            {
                childForm.Close();
            }
        }

        private void Main_Load(object sender, EventArgs e)
        {
            /*
            fmLogin fLogin = new fmLogin();

            fLogin.ShowDialog(this);

            if (!fLogin.bLogin) this.Close();
            */
        }

        private void treeView1_DoubleClick(object sender, EventArgs e)
        {
            Form frm = null;

            //트리노드이름으로 창이 있는지 검색
            foreach (Form tmpFrm in this.MdiChildren)
            {
                if (tmpFrm.Name == this.tree_Menu.SelectedNode.Name)
                {
                    frm = tmpFrm;
                }
            }

            //이미 창이 있다면
            if (frm != null)
            {
                //해당 창으로 이동
                frm.Focus();
            }
            else //창이 없다면
            {

                #region < 프로그램 assem을 가지고 콘트롤 하는 방법 : 복잡 응용가능 >
                try
                {
                    if (this.tree_Menu.SelectedNode.Parent == null) return;

                    //프로그램에 검색자 만듬 
                    string sMenuName = "Sekonix_pop." + this.tree_Menu.SelectedNode.Parent.Parent.Name + "." + this.tree_Menu.SelectedNode.Parent.Name + "." + this.tree_Menu.SelectedNode.Name;

                    //현재 프로그램에 어셈블리를 가져옴
                    Assembly assem = Assembly.GetExecutingAssembly();

                    //노드이름과 같은 클래스를 신규생성
                    object o = assem.CreateInstance(sMenuName);

                    //클래스생성 체크
                    if (o == null) return;

                    //생성된 클래스에 MdiParent 속성을 가져옴
                    PropertyInfo pMdiParnet = assem.GetType(sMenuName).GetProperty("MdiParent");
                    //속성값으로 자신(this)를 지정                
                    pMdiParnet.SetValue(o, this, null);

                    //Show 멤버 실행
                    assem.GetType(sMenuName).InvokeMember("Show", BindingFlags.InvokeMethod, null, o, new object[] { });
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
                #endregion


                #region < 폼으로 강제 변환후 사용하는 방법 : 간단한 방법임 >
                //object o = Activator.CreateInstance(Type.GetType(sMenuName));
                //Form frm = (Form)o;

                //frm.MdiParent = this;
                //frm.Show();
                #endregion
            }
        }
    }
}
