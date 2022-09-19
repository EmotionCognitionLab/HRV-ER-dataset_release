using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Amazon.S3;
using Amazon.Util;
using Amazon.S3.Transfer;
using Amazon.S3.Model;
using emwave_pulse_PC_App.Util;
using System.ComponentModel;
using System.Windows.Forms;

namespace emwave_pulse_PC_App.AmazonUpload
{
    class Upload_emWaveData
    {
        private String filePath_, backupFilePath_;
        private String userName_;
        private int numberOfDone_ = 0;
        
        public Upload_emWaveData(String userName) {
            string currentFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string folder = System.IO.Path.Combine(currentFolder, "emWave_Pilot");
            string backupFolder = System.IO.Path.Combine(folder, "Backup");
            checkFolderExist(backupFolder);
            this.userName_ = userName;

            transmitLogFolderFiles();

            if (checkDataFolder() > 0) {
                transmitDataFolderFiles();
            }
            else if (checkemWaveDataFolder() > 0) {
                transmitemDataFolderFiles();
            }
            
        }

        private void transmitDataFolderFiles() {
            string currentFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string folder = System.IO.Path.Combine(currentFolder, "emWave_Pilot");
            string dataFolder = System.IO.Path.Combine(folder, "Data");
            string backupFolder = System.IO.Path.Combine(folder, "Backup");

            checkFolderExist(dataFolder);

            DirectoryInfo emwaveDataFolder = new DirectoryInfo(dataFolder);//Assuming Test is your Folder
            FileInfo[] Files = emwaveDataFolder.GetFiles("*.csv"); //Getting Text files

            filePath_ = System.IO.Path.Combine(dataFolder, Files[0].Name);
            backupFilePath_ = System.IO.Path.Combine(backupFolder, Files[0].Name);
            Console.WriteLine(filePath_);
            uploadData(filePath_);

        }

        private void transmitLogFolderFiles()
        {
            string currentFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string folder = System.IO.Path.Combine(currentFolder, "emWave_Pilot");
            string logFolder = System.IO.Path.Combine(folder, "Log");
            string backupFolder = System.IO.Path.Combine(folder, "Backup");

            checkFolderExist(logFolder);

            DirectoryInfo emwaveDataFolder = new DirectoryInfo(logFolder);//Assuming Test is your Folder
            FileInfo[] Files = emwaveDataFolder.GetFiles("*.csv"); //Getting Text files

            filePath_ = System.IO.Path.Combine(logFolder, Files[0].Name);
            backupFilePath_ = System.IO.Path.Combine(backupFolder, Files[0].Name);
            Console.WriteLine(filePath_);
            uploadData(filePath_);

        }

        private void transmitemDataFolderFiles()
        {
            string currentFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string folder = System.IO.Path.Combine(currentFolder, "emWave_Pilot");
            string dataFolder = System.IO.Path.Combine(folder, "emWaveData");
            string backupFolder = System.IO.Path.Combine(folder, "Backup");

            checkFolderExist(dataFolder);

            DirectoryInfo emwaveDataFolder = new DirectoryInfo(dataFolder);//Assuming Test is your Folder
            FileInfo[] Files = emwaveDataFolder.GetFiles("*.csv"); //Getting Text files

            filePath_ = System.IO.Path.Combine(dataFolder, Files[0].Name);
            backupFilePath_ = System.IO.Path.Combine(backupFolder, Files[0].Name);
            Console.WriteLine(filePath_);
            uploadData(filePath_);

        }

        private int checkDataFolder() {
            int numberOfEmptyFiles = 0;

            string currentFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string folder = System.IO.Path.Combine(currentFolder, "emWave_Pilot");
            string dataFolder = System.IO.Path.Combine(folder, "Data");

            checkFolderExist(dataFolder);

            DirectoryInfo emwaveDataFolder = new DirectoryInfo(dataFolder);//Assuming Test is your Folder
            FileInfo[] Files = emwaveDataFolder.GetFiles("*.csv"); //Getting Text files

            foreach (FileInfo file in Files)
            {
                if (file.Length < 10)
                {
                    numberOfEmptyFiles = numberOfEmptyFiles + 1;
                    System.Threading.Thread.Sleep(100);
                    String filePath = System.IO.Path.Combine(dataFolder, file.Name);
                    if (System.IO.File.Exists(filePath))
                    {
                        System.IO.File.Delete(filePath);
                    }

                    System.Threading.Thread.Sleep(100);

                }
            }

            return Files.Length;
        }

        private int checkemWaveDataFolder()
        {
            int numberOfEmptyFiles = 0;
            string currentFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string folder = System.IO.Path.Combine(currentFolder, "emWave_Pilot");
            string emdataFolder = System.IO.Path.Combine(folder, "emWaveData");

            checkFolderExist(emdataFolder);

            DirectoryInfo emwaveDataFolder = new DirectoryInfo(emdataFolder);//Assuming Test is your Folder
            FileInfo[] Files = emwaveDataFolder.GetFiles("*.csv"); //Getting Text files

            foreach (FileInfo file in Files) {
                if (file.Length < 10) {
                    numberOfEmptyFiles = numberOfEmptyFiles + 1;
                    System.Threading.Thread.Sleep(100);
                    String filePath = System.IO.Path.Combine(emdataFolder, file.Name);
                    if (System.IO.File.Exists(filePath_))
                    {
                        System.IO.File.Delete(filePath_);
                    }

                    System.Threading.Thread.Sleep(100);

                }
            }

            return Files.Length - numberOfEmptyFiles;
        }

        private void uploadData(String filePath)
        {
            String bucketName = "hrv-usr-data";
            String accessKey = "CHANGE ME";
            String secretKey = "CHANGE ME";

            AmazonS3Client amazonClient = new AmazonS3Client(accessKey, secretKey, Amazon.RegionEndpoint.USEast2);

            try{
                // Step 1 : Create "Transfer Utility" (replacement of old "Transfer Manager")
                TransferUtility fileTransferUtility =
                    new TransferUtility(amazonClient);


                // Step 2 : Create Request object
                TransferUtilityUploadRequest uploadRequest =
                    new TransferUtilityUploadRequest
                    {
                        BucketName = bucketName + "/" + userName_,
                        FilePath = filePath,
                        //Key = secretKey,
                        //CannedACL = S3CannedACL.PublicRead
                    };

                
                // Step 3 : Event Handler that will be automatically called on each transferred byte 
                uploadRequest.UploadProgressEvent +=
                    new EventHandler<UploadProgressArgs>
                        (uploadRequest_UploadPartProgressEvent);

                // Step 4 : Hit upload and send data to S3
                //fileTransferUtility.Upload(uploadRequest);
                fileTransferUtility.BeginUpload(uploadRequest, new AsyncCallback(uploadComplete), null);
                
            }

            catch (AmazonS3Exception s3Exception)
            {
                Console.WriteLine(s3Exception.Message, s3Exception.InnerException);
            }
            
        }

        private void uploadComplete(IAsyncResult result)
        {
            var x = result;
        }

        private void uploadRequest_UploadPartProgressEvent(object sender, UploadProgressArgs e)
        {
            Console.WriteLine("File Name:{0}", filePath_);
            Console.WriteLine("Total Percent:{0}; Number of Done:{1}", e.PercentDone, numberOfDone_);
            Console.WriteLine("Transferred:{0}/Total:{1}", e.TransferredBytes, e.TotalBytes);

            if (e.PercentDone == 100) {
                if (numberOfDone_ == 1)
                {
                    
                    System.Threading.Thread.Sleep(300);

                    if (System.IO.File.Exists(filePath_)) {
                        System.IO.File.Copy(filePath_, backupFilePath_, true);
                        System.Threading.Thread.Sleep(300);
                        if (!filePath_.Equals("log.csv")) {
                            System.IO.File.Delete(filePath_);
                        }
                    }

                    if (checkDataFolder() > 0)
                    {
                        transmitDataFolderFiles();
                    }
                    else if (checkemWaveDataFolder() > 0)
                    {
                        transmitemDataFolderFiles();
                    } else {
                        Common_Util.safeKillemWave();

                        if (MessageBox.Show("Transmitting Done",
                            "OK", MessageBoxButtons.OK) == DialogResult.OK)
                        {
                            Common_Util.safeKillemWave();
                            Environment.Exit(0);
                        }
                    }
                }
                else {
                    numberOfDone_++;
                }
            }
            

        }

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
