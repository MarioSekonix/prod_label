using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.Common;
using System.Data.OleDb;
using System.Data.OracleClient;

namespace Sekonix_pop
{
    public class DataCommon
    {
        private OracleDataAdapter oDataAdapter;
        public OracleConnection oConn;
        private OracleTransaction oTr;
        //private OdbcConnection odConn;
        //cn= new OleDbConnection("Driver={SQL Server};Server=mySQLServer;UID=sa;PWD=myPassword;Database=Northwind;");
        //string sqlConn = "Data Source=118.32.222.193:1521/SekoNix;User ID='SEKONIX_POP';Password='bkit'";

        
        public DataCommon()
        {
            //ID : MONITOR, PW : GERPMONITOR, service name=ptprod
            //this.odConn = new OdbcConnection("Driver={Microsoft ODBC for Oracle};Server=FCS;UID=scott;PW=scott;");
            //this.oConn = new OleDbConnection("Provider=MSDAORA;Server=191.1.8.81;User Id=PTMON;Password=MONITOR01;");
            //this.oConn = new OleDbConnection("Provider=MSDAORA;Data Source=" + strDBName + ";Persist Security Info=True;Password=" + strPassword + ";User ID=" + strUserID + " ");
            //this.oConn = new OleDbConnection("Provider=MSDAORA;Persist Security Info=False;User ID=아이디;Password=비밀번호;Data Source=TEST");
            //this.oConn.ConnectionTimeout = 15;

            //this.oConn = new OracleConnection("Data Source=( DESCRIPTION = ( ADDRESS_LIST = ( ADDRESS = ( PROTOCOL = TCP )( HOST = 192.168.10.173 )( PORT = 1521 ) ) )( CONNECT_DATA = ( SERVER = DEDICATED )( SERVICE_NAME = orcl ) ) ); User Id= bkit_admin; Password = 1111;");

            this.oConn = new OracleConnection("Data Source=( DESCRIPTION = ( ADDRESS_LIST = ( ADDRESS = ( PROTOCOL = TCP )( HOST = 118.32.222.193 )( PORT = 1521 ) ) )( CONNECT_DATA = ( SERVER = DEDICATED )( SERVICE_NAME = SekoNix ) ) ); User Id= SEKONIX_POP; Password = bkit;");
            //this.oConn = new OracleConnection(sqlConn);
        }


        public DataCommon(string HOST , string PORT)
        {
            this.oConn = new OracleConnection("Data Source=( DESCRIPTION = ( ADDRESS_LIST = ( ADDRESS = ( PROTOCOL = TCP )( HOST = " + HOST + " )( PORT = " + PORT + " ) ) )( CONNECT_DATA = ( SERVER = DEDICATED )( SERVICE_NAME = SekoNix ) ) ); User Id= SEKONIX_POP; Password = bkit;");
        }



        public DataCommon(string strUserID, string strPassword, string strDBName)
        {
            //ID : MONITOR, PW : GERPMONITOR, service name=ptprod
            //this.odConn = new OdbcConnection("Driver={Microsoft ODBC for Oracle};Server=FCS;UID=scott;PW=scott;");
            //this.oConn = new OleDbConnection("Provider=MSDAORA;Server=191.1.8.81;User Id=PTMON;Password=MONITOR01;");
            //this.oConn = new OleDbConnection("Provider=MSDAORA;Data Source=" + strDBName + ";Persist Security Info=True;Password=" + strPassword + ";User ID=" + strUserID + " ");
            //this.oConn = new OleDbConnection("Provider=MSDAORA;Persist Security Info=False;User ID=아이디;Password=비밀번호;Data Source=TEST");
            //this.oConn.ConnectionTimeout = 15;

            this.oConn = new OracleConnection("Data Source=" + strDBName + ";User ID=" + strUserID + ";Password=" + strPassword + ";persist security info=false; charset=utf8");

        }

        public DataCommon(OracleConnection oConn)
        {
            this.oConn = oConn;
        }

        public OracleConnection getConnection()
        {
            return this.oConn;
        }
        public bool DBConnOpen()
        {
            try
            {
                this.oConn.Open();
            }
            catch
            {
                return false;
            }
            return true;
        }

        public bool DBConnClose()
        {
            try
            {
                this.oConn.Close();
            }
            catch
            {
                return false;
            }
            return true;
        }

        public void DBConnOpenWithTr()
        {
            this.oConn.Open();
            this.oTr = this.oConn.BeginTransaction();
        }

        public void DBConnCloseWithCommit()
        {
            this.oTr.Commit();
            this.oConn.Close();
        }

        public void DBConnCloseWithRollback()
        {
            this.oTr.Rollback();
            this.oConn.Close();
        }

        public void startTransaction()
        {
            if (this.oConn.State == ConnectionState.Open)
            {
                this.oTr = this.oConn.BeginTransaction();
            }
            else
            {
                this.oConn.Open();
                this.oTr = this.oConn.BeginTransaction();
            }
        }

        public DataTable getTableSchema(string tableName)
        {
            DataTable tableSchema = new DataTable();
            this.oDataAdapter = new OracleDataAdapter("SELECT TOP 0 * FROM " + tableName, this.oConn);
            this.oDataAdapter.MissingSchemaAction = MissingSchemaAction.AddWithKey; //Default Add
            this.oDataAdapter.MissingMappingAction = MissingMappingAction.Passthrough; //Default
            this.oDataAdapter.FillSchema(tableSchema, SchemaType.Mapped);
            //this.oDataAdapter.Fill(tableSchema);
            return tableSchema;
        }

        /// <summary>
        /// 오라클용 테이블스키마 가져오기

        /// </summary>
        /// <param name="tableName">테이블명</param>
        /// <returns></returns>
        public DataTable getTableSchemaOra(string tableName)
        {
            DataTable tableSchema = new DataTable();
            this.oDataAdapter = new OracleDataAdapter("SELECT * FROM " + tableName + " WHERE ROWNUM < 1", this.oConn);
            this.oDataAdapter.MissingSchemaAction = MissingSchemaAction.AddWithKey; //Default Add
            this.oDataAdapter.MissingMappingAction = MissingMappingAction.Passthrough; //Default
            this.oDataAdapter.FillSchema(tableSchema, SchemaType.Mapped);
            //this.oDataAdapter.Fill(tableSchema);
            return tableSchema;
        }

        public DataTable getTable(string strSQL)
        {
            DataTable dt = new DataTable();
            try
            {
                //Console.WriteLine("1111=" + strSQL);
                this.oDataAdapter = new OracleDataAdapter(strSQL, this.oConn);
                //this.oDataAdapter.MissingSchemaAction = MissingSchemaAction.AddWithKey;
                this.oDataAdapter.Fill(dt);
                //this.oConn.Dispose();
            }
            catch (Exception ex)
            {
                Console.WriteLine( ex.Message);
            }
            return dt;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ht"></param>
        /// <returns></returns>
        public DataSet getDataSet(System.Collections.Hashtable ht)
        {
            DataSet ds = new DataSet();

            try
            {
                foreach (System.Collections.DictionaryEntry de in ht)
                {
                    this.oDataAdapter = new OracleDataAdapter(de.Value.ToString(), this.oConn);
                    //this.oDataAdapter.MissingSchemaAction = MissingSchemaAction.AddWithKey;
                    this.oDataAdapter.Fill(ds, de.Key.ToString());
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return ds;
        }

        public object getSimpleScalar(string strSQL)
        {
            object obj;

            try
            {
                OracleCommand cmd = new OracleCommand(strSQL, this.oConn);
                
                this.oConn.Open();
                obj = cmd.ExecuteScalar();
                this.oConn.Close();
                this.oConn.Dispose();
                if (obj == null)
                {
                    obj = "";
                }
                return obj;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return null;
            }
        }

        /// <summary>
        /// 단일 실행
        /// </summary>
        /// <param name="strSQL"></param>
        /// <returns></returns>
        public int execNonQuery(string strSQL)
        {
            int affectRows = 0;
            bool isConnOpened = false;

            if (this.oConn.State == ConnectionState.Open)
                isConnOpened = true;
            else
            {
                this.oConn.Open();
            }

            try
            {

                this.oTr = this.oConn.BeginTransaction();
                OracleCommand cmd = new OracleCommand(strSQL, this.oConn, this.oTr);
                cmd.CommandType = CommandType.Text;
                affectRows = cmd.ExecuteNonQuery();
                //Console.WriteLine(">>>>>>>>>>>>>>>>> isConnOpened =" + isConnOpened);
                if (!isConnOpened)
                {
                    this.oTr.Commit();
                    this.oConn.Close();
                }
                return affectRows;
            }
            catch (Exception ex)
            {
                this.oTr.Rollback();
                this.oConn.Close();
                throw ex;
            }
        }

        /// <summary>
        /// 복수 실행
        /// </summary>
        /// <param name="listSQL"></param>
        /// <returns></returns>
        public int execNonQuery(List<string> listSQL)
        {
            int affectRows = 0;
            bool isConnOpened = false;

            if (this.oConn.State == ConnectionState.Open)
                isConnOpened = true;
            else
            {
                this.oConn.Open();
            }

            try
            {

                this.oTr = this.oConn.BeginTransaction();
                OracleCommand cmd = new OracleCommand();
                cmd.Connection = this.oConn;
                cmd.Transaction = this.oTr;
                cmd.CommandType = CommandType.Text;

                foreach (string str in listSQL)
                {
                    cmd.CommandText = str;
                    affectRows += cmd.ExecuteNonQuery();
                }

                if (!isConnOpened)
                {
                    this.oTr.Commit();
                    this.oConn.Close();
                }
                return affectRows;
            }
            catch (Exception ex)
            {
                this.oTr.Rollback();
                this.oConn.Close();
                throw ex;
            }
        }

        /// <summary>
        /// 프로시져 실행, 결과값을 리턴
        /// </summary>
        /// <param name="spName"></param>
        /// <param name="cmd"></param>
        /// <returns></returns>
        public OracleCommand execNonQuery(string spName, OracleCommand cmd)
        {
            int affectRows = 0;
            bool isConnOpened = false;
            //OracleParameterCollection opc = new OracleParameterCollection();
            //OracleDataReader odrReturn;

            if (this.oConn.State == ConnectionState.Open)
                isConnOpened = true;
            else
            {
                this.oConn.Open();
            }

            try
            {
                this.oTr = this.oConn.BeginTransaction();
                cmd.CommandText = spName;
                cmd.Connection = this.oConn;
                cmd.Transaction = this.oTr;
                cmd.CommandType = CommandType.StoredProcedure;
                //cmd.Parameters.AddRange(opc);

                affectRows = cmd.ExecuteNonQuery();

                if (!isConnOpened)
                {
                    this.oTr.Commit();
                    this.oConn.Close();

                    //foreach (OracleParameter op in cmd.Parameters)
                    //{
                    //    if (op.Direction == ParameterDirection.Output)
                    //        opc.Add(op);
                    //}
                }

                return cmd;
            }
            catch (Exception ex)
            {
                this.oTr.Rollback();
                this.oConn.Close();
                throw ex;
            }
        }

        /// <summary>
        /// 프로시져 실행, 결과값을 리턴
        /// </summary>
        /// <param name="ht">key = 프로시져 이름, value = oraclecommand</param>
        /// <returns></returns>
        public int execNonQuery(string strSpName, List<OracleCommand> list, CommandType cmdType)
        {
            int affectRows = 0;
            bool isConnOpened = false;
            //OracleDataReader odrReturn;

            if (this.oConn.State == ConnectionState.Open)
                isConnOpened = true;
            else
                this.oConn.Open();

            try
            {
                this.oTr = this.oConn.BeginTransaction();

                foreach (OracleCommand cmd in list)
                {
                    cmd.CommandText = strSpName;
                    cmd.Connection = this.oConn;
                    cmd.Transaction = this.oTr;
                    cmd.CommandType = cmdType;
                    //cmd.Parameters.AddRange(opc);

                    affectRows = cmd.ExecuteNonQuery();
                }

                if (!isConnOpened)
                {
                    this.oTr.Commit();
                    this.oConn.Close();
                }

                return affectRows;
            }
            catch (Exception ex)
            {
                this.oTr.Rollback();
                this.oConn.Close();
                throw ex;
            }
        }

        public int execNonQuery(string strSpName, List<OracleCommand> list)
        {
            return execNonQuery(strSpName, list, CommandType.StoredProcedure);
        }

        /// <summary>
        /// 프로시져 실행
        /// </summary>
        /// <param name="spName"></param>
        /// <param name="arrParms"></param>
        /// <returns></returns>
        public int execNonQuery(string spName, OracleParameter[] arrParms, CommandType cmdType)
        {
            int affectRows = 0;
            bool isConnOpened = false;
            if (this.oConn.State == ConnectionState.Open)
                isConnOpened = true;
            else
            {
                this.oConn.Open();
            }

            try
            {
                this.oTr = this.oConn.BeginTransaction();
                OracleCommand cmd = new OracleCommand(spName, this.oConn, this.oTr);
                cmd.CommandType = cmdType;
                foreach (OracleParameter P in arrParms)
                    cmd.Parameters.Add(P);

                affectRows = cmd.ExecuteNonQuery();
                if (!isConnOpened)
                {
                    this.oTr.Commit();
                    this.oConn.Close();
                }
                return affectRows;
            }
            catch (Exception ex)
            {
                this.oTr.Rollback();
                this.oConn.Close();
                throw ex;
            }
        }

        public int execNonQuery(string spName, OracleParameter[] arrParms)
        {
            return execNonQuery(spName, arrParms, CommandType.StoredProcedure);
        }

        /// <summary>
        /// 프로시져 실행
        /// </summary>
        /// <param name="spName"></param>
        /// <param name="listParms"></param>
        /// <returns></returns>
        public int execNonQuery(string spName, List<OracleParameter> listParms, CommandType cmdType)
        {
            int affectRows = 0;
            bool isConnOpened = false;
            if (this.oConn.State == ConnectionState.Open)
                isConnOpened = true;
            else
            {
                this.oConn.Open();
            }

            try
            {
                this.oTr = this.oConn.BeginTransaction();
                OracleCommand cmd = new OracleCommand(spName, this.oConn, this.oTr);
                cmd.CommandType = cmdType;
                foreach (OracleParameter P in listParms)
                    cmd.Parameters.Add(P);

                affectRows = cmd.ExecuteNonQuery();
                if (!isConnOpened)
                {
                    this.oTr.Commit();
                    this.oConn.Close();
                }
                return affectRows;
            }
            catch (Exception ex)
            {
                this.oTr.Rollback();
                this.oConn.Close();
                throw ex;
            }
        }

        public int execNonQuery(string spName, List<OracleParameter> listParms)
        {
            return execNonQuery(spName, listParms, CommandType.StoredProcedure);
        }

        /// <summary>
        /// 단일 실행형(테이블 전체를 업데이트 할때)
        /// </summary>
        /// <param name="dt">업데이트할 테이블</param>
        public int updateTable(DataTable dt, string strTableName)
        {
            int affectRow = this.updateTable(dt, strTableName, "*");
            return affectRow;
        }

        /// <summary>
        /// 단일 실행형(일부 컬럼만 업데이트 할때)
        /// </summary>
        /// <param name="dt">업데이트할 테이블</param>
        /// <param name="strColNm">수정할 컬럼이름</param>
        /// <returns></returns>
        public int updateTable(DataTable dt, string strTableName, string strColNm)
        {
            int affectRow = 0;
            OracleCommandBuilder cb;
            this.oDataAdapter = new OracleDataAdapter("SELECT " + strColNm + " FROM " + strTableName, this.oConn);
            cb = new OracleCommandBuilder(this.oDataAdapter);

            this.oDataAdapter.DeleteCommand = cb.GetDeleteCommand();
            this.oDataAdapter.InsertCommand = cb.GetInsertCommand();
            this.oDataAdapter.UpdateCommand = cb.GetUpdateCommand();
            try
            {
                this.oConn.Open();
                this.oTr = this.oConn.BeginTransaction();
                this.oDataAdapter.DeleteCommand.Transaction = this.oTr;
                this.oDataAdapter.InsertCommand.Transaction = this.oTr;
                this.oDataAdapter.UpdateCommand.Transaction = this.oTr;

                affectRow = this.oDataAdapter.Update(dt);
                this.oTr.Commit();
                return affectRow;
            }
            catch (Exception ex)
            {
                this.oTr.Rollback();
                this.oConn.Close();
                throw new Exception("DB 업로드 오류", ex);
            }
        }

        //연속실행형

        public void updateDataSet(DataSet ds)
        {
            //DataTable dt;
            int i = 0;
            OracleCommandBuilder cb;
            OracleDataAdapter[] oDA = new OracleDataAdapter[ds.Tables.Count];
            foreach (DataTable dt in ds.Tables)
            {
                oDA[i] = new OracleDataAdapter("SELECT * FROM " + dt.TableName, this.oConn);
                cb = new OracleCommandBuilder(oDA[i]);
                oDA[i].DeleteCommand = cb.GetDeleteCommand();
                oDA[i].InsertCommand = cb.GetInsertCommand();
                oDA[i].UpdateCommand = cb.GetUpdateCommand();
                i++;
            }

            try
            {
                i = 0;
                this.oConn.Open();
                this.oTr = this.oConn.BeginTransaction();
                foreach (DataTable updateTable in ds.Tables)
                {
                    oDA[i].DeleteCommand.Transaction = this.oTr;
                    oDA[i].InsertCommand.Transaction = this.oTr;
                    oDA[i].UpdateCommand.Transaction = this.oTr;

                    oDA[i].Update(updateTable);
                    i++;
                }
                this.oTr.Commit();
            }
            catch (Exception oErr)
            {
                this.oTr.Rollback();
                this.oConn.Close();
                throw oErr;
            }
        }

        #region DsReturn - 공용 데이터셋 반환 함수
        /// <summary>
        /// 작성자: 손정민

        /// 작성일: 2011-09-22
        /// 설  명: 공용 데이터셋 반환 함수
        /// </summary>
        /// <param name="_htOutputCursorName">Output 커서명</param>
        /// <param name="_strProcedureName">프로시저명(패키지로 실행)</param>
        /// <param name="_htParams">변수값을 가지는 Hashtable</param>
        /// <returns>데이터셋</returns>
        public DataSet DsReturn(System.Collections.Hashtable _htOutputCursorName, string _strProcedureName, System.Collections.Hashtable _htParams)
        {
            DataSet _ds = new DataSet();
            try
            {
                OracleCommand _oCmd = new OracleCommand();
                _oCmd.Connection = this.oConn;
                _oCmd.CommandText = _strProcedureName;
                _oCmd.CommandType = CommandType.StoredProcedure;

                // Input 파라미터
                foreach (System.Collections.DictionaryEntry _dicEnt in _htParams)
                {
                    /*
                     * Remark: 오라클용 데이터 타입 추가로 파악 필요
                     */
                    switch (_dicEnt.Value.GetType().Name)
                    {
                        case "Int16":
                            _oCmd.Parameters.Add(_dicEnt.Key.ToString(), OracleType.Int16).Value = Int16.Parse(_dicEnt.Value.ToString());
                            break;
                        case "Int32":
                            _oCmd.Parameters.Add(_dicEnt.Key.ToString(), OracleType.Int32).Value = Int32.Parse(_dicEnt.Value.ToString());
                            break;
                        case "Decimal":
                            _oCmd.Parameters.Add(_dicEnt.Key.ToString(), OracleType.Float).Value = Decimal.Parse(_dicEnt.Value.ToString());
                            break;
                        case "Numeric":
                            _oCmd.Parameters.Add(_dicEnt.Key.ToString(), OracleType.Number).Value = int.Parse(_dicEnt.Value.ToString());
                            break;
                        default:
                            _oCmd.Parameters.Add(_dicEnt.Key.ToString(), OracleType.VarChar).Value = _dicEnt.Value.ToString();
                            break;
                    }
                }

                // Output 파라미터
                foreach (System.Collections.DictionaryEntry _dicEnt in _htOutputCursorName)
                {
                    _oCmd.Parameters.Add(_dicEnt.Value.ToString(), OracleType.Cursor).Direction = ParameterDirection.Output;
                }

                OracleDataAdapter _oDa = new OracleDataAdapter(_oCmd);
                _oDa.Fill(_ds);
            }
            catch (Exception _err)
            {
                throw _err;
            }
            return _ds;
        }
        #endregion

        #region ExecuteNonQuery : 반환 결과 없는 쿼리 실행
        /// <summary>
        /// 반환 결과 없는 쿼리 실행
        /// </summary>
        /// <param name="_strProcedureName">프로시저명</param>
        /// <param name="_htParams">Input 파라미터</param>
        /// <returns>실행 성공여부</returns>
        public bool ExecuteNonQuery(string _strProcedureName, System.Collections.Hashtable _htParams)
        {
            bool _retValue = false;
            try
            {
                OracleCommand _oCmd = new OracleCommand();
                if (this.oConn.State != ConnectionState.Open)
                    this.oConn.Open();
                _oCmd.Connection = this.oConn;
                _oCmd.CommandText = _strProcedureName;
                _oCmd.CommandType = CommandType.StoredProcedure;

                // Input 파라미터
                foreach (System.Collections.DictionaryEntry _dicEnt in _htParams)
                {
                    /*
                     * Remark: 오라클용 데이터 타입 추가로 파악 필요
                     */
                    switch (_dicEnt.Value.GetType().Name)
                    {
                        case "Int16":
                            _oCmd.Parameters.Add(_dicEnt.Key.ToString(), OracleType.Int16).Value = Int16.Parse(_dicEnt.Value.ToString());
                            break;
                        case "Int32":
                            _oCmd.Parameters.Add(_dicEnt.Key.ToString(), OracleType.Int32).Value = Int32.Parse(_dicEnt.Value.ToString());
                            break;
                        case "Decimal":
                            _oCmd.Parameters.Add(_dicEnt.Key.ToString(), OracleType.Float).Value = Decimal.Parse(_dicEnt.Value.ToString());
                            break;
                        case "Numeric":
                            _oCmd.Parameters.Add(_dicEnt.Key.ToString(), OracleType.Number).Value = int.Parse(_dicEnt.Value.ToString());
                            break;
                        default:
                            _oCmd.Parameters.Add(_dicEnt.Key.ToString(), OracleType.VarChar).Value = _dicEnt.Value.ToString();
                            break;
                    }
                }

                int _intReturn = _oCmd.ExecuteNonQuery();
                _retValue = true;
            }
            catch (Exception _err)
            {
                _retValue = false;
                throw _err;
            }
            return _retValue;
        }
        #endregion
    }
}
