using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
 
using System.Drawing; 
using System.Threading.Tasks; 

//using System.Windows.Forms;
using System.IO;
using Microsoft.Office.Interop.Excel;
using System.Diagnostics;
using System.Drawing.Imaging;

namespace Sekonix_pop
{
    class ExcelCommon
    {
        Application app; //= new Application();
        Workbook wb;        
        Worksheet ws;

        //
        public void WorksheerName(int count, string name)
        {
            app.Worksheets[count].name = name;
        }
        public void WorksheetCopy(int count)
        {
            for (int i = 0; count > i; i++)
            {
                app.Worksheets[1].Copy(app.Worksheets[app.Worksheets.Count]);
            }
        }
        public void wsadd()
        {
            app.Worksheets.Add();
        }
        Color Cl;

        public ExcelCommon()
        {
            //app.Visible = true;
            app = new Application();
            wb = app.Workbooks.Add();
            ws = app.ActiveSheet;
            //app.Worksheets.Copy("양식", "sheet2");
            //app.Worksheets.Copy("sheet1", "sheet2"); d워크시트 복사 양식 
            

            //wsDic.Add(ws.Name, ws);

            ws.Cells.HorizontalAlignment = XlHAlign.xlHAlignCenter;
        }


        public bool toPrint(int count, string name)
        {
            //app = new Application();
            //app.Sheets.Select(count);            
            app.Worksheets[count].name = name;            
            //wb = app.Workbooks.Add();
            ws = app.ActiveSheet;

            object printer = Type.Missing; // "프린터명"
            //ws.PrintOut(1, Type.Missing, 1, false, printer, false, false, Type.Missing);
            ws.PrintOutEx(1, Type.Missing, 1, false, printer, false, false, Type.Missing);
            return true;

        }



        public ExcelCommon(string url)
        {
            //app.Visible = true;
            app = new Application();
            wb = app.Workbooks.Add(url);
            ws = app.ActiveSheet;

            //wsDic.Add(ws.Name, ws);

            ws.Cells.HorizontalAlignment = XlHAlign.xlHAlignCenter;
            //ws.Cells.HorizontalAlignment = XlHAlign.xlHAlignLeft;
        }

        public void CreateSheet(string sName,int count)
        {
            if (sName != "")
            {

                app.Sheets.Add(Type.Missing, Type.Missing, count + 1, Type.Missing);

                SeleteSheet(count);

                RenameSheet(sName);
            }
            
            //워크북에 추가하는거 빠짐
            //wsDic.Add(ws.Name, ws);
        }

        //테스트 필요!!
        //public void DeleteSheet(int iCnt)//string sName)
        public void DeleteSheet(string sName)
        {
            //wsDic[sName].Delete();
            app.Sheets[sName].Delete();
            
            //app.Sheets[iCnt].Delete();
            //ws.Delete();
        }


        public void SeleteSheet(int iCnt)//string sName)
        {
            //app.Worksheets[app.Worksheets.Count]
            //try
            //{
            //app.Worksheets.Select(iCnt);
            app.Sheets.Select(iCnt);            
            ws = app.ActiveSheet;

                //ws = wsDic[sName];
                //ws = app.Sheets[0].Select();
            //}
            //catch (Exception ex)
            //{ 

            //}

            
        }

        public void RenameSheet(string sName)
        {             
            //app.Sheets[ws.Name].Name = sName;

            if (sName == "")
            {
                return;
            }

            ws.Name =   sName;
        }

        public void visible(bool bVisible)
        {
            try
            {
                app.Visible = bVisible;
            }
            catch
            { }
        }

        public void setText(int iRow, string sColumn, string sValue)
        {
            ws.Cells[iRow, sColumn] = sValue;
            
        }
        public void setText(int iRow, string sColumn, string sValue, int count)
        {
            app.Worksheets[count].Cells[iRow, sColumn] = sValue;
            
        }
        public void setImg(int iRow, string sColumn, Image img1)
        {
            //ws.Cells[iRow, sColumn] = sValue;

            //MemoryStream ms = new MemoryStream();
            //Image img1 = Image.FromStream(ms);

            System.Windows.Forms.Clipboard.SetImage(img1);

            object missingParam = Type.Missing;
            //Microsoft.Office.Interop.Excel.Application objExcel = new Microsoft.Office.Interop.Excel.Application();
            //Workbook workbook = app.Workbooks.Open(@"c:\book3.xls");
            //Worksheet worksheet = (Worksheet)workbook.Worksheets["Sheet1"];
            //Range cellRange1 = ws.get_Range("A2", "A2");
            //System.Windows.Forms.Clipboard.SetDataObject(img1, true);

            Range cellRange = ws.get_Range(sColumn + iRow.ToString(), sColumn + iRow.ToString());

            ws.Paste(cellRange, img1);

            //ws.Cells[iRow, sColumn].Paste(img1);

            System.Windows.Forms.Clipboard.Clear();

            //MemoryStream ms2 = new MemoryStream();
            //chart2.SaveImage(ms2, ImageFormat.Png);
            //Image img2 = Image.FromStream(ms2);
            //Clipboard.SetImage(img2);
            //Range cellRange2 = worksheet.get_Range("A10", "A11");
            //System.Windows.Forms.Clipboard.SetDataObject(img2, true);
            //worksheet.Paste(cellRange2, img2);
            //workbook.SaveAs(@"c:\book3.xls");
            //System.Windows.Forms.Clipboard.Clear();

            //objExcel.Quit();


        }

        public void setWidth(string sColumn, double dWidth)
        {
            ws.Cells[1, sColumn].ColumnWidth = dWidth;

            //ws.get_Range("A6", "A6").RowHeight = 18; //임시
            //ws.get_Range("A15", "A15").RowHeight = 18; //임시
        }

        public void setWidth(string sRange, string sRange2, double dWidth)
        {
            ws.get_Range(sRange, sRange2).ColumnWidth = dWidth;
        }

        public void setHeight(int iRow, double dHeight)
        {
            ws.Cells[iRow, "A"].RowHeight = dHeight;

            //ws.get_Range("A6", "A6").RowHeight = 18; //임시
            //ws.get_Range("A15", "A15").RowHeight = 18; //임시
        }

        public void setHeight(string sRange, string sRange2, double dHeight)
        {
            ws.get_Range(sRange, sRange2).RowHeight = dHeight;
        }

        public void setAlign(int iRow, string sColumn, string sAlign)
        {
            XlHAlign xAlign = xAlign = XlHAlign.xlHAlignLeft;

            switch (sAlign.ToUpper())
            {
                case "L":
                    xAlign = XlHAlign.xlHAlignLeft;
                    break;
                case "R":
                    xAlign = XlHAlign.xlHAlignRight;
                    break;
                case "C":
                    xAlign = XlHAlign.xlHAlignCenter;
                    break;
            }

            ws.Cells[iRow, sColumn].HorizontalAlignment = xAlign;
            //ws.get_Range("", "").HorizontalAlignment = xAlign;
        }

        public void setBgcolor(int iRow, string sColumn, int iR, int iG, int iB)
        {
            ws.Cells[iRow, sColumn].Interior.Color = Color.FromArgb(iR, iG, iB);//.ToArgb();
        }

        public void setBgcolor(string sRange, string sRange2, int iR, int iG, int iB)
        {
            ws.get_Range(sRange, sRange2).Interior.Color = Color.FromArgb(iR, iG, iB);//.ToArgb();
        }



        public void setFcolor(int iRow, string sColumn, int iR, int iG, int iB)
        {
            ws.Cells[iRow, sColumn].Font.Color = Color.FromArgb(iR, iG, iB);//.ToArgb();
        }



        public void setFont(int iRow, string sColumn, int iSize)
        {
            ws.Cells[iRow, sColumn].Font.Size = iSize;
        }

        public void setFont(string sRange, string sRange2, int iSize)
        {
            ws.get_Range(sRange, sRange2).Font.Size = iSize;
        }

        public void setFontNM(int iRow, string sColumn, string sFontName)
        {
            switch (sFontName.ToUpper())
            {
                case "1":
                    sFontName = "Arial";
                    break;
                case "2":
                    sFontName = "돋움체";
                    break;
            }

            ws.Cells[iRow, sColumn].Font.Name = sFontName.ToString().Trim().Replace("­", "");
        }

        public void setFontNM(string sRange, string sRange2)
        {
            ws.get_Range(sRange, sRange2).Font.Name = "돋움체".ToString().Trim().Replace("­", "");
        }

        public void setBold(int iRow, string sColumn, bool bBold = true)
        {
            ws.Cells[iRow, sColumn].Font.Bold = bBold;
        }

        public void setBold(string sRange, string sRange2, bool bBold = true)
        {
            ws.get_Range(sRange, sRange2).Font.Bold = bBold;
        }

        public void setMerge(string sRange, string sRange2)
        {
            ws.get_Range(sRange, sRange2).Merge();
            //ws.get_Range(sRange, sRange2).HorizontalAlignment = XlHAlign.xlHAlignCenter;

            ws.get_Range(sRange, sRange2).WrapText = true; //자동줄바꿈

        }

        public void setLine(string sRange, string sRange2, int iWeightInner = 2, int iWeightOuter = 3)
        {
            this.setLineInner(sRange, sRange2, iWeightInner);
            this.setLineOuter(sRange, sRange2, iWeightOuter);

            //try
            //{
            //    ws.get_Range(sRange, sRange2).Borders.LineStyle = XlLineStyle.xlContinuous;

            //    ws.get_Range(sRange, sRange2).BorderAround(XlLineStyle.xlContinuous, XlBorderWeight.xlThick, XlColorIndex.xlColorIndexAutomatic, XlColorIndex.xlColorIndexAutomatic);

            //    ws.get_Range(sRange, sRange2).WrapText = true; //자동줄바꿈

            //}
            //catch (Exception ex)
            //{ 

            //}
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sRange"></param>
        /// <param name="sRange2"></param>
        /// <param name="iWeight">1: Thin 2: medium 3:Hairline 0:Thick</param>
        public void setLineInner(string sRange, string sRange2, int iWeight = 2)
        {
            XlBorderWeight xBorderWeight;

            switch (iWeight)
            {
                case 1:
                    xBorderWeight = XlBorderWeight.xlHairline;
                    break;
                case 2:
                    xBorderWeight = XlBorderWeight.xlThin;
                    break;
                case 3:
                    xBorderWeight = XlBorderWeight.xlMedium;
                    break;
                default:
                    xBorderWeight = XlBorderWeight.xlThick;
                    break;
            }


            //ws.get_Range(sRange, sRange2).Borders[XlBordersIndex.xlInsideHorizontal].Color = Color.Red;
            //ws.get_Range(sRange, sRange2).Borders[XlBordersIndex.xlInsideHorizontal].Color = Color.Blue;

            ws.get_Range(sRange, sRange2).Borders[XlBordersIndex.xlInsideHorizontal].LineStyle = XlLineStyle.xlContinuous;
            ws.get_Range(sRange, sRange2).Borders[XlBordersIndex.xlInsideHorizontal].ColorIndex = XlColorIndex.xlColorIndexAutomatic;
            ws.get_Range(sRange, sRange2).Borders[XlBordersIndex.xlInsideHorizontal].TintAndShade = 0;
            ws.get_Range(sRange, sRange2).Borders[XlBordersIndex.xlInsideHorizontal].Weight = xBorderWeight;

            ws.get_Range(sRange, sRange2).Borders[XlBordersIndex.xlInsideVertical].LineStyle = XlLineStyle.xlContinuous;
            ws.get_Range(sRange, sRange2).Borders[XlBordersIndex.xlInsideVertical].ColorIndex = XlColorIndex.xlColorIndexAutomatic;
            ws.get_Range(sRange, sRange2).Borders[XlBordersIndex.xlInsideVertical].TintAndShade = 0;
            ws.get_Range(sRange, sRange2).Borders[XlBordersIndex.xlInsideVertical].Weight = xBorderWeight;

        }

        public void setLineTop(string sRange, string sRange2, string IColor)
        {
            switch (IColor)
            {
                case "R":
                    Cl = Color.Red;
                    break;
                case "B":
                    Cl = Color.Blue;
                    break;
                default:
                    Cl = Color.Black;
                    break;
            }

            ws.get_Range(sRange, sRange2).Borders[XlBordersIndex.xlEdgeTop].LineStyle = XlLineStyle.xlContinuous;
            ws.get_Range(sRange, sRange2).Borders[XlBordersIndex.xlEdgeTop].Color = Cl;
            ws.get_Range(sRange, sRange2).Borders[XlBordersIndex.xlEdgeTop].Weight = XlBorderWeight.xlThick;
        }

        public void setLineDown(string sRange, string sRange2, string IColor)
        {
            switch (IColor)
            {
                case "R":
                    Cl = Color.Red;
                    break;
                case "B":
                    Cl = Color.Blue;
                    break;
                default:
                    Cl = Color.Black;
                    break;
            }

            ws.get_Range(sRange, sRange2).Borders[XlBordersIndex.xlEdgeBottom].LineStyle = XlLineStyle.xlContinuous;
            ws.get_Range(sRange, sRange2).Borders[XlBordersIndex.xlEdgeBottom].Color = Cl;
            ws.get_Range(sRange, sRange2).Borders[XlBordersIndex.xlEdgeBottom].Weight = XlBorderWeight.xlThick;
        }

        public void setLineLeft(string sRange, string sRange2, string IColor)
        {
            switch (IColor)
            {
                case "R":
                    Cl = Color.Red;
                    break;
                case "B":
                    Cl = Color.Blue;
                    break;
                default:
                    Cl = Color.Black;
                    break;
            }

            ws.get_Range(sRange, sRange2).Borders[XlBordersIndex.xlEdgeLeft].LineStyle = XlLineStyle.xlContinuous;
            ws.get_Range(sRange, sRange2).Borders[XlBordersIndex.xlEdgeLeft].Color = Cl;
            ws.get_Range(sRange, sRange2).Borders[XlBordersIndex.xlEdgeLeft].TintAndShade = 0;
            ws.get_Range(sRange, sRange2).Borders[XlBordersIndex.xlEdgeLeft].Weight = XlBorderWeight.xlThick;
        }

        public void setLineRight(string sRange, string sRange2, string IColor)
        {
            switch (IColor)
            {
                case "R":
                    Cl = Color.Red;
                    break;
                case "B":
                    Cl = Color.Blue;
                    break;
                default:
                    Cl = Color.Black;
                    break;
            }

            ws.get_Range(sRange, sRange2).Borders[XlBordersIndex.xlEdgeRight].LineStyle = XlLineStyle.xlContinuous;
            ws.get_Range(sRange, sRange2).Borders[XlBordersIndex.xlEdgeRight].Color = Cl;
            ws.get_Range(sRange, sRange2).Borders[XlBordersIndex.xlEdgeRight].TintAndShade = 0;
            ws.get_Range(sRange, sRange2).Borders[XlBordersIndex.xlEdgeRight].Weight = XlBorderWeight.xlThick;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sRange"></param>
        /// <param name="sRange2"></param>
        /// <param name="iWeight">1: Thin 2: medium 3:Hairline 0:Thick</param>
        public void setLineOuter(string sRange, string sRange2, int iWeight = 3)
        {
            XlBorderWeight xBorderWeight;

            switch (iWeight)
            {
                case 1:
                    xBorderWeight = XlBorderWeight.xlHairline;
                    break;
                case 2:
                    xBorderWeight = XlBorderWeight.xlThin;
                    break;
                case 3:
                    xBorderWeight = XlBorderWeight.xlMedium;
                    break;
                default:
                    xBorderWeight = XlBorderWeight.xlThick;
                    break;
            }

            ws.get_Range(sRange, sRange2).Borders[XlBordersIndex.xlEdgeLeft].LineStyle = XlLineStyle.xlContinuous;
            ws.get_Range(sRange, sRange2).Borders[XlBordersIndex.xlEdgeLeft].ColorIndex = XlColorIndex.xlColorIndexAutomatic;
            ws.get_Range(sRange, sRange2).Borders[XlBordersIndex.xlEdgeLeft].TintAndShade = 0;
            ws.get_Range(sRange, sRange2).Borders[XlBordersIndex.xlEdgeLeft].Weight = xBorderWeight;

            ws.get_Range(sRange, sRange2).Borders[XlBordersIndex.xlEdgeRight].LineStyle = XlLineStyle.xlContinuous;
            ws.get_Range(sRange, sRange2).Borders[XlBordersIndex.xlEdgeRight].ColorIndex = XlColorIndex.xlColorIndexAutomatic;
            ws.get_Range(sRange, sRange2).Borders[XlBordersIndex.xlEdgeRight].TintAndShade = 0;
            ws.get_Range(sRange, sRange2).Borders[XlBordersIndex.xlEdgeRight].Weight = xBorderWeight;

            ws.get_Range(sRange, sRange2).Borders[XlBordersIndex.xlEdgeTop].LineStyle = XlLineStyle.xlContinuous;
            ws.get_Range(sRange, sRange2).Borders[XlBordersIndex.xlEdgeTop].ColorIndex = XlColorIndex.xlColorIndexAutomatic;
            ws.get_Range(sRange, sRange2).Borders[XlBordersIndex.xlEdgeTop].TintAndShade = 0;
            ws.get_Range(sRange, sRange2).Borders[XlBordersIndex.xlEdgeTop].Weight = xBorderWeight;

            ws.get_Range(sRange, sRange2).Borders[XlBordersIndex.xlEdgeBottom].LineStyle = XlLineStyle.xlContinuous;
            ws.get_Range(sRange, sRange2).Borders[XlBordersIndex.xlEdgeBottom].ColorIndex = XlColorIndex.xlColorIndexAutomatic;
            ws.get_Range(sRange, sRange2).Borders[XlBordersIndex.xlEdgeBottom].TintAndShade = 0;
            ws.get_Range(sRange, sRange2).Borders[XlBordersIndex.xlEdgeBottom].Weight = xBorderWeight;

            //20131108 이부분 수정하면 될거같음
            //ws.get_Range(sRange, sRange2).Borders[XlBordersIndex.xlEdgeBottom].LineStyle = XlLineStyle.xlContinuous;
            //ws.get_Range(sRange, sRange2).Borders[XlBordersIndex.xlEdgeBottom].Color = Color.Red;
            //ws.get_Range(sRange, sRange2).Borders[XlBordersIndex.xlEdgeBottom].Weight = XlBorderWeight.xlThick;

            //Selection.Borders(xlDiagonalDown).LineStyle = xlNone
            //Selection.Borders(xlDiagonalUp).LineStyle = xlNone


        }

        public void setLineOuter2(string sRange, string sRange2, int iWeight = 3)
        {
            XlBorderWeight xBorderWeight;

            switch (iWeight)
            {
                case 1:
                    xBorderWeight = XlBorderWeight.xlHairline;
                    break;
                case 2:
                    xBorderWeight = XlBorderWeight.xlThin;
                    break;
                case 3:
                    xBorderWeight = XlBorderWeight.xlMedium;
                    break;
                default:
                    xBorderWeight = XlBorderWeight.xlThick;
                    break;
            }

            ws.get_Range(sRange, sRange2).Borders[XlBordersIndex.xlEdgeLeft].LineStyle = XlLineStyle.xlContinuous;
            ws.get_Range(sRange, sRange2).Borders[XlBordersIndex.xlEdgeLeft].Color = Color.Red;
            ws.get_Range(sRange, sRange2).Borders[XlBordersIndex.xlEdgeLeft].TintAndShade = 0;
            ws.get_Range(sRange, sRange2).Borders[XlBordersIndex.xlEdgeLeft].Weight = xBorderWeight;

            ws.get_Range(sRange, sRange2).Borders[XlBordersIndex.xlEdgeRight].LineStyle = XlLineStyle.xlContinuous;
            ws.get_Range(sRange, sRange2).Borders[XlBordersIndex.xlEdgeRight].Color = Color.Red;
            ws.get_Range(sRange, sRange2).Borders[XlBordersIndex.xlEdgeRight].TintAndShade = 0;
            ws.get_Range(sRange, sRange2).Borders[XlBordersIndex.xlEdgeRight].Weight = xBorderWeight;

            ws.get_Range(sRange, sRange2).Borders[XlBordersIndex.xlEdgeTop].LineStyle = XlLineStyle.xlContinuous;
            ws.get_Range(sRange, sRange2).Borders[XlBordersIndex.xlEdgeTop].Color = Color.Red;
            ws.get_Range(sRange, sRange2).Borders[XlBordersIndex.xlEdgeTop].TintAndShade = 0;
            ws.get_Range(sRange, sRange2).Borders[XlBordersIndex.xlEdgeTop].Weight = xBorderWeight;

            ws.get_Range(sRange, sRange2).Borders[XlBordersIndex.xlEdgeBottom].LineStyle = XlLineStyle.xlContinuous;
            ws.get_Range(sRange, sRange2).Borders[XlBordersIndex.xlEdgeBottom].Color = Color.Red;
            ws.get_Range(sRange, sRange2).Borders[XlBordersIndex.xlEdgeBottom].TintAndShade = 0;
            ws.get_Range(sRange, sRange2).Borders[XlBordersIndex.xlEdgeBottom].Weight = xBorderWeight;

            //블루

            ws.get_Range(sRange, sRange2).Borders[XlBordersIndex.xlEdgeLeft].LineStyle = XlLineStyle.xlContinuous;
            ws.get_Range(sRange, sRange2).Borders[XlBordersIndex.xlEdgeLeft].Color = Color.Blue;
            ws.get_Range(sRange, sRange2).Borders[XlBordersIndex.xlEdgeLeft].TintAndShade = 0;
            ws.get_Range(sRange, sRange2).Borders[XlBordersIndex.xlEdgeLeft].Weight = xBorderWeight;

            ws.get_Range(sRange, sRange2).Borders[XlBordersIndex.xlEdgeRight].LineStyle = XlLineStyle.xlContinuous;
            ws.get_Range(sRange, sRange2).Borders[XlBordersIndex.xlEdgeRight].Color = Color.Blue;
            ws.get_Range(sRange, sRange2).Borders[XlBordersIndex.xlEdgeRight].TintAndShade = 0;
            ws.get_Range(sRange, sRange2).Borders[XlBordersIndex.xlEdgeRight].Weight = xBorderWeight;

            ws.get_Range(sRange, sRange2).Borders[XlBordersIndex.xlEdgeTop].LineStyle = XlLineStyle.xlContinuous;
            ws.get_Range(sRange, sRange2).Borders[XlBordersIndex.xlEdgeTop].Color = Color.Blue;
            ws.get_Range(sRange, sRange2).Borders[XlBordersIndex.xlEdgeTop].TintAndShade = 0;
            ws.get_Range(sRange, sRange2).Borders[XlBordersIndex.xlEdgeTop].Weight = xBorderWeight;

            ws.get_Range(sRange, sRange2).Borders[XlBordersIndex.xlEdgeBottom].LineStyle = XlLineStyle.xlContinuous;
            ws.get_Range(sRange, sRange2).Borders[XlBordersIndex.xlEdgeBottom].Color = Color.Blue;
            ws.get_Range(sRange, sRange2).Borders[XlBordersIndex.xlEdgeBottom].TintAndShade = 0;
            ws.get_Range(sRange, sRange2).Borders[XlBordersIndex.xlEdgeBottom].Weight = xBorderWeight;
        }

        public bool SaveAs(string sFilePath)
        {
            try
            {
                wb.SaveAs(sFilePath, XlFileFormat.xlWorkbookNormal);                
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return false;
            }

            return true;
        }


        public bool Quit()
        {
            try
            {
                //app.Quit();       

                System.Runtime.InteropServices.Marshal.ReleaseComObject(app);
                app = null;
                System.Runtime.InteropServices.Marshal.ReleaseComObject(wb);
                wb = null;
                System.Runtime.InteropServices.Marshal.ReleaseComObject(ws);
                ws = null;

                System.GC.Collect();
                System.GC.WaitForPendingFinalizers();
                System.GC.Collect();
                System.GC.WaitForPendingFinalizers();

                System.Diagnostics.Process[] PROC = System.Diagnostics.Process.GetProcessesByName("EXCEL");
                foreach (System.Diagnostics.Process PK in PROC)
                {
                    /// Process로 실행되고 타이틀이 없는 것을 이용하여 현재 EXCEL.EXE Process를 끝낸다.
                    if (PK.MainWindowTitle.Length == 0)
                        PK.Kill();
                }
            }
            catch  
            {
                return false;
            }

            return true;
        }





        /// <summary>
        /// 엑셀파일을 출력한다.
        /// </summary>
        /// <param name="sFilePath">엑셀파일의 경로</param>
        /// <param name="sSheetName">엑셀파일의 Sheet Name</param>
        public bool ExcelToPrint(string sFilePath, string sSheetName = "")
        {


            Application excelApp = new Application();
            Workbooks wbs_1 = excelApp.Workbooks;
            _Workbook wb_1 = null;
            Worksheet ws_1 = null;

            try
            {
                excelApp.DisplayAlerts = false;

                wb_1 = wbs_1.Open(sFilePath, 0, true, 5, "", "", true, XlPlatform.xlWindows, "\t", false, false, 0, true, true, true);

                if (sSheetName.Length > 0)
                {
                    ws_1 = (Worksheet)wb_1.Sheets[sSheetName];
                }
                else
                {
                    ws_1 = (Worksheet)wb_1.ActiveSheet;
                }

                //_Workbook.PrintOut(object From /// 인쇄를 시작할 페이지 번호입니다. 이 인수를 생략하면 인쇄가 처음부터 시작됩니다. 
                // , Object To     /// 인쇄할 마지막 페이지 번호입니다. 이 인수를 생략하면 마지막 페이지까지 인쇄됩니다. 
                // , object Copies    /// 인쇄할 매수입니다. 이 인수를 생략하면 한 부만 인쇄됩니다.
                // , object Preview   /// Microsoft Office Excel에서 개체를 인쇄하기 전에 인쇄 미리 보기를 호출하려면 true이고, 개체를 즉시 인쇄하려면 false(또는 생략)입니다.
                // , object ActivePrinter  /// 활성 프린터의 이름을 설정합니다
                // , object PrintToFile  /// 파일로 인쇄하는 경우 true입니다. PrToFileName이 지정되지 않으면 Excel에서 출력 파일의 이름을 입력하라는 메시지를 표시합니다.
                // , object Collate   /// 여러 장을 한 부씩 인쇄하는 경우 true입니다.
                // , object PrToFileName  /// PrintToFile이 true로 설정되면 이 인수는 인쇄할 파일의 이름을 지정합니다.
                //);
                object printer = Type.Missing; // "프린터명"
                ws_1.PrintOut(1, Type.Missing, 1, false, printer, false, false, Type.Missing);

                return true;

            }
            catch (Exception ex)
            {
                Console.WriteLine("ExcelCommon Err=" + ex.ToString());
                //MessageBox.Show(ex.Message);
                 
            }
            finally
            {
                QuitExcel(excelApp, wb_1, ws_1);
            }

            return false;
        }

        //[System.Runtime.InteropServices.DllImport("user32.dll")]///, SetLastError = true)]
        //static extern int GetWindowThreadProcessId(int hWnd, out int ProcessId);
        /// <summary>
        /// 엑셀과 관련된 변수들을 해제한다.
        /// </summary>
        /// <param name="excelApp"></param>
        /// <param name="wb"></param>
        /// <param name="ws"></param>
        private void QuitExcel(Application xlApp, _Workbook wb, Worksheet ws)
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();

            wb.Close(false);
            xlApp.Quit();

            // Clean up
            System.Runtime.InteropServices.Marshal.ReleaseComObject(ws);
            app = null;

            System.Runtime.InteropServices.Marshal.ReleaseComObject(wb);
            wb = null;

            System.Runtime.InteropServices.Marshal.ReleaseComObject(xlApp);
            ws = null;



            System.GC.Collect();
            System.GC.WaitForPendingFinalizers();
            System.GC.Collect();
            System.GC.WaitForPendingFinalizers();

            System.Diagnostics.Process[] PROC = System.Diagnostics.Process.GetProcessesByName("EXCEL");
            foreach (System.Diagnostics.Process PK in PROC)
            {
                /// Process로 실행되고 타이틀이 없는 것을 이용하여 현재 EXCEL.EXE Process를 끝낸다.
                if (PK.MainWindowTitle.Length == 0)
                    PK.Kill();
            }
        } 
    }
}
