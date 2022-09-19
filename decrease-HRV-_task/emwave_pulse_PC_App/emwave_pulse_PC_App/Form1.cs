using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using System.IO;
using System.Numerics;
using ZedGraph;
using System.Diagnostics;
using emwave_pulse_PC_App.AmazonUpload;
using emwave_pulse_PC_App.Util;

namespace emwave_pulse_PC_App
{   
   public partial class emwave_app : Form
   {
      private static Boolean tcp_Error = false;
      private int tcp_data_Alive = 0;
      private Boolean is_Connect_TCP = false;
      private TcpListener pulse_sever = null;
      private static Int32 emwave_port = 20480;
      private string localhost_IP = "127.0.0.1";
      private static StateObject state = new StateObject();
      
      // ManualResetEvent instances signal completion.  
      private static ManualResetEvent connectDone =
          new ManualResetEvent(false);
      private static ManualResetEvent sendDone =
          new ManualResetEvent(false);
      private static ManualResetEvent receiveDone =
          new ManualResetEvent(false);

      // The response from the remote device.  
      private static String response = String.Empty;

      private static Boolean isNewDataComing = false;
      private static Boolean isLiveDataComing = false;
      private static Boolean isStartCalEP = false;
      private Object workThreadLock = new Object();   //Thread synchronization

      private PPG_Data dataBuffer_ = new PPG_Data();  // Stripchart data
      private HR_Analysis_Data hrAnalysisBuffer_ = new HR_Analysis_Data();
      private emWaveData emWaveDataBuffer_ = new emWaveData();

      private static double currentTime, currentIBI, lastIBI, liveIBI;
      private static double[] tempIBI = new double[5];
      private static Complex[] fftIBI = new Complex[128];
      private static int fftIBIIndex = 0;
      private static int currentHR, currentEP, currentAS, currentMdLowEP, currentMdHighEP, currentS;
      private static int lastEPIndex = 0;

      private bool isFirstFrame_ = true;              // Is first frame after boot
      private const int graphXRange_ = 135;           // 120 seconds
      private int gui_timerCounter = 0;

      private UserInfo userInfo;
      private long session_Time;
      private long session_Spend_On_Session_Time;
      private long init_Session_Time;
      private string session_Name;
      private Int16 session_Attempt;
      private static Socket client;

      private Boolean isUploading = false;
      private Boolean isFirstFrame = true;
      private Boolean isSessionFinished = false;
      public int numAverageHR = 0;
      public List<double> axisUpdateDataBuffer = new List<double>();
      private double maxGraphHR = 0, minGraphHR = 9999;
      private int timeSinceLastNewData = 0;
      private Boolean isSensorLoseContactError = false;
      private long milliSeconds = 0, startMiliseconds = 0;
      private double currentSystemTime = 0;

      private int numberOfCalm = 0;
      private double averageCalm = 0;

      public emwave_app()
      {
         userInfo = new UserInfo();
        
         InitializeComponent();
         acc_Calm_Txt.Text  = "";
         calm_Txt.Text      = "";
         if ((userInfo.sessionTime - session_Time) % 60 < 10)
         {
             session_time_txt.Text = Convert.ToString((userInfo.sessionTime) / 60) + ":0" + Convert.ToString((userInfo.sessionTime) % 60);
         }
         else {
             session_time_txt.Text = Convert.ToString((userInfo.sessionTime) / 60) + ":" + Convert.ToString((userInfo.sessionTime) % 60);
         }
         
         CreateStripChart();
         
         string currentFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
         Console.WriteLine(currentFolder);

         string documentFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
         string emWaveDocumentFolder = System.IO.Path.Combine(documentFolder, "emWave");
         string dbFile = System.IO.Path.Combine(emWaveDocumentFolder, "emWave.emdb");

         Console.WriteLine(dbFile);

         string connStr = "Data Source=" + dbFile + ";Version=3;Synchronous=Off;UTF8Encoding=True;";

         if (File.Exists(dbFile)) {
             Console.WriteLine(dbFile);
         }

         string folder = System.IO.Path.Combine(currentFolder, "emWave_Pilot");

         for (int i = 0; i < 128; i++) {
             fftIBI[i] = new Complex(0, 0);
         }

      }

      private void initialSessionParams()
      {
          string currentFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
          string folder = System.IO.Path.Combine(currentFolder, "emWave_Pilot");
          string logFolder = System.IO.Path.Combine(folder, "Log");

          checkFolderExist(logFolder);
          string separator = safeSeparator();

          string csvFileName = "log" + ".csv";
          string csvFilePath = Path.Combine(logFolder, csvFileName);

          // Write Column Entry if this is the first time running the software
          if (!File.Exists(csvFilePath))
          {
              File.WriteAllText(csvFilePath, "User" + separator + "Session Name" + separator + 
                  "Time Spent On This Attempt" + separator + 
                  "Attempt" + separator +
                  "Finish Status" + separator + 
                  "Session Time" + separator + 
                  "Threshold" + separator +
                  "Date" + separator +
                  "Ave Calmness" + separator +
                  "Time Spending for the Session"+ "\n");
              session_Time = 0;
              init_Session_Time = session_Time;
              session_Attempt = 0;
              //session_Name = DateTime.Now.ToString("MM-dd-yyyy") 
              //    + "-" + DateTime.Now.ToString("HH-mm-ss");
              int i = 1;
              session_Name = i.ToString();
          }
          // Restore last Session or start a new session
          else {
              string lastLine = File.ReadLines(csvFilePath).Last();
              if (lastLine != "") {
                  if (lastLine.Contains("Not"))
                  {
                      if (MessageBox.Show("Do you want to continue your last session?", "Message", MessageBoxButtons.YesNo) == DialogResult.Yes)
                      {
                          String[] words = lastLine.Split(new[] { ",", ",", ",", ",", ",", ",", ",", ",", "," }, StringSplitOptions.None);
                          session_Time = Convert.ToInt16(words[2]);
                          
                          userInfo.sessionTime = Convert.ToInt16(words[5]);
                          userInfo.userName = words[0];
                          //userInfo.userName = Environment.UserName;
                          userInfo.threshold = Convert.ToDouble(words[6]);
                          session_Attempt = (Int16) (Convert.ToInt16(words[3]) + 1);
                          session_Name = words[1];
                          session_Spend_On_Session_Time = Convert.ToInt16(words[9]);
                          session_Time = session_Spend_On_Session_Time;

                          init_Session_Time = session_Spend_On_Session_Time;

                          /*
                          if (userInfo.sessionTime - session_Time < 60)
                          {
                              userInfo.sessionTime = (int)session_Time + 60;
                          }*/
                      }
                      else {
                          if (userInfo.sessionTime < 60)
                          {
                              userInfo.sessionTime = 60;
                          }
                          session_Time = 0;
                          init_Session_Time = session_Time;
                          session_Attempt = 0;
                          //session_Name = DateTime.Now.ToString("MM-dd-yyyy")
                          //                + "-" + DateTime.Now.ToString("HH-mm-ss");
                          String[] words = lastLine.Split(new[] { ",", ",", ",", ",", ",", ",", "," }, StringSplitOptions.None);
                          
                          int i = Int32.Parse(words[1]) + 1;
                          session_Name = i.ToString();
                      }
                  }
                  else if (lastLine.Contains("Status"))
                  {
                      if (userInfo.sessionTime < 60)
                      {
                          userInfo.sessionTime = 60;
                      }
                      session_Time = 0;
                      init_Session_Time = session_Time;
                      //session_Name = DateTime.Now.ToString("MM-dd-yyyy")
                      //                  + "-" + DateTime.Now.ToString("HH-mm-ss");
                      int i = 1;
                      session_Name = i.ToString();
                  }
                  else {
                      if (userInfo.sessionTime < 60)
                      {
                          userInfo.sessionTime = 60;
                      }
                      session_Time = 0;
                      init_Session_Time = session_Time;
                      session_Attempt = 0;
                      //session_Name = DateTime.Now.ToString("MM-dd-yyyy")
                      //               + "-" + DateTime.Now.ToString("HH-mm-ss");
                      String[] words = lastLine.Split(new[] { ",", ",", ",", ",", ",", ",", "," }, StringSplitOptions.None);

                      int i = Int32.Parse(words[1]) + 1;
                      session_Name = i.ToString();
                  
                  }
              }
          }

      }

      public void Test_FFT(){

        double[] inputReal = new double[16];
        double[] inputImg = new double[16];

        for (int i = 0; i < 4; i++) {
            inputReal[i] = 1;
        }

        for (int i = 0; i < 8; i++)
        {
            inputReal[i+8] = i + 1;
        }

        Complex.FFT(1, 4, inputReal, inputImg);

        for (int i = 0; i < 16; i++)
        {
            Console.WriteLine(inputReal[i] + " + " + inputImg[i]);
        }

        //FFT_Lib.FFT(input);

      }
      
      private void Connect_OnListener(object sender, EventArgs e)
      {
          initialSessionParams();
          Byte[] bytes = new Byte[256];
          
          if (is_Connect_TCP == true)
          {
              client.Shutdown(SocketShutdown.Both);
              client.Close();
              start_btn.Text = "Start";
              guiTimer_.Stop();
              is_Connect_TCP = false;
          }
          else
          {
              start_btn.Text = "Stop";
              start_btn.Visible = false;
              start_btn.Enabled = false;
              Initialize_TCP(localhost_IP, emwave_port);
              guiTimer_.Start();
              
          }
      }

      public void Initialize_TCP(string IP, Int32 port)
      {

         IPHostEntry ipHostInfo = Dns.Resolve(Dns.GetHostName());
         IPAddress ipAddress = ipHostInfo.AddressList[0];
         IPEndPoint localEndPoint = new IPEndPoint(ipAddress, port);


         client = new Socket(AddressFamily.InterNetwork,
            SocketType.Stream, ProtocolType.Tcp);

         // Connect to the remote endpoint.  
         client.BeginConnect(localEndPoint,
             new AsyncCallback(ConnectCallback), client);
         connectDone.WaitOne();
          
         Receive(client);
         is_Connect_TCP = true;
         
      }

      private static void ConnectCallback(IAsyncResult ar)
      {
          if (ar.IsCompleted) {
              try
              {
                  // Retrieve the socket from the state object.  
                  Socket client = (Socket)ar.AsyncState;

                  // Complete the connection.  
                  client.EndConnect(ar);

                  Console.WriteLine("Socket connected to {0}",
                      client.RemoteEndPoint.ToString());

                  // Signal that the connection has been made.  
                  connectDone.Set();
              }
              catch (Exception e)
              {
                  client.Close();
                  Console.WriteLine(e.ToString());
                  tcp_Error = true;
                  Application.Exit();
              }         
          }          
      }

      private static void Receive(Socket client)
      {
          try
          {
              // Create the state object.  
              
              state.workSocket = client;

              ASCIIEncoding asen = new ASCIIEncoding();

              client.Send(asen.GetBytes("<CMD ID=2 />"));

              // Begin receiving the data from the remote device.  
              client.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
                  new AsyncCallback(ReceiveCallback), state);
          }
          catch (Exception e)
          {
              Console.WriteLine(e.ToString());
          }
      }

      private static void ReceiveCallback(IAsyncResult ar)
      {
          try
          {
              // Retrieve the state object and the client socket   
              // from the asynchronous state object.  
              StateObject state = (StateObject)ar.AsyncState;
              Socket client = state.workSocket;

              // Read data from the remote device.  
              int bytesRead = client.EndReceive(ar);
              String TCP_Data = null;

              if (bytesRead > 0)
              {
                  // There might be more data, so store the data received so far.  
                  //state.sb.Append(Encoding.ASCII.GetString(state.buffer, 0, bytesRead));
                  TCP_Data = Encoding.ASCII.GetString(state.buffer, 0, bytesRead);
                  
                  if (TCP_Data != "") {

                      Console.Write(Encoding.ASCII.GetString(state.buffer, 0, bytesRead));

                      if (TCP_Data.Contains("D01")) {

                          //<D01 NAME="tiantiaf Feng" LVL="1" SSTAT="2" STIME="11500" S="0" AS="0" EP="0" IBI="894" ART="FALSE" HR="0" />
                          //Console.Write(Encoding.ASCII.GetString(state.buffer, 0, bytesRead));
                          String timestamp, hr, IBI, EP, AS, s;

                          String[] tempWords;
                          String[] words = TCP_Data.Split(new[] { "STIME=\"", "S=\"", "AS=\"", "EP=\"", "IBI=\"", "HR=\"" }, StringSplitOptions.None);

                          tempWords = words[1].Split(new[] { "\"" }, StringSplitOptions.None);
                          timestamp = tempWords[0];
                          //Console.Write("Timestamp: " + timestamp);

                          tempWords = words[2].Split(new[] { "\"" }, StringSplitOptions.None);
                          s = tempWords[0];

                          tempWords = words[3].Split(new[] { "\"" }, StringSplitOptions.None);
                          AS = tempWords[0];
                          //Console.Write("Timestamp: " + timestamp);

                          tempWords = words[4].Split(new[] { "\"" }, StringSplitOptions.None);
                          EP = tempWords[0];
                          //Console.Write("Timestamp: " + timestamp);

                          tempWords = words[5].Split(new[] { "\"" }, StringSplitOptions.None);
                          IBI = tempWords[0];
                          //Console.Write("IBI: " + IBI);

                          tempWords = words[6].Split(new[] { "\"" }, StringSplitOptions.None);
                          hr = tempWords[0];
                          //Console.Write(" HR: " + hr + "\n");

                          currentTime = Convert.ToDouble(timestamp);
                          currentHR = Convert.ToInt16(hr);
                          currentIBI = Convert.ToDouble(IBI);
                          currentAS = Convert.ToInt16(AS);
                          currentEP = Convert.ToInt16(EP);
                          currentS = Convert.ToInt16(s);

                          /* Moving Buffer for IBI */
                          for (int i = 0; i < 4; i++)
                            tempIBI[i] = tempIBI[i+1];
                          tempIBI[4] = currentIBI;

                          
                          isNewDataComing = true;

                      }
                      else if (TCP_Data.Contains("</IBI>")) {
                          //<IBI> 636 </IBI>
                          //Console.Write(Encoding.ASCII.GetString(state.buffer, 0, bytesRead));
                          String IBI;
                          String[] tempWords;
                          String[] words = TCP_Data.Split(new[] { "<IBI> " }, StringSplitOptions.None);
                          tempWords = words[1].Split(new[] { " <" }, StringSplitOptions.None);
                          IBI = tempWords[0];
                          liveIBI = Convert.ToDouble(IBI + "\n");
                          isLiveDataComing = true;
                      }
                      
                  }
                  
                  // Get the rest of the data.  
                  client.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
                      new AsyncCallback(ReceiveCallback), state);
              }
              else
              {
                  // All the data has arrived; put it in response.  
                  if (state.sb.Length > 1)
                  {
                      response = state.sb.ToString();
                  }
                  // Signal that all bytes have been received.  
                  receiveDone.Set();
              }
          }
          catch (Exception e)
          {
              Console.WriteLine(e.ToString());
          }
      }

      private static void calculateEP(Boolean isArrayCopy) {
          
          Complex[] tempFFTIBI = new Complex[128];
          Array.Copy(fftIBI, 0, tempFFTIBI, 0, 128);
          
          double[] tempIBIRe = new double[128];
          double[] tempIBIIm = new double[128];

          for (int i = 0; i < 128; i++) {
              tempIBIRe[i] = tempFFTIBI[i].re;
              tempIBIIm[i] = tempFFTIBI[i].im;
          }

          Complex.FFT(1, 7, tempIBIRe, tempIBIIm);

          double[] FFTPower = new double[128];
          
          for (int i = 3; i < 17; i++)
          {
              FFTPower[i] = (Math.Pow(tempIBIRe[i], 2) + Math.Pow(tempIBIIm[i], 2)) / 128;

          }
          double maxPowerEP = FFTPower.Max();
          int maxIndexEP = FFTPower.ToList().IndexOf(maxPowerEP);

          for (int i = 1; i < 2; i++)
          {
              FFTPower[i] = (Math.Pow(tempIBIRe[i], 2) + Math.Pow(tempIBIIm[i], 2)) / 128;
          }

          currentMdLowEP = calMdLowEP(maxPowerEP, maxIndexEP, FFTPower);
          
          maxPowerEP = 0;
          maxIndexEP = 0;

          for (int i = 0; i < 17; i++) {
              FFTPower[i] = 0;
          }

          for (int i = 17; i < 26; i++)
          {
              FFTPower[i] = (Math.Pow(tempIBIRe[i], 2) + Math.Pow(tempIBIIm[i], 2)) / 128;
          }

          maxPowerEP = FFTPower.Max();
          maxIndexEP = FFTPower.ToList().IndexOf(maxPowerEP);
          currentMdHighEP = calMdHighEP(maxPowerEP, maxIndexEP, FFTPower) / 2;

          Console.Write("EP = " + currentMdLowEP + "\n");

          if (isArrayCopy)
          {
              fftIBIIndex -= 12;
              Array.Copy(fftIBI, 10, fftIBI, 0, 118);
          }
          
      }

      private static int calMdLowEP(double _maxPower, int _maxIndex, double[] _FFTPower)
      {
          double belowPower = 0, abovePower = 0;

          _maxPower = _maxPower + _FFTPower[_maxIndex - 1] + _FFTPower[_maxIndex + 1];

          if (_maxIndex >= 15)
          {
              for (int i = 2; i < _maxIndex - 1; i++)
              {
                  belowPower += _FFTPower[i];
              }
              if (belowPower < 1)
                  belowPower = 1;
              return (int)(_maxPower / belowPower);

          }
          else if (_maxIndex == 2)
          {
              _maxPower = _maxPower - _FFTPower[_maxIndex - 1];
              for (int i = _maxIndex + 2; i < 17; i++)
              {
                  abovePower += _FFTPower[i];
              }
              if (abovePower < 1)
                  abovePower = 1;

              return (int)(_maxPower / abovePower);
          } else
          {
              for (int i = 1; i < _maxIndex - 1; i++)
              {
                  belowPower += _FFTPower[i];
              }
              
              for (int i = _maxIndex + 2; i < 17; i++)
              {
                  abovePower += _FFTPower[i];
              }

              if (belowPower < 1)
                  belowPower = 1;

              return (int)((_maxPower / abovePower) * (_maxPower / belowPower));

          }

      }

      private static int calMdHighEP(double _maxPower, int _maxIndex, double[] _FFTPower)
      {
          double belowPower = 0, abovePower = 0;

          _maxPower = _maxPower + _FFTPower[_maxIndex - 1] + _FFTPower[_maxIndex + 1];

          if (_maxIndex >= 24)
          {
              for (int i = 17; i < _maxIndex - 1; i++)
              {
                  belowPower += _FFTPower[i];
              }
              if (belowPower < 1)
                  belowPower = 1;

              return (int)(_maxPower / belowPower);
          }
          else if (_maxIndex <= 18)              
          {
              for (int i = _maxIndex + 2; i < 26; i++)
              {
                  abovePower += _FFTPower[i];
              }
              if (abovePower < 1)
                  abovePower = 1;

              return (int)(_maxPower / abovePower);
          } else {
              for (int i = 17; i < _maxIndex - 1; i++)
              {
                  belowPower += _FFTPower[i];
              }

              for (int i = _maxIndex + 2; i < 26; i++)
              {
                  abovePower += _FFTPower[i];
              }

              if (abovePower < 1)
                  abovePower = 1;

              if (belowPower < 1)
                  belowPower = 1;

              return (int)((_maxPower / abovePower) * (_maxPower / belowPower));

          }

      }

      public void Stop_TCP_Server() {
         pulse_sever.Stop();
         pulse_sever = null;
      }

      void CreateStripChart()
      {
          // This is to remove all plots
          graph_.GraphPane.CurveList.Clear();

          // GraphPane object holds one or more Curve objects (or plots)
          GraphPane myPane = graph_.GraphPane;

          //Lables
          myPane.XAxis.Title.Text = "Time (s)";
          myPane.YAxis.Title.Text = "BPM";
          myPane.Title.IsVisible = false;
          myPane.Legend.IsVisible = false;

          //Set scale
          myPane.XAxis.Scale.Max = 125; //Show 120s of data
          myPane.XAxis.Scale.Min = -10;

          //Set scale
          myPane.YAxis.Scale.Max = 100; //Valid range
          myPane.YAxis.Scale.Min = 65;

          // Refeshes the plot.
          graph_.AxisChange();
          graph_.Invalidate();
          graph_.Refresh();
          graph_.AutoScroll = false;
          graph_.IsEnableZoom = false;
      }

      private void updateGUITimer(object sender, EventArgs e)
      {
          try
          {
              lock (workThreadLock)
              {        

                  if (isNewDataComing) {
                      
                      isFirstFrameFun();

                      milliSeconds = (long)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalMilliseconds - startMiliseconds;
                      currentSystemTime = milliSeconds / 1000;

                      numAverageHR = numAverageHR + 1;

                      if (numAverageHR > 6)
                      {
                          // Add HRV to a data buffer for updating max and min of Y-Axis
                          addDataToGraph((double)(session_Time + gui_timerCounter * 0.05));
                          /* Moving Buffer for FFT */
                          fftIBI[fftIBIIndex++] = new Complex(60 * 1000 / currentIBI, 0);


                          if (fftIBIIndex >= 128) {
                              calculateEP(true);
                              isStartCalEP = true;
                          }
                          else if (fftIBIIndex < 110 && fftIBIIndex > 25 && (fftIBIIndex - lastEPIndex) > 11)
                          {
                              lastEPIndex = fftIBIIndex;
                              calculateEP(false);
                              isStartCalEP = true;                             
                          }

                          if (currentTime == 0 && currentHR == 0 && currentIBI == 0
                              && currentAS == 0 && currentEP == 0)
                          {
                              tcp_Error = true;
                              Application.Exit();
                          }

                          //emWaveDataBuffer_.AddData(currentHR, currentEP, currentAS, currentS, currentIBI, (double)(session_Time + gui_timerCounter * 0.05));
                          emWaveDataBuffer_.AddData(currentHR, currentEP, currentAS, currentS, currentIBI, (double)(session_Time + gui_timerCounter * 0.05));
                       
                          
                      }
                      isNewDataComing = false;
                      timeSinceLastNewData = 0;
                      
                  }

                  if (isLiveDataComing) {
                      addHRAnalysisData((double)(session_Time + gui_timerCounter * 0.05));
                      isLiveDataComing = false;
                  }
                  updateTime();
                  if (timeSinceLastNewData > 200)
                  {
                      isSensorLoseContactError = true;
                      guiTimer_.Stop();
                      if (MessageBox.Show("USB unplugged or sensor is placed incorrectly! Please check setup.",
                        "Error", MessageBoxButtons.OK) == DialogResult.OK) {
                            
                            if (!isFirstFrame && isStartCalEP)
                                savingDataAndLog(true);

                            if (MessageBox.Show("Your average calmness score in this session is: " + averageCalm.ToString("f2"),
                                "Session Summary!",
                                MessageBoxButtons.OK) == DialogResult.OK){
                                Common_Util.safeKillemWave();
                                Environment.Exit(0);
                            }                           
                      }
                      
                  }
                  timeSinceLastNewData++;                  
              }
          }
          catch (Exception) { }
      }

      public void AddData(double time, double[] measurements)
      {
          dataBuffer_.AddData(measurements, time);
          
          GraphPane graphPane = graph_.GraphPane;

          while (graphPane.CurveList.Count < measurements.Length)
          {
              string name = "Sensor " + (graphPane.CurveList.Count + 1).ToString();
              LineItem myCurve = new LineItem(
                  name,
                  dataBuffer_.data[graphPane.CurveList.Count],
                  Color.Blue,
                  SymbolType.None,
                  3.0f);
              graphPane.CurveList.Add(myCurve);
          }

          for (int i = 0; i < measurements.Length; i++)
          {
              graphPane.CurveList[i].Points = dataBuffer_.data[i];
          }

          // This is to update the max and min value
          if (isFirstFrame_)
          {
              graphPane.XAxis.Scale.Min = dataBuffer_.MostRecentTime;
              graphPane.XAxis.Scale.Max = graphPane.XAxis.Scale.Min + graphXRange_;
              isFirstFrame_ = false;
          }

          if (dataBuffer_.MostRecentTime >= graphPane.XAxis.Scale.Max)
          {
              graphPane.XAxis.Scale.Max = dataBuffer_.MostRecentTime;
              graphPane.XAxis.Scale.Min = graphPane.XAxis.Scale.Max - graphXRange_;
          }
          
          graph_.Refresh();
          graph_.AxisChange();
      }

      private void exitBtn(object sender, EventArgs e)
      {
          if (!isUploading)
          {
              Application.Exit();
          }
          else {
              Common_Util.safeKillemWave();
              Environment.Exit(0);
          }
          
      }

      private string safeSeparator()
      {
          double testValue = 3.14;
          string testText = testValue.ToString();

          if (testText.IndexOf(',') < 0)
              return ",";

          return ";";
      }

      private void exit_Click(object sender, FormClosingEventArgs e)
      {
          if (!tcp_Error)
          {
              if (isSensorLoseContactError)
              {
                  if (MessageBox.Show("USB unplugged or sensor is placed incorrectly! Please check setup.",
                        "Error", MessageBoxButtons.OK) == DialogResult.OK)
                  {
                      if (!isFirstFrame && isStartCalEP)
                        savingDataAndLog(true);

                      if (MessageBox.Show("Your average calmness score in this session is: " + averageCalm.ToString("f2"), 
                          "Session Summary!",
                          MessageBoxButtons.OK) == DialogResult.OK) {
                        Common_Util.safeKillemWave();
                      }
                  }
              }
              else if (isSessionFinished) {
                  if (MessageBox.Show("Session Finished!", "Congratualations", MessageBoxButtons.OK) == DialogResult.OK)
                  {
                      if (!isFirstFrame && isStartCalEP)
                      {
                          savingDataAndLog(false);
                          if (MessageBox.Show("Your average calmness score in this session is: " + averageCalm.ToString("f2"),
                                "Session Summary!",
                                MessageBoxButtons.OK) == DialogResult.OK)
                          {
                              tryUploadData(e);
                          }
                          
                      }
                  }
              }
              else
              {
                  if (MessageBox.Show("Do you want to quit the session?", "Message", MessageBoxButtons.YesNo) == DialogResult.Yes)
                  {
                      guiTimer_.Stop();
                      if (!isFirstFrame && isStartCalEP){
                          savingDataAndLog(false);
                      }

                      if (MessageBox.Show("Your average calmness score in this session is: " + averageCalm.ToString("f2"),
                            "Session Summary!",
                            MessageBoxButtons.OK) == DialogResult.OK)
                      {
                           tryUploadData(e);
                      }
                     
                  }
                  else
                  {
                      e.Cancel = true;
                  }
              }
          }
          else {

              if (MessageBox.Show("USB unplugged or sensor is placed incorrectly! Please check setup.",
                        "Error", MessageBoxButtons.OK) == DialogResult.OK)
              {
                  if (!isFirstFrame && isStartCalEP)
                      savingDataAndLog(true);

                  if (MessageBox.Show("Your average calmness score in this session is: " + averageCalm.ToString("f2"),
                          "Session Summary!",
                          MessageBoxButtons.OK) == DialogResult.OK)
                  {
                        Common_Util.safeKillemWave();
                        Environment.Exit(0);
                  }
                  
                  
              }
              
          }
            
      }

      private void tryUploadData(FormClosingEventArgs e)
      {
          isUploading = true;
          int filesLength;

          string currentFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
          string folder = System.IO.Path.Combine(currentFolder, "emWave_Pilot");
          string dataFolder = System.IO.Path.Combine(folder, "Data");

          checkFolderExist(dataFolder);

          DirectoryInfo dataFolderDi = new DirectoryInfo(dataFolder);//Assuming Test is your Folder
          FileInfo[] dataFiles = dataFolderDi.GetFiles("*.csv"); //Getting Text files

          string emdataFolder = System.IO.Path.Combine(folder, "emWaveData");

          checkFolderExist(emdataFolder);

          DirectoryInfo emdataFolderDi = new DirectoryInfo(emdataFolder);//Assuming Test is your Folder
          FileInfo[] emDataFiles = emdataFolderDi.GetFiles("*.csv"); //Getting Text files

          filesLength = dataFiles.Length + emDataFiles.Length;

          // If there is internet connection
          if (Common_Util.CheckForInternetConnection() && filesLength > 0)
          {
              if (MessageBox.Show("You have data to upload, are you willing to upload now?", "Message", MessageBoxButtons.YesNo) == DialogResult.Yes)
              {
                  Upload_emWaveData uploadData_ = new Upload_emWaveData(userInfo.userName);
                  e.Cancel = true;
                  showWait();
                  //upLoadTimer_.Start();
              }
              else
              {
                  Common_Util.safeKillemWave();
              }
          }
          else
          {
              Common_Util.safeKillemWave();
          }
      
      }

      private void showWait(){
          if (MessageBox.Show("Transmitting at the moment, please wait...", "Message", MessageBoxButtons.OK) == DialogResult.Yes)
          {
              showWait();
          }
      }

      private static void SendCallBack(IAsyncResult ar)
      {
          try
          {
              // Retrieve the state object and the client socket   
              // from the asynchronous state object.  
              StateObject state = (StateObject)ar.AsyncState;
              Socket client = state.workSocket;

              // Read data from the remote device.  
              int bytesRead = client.EndSend(ar);
              sendDone.Set();
          }
          catch (Exception e)
          {
              Console.WriteLine(e.ToString());
          }
      }

      private void savingDataAndLog(Boolean error) {

          string currentFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
          string folder = System.IO.Path.Combine(currentFolder, "emWave_Pilot");
          string logFolder = System.IO.Path.Combine(currentFolder, "Log");

          if (is_Connect_TCP == true && isFirstFrame == false && isSessionFinished == false)
          {

              if (error)
              {
                  saveDataToCSV("Disconnect-" + session_Name);
                  saveemWaveDataToAppCSV("Disconnect-" + session_Name);
              }
              else
              {
                  saveDataToCSV(session_Name);
                  saveemWaveDataToAppCSV(session_Name);
              }
              saveLogToCSV(session_Name);
          }
          else {
              /*
              if (isFirstFrame == false) {
                  saveDataToCSV("Sensor_Disconnect-" + session_Name);
                  saveemWaveDataToAppCSV("Sensor_Disconnect-" + session_Name);
              }*/
          }

          if (isSessionFinished)
          {
              saveDataToCSV(session_Name);
              saveemWaveDataToAppCSV(session_Name);
              saveLogToCSV(session_Name);
          }
      
      }

      private void saveDataToCSV(string sessionName) {

          double accCalmOfAllSecond = 0;
          string currentFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
          string folder = System.IO.Path.Combine(currentFolder, "emWave_Pilot");
          string dataFolder = System.IO.Path.Combine(folder, "Data");
          
          checkFolderExist(dataFolder);
          string date = DateTime.Now.ToString("MM-dd-yyyy") 
                         + "-" + DateTime.Now.ToString("HH-mm-ss");
          string csvFileName = "App-" + userInfo.userName + "-" + sessionName + "-" + date
                                + "-" + Convert.ToString(session_Attempt) + ".csv";
          string csvFilePath = Path.Combine(dataFolder, csvFileName);

          string separator = safeSeparator();
          string saveFileName = csvFilePath;
          StreamWriter dataWriter = new StreamWriter(saveFileName, true, Encoding.Default);

          // Write a header
          dataWriter.WriteLine("Time (s)" + separator + "HR" + separator + "AS" + separator + "lowEP" +
              separator + "highEP" + separator + "Anti-Coherence" + separator + "Local Max HR" + 
              separator + "Calm" + separator + "Acc_Calm" + separator + "emWaveEP");
          
          // Just export first trace (we only support 1 sensor)
          if (hrAnalysisBuffer_.calmPoint[0].Count != 0) {
              for (int i = 0; i < hrAnalysisBuffer_.calmPoint[0].Count; i++)
              {
                  dataWriter.WriteLine(hrAnalysisBuffer_.calmPoint[0][i].X
                      + separator + hrAnalysisBuffer_.instantHRPoint[0][i].Y
                      + separator + hrAnalysisBuffer_.ASPoint[0][i].Y
                      + separator + hrAnalysisBuffer_.EPPoint[0][i].Y
                      + separator + hrAnalysisBuffer_.EPPoint[0][i].Z
                      + separator + hrAnalysisBuffer_.antiCoherencePoint[0][i].Y
                      + separator + hrAnalysisBuffer_.localMaxHRPoint[0][i].Y
                      + separator + hrAnalysisBuffer_.calmPoint[0][i].Y
                      + separator + hrAnalysisBuffer_.accCalmPoint[0][i].Y
                      + separator + hrAnalysisBuffer_.emWaveEPPoint[0][i].Y);
                  if (hrAnalysisBuffer_.calmPoint[0][i].Y != 0) {
                      accCalmOfAllSecond = accCalmOfAllSecond + hrAnalysisBuffer_.calmPoint[0][i].Y;
                      numberOfCalm = numberOfCalm + 1;
                  }
              }
              averageCalm = accCalmOfAllSecond / numberOfCalm;
          }
          
          dataWriter.Close();
      }

      private void saveDataToAppCSV(string sessionName)
      {
          
          string currentFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
          string folder = System.IO.Path.Combine(currentFolder, "emWave_Pilot");
          string dataFolder = System.IO.Path.Combine(folder, "Data");

          checkFolderExist(dataFolder);

          string date = DateTime.Now.ToString("MM-dd-yyyy")
                         + "-" + DateTime.Now.ToString("HH-mm-ss");
          string csvFileName = "App-" + userInfo.userName + "-" + sessionName + "-" + date
                                + "-" + Convert.ToString(session_Attempt) + ".csv";

          string csvFilePath = Path.Combine(dataFolder, csvFileName);

          string separator = safeSeparator();
          string saveFileName = csvFilePath;
          StreamWriter dataWriter = new StreamWriter(saveFileName, true, Encoding.Default);

          // Write a header
          dataWriter.WriteLine("Time (s)" + separator + "HR" + separator + "AS" + separator + "lowEP" +
              separator + "highEP" + separator + "Anti-Coherence" + separator + "Local Max HR" +
              separator + "Calm" + separator + "Acc_Calm" + separator + "emWaveEP");

          // Just export first trace (we only support 1 sensor)
          if (hrAnalysisBuffer_.calmPoint[0].Count != 0)
          {
              for (int i = 0; i < hrAnalysisBuffer_.calmPoint[0].Count; i++)
              {
                  dataWriter.WriteLine(hrAnalysisBuffer_.calmPoint[0][i].X
                      + separator + hrAnalysisBuffer_.instantHRPoint[0][i].Y
                      + separator + hrAnalysisBuffer_.ASPoint[0][i].Y
                      + separator + hrAnalysisBuffer_.EPPoint[0][i].Y
                      + separator + hrAnalysisBuffer_.EPPoint[0][i].Z
                      + separator + hrAnalysisBuffer_.antiCoherencePoint[0][i].Y
                      + separator + hrAnalysisBuffer_.localMaxHRPoint[0][i].Y
                      + separator + hrAnalysisBuffer_.calmPoint[0][i].Y
                      + separator + hrAnalysisBuffer_.accCalmPoint[0][i].Y
                      + separator + hrAnalysisBuffer_.emWaveEPPoint[0][i].Y);
              }
          }

          dataWriter.Close();
      }

      private void saveemWaveDataToAppCSV(string sessionName)
      {
          string currentFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
          string folder = System.IO.Path.Combine(currentFolder, "emWave_Pilot");
          string dataFolder = System.IO.Path.Combine(folder, "emWaveData");

          checkFolderExist(dataFolder);

          string date = DateTime.Now.ToString("MM-dd-yyyy")
                         + "-" + DateTime.Now.ToString("HH-mm-ss");
          string csvFileName = "EW-" + userInfo.userName + "-" + sessionName + "-" + date
                                + "-" + Convert.ToString(session_Attempt) + ".csv";
          string csvFilePath = Path.Combine(dataFolder, csvFileName);

          string separator = safeSeparator();
          string saveFileName = csvFilePath;
          StreamWriter dataWriter = new StreamWriter(saveFileName, true, Encoding.Default);

          // Write a header
          dataWriter.WriteLine("Time (s)" + separator + "HR" + separator + "AS" + separator + "S" +
              separator + "IBI" + separator + "EP");

          // Just export first trace (we only support 1 sensor)
          if (emWaveDataBuffer_.IBIPoint.Count != 0)
          {
              for (int i = 0; i < emWaveDataBuffer_.IBIPoint[0].Count; i++)
              {
                  dataWriter.WriteLine(emWaveDataBuffer_.IBIPoint[0][i].X
                      + separator + emWaveDataBuffer_.HRPoint[0][i].Y
                      + separator + emWaveDataBuffer_.ASPoint[0][i].Y
                      + separator + emWaveDataBuffer_.sPoint[0][i].Y
                      + separator + emWaveDataBuffer_.IBIPoint[0][i].Y
                      + separator + emWaveDataBuffer_.EPPoint[0][i].Y);
              }
          }

          dataWriter.Close();
      }
      
      private void saveLogToCSV(string sessionName)
      {

          string currentFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
          string folder = System.IO.Path.Combine(currentFolder, "emWave_Pilot");
          string logFolder = System.IO.Path.Combine(folder, "Log");

          checkFolderExist(logFolder);
          string separator = safeSeparator();

          List<string> newLine = new List<string>();
          string sessionFinsh = isSessionFinished ? "Finished" : "Not Finished";
          string date = DateTime.Now.ToString("MM-dd-yyyy") 
                         + "-" + DateTime.Now.ToString("HH-mm-ss");

          long saving_session_Time = session_Time - init_Session_Time;
          session_Spend_On_Session_Time = session_Spend_On_Session_Time + saving_session_Time;

          string logInfo = userInfo.userName + separator + sessionName + separator +
                            Convert.ToString(saving_session_Time) + separator 
                            + session_Attempt + separator
                            + sessionFinsh + separator + userInfo.sessionTime
                            + separator + userInfo.threshold + separator + date + separator + averageCalm
                            + separator + session_Spend_On_Session_Time;

          newLine.Add(logInfo);
          
          string csvFileName = "log" + ".csv";
          string csvFilePath = Path.Combine(logFolder, csvFileName);

          string[] lines = File.ReadAllLines(csvFilePath);

          using (StreamWriter sw = new StreamWriter(csvFilePath, false)) {
              foreach (var line in lines) {
                  if (line != "") {

                      String[] words = line.Split(new[] { ",", ",", ",", ",", ",", ",", "," }, StringSplitOptions.None);
                      String temp_session_Name = words[1];
                      sw.WriteLine(line);
                      //if (!temp_session_Name.Equals(sessionName))
                      //{
                          //sw.WriteLine(line);
                      //}
                  }
              }
              sw.Close();
          }
          
          File.AppendAllLines(csvFilePath, newLine);
      }

      private void checkFolderExist(string fileName) {
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

      private void isFirstFrameFun() {
          if (isFirstFrame == true)
          {
              start_btn.Enabled = false;

              lastIBI = currentIBI;
              startMiliseconds = (long)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalMilliseconds;
             
              GraphPane myPane = graph_.GraphPane;
              //Set scale
              myPane.YAxis.Scale.Max = (int)(60 * 2000 / ((currentIBI + lastIBI))) + 10; //Valid range
              myPane.YAxis.Scale.Min = (int)(60 * 2000 / ((currentIBI + lastIBI))) - 10;
              graph_.AxisChange();
              graph_.Invalidate();
              graph_.Refresh();

              isFirstFrame = false;
          }

      }

      private void addDataToGraph(double time) {

          double[] HRV = new double[1];

          currentIBI = 0;
          for (int i = 0; i < 5; i++)
              currentIBI += tempIBI[i];

          currentIBI = currentIBI / 5;
          HRV[0] = 60 * 1000 / currentIBI;
          axisUpdateDataBuffer.Add(HRV[0]);

          if (axisUpdateDataBuffer.Count > 240)
          {
              axisUpdateDataBuffer.RemoveAt(0);
          }

          if (axisUpdateDataBuffer.Count != 0)
          {

              if (axisUpdateDataBuffer.Max() > maxGraphHR)
              {
                  maxGraphHR = axisUpdateDataBuffer.Max();

                  GraphPane myPane = graph_.GraphPane;

                  myPane.YAxis.Scale.Max = (int)maxGraphHR + 10; //Valid range
                  graph_.AxisChange();
                  graph_.Invalidate();
                  graph_.Refresh();
              }
              if (axisUpdateDataBuffer.Min() < minGraphHR)
              {
                  minGraphHR = axisUpdateDataBuffer.Min();

                  GraphPane myPane = graph_.GraphPane;

                  myPane.YAxis.Scale.Min = (int)minGraphHR - 10; //Valid range
                  graph_.AxisChange();
                  graph_.Invalidate();
                  graph_.Refresh();
              }
          }


          AddData(time, HRV);
          HR_Txt.Text = Convert.ToString(currentHR);
      }

      private void addHRAnalysisData(double time) {
          hrAnalysisBuffer_.AddData(liveIBI, currentMdLowEP, currentMdHighEP, currentEP, currentAS, time, isStartCalEP);
          // Update the AS Text

          acc_Calm_Txt.Text = string.Format("{0:0.00}", Convert.ToString(Math.Round(hrAnalysisBuffer_.accCalmScore, 2)));
          calm_Txt.Text = string.Format("{0:0.00}", Convert.ToString(Math.Round(hrAnalysisBuffer_.calmScore, 2)));

          // New calm score, update feedback box
          if (hrAnalysisBuffer_.isNewCalmScoreComing && isStartCalEP)
          {
              feedbackTxt.Visible = true;
              if (hrAnalysisBuffer_.calmScore >= userInfo.threshold)
              {
                  feedbackTxt.BackColor = Color.Lime;
              }
              else if (hrAnalysisBuffer_.calmScore < userInfo.threshold && hrAnalysisBuffer_.calmScore >= (userInfo.threshold - 2.5))
              {
                  feedbackTxt.BackColor = Color.Blue;
              }
              else if (hrAnalysisBuffer_.calmScore < (userInfo.threshold - 3.75))
              {
                  feedbackTxt.BackColor = Color.Red;
              }

          }

      }

      private void updateTime() {
          if (timeSinceLastNewData < 100)
          {
              if (!isFirstFrame)
              {
                  gui_timerCounter++;
                  if (gui_timerCounter == 20)
                  {
                      session_Time++;
                      if (session_Time <= userInfo.sessionTime)
                      {
                          if ((userInfo.sessionTime - session_Time) % 60 < 10)
                          {
                              session_time_txt.Text = Convert.ToString((userInfo.sessionTime - session_Time) / 60) + ":0" + Convert.ToString((userInfo.sessionTime - session_Time) % 60);
                          }
                          else {
                              session_time_txt.Text = Convert.ToString((userInfo.sessionTime - session_Time) / 60) + ":" + Convert.ToString((userInfo.sessionTime - session_Time) % 60);
                          }
                      }
                      else
                      {
                          //saveDataToCSV(session_Name);
                          isSessionFinished = true;
                          guiTimer_.Stop();
                          Application.Exit();
                      }
                      gui_timerCounter = 0;
                  }
              }
          }

      }

      private void updateUploadTimer(object sender, EventArgs e)
      {
          
      }


   }

   // State object for receiving data from remote device.  
   public class StateObject
   {
       // Client socket.  
       public Socket workSocket = null;
       // Size of receive buffer.  
       public const int BufferSize = 256;
       // Receive buffer.  
       public byte[] buffer = new byte[BufferSize];
       // Received data string.  
       public StringBuilder sb = new StringBuilder();
   }  

}
