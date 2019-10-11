using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;           
using System.Text;
using System.Threading.Tasks;
using Excel = Microsoft.Office.Interop.Excel; 

namespace FantaBz
{
    class VotiExcelReader
    {

        public VotiExcelReader() {
            parseExcelFile();
        }

        private void parseExcelFile()
        {

            Excel.Application xlApp = new Excel.Application();
            Excel.Workbook xlWorkbook = xlApp.Workbooks.Open("C:\\voti.xlsx");
            Excel._Worksheet xlWorksheet = xlWorkbook.Sheets[1];
            Excel.Range xlRange = xlWorksheet.UsedRange;

            int rowCount = xlRange.Rows.Count;
            //int colCount = xlRange.Columns.Count;

            for (int i = 5; i <= rowCount; i++)
            {

                    PlayerRating pr = new PlayerRating();
                    string id = xlRange.Cells[i, 1].Value2.ToString();
                if (!id.Equals("ALL."))
                {
                    pr.Id = id;
                    pr.VotoGazzetta = xlRange.Cells[i, 7].Value2;
                    pr.GolFattiGazzetta = xlRange.Cells[i, 8].Value2;
                    pr.GolSubitiGazzetta = xlRange.Cells[i, 9].Value2;
                    pr.AutoRetiGazzetta = xlRange.Cells[i, 10].Value2;
                    pr.AssistGazzetta = xlRange.Cells[i, 11].Value2;

                    pr.VotoCorriere = xlRange.Cells[i, 12].Value2;
                    pr.GolFattiCorriere = xlRange.Cells[i, 13].Value2;
                    pr.GolSubitiCorriere = xlRange.Cells[i, 14].Value2;
                    pr.AutoRetiCorriere = xlRange.Cells[i, 15].Value2;
                    pr.AssistCorriere = xlRange.Cells[i, 16].Value2;

                    pr.Ammonizione = xlRange.Cells[i, 24].Value2;
                    pr.Esplusione = xlRange.Cells[i, 25].Value2;
                    pr.GolVittoria = xlRange.Cells[i, 26].Value2;

                    pr.RigoreSbagliato = xlRange.Cells[i, 28].Value2;
                    pr.RigoreParato = xlRange.Cells[i, 29].Value2; ;
                    pr.RigoreTrasformato = xlRange.Cells[i, 30].Value2;
                    PlayerRatingsList.addPlayerRating(pr);
                }

                
                //Console.Out.WriteLine(pr.toString());

            }





            //cleanup
            GC.Collect();
            GC.WaitForPendingFinalizers();
            Marshal.ReleaseComObject(xlRange);
            Marshal.ReleaseComObject(xlWorksheet);
            xlWorkbook.Close();
            Marshal.ReleaseComObject(xlWorkbook);
            xlApp.Quit();
            Marshal.ReleaseComObject(xlApp);
        }
    }
}
