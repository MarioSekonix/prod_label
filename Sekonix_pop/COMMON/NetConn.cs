using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace Sekonix_pop.COMMON
{
    class NetConn
    {
        //구조체 선언
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public struct NETRESOURCE
        {
            public uint dwScope;
            public uint dwType;
            public uint dwDisplayType;
            public uint dwUsage;
            public string IpLocalName;
            public string IpRemoteName;
            public string IpComment;
            public string IpProvider;
        }

        //API함수 선언
        [DllImport("mpr.dll", CharSet = CharSet.Auto)]
        public static extern int WNetUseConnection(
            IntPtr hwnOwner,
            [MarshalAs(UnmanagedType.Struct)] ref NETRESOURCE IpNetResource,
            string IpPassword,
            string IpUserID,
            uint dwFlags,
            StringBuilder IpAccessName,
            ref int IpBufferSize,
            out uint IpResult
        );

        //API 함수선언(공유해제)
        [DllImport("mpr.dll", EntryPoint = "WNetCancelConnection2", CharSet = CharSet.Auto)]
        public static extern int WNetCancelConnection2A(string IpName, int dwFlags, int fForce);

        /*
        //공유연결
        public int ConnectRemoteServer(string server)
        {
            int capacity = 64;
            uint resultFlags = 0;
            uint flags = 0;
            System.Text.StringBuilder sb = new System.Text.StringBuilder(capacity);
            NETRESOURCE ns = new NETRESOURCE();
            ns.dwType = 1;
            ns.IpLocalName = server;
            ns.IpProvider = null;
            int result = 0;
            if (server == @"\\118.32.222.220\z$")
            {
                result = WNetUseConnection(IntPtr.Zero, ref ns, "SekoNixA", "admin", flags, sb, ref capacity, out resultFlags);
            }
            else
            {
                result = WNetUseConnection(IntPtr.Zero, ref ns, "PASSWORD", "LOGINID", flags, sb, ref capacity, out resultFlags);
            }

            return result;

        }
        */

        //공유해제
        public void CancelRemoteServer(string server)
        {
            WNetCancelConnection2A(server, 1, 0);
        }


        public string setRemoteConnection(string networkDrive, string strRemoteConnectString, string strRemoteUserID, string strRemotePWD)
        {
            int capacity = 64;
            uint resultFlags = 0;
            uint flags = 0;
            try
            {

                //System.Diagnostics.Process.Start("cmd.exe", "net use * /delete /YES");
                System.Diagnostics.ProcessStartInfo proInfo = new System.Diagnostics.ProcessStartInfo();
                System.Diagnostics.Process pro = new System.Diagnostics.Process();

                //실행할 파일명 입력
                proInfo.FileName = @"cmd";

                proInfo.CreateNoWindow = true;  //cmd창 띄우기 --->  띄우기:true,  띄우지 않기 : false
                proInfo.UseShellExecute = false;

                //cmd 데이터 받기
                proInfo.RedirectStandardOutput = true;
                
                //cmd 데이터 보내기
                proInfo.RedirectStandardInput = true;

                //cmd오류내용 담기
                proInfo.RedirectStandardError = true;

                pro.StartInfo = proInfo;
                pro.Start();

                //cmd에 보낼 명령어를 입력
                pro.StandardInput.Write(@"net use * /delete /YES" + Environment.NewLine);
                pro.StandardInput.Close();

                //결과갑을 리턴
                string resultValue = pro.StandardOutput.ReadToEnd();
                pro.WaitForExit();
                pro.Close();

                //결과값 확인
                Console.WriteLine("CMD 결과값= " + resultValue);




                if ((strRemoteConnectString != "" || strRemoteConnectString != string.Empty) &&
                   (strRemoteUserID != "" || strRemoteUserID != string.Empty) &&
                   (strRemotePWD != "" || strRemotePWD != string.Empty))
                {
                    System.Text.StringBuilder sb = new System.Text.StringBuilder(capacity);
                    NETRESOURCE ns = new NETRESOURCE();
                    ns.dwType = 1;                                      // 공유 디스크
                    ns.IpLocalName = null;                              // 로컬 드라이브 지정하지 않음
                    ns.IpRemoteName = strRemoteConnectString;
                    ns.IpProvider = null;

                    //오류코드
                    // 0    : 정상접속
                    // 1326 : 사용자 아이디/패스워드 오류
                    // 1203 : 공유폴더 오류
                    // 85   : 이미 사용중
                    // 234  : capacity 사이즈 부족
                    // 1202 : RemoteName이 잘못되었을때


                    int result = WNetUseConnection(IntPtr.Zero, ref ns, strRemotePWD, strRemoteUserID, flags, sb, ref capacity, out resultFlags);

                    if (result == 0)
                    {
                        return "OK";
                    }
                    else if (result == 85)
                    {
                        return "OK";
                    }
                    else
                    {
                        return "FAIL";
                    }
                }
                else
                {
                    return "FAIL";
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("error= " + e.ToString());
                return "FAIL"; 
            }
        }
    }
}
