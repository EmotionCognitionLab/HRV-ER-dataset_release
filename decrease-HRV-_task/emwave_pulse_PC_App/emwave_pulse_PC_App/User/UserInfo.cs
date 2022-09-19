using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace emwave_pulse_PC_App
{
    class UserInfo
    {
        private String _userName;
        private int _sessionTime;
        private double _threshold;

        private const string _defaultUserName = "User";
        private const int _defaultSessionTime = 180;
        private const double _defaultThreshold = 5;

        public String userName
        {
            set { _userName = value; }
            get { return _userName; }
        }

        public int sessionTime
        {
            set { _sessionTime = value; }
            get { return _sessionTime; }
        }

        public double threshold
        {
            set { _threshold = value; }
            get { return _threshold; }
        }

        public UserInfo() {
            _userName = readUser();
            _sessionTime = readSessionTime();
            _threshold = readThreshold();
        }

        public string readUser() {
            string currentFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string folder = System.IO.Path.Combine(currentFolder, "emWave_Pilot");
            string infoFolder = System.IO.Path.Combine(folder, "Info");

            string csvFileName = "info" + ".csv";
            string csvFilePath = Path.Combine(infoFolder, csvFileName);
            string separator = safeSeparator();

            if (!checkFolderExist(infoFolder))
            {
                StreamWriter dataWriter = new StreamWriter(csvFilePath, true, Encoding.Default);

                // Write a header
                dataWriter.WriteLine("User Name" + separator + "Session Time" + separator + "Threshold");
                dataWriter.Close();

                writeDefault();
                //return Environment.UserName;
                return _defaultUserName;
            }
            else {
                if (!File.Exists(csvFilePath)) {
                    StreamWriter dataWriter = new StreamWriter(csvFilePath, true, Encoding.Default);

                    // Write a header
                    dataWriter.WriteLine("User Name" + separator + "Session Time" + separator + "Threshold");
                    dataWriter.Close();

                    writeDefault();
                    //return Environment.UserName;
                    return _defaultUserName;
                }
                string[] lines = File.ReadAllLines(csvFilePath);
                string lastLine = File.ReadLines(csvFilePath).Last();
                if (lines.Length < 2)
                {
                    //return Environment.UserName;
                    return _defaultUserName;
                }
                else {
                    String[] words = lastLine.Split(new[] { ",", "," }, StringSplitOptions.None);
                    return words[0];
                    //return Environment.UserName;

                }
            }
        }

        public int readSessionTime()
        {
            string currentFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string folder = System.IO.Path.Combine(currentFolder, "emWave_Pilot");
            string infoFolder = System.IO.Path.Combine(folder, "Info");

            string csvFileName = "info" + ".csv";
            string csvFilePath = Path.Combine(infoFolder, csvFileName);
            string separator = safeSeparator();

            if (!checkFolderExist(infoFolder))
            {
                StreamWriter dataWriter = new StreamWriter(csvFilePath, true, Encoding.Default);

                // Write a header
                dataWriter.WriteLine("User Name" + separator + "Session Time" + separator + "Threshold");
                dataWriter.Close();
                return _defaultSessionTime;

            }
            else
            {

                string[] lines = File.ReadAllLines(csvFilePath);
                string lastLine = File.ReadLines(csvFilePath).Last();
                if (lines.Length < 2)
                {
                    return _defaultSessionTime;
                }
                else
                {
                    String[] words = lastLine.Split(new[] { ",", "," }, StringSplitOptions.None);
                    return Convert.ToInt16(words[1]);
                }
            }
        }

        public double readThreshold()
        {
            string currentFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string folder = System.IO.Path.Combine(currentFolder, "emWave_Pilot");
            string infoFolder = System.IO.Path.Combine(folder, "Info");

            string csvFileName = "info" + ".csv";
            string csvFilePath = Path.Combine(infoFolder, csvFileName);
            string separator = safeSeparator();

            if (!checkFolderExist(infoFolder))
            {
                StreamWriter dataWriter = new StreamWriter(csvFilePath, true, Encoding.Default);

                // Write a header
                dataWriter.WriteLine("User Name" + separator + "Session Time" + separator + "Threshold");
                dataWriter.Close();
                return _defaultThreshold;
            }
            else
            {

                string[] lines = File.ReadAllLines(csvFilePath);
                string lastLine = File.ReadLines(csvFilePath).Last();
                if (lines.Length < 2)
                {
                    return _defaultThreshold;
                }
                else
                {
                    String[] words = lastLine.Split(new[] { ",", "," }, StringSplitOptions.None);
                    return Convert.ToDouble(words[2]);
                }
            }
        }

        public void writeUserName(string userName) {
            _userName = userName;
            string currentFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string folder = System.IO.Path.Combine(currentFolder, "emWave_Pilot");
            string infoFolder = System.IO.Path.Combine(folder, "Info");

            string csvFileName = "info" + ".csv";
            string csvFilePath = Path.Combine(infoFolder, csvFileName);
            string separator = safeSeparator();

            if (!checkFolderExist(infoFolder))
            {
                StreamWriter dataWriter = new StreamWriter(csvFilePath, true, Encoding.Default);

                // Write a header
                dataWriter.WriteLine("User Name" + separator + "Session Time" + separator + "Threshold");
                dataWriter.Close();
            }
            else
            {

                string[] lines = File.ReadAllLines(csvFilePath);
                string lastLine = File.ReadLines(csvFilePath).Last();
                if (lines.Length < 2)
                {
                    StreamWriter dataWriter = new StreamWriter(csvFilePath, true, Encoding.Default);

                    // Write a header
                    dataWriter.WriteLine(_userName + separator + _sessionTime + separator + _threshold);
                    dataWriter.Close();
                }
                else
                {
                    String[] words = lastLine.Split(new[] { ",", "," }, StringSplitOptions.None);

                    StreamWriter dataWriter = new StreamWriter(csvFilePath, true, Encoding.Default);
                    dataWriter.WriteLine("User Name" + separator + "Session Time" + separator + "Threshold");
                
                    // Write a header
                    dataWriter.WriteLine(_userName + separator + _sessionTime + separator + _threshold);
                    dataWriter.Close();
                   
                }
            }
        
        }

        public void writeSessionName(int sessionTime)
        {
            _sessionTime = sessionTime;
            string currentFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string folder = System.IO.Path.Combine(currentFolder, "emWave_Pilot");
            string infoFolder = System.IO.Path.Combine(folder, "Info");
            string csvFileName = "info" + ".csv";
            string csvFilePath = Path.Combine(infoFolder, csvFileName);
            string separator = safeSeparator();

            if (!checkFolderExist(infoFolder))
            {
                StreamWriter dataWriter = new StreamWriter(csvFilePath, true, Encoding.Default);

                // Write a header
                dataWriter.WriteLine("User Name" + separator + "Session Time" + separator + "Threshold");
                dataWriter.Close();
            }
            else
            {

                string[] lines = File.ReadAllLines(csvFilePath);
                string lastLine = File.ReadLines(csvFilePath).Last();
                if (lines.Length < 2)
                {
                    StreamWriter dataWriter = new StreamWriter(csvFilePath, true, Encoding.Default);

                    // Write a header
                    dataWriter.WriteLine(_userName + separator + _sessionTime + separator + _threshold);
                    dataWriter.Close();
                }
                else
                {
                    String[] words = lastLine.Split(new[] { ",", "," }, StringSplitOptions.None);

                    StreamWriter dataWriter = new StreamWriter(csvFilePath, true, Encoding.Default);
                    dataWriter.WriteLine("User Name" + separator + "Session Time" + separator + "Threshold");
                
                    // Write a header
                    dataWriter.WriteLine(_userName + separator + _sessionTime + separator + _threshold);
                    dataWriter.Close();

                }
            }

        }

        public void writeThreshold(double threshold)
        {
            _threshold = threshold;
            string currentFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string folder = System.IO.Path.Combine(currentFolder, "emWave_Pilot");
            string infoFolder = System.IO.Path.Combine(folder, "Info");
            string csvFileName = "info" + ".csv";
            string csvFilePath = Path.Combine(infoFolder, csvFileName);
            string separator = safeSeparator();

            if (!checkFolderExist(infoFolder))
            {
                StreamWriter dataWriter = new StreamWriter(csvFilePath, true, Encoding.Default);

                // Write a header
                dataWriter.WriteLine("User Name" + separator + "Session Time" + separator + "Threshold");
                dataWriter.Close();
            }
            else
            {

                string[] lines = File.ReadAllLines(csvFilePath);
                string lastLine = File.ReadLines(csvFilePath).Last();
                if (lines.Length < 2)
                {
                    StreamWriter dataWriter = new StreamWriter(csvFilePath, true, Encoding.Default);

                    // Write a header
                    dataWriter.WriteLine(_userName + separator + _sessionTime + separator + _threshold);
                    dataWriter.Close();
                }
                else
                {
                    String[] words = lastLine.Split(new[] { ",", "," }, StringSplitOptions.None);

                    StreamWriter dataWriter = new StreamWriter(csvFilePath, true, Encoding.Default);
                    dataWriter.WriteLine("User Name" + separator + "Session Time" + separator + "Threshold");

                    // Write a header
                    dataWriter.WriteLine(_userName + separator + _sessionTime + separator + _threshold);
                    dataWriter.Close();

                }
            }

        }

        public void writeDefault()
        {
            string currentFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string folder = System.IO.Path.Combine(currentFolder, "emWave_Pilot");
            string infoFolder = System.IO.Path.Combine(folder, "Info");

            string csvFileName = "info" + ".csv";
            string csvFilePath = Path.Combine(infoFolder, csvFileName);
            string separator = safeSeparator();
            
            StreamWriter dataWriter = new StreamWriter(csvFilePath, true, Encoding.Default);
            // Write a header
            dataWriter.WriteLine(_defaultUserName + separator + _defaultSessionTime + separator + _defaultThreshold);
            dataWriter.Close();

        }

        private Boolean checkFolderExist(string fileName)
        {
            try
            {
                if (!Directory.Exists(fileName))
                {
                    Directory.CreateDirectory(fileName);
                    return false;
                }
                
            }
            catch (IOException)
            {
            }

            return true;

        }

        private string safeSeparator()
        {
            double testValue = 3.14;
            string testText = testValue.ToString();

            if (testText.IndexOf(',') < 0)
                return ",";

            return ";";
        }


    }
}
