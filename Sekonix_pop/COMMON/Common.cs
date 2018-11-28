using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;
using System.Data;
using System.Text.RegularExpressions;
using System.Globalization;


namespace Sekonix_pop
{
    class Common
    {
        public static DataCommon dc = new DataCommon();


        //public static string sLocalImgDir = @"C:\IMG\"; //로컬
        public static string sLocalImgDir = @"\\118.32.222.220\mes\MES_DATA\IMG"; //세코닉스서버 - 이미지
        public static string sLocalExcelDir = @"\\118.32.222.220\mes\MES_DATA\EXCEL"; //세코닉스서버- 공정이력카드
        //public static string sLocalImgDir = @"\\192.168.10.37\\공유 볼륨\\IMG"; //테스트서버


        public static string sExetension = "JPG";
        
        public static string makeImgFilename(string sType, string sTbMatNo)
        {
            string ret = null;

            //if( sTbMatNo.Split('_').Length > 1 )

            switch (sType)
            {
                default:
                case "M":
                    ret = string.Format("{0}.{1}", sTbMatNo, sExetension);
                    break;
            }

            return ret;
        }

        public static string makeImgPath(string sType, string sTbMatNo)
        {
            string ret = null;
            
            //if( sTbMatNo.Split('_').Length > 1 )

            string sImgFilename = makeImgFilename(sType, sTbMatNo);

            if( string.IsNullOrEmpty(sImgFilename ) ) return ret;
            
            ret = string.Format("{0}\\{1}", sLocalImgDir, sImgFilename );
            
            return ret;
        }
        
        public static bool tranFile(string sSource, string sDestination)
        {
            bool ret = false;
            try
            {
                if (System.IO.File.Exists(sDestination)) System.IO.File.Delete(sDestination);

                System.IO.File.Copy(sSource, sDestination);
                
                ret = true;
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message, "ERROR");
            }

            return ret;
        }

        public static bool makeImgNtranFile(string sType, string sTbMatNo, string sSource)
        {
            bool ret = false;
            try 
            {
                string sDestination = makeImgPath(sType, sTbMatNo);

                if (string.IsNullOrEmpty(sDestination)) return ret;

                if (tranFile(sSource, sDestination))
                {
                    ret = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "ERROR");
            }

            return ret;
        }
        public static double NORMDIST(double x, double mean, double standard_dev, bool cumulative)
        {
            const double parts = 50000.0; //large enough to make the trapzoids small enough

            double lowBound = 0.0;
            if (cumulative) //do integration: trapezoidal rule used here
            {
                double width = (x - lowBound) / (parts - 1.0);
                double integral = 0.0;
                for (int i = 1; i < parts - 1; i++)
                {
                    integral += 0.5 * width * (NormalDist(lowBound + width * i, mean, standard_dev) +
                        (NormalDist(lowBound + width * (i + 1), mean, standard_dev)));
                }
                return integral;
            }
            else //return function value
            {
                return NormalDist(x, mean, standard_dev);
            }
        }
        public static double NormalDist(double x, double mean, double standard_dev)
        {
            double fact = standard_dev * Math.Sqrt(2.0 * Math.PI);
            double expo = (x - mean) * (x - mean) / (2.0 * standard_dev * standard_dev);
            return Math.Exp(-expo) / fact;
        }

        public String strUTF8(string str)
        {
            Encoding utf8 = Encoding.UTF8;
            byte[] utf8Bytes = utf8.GetBytes((str));

            String decodeStringByUTF8 = utf8.GetString(utf8Bytes);

            return decodeStringByUTF8;
        }


        public void ReadData(string sFile)
        {             
            try
            {
                System.IO.StreamReader psReader = new System.IO.StreamReader(sFile, System.Text.Encoding.GetEncoding(949));
                string HTMLString1 = psReader.ReadToEnd();
                psReader.Close();

            }
            catch  
            {
                Console.WriteLine("Not Found file");
            }
        }

        //파일 덥어쓰기
        public void WriteData(string sFile, string sText)
        {
            try
            {
                System.IO.FileStream file_stream = new System.IO.FileStream(sFile, System.IO.FileMode.Create, System.IO.FileAccess.Write);
                System.IO.StreamWriter psWriter = new System.IO.StreamWriter(file_stream, System.Text.Encoding.Default);
                 
                psWriter.WriteLine(sText);
                psWriter.Close();


            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }


        //파일 이어쓰기
        public void Log(string sFileName, string sText, string sRemark)
        {
            try
            {
                string sChkDay = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                string sToDay = System.DateTime.Today.ToString("yyyyMMdd");
                string sSavePath = "\\LOG\\" + sToDay + "\\";
                string sDirPath = Application.StartupPath + sSavePath ;
                 
                System.IO.DirectoryInfo di = new System.IO.DirectoryInfo(sDirPath);
                if (di.Exists == false)
                {
                    di.Create();
                }

                System.IO.FileStream file_stream = new System.IO.FileStream(sDirPath + "\\" + sFileName, System.IO.FileMode.Append, System.IO.FileAccess.Write);
                System.IO.StreamWriter psWriter = new System.IO.StreamWriter(file_stream, System.Text.Encoding.Default);

                psWriter.WriteLine(sChkDay + "_" + sText + "_" + Sekonix_pop.Para.USER.ID + "_" + sRemark);
                psWriter.Close(); 


            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        //#############################//
        //#############################//
        //CSV 파일 생성
        //#############################//
        //#############################//
        public void WriteToCSV(TextWriter stream, DataTable table, bool header, bool quoteall)
        {
            if (header)
            {
                for (int i = 0; i < table.Columns.Count; i++)
                {
                    WriteItem(stream, table.Columns[i].Caption, quoteall);
                    if (i < table.Columns.Count - 1)
                        stream.Write(',');
                    else
                        stream.Write("\r\n");
                }
            }
            foreach (DataRow row in table.Rows)
            {
                for (int i = 0; i < table.Columns.Count; i++)
                {
                    WriteItem(stream, row[i], quoteall);
                    if (i < table.Columns.Count - 1)
                        stream.Write(',');
                    else
                        stream.Write("\r\n");
                }
            }
            stream.Flush();
            stream.Close();
        }

        private static void WriteItem(TextWriter stream, object item, bool quoteall)
        {
            if (item == null)
                return;
            string s = item.ToString();
            if (quoteall || s.IndexOfAny("\",\x0A\x0D".ToCharArray()) > -1)
                stream.Write("\"" + s.Replace("\"", "\"\"") + "\"");
            else
                stream.Write(s);
            stream.Flush();
        }


        
        public static bool isChkDate(string sDate, string sFlag)
        {
            if(sFlag == "YYYY-MM-DD"){
                //날짜형식이 YYYY-MM-DD 형식인지 체크
                return Regex.IsMatch(sDate, @"^(19|20)\d{2}-(0[1-9]|1[012])-(0[1-9]|[12][0-9]|3[0-1])$");

            }
            else if (sFlag == "YYYYMMDD")
            {
                //날짜형식이 YYYYMMDD 형식인지 체크
                return Regex.IsMatch(sDate, @"^(19|20)\d{2}(0[1-9]|1[012])(0[1-9]|[12][0-9]|3[0-1])$");
            }
            else if (sFlag == "MMDD")
            {
                //날짜형식이 MMDD 형식인지 체크
                return Regex.IsMatch(sDate, @"^\d{2}(0[1-9]|1[012])(0[1-9]|[12][0-9]|3[0-1])$");
            }

            return false;

        }

        //파일및 폴더 복사
        public static void CopyFolder(string sourceFolder, string destFolder)
        { 

            String timeStamp = DateTime.Now.ToString("yyyyMMddHHmmssffff");



            //복사대상 폴더 확인
            if (!Directory.Exists(destFolder))
            {
                Directory.CreateDirectory(destFolder);
            }
            
            //원본폴더 안의 파일 
            String[] files = Directory.GetFiles(sourceFolder);
            //원본폴더 안의 폴더
            String[] folders = Directory.GetDirectories(sourceFolder);

            //C:\caminsp\execute\report 경로에 파일이 있는냐?
            foreach (String file in files)
            {
                String name = Path.GetFileName(file);
                String dest = Path.Combine(destFolder, name);
                
                //파일을 복사
                File.Copy(file, dest);
            }

            //C:\caminsp\execute\report 에 폴더가 있느냐?
            foreach (String folder in folders)
            {
                String name = Path.GetFileName(folder);

                //같은 폴더가 존재시 문제되므로 폴더이름 앞에 timeseamp를 적용
                name = name + "-" + timeStamp ;
                String dest = Path.Combine(destFolder, name);

                
                CopyFolder(folder, dest);
                 

            }
        }

        //해당경로의 하위폴더 및 파일 삭제
        public static void DeleteFolder(string directory)
        {
            string[] files = System.IO.Directory.GetFiles(directory);
            foreach (string file in files)
            {
                DeleteFile(file);
            }

            DeleteDirectory(directory);
        }


        private static void DeleteFile(string filePath)
        {
            System.IO.FileInfo info = new System.IO.FileInfo(filePath);
            info.Attributes = System.IO.FileAttributes.Normal;
            System.IO.File.Delete(filePath);
        }

        private static void DeleteDirectory(string directory)
        {
            System.IO.DirectoryInfo info = new System.IO.DirectoryInfo(directory);
            info.Attributes = System.IO.FileAttributes.Normal;
            System.IO.Directory.Delete(directory);
        }

        //<summary>
        //해당 폼 이름으로 폼이 실행되고 있으면 해당폼의 form값을 반환
        public static Form GetForm(string formName)
        {
            foreach (Form frm in Application.OpenForms)
            {
                if (frm.Name == formName)
                {
                    return frm;
                }
            }

            return null;
        }

        #region #. CheckEnglish
        /// <summary> 
        /// 영문체크 
        /// </summary> 
        /// <param name="letter">문자 
        /// <returns></returns> 

        public static bool CheckEnglish(string letter)
        {
            bool IsCheck = true;
            Regex engRegex = new Regex(@"[a-zA-Z]");
            Boolean ismatch = engRegex.IsMatch(letter);
            if (!ismatch)
            {
                IsCheck = false;
            }

            return IsCheck;
        }
        #endregion

        #region #. CheckNumber

        /// <summary> 
        /// 숫자체크 
        /// </summary> 
        /// <param name="letter">문자 
        /// <returns></returns> 

        public static bool CheckNumber(string letter)
        {
            bool IsCheck = true;
            Regex numRegex = new Regex(@"[0-9]");
            Boolean ismatch = numRegex.IsMatch(letter);
            if (!ismatch)
            {
                IsCheck = false;
            }

            return IsCheck;
        }

        #endregion


        #region #. CheckEnglishNumber

        /// <summary> 
        /// 영문/숫자체크 
        /// </summary> 
        /// <param name="letter">문자 
        /// <returns></returns> 

        public static bool CheckEnglishNumber(string letter)
        {
            bool IsCheck = true;
            Regex engRegex = new Regex(@"[a-zA-Z]");
            Boolean ismatch = engRegex.IsMatch(letter);
            Regex numRegex = new Regex(@"[0-9]");
            Boolean ismatchNum = numRegex.IsMatch(letter);
            if (!ismatch && !ismatchNum)
            {
                IsCheck = false;
            }

            return IsCheck;
        }


        #endregion 
         

        #region 진법변환
        /// <summary>
        /// Converts the given decimal number to the numeral system with the
        /// specified radix (in the range [2, 36]).
        /// </summary>
        /// <param name="decimalNumber">The number to convert.</param>
        /// <param name="radix">The radix of the destination numeral system
        /// (in the range [2, 36]).</param>
        /// <returns></returns>
        public static string DecimalToArbitrarySystem(long decimalNumber, int radix)
        {
            const int BitsInLong = 64;
            const string Digits = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";

            if (radix < 2 || radix > Digits.Length)
                throw new ArgumentException("The radix must be >= 2 and <= " + Digits.Length.ToString());

            if (decimalNumber == 0)
                return "0";

            int index = BitsInLong - 1;
            long currentNumber = Math.Abs(decimalNumber);
            char[] charArray = new char[BitsInLong];

            while (currentNumber != 0)
            {
                int remainder = (int)(currentNumber % radix);
                charArray[index--] = Digits[remainder];
                currentNumber = currentNumber / radix;
            }

            string result = new String(charArray, index + 1, BitsInLong - index - 1);
            if (decimalNumber < 0)
            {
                result = "-" + result;
            }

            return result;
        }
        #endregion

         

    }
}
