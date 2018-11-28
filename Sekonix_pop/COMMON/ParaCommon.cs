using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sekonix_pop
{
    public static class Para
    {
        /// <summary>
        /// 사용자정보
        /// </summary>
        public static class USER
        {
            /// <summary>
            /// 사용자ID
            /// </summary>
            public static string ID
            {
                get;
                set;
            }

            /// <summary>
            /// 사용자명
            /// </summary>
            public static string NAME
            {
                get;
                set;
            }

            /// <summary>
            /// 부서
            /// </summary>
            public static string DEPT
            {
                get;
                set;
            }

        }

        /// <summary>
        /// 작업정보 ( 단말기 )
        /// </summary>
        public static class Work
        {
            
            /// <summary>
            /// 작업번호 
            /// </summary>
            public static string MapNo
            {
                get;
                set;
            }


            /// <summary>
            /// 작업상태 
            /// </summary>
            public static string JobState
            {
                get;
                set;
            }


            /// <summary>
            /// 품번
            /// </summary>
            public static string ItemNo
            {
                get;
                set;
            }


            /// <summary>
            /// 모델명
            /// </summary>
            public static string Model
            {
                get;
                set;
            }


            /// <summary>
            /// 작업단위 ( TRAY , REAR , SERIAL , SW_BOX , BOX )
            /// </summary>
            public static string JobUnit
            {
                get;
                set;
            }


            /// <summary>
            /// 자재번호 ( MAT_CODE + LOT )
            /// </summary>
            public static string MatNo
            {
                get;
                set;
            }


        }

        public static class Terminal
        {
            /// <summary>
            /// 제품코드
            /// </summary>
            public static string ItemCode
            {
                get;
                set;
            }

            /// <summary>
            /// 지시번호
            /// </summary>
            public static string JobCode
            {
                get;
                set;
            }

            /// <summary>
            /// 공정코드
            /// </summary>
            public static string OpCode
            {
                get;
                set;
            }


            /// <summary>
            /// 공정명
            /// </summary>
            public static string OpName
            {
                get;
                set;
            }

            /// <summary>
            /// 설비코드
            /// </summary>
            public static string MchCode
            {
                get;
                set;
            }

            /// <summary>
            /// 설비명
            /// </summary>
            public static string MchName
            {
                get;
                set;
            }


            /// <summary>
            /// 작업단위 ( TRAY , REAR , SERIAL , SW_BOX , BOX )
            /// </summary>
            public static string JobUnit
            {
                get;
                set;
            }


            /// <summary>
            /// 입력방식 ( S : Scanner / W : Window Mesaage 
            /// </summary>
            public static string InputType
            {
                get;
                set;
            }

            /// <summary>
            /// BIT정보
            /// </summary>
            public static string Bit
            {
                get;
                set;
            } 
        }
    }
}
