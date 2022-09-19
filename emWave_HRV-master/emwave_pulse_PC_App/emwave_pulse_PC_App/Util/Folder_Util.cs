using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace emwave_pulse_PC_App.Util
{
    class Folder_Util
    {
        private void checkFolderExist(string fileName)
        {
            try
            {
                if (!Directory.Exists(fileName))
                {
                    Directory.CreateDirectory(fileName);
                }
            }
            catch (IOException)
            {
            }

        }
    }
}
