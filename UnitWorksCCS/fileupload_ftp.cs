
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using static UnitWorksCCS.FTPLibrary;

namespace UnitWorksCCS
{
    public class fileupload_ftp
    {
        #region Members
        //String Variables we will need to use throughout the File.
        string FileName;

        //FTPclient that will be used to Upload the File and manage all the events
        public FTPclient FtpClient;
        #endregion
        public string fileuploadftp(string UploadFilePath, string UploadDirectory, FTPclient _Ftpclient)
        {
            string result = "Success";
            try
            {
                FileName = System.IO.Path.GetFileName(UploadFilePath);
                string filepath = _Ftpclient.Hostname + UploadDirectory + FileName;
                string uploadDirectory = UploadDirectory;
                FtpClient = _Ftpclient;

                //Setup our Download Client and Start Downloading
                FtpClient.CurrentDirectory = uploadDirectory;
                FtpClient.OnUploadCompleted += new FTPclient.UploadCompletedHandler(FtpClient_OnUploadCompleted);
                FtpClient.OnUploadProgressChanged += new FTPclient.UploadProgressChangedHandler(FtpClient_OnUploadProgressChanged);
                FtpClient.Upload(UploadFilePath, UploadDirectory + FileName);
            }
            catch(Exception e)
            {
                result = "fail";
            }
            return result;
        }

        public bool CheckIfFileExistsOnServer(string fileName,FTPclient _Ftpclient)
        {
            var request = (FtpWebRequest)WebRequest.Create(_Ftpclient.Hostname +"//" + fileName);
            request.Credentials = new NetworkCredential("FTP", "FTP");
            request.Method = WebRequestMethods.Ftp.GetFileSize;

            try
            {
                FtpWebResponse response = (FtpWebResponse)request.GetResponse();
                return true;
            }
            catch (WebException ex)
            {
                FtpWebResponse response = (FtpWebResponse)ex.Response;
                if (response.StatusCode == FtpStatusCode.ActionNotTakenFileUnavailable)
                    return false;
            }
            return false;
        }


        #region Events
        #region Uploading File Events
        public void FtpClient_OnUploadProgressChanged(object sender, UploadProgressChangedArgs e)
        {
            //Set Value and Maximum for Progressbar
            int ProgrssMax = Convert.ToInt32(e.TotleBytes);
            int progressval = Convert.ToInt32(e.BytesUploaded);
            //progressBar1.Maximum = Convert.ToInt32(e.TotleBytes);
            //progressBar1.Value = Convert.ToInt32(e.BytesUploaded);
            // Calculate the Upload progress in percentages
            Int64 PercentProgress = Convert.ToInt64((e.BytesUploaded * 100) / e.TotleBytes);
            string PercentProgres = PercentProgress.ToString() + " % Uploading " + FileName;
            //this.Text = PercentProgress.ToString() + " % Uploading " + FileName;

            string res = "Upload Status: Uploaded " + GetFileSize(e.BytesUploaded) + " out of " + GetFileSize(e.TotleBytes) + " (" + PercentProgress.ToString() + "%)";
            //lblDownloadStatus.Text = "Upload Status: Uploaded " + GetFileSize(e.BytesUploaded) + " out of " + GetFileSize(e.TotleBytes) + " (" + PercentProgress.ToString() + "%)";
          
        }

       public  void FtpClient_OnUploadCompleted(object sender, UploadCompletedArgs e)
        {
            string res = "";
            if (e.UploadCompleted)
            {
                //No Error
                //this.Text = "Upload Completed!";
                //lblDownloadStatus.Text = "Uploaded File Successfully!";
                ////change value
                //progressBar1.Value = progressBar1.Maximum;
                //btnCancel.Text = "Exit";
                //MessageBox.Show("File Successfully Uploaded!", "Upload Message", MessageBoxButtons.OK, MessageBoxIcon.Information);

                res = "File Successfully Uploaded!" + "Upload Message";
            }
            else
            {
                //Change Texts of various controls.
                //lblDownloadStatus.Text = "Upload Status: " + e.UploadStatus;
                //this.Text = "Upload Error";
                //btnCancel.Text = "Exit";
                ////Display a Messagebox with the error.
                //MessageBox.Show("Error: " + e.UploadStatus, "Upload Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                res = "Error: " + e.UploadStatus + "Upload Error";
            }
            
        }

        #endregion
        #endregion

        #region Functions
        //Code Below Converts Bytes to KB, MB, GB, or just Bytes.  Makes the App more look :)
        //Obtained from: http://www.freevbcode.com/ShowCode.Asp?ID=1971
        private string GetFileSize(double byteCount)
        {
            string size = "0 Bytes";
            if (byteCount >= 1073741824.0)
                size = String.Format("{0:##.##}", byteCount / 1073741824.0) + " GB";
            else if (byteCount >= 1048576.0)
                size = String.Format("{0:##.##}", byteCount / 1048576.0) + " MB";
            else if (byteCount >= 1024.0)
                size = String.Format("{0:##.##}", byteCount / 1024.0) + " KB";
            else if (byteCount > 0 && byteCount < 1024.0)
                size = byteCount.ToString() + " Bytes";

            return size;
        }
        #endregion
    }
}
