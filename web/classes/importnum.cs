using MySql.Data.MySqlClient;
using System;
using System.Configuration;
using sms_submit;

namespace sms_submit
{
    public class importnum
    {
        public static int importdata(string username, string password, string senderid, string cdmaheader, string message, string mobileno, string messageid, int isUnicode)
        {
            try
            {

                int num1 = 0;
                string[] strArray = mobileno.Split(',');
                int length = strArray.Length;
                if (length <= 0)
                    return 0;
                int count1 = int.Parse(ConfigurationManager.AppSettings["inserttps"].ToString());
                int num2 = length / count1;
                int count2 = length % count1;
                for (int index = 0; index <= num2; ++index)
                {
                    if (index < num2)
                    {
                        string str = string.Join(",", strArray, index * count1, count1);
                        num1 = DL.DL_ExecuteSimpleNonQuery("insert into " + ConfigurationManager.AppSettings["InsertTable"] + " ( username, password, senderid, cdmaheader, messageid, message, date_time, mobileno,is_unicode)  values('" + MySqlHelper.EscapeString(username) + "', '" + MySqlHelper.EscapeString(password) + "', '" + MySqlHelper.EscapeString(senderid) + "', '" + MySqlHelper.EscapeString(cdmaheader) + "', '" + MySqlHelper.EscapeString(messageid) + "', '" + MySqlHelper.EscapeString(message) + "',now(), '" + str + "'," + isUnicode + ")");
                    }
                    else if (count2 != 0)
                    {
                        string str = string.Join(",", strArray, index * count1, count2);
                        num1 = DL.DL_ExecuteSimpleNonQuery("insert into " + ConfigurationManager.AppSettings["InsertTable"] + " ( username, password, senderid, cdmaheader, messageid, message, date_time, mobileno,is_unicode)  values('" + MySqlHelper.EscapeString(username) + "', '" + MySqlHelper.EscapeString(password) + "', '" + MySqlHelper.EscapeString(senderid) + "', '" + MySqlHelper.EscapeString(cdmaheader) + "', '" + MySqlHelper.EscapeString(messageid) + "', '" + MySqlHelper.EscapeString(message) + "',now(), '" + str + "'," + isUnicode + ")");
                    }
                }
                return num1;
            }
            catch (Exception ex)
            {
                ____logconfig.Error_Write(____logconfig.LogLevel.EXC, 0, ex);
                throw;
            }
        }

        public static int ImportInvalidRequest(string userId, string username, string messageId, string senderId, string message, string senddate, string mobileno, string status, string coding)
        {
            int creditCount = GetCredit(message, coding);

            var sql = $@"Insert into dlr_main (userId,messageId,senderId,message,msgcount,senddate) 
                                       values({userId},'{ messageId}','{senderId}','{message}',{ creditCount },{ senddate })";
            var rowinserted = DL.DL_ExecuteSimpleNonQuery(sql, MSCon.DecryptConnectionString(ConfigurationManager.AppSettings["voiceDB"]));
            if (rowinserted > 0)
            {
                var noDetailsInsrt = $@"Insert into dlr_main_no_details (userid,messageid,mobileno,senddate,msglength) 
                                                                values ({userId},'{messageId}','{mobileno}',{senddate},{message.Length})";
                var noDetailsInserted = DL.DL_ExecuteSimpleNonQuery(noDetailsInsrt, MSCon.DecryptConnectionString(ConfigurationManager.AppSettings["voiceDB"]));

                var dlrSql = $@"Insert into sms (destination,status,deliver_date,batchid,date_current,username,message_count) 
                                            values('{mobileno}','{status}',now(),'{messageId}',{senddate},'{username}',{creditCount})";
                var insrtRow = DL.DL_ExecuteSimpleNonQuery(dlrSql, MSCon.DecryptConnectionString(ConfigurationManager.AppSettings["voiceDB"]));

                if (coding == "2" || coding == "3")
                {
                    InsertOrUpdateInvalidrequestCount(username, creditCount, 0, senderId);
                }
                else
                {
                    InsertOrUpdateInvalidrequestCount(username, 0, creditCount, senderId);
                }

            }
            return rowinserted;


        }

        public static void InsertOrUpdateInvalidrequestCount(string username, int usms, int nsms, string senderid)
        {

            var uscId = string.Format("{0}{1}{2}", username, senderid, DateTime.Now.ToString("yyyyMMdd"));
            var sql = $@"INSERT INTO usc_smscount_iifl (username, usms, nsms, senderid,scdate,send_date,mobilecount,usc_id)
                                    VALUES ('{username}',{usms},{nsms},'{senderid}',now(),{DateTime.Now.ToString("yyyyMMdd")},1, '{uscId}')
                                    ON DUPLICATE KEY UPDATE
                                     usms = usms+{usms},
                                      nsms = nsms+{nsms},
                                      mobilecount = mobilecount+1;";
            var rowInserted = DL.DL_ExecuteSimpleNonQuery(sql);
        }

        public static int GetCredit(string strMessage, string coding)
        {
            try
            {
                //if (coding == "2" || coding == "3")
                //    strMessage = System.Text.Encoding.Default.GetString(System.Text.Encoding.UTF8.GetBytes(strMessage));


                strMessage = System.Web.HttpUtility.UrlEncode(strMessage);
                strMessage = strMessage.Replace("%0d%0a", "xx");
                strMessage = strMessage.Replace("%0a", "xx");

                strMessage = strMessage.Replace("%7e", "xx");
                strMessage = strMessage.Replace("%5c", "xx");
                strMessage = strMessage.Replace("%7c", "xx");
                strMessage = strMessage.Replace("%5e", "xx");
                strMessage = strMessage.Replace("%5b", "xx");
                strMessage = strMessage.Replace("%5d", "xx");
                strMessage = strMessage.Replace("%7b", "xx");
                strMessage = strMessage.Replace("%7d", "xx");
                strMessage = System.Web.HttpUtility.UrlDecode(strMessage);

                float intMsg = 0;


                if (coding == "2" || coding == "3")
                {
                    if (strMessage.Length > 70)
                    {
                        intMsg = (float)strMessage.Length / 65;
                    }
                    else
                    {
                        intMsg = (float)strMessage.Length / 70;

                    }
                }
                else
                {
                    if (strMessage.Length > 160)
                    {
                        intMsg = (float)strMessage.Length / 153;
                    }
                    else
                    { intMsg = (float)strMessage.Length / 160; }
                }

                if (Math.Floor(intMsg) == intMsg)
                {
                    return ((int)Math.Floor(intMsg));
                }
                else
                {
                    int i = (int)Math.Floor(intMsg);
                    return (i + 1);
                }
            }
            catch (Exception ex)
            {
                (new WriteErrLog()).WriteError_File(ex.ToString(), "utility.cs getcredit");
                return (3);
            }
        }
    }
}
