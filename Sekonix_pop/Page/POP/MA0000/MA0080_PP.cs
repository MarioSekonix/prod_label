using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Npgsql;

namespace Sekonix_pop.Page.POP.MA0000
{
    public partial class MA0080_PP : Form
    {
        string NPG_connect = String.Format("Server={0};Port={1};User Id={2};Password={3};Database={4};", "127.0.0.1", "5432", "postgres", "sekonix", "postgres");
        NpgsqlConnection NPG_conn;
        NpgsqlCommand com; 

        DataSet NPG_ds = new DataSet();
        DataTable NPG_dt = new DataTable();

        DataCommon dc = new DataCommon();
        DataTable dt = new DataTable();

        public MA0080_PP()
        {
            InitializeComponent();
        }

        private void MA0080_PP_Load(object sender, EventArgs e)
        {

            gridSearch(); 
        }
       
        private void button3_Click(object sender, EventArgs e)
        {
              
            if (DialogResult.OK == MessageBox.Show("Do you want save?", "Save", MessageBoxButtons.OKCancel))
            { 

                //몽땅 지워버려
                NPG_conn = new NpgsqlConnection(NPG_connect);
                if (NPG_conn != null && NPG_conn.State == ConnectionState.Open)
                {
                    NPG_conn.Close();
                }
                else
                {
                    NPG_conn.Open();
                }

                NpgsqlCommand cmd = null;

                try
                {
                    string sSql; 

                    //수정
                    StringBuilder sbSql = new StringBuilder();
                    sbSql.AppendLine(" delete from P_MODEL_MAST   ");
                    sSql = sbSql.ToString();
                    cmd = new NpgsqlCommand(sSql, NPG_conn);
                    cmd.ExecuteNonQuery();
                }
                catch (NpgsqlException ne)
                {
                    Console.WriteLine("INSERT INTO Error=" + ne.ToString());
                    return;
                }
                finally
                {
                    // Console.WriteLine("Closing connections");                             
                    NPG_conn.Close();
                    NPG_conn = null;
                    cmd.Dispose();
                    cmd = null;
                }




                string sModelNo = "";
                string sModelName = "";
                string sCustomerCode = "";
                string sCarType = "";
                string sPosition = "";
                string sLotQty = "";

                 for (int row = 0; row < dataGridView1.Rows.Count; row++)
                {
                    try
                    {
                        sCustomerCode = dataGridView1.Rows[row].Cells[0].Value.ToString();
                        sModelNo = dataGridView1.Rows[row].Cells[1].Value.ToString();
                        sModelName = dataGridView1.Rows[row].Cells[2].Value.ToString();
                        sCarType = dataGridView1.Rows[row].Cells[3].Value.ToString();
                        sPosition = dataGridView1.Rows[row].Cells[4].Value.ToString();
                        sLotQty = dataGridView1.Rows[row].Cells[5].Value.ToString();
                    }
                    catch { }
                       
                     
                     NPG_conn = new NpgsqlConnection(NPG_connect);
                     if (NPG_conn != null && NPG_conn.State == ConnectionState.Open)
                     {
                         NPG_conn.Close();
                     }
                     else
                     {
                         NPG_conn.Open();
                     }

                     cmd = null;

                     try
                     { 
                         StringBuilder sbSql = new StringBuilder();
                         sbSql.AppendLine("INSERT INTO P_MODEL_MAST(        ");
                         sbSql.AppendLine("     MODEL_NO                    ");
                         sbSql.AppendLine("   , MODEL_NAME                    ");
                         sbSql.AppendLine("   , CUSTOMER_CODE                    ");
                         sbSql.AppendLine("   , CAR_TYPE                    ");
                         sbSql.AppendLine("   , POSITION                    ");
                         sbSql.AppendLine("   , LOT_QTY                    ");
                         sbSql.AppendLine(" )values (                        ");
                         sbSql.AppendLine("     '" + sModelNo.Trim() + "'   ");
                         sbSql.AppendLine("    ,'" + sModelName.Trim() + "'   ");
                         sbSql.AppendLine("    ,'" + sCustomerCode.Trim() + "'   ");
                         sbSql.AppendLine("    ,'" + sCarType.Trim() + "'   ");
                         sbSql.AppendLine("    ,'" + sPosition.Trim() + "'   ");
                         sbSql.AppendLine("    ,'" + sLotQty.Trim() + "'   ");
                         sbSql.AppendLine(")                                 ");

                         string sSql = sbSql.ToString();
                         cmd = new NpgsqlCommand(sSql, NPG_conn);
                         cmd.ExecuteNonQuery();
                     }
                     catch (NpgsqlException ne)
                     {
                         Console.WriteLine("INSERT INTO Error=" + ne.ToString());
                         return;
                     }
                     finally
                     {
                         // Console.WriteLine("Closing connections");                             
                         NPG_conn.Close();
                         NPG_conn = null;
                         cmd.Dispose();
                         cmd = null;
                     }
                } 

                gridSearch(); 
            }
             
        } 
  
        private void gridSearch()
        {
            string sSql;
            StringBuilder sbSql = new StringBuilder(); 
            try
            {
                //모듈 선택(LED모듈-100. PROJECTION모듈-200
                NPG_conn = new NpgsqlConnection(NPG_connect);
                if (NPG_conn != null && NPG_conn.State == ConnectionState.Open)
                {
                    NPG_conn.Close();
                }
                else
                {
                    NPG_conn.Open();
                }
                sbSql.AppendLine(" SELECT customer_code as MODULE, ");
                sbSql.AppendLine("          model_no as MODEL,      ");
                sbSql.AppendLine("          model_name as MODEL_NAME,  ");
                sbSql.AppendLine("          car_type as CAR_TYPE ,     ");
                sbSql.AppendLine("          position as POSITION,      ");
                sbSql.AppendLine("          lot_qty as LOT_QTY      ");
                sbSql.AppendLine(" FROM p_model_mast   ");
                sbSql.AppendLine(" WHERE 1=1           ");
                if (txtModelNo.Text.Trim() != "")
                {
                    sbSql.AppendLine(" AND   model_no like '%" + txtModelNo.Text.Trim() + "%'   ");
                }
                if (txtModelName.Text.Trim() != "")
                {
                    sbSql.AppendLine(" AND   model_name like '%" + txtModelName.Text.Trim() + "%'   ");
                }
                sSql = sbSql.ToString();
                Console.WriteLine("SQL=" + sSql);
                com = new NpgsqlCommand(sSql, NPG_conn);
                NpgsqlDataAdapter ad = new NpgsqlDataAdapter(com);

                dt = new DataTable();
                ad.Fill(dt);

                this.dataGridView1.DataSource = dt;


            }
            catch (NpgsqlException ne)
            {
                MessageBox.Show(" Error(400) : " + ne.ToString());
                return;
            }
            finally
            {
                NPG_conn.Close();
                NPG_conn = null;
                com.Dispose();
                com = null;
            }
        }
   
         
        private void btnSearch_Click(object sender, EventArgs e)
        {
            gridSearch();
        }

        private void dataGridView1_DoubleClick(object sender, EventArgs e)
        {

            if (dataGridView1.RowCount  > 1)
            {
                if (DialogResult.OK == MessageBox.Show("Do you want delete?", "Delete", MessageBoxButtons.OKCancel))
                {

                    string sModelNo = "";
                    string sCustomerCode ="";
                     
                    try
                    {
                        sCustomerCode = dataGridView1.CurrentRow.Cells[0].Value.ToString();
                        sModelNo = dataGridView1.CurrentRow.Cells[1].Value.ToString();
                        
                        
                    }
                    catch { }

                    if (sModelNo != "" && sCustomerCode != "")
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

                        NpgsqlCommand cmd = null;

                        try
                        {
                            string sSql;
                            StringBuilder sbSql = new StringBuilder();
                            //삭제

                            sbSql.AppendLine(" DELETE FROM P_MODEL_MAST                       ");
                            sbSql.AppendLine(" where CUSTOMER_CODE = '" + sCustomerCode.Trim().Trim() + "'  ");
                            sbSql.AppendLine(" AND   MODEL_NO = '" + sModelNo.Trim() + "'  ");
                            sSql = sbSql.ToString();
                            cmd = new NpgsqlCommand(sSql, NPG_conn);
                            cmd.ExecuteNonQuery();
                        }
                        catch (NpgsqlException ne)
                        {
                            Console.WriteLine("INSERT INTO Error=" + ne.ToString());
                        }
                        finally
                        {
                            // Console.WriteLine("Closing connections");                             
                            NPG_conn.Close();
                            NPG_conn = null;
                            cmd.Dispose();
                            cmd = null;
                        }

                        gridSearch();
                         
                    }
                }
            }
        }

        private void txtModelNo_TextChanged(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void txtModelName_TextChanged(object sender, EventArgs e)
        {

        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
    }
}
