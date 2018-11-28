using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Sekonix_pop
{
    static class Program
    {
        /// <summary>
        /// 해당 응용 프로그램의 주 진입점입니다.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            Sekonix_pop.Properties.Settings.Default.Reload();

            if (Sekonix_pop.Properties.Settings.Default.APP_TYPE == "TERMINAL")
            {
                if (Sekonix_pop.Properties.Settings.Default.OP_CODE == "MA0080")
                {
                    Application.Run(new Page.POP.MA0000.MA0080());
                }
                     
                else
                {
                    MessageBox.Show("환결설정 파일을 확인하세요");
                    return;
                }
                

            }
            else
            {
                MessageBox.Show("환결설정 파일을 확인하세요");
                return;
            }
        }
    }
}
