using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;
using System.IO.Ports;
using System.Threading;
using Microsoft.Win32;


namespace Sekonix_pop.Page.SYS.Main
{
    public partial class Login : Form
    {
        DataCommon dc = new DataCommon();
        delegate void SetTextCallback(string Text);

        string sScanPortName = "";

        public bool bLogin = false;


        public Login()
        {
            InitializeComponent();

            global::Sekonix_pop.Properties.Settings.Default.Reload();
            //this.txt_id.Text = global::Sekonix_pop.Properties.Settings.Default.sIdOld;
        }
        
        private void Login_Load(object sender, EventArgs e)
        {
            //로그인창 위치잡기
            Rectangle recScreen = Screen.PrimaryScreen.Bounds;
            Point pt = new Point((recScreen.Width / 2) - (this.Size.Width / 2), (recScreen.Height / 2) - (this.Size.Height / 2));
            this.Location = pt;             
            this.txt_id.Select();


            sScanPortName = (Registry.GetValue(@"HKEY_CURRENT_USER\SEKONIX\PortConfig", "ScanPortName", "") == null ? sScanPortName : (string)Registry.GetValue(@"HKEY_CURRENT_USER\SEKONIX\PortConfig", "ScanPortName", ""));

           
            //시리얼 포트 설정
            this.serialPort1 = new System.IO.Ports.SerialPort();
            this.serialPort1.DataReceived += new SerialDataReceivedEventHandler(serialPort1_DataReceived);

            //스캐너포트
            comboBox1.Items.Add("");
            comboBox1.Items.Add("COM1");
            comboBox1.Items.Add("COM2");
            comboBox1.Items.Add("COM3");
            comboBox1.Items.Add("COM4");
            comboBox1.Items.Add("COM5");
            comboBox1.Items.Add("COM6");
            comboBox1.Items.Add("COM7");
            comboBox1.Items.Add("COM8");
            comboBox1.Items.Add("COM9");

            
           // if (sScanPortName != "")
           // {
                try
                {
                    comboBox1.Text = sScanPortName;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
           // }  
            //SerialPort 초기 설정.  
            //serialPort1.PortName = sScanPortName;
            serialPort1.BaudRate = (int)9600;
            serialPort1.DataBits = (int)8;
            serialPort1.Parity = System.IO.Ports.Parity.None;
            serialPort1.StopBits = StopBits.One;
     
            
            //버젼체크시작 
            string sVerNo = "";
            string sLocalVer = "";
            string line;
            string fileAndPath = @"C:\SEKONIX_MES\version.ini";

            try
            {
                //DB에서 버전 추출
                dc = new DataCommon();
                sVerNo = dc.getSimpleScalar("SELECT VER_NO FROM POP_VERSION").ToString();

                //로컬PC의 version.ini에서 버전 추출                
                System.IO.StreamReader file = new System.IO.StreamReader(fileAndPath);
                while ((line = file.ReadLine()) != null)
                {
                    sLocalVer = line.ToString(); 
                }
                file.Close();

                System.Console.WriteLine("sLocalVer2= " + sLocalVer); 
            }
            catch { }
            
            //서버버젼과 클라이언트 버전 비교         
            if (sVerNo != sLocalVer)
            {

                //#################################
                //version.ini 버전 수정
                //#################################
                try
                {
                    FileStream fileStream = new FileStream(fileAndPath, FileMode.Create, FileAccess.Write);
                    byte[] byteArr = Encoding.UTF8.GetBytes(sVerNo);
                    //FileStream 에 직접 기록
                    fileStream.Write(byteArr, 0, byteArr.Length);
                    //FileStream 닫기
                    fileStream.Close();


                    //다르면 자동업데이트 프로그램 실행
                    string strappname = @"C:\SEKONIX_MES\SimpleDownloader.exe";
                    Process.Start(strappname);
                }
                catch { }

            } 
           
        }


        

        private void btn_cancel_Click(object sender, EventArgs e)
        {
            //this.Close();            

            Application.ExitThread();
            Environment.Exit(0);

        }

        private void btn_login_Click(object sender, EventArgs e)
        {
            // 아이디유무체크
            string loginUid = "";
            string sid = this.txt_id.Text.Trim();
            string spass = this.txt_pass.Text.Trim();


            if (sid == "")
            {
                MessageBox.Show("아이디를 입력해주세요.");
                this.txt_id.Text = "";
                this.txt_pass.Text = "";

                this.txt_id.Focus();
                return;
            }


            try
            {
                dc = new DataCommon();
                loginUid = dc.getSimpleScalar("select id from sys_emp where UPPER(Id) ='" + sid.ToUpper() + "' ").ToString();

            }
            catch
            {

                if (sid.ToString().ToUpper() != loginUid.ToString().ToUpper())
                {
                    //this.lb_msg.Text = "아이디가 없습니다.";
                    MessageBox.Show("아이디가 존재하지 않습니다.");
                    //this.txt_id.Text = "";
                    this.txt_pass.Text = "";

                    this.txt_id.Focus();
                    return;
                }
                else
                {
                    sid = loginUid;
                }
            }

            if (loginUid == "")
            {
                MessageBox.Show("아이디가 존재하지 않습니다.");
                //this.txt_id.Text = "";
                this.txt_pass.Text = "";

                this.txt_id.Focus();
                return;
            }

            try
            {

                if (spass == "")
                {
                    MessageBox.Show("비밀번호가 입력해주세요.");                    
                    this.txt_pass.Text = "";
                    this.txt_pass.Focus();
                    return;
                }



                //string spass2 = dc.getSimpleScalar("select pw from MemberLogin where UserID ='" + sid + "' ").ToString();  
                dc = new DataCommon();
                string spass2 = dc.getSimpleScalar("select Pw from sys_emp where UPPER(Id) ='" + sid.ToUpper() + "' ").ToString();

                if (spass2 == "")
                {
                    MessageBox.Show("비밀번호가 틀렸습니다.");
                    this.txt_pass.Text = "";
                    this.txt_pass.Focus();
                    return;
                }

                if (spass.ToUpper() != spass2.ToUpper())
                {
                    //this.lb_msg.Text = "비밀번호가 틀렸습니다.";
                    MessageBox.Show("비밀번호가 틀렸습니다.");
                    this.txt_pass.Text = "";
                    this.txt_pass.Focus();
                    return;
                }
                else
                {
                    //dc.execNonQuery("update MemberLogin set LastLoginDate = getdate() , LoginSum = LoginSum + 1 where UserId = '" + sid + "' ");

                    dc = new DataCommon();
                    string sName = dc.getSimpleScalar("select NAME from sys_emp where UPPER(Id) ='" + sid.ToUpper() + "' ").ToString();

                    dc = new DataCommon();
                    string sID = dc.getSimpleScalar("select id from sys_emp where UPPER(Id) ='" + sid.ToUpper() + "' ").ToString();


                    //global::Sekonix_pop.Properties.Settings.Default.sIdOld = sid;
                    global::Sekonix_pop.Properties.Settings.Default.Save();
                    //Main.para.sAgentCode = sAreaC;
                    //Sekonix_pop.Main.pId = sId;

                    //아이디담기
                    Para.USER.ID = sID;
                    Para.USER.NAME = sName;

                    bLogin = true;
                    this.Close();
                }
            }
            catch
            {
                MessageBox.Show("DB접속에 문제가 있습니다.");

                //test용

                bLogin = false;
                //this.Close();
            }
        }



        //엔터키 20150722 bkit 안용현
        //private void KeyDown(object sender, KeyEventArgs e)
        //{
        //    if (e.KeyCode == Keys.Enter)
        //        btn_login_Click(sender, e);
        //}



        //아이디 체크
        private void txt_id_KeyPress(object sender, KeyPressEventArgs e)
        {
            
            string sid = this.txt_id.Text.Trim();
            

            if (e.KeyChar == 13)
            {

                if (string.IsNullOrEmpty(sid))
                {
                    //this.lb_msg.Text = "아이디를 입력하세요.";
                    MessageBox.Show("아이디를 입력하세요.");
                    this.txt_id.Text = "";
                    this.txt_id.Focus();
                    return;
                }
                else
                {
                    this.txt_pass.Select();
                }
                 
            }
        }


        //패스워드 체크
        private void txt_pass_KeyPress(object sender, KeyPressEventArgs e)
        {
            string sid = this.txt_id.Text.Trim();
            string spass = this.txt_pass.Text.Trim();

            if (e.KeyChar == 13)
            {

                if (string.IsNullOrEmpty(spass))
                {
                    //this.lb_msg.Text = "비밀번호를 입력하세요.";
                    MessageBox.Show("비밀번호를 입력하세요.");
                    this.txt_pass.Text = "";
                    this.txt_pass.Select();
                    return;
                }
                else
                {
                    btn_login_Click(sender, e);
                }
            }

        }

        private void serialPort1_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            System.Threading.Thread.Sleep(500);

            //포트로 데이터가 들어오면
            if (serialPort1.IsOpen)
            {
                string data = serialPort1.ReadExisting();
                if (data != string.Empty)
                {
                    PacketData(data);
                }
            }  
        }


        //data 처리...!!!!!!!!!!!!!!!!!!!
        private void PacketData(string Text)
        {
            if (this.txt_id.InvokeRequired)
            {
                //스캔 종료
                SetTextCallback d = new SetTextCallback(PacketData);
                this.Invoke(d, new object[] { Text });
            }
            else
            {
                txt_id.Text =Text;
                scanLogin();
            }
        }


        //스캔해서 로그인하면 패스워드 체크 안함
        private void scanLogin()
        {
            // 아이디유무체크
            string loginUid = "";
            string sid = this.txt_id.Text.Trim();

            if (sid == "")
            {
                MessageBox.Show("아이디를 입력해주세요.");
                this.txt_id.Text = "";
                this.txt_pass.Text = "";

                this.txt_id.Focus();
                return;
            }


            try
            {
                dc = new DataCommon();
                loginUid = dc.getSimpleScalar("select id from sys_emp where UPPER(Id) ='" + sid.ToUpper() + "' ").ToString();

            }
            catch
            {

                if (sid.ToString().ToUpper() != loginUid.ToString().ToUpper())
                {
                    //this.lb_msg.Text = "아이디가 없습니다.";
                    MessageBox.Show("아이디가 존재하지 않습니다.");
                    //this.txt_id.Text = "";
                    this.txt_pass.Text = "";

                    this.txt_id.Focus();
                    return;
                }
                else
                {
                    sid = loginUid;
                }
            }

            if (loginUid == "")
            {
                MessageBox.Show("아이디가 존재하지 않습니다.");
                //this.txt_id.Text = "";
                this.txt_pass.Text = "";

                this.txt_id.Focus();
                return;
            }

            try
            {

                dc = new DataCommon();
                string sName = dc.getSimpleScalar("select NAME from sys_emp where UPPER(Id) ='" + sid.ToUpper() + "' ").ToString();

                dc = new DataCommon();
                string sID = dc.getSimpleScalar("select id from sys_emp where UPPER(Id) ='" + sid.ToUpper() + "' ").ToString();


                //global::Sekonix_pop.Properties.Settings.Default.sIdOld = sid;
                global::Sekonix_pop.Properties.Settings.Default.Save();
                //Main.para.sAgentCode = sAreaC;
                //Sekonix_pop.Main.pId = sId;

                //아이디담기
                Para.USER.ID = sID;
                Para.USER.NAME = sName;

                bLogin = true;
                this.Close();
            }
            catch
            {
                MessageBox.Show("DB접속에 문제가 있습니다.");

                //test용

                bLogin = false;
                this.Close();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Registry.SetValue(@"HKEY_CURRENT_USER\SEKONIX\PortConfig", "ScanPortName", comboBox1.Text);
            panel2.Visible = false;
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {


            if (serialPort1.IsOpen)
            {
                serialPort1.Close();
            }

            try
            {
                serialPort1.PortName = comboBox1.SelectedItem.ToString();
                serialPort1.Open();
                label6.Visible = false;
            }
            catch(Exception ex)
            {
                label6.Visible = true;                
                serialPort1.Close();

                Console.WriteLine("스캐너= " + ex.ToString());
               
            }

            if (serialPort1.IsOpen)
            {
                label4.Text = "Open";

            }
            else
            {
                label4.Text = "Close";
            }
        }

        private void Login_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (serialPort1.IsOpen)
            {
                serialPort1.Close();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            panel2.Visible = true;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            panel2.Visible = false;
        }

        private void btnReconn_Click(object sender, EventArgs e)
        {
            //시리얼 포트확인
            try
            {
                if (serialPort1.IsOpen)
                {
                    this.serialPort1.Close();
                    this.serialPort1.Open();
                }
                else
                {
                    this.serialPort1.Open();
                }

                this.label4.ForeColor = Color.Black;
                this.label4.Text = "Open";
                label6.Visible = false; 

            }
            catch
            {
                try
                {
                    this.serialPort1.Close();

                    this.label4.ForeColor = Color.Red;
                    this.label4.Text = "Close";
                    label6.Visible = true; 
                }
                catch { }
            }
        }
         
    }
}
