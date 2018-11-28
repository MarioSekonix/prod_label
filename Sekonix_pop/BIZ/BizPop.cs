using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Data;

using Sekonix_pop;

namespace BIZ
{
    static class BizPop
    {
        static DataCommon dc = new DataCommon();
        //static DataCommon dc = new DataCommon(Sekonix_pop.Properties.Settings.Default.HOST, Sekonix_pop.Properties.Settings.Default.PORT);

        /// <summary>
        /// 반환값형식
        /// </summary>
        public class Result
        {
            int iValue = 0;
            string sMessage = string.Empty;
            DataTable dtData = null;

            public Result()
            { }

            public Result(int _value, string _Message, DataTable _data)
            {
                iValue = _value;
                sMessage = _Message;
                dtData = _data.Copy();
            }

            /// <summary>
            /// 결과값 : [1] 성공 , [0] 값없음 , [-1]에러
            /// </summary>
            public int VALUE
            {
                get { return iValue; }
                set { iValue = value; }
            }

            /// <summary>
            /// 결과메세지 : 성공시 메세지는 없음.
            /// </summary>
            public string MESSAGE
            {
                get { return sMessage; }
                set { sMessage = value; }
            }

            /// <summary>
            /// 결과DATA : DataTable형식이다.
            /// </summary>
            public DataTable DATA
            {
                get { return dtData; }
                set { dtData = value.Copy(); }
            }


        }

        #region < TEST >

        static public Result getTestCombo()
        {
            Result ret = new Result();

            try
            {

                DataTable dt = new DataTable();

                StringBuilder sb = new StringBuilder();
                sb.AppendLine("SELECT                   ");
                sb.AppendLine("      CODE               ");
                sb.AppendLine("    , NAME               ");
                sb.AppendLine("    , UPPER_CODE         ");
                sb.AppendLine("FROM SYS_CODE            ");
                sb.AppendLine("WHERE DEL_YN != 'Y'      ");
                sb.AppendLine("AND TYPE = 'TEST'        ");

                string sSql = string.Format(sb.ToString());
                dc = new DataCommon();
                dt = dc.getTable(sSql);


                if (dt != null)
                {
                    if (dt.Rows.Count < 1)
                    {
                        ret.VALUE = 0;
                        ret.MESSAGE = "DATA가 없습니다.";
                    }
                    else
                    {
                        ret.VALUE = 1;
                    }

                    ret.DATA = dt;
                }
            }
            catch (Exception ex)
            {
                ret.VALUE = -1;
                ret.MESSAGE = ex.Message;
            }

            return ret;
        }


        static public Result getTestGrid()
        {
            Result ret = new Result();

            try
            {

                DataTable dt = new DataTable();

                StringBuilder sb = new StringBuilder();
                sb.AppendLine("SELECT                   ");
                sb.AppendLine("      1 AS CHK           ");
                sb.AppendLine("    , TYPE               ");
                sb.AppendLine("    , NAME               ");
                sb.AppendLine("    , CODE               ");
                sb.AppendLine("    , CRT_DATE           ");
                sb.AppendLine("    , DEL_YN             ");
                sb.AppendLine("FROM SYS_CODE            ");
                sb.AppendLine("WHERE DEL_YN != 'Y'      ");
                sb.AppendLine("AND TYPE = 'TEST'        ");

                string sSql = string.Format(sb.ToString());
                dc = new DataCommon();
                dt = dc.getTable(sSql);


                if (dt != null)
                {
                    if (dt.Rows.Count < 1)
                    {
                        ret.VALUE = 0;
                        ret.MESSAGE = "DATA가 없습니다.";
                    }
                    else
                    {
                        ret.VALUE = 1;
                    }

                    ret.DATA = dt;
                }
            }
            catch (Exception ex)
            {
                ret.VALUE = -1;
                ret.MESSAGE = ex.Message;
            }

            return ret;

        }

        #endregion 


        /// <summary>
        /// 시스템코드정보를 조회한다.
        /// </summary>
        /// <param name="TYPE">코드유형</param>
        /// <returns></returns>
        static public Result getSysCode(string TYPE , string UPPER_CODE = "" )
        {
            Result ret = new Result();

            try
            {

                DataTable dt = new DataTable();

                StringBuilder sb = new StringBuilder();
                sb.AppendLine("SELECT                   ");
                sb.AppendLine("      CODE               ");
                sb.AppendLine("    , NAME               ");
                sb.AppendLine("FROM SYS_CODE            ");
                sb.AppendLine("WHERE DEL_YN = 'N'       ");
                sb.AppendLine("AND TYPE = '{0}'         ");
                
                if( !string.IsNullOrEmpty( UPPER_CODE ) )
                {
                    sb.AppendLine("AND UPPER_CODE = '{1}'   ");
                }
                string sSql = string.Format(sb.ToString(), TYPE , UPPER_CODE );
                dc = new DataCommon();
                dt = dc.getTable(sSql);


                if (dt != null)
                {
                    if (dt.Rows.Count < 1)
                    {
                        ret.VALUE = 0;
                        ret.MESSAGE = "DATA가 없습니다.";
                    }
                    else
                    {
                        ret.VALUE = 1;
                    }

                    ret.DATA = dt;
                }
            }
            catch (Exception ex)
            {
                ret.VALUE = -1;
                ret.MESSAGE = ex.Message;
            }

            return ret;
        }


        /// <summary>
        /// 자재가 사용가능한 상태인지 체크.
        /// </summary>
        /// <param name="BARCODE">자재바코드 ( 코드+LOT )</param>
        /// <returns>빈칸 :사용가능 / 그외 :에러메세지</returns>
        static public string CheckUsedMaterialYN(string BARCODE)
        {
            string ret = string.Empty;

            try
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("SELECT FN_MATERIAL_USE_YN( '{0}') FROM DUAL ");
               
                
                //Console.WriteLine("*********** 자재가 사용가능한 상태인지 체크(pop warehouse에서 INOUT=out인지 확인) 시작***************************");
                //Console.WriteLine("SQL= " + sb.ToString());
                //Console.WriteLine("BARCODE=[" + BARCODE.ToString() + "]"); 
                //Console.WriteLine("*********** 자재가 사용가능한 상태인지 체크(pop warehouse에서 INOUT=out인지 확인) 종료***************************");
                 
                string sSql = string.Format(sb.ToString(), BARCODE.Trim() );

                //Console.WriteLine("자재출고정보확인=" + sSql);

                dc = new DataCommon();
                object oRet = dc.getSimpleScalar(sSql);
                //Console.WriteLine("FN_MATERIAL_USE_YN= " + oRet.ToString());                
                if (oRet != null)
                {
                    ret = oRet.ToString();

                    //if (oRet.ToString() == "Y")
                    //{
                    //    return true;
                    //}
                }
            }
            catch (Exception ex)
            {
                //throw ex;
                Console.WriteLine(ex.ToString());
                ret = "자재 확인 프로세스에 문제가 발생하였습니다(ERR-2000)";
            }

            return ret;
        }


        /// <summary>
        /// 해당공정에 자재가 이미 등록되었는지 체크
        /// </summary>
        /// <param name="MAP_NO">조회번호</param>
        /// <param name="OP_CODE">공정코드</param>
        /// <param name="MAT_CODE">자재코드</param>        
        /// <param name="LOT">자재LOT</param>
        /// <returns>빈칸 : 없음 / 자재SEQ : 이미존재</returns>
        static public string CheckMaterialExistYN(string MAP_NO, string OP_CODE, string MAT_CODE, string LOT)
        {
            string ret = string.Empty;

            try
            {                
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("SELECT SEQ FROM POP_PROD_IN_MAT WHERE MAP_NO='{0}' AND OP_CODE='{1}' AND MAT_CODE='{2}' AND LOT='{3}' ");

                /*
                Console.WriteLine("***************해당공정에 자재가 이미 등록되었는지 체크 시작************");
                Console.WriteLine("SQL " + sb.ToString());
                Console.WriteLine("MAP_NO = " + MAP_NO);
                Console.WriteLine("OP_CODE = " + OP_CODE);
                Console.WriteLine("MAT_CODE = " + MAT_CODE);
                Console.WriteLine("LOT = " + LOT);
                Console.WriteLine("***************해당공정에 자재가 이미 등록되었는지 체크 종료************");
                */
                string sSql = string.Format(sb.ToString(), MAP_NO, OP_CODE, MAT_CODE, LOT );
                dc = new DataCommon();
                object oRet = dc.getSimpleScalar(sSql);

                if (oRet != null)
                {
                    ret = oRet.ToString();
                }
            }
            catch  
            {
                throw ;
                //ret = ex.Message;
            }

            return ret;
        }



        /// <summary>
        /// 공정명을 반환한다.
        /// </summary>
        /// <param name="OP_CODE">공정코드</param>
        /// <returns>공정명</returns>
        static public string getOpName(string OP_CODE)
        {
            string ret = string.Empty;

            try
            {
                DataTable dt = new DataTable();

                StringBuilder sb = new StringBuilder();
                sb.AppendLine("SELECT OP_NAME FROM POP_OP_CODE WHERE OP_CODE = '{0}' ");

                string sSql = string.Format(sb.ToString(), OP_CODE);
                dc = new DataCommon();
                object oRet = dc.getSimpleScalar(sSql);

                if (oRet != null)
                {
                    ret = oRet.ToString();

                    //if (oRet.ToString() == "Y")
                    //{
                    //    return true;
                    //}
                }
            }
            catch (Exception ex)
            {
                //throw ex;
                ret = ex.Message;
            }

            return ret;
        }



        #region < 단말기 >


        #region < GET >

        /// <summary>
        /// 시간을 반환한다.
        /// </summary>
        /// <param name="FORMAT">시간반환 FORMAT형식</param>
        /// <returns></returns>
        static public Result getSysdate(string FORMAT = "yy/MM/dd HH:mi:ss")
        {
            Result ret = new Result();

            try
            {

                DataTable dt = new DataTable();

                StringBuilder sb = new StringBuilder();
                sb.AppendLine("SELECT                                       ");
                sb.AppendLine("      TO_CHAR( SYSDATE , '{0}' ) AS \"DATE\" ");
                sb.AppendLine("FROM DUAL                                    ");

                string sSql = string.Format(sb.ToString(), FORMAT);
                dc = new DataCommon();
                dt = dc.getTable(sSql);


                if (dt != null)
                {
                    if (dt.Rows.Count < 1)
                    {
                        ret.VALUE = 0;
                        ret.MESSAGE = "DATA가 없습니다.";
                    }
                    else
                    {
                        ret.VALUE = 1;
                    }

                    ret.DATA = dt;
                }
            }
            catch (Exception ex)
            {
                ret.VALUE = -1;
                ret.MESSAGE = ex.Message;
            }

            return ret;
        }





        /// <summary>
        /// 현재공정의 작업번호 및 작업상태를 조회 합니다.
        /// </summary>
        /// <param name="OP_CODE">공정코드</param>
        /// <param name="MAP_NO">조회번호</param>
        /// <returns></returns>
        static public Result getMappingState(string OP_CODE , string MAP_NO)
        {
            Result ret = new Result();

            try
            {
                DataTable dt = new DataTable();
                StringBuilder sb = new StringBuilder();               
                
                sb.AppendLine("SELECT                                                               ");
                sb.AppendLine("      MAP.SEQ                                                        ");
                sb.AppendLine("    , JOB_STATE                                                      ");
                sb.AppendLine("    , IRT.JOB_UNIT                                                   ");
                sb.AppendLine("    , CASE WHEN JOB_UNIT = 'REAR' THEN MAP.REAR_NO                   ");
                sb.AppendLine("           WHEN JOB_UNIT = 'SERIAL' THEN MAP.SERIAL_NO               ");
                //sb.AppendLine("           WHEN JOB_UNIT = 'SW_BOX' THEN MAP.SW_BOX_NO               ");
                sb.AppendLine("           WHEN JOB_UNIT = 'SW_BOX' THEN MAP.JOB_NO               ");
                sb.AppendLine("           WHEN JOB_UNIT = 'TRAY' THEN MAP.JOB_NO                    ");
                sb.AppendLine("           ELSE 'JOB UNIT NOT FOUND: ' || MAP.JOB_NO  END AS MAP_NO  ");
                sb.AppendLine("FROM POP_LOT_MAPPING MAP                                             ");
                sb.AppendLine("LEFT JOIN POP_ITEM_ROUTINE IRT                                       ");
                sb.AppendLine("    ON MAP.ITEM_CODE = IRT.ITEM_CODE                                 ");
                sb.AppendLine("    AND MAP.NOW_OP_SEQ = IRT.SEQ                                     ");                
                sb.AppendLine("WHERE MAP.DEL_YN = 'N'                                               ");
                //sb.AppendLine("AND JOB_STATE IN ( '20' , '30' , '40') -- 작업중 :: 시작 , 중지 ,    ");
                sb.AppendLine("AND NOW_OP_CODE = '{0}'          -- 단말기의 공정코드                ");
                sb.AppendLine(" AND ( MAP.JOB_NO = '{1}'                                            ");
                sb.AppendLine("    OR MAP.REAR_NO = '{1}'                                           ");
                sb.AppendLine("    OR MAP.SERIAL_NO = '{1}'                                         ");
                sb.AppendLine("    OR MAP.SW_BOX_NO = '{1}'                                         ");
                sb.AppendLine(" )                                                                   ");

                /*
                Console.WriteLine("*********** 현재공정의 작업번호 및 작업상태를 조회 시작***************************");
                Console.WriteLine("SQL= " + sb.ToString());
                Console.WriteLine("OP_CODE= " + OP_CODE.ToString());
                Console.WriteLine("MAP_NO= " + MAP_NO.ToString()); 
                Console.WriteLine("*********** 현재공정의 작업번호 및 작업상태를 조회 종료***************************");
                */
 
                string sSql = string.Format(sb.ToString(), OP_CODE.Trim(), MAP_NO.Trim());
                dc = new DataCommon();
                dt = dc.getTable(sSql.ToString());
                
                if (dt != null)
                {
                    if (dt.Rows.Count < 1)
                    {
                        ret.VALUE = 0;
                        //ret.MESSAGE = "해당 작업번호는 현재공정에서 작업할 수 없습니다.";
                        ret.MESSAGE = "현재 공정에서 작업할 수 없는 품번입니다. \r확인 후 해당공정으로 전달해주세요";
                    }
                    else
                    {
                        ret.VALUE = 1;
                    }

                    ret.DATA = dt;
                }
            }
            catch (Exception ex)
            {
                ret.VALUE = -1;
                ret.MESSAGE = ex.Message;
            }

            return ret;
        }




        /// <summary>
        /// 터미널 MAIN작업목록 조회
        /// </summary>
        /// <param name="OP_CODE">공정코드</param>
        /// <returns></returns>
        static public Result getPU0410_MAIN(string OP_CODE, string MCH_CODE)
        {
            Result ret = new Result();

            try
            {

                DataTable dt = new DataTable();

                StringBuilder sb = new StringBuilder();

                /*
                sb.AppendLine("SELECT                                                               ");
                sb.AppendLine("      MAP.SEQ                                                        ");
                sb.AppendLine("    , JOB_STATE                                                      ");
                sb.AppendLine("    , IRT.JOB_UNIT                                                   ");
                sb.AppendLine("    , CASE WHEN JOB_UNIT = 'REAR' THEN MAP.REAR_NO                   ");
                sb.AppendLine("           WHEN JOB_UNIT = 'SERIAL' THEN MAP.SERIAL_NO               ");
                //sb.AppendLine("           WHEN JOB_UNIT = 'SW_BOX' THEN MAP.SW_BOX_NO               ");
                sb.AppendLine("           WHEN JOB_UNIT = 'SW_BOX' THEN MAP.JOB_NO               ");
                sb.AppendLine("           WHEN JOB_UNIT = 'TRAY' THEN MAP.JOB_NO                    ");
                sb.AppendLine("           ELSE 'JOB UNIT NOT FOUND: ' || MAP.JOB_NO  END AS MAP_NO  ");
                sb.AppendLine("    , MAP.JOB_NO                                                     ");
                sb.AppendLine("    , REAR_NO                                                        ");
                sb.AppendLine("    , SERIAL_NO                                                      ");
                sb.AppendLine("    , SW_BOX_NO                                                      ");
                sb.AppendLine("    , BOX_NO                                                         ");
                sb.AppendLine("    , MAP.ITEM_CODE                                                  ");
                sb.AppendLine("    , JOB.LOT_QTY                                                    ");
                sb.AppendLine("    , NOW_OP_SEQ                                                     ");
                sb.AppendLine("    , NOW_OP_CODE                                                    ");
                sb.AppendLine("    , MIN(STA_DATE) AS STA_DATE                                      ");
                sb.AppendLine("    , MIN(FIN_DATE) AS FIN_DATE                                      ");
                sb.AppendLine("    , NVL( SUM( BAD_QTY ) , 0 ) AS BAD_QTY                           ");
                sb.AppendLine("    , JOB.LOT_QTY - NVL( SUM( BAD_QTY ) , 0 ) AS TOT_QTY             ");
                sb.AppendLine("    , JOB.REMARK                                                     ");
                sb.AppendLine("FROM POP_LOT_MAPPING MAP                                             ");
                sb.AppendLine("JOIN POP_JOB_ORDER JOB                                               ");
                sb.AppendLine("    ON MAP.JOB_NO = JOB.JOB_NO                                       ");
                sb.AppendLine("LEFT JOIN                                                            ");
                sb.AppendLine("    (                                                                ");
                sb.AppendLine("        SELECT                                                       ");
                sb.AppendLine("              BAD_QTY                                                ");
                sb.AppendLine("            --, OP_CODE                                              ");
                sb.AppendLine("            , MAP_NO                                                 ");
                sb.AppendLine("        FROM POP_PROD_INSP                                           ");
                sb.AppendLine("        WHERE DEL_YN = 'N'                                           ");
                sb.AppendLine("    ) BAD                                                            ");
                sb.AppendLine("    --ON BAD.OP_CODE = MAP.NOW_OP_CODE                               ");
                sb.AppendLine("    ON ( MAP.JOB_NO = BAD.MAP_NO OR MAP.REAR_NO = BAD.MAP_NO         ");
                sb.AppendLine("     OR MAP.SERIAL_NO = BAD.MAP_NO OR MAP.SW_BOX_NO = BAD.MAP_NO )   ");
                sb.AppendLine("LEFT JOIN POP_ITEM_ROUTINE IRT                                       ");
                sb.AppendLine("    ON MAP.ITEM_CODE = IRT.ITEM_CODE                                 ");
                sb.AppendLine("    AND MAP.NOW_OP_SEQ = IRT.SEQ                                     ");
                sb.AppendLine("LEFT JOIN                                                            ");
                sb.AppendLine("    (                                                                ");
                sb.AppendLine("        SELECT                                                       ");
                sb.AppendLine("              CRT_DATE AS STA_DATE                                   ");
                sb.AppendLine("            , OP_CODE                                                ");
                sb.AppendLine("            , MAP_NO                                                 ");
                sb.AppendLine("        FROM POP_JOB_HIST                                            ");
                sb.AppendLine("        WHERE DEL_YN = 'N'                                           ");
                sb.AppendLine("        AND JOB_STATE = '30' --시작                                  ");
                sb.AppendLine("    ) STA                                                            ");
                sb.AppendLine("    ON STA.OP_CODE = MAP.NOW_OP_CODE                                 ");
                sb.AppendLine("AND ( MAP.JOB_NO = STA.MAP_NO OR MAP.REAR_NO = STA.MAP_NO            ");
                sb.AppendLine(" OR MAP.SERIAL_NO = STA.MAP_NO OR MAP.SW_BOX_NO = STA.MAP_NO )       ");
                sb.AppendLine("LEFT JOIN                                                            ");
                sb.AppendLine("    (                                                                ");
                sb.AppendLine("        SELECT                                                       ");
                sb.AppendLine("              NVL( MOD_DATE , CRT_DATE) AS FIN_DATE                  ");
                sb.AppendLine("            , OP_CODE                                                ");
                sb.AppendLine("            , MAP_NO                                                 ");
                sb.AppendLine("        FROM POP_JOB_HIST                                            ");
                sb.AppendLine("        WHERE DEL_YN = 'N'                                           ");
                sb.AppendLine("        AND JOB_STATE = '50' --종료                                  ");
                sb.AppendLine("    ) FIN                                                            ");
                sb.AppendLine("    ON FIN.OP_CODE = MAP.NOW_OP_CODE                                 ");
                sb.AppendLine("    AND ( MAP.JOB_NO = FIN.MAP_NO OR MAP.REAR_NO = FIN.MAP_NO        ");
                sb.AppendLine("     OR MAP.SERIAL_NO = FIN.MAP_NO OR MAP.SW_BOX_NO = FIN.MAP_NO )   ");
                sb.AppendLine("WHERE MAP.DEL_YN = 'N'                                               ");
                sb.AppendLine("AND JOB.DEL_YN = 'N'                                                 ");
                sb.AppendLine("AND JOB_STATE IN (  '30' , '40') -- 10:생성, 20:대기, 30:시작, 40:중지, 50:완료, 90:종료          ");
                sb.AppendLine("AND NOW_OP_CODE = '{0}'          -- 단말기의 공정코드                ");
                sb.AppendLine("GROUP BY MAP.SEQ                                                     ");
                sb.AppendLine("    , JOB_STATE                                                      ");
                sb.AppendLine("    , IRT.JOB_UNIT                                                   ");
                sb.AppendLine("    , MAP.JOB_NO                                                     ");
                sb.AppendLine("    , REAR_NO                                                        ");
                sb.AppendLine("    , SERIAL_NO                                                      ");
                sb.AppendLine("    , SW_BOX_NO                                                      ");
                sb.AppendLine("    , BOX_NO                                                         ");
                sb.AppendLine("    , MAP.ITEM_CODE                                                  ");
                sb.AppendLine("    , JOB.LOT_QTY                                                    ");
                sb.AppendLine("    , NOW_OP_SEQ                                                     ");
                sb.AppendLine("    , NOW_OP_CODE                                                    ");
                //sb.AppendLine("    , STA_DATE                                                       ");
                //sb.AppendLine("    , FIN_DATE                                                       ");
                sb.AppendLine("    , JOB.REMARK                                                     ");
                sb.AppendLine("ORDER BY  MAP.JOB_STATE                                              ");
                */

                //*************************************************************
                //*************************************************************
                //자기공정의 자기장비번호에서 작업한 목록만 보여주기

              
                sb.AppendLine(" select    a.SEQ                                                                 ");
                sb.AppendLine("         , a.job_no                                                              ");
                sb.AppendLine("         , a.item_code                                                           ");
                sb.AppendLine("         , a.job_state                                                           ");                
                sb.AppendLine("         , a.now_op_seq                                                          ");
                sb.AppendLine("         , a.now_op_code                                                         ");
                sb.AppendLine("         , hist.mch_code                                                         ");
                sb.AppendLine("         , b.lot_qty                                                             ");    
                sb.AppendLine("         , MAX(START_JOB.start_date) AS STA_DATE                                 ");            
                sb.AppendLine("         , MAX(START_JOB.start_date) AS STA_DATE                                 ");    
                sb.AppendLine("         , MAX(END_JOB.end_date) AS END_DATE                                     ");
                sb.AppendLine("         , NVL( SUM( BAD.BAD_QTY ) , 0 ) AS BAD_QTY                              ");
                sb.AppendLine("         , b.LOT_QTY - NVL( SUM( BAD.BAD_QTY ) , 0 ) AS TOT_QTY                  ");
                sb.AppendLine("         , a.REMARK                                                              ");
                sb.AppendLine(" from pop_lot_mapping a join pop_job_order b on a.job_no = b.job_no              ");
                sb.AppendLine(" inner join pop_job_hist hist on a.job_no = hist.map_no and a.now_op_code = hist.op_code                     ");
                sb.AppendLine(" left outer join ( select bad_qty, map_no from pop_prod_insp where del_yn='N') BAD on a.job_no = BAD.map_no  ");// 불량제품 수량 추출
                sb.AppendLine(" left outer join ( select crt_date as start_date, op_code, map_no from pop_job_hist where del_yn='N' and job_state ='30' ) START_JOB on a.job_no = START_JOb.map_no and START_JOB.op_code = a.now_op_code        ");// --시작시간 추출
                sb.AppendLine(" left outer join ( select nvl(mod_date, crt_date) as end_date, op_code, map_no from pop_job_hist where del_yn='N' and job_state ='50') END_JOB on END_JOB.map_no = a.job_no and END_JOB.op_code =  a.now_op_code ");//--종료시간 추출
                sb.AppendLine(" where a.del_yn='N'                                                                                          ");
                sb.AppendLine(" and   b.del_yn='N'                                                                                          ");
                sb.AppendLine(" AND   a.JOB_STATE IN (  '30' , '40', '50')                                          "); // 10:생성, 20:대기, 30:시작, 40:중지, 50:완료, 90:종료
                sb.AppendLine(" and   a.now_op_code = '{0}'                                                                                  ");
                sb.AppendLine(" and   hist.mch_code = '{1}'                                                                            ");
                sb.AppendLine(" GROUP BY  a.SEQ                                                                                         ");
                sb.AppendLine("           , a.JOB_NO                                                                                        ");
                sb.AppendLine("           , a.ITEM_CODE                                                                                     ");
                sb.AppendLine("           , a.JOB_STATE                                                                                     ");
                sb.AppendLine("           , a.NOW_OP_SEQ                                                                                    ");
                sb.AppendLine("           , a.NOW_OP_CODE                                                                                   ");
                sb.AppendLine("           , a.REMARK                                                                                        ");    
                sb.AppendLine("           , b.LOT_QTY                                                                                       ");
                sb.AppendLine("           , hist.mch_code                                                                                   ");
                sb.AppendLine(" ORDER BY  a.JOB_STATE                                                                                       ");
                 
                /*
                Console.WriteLine("*********** POP MAIN 조회 시작***************************");
                Console.WriteLine("SQL= " + sb.ToString());
                Console.WriteLine("NOW OP_CODE= " + OP_CODE.ToString());
                Console.WriteLine("NOW OP_CODE= " + MCH_CODE.ToString());
                Console.WriteLine("*********** POP MAIN 조회 종료***************************");
                */
                 
                string sSql = string.Format(sb.ToString(), OP_CODE, MCH_CODE);
                dc = new DataCommon();
                dt = dc.getTable(sSql);


                if (dt != null)
                {
                    if (dt.Rows.Count < 1)
                    {
                        ret.VALUE = 0;
                        ret.MESSAGE = "DATA가 없습니다.";
                    }
                    else
                    {
                        ret.VALUE = 1;
                    }

                    ret.DATA = dt;
                }
            }
            catch (Exception ex)
            {
                ret.VALUE = -1;
                ret.MESSAGE = ex.Message;
            }

            return ret;
        }


        /// <summary>
        /// 터미널 작업지시번호 스캔시 다른 시작한 작업지시가 있는지 조회
        /// </summary>
        /// <param name="OP_CODE">공정코드</param>
        /// <returns></returns>
        static public Result getWorkChk(string OP_CODE, string MCH_CODE, string JOB_NO)
        {
            Result ret = new Result();

            try
            {

                DataTable dt = new DataTable();

                StringBuilder sb = new StringBuilder();
                 

                //*************************************************************
                //*************************************************************
                //자기공정의 자기장비번호에서 작업한 목록만 보여주기


                sb.AppendLine(" select    a.SEQ                                                                 ");
                sb.AppendLine("         , a.job_no                                                              ");
                sb.AppendLine("         , a.item_code                                                           ");
                sb.AppendLine("         , a.job_state                                                           ");
                sb.AppendLine("         , a.now_op_seq                                                          ");
                sb.AppendLine("         , a.now_op_code                                                         ");
                sb.AppendLine("         , hist.mch_code                                                         ");
                sb.AppendLine("         , b.lot_qty                                                             ");
                sb.AppendLine("         , MAX(START_JOB.start_date) AS STA_DATE                                 ");
                sb.AppendLine("         , MAX(START_JOB.start_date) AS STA_DATE                                 ");
                sb.AppendLine("         , MAX(END_JOB.end_date) AS END_DATE                                     ");
                sb.AppendLine("         , NVL( SUM( BAD.BAD_QTY ) , 0 ) AS BAD_QTY                              ");
                sb.AppendLine("         , b.LOT_QTY - NVL( SUM( BAD.BAD_QTY ) , 0 ) AS TOT_QTY                  ");
                sb.AppendLine("         , a.REMARK                                                              ");
                sb.AppendLine(" from pop_lot_mapping a join pop_job_order b on a.job_no = b.job_no              ");
                sb.AppendLine(" inner join pop_job_hist hist on a.job_no = hist.map_no and a.now_op_code = hist.op_code                     ");
                sb.AppendLine(" left outer join ( select bad_qty, map_no from pop_prod_insp where del_yn='N') BAD on a.job_no = BAD.map_no  ");// 불량제품 수량 추출
                sb.AppendLine(" left outer join ( select crt_date as start_date, op_code, map_no from pop_job_hist where del_yn='N' and job_state ='30' ) START_JOB on a.job_no = START_JOb.map_no and START_JOB.op_code = a.now_op_code        ");// --시작시간 추출
                sb.AppendLine(" left outer join ( select nvl(mod_date, crt_date) as end_date, op_code, map_no from pop_job_hist where del_yn='N' and job_state ='50') END_JOB on END_JOB.map_no = a.job_no and END_JOB.op_code =  a.now_op_code ");//--종료시간 추출
                sb.AppendLine(" where a.del_yn='N'                                                                                          ");
                sb.AppendLine(" and   b.del_yn='N'                                                                                          ");
                // 10:생성, 20:대기, 30:시작, 40:중지, 50:완료, 90:종료        
                sb.AppendLine(" AND   a.JOB_STATE IN (  '30' , '40')                                                                        ");                
                sb.AppendLine(" and   a.now_op_code = '{0}'                                                                                  ");
                sb.AppendLine(" and   hist.mch_code = '{1}'                                                                            ");
                sb.AppendLine(" AND   a.JOB_NO !=  '{2}'                                                                                     ");
                sb.AppendLine(" GROUP BY  a.SEQ                                                                                         ");
                sb.AppendLine("           , a.JOB_NO                                                                                        ");
                sb.AppendLine("           , a.ITEM_CODE                                                                                     ");
                sb.AppendLine("           , a.JOB_STATE                                                                                     ");
                sb.AppendLine("           , a.NOW_OP_SEQ                                                                                    ");
                sb.AppendLine("           , a.NOW_OP_CODE                                                                                   ");
                sb.AppendLine("           , a.REMARK                                                                                        ");
                sb.AppendLine("           , b.LOT_QTY                                                                                       ");
                sb.AppendLine("           , hist.mch_code                                                                                   ");
                sb.AppendLine(" ORDER BY  a.JOB_STATE                                                                                       ");

                //Console.WriteLine("*********** POP MAIN 조회 시작***************************");
                //Console.WriteLine("SQL= " + sb.ToString());
                //Console.WriteLine("NOW OP_CODE= " + OP_CODE.ToString());
                //Console.WriteLine("NOW OP_CODE= " + MCH_CODE.ToString());
                //Console.WriteLine("*********** POP MAIN 조회 종료***************************");

                string sSql = string.Format(sb.ToString(), OP_CODE, MCH_CODE, JOB_NO);
                dc = new DataCommon();
                dt = dc.getTable(sSql);


                if (dt != null)
                {
                    if (dt.Rows.Count < 1)
                    {
                        ret.VALUE = 0;
                        ret.MESSAGE = "DATA가 없습니다.";
                    }
                    else
                    {
                        ret.VALUE = 1;
                    }

                    ret.DATA = dt;
                }
            }
            catch (Exception ex)
            {
                ret.VALUE = -1;
                ret.MESSAGE = ex.Message;
            }

            return ret;
        }





        /// <summary>
        /// 터미널 DETAIL 정보조회
        /// </summary>
        /// <param name="MAP_SEQ">매칭정보순번</param>
        /// <returns></returns>
        static public Result getPU0410_DETAIL(string MAP_SEQ)
        {
            Result ret = new Result();

            try
            {

                DataTable dt = new DataTable();

                StringBuilder sb = new StringBuilder();


                sb.AppendLine("SELECT                                                           ");
                sb.AppendLine("      MAP.NOW_OP_SEQ                                             ");
                sb.AppendLine("    , MAP.NOW_OP_CODE                                            ");
                sb.AppendLine("    , MAP.JOB_STATE                                              ");
                sb.AppendLine("    , JOB.JOB_TYPE                                               ");
                sb.AppendLine("    , MAP.JOB_NO                                                 ");
                sb.AppendLine("    , MAP.REAR_NO                                                ");
                sb.AppendLine("    , MAP.SERIAL_NO                                              ");
                sb.AppendLine("    , MAP.SW_BOX_NO                                              ");
                sb.AppendLine("    , MAP.BOX_NO                                                 ");
                sb.AppendLine("    , MAP.ITEM_CODE                                              ");
                sb.AppendLine("    , ITM.ITEM_NAME                                              ");
                //sb.AppendLine("    --, IRT.SEQ                                                ");
                sb.AppendLine("    , IRT.JOB_UNIT                                               ");
                sb.AppendLine("    , JOB.REMARK                                                 ");
                sb.AppendLine("    , ITM.IMAGE                                                  ");
                //sb.AppendLine("    , CASE WHEN IRT.JOB_UNIT = 'TRAY' THEN JOB.LOT_QTY ELSE 1 END  AS LOT_QTY ");
                sb.AppendLine("    , JOB.LOT_QTY                                                ");
                sb.AppendLine("    , CASE WHEN IRT.JOB_UNIT = 'TRAY' THEN JOB.LOT_QTY - NVL(    ");
                sb.AppendLine("        (                                                        ");
                sb.AppendLine("            SELECT SUM(BAD_QTY) AS BAD_QTY FROM POP_PROD_INSP    ");
                sb.AppendLine("            WHERE DEL_YN = 'N'                                   ");
                sb.AppendLine("            AND ( MAP_NO = MAP.JOB_NO                            ");
                sb.AppendLine("                OR MAP_NO = MAP.REAR_NO                          ");
                sb.AppendLine("                OR MAP_NO = MAP.SERIAL_NO                        ");
                sb.AppendLine("                OR MAP_NO = MAP.SW_BOX_NO                        ");
                sb.AppendLine("                )                                                ");
                sb.AppendLine("        ) ,0 )                                                   ");
                sb.AppendLine("      ELSE NVL(                                              ");
                sb.AppendLine("        (                                                        ");
                sb.AppendLine("            SELECT  count(1) CNT                                 ");
                sb.AppendLine("            FROM POP_PROD_IN_SWBOX                               ");
                sb.AppendLine("            WHERE 1=1                                            ");
                sb.AppendLine("                AND OP_CODE = MAP.NOW_OP_CODE                    ");
                sb.AppendLine("                AND MAP_NO  = MAP.JOB_NO                         ");
                sb.AppendLine("                and LENGTH(SW_BOX_NO) > 20                       ");
                sb.AppendLine("                and SUBSTR(SW_BOX_NO,0,2) in('95','[)')          ");
                sb.AppendLine("        ) ,0 ) END AS QTY                                        ");



                /*
                sb.AppendLine("      ELSE 1 - NVL(                                               ");
                sb.AppendLine("        (                                                         ");
                sb.AppendLine("            SELECT SUM(BAD_QTY) AS BAD_QTY FROM POP_PROD_INSP     ");
                sb.AppendLine("            WHERE DEL_YN = 'N'                                    ");
                sb.AppendLine("            AND ( MAP_NO = MAP.REAR_NO                            ");
                sb.AppendLine("                OR MAP_NO = MAP.SERIAL_NO                         ");
                sb.AppendLine("                OR MAP_NO = MAP.SW_BOX_NO                         ");
                sb.AppendLine("                )                                                 ");
                sb.AppendLine("        ) ,0 ) END  AS QTY                                        ");
                */

                sb.AppendLine("     , CASE WHEN JOB_UNIT = 'REAR' THEN MAP.REAR_NO               ");
                sb.AppendLine("       WHEN JOB_UNIT = 'SERIAL' THEN MAP.SERIAL_NO                ");
                //sb.AppendLine("       WHEN JOB_UNIT = 'SW_BOX' THEN MAP.SW_BOX_NO              ");
                sb.AppendLine("       WHEN JOB_UNIT = 'SW_BOX' THEN MAP.JOB_NO                   ");
                sb.AppendLine("       WHEN JOB_UNIT = 'TRAY' THEN MAP.JOB_NO                     ");
                sb.AppendLine("       ELSE 'JOB UNIT NOT FOUND: ' || MAP.JOB_NO  END AS MAP_NO   ");
                sb.AppendLine("     ,CHANGE_ITEM_CODE                                            ");
                sb.AppendLine("FROM POP_LOT_MAPPING MAP                                          ");
                sb.AppendLine("JOIN POP_JOB_ORDER JOB                                            ");
                sb.AppendLine("ON MAP.JOB_NO = JOB.JOB_NO                                        ");
                sb.AppendLine("LEFT JOIN POP_ITEM_MAST ITM                                       ");
                sb.AppendLine("ON MAP.ITEM_CODE = ITM.ITEM_CODE                                  ");
                sb.AppendLine("LEFT JOIN POP_ITEM_ROUTINE IRT                                    ");
                sb.AppendLine("ON MAP.ITEM_CODE = IRT.ITEM_CODE                                  ");
                sb.AppendLine("AND MAP.NOW_OP_SEQ = IRT.SEQ                                      ");
                sb.AppendLine("WHERE MAP.DEL_YN = 'N'                                            ");
                sb.AppendLine("AND JOB.DEL_YN ='N'                                               ");
                sb.AppendLine("AND MAP.SEQ = '{0}'                                               ");

                /*
                Console.WriteLine("*********** POP 상세 조회 시작***************************");
                Console.WriteLine("SQL= " + sb.ToString());
                Console.WriteLine("MAP_SEQ= " + MAP_SEQ.ToString());
                Console.WriteLine("*********** POP 상세 조회 종료***************************");
                 */
                 

                string sSql = string.Format(sb.ToString(), MAP_SEQ );
                dc = new DataCommon();
                dt = dc.getTable(sSql);


                if (dt != null)
                {
                    if (dt.Rows.Count < 1)
                    {
                        ret.VALUE = 0;
                        ret.MESSAGE = "DATA가 없습니다.";
                    }
                    else
                    {
                        ret.VALUE = 1;
                    }

                    ret.DATA = dt;
                }
            }
            catch (Exception ex)
            {
                ret.VALUE = -1;
                ret.MESSAGE = ex.Message;
            }

            return ret;
        }
                



        /// <summary>
        /// 터미널 DETAIL 자재정보 조회
        /// </summary>
        /// <param name="MAP_NO">조회번호</param>
        /// <returns></returns>
        static public Result getPU0410_MATERIAL(string MAP_NO, string  OP_CODE)
        {
            Result ret = new Result();

            try
            {

                DataTable dt = new DataTable();

                StringBuilder sb = new StringBuilder();


                sb.AppendLine("SELECT                               ");
                sb.AppendLine("     BOM.MAT_CODE                    ");
                sb.AppendLine("    ,MAT.SEQ AS MAT_SEQ              ");
                sb.AppendLine("    ,DECODE(MAT.LOT,'', '', MAT.LOT || '-' || MAT.MAT_CODE)   LOT      ");
                sb.AppendLine("    ,BOM.SEQ                         ");
                sb.AppendLine("    ,BOM.OP_CODE                     ");
                sb.AppendLine("    ,MAT.MAP_NO                      ");
                sb.AppendLine("    ,MAT.INPUT_YN                    ");
                sb.AppendLine("    ,MAT.TMP_BARCODE                 ");
                sb.AppendLine("    ,MAC.MAT_NAME                    ");
                sb.AppendLine("FROM POP_ITEM_BOM BOM                ");
                sb.AppendLine("JOIN POP_LOT_MAPPING MAP             ");
                sb.AppendLine("ON BOM.ITEM_CODE = MAP.ITEM_CODE     ");
                sb.AppendLine("LEFT JOIN POP_PROD_IN_MAT MAT        ");
                sb.AppendLine("ON ( MAT.MAP_NO = MAP.JOB_NO         ");
                sb.AppendLine("    OR MAT.MAP_NO = MAP.REAR_NO      ");
                sb.AppendLine("    OR MAT.MAP_NO = MAP.SERIAL_NO    ");
                sb.AppendLine("    OR MAT.MAP_NO = MAP.SW_BOX_NO    ");
                sb.AppendLine("    )                                ");
                sb.AppendLine("AND MAT.DEL_YN ='N'                  ");
                sb.AppendLine("AND BOM.MAT_CODE = MAT.MAT_CODE      ");                
                sb.AppendLine("LEFT JOIN POP_MATERIAL MAC           ");
                sb.AppendLine("ON BOM.MAT_CODE = MAC.MAT_CODE       ");
                sb.AppendLine("WHERE BOM.DEL_YN = 'N'               ");
                sb.AppendLine("AND MAP.DEL_YN = 'N'                 ");
                //sb.AppendLine("AND MAT.MAP_NO = '{0}'               ");
                sb.AppendLine("AND BOM.OP_CODE ='{0}'               ");
                sb.AppendLine("AND ( JOB_NO = '{1}'                 ");
                sb.AppendLine("    OR REAR_NO = '{1}'               ");
                sb.AppendLine("    OR SERIAL_NO = '{1}'             ");
                sb.AppendLine("    OR SW_BOX_NO = '{1}'             ");
                sb.AppendLine("    )                                ");
                sb.AppendLine("ORDER BY BOM.SEQ , BOM.MAT_CODE      ");

             
                Console.WriteLine("*********** POP 자재 조회 시작***************************");
                Console.WriteLine("SQL= " + sb.ToString());
                Console.WriteLine("OP_CODE= " + OP_CODE.ToString());
                Console.WriteLine("MAP_NO= " + MAP_NO.ToString());
                Console.WriteLine("*********** POP 자재 조회 종료***************************");
                

                string sSql = string.Format(sb.ToString(), OP_CODE, MAP_NO );
                dc = new DataCommon();
                dt = dc.getTable(sSql);


                if (dt != null)
                {
                    if (dt.Rows.Count < 1)
                    {
                        ret.VALUE = 0;
                        ret.MESSAGE = "DATA가 없습니다.";
                    }
                    else
                    {
                        ret.VALUE = 1;
                    }

                    ret.DATA = dt;
                }
            }
            catch (Exception ex)
            {
                ret.VALUE = -1;
                ret.MESSAGE = ex.Message;
            }

            return ret;
        }
        


        /// <summary>
        /// 터미널 DETAIL 불량정보 조회
        /// </summary>
        /// <param name="OP_CODE">공정코드</param>
        /// <param name="MAP_NO">조회번호</param>
        /// <returns></returns>
        static public Result getPU0410_BAD(string OP_CODE , string MAP_NO)
        {
            Result ret = new Result();

            try
            {

                DataTable dt = new DataTable();

                StringBuilder sb = new StringBuilder();
                                                                   
                sb.AppendLine("SELECT                                        ");
                sb.AppendLine("    OP_BAD.BAD_CODE                           ");
                sb.AppendLine("    , BAD_INFO.BAD_NAME                       ");
                //sb.AppendLine("    , SUM( NVL( BAD.BAD_QTY,0) ) AS BAD_QTY   ");
                sb.AppendLine("    , NVL( BAD.BAD_QTY,0)  AS BAD_QTY         ");
                sb.AppendLine("    , 0  AS ADD_QTY                           ");
                sb.AppendLine("    , BAD.SEQ                                 ");
                sb.AppendLine("    , '+' V_ADD                                  ");
                sb.AppendLine("    , '_' V_DEL                                ");
                sb.AppendLine("FROM POP_OP_BAD OP_BAD                        ");
                sb.AppendLine("JOIN POP_BAD_CODE BAD_INFO                    ");
                sb.AppendLine("ON OP_BAD.BAD_CODE = BAD_INFO.BAD_CODE        ");
                sb.AppendLine("LEFT JOIN POP_PROD_INSP BAD                   ");
                sb.AppendLine("ON OP_BAD.OP_CODE = BAD.OP_CODE               ");
                sb.AppendLine("AND OP_BAD.BAD_CODE = BAD.BAD_CODE            ");
                sb.AppendLine("AND BAD.DEL_YN ='N'                           ");
                sb.AppendLine("AND BAD.MAP_NO = '{1}'                        ");
                sb.AppendLine("WHERE OP_BAD.DEL_YN = 'N'                     ");
                sb.AppendLine("AND BAD_INFO.DEL_YN = 'N'                     ");
                sb.AppendLine("AND OP_BAD.OP_CODE = '{0}'                    ");
                //sb.AppendLine("GROUP BY OP_BAD.BAD_CODE                      ");
                //sb.AppendLine("       , BAD_INFO.BAD_NAME                    ");

                
                //Console.WriteLine("*********** POP 불량정보 조회 시작***************************");
                //Console.WriteLine("SQL= " + sb.ToString());
                //Console.WriteLine("OP_CODE= " + OP_CODE.ToString());
                //Console.WriteLine("MAP_NO= " + MAP_NO.ToString());
                //Console.WriteLine("*********** POP 불량정보 조회 종료***************************");
                

                string sSql = string.Format(sb.ToString(), OP_CODE , MAP_NO );
                dc = new DataCommon();
                dt = dc.getTable(sSql);


                if (dt != null)
                {
                    if (dt.Rows.Count < 1)
                    {
                        ret.VALUE = 0;
                        ret.MESSAGE = "DATA가 없습니다.";
                    }
                    else
                    {
                        ret.VALUE = 1;
                    }

                    ret.DATA = dt;
                }
            }
            catch (Exception ex)
            {
                ret.VALUE = -1;
                ret.MESSAGE = ex.Message;
            }

            return ret;
        }
        
               

        /// <summary>
        /// 터미널 DETAIL 공정순서 조회
        /// </summary>
        /// <param name="ITEM_CODE">품번</param>
        /// <returns></returns>
        static public Result getPU0410_ROUTING(string ITEM_CODE)
        {
            Result ret = new Result();

            try
            {

                DataTable dt = new DataTable();

                StringBuilder sb = new StringBuilder();


                sb.AppendLine("SELECT                                ");
                sb.AppendLine("    IRT.SEQ                           ");
                sb.AppendLine("  , IRT.OP_CODE                       ");
                sb.AppendLine("  , OPC.OP_NAME                       ");
                sb.AppendLine("  , ITM.ITEM_NAME                     ");
                sb.AppendLine("FROM POP_ITEM_ROUTINE IRT             ");
                sb.AppendLine("LEFT JOIN POP_ITEM_MAST ITM           ");
                sb.AppendLine("ON IRT.ITEM_CODE = ITM.ITEM_CODE      ");                
                sb.AppendLine("LEFT JOIN POP_OP_CODE OPC             ");
                sb.AppendLine("ON IRT.OP_CODE = OPC.OP_CODE          ");
                sb.AppendLine("WHERE IRT.DEL_YN = 'N'                ");
                sb.AppendLine("AND IRT.ITEM_CODE = '{0}'             ");
                sb.AppendLine("ORDER BY IRT.SORT_NUM, IRT.OP_CODE            ");

                /*
                Console.WriteLine("*********** POP 라우팅 조회 시작***************************");
                Console.WriteLine("SQL= " + sb.ToString());
                Console.WriteLine("ITEM_CODE= " + ITEM_CODE.ToString());
                Console.WriteLine("*********** POP 라우팅 조회 종료***************************");
                */                             
                                                                     
                string sSql = string.Format(sb.ToString(), ITEM_CODE );
                dc = new DataCommon();
                dt = dc.getTable(sSql);


                if (dt != null)
                {
                    if (dt.Rows.Count < 1)
                    {
                        ret.VALUE = 0;
                        ret.MESSAGE = "DATA가 없습니다.";
                    }
                    else
                    {
                        ret.VALUE = 1;
                    }

                    ret.DATA = dt;
                }
            }
            catch (Exception ex)
            {
                ret.VALUE = -1;
                ret.MESSAGE = ex.Message;
            }

            return ret;
        }
        


        /// <summary>
        /// 터미널 DETAIL 작업이력 조회
        /// </summary>
        /// <param name="MAP_NO">조회번호</param>
        /// <returns></returns>
        static public Result getPU0410_HISTORY(string MAP_NO)
        {
            Result ret = new Result();

            try
            {

                DataTable dt = new DataTable();

                StringBuilder sb = new StringBuilder();
                sb.AppendLine("     SELECT                                                ");
                sb.AppendLine("           HIS.MAP_NO                                      ");
                sb.AppendLine("         , HIS.OP_CODE                                     ");
                sb.AppendLine("         , OPC.OP_NAME                                     ");
                sb.AppendLine("         , HIS.MCH_CODE                                    ");
                sb.AppendLine("         , MCH.MCH_NAME                                    ");
                sb.AppendLine("         , HIS.JOB_STATE                                   ");
                //sb.AppendLine("       , JST.NAME                                        ");
                sb.AppendLine("         , HIS.CHANGE_ITEM_CODE                            ");
                sb.AppendLine("         , HIS.REMARK                                      ");
                //sb.AppendLine("       , NVL( HIS.MOD_EMP , HIS.CRT_EMP ) AS EMP         ");
                sb.AppendLine("         , EMP.NAME AS EMP                                 ");
                sb.AppendLine("         , to_char(max(NVL( HIS.MOD_DATE , HIS.CRT_DATE)),'yyyy-mm-dd hh24:mi:ss') AS WORK_DATE  ");
                sb.AppendLine("     FROM POP_JOB_HIST HIS                                 ");
                sb.AppendLine("     LEFT JOIN POP_OP_CODE OPC                             ");
                sb.AppendLine("     ON HIS.OP_CODE = OPC.OP_CODE                          ");
                sb.AppendLine("     LEFT JOIN POP_MACHINE  MCH                            ");
                sb.AppendLine("     ON HIS.MCH_CODE = MCH.MCH_CODE                        ");
                sb.AppendLine("     JOIN POP_LOT_MAPPING MAP                              ");
                sb.AppendLine("     ON ( MAP.JOB_NO = HIS.MAP_NO                          ");
                sb.AppendLine("          OR MAP.REAR_NO = HIS.MAP_NO                      ");
                sb.AppendLine("          OR MAP.SERIAL_NO = HIS.MAP_NO                    ");
                sb.AppendLine("          OR MAP.SW_BOX_NO = HIS.MAP_NO                    ");
                sb.AppendLine("          OR MAP.BOX_NO = HIS.MAP_NO )                     ");
                //sb.AppendLine("   LEFT JOIN SYS_CODE JST                                ");
                //sb.AppendLine("   ON TYPE = 'JOB_STATE'                                 ");
                //sb.AppendLine("   AND HIS.JOB_STATE = JST.CODE                          ");                
                sb.AppendLine("     LEFT JOIN SYS_EMP EMP                                 ");
                sb.AppendLine("     ON EMP.ID = NVL( HIS.MOD_EMP , HIS.CRT_EMP )          ");
                sb.AppendLine("     WHERE HIS.DEL_YN = 'N'                                ");
                //sb.AppendLine("   AND HIS.MAP_NO = '{0}'                                ");
                
                sb.AppendLine("     AND ( MAP.JOB_NO = '{0}'                        ");                    
                sb.AppendLine("     OR MAP.REAR_NO = '{0}'                          ");
                sb.AppendLine("     OR MAP.SERIAL_NO = '{0}'                        ");
                sb.AppendLine("     OR MAP.SW_BOX_NO = '{0}'                        ");
                sb.AppendLine("     OR MAP.BOX_NO = '{0}' )                         ");                
                sb.AppendLine("     group by                                        ");
                sb.AppendLine("     HIS.MAP_NO                                      ");
                sb.AppendLine("     , HIS.OP_CODE                                   ");
                sb.AppendLine("     , OPC.OP_NAME                                   ");
                sb.AppendLine("     , HIS.MCH_CODE                                  ");
                sb.AppendLine("     , MCH.MCH_NAME                                  ");
                sb.AppendLine("     , HIS.JOB_STATE                                 ");
                sb.AppendLine("     , HIS.CHANGE_ITEM_CODE                          ");
                sb.AppendLine("     , HIS.REMARK                                    ");
                sb.AppendLine("     , EMP.NAME                                      ");
                sb.AppendLine("order by work_date desc, job_state                   ");

                /*
                Console.WriteLine("*********** POP 이력정보 조회 시작***************************");
                Console.WriteLine("SQL= " + sb.ToString()); 
                Console.WriteLine("MAP_NO= " + MAP_NO.ToString());
                Console.WriteLine("********** POP 이력정보 조회 종료***************************");
                */
                 

                string sSql = string.Format(sb.ToString(), MAP_NO );
                dc = new DataCommon();
                dt = dc.getTable(sSql);


                if (dt != null)
                {
                    if (dt.Rows.Count < 1)
                    {
                        ret.VALUE = 0;
                        ret.MESSAGE = "이력정보가 없습니다.";
                    }
                    else
                    {
                        ret.VALUE = 1;
                    }

                    ret.DATA = dt;
                }
            }
            catch (Exception ex)
            {
                ret.VALUE = -1;
                ret.MESSAGE = ex.Message;
            }

            return ret;
        }



        /// <summary>
        /// 터미널 DETAIL LOCK정보 조회
        /// </summary>
        /// <param name="MAP_NO">조회번호</param>
        /// <returns></returns>
        static public Result getPU0410_LOCK(string MAP_NO)
        {
            Result ret = new Result();

            try
            {

                DataTable dt = new DataTable();

                StringBuilder sb = new StringBuilder();

                sb.AppendLine("SELECT                                           ");
                sb.AppendLine("      LCK.SEQ                                    ");
                sb.AppendLine("    , LCK.LOCK_CODE                              ");
                sb.AppendLine("    , LCK.LOCK_TYPE                              ");
                sb.AppendLine("    , LCK.DETAIL_CODE                            ");
                sb.AppendLine("    , LCK.LOCK_COMMENT                           ");
                sb.AppendLine("    , LCK.CRT_DATE                               ");
                sb.AppendLine("    , LCK.CRT_EMP                                ");
                sb.AppendLine("    , EMP.NAME                                   ");
                sb.AppendLine("FROM SYS_LOCK_MNG LCK                            ");
                sb.AppendLine("JOIN POP_PROD_IN_MAT MAT                         ");
                sb.AppendLine("ON MAT.DEL_YN = 'N'                              ");
                sb.AppendLine("AND LCK.LOCK_TYPE = 'M'                          ");
                sb.AppendLine("AND LCK.DETAIL_CODE = MAT.MAT_CODE || MAT.LOT    ");
                sb.AppendLine("JOIN POP_LOT_MAPPING MAP                         ");
                sb.AppendLine("ON MAP.DEL_YN = 'N'                              ");
                sb.AppendLine("AND ( MAT.MAP_NO = MAP.JOB_NO                    ");
                sb.AppendLine("    OR MAT.MAP_NO = MAP.REAR_NO                  ");
                sb.AppendLine("    OR MAT.MAP_NO = MAP.SERIAL_NO                ");
                sb.AppendLine("    OR MAT.MAP_NO = MAP.SW_BOX_NO                ");
                sb.AppendLine("    )                                            ");
                sb.AppendLine("LEFT JOIN SYS_EMP EMP                            ");
                sb.AppendLine("ON LCK.CRT_EMP = EMP.ID                          ");
                sb.AppendLine("WHERE LCK.DEL_YN = 'N'                           ");
                sb.AppendLine("AND LCK.LOCK_YN = 'Y'                            ");
                sb.AppendLine("AND ( MAP.JOB_NO = '{0}'                         ");
                sb.AppendLine("    OR MAP.REAR_NO = '{0}'                       ");
                sb.AppendLine("    OR MAP.SERIAL_NO = '{0}'                     ");
                sb.AppendLine("    OR MAP.SW_BOX_NO = '{0}'                     ");
                sb.AppendLine("    )                                            ");
                sb.AppendLine("UNION                                            ");
                sb.AppendLine("SELECT                                           ");
                sb.AppendLine("      LCK.SEQ                                    ");
                sb.AppendLine("    , LCK.LOCK_CODE                              ");
                sb.AppendLine("    , LCK.LOCK_TYPE                              ");
                sb.AppendLine("    , LCK.DETAIL_CODE                            ");
                sb.AppendLine("    , LCK.LOCK_COMMENT                           ");
                sb.AppendLine("    , LCK.CRT_DATE                               ");
                sb.AppendLine("    , LCK.CRT_EMP                                ");
                sb.AppendLine("    , EMP.NAME                                   ");
                sb.AppendLine("FROM SYS_LOCK_MNG LCK                            ");
                sb.AppendLine("JOIN POP_LOT_MAPPING MAP                         ");
                sb.AppendLine("ON MAP.DEL_YN = 'N'                              ");
                sb.AppendLine("AND LCK.LOCK_TYPE = 'W'                          ");
                sb.AppendLine("AND ( LCK.DETAIL_CODE = MAP.JOB_NO               ");
                sb.AppendLine("    OR LCK.DETAIL_CODE = MAP.REAR_NO             ");
                sb.AppendLine("    OR LCK.DETAIL_CODE = MAP.SERIAL_NO           ");
                sb.AppendLine("    OR LCK.DETAIL_CODE = MAP.SW_BOX_NO           ");
                sb.AppendLine("    )                                            ");
                sb.AppendLine("LEFT JOIN SYS_EMP EMP                            ");
                sb.AppendLine("ON LCK.CRT_EMP = EMP.ID                          ");
                sb.AppendLine("WHERE LCK.DEL_YN = 'N'                           ");
                sb.AppendLine("AND LCK.LOCK_YN = 'Y'                            ");
                sb.AppendLine("AND ( MAP.JOB_NO = '{0}'                         ");
                sb.AppendLine("    OR MAP.REAR_NO = '{0}'                       ");
                sb.AppendLine("    OR MAP.SERIAL_NO = '{0}'                     ");
                sb.AppendLine("    OR MAP.SW_BOX_NO = '{0}'                     ");
                sb.AppendLine("    )                                            ");

                /*
                Console.WriteLine("DETAIL LOCK정보 조회 시작 *********************************");
                Console.WriteLine("sb.ToString() = " + sb.ToString());
                Console.WriteLine("MAP_NO = " + MAP_NO);
                Console.WriteLine("DETAIL LOCK정보 조회 종료 *********************************");
                */
                string sSql = string.Format(sb.ToString(), MAP_NO );
                dc = new DataCommon();
                dt = dc.getTable(sSql);


                if (dt != null)
                {
                    //if (dt.Rows.Count < 1)
                    //{
                    //    ret.VALUE = 0;
                    //    ret.MESSAGE = "DATA가 없습니다.";
                    //}
                    //else
                    //{
                        ret.VALUE = 1;
                    //}

                    ret.DATA = dt;
                }
            }
            catch (Exception ex)
            {
                ret.VALUE = -1;
                ret.MESSAGE = ex.Message;
            }

            return ret;
        }


        #endregion 


        # region < SET >


        /// <summary>
        /// 터미널 DETAIL 작업시작 & 중지
        /// </summary>
        /// <param name="MAP_SEQ">조회순번</param>
        /// <param name="MAP_NO">조회번호</param>
        /// <param name="OP_CODE">공정코드</param>
        /// <param name="MCH_CODE">설비코드 (생략가능)</param>
        /// <param name="JOB_STATE">10 생성 , 20대기 , 30시작 , 40증지 , 50완료 , 90종료 </param>
        /// <param name="REMARK">특이사항</param>
        /// <returns></returns>
        static public Result setPU0410_WorkStartStop(string MAP_SEQ, string MAP_NO, string OP_CODE , string MCH_CODE , string JOB_STATE , string CHANGE_ITEM_CODE,string REMARK = "" )
        {
            Result ret = new Result();

            try
            {

                List<string> lsSql = new List<string>();

                lsSql.Add(queryUpdateJobState(MAP_SEQ, JOB_STATE, CHANGE_ITEM_CODE));
                lsSql.Add(queryInsertJobHist(MAP_NO, OP_CODE, MCH_CODE, JOB_STATE, CHANGE_ITEM_CODE, REMARK));
                dc = new DataCommon();
                int iRet = dc.execNonQuery(lsSql);

                if ( iRet  != 2  )
                {
                    ret.VALUE = -1;
                    ret.MESSAGE = "setPU0410_WorkStart의 쿼리실행수에 문제가 있습니다.";
                    return ret;
                }

                ret.VALUE = 1;
                
            }
            catch (Exception ex)
            {
                ret.VALUE = -1;
                ret.MESSAGE = ex.Message;
            }

            return ret;
        }




        /// <summary>
        /// 터미널 DETAIL 작업완료
        /// </summary>
        /// <param name="ITEM_CODE">품번</param>
        /// <param name="NOW_OP_SEQ">현재공정순번</param>
        /// <param name="MAP_SEQ">조회순번</param>
        /// <param name="MAP_NO">조회번호</param>
        /// <param name="OP_CODE">공정코드</param>
        /// <param name="MCH_CODE">설비코드 (생략가능)</param>        
        /// <param name="REMARK">특이사항</param>
        /// <returns></returns>
        static public Result setPU0410_WorkComplete(string ITEM_CODE, string NOW_OP_SEQ, string MAP_SEQ, string MAP_NO, string OP_CODE, string MCH_CODE, string CHANGE_ITEM_CODE, string REMARK = "")
        {
            Result ret = new Result();

            try
            {
                //Result retNextOp = new Result();
                StringBuilder sbSql = new StringBuilder();

                sbSql.AppendLine("SELECT * FROM (                                               ");
                sbSql.AppendLine("  SELECT                                                        ");
                sbSql.AppendLine("     SEQ                                                      ");
                sbSql.AppendLine("    ,OP_CODE                                                  ");
                sbSql.AppendLine("    ,LEAD( SEQ ) OVER ( ORDER BY SORT_NUM ) AS NEXT_OP_SEQ         ");
                sbSql.AppendLine("    ,LEAD( OP_CODE ) OVER ( ORDER BY SORT_NUM ) AS NEXT_OP_CODE    ");
                sbSql.AppendLine("  FROM POP_ITEM_ROUTINE IRT                                     ");
                sbSql.AppendLine("  WHERE IRT.DEL_YN ='N'                                         ");
                sbSql.AppendLine("  AND IRT.ITEM_CODE = '{0}'                                     ");
                sbSql.AppendLine(") A                                                           ");
                sbSql.AppendLine("WHERE SEQ = '{1}'                                             ");
                 
                Console.WriteLine("순번 = " + sbSql.ToString());
                Console.WriteLine("ITEM_CODE = " + ITEM_CODE);
                Console.WriteLine("NOW_OP_SEQ = " + NOW_OP_SEQ);
                  
                string sSql = string.Format(sbSql.ToString(), ITEM_CODE, NOW_OP_SEQ );
                dc = new DataCommon();
                DataTable dtNextOp = dc.getTable(sSql);

                /*
                Console.WriteLine("dtNextOp.Rows.Count = " + dtNextOp.Rows.Count);
                Console.WriteLine("dtNextOp.Rows[0][NEXT_OP_SEQ] = " + dtNextOp.Rows[0]["NEXT_OP_SEQ"]);
                Console.WriteLine("dtNextOp.Rows[0][NEXT_OP_CODE] = " + dtNextOp.Rows[0]["NEXT_OP_CODE"]);                                
                Console.WriteLine("MAP_SEQ = " + MAP_SEQ);
                Console.WriteLine("MAP_NO = " + MAP_NO);
                */

                List<string> lsSql = new List<string>();
                if (dtNextOp.Rows.Count < 1 || dtNextOp.Rows[0]["NEXT_OP_SEQ"] == System.DBNull.Value)
                {
                    lsSql.Add(queryUpdateJobState(MAP_SEQ, "90", CHANGE_ITEM_CODE));  //다음공정이 없으므로 종료.
                }
                else
                {
                    string sNexpOpSeq = dtNextOp.Rows[0]["NEXT_OP_SEQ"].ToString();
                    string sNexpOpCode = dtNextOp.Rows[0]["NEXT_OP_CODE"].ToString();
                    lsSql.Add(queryUpdateJobState(MAP_SEQ, "20", CHANGE_ITEM_CODE, sNexpOpSeq, sNexpOpCode)); //다음공정 대기   
                }

                lsSql.Add(queryInsertJobHist(MAP_NO, OP_CODE, MCH_CODE, "50", CHANGE_ITEM_CODE, REMARK)); //기록에는 완료로 처리
                dc = new DataCommon();
                int iRet = dc.execNonQuery(lsSql);

                if (iRet != 2)
                {
                    ret.VALUE = -1;
                    ret.MESSAGE = "setPU0410_Workcomplete의 쿼리실행수에 문제가 있습니다.";
                    return ret;
                }

                ret.VALUE = 1;

            }
            catch (Exception ex)
            {
                ret.VALUE = -1;
                ret.MESSAGE = ex.Message;
            }

            return ret;
        }

        

        /// <summary>
        /// 터미널 DETAIL 자재등록
        /// </summary>
        /// <param name="MAP_SEQ">조회순번</param>
        /// <param name="MAP_NO">조회번호</param>
        /// <param name="OP_CODE">공정코드</param>
        /// <param name="MAT_CODE">자재코드</param>        
        /// <param name="LOT">자재LOT</param>
        /// <param name="INPUT_YN">자재사용여부 (REWORK전용)</param>
        /// <returns></returns>
        static public Result setPU0410_Material(string MAT_SEQ, string MAP_NO, string OP_CODE, string MAT_CODE, string LOT, string sTmpBarCode, string INPUT_YN = "")
        {
            Result ret = new Result();

            try
            {
                //순번이 없는 경우 (insert인경우) 해당하는 LOT가 있는지 체크하여 있다면 PASS (REWORK가 아닌경우)
                if (string.IsNullOrEmpty(MAT_SEQ) && string.IsNullOrEmpty(INPUT_YN))
                {
                    MAT_SEQ = CheckMaterialExistYN(MAP_NO, OP_CODE, MAT_CODE, LOT);
                    if (!string.IsNullOrEmpty(MAT_SEQ))
                    {
                        ret.VALUE = 1;
                        return ret;
                    }
                }

                StringBuilder sbSql = new StringBuilder();
                string sSql = string.Empty;

                if (string.IsNullOrEmpty(MAT_SEQ))
                {
                    sbSql.AppendLine("INSERT INTO POP_PROD_IN_MAT                                   ");
                    sbSql.AppendLine("(                                                             ");
                    sbSql.AppendLine("     SEQ                                                      ");
                    sbSql.AppendLine("    ,MAP_NO                                                   ");
                    sbSql.AppendLine("    ,OP_CODE                                                  ");
                    sbSql.AppendLine("    ,MAT_CODE                                                 ");
                    sbSql.AppendLine("    ,LOT                                                      ");
                    sbSql.AppendLine("    ,INPUT_YN                                                 ");
                    sbSql.AppendLine("    ,CRT_DATE                                                 ");
                    sbSql.AppendLine("    ,CRT_EMP                                                  ");
                    sbSql.AppendLine("    ,DEL_YN                                                   ");
                    sbSql.AppendLine("    ,TMP_BARCODE                                              ");
                    sbSql.AppendLine(") VALUES (                                                    ");
                    sbSql.AppendLine("    ( SELECT NVL(  MAX(SEQ)+1 , 1  ) FROM POP_PROD_IN_MAT  )  ");
                    sbSql.AppendLine("    , '{0}'                                                   ");
                    sbSql.AppendLine("    , '{1}'                                                   ");
                    sbSql.AppendLine("    , '{2}'                                                   ");
                    sbSql.AppendLine("    , '{3}'                                                   ");
                    sbSql.AppendLine("    , {4}                                                     ");
                    sbSql.AppendLine("    , SYSDATE                                                 ");
                    sbSql.AppendLine("    , '{5}'                                                   ");
                    sbSql.AppendLine("    , 'N'                                                     ");
                    sbSql.AppendLine("    , '{6}'                                                   ");
                    sbSql.AppendLine(")                                                             ");

                    sSql = string.Format(sbSql.ToString(), MAP_NO, OP_CODE, MAT_CODE, LOT, (!string.IsNullOrEmpty(INPUT_YN) ? string.Format("'{0}'", INPUT_YN) : "NULL"), Para.USER.ID, sTmpBarCode);
                }
                else
                { 

                    sbSql.AppendLine("UPDATE POP_PROD_IN_MAT    ");
                    sbSql.AppendLine("SET LOT = '{1}'           ");
                    if (!string.IsNullOrEmpty(INPUT_YN))
                    {
                        sbSql.AppendLine(", INPUT_YN = '{2}'         ");
                    }
                    sbSql.AppendLine(", DEL_YN ='N'             ");
                    sbSql.AppendLine(", MOD_EMP = '{3}'         ");
                    sbSql.AppendLine(", MOD_DATE = SYSDATE      ");
                    sbSql.AppendLine("WHERE SEQ = '{0}'         ");

                    sSql = string.Format(sbSql.ToString(), MAT_SEQ, LOT, INPUT_YN, Para.USER.ID);
                }

                /*
                Console.WriteLine("*********** POP_PROD_IN_MAT 스캔한 자재내역 INSERT (수정) 시작***************************");
                Console.WriteLine("SQL= " + sbSql.ToString());
                Console.WriteLine("MAT_SEQ= " + MAT_SEQ.ToString());
                Console.WriteLine("INPUT_YN= " + INPUT_YN.ToString());
                Console.WriteLine("Para.USER.ID= " + Para.USER.ID); 
                Console.WriteLine("*********** POP_PROD_IN_MAT 스캔한 자재내역 INSERT (수정) 종료***************************");
                 * */
                

                dc = new DataCommon();
                int iRet = dc.execNonQuery(sSql);

                if (iRet != 1)
                {
                    ret.VALUE = -1;
                    ret.MESSAGE = "setPU0410_Material의 쿼리실행수에 문제가 있습니다.";
                    return ret;
                }

                ret.VALUE = 1;

            }
            catch (Exception ex)
            {
                ret.VALUE = -1;
                ret.MESSAGE = ex.Message;
            }

            return ret;
        }




        /// <summary>
        /// 터미널 DETAIL 제품라벨등록
        /// </summary>
        /// <param name="MAP_SEQ">조회순번</param>
        /// <param name="MAP_NO">조회번호</param>
        /// <param name="OP_CODE">공정코드</param>
        /// <param name="MAT_CODE">자재코드</param>        
        /// <param name="LOT">자재LOT</param>
        /// <param name="INPUT_YN">자재사용여부 (REWORK전용)</param>
        /// <returns></returns>
        static public Result setPU0410_PROD(  string MAP_NO, string OP_CODE, string PROD_CODE  )
        {
            Result ret = new Result();

            try
            {

                //중복제거
                string sSql = "";
                sSql += "SELECT  COUNT(1) CNT                     ";
                sSql += "FROM POP_PROD_IN_SWBOX                   ";
                sSql += "WHERE 1=1                                ";
                sSql += "AND OP_CODE ='" + OP_CODE + "'           ";
                sSql += "AND MAP_NO = '" + MAP_NO + "'            ";
                sSql += "and SW_BOX_NO ='" + PROD_CODE + "'       ";
                Console.WriteLine("*********** POP_PROD_IN_SWBOX 스캔한 자재내역 중복확인***************************");
                Console.WriteLine("sSql= " + sSql.ToString());
                Console.WriteLine("MAP_NO   = " + MAP_NO.ToString());
                Console.WriteLine("OP_CODE  = " + OP_CODE.ToString());
                Console.WriteLine("PROD_CODE  = " + PROD_CODE.ToString());

                dc = new DataCommon();
                String getRegCnt = dc.getSimpleScalar(sSql).ToString();

                Console.WriteLine("getRegCnt  = " + getRegCnt);
                Console.WriteLine("*********** POP_PROD_IN_MAT 스캔한 자재내역 중복확인종료***************************");


                if (Convert.ToInt32(getRegCnt) > 0)
                {
                    ret.VALUE = 1;
                }
                else
                {


                    StringBuilder sbSql = new StringBuilder();
                    string sSql2 = string.Empty;

                    sbSql.AppendLine("INSERT INTO POP_PROD_IN_SWBOX                                 ");
                    sbSql.AppendLine("(                                                             ");
                    sbSql.AppendLine("     SEQ                                                      ");
                    sbSql.AppendLine("    ,MAP_NO                                                   ");
                    sbSql.AppendLine("    ,OP_CODE                                                  ");
                    sbSql.AppendLine("    ,SW_BOX_NO                                                 ");
                    sbSql.AppendLine("    ,LOT                                                      ");
                    sbSql.AppendLine("    ,INPUT_YN                                                 ");
                    sbSql.AppendLine("    ,CRT_DATE                                                 ");
                    sbSql.AppendLine("    ,CRT_EMP                                                  ");
                    sbSql.AppendLine("    ,DEL_YN                                                   ");
                    sbSql.AppendLine(") VALUES (                                                    ");
                    sbSql.AppendLine("    ( SELECT NVL(  MAX(SEQ)+1 , 1  ) FROM POP_PROD_IN_SWBOX)  ");
                    sbSql.AppendLine("    , '{0}'                                                   ");
                    sbSql.AppendLine("    , '{1}'                                                   ");
                    sbSql.AppendLine("    , '{2}'                                                   ");
                    sbSql.AppendLine("    , ''                                                   ");
                    sbSql.AppendLine("    , ''                                                      ");
                    sbSql.AppendLine("    , SYSDATE                                                 ");
                    sbSql.AppendLine("    , '{3}'                                                   ");
                    sbSql.AppendLine("    , 'N'                                                     ");
                    sbSql.AppendLine(")                                                             ");

                    sSql2 = string.Format(sbSql.ToString(), MAP_NO, OP_CODE, PROD_CODE, Para.USER.ID);

                    /*
                    Console.WriteLine("*********** POP_PROD_IN_SWBOX 스캔한 자재내역 INSERT 시작***************************");
                    Console.WriteLine("SQL= " + sbSql.ToString());
                    Console.WriteLine("MAP_NO   = " + MAP_NO.ToString());
                    Console.WriteLine("OP_CODE  = " + OP_CODE.ToString());
                    Console.WriteLine("PROD_CODE  = " + PROD_CODE.ToString());
                    Console.WriteLine("Para.USER.ID  = " + Para.USER.ID);  
                    Console.WriteLine("*********** POP_PROD_IN_MAT 스캔한 자재내역 INSERT 종료***************************");
                    */

                    dc = new DataCommon();
                    int iRet = dc.execNonQuery(sSql2);

                    if (iRet != 1)
                    {
                        ret.VALUE = -1;
                        //ret.MESSAGE = "setPU0410_PROD의 쿼리실행수에 문제가 있습니다.";
                        ret.MESSAGE = "제품의 라벨정보 등록시 문제가 발생하였습니다.(ERR-1000)";
                        return ret;
                    }

                    ret.VALUE = 1;
                }
            }
            catch  
            {
                ret.VALUE = -1;
                //ret.MESSAGE = ex.Message;
                ret.MESSAGE = "제품의 라벨정보 등록시 문제가 발생하였습니다.(ERR-2000)";
            }

            return ret;
        }
         


        /// <summary>
        /// 터미널 DETAIL 제품정보정보 조회
        /// </summary>
        /// <param name="MAP_NO">조회번호</param>
        /// <returns></returns>
        static public Result getPU0410_PROD(string MAP_NO, string OP_CODE)
        {
            Result ret = new Result();

            try
            {
                DataTable dt = new DataTable();
                StringBuilder sb = new StringBuilder();

                //DataMatrix코드 - [)>06 VSS1F P95760A7AA0 SYAAC EKA7F0067 T160227P1A1A0601 MN CSOP-H1.00-S1.002016-10-19
                //QR코드         - 95760F2000-ALL-SOP-1.00-1.01-16D250738 
                //위 코드만 조회


                sb.AppendLine("SELECT                                   ");
                sb.AppendLine("     SW_BOX_NO as MAT_CODE               ");
                sb.AppendLine("    ,CRT_DATE as MAT_NAME                ");
                sb.AppendLine("    ,'' LOT                              ");
                sb.AppendLine("    ,'' SEQ                              ");
                sb.AppendLine("    ,OP_CODE                             ");
                sb.AppendLine("    ,MAP_NO                              ");
                sb.AppendLine("    ,INPUT_YN                            ");
                sb.AppendLine("    ,'' MAT_NAME                         ");
                sb.AppendLine("FROM POP_PROD_IN_SWBOX                   ");
                sb.AppendLine("WHERE 1=1                                ");
                sb.AppendLine("AND OP_CODE ='{0}'                       ");
                sb.AppendLine("AND MAP_NO = '{1}'                       ");
                sb.AppendLine("and LENGTH(SW_BOX_NO) > 20               ");
                sb.AppendLine("and SUBSTR(SW_BOX_NO,0,2) in('95','[)')  ");
                sb.AppendLine("ORDER BY SEQ DESC                        ");
                 
                //Console.WriteLine("*********** 제품라벨 조회 시작***************************");
                //Console.WriteLine("SQL= " + sb.ToString());
                //Console.WriteLine("OP_CODE= " + OP_CODE.ToString());
                //Console.WriteLine("MAP_NO= " + MAP_NO.ToString());
                //Console.WriteLine("*********** 제품라벨 조회 종료***************************");
                

                string sSql = string.Format(sb.ToString(), OP_CODE, MAP_NO);
                dc = new DataCommon();
                dt = dc.getTable(sSql);


                if (dt != null)
                {
                    if (dt.Rows.Count < 1)
                    {
                        ret.VALUE = 0;
                        ret.MESSAGE = "DATA가 없습니다.";
                    }
                    else
                    {
                        ret.VALUE = 1;
                    }

                    ret.DATA = dt;
                }
            }
            catch (Exception ex)
            {
                ret.VALUE = -1;
                ret.MESSAGE = ex.Message;
            }

            return ret;
        }


         


        /// <summary>
        /// 터미널 DETAIL 불량등록
        /// </summary>
        /// <param name="MAP_SEQ">조회순번</param>
        /// <param name="MAP_NO">조회번호</param>
        /// <param name="OP_CODE">공정코드</param>
        /// <param name="BAD_CODE">불량코드</param>        
        /// <param name="BAD_QTY">불량수량</param>
        /// <param name="BAD_COMMENT">불량비고</param>
        /// <returns></returns>
        static public Result setPU0410_Bad(string BAD_SEQ, string MAP_NO, string OP_CODE, string BAD_CODE, string BAD_QTY, string BAD_COMMENT = "")
        {
            Result ret = new Result();

            try
            {
                //string sCheckSql = string.Format(
                //    "SELECT COUNT(*) AS CNT FROM POP_PROD_INSP WHERE DEL_YN ='N' AND OP_CODE = '{0}' AND BAD_CODE = '{1}'"
                //    , OP_CODE, BAD_CODE);

                //int iCnt = int.Parse( dc.getSimpleScalar(sCheckSql).ToString() );

                StringBuilder sbSql = new StringBuilder();
                string sSql = string.Empty;

                //if (iCnt < 1)
                if (string.IsNullOrEmpty(BAD_SEQ))
                {
                    sbSql.AppendLine("INSERT INTO POP_PROD_INSP                                     ");
                    sbSql.AppendLine("(                                                             ");
                    sbSql.AppendLine("     SEQ                                                      ");
                    sbSql.AppendLine("    ,MAP_NO                                                   ");
                    sbSql.AppendLine("    ,OP_CODE                                                  ");
                    sbSql.AppendLine("    ,BAD_CODE                                                 ");
                    sbSql.AppendLine("    ,BAD_QTY                                                  ");
                    sbSql.AppendLine("    ,BAD_COMMENT                                              ");
                    sbSql.AppendLine("    ,CRT_DATE                                                 ");
                    sbSql.AppendLine("    ,CRT_EMP                                                  ");
                    sbSql.AppendLine("    ,DEL_YN                                                   ");
                    sbSql.AppendLine("    ,MCH_CODE                                                 ");
                    sbSql.AppendLine(") VALUES (                                                    ");
                    sbSql.AppendLine("    ( SELECT NVL(  MAX(SEQ)+1 , 1  ) FROM POP_PROD_INSP  )    ");
                    sbSql.AppendLine("    , '{0}'                                                   ");
                    sbSql.AppendLine("    , '{1}'                                                   ");
                    sbSql.AppendLine("    , '{2}'                                                   ");
                    sbSql.AppendLine("    , '{3}'                                                   ");
                    sbSql.AppendLine("    , N'{4}'                                                  ");
                    sbSql.AppendLine("    , SYSDATE                                                 ");
                    sbSql.AppendLine("    , '{5}'                                                   ");
                    sbSql.AppendLine("    , 'N'                                                     ");
                    sbSql.AppendLine("    , '{6}'                                                   ");
                    sbSql.AppendLine(")                                                             ");


                    sSql = string.Format(sbSql.ToString(), MAP_NO, OP_CODE, BAD_CODE, BAD_QTY, BAD_COMMENT, Para.USER.ID, Para.Terminal.MchCode);
                }
                else
                {

                    sbSql.AppendLine("UPDATE POP_PROD_INSP    ");
                    sbSql.AppendLine("SET   BAD_QTY = '{1}'           ");
                    sbSql.AppendLine("      ,MCH_CODE = '{4}'           ");
                    if (!string.IsNullOrEmpty(BAD_COMMENT))
                    {
                        sbSql.AppendLine(", BAD_COMMENT = N'{2}'         ");
                    }
                    sbSql.AppendLine(", MOD_EMP = '{3}'         ");
                    sbSql.AppendLine(", MOD_DATE = SYSDATE      ");
                    sbSql.AppendLine("WHERE SEQ = '{0}'         ");

                    sSql = string.Format(sbSql.ToString(), BAD_SEQ, BAD_QTY, BAD_COMMENT, Para.USER.ID, Para.Terminal.MchCode);
                }

                /*
                Console.WriteLine("*********** POP_PROD_INSP 불량바코드 등록(수정) 시작***************************");
                Console.WriteLine("SQL= " + sbSql.ToString());
                Console.WriteLine("BAD_SEQ= " + BAD_SEQ.ToString());
                Console.WriteLine("BAD_QTY= " + BAD_QTY.ToString());
                Console.WriteLine("BAD_COMMENT= " + BAD_COMMENT.ToString());
                Console.WriteLine("Para.USER.ID= " + Para.USER.ID );
                Console.WriteLine("*********** POP_PROD_INSP 불량바코드 등록(수정) 종료***************************");
                */

                dc = new DataCommon();
                int iRet = dc.execNonQuery(sSql);

                if (iRet != 1)
                {
                    ret.VALUE = -1;
                    //ret.MESSAGE = "setPU0410_Bad의 쿼리실행수에 문제가 있습니다.";
                    ret.MESSAGE = "불량 등록 중 문제가 발생하였습니다.(ERR-100)";
                    return ret;
                }

                ret.VALUE = 1;

            }
            catch  
            {
                ret.VALUE = -1;
                //ret.MESSAGE = ex.Message;
                ret.MESSAGE = "불량 등록 중 문제가 발생하였습니다.(ERR-200)";
            }

            return ret;
        }





        /// <summary>
        /// 터미널 DETAIL 작업완료
        /// </summary>
        /// <param name="MAP_SEQ">조회순번</param>
        /// <param name="MAP_NO">조회번호</param>
        /// <param name="JOB_UNIT">공정단위</param>        
        /// <param name="REMARK">특이사항</param>
        /// <returns></returns>
        static public Result setPU0410_Remark(string MAP_SEQ, string MAP_NO, string JOB_UNIT, string REMARK)
        {
            Result ret = new Result();

            try
            {
                StringBuilder sbSql = new StringBuilder();

                //TRAY단위라면 MAPPING테이블에 REAMRK사용
                if (JOB_UNIT != "TRAY")
                {
                    sbSql.AppendLine("UPDATE POP_LOT_MAPPING    ");
                }
                else
                {
                    sbSql.AppendLine("UPDATE POP_JOB_ORDER      ");
                }
                
                sbSql.AppendLine("SET REMARK = N'{2}'        ");
                sbSql.AppendLine(", MOD_EMP = '{3}'         ");
                sbSql.AppendLine(", MOD_DATE = SYSDATE      ");

                //TRAY일 경우만 MAP_NO를 JOB_NO로 인식하여 JOB_ORDER테이블에 저장
                if (JOB_UNIT != "TRAY")
                {
                    sbSql.AppendLine("WHERE SEQ = '{0}'         ");
                }
                else
                {
                    sbSql.AppendLine("WHERE JOB_NO = '{1}'         ");
                }
                /*
                Console.WriteLine("*********** POP REMARK 업데이트 시작***************************");
                Console.WriteLine("SQL= " + sbSql.ToString());
                Console.WriteLine("MAP_SEQ= " + MAP_SEQ.ToString());
                Console.WriteLine("MAP_NO= " + MAP_NO.ToString());
                Console.WriteLine("JOB_UNIT= " + JOB_UNIT.ToString());
                Console.WriteLine("REMARK= " + REMARK.ToString());
                Console.WriteLine("*********** POP REMARK 업데이트 종료***************************");
                */

                string sSql = string.Format(sbSql.ToString(), MAP_SEQ, MAP_NO, REMARK.Replace("'","''") , Para.USER.ID);
                dc = new DataCommon();
                int iRet = dc.execNonQuery(sSql);

                if (iRet != 1)
                {
                    ret.VALUE = -1;
                    //ret.MESSAGE = "setPU0410_Workcomplete의 쿼리실행수에 문제가 있습니다.";
                    ret.MESSAGE = "REMARK 등록 중 문제가 발생하였습니다(ERR-200)";
                    return ret;
                }

                ret.VALUE = 1;

            }
            catch  
            {
                ret.VALUE = -1;
                //ret.MESSAGE = ex.Message;
                ret.MESSAGE = "REMARK 등록 중 문제가 발생하였습니다(ERR-300)";
            }

            return ret;
        }


        #endregion


        #endregion



        #region < 쿼리생성 >

        /// <summary>
        /// 작업상태를 변경하는 쿼리반환
        /// </summary>
        /// <param name="MAP_SEQ"></param>
        /// <param name="JOB_STATE">10 생성 , 20대기 , 30시작 , 40증지 , 50완료 , 90종료 </param>
        /// <param name="NOW_OP_SEQ">현재공정순서 </param>
        /// <param name="NOW_OP_CODE">현재공정코드 </param>
        /// <returns></returns>
        static public string queryUpdateJobState(string MAP_SEQ, string JOB_STATE, string CHANGE_ITEM_CODE, string NOW_OP_SEQ = "", string NOW_OP_CODE = "")
        {
            string ret = string.Empty;
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("UPDATE POP_LOT_MAPPING   ");
            sb.AppendLine("SET JOB_STATE = '{1}'    ");

            if (!string.IsNullOrEmpty(NOW_OP_SEQ))
            {
                sb.AppendLine(", NOW_OP_SEQ = '{2}'     ");
                sb.AppendLine(", NOW_OP_CODE = '{3}'    ");
            }

            if (CHANGE_ITEM_CODE != "")
            {
                sb.AppendLine(", CHANGE_ITEM_CODE = '{4}'    ");
            }
            sb.AppendLine("WHERE SEQ = '{0}'    ");

          
            /*
            Console.WriteLine("*********** POP_LOT_MAPPING 업데이트 시작***************************");
            Console.WriteLine("SQL= " + sb.ToString());
            Console.WriteLine("MAP_SEQ= " + MAP_SEQ.ToString());
            Console.WriteLine("JOB_STATE= " + JOB_STATE.ToString());
            Console.WriteLine("NOW_OP_SEQ= " + NOW_OP_SEQ.ToString());
            Console.WriteLine("NOW_OP_CODE= " + NOW_OP_CODE.ToString());
            Console.WriteLine("*********** POP_LOT_MAPPING 업데이트 종료***************************");
            */


            ret = string.Format(sb.ToString(), MAP_SEQ, JOB_STATE, NOW_OP_SEQ, NOW_OP_CODE, CHANGE_ITEM_CODE);

            return ret;

        }


        /// <summary>
        /// 작업이력저장 쿼리반환
        /// </summary>
        /// <param name="MAP_NO">조회번호</param>
        /// <param name="OP_CODE">공정코드</param>
        /// <param name="MCH_CODE">설비코드 (생략가능)</param>
        /// <param name="JOB_STATE">10 생성 , 20대기 , 30시작 , 40증지 , 50완료 , 90종료 </param>
        /// <param name="REMARK">특이사항</param>
        /// <returns></returns>
        static public string queryInsertJobHist( string MAP_NO, string OP_CODE , string MCH_CODE , string JOB_STATE , string CHANGE_ITEM_CODE, string REMARK = "")
        {
            string ret = string.Empty;
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("INSERT INTO POP_JOB_HIST     ");
            sb.AppendLine("(                            ");
            sb.AppendLine("     SEQ                     ");
            sb.AppendLine("   , MAP_NO                  ");
            sb.AppendLine("   , OP_CODE                 ");
            sb.AppendLine("   , MCH_CODE                ");
            sb.AppendLine("   , JOB_STATE               ");
            sb.AppendLine("   , REMARK                  ");
            sb.AppendLine("   , CRT_DATE                ");
            sb.AppendLine("   , CRT_EMP                 ");
            sb.AppendLine("   , DEL_YN                  ");
            sb.AppendLine("   , CHANGE_ITEM_CODE        ");
            sb.AppendLine(")                            ");
            sb.AppendLine("VALUES                       ");
            sb.AppendLine("(                            ");
            sb.AppendLine("    ( SELECT NVL( MAX(SEQ)+1 , 1 ) FROM POP_JOB_HIST )   ");
            sb.AppendLine("    , '{0}'                  ");
            sb.AppendLine("    , '{1}'                  ");
            sb.AppendLine("    , '{2}'                  ");
            sb.AppendLine("    , '{3}'                  ");
            sb.AppendLine("    , N'{4}'                 ");
            sb.AppendLine("    , SYSDATE                ");
            sb.AppendLine("    , '{5}'                  ");
            sb.AppendLine("    , 'N'                    ");
            sb.AppendLine("    , '{6}'                  ");
            sb.AppendLine(")                            ");             
      
            /*
            Console.WriteLine("*********** POP_JOB_HIST 등록 시작***************************");
            Console.WriteLine("SQL= " + sb.ToString());
            Console.WriteLine("MAP_NO= " + MAP_NO.ToString());
            Console.WriteLine("OP_CODE= " + OP_CODE.ToString());
            Console.WriteLine("MCH_CODE= " + MCH_CODE.ToString());
            Console.WriteLine("JOB_STATE= " + JOB_STATE.ToString());
            Console.WriteLine("REMARK= " + REMARK.ToString());
            Console.WriteLine("Para.USER.ID= " + Para.USER.ID);
            Console.WriteLine("*********** POP_JOB_HIST 등록 종료***************************");
            */
            
            ret = string.Format(sb.ToString(), MAP_NO, OP_CODE, MCH_CODE, JOB_STATE, REMARK, Para.USER.ID, CHANGE_ITEM_CODE); 

            return ret;

        }


        /// <summary>
        /// 현장 POP화면에서 현재공정에서 사용하지 못하는 공정이력카드 바코드를
        /// 스캔시 작업리스트를 사용자에게 보여준다
        /// </summary>
        /// <param name="sItemCode"></param>
        /// <param name="sMapNo"></param>
        /// <returns></returns>
        static public Result gfnWorkList(string sItemCode, string sMapNo)
        {
            Result ret = new Result();

            try
            {
                DataTable dt = new DataTable();
                StringBuilder sbSql = new StringBuilder();
                sbSql.AppendLine(" select   rownum,     ");
                sbSql.AppendLine("          tt.item_code,     ");
                sbSql.AppendLine("          tt.op_code,     ");
                sbSql.AppendLine("          tt.op_name,     ");
                sbSql.AppendLine("          tt.mch_code,     ");
                sbSql.AppendLine("          tt.job_state,     ");
                sbSql.AppendLine("          tt.CRT_DATE     ");
                sbSql.AppendLine(" FROM(                    ");
                sbSql.AppendLine("      SELECT  a.item_code,             ");
                sbSql.AppendLine("              a.op_code,               ");
                sbSql.AppendLine("              (select op.op_name from pop_op_code op where op.op_code = a.op_code) op_name,                        ");
                sbSql.AppendLine("              b.mch_code,                                                                                          ");
                sbSql.AppendLine("              (select sy.name from sys_code sy where sy.type='JOB_STATE' and sy.code = b.job_state) job_state,     ");
                sbSql.AppendLine("              b.CRT_DATE                                               ");
                sbSql.AppendLine("      from pop_item_routine a  left outer join pop_job_hist b  "); 
                sbSql.AppendLine("      on a.op_code = b.op_code             ");
                sbSql.AppendLine("      and b.map_no='" + sMapNo + "'        ");
                sbSql.AppendLine("      where 1=1                            ");
                sbSql.AppendLine("      and a.item_code ='" + sItemCode + "' ");
                sbSql.AppendLine("      order by a.sort_num                  ");
                sbSql.AppendLine(" ) tt ");
                Console.WriteLine(sbSql.ToString());
                dc = new DataCommon();
                dt = dc.getTable(sbSql.ToString());


                if (dt != null)
                {
                    ret.VALUE = 1;
                    ret.DATA = dt;
                }
            }
            catch (Exception ex)
            {
                ret.VALUE = -1;
                ret.MESSAGE = ex.Message;
            }

            return ret;
        }



        /// <summary>
        /// 라벨부착 등록
        /// </summary>
        /// <param name="MAP_SEQ">조회순번</param>
        /// <param name="MAP_NO">조회번호</param>
        /// <param name="OP_CODE">공정코드</param>
        /// <param name="MAT_CODE">자재코드</param>        
        /// <param name="LOT">자재LOT</param>
        /// <param name="INPUT_YN">자재사용여부 (REWORK전용)</param>
        /// <returns></returns>
        static public Result insertProductInfo(string sSekoNixCode, string sModelNo, string sBoxSerialBarCode, string sTmpBarCode, string sACL, string sHW, string sSW, string sOpCode)
        {
            Result ret = new Result();

            try
            {
                //중복데이터 PASS

                string sChk = DoubleChcekData(sSekoNixCode, sModelNo, sBoxSerialBarCode, sOpCode);
                if ( Convert.ToInt32( sChk ) > 0)
                {
                    ret.VALUE = 99;
                    ret.MESSAGE = "중복";
                    return ret;
                }
                

                StringBuilder sbSql = new StringBuilder();
                string sSql = string.Empty; 
                sbSql.AppendLine("INSERT INTO POP_PRODUCT_INFO      ");
                sbSql.AppendLine("(                                 ");
                sbSql.AppendLine("     SEQ_NO                       ");
                sbSql.AppendLine("    ,SEKONIX_CODE                ");
                sbSql.AppendLine("    ,MODEL_NO                     ");
                sbSql.AppendLine("    ,BOX_BARCODE                  ");
                sbSql.AppendLine("    ,TMP_BARCODE                  ");
                sbSql.AppendLine("    ,ACL_CODE                     ");
                sbSql.AppendLine("    ,HW_VER                       ");
                sbSql.AppendLine("    ,SW_VER                       ");
                sbSql.AppendLine("    ,OP_CODE                      ");
                sbSql.AppendLine("    ,CRT_DATE                     ");
                sbSql.AppendLine("    ,CRT_USER                     ");
                sbSql.AppendLine(") VALUES (                        ");
                sbSql.AppendLine("    ( SELECT NVL(  MAX(TO_NUMBER(SEQ_NO))+1 , 1  ) FROM POP_PRODUCT_INFO  )  ");
                sbSql.AppendLine("    , '{0}'                       ");
                sbSql.AppendLine("    , '{1}'                       ");
                sbSql.AppendLine("    , '{2}'                       ");
                sbSql.AppendLine("    , '{3}'                       ");
                sbSql.AppendLine("    , '{4}'                       ");
                sbSql.AppendLine("    , '{5}'                       ");
                sbSql.AppendLine("    , '{6}'                       ");
                sbSql.AppendLine("    , '{7}'                       ");
                sbSql.AppendLine("    , SYSDATE                     ");                
                sbSql.AppendLine("    , '{8}'                       ");
                sbSql.AppendLine(")                                 ");
                sSql = string.Format(sbSql.ToString(), sSekoNixCode, sModelNo, sBoxSerialBarCode,sTmpBarCode, sACL, sHW, sSW, sOpCode, Para.USER.ID);

                
                Console.WriteLine("*********** POP_PRODUCT_INFO INSERT 시작***************************");
                Console.WriteLine("SQL= " + sbSql.ToString());
                Console.WriteLine("sSekoNixCode= " + sSekoNixCode);
                Console.WriteLine("sModelNo= " + sModelNo);
                Console.WriteLine("sBoxSerialBarCode= " + sBoxSerialBarCode);
                Console.WriteLine("sTmpBarCode= " + sTmpBarCode);
                Console.WriteLine("sACL= " + sACL);
                Console.WriteLine("sHW= " + sHW);
                Console.WriteLine("sSW= " + sSW);                  
                Console.WriteLine("sOpCode= " + sOpCode);                  
                Console.WriteLine("Para.USER.ID= " + Para.USER.ID); 
                Console.WriteLine("*********** POP_PRODUCT_INFO INSERT 종료***************************");
                 

                dc = new DataCommon();
                int iRet = dc.execNonQuery(sSql);

                if (iRet != 1)
                {
                    ret.VALUE = -1;
                    ret.MESSAGE = "insertProductInfo의 쿼리실행수에 문제가 발생했습니다.";
                    return ret;
                }

                ret.VALUE = 1;

            }
            catch (Exception ex)
            {
                ret.VALUE = -1;
                ret.MESSAGE = ex.Message;
            }

            return ret;
        }



        /// <summary>
        /// 생산 수량 체크
        /// </summary>
        /// <param name="MAP_SEQ">조회순번</param>
        /// <param name="MAP_NO">조회번호</param>
        /// <param name="OP_CODE">공정코드</param>
        /// <param name="MAT_CODE">자재코드</param>        
        /// <param name="LOT">자재LOT</param>
        /// <param name="INPUT_YN">자재사용여부 (REWORK전용)</param>
        /// <returns></returns>
        static public Result insertProductQtyChk(string sModelNo, string sBoxLabel, string sProdLabel,string sHwVer, string sSwVer, string sOrderQty, string sWorkQty, string sOpCode)
        {
            Result ret = new Result();

            try
            {
                //중복데이터 PASS                
                string sCount = "0";
                string sSql = "";
                sSql += " SELECT COUNT(1) CNT                   ";
                sSql += " FROM  POP_M302_HIST                   ";
                sSql += " WHERE MODEL_NO    ='"+sModelNo    +"' ";
                sSql += " AND   BOX_LABEL   ='"+sBoxLabel   +"' ";
                sSql += " AND   PROD_LABEL  ='"+sProdLabel  +"' ";
                sSql += " AND   OP_CODE     ='"+sOpCode     +"' ";
                Console.WriteLine("중복=" + sSql);
                dc = new DataCommon();
                sCount = dc.getSimpleScalar(sSql).ToString();

                if (Convert.ToInt32(sCount) > 0)
                {
                    ret.VALUE = 99;
                    ret.MESSAGE = "중복";
                    return ret;
                }


                StringBuilder sbSql = new StringBuilder();                
                sbSql.AppendLine("INSERT INTO POP_M302_HIST         ");
                sbSql.AppendLine("(                                 ");
                sbSql.AppendLine("     MODEL_NO                     ");
                sbSql.AppendLine("    ,BOX_LABEL                    ");
                sbSql.AppendLine("    ,PROD_LABEL                   ");
                sbSql.AppendLine("    ,HW_VER                       ");
                sbSql.AppendLine("    ,SW_VER                       ");
                sbSql.AppendLine("    ,ORDER_QTY                    ");
                sbSql.AppendLine("    ,WORK_QTY                     ");
                sbSql.AppendLine("    ,OP_CODE                      ");                
                sbSql.AppendLine("    ,CRT_DATE                     ");
                sbSql.AppendLine("    ,CRT_EMP                      ");
                sbSql.AppendLine(") VALUES (                        ");                
                sbSql.AppendLine("      '{0}'                       ");
                sbSql.AppendLine("    , '{1}'                       ");
                sbSql.AppendLine("    , '{2}'                       ");
                sbSql.AppendLine("    , '{3}'                       ");
                sbSql.AppendLine("    , '{4}'                       ");
                sbSql.AppendLine("    , '{5}'                       ");
                sbSql.AppendLine("    , '{6}'                       ");
                sbSql.AppendLine("    , '{7}'                       ");
                sbSql.AppendLine("    , SYSDATE                     ");
                sbSql.AppendLine("    , '{8}'                       ");
                sbSql.AppendLine(")                                 ");
                sSql = string.Format(sbSql.ToString(), sModelNo, sBoxLabel, sProdLabel, sHwVer, sSwVer, sOrderQty, sWorkQty, sOpCode, Para.USER.ID);
                dc = new DataCommon();
                int iRet = dc.execNonQuery(sSql);

                if (iRet != 1)
                {
                    ret.VALUE = -1;
                    ret.MESSAGE = "insertProductInfo의 쿼리실행수에 문제가 발생했습니다.";
                    return ret;
                }

                ret.VALUE = 1;

            }
            catch (Exception ex)
            {
                ret.VALUE = -1;
                ret.MESSAGE = ex.Message;
            }

            return ret;
        }



        /// <summary>
        /// POP_PRODUCT_INFO에 이미 등록되었는지 체크
        /// </summary>
        /// <param name="sModelNo">제품코드</param>
        /// <param name="sBoxBarCode">BOX바코드</param>
        /// /// <param name="sOpCode">공정코드</param>
        static public string DoubleChcekData(string sSekoNixCode, string sModelNo, string sBoxSerialBarCode, string sOpCode)
        {
            string ret = string.Empty;

            try
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("SELECT COUNT(1) CNT FROM POP_PRODUCT_INFO WHERE SEKONIX_CODE='{0}' AND MODEL_NO='{1}' AND BOX_BARCODE='{2}' AND OP_CODE='{3}' ");

                /*
                Console.WriteLine("***************POP_PRODUCT_INFO 이미 등록되었는지 체크 시작************");
                Console.WriteLine("SQL =" + sb.ToString());
                Console.WriteLine("sSekoNixCode = " + sSekoNixCode);
                Console.WriteLine("sModelNo = " + sModelNo);
                Console.WriteLine("sBoxSerialBarCode = " + sBoxSerialBarCode);
                Console.WriteLine("sOpCode = " + sOpCode);
                Console.WriteLine("***************POP_PRODUCT_INFO 이미 등록되었는지 체크 종료************");
                */

                string sSql = string.Format(sb.ToString(), sSekoNixCode, sModelNo, sBoxSerialBarCode, sOpCode);
                dc = new DataCommon();
                object oRet = dc.getSimpleScalar(sSql);

                if (oRet != null)
                {
                    ret = oRet.ToString();
                }
            }
            catch
            {
                throw;
                //ret = ex.Message;
            }

            return ret;
        }

        #endregion


    }
}
