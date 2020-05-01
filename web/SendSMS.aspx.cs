using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using sms_submit;
using System.Data;
using web.classes;

namespace web
{
    public partial class SendSMS : System.Web.UI.Page
    {
        private WriteErrLog errorlogs = new WriteErrLog();
        protected void Page_Load(object sender, EventArgs e)
        {
            string username = ""; string password = ""; string mobileno = ""; string senderid = ""; string cdmaheader = ""; string message = "";
            try
            {
                if (this.Request.HttpMethod == "GET")
                {
                    username = this.Request.QueryString["UserName"];
                    password = this.Request.QueryString["password"];
                    mobileno = this.Request.QueryString["MobileNo"];
                    senderid = this.Request.QueryString["SenderID"];
                    cdmaheader = senderid;
                    message = this.Request.QueryString["Message"];
                }
                if (this.Request.HttpMethod == "POST")
                {
                    username = this.Request.Form["UserName"];
                    password = this.Request.Form["password"];
                    mobileno = this.Request.Form["MobileNo"];
                    senderid = this.Request.Form["SenderID"];
                    cdmaheader = senderid;
                    message = this.Request.Form["Message"];
                }
                if (username == null || password == null || (mobileno == null || senderid == null) || message == null)
                {
                    this.Response.Write("Code=0 SendSMS Pageload");
                }
                else
                {
                    ____logconfig.Log_Write(____logconfig.LogLevel.DEBUG, 77, "username==>" + username + "password==>" + password + "mobileno===>" + mobileno + "semderID===>" + senderid + "cdmaheader==>" + cdmaheader + "message==>" + message);
                    var sql = "select username,passkey,Activated from userdetails where username like binary ?global.username and passkey like binary ?global.passkey";
                    var param = new string[2] { username, password };
                    DataSet dataSet = DL.DL_ExecuteQuery(sql, param);
                    if (dataSet.Tables[0].Rows.Count > 0)
                    {
                        if (dataSet.Tables[0].Rows[0]["Activated"].ToString() == "1")
                        {
                            if (Util.MatchTemplate(username, senderid, message))
                            {

                                Dictionary<string, string> dictionary = new Dictionary<string, string>();

                                foreach (string key in mobileno.Split(','))
                                {
                                    if (!dictionary.ContainsKey(key))
                                        dictionary.Add(key, key);
                                }
                                string mobile_no = string.Join(",", dictionary.Keys.ToList<string>());
                                if (cdmaheader == null)
                                    cdmaheader = senderid;

                                string messageid = dataSet.Tables[0].Rows[0]["username"].ToString() + "-" + this.GenerateId();
                                int num = importnum.importdata(username, password, senderid, cdmaheader, message, mobileno, messageid);
                                if (num > 0)
                                    this.Response.Write("MessageSent GUID=\"" + messageid + "\" SUBMITDATE=\"" + DateTime.Now.GetDateTimeFormats()[84] + "\"");
                                else
                                    this.Response.Write("Code=0 SendSMS Pageload");
                            }
                            else
                                this.Response.Write("Code=0 Template Matching failed");

                        }
                        else
                            this.Response.Write("Code=0 User Account De-Activated");
                    }
                    else
                        this.Response.Write("Invalid username or password");
                }
            }
            catch (Exception ex)
            {
                this.Response.Write("Code=0 Internal exception occured");
                this.errorlogs.WriteAsync("exception.txt", DateTime.Now.ToString() + "On SMSSUBMIT Page Main method" + ex.ToString());
            }
        }


        public string GenerateId()
        {
            try
            {
                long num1 = 1;
                foreach (byte num2 in Guid.NewGuid().ToByteArray())
                    num1 *= (long)((int)num2 + 1);
                return string.Format("{0:x}", (object)(num1 - DateTime.Now.Ticks));
            }
            catch (Exception ex)
            {
                this.errorlogs.WriteAsync("exception.txt", DateTime.Now.ToString() + "On SMSSUBMIT Page GemerateId method" + ex.ToString());
                return "";
            }
        }
    }
}