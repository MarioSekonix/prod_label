using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Win32;
using System.Text.RegularExpressions;
using System.Globalization;
using Npgsql;
using System.IO;
using System.IO.Ports;
using System.Drawing.Printing;

using BIZ;
using System.Media;
using System.Runtime.InteropServices;


namespace Sekonix_pop.Page.POP.MA0000
{
    public partial class MA0080 : Form
    {

        [DllImport("USER32.DLL")]
        public static extern uint FindWindow(string lpClassName, string lpWindowName);
        [DllImport("user32.dll")]
        public static extern uint FindWindowEx(uint hWnd1, uint hWnd2, string lpsz1, string lpsz2);
        [DllImport("user32.dll")]
        public static extern uint SendMessage(uint hwnd, uint wMsg, uint wParam, uint lParam);
        [DllImport("user32.dll")]
        public static extern uint PostMessage(uint hwnd, uint wMsg, uint wParam, uint lParam);
        
        uint handle;

        int iiTimer2_cnt = 0;


        //델리게이트 선언(크로스 쓰레드 해결하기 위한 용도)
        delegate void MyDelegate();      


        string NPG_connect = String.Format("Server={0};Port={1};User Id={2};Password={3};Database={4};", "127.0.0.1", "5432", "postgres", "sekonix", "postgres");
        NpgsqlConnection NPG_conn;
        NpgsqlCommand com;
        NpgsqlDataAdapter ad;
        NpgsqlDataReader dRead;

        DataSet NPG_ds = new DataSet();
        DataTable NPG_dt = new DataTable(); 


        DataCommon dc = new DataCommon();
        DataTable dt = new DataTable();

        string sPrintPortName = "";
        string sScanPortName = "";

        public MA0080()
        {
            InitializeComponent();
        }

        private void MA0080_Load(object sender, EventArgs e)
        {
            
            string sSql;
            StringBuilder sbSql = new StringBuilder();

            chkEnglish.Checked = true;
            chkEnglish_Click(sender, e);

            Para.Terminal.OpCode = Sekonix_pop.Properties.Settings.Default.OP_CODE;            
            Para.Terminal.MchCode = Sekonix_pop.Properties.Settings.Default.MCH_CODE;
            Para.Terminal.MchName = Sekonix_pop.Properties.Settings.Default.MCH_NAME;

            try
            {
 
                sScanPortName = (Registry.GetValue(@"HKEY_CURRENT_USER\SEKONIX\PortConfig", "ScanPortName2", "") == null ? sScanPortName : (string)Registry.GetValue(@"HKEY_CURRENT_USER\SEKONIX\PortConfig", "ScanPortName2", ""));
                sPrintPortName = (Registry.GetValue(@"HKEY_CURRENT_USER\SEKONIX\PortConfig", "PrintPortName2", "") == null ? sPrintPortName : (string)Registry.GetValue(@"HKEY_CURRENT_USER\SEKONIX\PortConfig", "PrintPortName2", ""));
                txtTop.Text = (Registry.GetValue(@"HKEY_CURRENT_USER\SEKONIX\PortConfig", "sTop", "") == null ? txtTop.Text : (string)Registry.GetValue(@"HKEY_CURRENT_USER\SEKONIX\PortConfig", "sTop", ""));
                txtLeft.Text = (Registry.GetValue(@"HKEY_CURRENT_USER\SEKONIX\PortConfig", "sLeft", "") == null ? txtLeft.Text : (string)Registry.GetValue(@"HKEY_CURRENT_USER\SEKONIX\PortConfig", "sLeft", ""));
                txtBold.Text = (Registry.GetValue(@"HKEY_CURRENT_USER\SEKONIX\PortConfig", "sBold", "") == null ? txtBold.Text : (string)Registry.GetValue(@"HKEY_CURRENT_USER\SEKONIX\PortConfig", "sBold", ""));

                txtTmpLeft.Text = (Registry.GetValue(@"HKEY_CURRENT_USER\SEKONIX\PortConfig", "sTmpLeft", "") == null ? txtTmpLeft.Text : (string)Registry.GetValue(@"HKEY_CURRENT_USER\SEKONIX\PortConfig", "sTmpLeft", "30"));
                txtTmpTop.Text = (Registry.GetValue(@"HKEY_CURRENT_USER\SEKONIX\PortConfig", "sTmpTop", "") == null ? txtTmpTop.Text : (string)Registry.GetValue(@"HKEY_CURRENT_USER\SEKONIX\PortConfig", "sTmpTop", "10"));
                 

                //label7.Text = Para.Terminal.MchCode + " 제품라벨 발행";
                //this.Text = Para.Terminal.MchCode + " 제품라벨 발행";

                chkAuto.Checked = true;
                chkManual.Checked = false;
                txtPrtQty.Text = "1";
                txtPrtQty.Enabled = false;
                txtScanLAM.BackColor = System.Drawing.Color.FromArgb(192, 255, 255);
                txtScanLDM.BackColor = System.Drawing.Color.FromArgb(192, 255, 255);
                groupBox2.Enabled = true;
                btnSave.Enabled = false;


                //모듈 선택
                NPG_conn = new NpgsqlConnection(NPG_connect);
                NPG_conn.Open();
                sbSql = new StringBuilder();
                sbSql.AppendLine(" SELECT customer_name      ");
                sbSql.AppendLine(" FROM p_customer   ");
                sbSql.AppendLine(" WHERE 1=1           ");
                //sbSql.AppendLine(" AND   customer_name = '" + Para.Terminal.MchCode + "'           ");
                sSql = sbSql.ToString();
                Console.WriteLine("SQL=" + sSql);
                com = new NpgsqlCommand(sSql, NPG_conn);
                ad = new NpgsqlDataAdapter(com);
                dRead = com.ExecuteReader();
                string sCustomerName = "";
                try
                {
                    while (dRead.Read())
                    {
                        sCustomerName = dRead["customer_name"].ToString().Trim();
                        cmbCustomerCode.Items.Add(sCustomerName);
                        //cmbCustomerCode.Text = sCustomerName;
                    }
                }
                catch (NpgsqlException ne)
                {
                    Console.WriteLine("ne=" + ne.ToString());
                    //MsgTerminal.Error("시스템 오류입니다. 관리자를 호출하세요(err-1901) : " + ne.ToString());
                    //return;
                }
                finally
                {
                    // Console.WriteLine("Closing connections");
                    dRead.Close();
                    dRead = null;
                    NPG_conn.Close();
                    NPG_conn = null;
                    com.Dispose();
                    com = null;
                }

                //cmbCustomerCode.SelectedIndex = 0;


                cmbLocation.Text = "";
                cmbLocation.Items.Clear();
                cmbLocation.Items.Add("");
                cmbLocation.Items.Add("F");
                cmbLocation.Items.Add("R");
                cmbLocation.Items.Add("H");
                cmbLocation.Items.Add("L");
                cmbLocation.SelectedIndex = 1;

                cmbColor.Text = "";
                cmbColor.Items.Clear();
                cmbColor.Items.Add("");
                cmbColor.Items.Add("1");
                cmbColor.Items.Add("2");
                cmbColor.Items.Add("3");
                cmbColor.Items.Add("4");
                cmbColor.Items.Add("5");
                cmbColor.Items.Add("6");
                cmbColor.Items.Add("7");
                cmbColor.SelectedIndex = 1;

                 
                txtYYYY.Text = DateTime.Now.ToString("yyyy");
                string sMM = DateTime.Now.ToString("MM");
                string sDD = DateTime.Now.ToString("dd");
                txtMMDD.Text = sMM + sDD;
                                             

            }
            catch (NpgsqlException ne)
            {
                Console.WriteLine("MA0080_Load.err=" + ne.ToString());
            }


            //#######################################
            // 프린터 포트 설정
            //#######################################

            try
            {
                //프린터 포트
                cboPrint.Items.Add("");
                cboPrint.Items.Add("COM1");
                cboPrint.Items.Add("COM2");
                cboPrint.Items.Add("COM3");
                cboPrint.Items.Add("COM4");
                cboPrint.Items.Add("COM5");
                cboPrint.Items.Add("COM6");
                cboPrint.Items.Add("COM7");
                cboPrint.Items.Add("COM8");
                cboPrint.Items.Add("COM9");
                cboPrint.Items.Add("COM10");
                cboPrint.Items.Add("COM11");
                cboPrint.Items.Add("COM12");
                cboPrint.Items.Add("COM12");
                cboPrint.Items.Add("COM13");
                cboPrint.Items.Add("COM14");
                cboPrint.Items.Add("COM15");
                cboPrint.Items.Add("COM16");
                cboPrint.Items.Add("COM17");
                cboPrint.Items.Add("COM18");
                cboPrint.Items.Add("COM19");
                cboPrint.Items.Add("COM20");
                cboPrint.Items.Add("COM21");
                cboPrint.Items.Add("COM22");
                cboPrint.Items.Add("COM23");
                cboPrint.Items.Add("COM24");
                cboPrint.Items.Add("COM25");
                cboPrint.Items.Add("COM26");
                cboPrint.Items.Add("COM27");
                if (sPrintPortName != "")
                {
                    cboPrint.Text = sPrintPortName;
                }
            }
            catch
            {
                panelPortSet.Visible = true;
                lblPrintMsg.Text = "Close";
                lblPrintMsg.ForeColor = Color.Black;

                if (printPort.IsOpen)
                {
                    printPort.Close();
                }
            }

            if (checkBox1.Checked == false)
            {
                if (printPort.IsOpen)
                {
                    panelPortSet.Visible = false;
                    lblPortMsg.Visible = false;
                }
                else
                {
                    panelPortSet.Visible = true;
                    lblPortMsg.Visible = true;
                }
            }



            //#######################################
            // 스캐너 포트 설정 
            //#######################################
             
            try
            {
                //스캐너 포트
                cboScan.Items.Add("");
                cboScan.Items.Add("COM1");
                cboScan.Items.Add("COM2");
                cboScan.Items.Add("COM3");
                cboScan.Items.Add("COM4");
                cboScan.Items.Add("COM5");
                cboScan.Items.Add("COM6");
                cboScan.Items.Add("COM7");
                cboScan.Items.Add("COM8");
                cboScan.Items.Add("COM9");
                cboScan.Items.Add("COM10");
                cboScan.Items.Add("COM11");
                cboScan.Items.Add("COM12");
                cboScan.Items.Add("COM13");
                cboScan.Items.Add("COM14");
                cboScan.Items.Add("COM15");
                cboScan.Items.Add("COM16");
                cboScan.Items.Add("COM17");
                cboScan.Items.Add("COM18");
                cboScan.Items.Add("COM19");
                cboScan.Items.Add("COM20");
                cboScan.Items.Add("COM21");
                cboScan.Items.Add("COM22");
                cboScan.Items.Add("COM23");
                cboScan.Items.Add("COM24");
                cboScan.Items.Add("COM25");
                cboScan.Items.Add("COM26");
                cboScan.Items.Add("COM27");
                cboScan.Items.Add("COM28");
                cboScan.Items.Add("COM29");
                cboScan.Items.Add("COM30");
                if (sScanPortName != "")
                {
                    cboScan.Text = sScanPortName;
                }
            }
            catch
            {
                panelPortSet.Visible = true;
                lblScanMsg.Text = "Close";
                lblScanMsg.ForeColor = Color.Black;

                if (scanPort.IsOpen)
                {
                    scanPort.Close();
                }
            }

            if (scanPort.IsOpen)
            {
                panelPortSet.Visible = false;
                //lblScanMsg.Visible = false;
            }
            else
            {
                panelPortSet.Visible = true;
                //lblScanMsg.Visible = true;
            }
        }


        //Print USB
        public class RawPrinterHelper
        {
            // Structure and API declarions:
            [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
            public class DOCINFOA
            {
                [MarshalAs(UnmanagedType.LPStr)]
                public string pDocName;
                [MarshalAs(UnmanagedType.LPStr)]
                public string pOutputFile;
                [MarshalAs(UnmanagedType.LPStr)]
                public string pDataType;
            }
            [DllImport("winspool.Drv", EntryPoint = "OpenPrinterA", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
            public static extern bool OpenPrinter([MarshalAs(UnmanagedType.LPStr)] string szPrinter, out IntPtr hPrinter, IntPtr pd);

            [DllImport("winspool.Drv", EntryPoint = "ClosePrinter", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
            public static extern bool ClosePrinter(IntPtr hPrinter);

            [DllImport("winspool.Drv", EntryPoint = "StartDocPrinterA", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
            public static extern bool StartDocPrinter(IntPtr hPrinter, Int32 level, [In, MarshalAs(UnmanagedType.LPStruct)] DOCINFOA di);

            [DllImport("winspool.Drv", EntryPoint = "EndDocPrinter", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
            public static extern bool EndDocPrinter(IntPtr hPrinter);

            [DllImport("winspool.Drv", EntryPoint = "StartPagePrinter", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
            public static extern bool StartPagePrinter(IntPtr hPrinter);

            [DllImport("winspool.Drv", EntryPoint = "EndPagePrinter", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
            public static extern bool EndPagePrinter(IntPtr hPrinter);

            [DllImport("winspool.Drv", EntryPoint = "WritePrinter", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
            public static extern bool WritePrinter(IntPtr hPrinter, IntPtr pBytes, Int32 dwCount, out Int32 dwWritten);

            // SendBytesToPrinter()
            // When the function is given a printer name and an unmanaged array
            // of bytes, the function sends those bytes to the print queue.
            // Returns true on success, false on failure.
            public static bool SendBytesToPrinter(string szPrinterName, IntPtr pBytes, Int32 dwCount)
            {
                Int32 dwError = 0, dwWritten = 0;
                IntPtr hPrinter = new IntPtr(0);
                DOCINFOA di = new DOCINFOA();
                bool bSuccess = false; // Assume failure unless you specifically succeed.

                di.pDocName = "My C#.NET RAW Document";
                di.pDataType = "RAW";

                // Open the printer.
                if (OpenPrinter(szPrinterName.Normalize(), out hPrinter, IntPtr.Zero))
                {
                    // Start a document.
                    if (StartDocPrinter(hPrinter, 1, di))
                    {
                        // Start a page.
                        if (StartPagePrinter(hPrinter))
                        {
                            // Write your bytes.
                            bSuccess = WritePrinter(hPrinter, pBytes, dwCount, out dwWritten);
                            EndPagePrinter(hPrinter);
                        }
                        EndDocPrinter(hPrinter);
                    }
                    ClosePrinter(hPrinter);
                }
                // If you did not succeed, GetLastError may give more information
                // about why not.
                if (bSuccess == false)
                {
                    dwError = Marshal.GetLastWin32Error();
                }
                return bSuccess;
            }

            public static bool SendFileToPrinter(string szPrinterName, string szFileName)
            {
                // Open the file.
                FileStream fs = new FileStream(szFileName, FileMode.Open);
                // Create a BinaryReader on the file.
                BinaryReader br = new BinaryReader(fs);
                // Dim an array of bytes big enough to hold the file's contents.
                Byte[] bytes = new Byte[fs.Length];
                bool bSuccess = false;
                // Your unmanaged pointer.
                IntPtr pUnmanagedBytes = new IntPtr(0);
                int nLength;

                nLength = Convert.ToInt32(fs.Length);
                // Read the contents of the file into the array.
                bytes = br.ReadBytes(nLength);
                // Allocate some unmanaged memory for those bytes.
                pUnmanagedBytes = Marshal.AllocCoTaskMem(nLength);
                // Copy the managed byte array into the unmanaged array.
                Marshal.Copy(bytes, 0, pUnmanagedBytes, nLength);
                // Send the unmanaged bytes to the printer.
                bSuccess = SendBytesToPrinter(szPrinterName, pUnmanagedBytes, nLength);
                // Free the unmanaged memory that you allocated earlier.
                Marshal.FreeCoTaskMem(pUnmanagedBytes);
                return bSuccess;
            }
            public static bool SendStringToPrinter(string szPrinterName, string szString)
            {
                IntPtr pBytes;
                Int32 dwCount;
                // How many characters are in the string?
                dwCount = szString.Length;
                // Assume that the printer is expecting ANSI text, and then convert
                // the string to ANSI text.
                pBytes = Marshal.StringToCoTaskMemAnsi(szString);
                // Send the converted ANSI string to the printer.
                SendBytesToPrinter(szPrinterName, pBytes, dwCount);
                Marshal.FreeCoTaskMem(pBytes);
                return true;
            }
        }

        private void cmbCustomerCode_SelectedIndexChanged(object sender, EventArgs e)
        {

            string sSql;
            StringBuilder sbSql = new StringBuilder();


            cmbCarType2.Text = "";
            cmbCarType2.Items.Clear();


            cmbModelNo.Text = "";
            cmbModelNo.Items.Clear();
            txtModleName.Text = "";
                       
            cmbCarType.Text = "";
            cmbCarType.Items.Clear(); 
             

            NPG_conn = new NpgsqlConnection(NPG_connect);
            NPG_conn.Open(); 

            sbSql.AppendLine(" SELECT distinct car_type     ");
            sbSql.AppendLine(" FROM p_model_mast   ");
            sbSql.AppendLine(" WHERE 1=1           ");
            sbSql.AppendLine(" AND   customer_code = '" + cmbCustomerCode.Text.Trim() + "'   ");
            sbSql.AppendLine(" ORDER BY CAR_TYPE          ");
            sSql = sbSql.ToString();
            Console.WriteLine("postsql productversion    =" + sSql);
            com = new NpgsqlCommand(sSql, NPG_conn);
            ad = new NpgsqlDataAdapter(com);
            dRead = com.ExecuteReader();
            try
            {
                string sCarType = "";
                while (dRead.Read())
                {
                    sCarType = dRead["car_type"].ToString().Trim();
                    cmbCarType2.Items.Add(sCarType);
                }
            }
            catch (NpgsqlException ne)
            {
                Console.WriteLine("ne=" + ne.ToString()); 
            }
            finally
            {
                // Console.WriteLine("Closing connections");
                dRead.Close();
                dRead = null;
                NPG_conn.Close();
                NPG_conn = null;
                com.Dispose();
                com = null;
            } 
        }


        private void cmbCarType2_SelectedIndexChanged(object sender, EventArgs e)
        {
            string sSql;
            StringBuilder sbSql = new StringBuilder();

            cmbModelNo.Text = "";
            cmbModelNo.Items.Clear(); 

            txtModleName.Text = "";

            cmbCarType.Text = "";
            cmbCarType.Items.Clear();
             

            NPG_conn = new NpgsqlConnection(NPG_connect);
            NPG_conn.Open();

            sbSql.AppendLine(" SELECT distinct model_no     ");
            sbSql.AppendLine(" FROM p_model_mast   ");
            sbSql.AppendLine(" WHERE 1=1           ");
            sbSql.AppendLine(" AND   customer_code = '" + cmbCustomerCode.Text.Trim() + "'   ");
            sbSql.AppendLine(" AND   car_type = '" + cmbCarType2.Text.Trim() + "'   ");
            sbSql.AppendLine(" ORDER BY MODEL_NO  ");
            sSql = sbSql.ToString();
            Console.WriteLine("postsql productversion    =" + sSql);
            com = new NpgsqlCommand(sSql, NPG_conn);
            ad = new NpgsqlDataAdapter(com);
            dRead = com.ExecuteReader();
            try
            {
                string sModelNo = "";
                while (dRead.Read())
                {
                    sModelNo = dRead["model_no"].ToString().Trim();
                    cmbModelNo.Items.Add(sModelNo);
                }
            }
            catch (NpgsqlException ne)
            {
                Console.WriteLine("ne=" + ne.ToString());
            }
            finally
            {
                // Console.WriteLine("Closing connections");
                dRead.Close();
                dRead = null;
                NPG_conn.Close();
                NPG_conn = null;
                com.Dispose();
                com = null;
            } 
        }


        private void cmbModelNo_SelectedIndexChanged(object sender, EventArgs e)
        {
            string sSql;
            StringBuilder sbSql = new StringBuilder();
            
            txtModleName.Text = "";

            cmbCarType.Text = "";
            cmbCarType.Items.Clear();
              
             
            //모델명 셋팅
            NPG_conn = new NpgsqlConnection(NPG_connect);
            NPG_conn.Open();
            
            sbSql.AppendLine(" SELECT distinct model_name     ");
            sbSql.AppendLine(" FROM p_model_mast   ");
            sbSql.AppendLine(" WHERE 1=1           ");
            sbSql.AppendLine(" AND   customer_code = '" + cmbCustomerCode.Text.Trim() + "'   ");
            sbSql.AppendLine(" AND   model_no = '" + cmbModelNo.Text.Trim() + "'   ");
            sSql = sbSql.ToString();
            //Console.WriteLine("SQL=" + sSql);
            com = new NpgsqlCommand(sSql, NPG_conn);
            ad = new NpgsqlDataAdapter(com);
            dRead = com.ExecuteReader();
            try
            {
                //Console.WriteLine("Contents of table in database: \n");
                while (dRead.Read())
                {
                    txtModleName.Text = dRead["model_name"].ToString().Trim(); 
                }
            }
            catch (NpgsqlException ne)
            {
                Console.WriteLine("ne=" + ne.ToString()); 
            }
            finally
            {
                // Console.WriteLine("Closing connections");
                dRead.Close();
                dRead = null;
                NPG_conn.Close();
                NPG_conn = null;
                com.Dispose();
                com = null;
            }
            
            //차종 셋팅
            NPG_conn = new NpgsqlConnection(NPG_connect);
            NPG_conn.Open();            
            
            sSql = "";
            sbSql = new StringBuilder();
            sbSql.AppendLine(" SELECT distinct car_type     ");
            sbSql.AppendLine(" FROM p_model_mast   ");
            sbSql.AppendLine(" WHERE 1=1           ");
            sbSql.AppendLine(" AND   customer_code = '" + cmbCustomerCode.Text.Trim() + "'   ");
            sbSql.AppendLine(" AND   model_no = '" + cmbModelNo.Text.Trim() + "'   ");
            sSql = sbSql.ToString();
            com = new NpgsqlCommand(sSql, NPG_conn);
            ad = new NpgsqlDataAdapter(com);
            dRead = com.ExecuteReader();
            try
            {
                string sCarType = "";
                while (dRead.Read())
                {
                    sCarType = dRead["car_type"].ToString().Trim();
                    cmbCarType.Items.Add(sCarType);
                }
            }
            catch (NpgsqlException ne)
            {
                Console.WriteLine("ne=" + ne.ToString());
                //MsgTerminal.Error("시스템 오류입니다. 관리자를 호출하세요(err-1901) : " + ne.ToString());
                //return;
            }
            finally
            {
                // Console.WriteLine("Closing connections");
                dRead.Close();
                dRead = null;
                NPG_conn.Close();
                NPG_conn = null;
                com.Dispose();
                com = null;
            }

            cmbCarType.SelectedIndex = 0;

        }

        private void cmbCarType_SelectedIndexChanged(object sender, EventArgs e)
        { 
        }



        private void btnYearNext_Click(object sender, EventArgs e)
        {
            setYear("NEXT");
            txtMMDD.Focus();
            txtMMDD.SelectAll();
        }

        private void btnYearBefore_Click(object sender, EventArgs e)
        {

            setYear("BEFORE");
            txtMMDD.Focus();
            txtMMDD.SelectAll();
        }


        private void setYear(string sStatus)
        {
            System.DateTime time_year = new System.DateTime(Convert.ToInt32(txtYYYY.Text), 1, 1);

            if (sStatus == "TO_DATE")
            {
                txtYYYY.Text = time_year.ToString("yyyy"); 
            }
            else if (sStatus == "NEXT")
            {
                txtYYYY.Text = time_year.AddYears(1).ToString("yyyy"); 
            }
            else if (sStatus == "BEFORE")
            {
                txtYYYY.Text = time_year.AddYears(-1).ToString("yyyy"); 
            }
        }


        private void cmbLocation_SelectedIndexChanged(object sender, EventArgs e)
        {
            cmbColor.Text = "";
            cmbColor.Items.Clear();
            cmbColor.Items.Add("");
            cmbColor.Items.Add("1");
            cmbColor.Items.Add("2");
            cmbColor.Items.Add("3");
            cmbColor.Items.Add("4");
            cmbColor.Items.Add("5");
            cmbColor.Items.Add("6");
            cmbColor.Items.Add("7");
             
        }

        private void cmbColor_SelectedIndexChanged(object sender, EventArgs e)
        { 
            txtEo.Focus();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //입력한 항목을 초기화 하시겠습니까?
            if (MessageBox.Show("Reset all enteries?", "Message", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                cmbCustomerCode.Text = "";
                cmbModelNo.Text = "";
                txtModleName.Text = "";
                cmbCarType.Text = "";
 

                txtStartSerial.Text = "";
                txtEndSerial.Text = "";
                txtScanLDM.Text = "";
                txtScanLAM.Text = "";
                txtScanData.Text = "";
                 
                if (chkAuto.Checked == true)
                {
                    txtPrtQty.Text = "1";
                }
                else
                {
                    txtPrtQty.Text = "";
                }
            }
        }

        private void txtPrtQty_KeyUp(object sender, KeyEventArgs e)
        {
            //시리얼번호 시작번호 생성
            gfnMakeSerial();           

        }

        private void txtPrtQty_KeyPress(object sender, KeyPressEventArgs e)
        {


            if (e.KeyChar == 13)
            {
                if (txtPrtQty.Text.Trim() == "" || txtPrtQty.Text.Trim() == "0")
                {
                    MessageBox.Show("Please enter the quantity to be issued");
                    //발행 수량을 입력해주세요
                    return;

                }                
            }
        }


        private void gfnMakeSerial()
        {
            string sQuery = "";
            int iiMatCnt = 0;
            string sRowCnt = "0";
            string sStartSerialNo = "";
            string sEndSerialNo = "";

            if (cmbModelNo.Text.Trim() == "")
            {
                MessageBox.Show("Please select a model"); //모델을 선택해주세요
                return;
            }



            try
            {
                NPG_conn = new NpgsqlConnection(NPG_connect);
                if (NPG_conn != null && NPG_conn.State == ConnectionState.Open)
                {
                    NPG_conn.Close();
                }
                else
                {
                    NPG_conn.Open();
                }
                sQuery = "";
                sQuery += " SELECT   mat_count      ";
                sQuery += " FROM     p_mat_count  ";
                sQuery += " WHERE    1=1            ";
                sQuery += " AND      UPPER(mat_code) ='" + cmbModelNo.Text.Trim().ToUpper() + "'   ";
                sQuery += " AND      lot_no ='" + txtYYYY.Text + txtMMDD.Text + cmbModelNo.Text.Trim().ToUpper() + "' ";
                Console.WriteLine("squery = " + sQuery);
                com = new NpgsqlCommand(sQuery, NPG_conn);
                ad = new NpgsqlDataAdapter(com);
                dt = new DataTable();
                ad.Fill(dt);
                dRead = com.ExecuteReader();

                sRowCnt = "0";

                try
                {
                    while (dRead.Read())
                    {
                        sRowCnt = dRead[0].ToString().Trim();
                    }
                }
                catch (NpgsqlException ne)
                {
                    MessageBox.Show(" gfnMakeSerial(200) : " + ne.ToString());
                    return;
                }
                finally
                {
                    // Console.WriteLine("Closing connections");
                    dRead.Close();
                    dRead = null;
                    NPG_conn.Close();
                    NPG_conn = null;
                    com.Dispose();
                    com = null;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("22 ---" + ex.ToString());
            }


            //고객사에 따른 바코드 및 문자표기용 일련번호 생성
            try
            {
                iiMatCnt = Convert.ToInt32(sRowCnt);
            }
            catch
            {
                iiMatCnt = 0;
            }


            //기존 카운터에서 1 증가 후 3자리 문자로 만든다
            iiMatCnt++;

            try
            {
                /*******************************************************************************/  
                //LED = 바코드 16진수 3자리,  문자 10진수 4자리
                sStartSerialNo = Common.DecimalToArbitrarySystem(iiMatCnt, 16);
                int iiSetCnt = 3 - sStartSerialNo.Length;
                for (int kk = 1; kk <= iiSetCnt; kk++)
                {
                    sStartSerialNo = "0" + sStartSerialNo;
                }
                //시작일련번호 생성
                txtStartSerial.Text = sStartSerialNo;
                /*******************************************************************************/


                /*******************************************************************************/
                //종료 일련번호  시작
                iiMatCnt = iiMatCnt + Convert.ToInt32(txtPrtQty.Text) - 1;
                //LED = 바코드 16진수 3자리,  문자 10진수 4자리(100)
                sEndSerialNo = Common.DecimalToArbitrarySystem(iiMatCnt, 16);
                //sEndSerialNo = Convert.ToString(iiMatCnt);
                iiSetCnt = 3 - sEndSerialNo.Length;
                for (int kk = 1; kk <= iiSetCnt; kk++)
                {
                    sEndSerialNo = "0" + sEndSerialNo;
                }
                //일련번호 생성 종료
                txtEndSerial.Text = sEndSerialNo;
                /*******************************************************************************/
            }
            catch (Exception ex)
            {
                Console.WriteLine("555---" + ex.ToString());
            }
            
        }



        private void gfnLabelPrint()
        {
             
            //프린터 시리얼 포트 설정 확인
            try
            {
                string sRowCnt = "0";
                string sQuery = "";
                string sStartSerialNo = "";
                string sDataMatrixCode = "";
                int iiMatCnt = 0;

                int iiCnt = 0;


                #region < 프린터 포트 오픈 >
                if (checkBox1.Checked == false)
                {
                    if (!printPort.IsOpen)
                    {
                        printPort.PortName = cboPrint.SelectedItem.ToString();
                        printPort.BaudRate = (int)9600;
                        printPort.DataBits = (int)8;
                        printPort.Parity = System.IO.Ports.Parity.None;
                        printPort.StopBits = StopBits.One;
                        printPort.WriteBufferSize = 4096;
                        printPort.Open();
                    }
                }
                #endregion

                #region < 모델 조회 >
                //모델번호
                string sModelCode = cmbModelNo.Text.Trim();
                int iiCodeLength = 14 - sModelCode.Length;
                if (iiCodeLength > 0)
                {
                    for (int kk = 1; kk <= iiCodeLength; kk++)
                    {
                        sModelCode = sModelCode + " ";
                    }
                }

                //생산년월
                //년도 - 16(2자리)
                //월   - 1~9, A~C (36진수)
                //이   - 1~9, A~V (36진수)
                string sYear = txtYYYY.Text.Trim().Substring(2, 2);
                string sMonth = txtMMDD.Text.Trim().Substring(0, 2);
                string sDay = txtMMDD.Text.Trim().Substring(2, 2);
                sMonth = Common.DecimalToArbitrarySystem(Convert.ToInt32(sMonth), 36);
                sDay = Common.DecimalToArbitrarySystem(Convert.ToInt32(sDay), 36);
                string sMakedate = sYear + sMonth + sDay;


                //공통 정보
                string sCarInfo = cmbLocation.Text.Trim() + cmbColor.Text.Trim() + txtEo.Text.Trim() + txtRev.Text.Trim() + txtCorp.Text.Trim() + "   ";

                #endregion

                #region < 바코드 생성 및 출력 >

                //Data Matrix 생성
                //################################################
                //^XA     => 라벨 Start
                //^LHx,y  => 라벨 Start Position (x: x축위치   y: y축위치)
                //^PRx    => 라벨 발행 속도      (x: a-f)
                //^BYx,y  => 바코드비율
                //################################################
                string sBarCodeNoPrint = "";
                sBarCodeNoPrint += "^XA";
                sBarCodeNoPrint += "^PRA";
                sBarCodeNoPrint += "~SD" + txtBold.Text.Trim();
                //sBarCodeNoPrint += "^PW1500";

                try
                {
                    NPG_conn = new NpgsqlConnection(NPG_connect);
                    if (NPG_conn != null && NPG_conn.State == ConnectionState.Open)
                    {
                        NPG_conn.Close();
                    }
                    else
                    {
                        NPG_conn.Open();
                    }
                    sQuery = "";
                    sQuery += " SELECT   mat_count      ";
                    sQuery += " FROM     p_mat_count  ";
                    sQuery += " WHERE    1=1            ";
                    sQuery += " AND      UPPER(mat_code) ='" + cmbModelNo.Text.Trim().ToUpper() + "'   ";
                    sQuery += " AND      lot_no ='" + txtYYYY.Text.Trim() + txtMMDD.Text.Trim() + cmbModelNo.Text.Trim().ToUpper() + "' ";
                    Console.WriteLine("squery = " + sQuery);
                    com = new NpgsqlCommand(sQuery, NPG_conn);
                    ad = new NpgsqlDataAdapter(com);
                    dt = new DataTable();
                    ad.Fill(dt);
                    dRead = com.ExecuteReader();
                    sRowCnt = "0";
                    try
                    {
                        while (dRead.Read())
                        {
                            sRowCnt = dRead[0].ToString().Trim();
                        }
                    }
                    catch (NpgsqlException ne)
                    {
                        MessageBox.Show(" Error(200) : " + ne.ToString());
                        return;
                    }
                    finally
                    {
                        // Console.WriteLine("Closing connections");
                        dRead.Close();
                        dRead = null;
                        NPG_conn.Close();
                        NPG_conn = null;
                        com.Dispose();
                        com = null;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("-6666--" + ex.ToString());
                }


                //고객사에 따른 바코드 및 문자표기용 일련번호 생성
                iiMatCnt = 0;
                try
                {
                    iiMatCnt = Convert.ToInt32(sRowCnt);
                }
                catch
                {
                    iiMatCnt = 0;
                }


                int iiSetCnt = 0;
                string ssMatCnt = "0";
                int iiSetCnt2 = 0;


                iiMatCnt++;

                //LED = 바코드 16진수 3자리,  문자 10진수 4자리                 
                sStartSerialNo = Common.DecimalToArbitrarySystem(iiMatCnt, 16);
                 
                iiSetCnt = 3 - sStartSerialNo.Length;
                for (int kk = 1; kk <= iiSetCnt; kk++)
                {
                    sStartSerialNo = "0" + sStartSerialNo;
                }
                //Data Matrix 정보 ###################################################
                sDataMatrixCode = sModelCode + sMakedate + sStartSerialNo + sCarInfo;
                //문자로 표시되는 일련번호 생성
                ssMatCnt = Convert.ToString(iiMatCnt);
                iiSetCnt2 = 4 - ssMatCnt.Length;

                for (int kk = 1; kk <= iiSetCnt2; kk++)
                {
                    ssMatCnt = "0" + ssMatCnt;
                }
                //1 바코드 번호
                string ssLeft = txtLeft.Text.Trim();
                string ssTop = txtTop.Text.Trim();

                sBarCodeNoPrint += "^FO" + ssLeft + "," + ssTop + "^BY2,2^BXN,4,200,N,N,N^FD" + sDataMatrixCode.Trim() + "^FS";
                //sBarCodeNoPrint += "^FO" + Convert.ToString(Convert.ToInt32(ssLeft) + 85) + "," + ssTop + "^CI26^AN,18,8^FD" + cmbModelNo.Text.Trim() + "^FS";
                //sBarCodeNoPrint += "^FO" + Convert.ToString(Convert.ToInt32(ssLeft) + 85) + "," + Convert.ToString(Convert.ToInt32(ssTop) + 30) + "^CI26^AN,18,8^FD0000" + txtYYYY.Text.Trim().Substring(2, 2) + txtMMDD.Text.Trim() + "^FS";
                //sBarCodeNoPrint += "^FO" + Convert.ToString(Convert.ToInt32(ssLeft) + 85) + "," + Convert.ToString(Convert.ToInt32(ssTop) + 60) + "^CI26^AN,18,8^FD" + cmbCarType.Text.Trim() + ssMatCnt.Trim() + "^FS";

                sBarCodeNoPrint += "^FO" + Convert.ToString(Convert.ToInt32(ssLeft) + 85) + "," + ssTop + "^A0,22,20^FD" + cmbModelNo.Text.Trim() + "^FS";
                sBarCodeNoPrint += "^FO" + Convert.ToString(Convert.ToInt32(ssLeft) + 85) + "," + Convert.ToString(Convert.ToInt32(ssTop) + 30) + "^A0,22,20^FD0000" + txtYYYY.Text.Trim().Substring(2, 2) + txtMMDD.Text.Trim() + "^FS";
                sBarCodeNoPrint += "^FO" + Convert.ToString(Convert.ToInt32(ssLeft) + 85) + "," + Convert.ToString(Convert.ToInt32(ssTop) + 60) + "^A0,22,20^FD" + cmbCarType.Text.Trim() + ssMatCnt.Trim() + "^FS";
                              
                sBarCodeNoPrint += "^XZ";


                //Print USB
                if (checkBox1.Checked == true)
                {
                    PrintDialog pd = new PrintDialog();
                    pd.PrinterSettings = new PrinterSettings();

                    //if (DialogResult.OK == pd.ShowDialog(this))
                    //{
                    RawPrinterHelper.SendStringToPrinter(pd.PrinterSettings.PrinterName, sBarCodeNoPrint);
                    //}
                }
                else if (checkBox1.Checked == false)
                {
                    printPort.Write(sBarCodeNoPrint);
                }
                Console.WriteLine("barcode= " + sBarCodeNoPrint);

                #endregion 

                #region <출력한 바코드 마지막시리얼 저장 시작>
                //-----------------------------------------------------------------------
                iiCnt = 0;


                NPG_conn = new NpgsqlConnection(NPG_connect);
                if (NPG_conn != null && NPG_conn.State == ConnectionState.Open)
                {
                    NPG_conn.Close();
                }
                else
                {
                    NPG_conn.Open();
                }
                sQuery = "";
                sQuery = "";
                sQuery += " SELECT   COUNT(1) CNT    ";
                sQuery += " FROM     p_mat_count   ";
                sQuery += " WHERE    1=1             ";
                sQuery += " AND      UPPER(mat_code) ='" + cmbModelNo.Text.Trim().ToUpper() + "'   ";
                sQuery += " AND      lot_no ='" + txtYYYY.Text.Trim() + txtMMDD.Text.Trim() + cmbModelNo.Text.Trim().ToUpper() + "' ";
                Console.WriteLine("POP_MAT_COUNT SQL=" + sQuery);

                com = new NpgsqlCommand(sQuery, NPG_conn);
                ad = new NpgsqlDataAdapter(com);
                dRead = com.ExecuteReader();
                sRowCnt = "0";
                try
                {
                    //Console.WriteLine("Contents of table in database: \n");
                    while (dRead.Read())
                    {
                        sRowCnt = dRead[0].ToString().Trim();
                    }
                }
                catch (NpgsqlException ne)
                {
                    MessageBox.Show(" Error(400) : " + ne.ToString());
                    return;
                }
                finally
                {
                    // Console.WriteLine("Closing connections");
                    dRead.Close();
                    dRead = null;
                    NPG_conn.Close();
                    NPG_conn = null;
                    com.Dispose();
                    com = null;
                }
                iiCnt = Convert.ToInt32(sRowCnt);
                if (iiCnt < 1)
                {

                    NPG_conn = new NpgsqlConnection(NPG_connect);
                    if (NPG_conn != null && NPG_conn.State == ConnectionState.Open)
                    {
                        NPG_conn.Close();
                    }
                    else
                    {
                        NPG_conn.Open();
                    }
                    //출력한 바코드 입력
                    //##################################################################################################
                    //데이터 베이스 등록
                    try
                    {
                        sQuery = " INSERT INTO p_mat_count(         ";
                        sQuery += "            mat_code             ";
                        sQuery += "           ,lot_no               ";
                        sQuery += "           ,mat_count            ";
                        sQuery += ")VALUES(                         ";
                        sQuery += "           '" + cmbModelNo.Text.Trim().ToUpper() + "'  ";
                        sQuery += "          ,'" + txtYYYY.Text.Trim() + txtMMDD.Text.Trim() + cmbModelNo.Text.Trim().ToUpper() + "'      ";
                        sQuery += "          , " + iiMatCnt + "     ";
                        sQuery += " )                               ";
                        //Console.WriteLine("바코드 출력 INSERT = " + sQuery);
                        com = new NpgsqlCommand(sQuery, NPG_conn);
                        com.ExecuteNonQuery();
                    }
                    catch (NpgsqlException ne)
                    {
                        MessageBox.Show(" Error(500) : " + ne.ToString());
                        return;
                    }
                    finally
                    {
                        // Console.WriteLine("Closing connections");                             
                        NPG_conn.Close();
                        NPG_conn = null;
                        com.Dispose();
                        com = null;
                    }
                }
                else
                {
                    //출력한 바코드 수정
                    //##################################################################################################
                    //데이터 베이스 등록
                    NPG_conn = new NpgsqlConnection(NPG_connect);
                    if (NPG_conn != null && NPG_conn.State == ConnectionState.Open)
                    {
                        NPG_conn.Close();
                    }
                    else
                    {
                        NPG_conn.Open();
                    }

                    try
                    {
                        sQuery = " UPDATE p_mat_count SET ";
                        sQuery += "         lot_no    = '" + txtYYYY.Text.Trim() + txtMMDD.Text.Trim() + cmbModelNo.Text.Trim().ToUpper() + "'  ,  ";
                        sQuery += "         mat_count =  " + iiMatCnt + "    ";
                        sQuery += "WHERE    mat_code    = '" + cmbModelNo.Text.Trim().ToUpper() + "'     ";
                        sQuery += " AND     lot_no   = '" + txtYYYY.Text.Trim() + txtMMDD.Text.Trim() + cmbModelNo.Text.Trim().ToUpper() + "'  ";
                        //Console.WriteLine("바코드 출력 UPDATE = " + sQuery);
                        com = new NpgsqlCommand(sQuery, NPG_conn);
                        com.ExecuteNonQuery();
                    }
                    catch (NpgsqlException ne)
                    {
                        MessageBox.Show(" Error(600) : " + ne.ToString());
                        return;
                    }
                    finally
                    {
                        // Console.WriteLine("Closing connections");                             
                        NPG_conn.Close();
                        NPG_conn = null;
                        com.Dispose();
                        com = null;
                    }
                }
                //-----------------------------------------------------------------------            
                #endregion

                #region <바코드 출력내용 이력 저장>
                //######################################################
                //######################################################
                //바코드 출력내용 이력 저장
                //######################################################
                //######################################################
                NPG_conn = new NpgsqlConnection(NPG_connect);
                if (NPG_conn != null && NPG_conn.State == ConnectionState.Open)
                {
                    NPG_conn.Close();
                }
                else
                {
                    NPG_conn.Open();
                }
                try
                {
                    sQuery = " INSERT INTO p_inspection_hist(                                 ";
                    sQuery += "            customer_code                                      ";
                    sQuery += "           ,model_no                                           ";
                    sQuery += "           ,car_type                                           ";
                    sQuery += "           ,car_location                                       ";
                    sQuery += "           ,car_color                                          ";
                    sQuery += "           ,car_eo                                             ";
                    sQuery += "           ,car_rev                                            ";
                    sQuery += "           ,car_corp                                           ";
                    sQuery += "           ,mat_ldm                                            ";
                    sQuery += "           ,mat_lam                                            ";
                    sQuery += "           ,insp_result                                        ";
                    sQuery += "           ,make_date                                          ";
                    sQuery += "           ,crt_date                                           ";
                    //sQuery += "           ,file_name                                           ";
                    sQuery += "           ,bar_code                                           ";
                    sQuery += ")VALUES(                                                       ";
                    sQuery += "           '" + cmbCustomerCode.Text.Trim().ToUpper() + "'  ";
                    sQuery += "          ,'" + cmbModelNo.Text.Trim().ToUpper() + "'  ";
                    sQuery += "          ,'" + cmbCarType.Text.Trim() + "'  ";
                    sQuery += "          ,'" + cmbLocation.Text.Trim() + "'  ";
                    sQuery += "          ,'" + cmbColor.Text.Trim() + "'  ";
                    sQuery += "          ,'" + txtEo.Text.Trim() + "'  ";
                    sQuery += "          ,'" + txtRev.Text.Trim() + "'  ";
                    sQuery += "          ,'" + txtCorp.Text.Trim() + "'  ";
                    sQuery += "          ,'" + txtScanLDM.Text.Trim() + "'  ";
                    sQuery += "          ,'" + txtScanLAM.Text.Trim() + "'  ";
                    sQuery += "          ,'" + txtYN.Text.Trim() + "'  ";
                    sQuery += "          ,'" + txtYYYY.Text.Trim() + txtMMDD.Text.Trim() + "'  ";
                    sQuery += "          , now()                                              ";
                    //sQuery += "          ,'" + txtFileName.Text.Trim() + "'  ";
                    sQuery += "          ,'" + sDataMatrixCode.Trim() + "'  ";
                    sQuery += " )                                                             ";
                    //Console.WriteLine("바코드 출력 INSERT = " + sQuery);
                    com = new NpgsqlCommand(sQuery, NPG_conn);
                    com.ExecuteNonQuery();
                }
                catch (NpgsqlException ne)
                {
                    //wLog.Log("error.log", ne.ToString(), "ERR-15");
                    MessageBox.Show(" Error(500) : " + ne.ToString());
                    return;
                }
                finally
                {
                    // Console.WriteLine("Closing connections");                             
                    NPG_conn.Close();
                    NPG_conn = null;
                    com.Dispose();
                    com = null;
                }
                //######################################################
                #endregion

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            finally
            {
                txtScanLDM.Text = "";
                txtScanLAM.Text = "";
            }
        }





        private void txtEo_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 13)
            {
                txtRev.Text = "";
                txtRev.Focus();
            }
        }

        private void txtRev_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 13)
            {
                txtCorp.Text = "";
                txtCorp.Focus();
            }
        }

        private void txtCorp_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 13)
            { 
                txtMMDD.Focus();
            }
        }

        private void txtMMDD_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 13)
            {
                var formats = new[] { "yyyyMMdd", "yyyy-MM-dd" };
                DateTime dtRtn;

                if (!DateTime.TryParseExact(txtYYYY.Text + txtMMDD.Text, formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out dtRtn))
                {
                    txtMMDD.Focus();
                    txtMMDD.SelectAll();

                    //MessageBox.Show("입력된 날짜가 잘못되었습니다.");
                    MessageBox.Show("Invalid date entered.");
                    return;
                }
                 
                txtPrtQty.Text = "1";
                 
                 
            }
        }

        private void txtMMDD_KeyUp(object sender, KeyEventArgs e)
        {
            if (txtMMDD.Text.Length == 4)
            {
                var formats = new[] { "yyyyMMdd", "yyyy-MM-dd" };
                DateTime dtRtn;

                if (!DateTime.TryParseExact(txtYYYY.Text.Trim() + txtMMDD.Text.Trim(), formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out dtRtn))
                {
                    txtMMDD.Focus();
                    txtMMDD.SelectAll();

                    //MessageBox.Show("입력된 날짜가 잘못되었습니다.");
                    MessageBox.Show("Invalid date entered.");
                    return;
                }

                if (chkAuto.Checked == true)
                {
                    txtPrtQty.Text = "1";
                }
                else
                {
                    txtPrtQty.Text = "";
                    txtPrtQty.Focus();
                    txtPrtQty.SelectAll();
                }
            }
        }

        

        private void button1_Click(object sender, EventArgs e)
        {
            Registry.SetValue(@"HKEY_CURRENT_USER\SEKONIX\PortConfig", "PrintPortName2", cboPrint.Text);
            Registry.SetValue(@"HKEY_CURRENT_USER\SEKONIX\PortConfig", "ScanPortName2", cboScan.Text);
            Registry.SetValue(@"HKEY_CURRENT_USER\SEKONIX\PortConfig", "sTop", txtTop.Text);
            Registry.SetValue(@"HKEY_CURRENT_USER\SEKONIX\PortConfig", "sLeft", txtLeft.Text);
            Registry.SetValue(@"HKEY_CURRENT_USER\SEKONIX\PortConfig", "sBold", txtBold.Text);

            Registry.SetValue(@"HKEY_CURRENT_USER\SEKONIX\PortConfig", "sTmpLeft", txtTmpLeft.Text);
            Registry.SetValue(@"HKEY_CURRENT_USER\SEKONIX\PortConfig", "sTmpTop", txtTmpTop.Text);

            if (checkBox1.Checked == false)
            {
                if (printPort.IsOpen)
                {
                    lblPortMsg.Visible = false;
                }
                else
                {
                    lblPortMsg.Visible = true;
                }
            }
            else
            {
            lblPortMsg.Visible = false;
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            panelPortSet.Visible = false;

            if (checkBox1.Checked == false)
            {
                if (printPort.IsOpen)
                {
                    lblPortMsg.Visible = false;
                }
                else
                {
                    lblPortMsg.Visible = true;
                }
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (chkAuto.Checked == true)
            {
                //MessageBox.Show("자동 라벨출력 상태에서는 사용할 수 없습니다.");
                MessageBox.Show("Can not be used with auto label output.");
                return;
            }

            gfnNext();
            
        }

        private void gfnNext()
        {
            string sQuery = "";

            if (cmbCustomerCode.Text == "")
            {
                //MessageBox.Show("제품을 선택해주세요");
                MessageBox.Show("Please select a product");
                return;
            }

            if (cmbModelNo.Text == "")
            {
                //MessageBox.Show("모델을 선택해주세요");
                MessageBox.Show("Please select a model");
                return;
            }

            if (cmbModelNo.Text.IndexOf("-") >= 0)
            {
                //MessageBox.Show("모델번호에 '-' 부호를 넣을수 없습니다.");
                MessageBox.Show("Model number can not contain '-' sign");
                return;
            }

            if (cmbCarType.Text == "")
            {
                //MessageBox.Show("차종을 선택해주세요");
                MessageBox.Show("Please select a Car Name");
                return;
            }

            if (cmbLocation.Text == "")
            {
                //MessageBox.Show("위치를 선택해주세요");
                MessageBox.Show("Please select a location");
                return;
            }

            if (cmbColor.Text == "")
            {
                //MessageBox.Show("컬러를 선택해주세요");
                MessageBox.Show("Please select a color");
                return;
            }

            if (txtEo.Text == "")
            {
                //MessageBox.Show("EO를 입력해주세요");
                MessageBox.Show("Please enter the EO");
                return;
            }

            if (txtRev.Text == "")
            {
                //MessageBox.Show("버전을 입력해주세요");
                MessageBox.Show("Please enter the version");
                return;
            }

            if (txtCorp.Text == "")
            {
                //MessageBox.Show("양산지를 입력해주세요");
                MessageBox.Show("Please enter the volume");
                return;
            }


            if (txtMMDD.Text == "")
            {
                //MessageBox.Show("생산일자를 입력해주세요");
                MessageBox.Show("Please enter production date");
                return;
            }



            if (txtPrtQty.Text == "" || txtPrtQty.Text == "0")
            {
                //MessageBox.Show("발행수량을 입력해주세요");
                MessageBox.Show("Please enter the number of labels issued");

                txtPrtQty.Text = "";
                txtStartSerial.Text = "";
                txtEndSerial.Text = "";

                return;
            }


            //-----------------------------------------------------------------------
            //신규모델 등록
            //-----------------------------------------------------------------------
            int iiCnt = 0;


            NPG_conn = new NpgsqlConnection(NPG_connect);
            if (NPG_conn != null && NPG_conn.State == ConnectionState.Open)
            {
                NPG_conn.Close();
            }
            else
            {
                NPG_conn.Open();
            }
            sQuery = "";
            sQuery = "";
            sQuery += " SELECT   COUNT(1) CNT    ";
            sQuery += " FROM     p_model_mast   ";
            sQuery += " WHERE    1=1             ";
            sQuery += " AND      model_no ='" + cmbModelNo.Text.Trim() + "'   ";
            sQuery += " AND      customer_code ='" + cmbCustomerCode.Text.Trim() + "' ";
            Console.WriteLine("POP_MAT_COUNT SQL=" + sQuery);
            com = new NpgsqlCommand(sQuery, NPG_conn);
            ad = new NpgsqlDataAdapter(com);
            dRead = com.ExecuteReader();
            string sRowCnt = "0";
            try
            {
                //Console.WriteLine("Contents of table in database: \n");
                while (dRead.Read())
                {
                    sRowCnt = dRead[0].ToString().Trim();
                }
            }
            catch (NpgsqlException ne)
            {
                MessageBox.Show(" Error(400) : " + ne.ToString());
                return;
            }
            finally
            {
                // Console.WriteLine("Closing connections");
                dRead.Close();
                dRead = null;
                NPG_conn.Close();
                NPG_conn = null;
                com.Dispose();
                com = null;
            }
            iiCnt = Convert.ToInt32(sRowCnt);
            if (iiCnt < 1)
            {

                NPG_conn = new NpgsqlConnection(NPG_connect);
                if (NPG_conn != null && NPG_conn.State == ConnectionState.Open)
                {
                    NPG_conn.Close();
                }
                else
                {
                    NPG_conn.Open();
                }

                //##################################################################################################
                //데이터 베이스 등록
                try
                {
                    sQuery = " INSERT INTO p_model_mast(         ";
                    sQuery += "            model_no             ";
                    sQuery += "           ,model_name               ";
                    sQuery += "           ,customer_code            ";
                    sQuery += "           ,car_type            ";
                    sQuery += ")VALUES(                         ";
                    sQuery += "           '" + cmbModelNo.Text.Trim() + "'  ";
                    sQuery += "          ,'" + txtModleName.Text.Trim() + "'      ";
                    sQuery += "          ,'" + cmbCustomerCode.Text.Trim() + "'     ";
                    sQuery += "          ,'" + cmbCarType.Text.Trim() + "'     ";
                    sQuery += " )                               ";
                    Console.WriteLine("바코드 출력 INSERT = " + sQuery);
                    com = new NpgsqlCommand(sQuery, NPG_conn);
                    com.ExecuteNonQuery();
                }
                catch (NpgsqlException ne)
                {
                    MessageBox.Show(" Error(500) : " + ne.ToString());
                    return;
                }
                finally
                {
                    // Console.WriteLine("Closing connections");                             
                    NPG_conn.Close();
                    NPG_conn = null;
                    com.Dispose();
                    com = null;
                }
            }
            //##########################################

            txtYN.Text = "OK";
            string sPartNumber = "";
            string sResult = "";
            string sCurrentResult = "";

            if (chkAuto.Checked == true)
            {

                //데이터베이스에서 txtScanLDM.Text의 값으로 결과값이 Y/N인지 확인
                NPG_conn = new NpgsqlConnection(NPG_connect);
                if (NPG_conn != null && NPG_conn.State == ConnectionState.Open)
                {
                    NPG_conn.Close();
                }
                else
                {
                    NPG_conn.Open();
                }

                sQuery = "";
                sQuery += " SELECT   result , part_number, CURRENT_RESULT   ";
                sQuery += " FROM     p_tsc  ";
                sQuery += " WHERE    1=1             ";
                sQuery += " AND      bar_code ='" + txtScanLDM.Text.Trim() + "'   ";
                sQuery += " ORDER BY WORK_DATE ASC, WORK_TIME ASC ";
                Console.WriteLine("POP_MAT_COUNT SQL=" + sQuery);
                com = new NpgsqlCommand(sQuery, NPG_conn);
                ad = new NpgsqlDataAdapter(com);
                dRead = com.ExecuteReader();

                txtYN.Text = "NG";

                try
                {
                    //Console.WriteLine("Contents of table in database: \n");
                    while (dRead.Read())
                    {
                        sResult = dRead[0].ToString().Trim();
                        sPartNumber = dRead[1].ToString().Trim();
                        sCurrentResult = dRead[2].ToString().Trim();

                        if (sResult == "PASS" && sCurrentResult == "PASS")
                        {
                            txtYN.Text = "PASS";
                        }
                        else
                        {
                            txtYN.Text = "FAIL";
                        }

                        //if (txtYN.Text.Trim() != "PASS")
                        //{
                        //    break;
                        //}
                    }
                }
                catch (NpgsqlException ne)
                {
                    MessageBox.Show(" Error(400) : " + ne.ToString());
                    return;
                }
                finally
                {
                    // Console.WriteLine("Closing connections");
                    dRead.Close();
                    dRead = null;
                    NPG_conn.Close();
                    NPG_conn = null;
                    com.Dispose();
                    com = null;
                }



                if (sPartNumber == "")
                {

                    if (chkEnglish.Checked == true)
                    {

                        lblMsg_1.Text = "No inspection history. Rework it. !!";
                    }
                    else
                    {
                        lblMsg_1.Text = "검사이력이 없습니다. 재작업 하세요 !!";
                    }

                    pnlMsg.Visible = true;
                    //SoundPlayer _SoundPlayer = new SoundPlayer(Sekonix_pop.Properties.Resources.ringin);
                    //_SoundPlayer.Play();
                    return;

                }



                if (cmbModelNo.Text != sPartNumber)
                {

                    if (chkEnglish.Checked == true)
                    {

                        lblMsg_1.Text = "Item number is inconsistent!!";
                    }
                    else
                    {
                        lblMsg_1.Text = "품번이 불일치 합니다!!";
                    }

                    pnlMsg.Visible = true;
                    //SoundPlayer _SoundPlayer = new SoundPlayer(Sekonix_pop.Properties.Resources.ringin);
                    //_SoundPlayer.Play();
                    return;

                }
            }
             
            //자동발행 OK
            if (txtYN.Text == "PASS")
            {
                SoundPlayer _SoundPlayer = new SoundPlayer(Sekonix_pop.Properties.Resources.OK);
                _SoundPlayer.Play();

                //###############################################
                //###############################################
                //라벨 발행
                //###############################################
                //###############################################
                int iiPrtQty = Convert.ToInt32(txtPrtQty.Text.Trim());

                for (int ii = 1; ii <= iiPrtQty; ii++)
                {
                    gfnLabelPrint();
                }


                if (chkAuto.Checked == true)
                {
                    txtPrtQty.Text = "1";
                }
                else
                {
                    txtPrtQty.Text = "";
                }

                txtStartSerial.Text = "";
                txtEndSerial.Text = "";
            }

            //수동발행
            else if (txtYN.Text == "OK")
            {
                SoundPlayer _SoundPlayer = new SoundPlayer(Sekonix_pop.Properties.Resources.OK);
                _SoundPlayer.Play();

                //###############################################
                //###############################################
                //라벨 발행
                //###############################################
                //###############################################
                int iiPrtQty = Convert.ToInt32(txtPrtQty.Text.Trim());

                for (int ii = 1; ii <= iiPrtQty; ii++)
                {
                    gfnLabelPrint();
                }
                
                txtPrtQty.Text = "";
                

                txtStartSerial.Text = "";
                txtEndSerial.Text = "";
            }

            //NG
            else
            {
                pnlMsg.Visible = true;
                SoundPlayer _SoundPlayer = new SoundPlayer(Sekonix_pop.Properties.Resources.ringin);
                _SoundPlayer.Play();

                NPG_conn = new NpgsqlConnection(NPG_connect);
                if (NPG_conn != null && NPG_conn.State == ConnectionState.Open)
                {
                    NPG_conn.Close();
                }
                else
                {
                    NPG_conn.Open();
                }
                try
                {
                    sQuery = " INSERT INTO p_inspection_hist(                                 ";
                    sQuery += "            customer_code                                      ";
                    sQuery += "           ,model_no                                           ";
                    sQuery += "           ,car_type                                           ";
                    sQuery += "           ,car_location                                       ";
                    sQuery += "           ,car_color                                          ";
                    sQuery += "           ,car_eo                                             ";
                    sQuery += "           ,car_rev                                            ";
                    sQuery += "           ,car_corp                                           ";
                    sQuery += "           ,mat_ldm                                            ";
                    sQuery += "           ,mat_lam                                            ";
                    sQuery += "           ,insp_result                                        ";
                    sQuery += "           ,make_date                                          ";
                    sQuery += "           ,crt_date                                           ";
                    //sQuery += "           ,file_name                                           ";
                    sQuery += ")VALUES(                                                       ";
                    sQuery += "           '" + cmbCustomerCode.Text.Trim().ToUpper() + "'  ";
                    sQuery += "          ,'" + cmbModelNo.Text.Trim().ToUpper() + "'  ";
                    sQuery += "          ,'" + cmbCarType.Text.Trim() + "'  ";
                    sQuery += "          ,'" + cmbLocation.Text.Trim() + "'  ";
                    sQuery += "          ,'" + cmbColor.Text.Trim() + "'  ";
                    sQuery += "          ,'" + txtEo.Text.Trim() + "'  ";
                    sQuery += "          ,'" + txtRev.Text.Trim() + "'  ";
                    sQuery += "          ,'" + txtCorp.Text.Trim() + "'  ";
                    sQuery += "          ,'" + txtScanLDM.Text.Trim() + "'  ";
                    sQuery += "          ,'" + txtScanLAM.Text.Trim() + "'  ";
                    sQuery += "          ,'" + txtYN.Text.Trim() + "'  ";
                    sQuery += "          ,'" + txtYYYY.Text.Trim() + txtMMDD.Text.Trim() + "'  ";
                    sQuery += "          , now()                                              ";
                   // sQuery += "          ,'" + txtFileName.Text.Trim() + "'  ";
                    sQuery += " )                                                             ";
                    Console.WriteLine("바코드 출력 INSERT = " + sQuery);
                    com = new NpgsqlCommand(sQuery, NPG_conn);
                    com.ExecuteNonQuery();
                }
                catch (NpgsqlException ne)
                {
                    //wLog.Log("error.log", ne.ToString(), "ERR-15");
                    MessageBox.Show(" Error(500) : " + ne.ToString());
                    return;
                }
                finally
                {
                    // Console.WriteLine("Closing connections");                             
                    NPG_conn.Close();
                    NPG_conn = null;
                    com.Dispose();
                    com = null;
                }
            }              
        }

        private void btnPort_Click(object sender, EventArgs e)
        {
            panelPortSet.Visible = true;
            txtTmpQty.Focus();
            txtTmpQty.SelectAll();
        }

        private void txtPrtQty_Click(object sender, EventArgs e)
        {
            if (chkAuto.Checked == true)
            {
                txtPrtQty.Text = "1";
            }
            else
            {
                txtPrtQty.Text = "";
                txtPrtQty.Focus();
            }
            

            txtStartSerial.Text = "";
            txtEndSerial.Text = "";
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (cmbModelNo.Text.Trim() == "")
            {
                //MessageBox.Show("모델을 선택해주세요");
                MessageBox.Show("Please select a model");
                return;
            }

            //if (MessageBox.Show(cmbModelNo.Text.Trim() + "의 일련번호를 초기화 하시겠습니까?", "알림", MessageBoxButtons.YesNo) == DialogResult.Yes)
            if (MessageBox.Show(cmbModelNo.Text.Trim() + " Reset serial number?", "Message", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {

                NPG_conn = new NpgsqlConnection(NPG_connect);
                if (NPG_conn != null && NPG_conn.State == ConnectionState.Open)
                {
                    NPG_conn.Close();
                }
                else
                {
                    NPG_conn.Open();
                }
                //데이터 베이스 등록
                try
                {
                    string sQuery = "";
                    sQuery = " UPDATE p_mat_count SET mat_count=0         ";
                    sQuery += "WHERE 1=1                         ";
                    sQuery += " AND mat_code='" + cmbModelNo.Text.Trim().ToUpper() + "'  ";
                    sQuery += " AND LOT_NO ='" + txtYYYY.Text.Trim() + txtMMDD.Text.Trim() + cmbModelNo.Text.Trim().ToUpper() + "'      ";
                    //Console.WriteLine("바코드 출력 INSERT = " + sQuery);
                    com = new NpgsqlCommand(sQuery, NPG_conn);
                    com.ExecuteNonQuery();
                }
                catch (NpgsqlException ne)
                {
                    MessageBox.Show(" Error(500) : " + ne.ToString());
                    return;
                }
                finally
                {
                    // Console.WriteLine("Closing connections");                             
                    NPG_conn.Close();
                    NPG_conn = null;
                    com.Dispose();
                    com = null;
                }

                gfnMakeSerial();
            }
        }

        private void txtEndSerial_TextChanged(object sender, EventArgs e)
        {

        }

        private void label16_Click(object sender, EventArgs e)
        {

        }

        private void txtStartSerial_TextChanged(object sender, EventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void button4_Click(object sender, EventArgs e)
        {
            Sekonix_pop.Page.POP.MA0000.MA0080_PP frmAdd = new Sekonix_pop.Page.POP.MA0000.MA0080_PP();
            frmAdd.ShowDialog(this);
        }

        private void chkAuto_Click(object sender, EventArgs e)
        {
            chkAuto.Checked = true;
            chkManual.Checked = false;

            txtPrtQty.Text = "1";
            txtPrtQty.Enabled = false;


            txtScanLAM.BackColor = System.Drawing.Color.FromArgb(192, 255, 255);
            txtScanLDM.BackColor = System.Drawing.Color.FromArgb(192, 255, 255);

            groupBox2.Enabled = true;
            btnSave.Enabled = false;
             
        }

        private void chkManual_Click(object sender, EventArgs e)
        {
            chkAuto.Checked = false;
            chkManual.Checked = true;

            txtYN.Text = "PASS";
            txtPrtQty.Text = "0";

            string path = "";
            string txtValue = "";


            path = Application.StartupPath + @"\pw.txt";
            txtValue = System.IO.File.ReadAllText(path);

            if (txtPW.Text.ToUpper() == txtValue.ToUpper())
            {

                //txtPrtQty.Enabled = true;

                txtScanLAM.BackColor = System.Drawing.Color.FromArgb(240, 240, 240);
                txtScanLDM.BackColor = System.Drawing.Color.FromArgb(240, 240, 240);

                groupBox2.Enabled = false;
                btnSave.Enabled = true;
            }

        }

        private void cboScan_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (scanPort.IsOpen)
            {
                scanPort.Close();
            }

            try
            {
                scanPort.PortName = cboScan.SelectedItem.ToString();

                if (!scanPort.IsOpen)
                {
                    scanPort.BaudRate = (int)9600;
                    scanPort.DataBits = (int)8;
                    scanPort.Parity = System.IO.Ports.Parity.None;
                    scanPort.StopBits = StopBits.One;
                    scanPort.Open();
                }


                if (!scanPort.IsOpen)
                {
                    lblScanMsg.Text = "Close";
                    lblScanMsg.ForeColor = Color.Black;
                }
                else
                {
                    lblScanMsg.Text = "Open";
                    lblScanMsg.ForeColor = Color.Red;
                }
            }
            catch (Exception ex)
            {
                if (checkBox1.Checked == false)
                {
                    panelPortSet.Visible = true;
                    lblPortMsg.Visible = true;
                }

                lblScanMsg.Text = "Close";
                lblScanMsg.ForeColor = Color.Black;
                
                //MessageBox.Show("스캐너 포트가 존재하지 않습니다");
                MessageBox.Show("Scanner port does not exist");

                Console.WriteLine("스캐터 포트 오류=" + ex.ToString());
            }
        }

        private void cboPrint_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (printPort.IsOpen)
            {
                printPort.Close();
            }

            try
            {
                printPort.PortName = cboPrint.SelectedItem.ToString();

                if (!printPort.IsOpen)
                {
                    printPort.BaudRate = (int)9600;
                    printPort.DataBits = (int)8;
                    printPort.Parity = System.IO.Ports.Parity.None;
                    printPort.StopBits = StopBits.One;
                    printPort.Open();
                }

                if (checkBox1.Checked == true)
                {
                    if (!printPort.IsOpen)
                    {
                        lblPrintMsg.Text = "Close";
                        lblPrintMsg.ForeColor = Color.Black;
                    }
                }
                else
                {
                    lblPrintMsg.Text = "Open";
                    lblPrintMsg.ForeColor = Color.Red;
                }
            }
            catch (Exception ex)
            {
                panelPortSet.Visible = true;
                lblPortMsg.Visible = true;

                lblPrintMsg.Text = "Close";
                lblPrintMsg.ForeColor = Color.Black;
                
                //MessageBox.Show("프린트 포트가 존재하지 않습니다");
                MessageBox.Show("Print port does not exist");

                Console.WriteLine("프린트포트 오류="+ ex.ToString());
            }
        }

        private void scanPort_ErrorReceived(object sender, SerialErrorReceivedEventArgs e)
        {
            try
            { 
                //MessageBox.Show("스캐너 연결 오류");
                MessageBox.Show("Scanner connection error");

                this.scanPort.Close();
                this.scanPort.Open();
            }
            catch
            {
                //MessageBox.Show("스캐너 재연결 불가");  
                MessageBox.Show("Scanner coan not reconnect"); 
            }
        }

        private void scanPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            byte[] array = new byte[1024];
            int temp;
            string str = string.Empty;
            temp = scanPort.Read(array, 0, 1024);

            try
            {
                MyDelegate dt = delegate()
                {
                    for (int i = 0; i < temp; i++)
                    {
                        if (array[i] == 13)
                        {
                            barCodeParsing(txtScanData.Text.Replace("\n", "").Replace("\r", "").Trim());
                            txtScanData.Text = "";
                        }
                        else
                        {
                            txtScanData.Text += (char)array[i];
                        }
                    }
                };
                this.Invoke(dt);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }


        private void barCodeParsing(string sBarCode)
        {

            pnlMsg.Visible = false;

            try
            {
                if (chkAuto.Checked == true)
                {
                    if (cmbCustomerCode.Text == "")
                    {
                        //MessageBox.Show("제품을 선택해주세요");
                        MessageBox.Show("Please select a product");
                        return;
                    }

                    if (cmbModelNo.Text == "")
                    {
                        //MessageBox.Show("모델을 선택해주세요");
                        MessageBox.Show("Please select a model");
                        return;
                    }

                    if (cmbModelNo.Text.IndexOf("-") >= 0)
                    {
                        //MessageBox.Show("모델번호에 '-' 부호를 넣을수 없습니다.");
                        MessageBox.Show("Model number can not contain '-' sign");
                        return;
                    }

                    if (cmbCarType.Text == "")
                    {
                        //MessageBox.Show("차종을 선택해주세요");
                        MessageBox.Show("Please select a Car Name");
                        return;
                    }

                    if (cmbLocation.Text == "")
                    {
                        //MessageBox.Show("위치를 선택해주세요");
                        MessageBox.Show("Please select a location");
                        return;
                    }

                    if (cmbColor.Text == "")
                    {
                        //MessageBox.Show("컬러를 선택해주세요");
                        MessageBox.Show("Please select a color");
                        return;
                    }

                    if (txtEo.Text == "")
                    {
                        //MessageBox.Show("EO를 입력해주세요");
                        MessageBox.Show("Please enter the EO");
                        return;
                    }

                    if (txtRev.Text == "")
                    {
                        //MessageBox.Show("버전을 입력해주세요");
                        MessageBox.Show("Please enter the version");
                        return;
                    }

                    if (txtCorp.Text == "")
                    {
                        //MessageBox.Show("양산지를 입력해주세요");
                        MessageBox.Show("Please enter the volume");
                        return;
                    }


                    if (txtMMDD.Text == "")
                    {
                        //MessageBox.Show("생산일자를 입력해주세요");
                        MessageBox.Show("Please enter production date");
                        return;
                    }



                    if (txtPrtQty.Text == "" || txtPrtQty.Text == "0")
                    {
                        //MessageBox.Show("발행수량을 입력해주세요");
                        MessageBox.Show("Please enter the number of labels issued");

                        txtPrtQty.Text = "";
                        txtStartSerial.Text = "";
                        txtEndSerial.Text = "";

                        return;
                    }

                    if (chkLDM.Checked == true && chkLAM.Checked == true)
                    {
                        if (txtScanLDM.Text.Trim() == "" && txtScanLAM.Text.Trim() == "")
                        {
                            txtScanLDM.Text = sBarCode;
                        }
                        else if (txtScanLDM.Text.Trim() != "" && txtScanLAM.Text.Trim() == "")
                        {
                            if (txtScanLDM.Text.Trim() == sBarCode.Trim())
                            {
                                //MessageBox.Show("바코드 스캔오류입니다. \n재스캔 하세요");
                                MessageBox.Show("Barcode scan error. Rescan");
                                txtScanLDM.Text = "";
                                txtScanLAM.Text = "";
                                return;
                            }

                            txtScanLAM.Text = sBarCode;

                            txtPrtQty.Text = "1";
                            gfnNext();

                            txtScanLDM.Text = "";
                            txtScanLAM.Text = "";
                        }
                        else
                        {
                            //MessageBox.Show("바코드 스캔오류입니다. \n재스캔 하세요");
                            MessageBox.Show("Barcode scan error. Rescan");
                            txtScanLDM.Text = "";
                            txtScanLAM.Text = "";
                            return;
                        }
                    }
                    else
                    {
                        txtScanLDM.Text = sBarCode;

                        txtPrtQty.Text = "1";
                        gfnNext();

                        txtScanLDM.Text = "";
                        txtScanLAM.Text = "";                         
                    }
                }                
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                txtScanLDM.Text = "";
                txtScanLAM.Text = "";
            } 
        }

        private void button5_Click(object sender, EventArgs e)
        {
            pnlMsg.Visible = false;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            handle = FindWindow(null, "Data Gathering");

            if (Convert.ToInt32(handle.ToString()) <= 0)
            {
                //lblMsg.Text = "파일 게더링 프로그램이 실행되지 않고 있습니다. 확인해주세요.";
                lblMsg.Text = "The file gererating program is not running. Please check.";
                timer2.Enabled = true;
            }
            else
            {
                lblMsg.Text = "   ";
                timer2.Enabled = false;
            }
             
            //Console.WriteLine("======>" +  handle.ToString() );
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            iiTimer2_cnt++;

            if (iiTimer2_cnt > 100)
            {
                iiTimer2_cnt = 1;
            }

            if (iiTimer2_cnt % 2 > 0)
            { 
                lblMsg.ForeColor = Color.Red;
            }
            else
            {
                lblMsg.ForeColor = Color.Pink;
            }
        }

        private void chkEnglish_Click(object sender, EventArgs e)
        {
            chkEnglish.Checked = true;
            chkKorean.Checked = false;
            gfnLanguage("ENG");
        }

        private void chkKorean_Click(object sender, EventArgs e)
        {
            chkEnglish.Checked = false;
            chkKorean.Checked = true;
            gfnLanguage("KOR");
        }

        private void gfnLanguage(string sLanguage)
        {
           
            if (sLanguage == "ENG")
            {
              
                label7.Text = "Issuance of product label";
                lblPortMsg.Text = "Please set communication port";
                groupBox1.Text = "product information";
                groupBox2.Text = "Scan information";
                groupBox3.Text = "Output information";
                lblMsg_1.Text = "Inspection result is defective product !!";
                lblMsg_2.Text = "Do not issue product labels. !!";
                label6.Text = "Part Name";
                label1.Text = "Model Number";
                label5.Text = "Model Name";
                label2.Text = "Car Name";
                label8.Text = "Position";
                label11.Text = "Color";
                label15.Text = "Product Area";
                label3.Text = "Product Date";
                label13.Text = "Issued Quantity";
                label4.Text = "Number";
              
                chkAuto.Text = "Auto";
                chkManual.Text = "Manual";
                btnSave.Text = "Label Issued";
                button2.Text = "Reset";                 
                button4.Text = "Edit Models";


            }
            else
            {
                label7.Text = "제품 라벨발행";
                lblPortMsg.Text = "통신포트를 셋팅해주세요";
                groupBox1.Text = "제품정보";
                groupBox2.Text = "";
                groupBox3.Text = "출력정보";
                lblMsg_1.Text = "검사결과 불량제품입니다!!";
                lblMsg_2.Text = "제품라벨을 발행하지 않습니다. !!";
                label6.Text = "제품모듈";
                label1.Text = "모델번호";
                label5.Text = "모델명";
                label2.Text = "차종";
                label8.Text = "위치";
                label11.Text = "컬러";
                label15.Text = "양산지";
                label3.Text = "생산일자";
                label13.Text = "발행수량";
                label4.Text = "일련번호";
                 
                chkAuto.Text = "자동발행";
                chkManual.Text = "수동발행";
                btnSave.Text = "라벨발행";
                button2.Text = "초 기 화";                 
                button4.Text = "모델수정";
            }
                

        }

        private void panel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void chkManual_CheckedChanged(object sender, EventArgs e)
        {

            string path = "";
            string txtValue = "";


                path = Application.StartupPath + @"\pw.txt";
                txtValue = System.IO.File.ReadAllText(path);


            if (chkManual.Checked == true )
            {
                pnlPW.Visible = true;



                if (txtPW.Text.ToUpper() == txtValue.ToUpper())
                {
                    txtPW.Text = "";
                    txtPW.Focus();
                    txtPrtQty.Enabled = true;
                    btnSave.Enabled = true;
                }
                else
                    txtPrtQty.Text = "1";

            }
            else
            {
                pnlPW.Visible = false; 
                txtPW.Text = "";
                chkAuto.Checked = true;
                chkManual.Checked = false;

            }
        }

        private void txtPW_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 13)
            {
                button7_Click(sender, e);
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {

            //all Text
            string path = "";
            string txtValue = "";

       
                path = Application.StartupPath + @"\pw.txt";
                txtValue = System.IO.File.ReadAllText(path);
            
           

            if (txtPW.Text.ToUpper() == txtValue.ToUpper())
            {
                txtPrtQty.Enabled = true;
                chkManual.Checked = true;
                chkAuto.Checked = false;
                btnSave.Enabled = true;
            }
            else
            {
                MessageBox.Show("Password Fail !!");
                chkAuto.Checked = true;
                chkManual.Checked = false;
                txtPrtQty.Enabled = false;
                txtPrtQty.Text = "1";

            }

            pnlPW.Visible = false;
        }

        private void button6_Click(object sender, EventArgs e)
        {
            pnlPW.Visible = false;
            txtPW.Text = "";
            chkAuto.Checked = true;
            chkManual.Checked = false;
            txtPrtQty.Text = "1";
        }

        private void chkLAM_CheckedChanged(object sender, EventArgs e)
        {
            if (chkLAM.Checked == true)
            {
                chkLDM.Text = "LDM Barcode";
                txtScanLAM.Enabled = true;
                txtScanLAM.BackColor = System.Drawing.Color.FromArgb(192, 255, 255);
                chkLAM.ForeColor = Color.Orange;
            }
            else
            {
                chkLDM.Text = "Barcode";
                txtScanLAM.Enabled = false;
                txtScanLAM.BackColor = System.Drawing.Color.FromArgb(240, 240, 240);
                chkLAM.ForeColor = Color.Silver;
            }
        }

        private void chkLDM_CheckedChanged(object sender, EventArgs e)
        {
            chkLDM.Checked = true;
        }

        private void txtTmpQty_Click(object sender, EventArgs e)
        {
            txtTmpQty.SelectAll();
        }

        private void button9_Click(object sender, EventArgs e)
        {
            if (txtTmpQty.Text == "" || txtTmpQty.Text == "0")
            {
                MessageBox.Show("Please enter the quantity !!");
                txtTmpQty.SelectAll();
            }

            try
            {
                int kk = Convert.ToInt32(txtTmpQty.Text);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Please enter the quantity !!");
                txtTmpQty.SelectAll();
                return;
            }

            goTmpLabel();
        }

        private void goTmpLabel()
        {
            //프린터 시리얼 포트 설정 확인
            try
            {

                #region < 프린터 포트 오픈 >

                if (checkBox1.Checked == false)
                    {
                if (!printPort.IsOpen)
                {
                    printPort.PortName = cboPrint.SelectedItem.ToString();
                    printPort.BaudRate = (int)9600;
                    printPort.DataBits = (int)8;
                    printPort.Parity = System.IO.Ports.Parity.None;
                    printPort.StopBits = StopBits.One;
                    printPort.WriteBufferSize = 4096;
                    printPort.Open();
                }
                }
               
                #endregion
                 
                //모델번호
                string sModelCode = cmbModelNo.Text.Trim();             

               
                string sDataMatrixCode = "";
                  
                #region < 바코드 생성 및 출력 >

                //Data Matrix 생성
                //################################################
                //^XA     => 라벨 Start
                //^LHx,y  => 라벨 Start Position (x: x축위치   y: y축위치)
                //^PRx    => 라벨 발행 속도      (x: a-f)
                //^BYx,y  => 바코드비율
                //################################################
                string sBarCodeNoPrint = "";

                for (int ii = 1; ii <= Convert.ToInt32(txtTmpQty.Text); ii++)
                {
                    DateTime dt = DateTime.Now; // Or whatever
                    System.Threading.Thread.Sleep(300);
                    sDataMatrixCode = dt.ToString("yyMMddHHmmssfff");
                    Console.WriteLine("sDataMatrixCode=" + sDataMatrixCode);

                    sBarCodeNoPrint = "";
                    sBarCodeNoPrint += "^XA";
                    sBarCodeNoPrint += "^PRA";
                    sBarCodeNoPrint += "~SD" + txtBold.Text.Trim();
                    sBarCodeNoPrint += "^LH" + txtTmpLeft.Text + "," + txtTmpTop.Text;
                    sBarCodeNoPrint += "^FO40,10^BY2,2^BXN,3,200,N,N,N^FD" + sDataMatrixCode.Trim() + "^FS";
                    sBarCodeNoPrint += "^FO5,55^A0,18,14^FD" + sDataMatrixCode.Trim() + "^FS";
                    //sBarCodeNoPrint += "^PQ" + txtTmpQty.Text;
                    sBarCodeNoPrint += "^XZ";

                    //Console.WriteLine("sBarCodeNoPrint=" + sBarCodeNoPrint);
                    //printPort.Write(sBarCodeNoPrint);


                    if (checkBox1.Checked == true)
                    {
                        PrintDialog pd = new PrintDialog();
                        pd.PrinterSettings = new PrinterSettings();

                        //if (DialogResult.OK == pd.ShowDialog(this))
                        //{
                        RawPrinterHelper.SendStringToPrinter(pd.PrinterSettings.PrinterName, sBarCodeNoPrint);
                        //}
                    }
                    else if (checkBox1.Checked == false)
                    {
                        printPort.Write(sBarCodeNoPrint);
                    }
                    Console.WriteLine("barcode= " + sBarCodeNoPrint);
                }
                 
                
                #endregion 
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            } 


            txtTmpQty.Text = "";
            panelPortSet.Visible = false;
        }

        private long UnixTimeNow()
        {
            var timeSpan = (DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0));
            return (long)timeSpan.TotalSeconds;

        }

        private void txtTmpQty_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                button9.PerformClick();
                txtTmpQty.Text = "";
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void checkBox1_CheckedChanged_1(object sender, EventArgs e)
        {
            if (checkBox1.Checked == true)
            {
                cboPrint.Enabled= false;
            }

                else
            {
                cboPrint.Enabled= true;
            }
        
            
        }

        private void lblPortMsg_Click(object sender, EventArgs e)
        {

        }

        private void button3_Click_1(object sender, EventArgs e)
        {
             
            PrintDialog pd = new PrintDialog();
            pd.PrinterSettings = new PrinterSettings();
            pd.ShowDialog(this);
            
               // pd.PrinterSettings.PrinterName = name;
            
        }

        private void panel4_Paint(object sender, PaintEventArgs e)
        {

        }

        private void chkAuto_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void txtPrtQty_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtTmpQty_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtPW_TextChanged(object sender, EventArgs e)
        {

        } 
         
    }

}
