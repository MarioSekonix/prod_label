using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms; 
using System.Reflection;
using System.IO;
using System.IO.Ports;
using System.Threading;
using System.Diagnostics; 

namespace Sekonix_pop
{
    public partial class Main : Form
    {

        DataCommon dc = new DataCommon();
        DataTable dt = new DataTable();

        public static string pId;
        public static string PName;
        public static Boolean sLoginChk = false;


        //static public ParaCommon para = new ParaCommon();
       
        private int childFormNumber = 0;
         

        public Main()
        {
            InitializeComponent();

            //this.FormClosing += Main_FormClosing;
            //this.notifyIcon1.DoubleClick += notifyIcon1_DoubleClick;
            //this.contextMenuStrip1.Click += contextMenuStrip1_Click;

            //this.tree_Menu.Nodes[0].Expand(); 


            //프로그램 중복실행 방지
            System.Threading.Mutex mut = new System.Threading.Mutex(false, Application.ProductName);
            bool running = !mut.WaitOne(0, false);
            if (running)
            {
                MessageBox.Show("이미 실행중입니다.");

                //notifyIcon1.Visible = false;
                Application.ExitThread();
                Environment.Exit(0);           
            }
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
            if (MessageBox.Show("종료하시겠습니까?", "Secutopia ServerManager",  MessageBoxButtons.YesNo,
                MessageBoxIcon.Question) == DialogResult.Yes)
            {
                //this.notifyIcon1.Visible = false;
                Application.Exit();
            }
             
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

        private void ToolBarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            toolStrip.Visible = toolBarToolStripMenuItem.Checked;
        }

        private void StatusBarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            statusStrip.Visible = statusBarToolStripMenuItem.Checked;
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

        private void tree_Menu_DoubleClick(object sender, EventArgs e)
        {
            Form frm = null;


            try 
            {
               
                if (this.tree_Menu.SelectedNode.Parent.Parent.Parent.Name == "ALL_MENU")
                {
                    //메뉴 권한 체크
                    //SYS_AUTHORITY테이블에서 조회하여 컬럼명이 NAME에서 값이 "화면"일경우 권한 여부 체크
                    //조회조건 : 아이디, DEL_YN, CODE
                    string sSql = "";
                    sSql = "  SELECT NAME  ";
                    sSql += " FROM SYS_AUTHORITY ";
                    sSql += " WHERE 1 = 1      ";
                    sSql += " AND DEL_YN = 'Y' ";
                    sSql += " AND CODE = '" + this.tree_Menu.SelectedNode.Name + "' ";
                    sSql += " AND UPPER(ID)   = '" + Sekonix_pop.Para.USER.ID.ToUpper() + "' ";
                    
                    Console.WriteLine("NAME=" + sSql);

                    dt = this.dc.getTable(sSql);
                    try
                    {
                        if (dt.Rows.Count <= 0)
                        {
                           MessageBox.Show("메뉴 권한이 없습니다");
                           return;
                        }
                    }
                    catch
                    {
                        //MessageBox.Show("ID :" + Sekonix_pop.Para.USER.ID + " 대메뉴 권한이 없습니다");
                    }
                }
                
            }             
            catch(Exception ex)
            {
                Console.WriteLine(ex);
            }


            try
            {
                //스캐너 포트 충돌방지를 위해 포트를 사용하는 창에 한해서 강제 닫아준다
                
                /*
                if (this.tree_Menu.SelectedNode.Name == "SM0070")
                {
                    //트리노드이름으로 창이 있는지 검색
                    foreach (Form tmpFrm in this.MdiChildren)
                    {
                        if (tmpFrm.Name == "MA0050" || tmpFrm.Name == "MA0020" || tmpFrm.Name == "MA0040" || tmpFrm.Name == "QU0210" || tmpFrm.Name == "SM0010" || tmpFrm.Name == "QU0220" || tmpFrm.Name == "MA0030" || tmpFrm.Name == "SM0040" || tmpFrm.Name == "SM0050" || tmpFrm.Name == "SM0060")
                        {
                            tmpFrm.Close();
                        }
                    }
                }

                if (this.tree_Menu.SelectedNode.Name == "MA0050")
                {
                    //트리노드이름으로 창이 있는지 검색
                    foreach (Form tmpFrm in this.MdiChildren)
                    {
                        if (tmpFrm.Name == "SM0070" || tmpFrm.Name == "MA0020" || tmpFrm.Name == "MA0040" || tmpFrm.Name == "QU0210" || tmpFrm.Name == "SM0010" || tmpFrm.Name == "QU0220" || tmpFrm.Name == "MA0030" || tmpFrm.Name == "SM0040" || tmpFrm.Name == "SM0050" || tmpFrm.Name == "SM0060")
                        {
                            tmpFrm.Close();
                        }
                    }
                }

                if (this.tree_Menu.SelectedNode.Name == "SM0060")
                {
                    //트리노드이름으로 창이 있는지 검색
                    foreach (Form tmpFrm in this.MdiChildren)
                    {
                        if (tmpFrm.Name == "SM0070" || tmpFrm.Name == "MA0020" || tmpFrm.Name == "MA0040" || tmpFrm.Name == "QU0210" || tmpFrm.Name == "SM0010" || tmpFrm.Name == "QU0220" || tmpFrm.Name == "MA0030" || tmpFrm.Name == "SM0040" || tmpFrm.Name == "SM0050" || tmpFrm.Name == "MA0050")
                        {
                            tmpFrm.Close();
                        }
                    }
                }
                if (this.tree_Menu.SelectedNode.Name == "SM0050")
                {
                    //트리노드이름으로 창이 있는지 검색
                    foreach (Form tmpFrm in this.MdiChildren)
                    {
                        if (tmpFrm.Name == "SM0070" || tmpFrm.Name == "MA0020" || tmpFrm.Name == "MA0040" || tmpFrm.Name == "QU0210" || tmpFrm.Name == "SM0010" || tmpFrm.Name == "QU0220" || tmpFrm.Name == "MA0030" || tmpFrm.Name == "SM0040" || tmpFrm.Name == "SM0060" || tmpFrm.Name == "MA0050")
                        {
                            tmpFrm.Close();
                        }
                    }
                }
                if (this.tree_Menu.SelectedNode.Name == "SM0040")
                {
                    //트리노드이름으로 창이 있는지 검색
                    foreach (Form tmpFrm in this.MdiChildren)
                    {
                        if (tmpFrm.Name == "SM0070" || tmpFrm.Name == "MA0020" || tmpFrm.Name == "MA0040" || tmpFrm.Name == "QU0210" || tmpFrm.Name == "SM0010" || tmpFrm.Name == "QU0220" || tmpFrm.Name == "MA0030" || tmpFrm.Name == "MA0050" || tmpFrm.Name == "SM0060" || tmpFrm.Name == "MA0050")
                        {
                            tmpFrm.Close();
                        }
                    }
                }
                if (this.tree_Menu.SelectedNode.Name == "MA0030")
                {
                    //트리노드이름으로 창이 있는지 검색
                    foreach (Form tmpFrm in this.MdiChildren)
                    {
                        if (tmpFrm.Name == "SM0070" || tmpFrm.Name == "MA0020" || tmpFrm.Name == "MA0040" || tmpFrm.Name == "QU0210" || tmpFrm.Name == "SM0010" || tmpFrm.Name == "QU0220" || tmpFrm.Name == "SM0040" || tmpFrm.Name == "MA0050" || tmpFrm.Name == "SM0060" || tmpFrm.Name == "MA0050")
                        {
                            tmpFrm.Close();
                        }
                    }
                }

                if (this.tree_Menu.SelectedNode.Name == "MA0020")
                {
                    //트리노드이름으로 창이 있는지 검색
                    foreach (Form tmpFrm in this.MdiChildren)
                    {
                        if (tmpFrm.Name == "SM0070" || tmpFrm.Name == "MA0030" || tmpFrm.Name == "MA0040" || tmpFrm.Name == "QU0210" || tmpFrm.Name == "SM0010" || tmpFrm.Name == "QU0220" || tmpFrm.Name == "SM0040" || tmpFrm.Name == "MA0050" || tmpFrm.Name == "SM0060" || tmpFrm.Name == "MA0050")
                        {
                            tmpFrm.Close();
                        }
                    }
                }

                if (this.tree_Menu.SelectedNode.Name == "QU0210")
                {
                    //트리노드이름으로 창이 있는지 검색
                    foreach (Form tmpFrm in this.MdiChildren)
                    {
                        if (tmpFrm.Name == "SM0070" || tmpFrm.Name == "MA0030" || tmpFrm.Name == "MA0040" || tmpFrm.Name == "MA0020" || tmpFrm.Name == "SM0010" || tmpFrm.Name == "QU0220" || tmpFrm.Name == "SM0040" || tmpFrm.Name == "MA0050" || tmpFrm.Name == "SM0060" || tmpFrm.Name == "MA0050")
                        {
                            tmpFrm.Close();
                        }
                    }
                }

                if (this.tree_Menu.SelectedNode.Name == "MA0040")
                {
                    //트리노드이름으로 창이 있는지 검색
                    foreach (Form tmpFrm in this.MdiChildren)
                    {
                        if (tmpFrm.Name == "SM0070" || tmpFrm.Name == "MA0030" || tmpFrm.Name == "MA0020" || tmpFrm.Name == "QU0210" || tmpFrm.Name == "SM0010" || tmpFrm.Name == "QU0220" || tmpFrm.Name == "SM0040" || tmpFrm.Name == "MA0050" || tmpFrm.Name == "SM0060" || tmpFrm.Name == "MA0050")
                        {
                            tmpFrm.Close();
                        }
                    }
                }

                if (this.tree_Menu.SelectedNode.Name == "SM0010")
                {
                    //트리노드이름으로 창이 있는지 검색
                    foreach (Form tmpFrm in this.MdiChildren)
                    {
                        if (tmpFrm.Name == "SM0070" || tmpFrm.Name == "MA0030" || tmpFrm.Name == "MA0020" || tmpFrm.Name == "QU0210" || tmpFrm.Name == "MA0040" || tmpFrm.Name == "QU0220" || tmpFrm.Name == "SM0040" || tmpFrm.Name == "MA0050" || tmpFrm.Name == "SM0060" || tmpFrm.Name == "MA0050")
                        {
                            tmpFrm.Close();
                        }
                    }
                }

                if (this.tree_Menu.SelectedNode.Name == "QU0220")
                {
                    //트리노드이름으로 창이 있는지 검색
                    foreach (Form tmpFrm in this.MdiChildren)
                    {
                        if (tmpFrm.Name == "SM0070" || tmpFrm.Name == "MA0030" || tmpFrm.Name == "MA0020" || tmpFrm.Name == "QU0210" || tmpFrm.Name == "MA0040" || tmpFrm.Name == "SM0010" || tmpFrm.Name == "SM0040" || tmpFrm.Name == "MA0050" || tmpFrm.Name == "SM0060" || tmpFrm.Name == "MA0050")
                        {
                            tmpFrm.Close();
                        }
                    }
                }
                */
            }
            catch { }



            //트리노드이름으로 창이 있는지 검색
            foreach (Form tmpFrm in this.MdiChildren)
            {
                if (tmpFrm.Name == this.tree_Menu.SelectedNode.Name)
                {
                    frm = tmpFrm;
                    //Console.WriteLine("this.tree_Menu.SelectedNode.Name=" + this.tree_Menu.SelectedNode.Name);
                }
                else
                {
                    tmpFrm.Close();
                }
            }


            //이미 창이 있다면

            if (frm != null ) 
            {
                //해당 창으로 이동
                frm.Focus();
            }                
            else //창이 없다면

            {
                
                #region < 프로그램 assem을 가지고 콘트롤 하는 방법 : 복잡 응용가능 >
                try
                {
                    if (this.tree_Menu.SelectedNode == null) return;

                    if (this.tree_Menu.SelectedNode.Parent == null) return;

                    string sMenuName;

                    //프로그램에 검색자 만듬 
                    try
                    {
                        //Console.WriteLine("########################################################");
                        //Console.WriteLine("this.tree_Menu.SelectedNode.Parent.Parent.Name=" + this.tree_Menu.SelectedNode.Parent.Parent.Name);
                        //Console.WriteLine("this.tree_Menu.SelectedNode.Parent.Name=" + this.tree_Menu.SelectedNode.Parent.Name);
                        //Console.WriteLine("this.tree_Menu.SelectedNode.Name=" + this.tree_Menu.SelectedNode.Name);
                        //Console.WriteLine("########################################################");

                        if (this.tree_Menu.SelectedNode.Name == "PU0050")
                        {
                            sMenuName = "Sekonix_pop.Page.POP.PU0000." + this.tree_Menu.SelectedNode.Name;
                        }
                        else
                        {
                            sMenuName = "Sekonix_pop.Page." + this.tree_Menu.SelectedNode.Parent.Parent.Name + "." + this.tree_Menu.SelectedNode.Parent.Name + "." + this.tree_Menu.SelectedNode.Name;
                        }
                    }
                    catch
                    {
                        //임시
                        sMenuName = "Sekonix_pop.";
                    }

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
                    object t = assem.GetType(sMenuName).InvokeMember("Show", BindingFlags.InvokeMethod, null, o, new object[] { });
                    
                }
                catch( Exception ex )
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

            #region < 일일이 case문으로 대조해 보는 방법 : 수량이 적을때 심플하게 사용 >
            //switch (sMenuName)
            //{ 
            //    case "shops":
            //        if (frmShops == null)
            //        {
            //            frmShops = new Shops();
            //            frmShops.MdiParent = this;
            //        }
            //        frmShops.Show();
            //        break;
            //}
            #endregion

        }

         


        private void Main_Load(object sender, EventArgs e)
        {
            Sekonix_pop.Page.SYS.Main.Login Login = new Sekonix_pop.Page.SYS.Main.Login();

            Login.ShowDialog(this);

            if (!Login.bLogin)
            {
                sLoginChk = false;
                this.Close();
            }

            sLoginChk = true;

            lblTop.Text = Sekonix_pop.Para.USER.NAME + "님 수고하세요. [비밀번호 변경]";

             

            /*
            //버전체크
            if (VersionChk())
            {
                MessageBox.Show("버전이 동일합니다");
            }
            else 
            {
                if (MessageBox.Show("새로운 버전으로 업그레이드합니다.", "공지", MessageBoxButtons.YesNo).ToString() == "Yes")
                {
                    //FTP 다운로드 팝업 호출
                }               
                
            }
             * */


            #region 대메뉴 생성
            string sSql = "";
            sSql = " SELECT CODE , D_CATEGORY  ";
            sSql += "   FROM SYS_AUTHORITY ";
            sSql += " WHERE NAME = '대'      ";
            sSql += " AND UPPER(ID) = '" + Sekonix_pop.Para.USER.ID.ToUpper() + "' ";
            sSql += " ORDER BY SEQ                                ";
            dc = new DataCommon();
            dt = this.dc.getTable(sSql); 
            try
            {
                if (dt.Rows.Count > 0)
                {
                    tree_Menu.Nodes.Add(dt.Rows[0].ItemArray[1].ToString());
                    tree_Menu.Nodes[0].Name = dt.Rows[0].ItemArray[0].ToString();

                }
                else
                {
                    MessageBox.Show("메뉴생성에 문제가 발생하였습니다. 관리자를 호출하세요");
                }
            }
            catch
            {
                //MessageBox.Show("ID :" + Sekonix_pop.Para.USER.ID + " 대메뉴 권한이 없습니다");
            }
            #endregion

            #region 중메뉴 생성
            sSql = " SELECT CODE , D_CATEGORY , J_CATEGORY   ";
            sSql += "   FROM SYS_AUTHORITY ";
            sSql += " WHERE NAME = '중'      ";
            sSql += " AND UPPER(ID) = '" + Sekonix_pop.Para.USER.ID.ToUpper() + "' ";
            sSql += " ORDER BY  SEQ";
            Console.WriteLine("중메뉴=" + sSql);
            dc = new DataCommon();
            dt = this.dc.getTable(sSql);
            try
            {
                for (int d = 0; dt.Rows.Count > d; d++)
                {
                    for (int td = 0; tree_Menu.Nodes.Count > td; td++)
                    {
                        if (tree_Menu.Nodes[td].Text == dt.Rows[d].ItemArray[1].ToString())
                        {
                            tree_Menu.Nodes[td].Nodes.Add(dt.Rows[d].ItemArray[2].ToString());
                            tree_Menu.Nodes[td].Nodes[tree_Menu.Nodes[td].Nodes.Count - 1].Name = dt.Rows[d].ItemArray[0].ToString();
                            tree_Menu.Nodes[td].Expand();
                        }
                     }
                }
            }
            catch
            {
                MessageBox.Show("중메뉴의 권한이 문제가 생겼습니다 \n 관리자에게 연락바랍니다");
            }
            #endregion

            #region 소메뉴 관리
            sSql = " SELECT CODE , D_CATEGORY , J_CATEGORY , S_CATEGORY  ";
            sSql += "   FROM SYS_AUTHORITY ";
            sSql += " WHERE NAME = '소'      ";
            sSql += " AND UPPER(ID) = '" + Sekonix_pop.Para.USER.ID.ToUpper() + "' ";
            Console.WriteLine("소메뉴=" + sSql);
            dc = new DataCommon();
            dt = this.dc.getTable(sSql);

            try
            {
                for (int d = 0; dt.Rows.Count > d; d++)
                {
                    for (int td = 0; tree_Menu.Nodes.Count > td; td++)
                    {
                        if (tree_Menu.Nodes[td].Text == dt.Rows[d].ItemArray[1].ToString())
                        {
                            for (int jd = 0; tree_Menu.Nodes[td].Nodes.Count > jd; jd++)
                            {
                                if (tree_Menu.Nodes[td].Nodes[jd].Text == dt.Rows[d].ItemArray[2].ToString())
                                {
                                    tree_Menu.Nodes[td].Nodes[jd].Nodes.Add(dt.Rows[d].ItemArray[3].ToString());
                                    tree_Menu.Nodes[td].Nodes[jd].Nodes[tree_Menu.Nodes[td].Nodes[jd].Nodes.Count - 1].Name = dt.Rows[d].ItemArray[0].ToString();
                                    tree_Menu.Nodes[td].Nodes[jd].Expand();
                                }
                            }
                        }
                    }
                }
            }
            catch
            {
                MessageBox.Show("소메뉴의 권한이 문제가 생겼습니다 관리자에게 연락바랍니다");
            }
            #endregion

            #region 화면관리
            sSql = " SELECT CODE , D_CATEGORY , J_CATEGORY , S_CATEGORY , SCREEN ";
            sSql += "   FROM SYS_AUTHORITY ";
            sSql += " WHERE NAME = '화면'      ";
            sSql += " AND UPPER(ID) = '" + Sekonix_pop.Para.USER.ID.ToUpper() + "' ";
            sSql += " ORDER BY SEQ ";

            dt = this.dc.getTable(sSql);
            try
            {
                for (int d = 0; dt.Rows.Count > d; d++)
                {
                    for (int td = 0; tree_Menu.Nodes.Count > td; td++)
                    {
                        if (tree_Menu.Nodes[td].Text == dt.Rows[d].ItemArray[1].ToString())
                        {
                            for (int jd = 0; tree_Menu.Nodes[td].Nodes.Count > jd; jd++)
                            {
                                if (tree_Menu.Nodes[td].Nodes[jd].Text == dt.Rows[d].ItemArray[2].ToString())
                                {
                                    for (int sd = 0; tree_Menu.Nodes[td].Nodes[jd].Nodes.Count > sd; sd++)
                                    {
                                        if (tree_Menu.Nodes[td].Nodes[jd].Nodes[sd].Text == dt.Rows[d].ItemArray[3].ToString())
                                        {
                                            tree_Menu.Nodes[td].Nodes[jd].Nodes[sd].Nodes.Add(dt.Rows[d].ItemArray[4].ToString());
                                            tree_Menu.Nodes[td].Nodes[jd].Nodes[sd].Nodes[tree_Menu.Nodes[td].Nodes[jd].Nodes[sd].Nodes.Count - 1].Name = dt.Rows[d].ItemArray[0].ToString();
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch
            {
                MessageBox.Show("화면단위 권한이 문제가 생겼습니다 관리자에게 연락바랍니다");
            }
            #endregion
             
        }

         


        private void optionsToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void pnlLeft_Resize(object sender, EventArgs e)
        {
            if (this.pnlLeft.Size.Width != 0)
            {
                //this.btnOpenClose.Location = new Point(this.pnlLeft.Location.X + this.pnlLeft.Size.Width - (this.btnOpenClose.Size.Width/2), this.btnOpenClose.Location.Y);
                this.btnOpenClose.Location = new Point(this.pnlLeft.Location.X + this.pnlLeft.Size.Width - (this.btnOpenClose.Size.Width / 2), this.pnlLeft.Location.Y + (this.pnlLeft.Size.Height / 2) - (this.btnOpenClose.Size.Height / 2));
                //this.btnOpenClose.Location = new Point(this.pnlLeft.Location.X + this.pnlLeft.Size.Width, this.pnlLeft.Location.Y + (this.pnlLeft.Size.Height/2) - (this.btnOpenClose.Size.Height/2) );
            }
            else
            {
                this.btnOpenClose.Location = new Point(-17, this.btnOpenClose.Location.Y);
            }
        }

        private void btnOpenClose_Click(object sender, EventArgs e)
        {
            if (this.pnlLeft.Size.Width != 0)
            {
                this.btnOpenClose.Text = "열기";
                this.pnlLeft.Size = new Size(0, this.pnlLeft.Size.Height);
            }
            else
            {
                this.btnOpenClose.Text = "닫기";
                this.pnlLeft.Size = new Size(190, this.pnlLeft.Size.Height);
            }


        }

        private void btnOpenClose_MouseEnter(object sender, EventArgs e)
        {
            if (this.pnlLeft.Size.Width == 0)
            {
                this.btnOpenClose.Location = new Point(this.pnlLeft.Location.X + this.pnlLeft.Size.Width, this.btnOpenClose.Location.Y);
            }
        }

        private void btnOpenClose_MouseLeave(object sender, EventArgs e)
        {
            if (this.pnlLeft.Size.Width == 0)
            {
                this.btnOpenClose.Location = new Point(-17, this.btnOpenClose.Location.Y);
            }
        }


        private void clickMenuItem(object sender, EventArgs e)
        {
            Form frm = null;

            
            //트리노드이름으로 창이 있는지 검색

            foreach (Form tmpFrm in this.MdiChildren)
            {
                if (tmpFrm.Name == (sender as ToolStripMenuItem).Tag.ToString() )
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
                    //if (this.tree_Menu.SelectedNode.Parent == null) return;

                    //프로그램에 검색자 만듬 
                    string sMenuName = "Sekonix_pop.Page." + (sender as ToolStripMenuItem).OwnerItem.Tag.ToString() + "." + (sender as ToolStripMenuItem).Tag.ToString();
                    
                    //권한체크
                    try
                    {
                        //if (this.tree_Menu.SelectedNode.Parent.Parent.Parent.Name == "ALL_MENU")
                        //{
                        //메뉴 권한 체크
                        //SYS_AUTHORITY테이블에서 조회하여 컬럼명이 NAME에서 값이 "화면"일경우 권한 여부 체크
                        //조회조건 : 아이디, DEL_YN, CODE
                        string sSql = "";
                        sSql = "  SELECT NAME  ";
                        sSql += " FROM SYS_AUTHORITY ";
                        sSql += " WHERE 1 = 1      ";
                        sSql += " AND DEL_YN = 'Y' ";
                        sSql += " AND SCREEN = '" + sender + "' ";
                        sSql += " AND UPPER(ID)   = '" + Sekonix_pop.Para.USER.ID.ToUpper() + "' ";

                        Console.WriteLine("NAME=" + sSql);
                        dc = new DataCommon();
                        dt = this.dc.getTable(sSql);

                        Console.WriteLine("dt.Rows.Count=" + dt.Rows.Count);

                        try
                        {
                            if (dt.Rows.Count <= 0)
                            {
                                MessageBox.Show("메뉴 권한이 없습니다");
                                return;
                            }
                        }
                        catch
                        {
                            //MessageBox.Show("ID :" + Sekonix_pop.Para.USER.ID + " 대메뉴 권한이 없습니다");
                            return;
                        } 

                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex);
                        return;
                    }


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
            }
        }

        

        private void pictureBox1_DoubleClick(object sender, EventArgs e)
        {
            /*
            frmLogin fLogin = new frmLogin();

            if (fLogin.ShowDialog(this) != DialogResult.Yes)
            {
                return;
            }
            */

            //bool bVis = true;

            //if (this.tree_Menu.Nodes["Menu"].FirstNode.Name != "Basic")
            //{
            //    this.tree_Menu.Nodes["Menu"].Nodes.Insert(0, "Basic","기본관리");
            //    this.tree_Menu.Nodes["Menu"].Nodes["Basic"].Nodes.Insert(0, "PLC","PLC기준정보");
            //}
            //else
            //{
            //    this.tree_Menu.Nodes["Menu"].Nodes["Basic"].Remove();

            //    bVis = false;
            //}

            //foreach (Form frmNow in this.MdiChildren)
            //{
            //    switch (frmNow.Name)
            //    {
            //        case "PPK":
            //            (frmNow as Page.Spc.PPK).btnRFC.Visible = bVis;
            //            break;
            //        case "XRCHART":
            //            (frmNow as Page.Spc.XRCHART).btnRFC.Visible = bVis;
            //            (frmNow as Page.Spc.XRCHART).pnlRadioNg.Visible = bVis;
            //            break;
            //        default:
            //            break;
            //    }
            //}

            /*
            foreach (Form frmNow in this.MdiChildren)
            {
                switch (frmNow.Name)
                {
                    case "PPK":
                        (frmNow as Page.Spc.PPK).btnRFC.Visible = !(frmNow as Page.Spc.PPK).btnRFC.Visible;
                        break;
                    case "XRCHART":
                        (frmNow as Page.Spc.XRCHART).btnRFC.Visible = !(frmNow as Page.Spc.XRCHART).btnRFC.Visible;
                        (frmNow as Page.Spc.XRCHART).pnlRadioNg.Visible = !(frmNow as Page.Spc.XRCHART).pnlRadioNg.Visible;
                        break;
                    case "PLC":
                        (frmNow as Page.Basic.PLC).bt_add.Visible = !(frmNow as Page.Basic.PLC).bt_add.Visible;
                        (frmNow as Page.Basic.PLC).bt_delete.Visible = !(frmNow as Page.Basic.PLC).bt_delete.Visible;
                        (frmNow as Page.Basic.PLC).bt_save.Visible = !(frmNow as Page.Basic.PLC).bt_save.Visible;
                        break;
                    default:
                        break;
                }
            }
            */

        }



        private void menuStrip_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void logOutToolStripMenuItem_Click(object sender, EventArgs e)
        {
             
            if (MessageBox.Show("로그아웃하시겠습니까?", "", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {  

                //트리노드이름으로 창이 있는지 검색
                foreach (Form tmpFrm in this.MdiChildren)
                {
                    tmpFrm.Close(); 
                }

                sLoginChk = false;
                lblTop.Text = "";


                tree_Menu.Nodes.Clear();

                Main_Load(sender, e); 

            }
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Sekonix_pop.Page.SYS.Main.Information Information = new Sekonix_pop.Page.SYS.Main.Information();

            Information.ShowDialog(this);
        }


        public bool VersionChk()
        {
            //파일의 버전정보 추출 
            string strPath = AppDomain.CurrentDomain.BaseDirectory;
            string sFile = Application.StartupPath + @"\version.ini";
            System.IO.StreamReader psReader = new System.IO.StreamReader(sFile, System.Text.Encoding.GetEncoding(949));
            string sVerNo = psReader.ReadToEnd();
            psReader.Close();
            Console.WriteLine("file ver no = " + sVerNo);


            //DB의 버전정보 추출
            int iiVerNo = 0;
            string sQuery = " SELECT VER_NO FROM POP_VERSION ";
            Console.WriteLine("sQuery=" + sQuery);
            dc = new DataCommon();
            iiVerNo = Convert.ToInt32(dc.getSimpleScalar(sQuery).ToString());
            Console.WriteLine("DB ver no = " + iiVerNo);

            if (sVerNo == iiVerNo.ToString())
            {
                return true;
            }

            return false;
        }

         

        private void contextMenuStrip1_Click(object sender, EventArgs e)
        {
            //notifyIcon1.Visible = false;

            Application.ExitThread();
            Environment.Exit(0);
        } 
          
            

        private void notifyIcon1_DoubleClick(object sender, EventArgs e)
        {

            /*
            this.Visible = true;
            
            if (this.WindowState == FormWindowState.Minimized)
            {
                this.WindowState = FormWindowState.Normal;
            }

            this.Activate();
            */
        }


        private void Main_FormClosing(object sender, FormClosingEventArgs e)
        {
            //e.Cancel = true;
            //this.Visible = false;
             
             

        } 

        private void timerLogout_Tick(object sender, EventArgs e)
        {
            //휴식시간 자동 로그아웃
            string sNowTime = DateTime.Now.ToString("HHmmss");
            //Console.WriteLine("now time = " + sNowTime  );

            Boolean isLogOut = false;
            string getLogTimeOut = "";
            string sSql = "";
            sSql += " SELECT OP_CODE, TIME_SET      ";
            sSql += " FROM pop_logout_Set           ";
            sSql += " WHERE 1=1                     ";
            sSql += " AND OP_CODE = 'M100'          ";  //관리자는 OP_CODE가 없으므로 M100의 값으로 로그아웃시킨다
            sSql += " ORDER BY  to_number(time_set) ";
            //Console.WriteLine("중메뉴=" + sSql);
            dc = new DataCommon();
            dt = this.dc.getTable(sSql);
            try
            {
                for (int d = 0; dt.Rows.Count > d; d++)
                {
                    getLogTimeOut = dt.Rows[d].ItemArray[1].ToString() + "00";

                    if (sNowTime == getLogTimeOut)
                    {
                        isLogOut = true;
                        break;
                    }
                }
                //Console.WriteLine("isLogOut=" + isLogOut + " ::: " + sNowTime);
                if (isLogOut)
                {
                    //트리노드이름으로 창이 있는지 검색
                    foreach (Form tmpFrm in this.MdiChildren)
                    {
                        tmpFrm.Close();
                    }

                    sLoginChk = false;

                    lblTop.Text = "";
                    tree_Menu.Nodes.Clear();

                    Main_Load(sender, e);
                }
            }
            catch { }

        }
    }
}
