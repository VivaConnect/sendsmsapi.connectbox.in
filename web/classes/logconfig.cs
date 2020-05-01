using System;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;

namespace sms_submit
{
    public class ____logconfig
    {
        public static void Log_Write(
          ____logconfig.LogLevel GetLog_level,
          int TR_Num,
          string Log_String)
        {
            new ____logconfig.Log_Write_Delgt1(____logconfig.Log_Write_Call).BeginInvoke(GetLog_level, TR_Num, Log_String, (AsyncCallback)null, (object)null);
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public static void Log_Write_Call(
          ____logconfig.LogLevel GetLog_level,
          int TR_Num,
          string Log_String)
        {
            string str1 = "";
            try
            {
                Log_String = Log_String.Replace("\r\n", "\n").Replace("\n", "*_*");
                if (!(ConfigurationManager.AppSettings["level" + (object)(int)GetLog_level].ToString() == "1"))
                    return;
                Log_String = "[" + GetLog_level.ToString() + "]::" + Log_String;
                str1 = string.Format("{0}{1,-8}{2}\n", (object)DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ffffff"), (object)("[" + (object)TR_Num + "]"), (object)Log_String);
                string str2 = ConfigurationManager.AppSettings["strlogpath"].ToString() + DateTime.Now.ToString("yyyy\\\\MM\\\\dd") + "\\";
                if (!Directory.Exists(str2))
                    Directory.CreateDirectory(str2);
                string filename = ConfigurationManager.AppSettings["strLogFileName"] + "_" + DateTime.Now.Hour.ToString("D2");
                using (FileStream fileStream = new FileStream(____logconfig.renameFiles(str2, filename), FileMode.Append, FileAccess.Write, FileShare.Write, 8, true))
                {
                    byte[] bytes = Encoding.ASCII.GetBytes(str1);
                    fileStream.BeginWrite(bytes, 0, bytes.Length, new AsyncCallback(____logconfig.ascallback), (object)fileStream);
                }
            }
            catch (Exception ex)
            {
                logger_for_exception.Append(ex.ToString(), 117, str1);
            }
        }

        public static void Log_Write(
          ____logconfig.LogLevel GetLog_level,
          StackTrace objStkTrc,
          string Log_String)
        {
            new ____logconfig.Log_Write_Delgt2(____logconfig.Log_Write_Call).BeginInvoke(GetLog_level, objStkTrc, Log_String, (AsyncCallback)null, (object)null);
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public static void Log_Write_Call(
          ____logconfig.LogLevel GetLog_level,
          StackTrace objStkTrc,
          string Log_String)
        {
            string str1 = "";
            try
            {
                Log_String = Log_String.Replace("\r\n", "\n").Replace("\n", "*_*");
                if (!(ConfigurationManager.AppSettings["level" + (object)(int)GetLog_level].ToString() == "1"))
                    return;
                string str2 = "[" + GetLog_level.ToString() + "]::";
                StackFrame frame = objStkTrc.GetFrame(objStkTrc.FrameCount - 1);
                StringBuilder stringBuilder1 = new StringBuilder();
                for (int index = 0; index < objStkTrc.FrameCount; ++index)
                {
                    frame = objStkTrc.GetFrame(index);
                    StringBuilder stringBuilder2 = new StringBuilder();
                    stringBuilder2.Append(Path.GetFileName(frame.GetFileName()));
                    stringBuilder2.Append("::");
                    stringBuilder2.Append(frame.GetMethod().Name + "()");
                    stringBuilder2.Append("::");
                    stringBuilder2.Append("[" + (object)frame.GetFileLineNumber() + "]");
                    stringBuilder2.Append("::");
                    str2 += stringBuilder2.ToString();
                }
                Log_String = str2 + Log_String;
                str1 = string.Format("{0}{1,-8}{2}\n", (object)DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ffffff"), (object)("[" + (object)frame.GetFileLineNumber() + "]"), (object)Log_String);
                string str3 = ConfigurationManager.AppSettings["strlogpath"].ToString();
                DateTime now = DateTime.Now;
                string str4 = now.ToString("yyyy\\\\MM\\\\dd");
                string str5 = str3 + str4 + "\\";
                if (!Directory.Exists(str5))
                    Directory.CreateDirectory(str5);
                string appSetting = ConfigurationManager.AppSettings["strLogFileName"];
                now = DateTime.Now;
                string str6 = now.Hour.ToString("D2");
                string filename = appSetting + "_" + str6;
                using (FileStream fileStream = new FileStream(____logconfig.renameFiles(str5, filename), FileMode.Append, FileAccess.Write, FileShare.None, 8, true))
                {
                    byte[] bytes = Encoding.ASCII.GetBytes(str1);
                    fileStream.BeginWrite(bytes, 0, bytes.Length, new AsyncCallback(____logconfig.ascallback), (object)fileStream);
                }
            }
            catch (Exception ex)
            {
                logger_for_exception.Append(ex.ToString(), 52, str1);
            }
        }

        public static void Log_Write(
          ____logconfig.LogLevel GetLog_level,
          StackTrace objStkTrc,
          string Log_String,
          string strLogFileName)
        {
            new ____logconfig.Log_Write_Delgt3(____logconfig.Log_Write_Call).BeginInvoke(GetLog_level, objStkTrc, Log_String, strLogFileName, (AsyncCallback)null, (object)null);
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public static void Log_Write_Call(
          ____logconfig.LogLevel GetLog_level,
          StackTrace objStkTrc,
          string Log_String,
          string strLogFileName)
        {
            string str1 = "";
            try
            {
                Log_String = Log_String.Replace("\r\n", "\n").Replace("\n", "*_*");
                if (!(ConfigurationManager.AppSettings["level" + (object)(int)GetLog_level].ToString() == "1"))
                    return;
                string str2 = "[" + GetLog_level.ToString() + "]::";
                StackFrame frame = objStkTrc.GetFrame(objStkTrc.FrameCount - 1);
                StringBuilder stringBuilder1 = new StringBuilder();
                for (int index = 0; index < objStkTrc.FrameCount; ++index)
                {
                    frame = objStkTrc.GetFrame(index);
                    StringBuilder stringBuilder2 = new StringBuilder();
                    stringBuilder2.Append(Path.GetFileName(frame.GetFileName()));
                    stringBuilder2.Append("::");
                    stringBuilder2.Append(frame.GetMethod().Name + "()");
                    stringBuilder2.Append("::");
                    stringBuilder2.Append("[" + (object)frame.GetFileLineNumber() + "]");
                    stringBuilder2.Append("::");
                    str2 += stringBuilder2.ToString();
                }
                Log_String = str2 + Log_String;
                str1 = string.Format("{0}{1,-8}{2}\n", (object)DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ffffff"), (object)("[" + (object)frame.GetFileLineNumber() + "]"), (object)Log_String);
                string str3 = ConfigurationManager.AppSettings["strlogpath"].ToString();
                DateTime now = DateTime.Now;
                string str4 = now.ToString("yyyy\\\\MM\\\\dd");
                string str5 = str3 + str4 + "\\";
                if (!Directory.Exists(str5))
                    Directory.CreateDirectory(str5);
                string str6 = strLogFileName;
                now = DateTime.Now;
                string str7 = now.Hour.ToString("D2");
                strLogFileName = str6 + "_" + str7;
                using (FileStream fileStream = new FileStream(____logconfig.renameFiles(str5, strLogFileName), FileMode.Append, FileAccess.Write, FileShare.None, 8, true))
                {
                    byte[] bytes = Encoding.ASCII.GetBytes(str1);
                    fileStream.BeginWrite(bytes, 0, bytes.Length, new AsyncCallback(____logconfig.ascallback), (object)fileStream);
                }
            }
            catch (Exception ex)
            {
                logger_for_exception.Append(ex.ToString(), 84, str1);
            }
        }

        public static void Log_Write(
          ____logconfig.LogLevel GetLog_level,
          int TR_Num,
          string Log_String,
          string strLogFileName)
        {
            new ____logconfig.Log_Write_Delgt4(____logconfig.Log_Write_Call).BeginInvoke(GetLog_level, TR_Num, Log_String, strLogFileName, (AsyncCallback)null, (object)null);
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public static void Log_Write_Call(
          ____logconfig.LogLevel GetLog_level,
          int TR_Num,
          string Log_String,
          string strLogFileName)
        {
            string str1 = "";
            try
            {
                Log_String = Log_String.Replace("\r\n", "\n").Replace("\n", "*_*");
                if (!(ConfigurationManager.AppSettings["level" + (object)(int)GetLog_level].ToString() == "1"))
                    return;
                Log_String = "[" + GetLog_level.ToString() + "]::" + Log_String;
                str1 = string.Format("{0}{1,-8}{2}\n", (object)DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ffffff"), (object)("[" + (object)TR_Num + "]"), (object)Log_String);
                string str2 = ConfigurationManager.AppSettings["strlogpath"].ToString() + DateTime.Now.ToString("yyyy\\\\MM\\\\dd") + "\\";
                if (!Directory.Exists(str2))
                    Directory.CreateDirectory(str2);
                strLogFileName = strLogFileName + "_" + DateTime.Now.Hour.ToString("D2");
                using (FileStream fileStream = new FileStream(____logconfig.renameFiles(str2, strLogFileName), FileMode.Append, FileAccess.Write, FileShare.None, 8, true))
                {
                    byte[] bytes = Encoding.ASCII.GetBytes(str1);
                    fileStream.BeginWrite(bytes, 0, bytes.Length, new AsyncCallback(____logconfig.ascallback), (object)fileStream);
                }
            }
            catch (Exception ex)
            {
                logger_for_exception.Append(ex.ToString(), 149, str1);
            }
        }

        public static void Error_Write(
          ____logconfig.LogLevel GetLog_level,
          int ER_Num,
          Exception exString)
        {
            new ____logconfig.Error_Write_Delgt1(____logconfig.Error_Write_Call).BeginInvoke(GetLog_level, ER_Num, exString, (AsyncCallback)null, (object)null);
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public static void Error_Write_Call(
          ____logconfig.LogLevel GetLog_level,
          int ER_Num,
          Exception exString)
        {
            string str1 = "";
            try
            {
                if (!(ConfigurationManager.AppSettings["level" + (object)(int)GetLog_level].ToString() == "1"))
                    return;
                StackTrace stackTrace = new StackTrace(exString, true);
                string str2 = "[" + GetLog_level.ToString() + "]::";
                StackFrame frame = stackTrace.GetFrame(stackTrace.FrameCount - 1);
                StringBuilder stringBuilder1 = new StringBuilder();
                for (int index = 0; index < stackTrace.FrameCount; ++index)
                {
                    frame = stackTrace.GetFrame(index);
                    StringBuilder stringBuilder2 = new StringBuilder();
                    stringBuilder2.Append(Path.GetFileName(frame.GetFileName()));
                    stringBuilder2.Append("::");
                    stringBuilder2.Append(frame.GetMethod().Name + "()");
                    stringBuilder2.Append("::");
                    stringBuilder2.Append("[" + (object)frame.GetFileLineNumber() + "]");
                    stringBuilder2.Append("::");
                    str2 += stringBuilder2.ToString();
                }
                ER_Num = frame.GetFileLineNumber();
                string str3 = str2 + exString.ToString().Replace("\r\n", "\n").Replace("\n", "*_*");
                DateTime now = DateTime.Now;
                str1 = string.Format("{0}{1,-8}{2}\n", (object)now.ToString("yyyy-MM-dd HH:mm:ss.ffffff"), (object)("[" + (object)ER_Num + "]"), (object)str3);
                string str4 = ConfigurationManager.AppSettings["strlogpath"].ToString();
                now = DateTime.Now;
                string str5 = now.ToString("yyyy\\\\MM\\\\dd");
                string str6 = str4 + str5 + "\\";
                if (!Directory.Exists(str6))
                    Directory.CreateDirectory(str6);
                string str7 = ConfigurationManager.AppSettings["ErrorLogfileName"].ToString();
                now = DateTime.Now;
                string str8 = now.Hour.ToString("D2");
                string filename = str7 + "_" + str8;
                using (FileStream fileStream = new FileStream(____logconfig.renameFiles(str6, filename), FileMode.Append, FileAccess.Write, FileShare.None, 8, true))
                {
                    byte[] bytes = Encoding.ASCII.GetBytes(str1);
                    fileStream.BeginWrite(bytes, 0, bytes.Length, new AsyncCallback(____logconfig.ascallback), (object)fileStream);
                }
            }
            catch (Exception ex)
            {
                logger_for_exception.Append(ex.ToString(), 205, str1);
            }
        }

        public static void Error_Write(
          ____logconfig.LogLevel GetLog_level,
          int ER_Num,
          Exception exString,
          string ErrorLogfileName)
        {
            new ____logconfig.Error_Write_Delgt2(____logconfig.Error_Write_Call).BeginInvoke(GetLog_level, ER_Num, exString, ErrorLogfileName, (AsyncCallback)null, (object)null);
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public static void Error_Write_Call(
          ____logconfig.LogLevel GetLog_level,
          int ER_Num,
          Exception exString,
          string ErrorLogfileName)
        {
            string str1 = "";
            try
            {
                if (!(ConfigurationManager.AppSettings["level" + (object)(int)GetLog_level].ToString() == "1"))
                    return;
                StackTrace stackTrace = new StackTrace(exString, true);
                string str2 = "[" + GetLog_level.ToString() + "]::";
                StackFrame frame = stackTrace.GetFrame(stackTrace.FrameCount - 1);
                StringBuilder stringBuilder1 = new StringBuilder();
                for (int index = 0; index < stackTrace.FrameCount; ++index)
                {
                    frame = stackTrace.GetFrame(index);
                    StringBuilder stringBuilder2 = new StringBuilder();
                    stringBuilder2.Append(Path.GetFileName(frame.GetFileName()));
                    stringBuilder2.Append("::");
                    stringBuilder2.Append(frame.GetMethod().Name + "()");
                    stringBuilder2.Append("::");
                    stringBuilder2.Append("[" + (object)frame.GetFileLineNumber() + "]");
                    stringBuilder2.Append("::");
                    str2 += stringBuilder2.ToString();
                }
                ER_Num = frame.GetFileLineNumber();
                string str3 = str2 + exString.ToString().Replace("\r\n", "\n").Replace("\n", "*_*");
                DateTime now = DateTime.Now;
                str1 = string.Format("{0}{1,-8}{2}\n", (object)now.ToString("yyyy-MM-dd HH:mm:ss.ffffff"), (object)("[" + (object)ER_Num + "]"), (object)str3);
                string str4 = ConfigurationManager.AppSettings["strlogpath"].ToString();
                now = DateTime.Now;
                string str5 = now.ToString("yyyy\\\\MM\\\\dd");
                string str6 = str4 + str5 + "\\";
                if (!Directory.Exists(str6))
                    Directory.CreateDirectory(str6);
                string str7 = ErrorLogfileName;
                now = DateTime.Now;
                string str8 = now.Hour.ToString("D2");
                ErrorLogfileName = str7 + "_" + str8;
                using (FileStream fileStream = new FileStream(____logconfig.renameFiles(str6, ErrorLogfileName), FileMode.Append, FileAccess.Write, FileShare.None, 8, true))
                {
                    byte[] bytes = Encoding.ASCII.GetBytes(str1);
                    fileStream.BeginWrite(bytes, 0, bytes.Length, new AsyncCallback(____logconfig.ascallback), (object)fileStream);
                }
            }
            catch (Exception ex)
            {
                logger_for_exception.Append(ex.ToString(), 258, str1);
            }
        }

        public static void Error_Write(
          ____logconfig.LogLevel GetLog_level,
          int ER_Num,
          string exString)
        {
            new ____logconfig.Error_Write_Delgt3(____logconfig.Error_Write_Call).BeginInvoke(GetLog_level, ER_Num, exString, (AsyncCallback)null, (object)null);
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public static void Error_Write_Call(
          ____logconfig.LogLevel GetLog_level,
          int ER_Num,
          string exString)
        {
            string str1 = "";
            try
            {
                if (!(ConfigurationManager.AppSettings["level" + (object)(int)GetLog_level].ToString() == "1"))
                    return;
                string str2 = "[" + GetLog_level.ToString() + "]::" + exString.ToString().Replace("\r\n", "\n").Replace("\n", "*_*");
                str1 = string.Format("{0}{1,-8}{2}\n", (object)DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ffffff"), (object)("[" + (object)ER_Num + "]"), (object)str2);
                string str3 = ConfigurationManager.AppSettings["strlogpath"].ToString();
                DateTime now = DateTime.Now;
                string str4 = now.ToString("yyyy\\\\MM\\\\dd");
                string str5 = str3 + str4 + "\\";
                if (!Directory.Exists(str5))
                    Directory.CreateDirectory(str5);
                string str6 = ConfigurationManager.AppSettings["ErrorLogfileName"].ToString();
                now = DateTime.Now;
                string str7 = now.Hour.ToString("D2");
                string filename = str6 + "_" + str7;
                using (FileStream fileStream = new FileStream(____logconfig.renameFiles(str5, filename), FileMode.Append, FileAccess.Write, FileShare.None, 8, true))
                {
                    byte[] bytes = Encoding.ASCII.GetBytes(str1);
                    fileStream.BeginWrite(bytes, 0, bytes.Length, new AsyncCallback(____logconfig.ascallback), (object)fileStream);
                }
            }
            catch (Exception ex)
            {
                logger_for_exception.Append(ex.ToString(), 298, str1);
            }
        }

        public static void Error_Write(
          ____logconfig.LogLevel GetLog_level,
          int ER_Num,
          string exString,
          string ErrorLogfileName)
        {
            new ____logconfig.Error_Write_Delgt4(____logconfig.Error_Write_Call).BeginInvoke(GetLog_level, ER_Num, exString, ErrorLogfileName, (AsyncCallback)null, (object)null);
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public static void Error_Write_Call(
          ____logconfig.LogLevel GetLog_level,
          int ER_Num,
          string exString,
          string ErrorLogfileName)
        {
            string str1 = "";
            try
            {
                if (!(ConfigurationManager.AppSettings["level" + (object)(int)GetLog_level].ToString() == "1"))
                    return;
                string str2 = "[" + GetLog_level.ToString() + "]::" + exString.ToString().Replace("\r\n", "\n").Replace("\n", "*_*");
                str1 = string.Format("{0}{1,-8}{2}\n", (object)DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ffffff"), (object)("[" + (object)ER_Num + "]"), (object)str2);
                string str3 = ConfigurationManager.AppSettings["strlogpath"].ToString();
                DateTime now = DateTime.Now;
                string str4 = now.ToString("yyyy\\\\MM\\\\dd");
                string str5 = str3 + str4 + "\\";
                if (!Directory.Exists(str5))
                    Directory.CreateDirectory(str5);
                string str6 = ErrorLogfileName;
                now = DateTime.Now;
                string str7 = now.Hour.ToString("D2");
                ErrorLogfileName = str6 + "_" + str7;
                using (FileStream fileStream = new FileStream(____logconfig.renameFiles(str5, ErrorLogfileName), FileMode.Append, FileAccess.Write, FileShare.None, 8, true))
                {
                    byte[] bytes = Encoding.ASCII.GetBytes(str1);
                    fileStream.BeginWrite(bytes, 0, bytes.Length, new AsyncCallback(____logconfig.ascallback), (object)fileStream);
                }
            }
            catch (Exception ex)
            {
                logger_for_exception.Append(ex.ToString(), 335, str1);
            }
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        private static void ascallback(IAsyncResult ar)
        {
            try
            {
                using (FileStream asyncState = (FileStream)ar.AsyncState)
                {
                    asyncState.EndWrite(ar);
                    asyncState.Close();
                }
            }
            catch (Exception ex)
            {
                logger_for_exception.Append(ex.ToString(), 349, ex.ToString());
            }
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        private static string renameFiles(string filepath, string filename)
        {
            FileInfo[] files = new DirectoryInfo(filepath).GetFiles(filename + ".*");
            if (files.Length > 0 && ____logconfig.checkfilesizefunction(new FileInfo(filepath + filename).Length) >= long.Parse(ConfigurationManager.AppSettings["filesize"].ToString()))
            {
                for (int index = files.Length - 1; index >= 0; --index)
                {
                    string extension = Path.GetExtension(files[index].FullName);
                    string str = (Convert.ToInt32(extension == "" ? "0" : extension.Replace(".", "")) + 1).ToString("D2");
                    try
                    {
                        File.Move(files[index].FullName, filepath + filename + "." + str);
                        new FileInfo(filepath + filename + "." + str).Attributes = FileAttributes.ReadOnly;
                    }
                    catch (Exception ex)
                    {
                        logger_for_exception.Append(ex.ToString(), 371, ex.ToString());
                    }
                }
            }
            return filepath + filename;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        private static long checkfilesizefunction(long filesizetocheck)
        {
            try
            {
                long num = filesizetocheck;
                long.Parse(ConfigurationManager.AppSettings["filesize"].ToString());
                string str = ConfigurationManager.AppSettings["FileSizeFormat"].ToString();
                if (str.ToUpper() == "KB")
                    return num / 1024L;
                if (str.ToUpper() == "MB")
                    return num / 1048576L;
                return str.ToUpper() == "GB" ? num / 1073741824L : num;
            }
            catch (Exception ex)
            {
                logger_for_exception.Append(ex.ToString(), 542, ex.ToString());
                return 0;
            }
        }

        public enum LogLevel
        {
            INFO,
            DEBUG,
            DB,
            EXC,
            CRIT,
        }

        public delegate void Log_Write_Delgt1(
          ____logconfig.LogLevel GetLog_level,
          int TR_Num,
          string Log_String);

        public delegate void Log_Write_Delgt2(
          ____logconfig.LogLevel GetLog_level,
          StackTrace objStkTrc,
          string Log_String);

        public delegate void Log_Write_Delgt3(
          ____logconfig.LogLevel GetLog_level,
          StackTrace objStkTrc,
          string Log_String,
          string strLogFileName);

        public delegate void Log_Write_Delgt4(
          ____logconfig.LogLevel GetLog_level,
          int TR_Num,
          string Log_String,
          string strLogFileName);

        public delegate void Error_Write_Delgt1(
          ____logconfig.LogLevel GetLog_level,
          int ER_Num,
          Exception exString);

        public delegate void Error_Write_Delgt2(
          ____logconfig.LogLevel GetLog_level,
          int ER_Num,
          Exception exString,
          string ErrorLogfileName);

        public delegate void Error_Write_Delgt3(
          ____logconfig.LogLevel GetLog_level,
          int ER_Num,
          string exString);

        public delegate void Error_Write_Delgt4(
          ____logconfig.LogLevel GetLog_level,
          int ER_Num,
          string exString,
          string ErrorLogfileName);
    }

    public class logger_for_exception
    {
        private static StreamWriter sw;
        private static string logDirectory;

        static logger_for_exception()
        {
            try
            {
                logger_for_exception.logDirectory = ConfigurationManager.AppSettings["strlogpath"].ToString() + "logconfig.err";
                if (!File.Exists(logger_for_exception.logDirectory))
                    File.Create(logger_for_exception.logDirectory).Close();
                logger_for_exception.sw = File.AppendText(logger_for_exception.logDirectory);
            }
            catch (Exception ex)
            {
            }
        }

        public static void Append(string Log_String, int TR_Num, string actualLogString)
        {
            try
            {
                lock (logger_for_exception.sw)
                {
                    Log_String = "[CRIT]::" + Log_String.Replace("\r\n", "\n").Replace("\n", "*_*");
                    string str = string.Format("{0}{1,-8}{2}\n", (object)DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ffffff"), (object)("[" + (object)TR_Num + "]"), (object)(actualLogString.Replace("\r\n", "\n").Replace("\n", "*_*") + ":::" + Log_String));
                    logger_for_exception.sw.Write(str);
                    logger_for_exception.sw.Flush();
                }
            }
            catch
            {
            }
        }
    }
}
