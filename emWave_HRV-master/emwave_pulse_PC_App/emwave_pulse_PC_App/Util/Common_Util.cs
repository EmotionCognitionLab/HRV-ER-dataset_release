using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using System.Diagnostics;

namespace emwave_pulse_PC_App.Util
{
    class Common_Util
    {
        public static bool CheckForInternetConnection()
        {
            try
            {
                using (var client = new WebClient())
                using (var stream = client.OpenRead("http://www.google.com"))
                {
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }

        private static void killemWave()
        {
            Process[] processList = Process.GetProcesses();
            foreach (Process theprocess in processList)
            {
                string processName = theprocess.ProcessName;
                processName = processName.ToLower();
                if (processName.Contains("emwavepc"))
                {
                    theprocess.Kill();
                    theprocess.CloseMainWindow();
                }
            }

            System.Threading.Thread.Sleep(500);
        }

        public static void safeKillemWave()
        {
            killemWave();
            killemWave();
        }

    }
}
