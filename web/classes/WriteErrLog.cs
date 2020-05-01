using System;
using System.IO;
using System.Text;

namespace sms_submit
{
    public class WriteErrLog
    {
        private string fileName;
        private string xmlfile;

        public void WriteAsync(string filename, string message)
        {
            try
            {
                using (FileStream fileStream = new FileStream("D:\\ExceptionLog\\IIFLInsert\\" + filename, FileMode.Append, FileAccess.Write, FileShare.ReadWrite, 8, true))
                {
                    byte[] bytes = Encoding.ASCII.GetBytes(message);
                    fileStream.BeginWrite(bytes, 0, bytes.Length, new AsyncCallback(WriteErrLog.OnWriteComplete), (object)fileStream);
                    fileStream.Close();
                    fileStream.Dispose();
                }
            }
            catch
            {
            }
        }

        private static void OnWriteComplete(IAsyncResult ar)
        {
            try
            {
                FileStream asyncState = (FileStream)ar.AsyncState;
                asyncState.EndWrite(ar);
                asyncState.Close();
            }
            catch
            {
            }
        }

        public WriteErrLog()
        {
            this.fileName = "AllExceptionsLogAPI.txt";
            this.xmlfile = "XmlRequestLogs.txt";
        }

        public WriteErrLog(string fileName)
        {
            this.fileName = fileName;
        }

        public void WriteError_File(string errorText)
        {
            this.writeLogs(errorText);
        }

        public void WriteError_File(string Err, string PageName, string UserName)
        {
            try
            {
                this.writeLogs(UserName + " : Error : " + Err + " " + (object)DateTime.Now + Environment.NewLine + "On Page : " + PageName + Environment.NewLine + Environment.NewLine);
            }
            catch (Exception ex)
            {
            }
        }

        public void WriteError_File(string Err, string PageName)
        {
            try
            {
                this.writeLogs("Error : " + Err + " " + (object)DateTime.Now + Environment.NewLine + "On Page : " + PageName + Environment.NewLine + Environment.NewLine);
            }
            catch (Exception ex)
            {
            }
        }

        protected void writeLogs(string logstring)
        {
            try
            {
                File.AppendAllText("D:\\ExceptionLog\\IIFLInsert\\" + this.fileName, logstring);
            }
            catch (Exception ex)
            {
            }
        }
    }
}
