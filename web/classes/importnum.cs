using MySql.Data.MySqlClient;
using System;
using System.Configuration;
using sms_submit;

namespace sms_submit
{
    public class importnum
    {
        public static int importdata(string username, string password, string senderid, string cdmaheader, string message, string mobileno, string messageid)
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
                        num1 = DL.DL_ExecuteSimpleNonQuery("insert into " + ConfigurationManager.AppSettings["InsertTable"] + " ( username, password, senderid, cdmaheader, messageid, message, date_time, mobileno)  values('" + MySqlHelper.EscapeString(username) + "', '" + MySqlHelper.EscapeString(password) + "', '" + MySqlHelper.EscapeString(senderid) + "', '" + MySqlHelper.EscapeString(cdmaheader) + "', '" + MySqlHelper.EscapeString(messageid) + "', '" + MySqlHelper.EscapeString(message) + "',now(), '" + str + "')");
                    }
                    else if (count2 != 0)
                    {
                        string str = string.Join(",", strArray, index * count1, count2);
                        num1 = DL.DL_ExecuteSimpleNonQuery("insert into " + ConfigurationManager.AppSettings["InsertTable"] + " ( username, password, senderid, cdmaheader, messageid, message, date_time, mobileno)  values('" + MySqlHelper.EscapeString(username) + "', '" + MySqlHelper.EscapeString(password) + "', '" + MySqlHelper.EscapeString(senderid) + "', '" + MySqlHelper.EscapeString(cdmaheader) + "', '" + MySqlHelper.EscapeString(messageid) + "', '" + MySqlHelper.EscapeString(message) + "',now(), '" + str + "')");
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
    }
}
