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

                    //var res = Util.ValidateMobileNumbers(mobileno);
                    ____logconfig.Log_Write(____logconfig.LogLevel.DEBUG, 77, "username==>" + username + "password==>" + password + "mobileno===>" + mobileno + "semderID===>" + senderid + "cdmaheader==>" + cdmaheader + "message==>" + message);
                    var sql = "select user_id,username,passkey,Activated from user_master where username like binary ?global.username and passkey like binary ?global.passkey";
                    var param = new string[2] { username, password };
                    DataSet dataSet = DL.DL_ExecuteQuery(sql, param);
                    if (dataSet.Tables[0].Rows.Count > 0)
                    {
                        var userId = dataSet.Tables[0].Rows[0]["user_id"].ToString();
                        string messageid = dataSet.Tables[0].Rows[0]["username"].ToString() + "-" + this.GenerateId();
                        var isUnicode = Util.ContainsUnicodeCharacter(message);
                        if (dataSet.Tables[0].Rows[0]["Activated"].ToString() == "1")
                        {
                            Dictionary<string, string> dictionary = new Dictionary<string, string>();

                            foreach (string key in mobileno.Split(','))
                            {
                                var validatedMobileNo = Util.ValidateMobileNumbers(key);
                                if (validatedMobileNo != "" && !dictionary.ContainsKey(validatedMobileNo))
                                    dictionary.Add(validatedMobileNo, validatedMobileNo);
                            }
                            if (dictionary.Count != 0)
                            {
                                string mobile_no = string.Join(",", dictionary.Keys.ToList<string>());
                                if (cdmaheader == null)
                                    cdmaheader = senderid;

                                if (isUnicode)
                                {
                                    int num = importnum.importdata(username, password, senderid, cdmaheader, message, mobileno, messageid, 2);
                                    if (num > 0)
                                        this.Response.Write("MessageSent GUID=\"" + messageid + "\" SUBMITDATE=\"" + DateTime.Now.GetDateTimeFormats()[84] + "\"");
                                    else
                                        this.Response.Write("Code=0 SendSMS Pageload");

                                }
                                else if (Util.isTemplateMatched(senderid, message))
                                {

                                    int num = importnum.importdata(username, password, senderid, cdmaheader, message, mobileno, messageid, 0);
                                    if (num > 0)
                                        this.Response.Write("MessageSent GUID=\"" + messageid + "\" SUBMITDATE=\"" + DateTime.Now.GetDateTimeFormats()[84] + "\"");
                                    else
                                        this.Response.Write("Code=0 SendSMS Pageload");


                                }
                                else
                                {
                                    importnum.ImportInvalidRequest(userId, username, messageid, senderid, message, DateTime.Now.ToString("yyyyMMddHHmmss"), mobileno, "TEMPLATE FAILED.", isUnicode ? "2" : "");
                                    this.Response.Write("Code=0 Template Matching failed");
                                }
                            }
                            else
                            {
                                importnum.ImportInvalidRequest(userId, username, messageid, senderid, message, DateTime.Now.ToString("yyyyMMddHHmmss"), mobileno, "INVALID MOBILE FAILED.", isUnicode ? "2" : "");
                                Response.Write("Code=0 Invalid Mobile Number");
                            }
                        }
                        else
                        {
                            importnum.ImportInvalidRequest(userId, username, messageid, senderid, message, DateTime.Now.ToString("yyyyMMddHHmmss"), mobileno, "USED DEACTIVE FAILED.", isUnicode ? "2" : "");
                            this.Response.Write("Code=0 User Account De-Activated");
                        }
                    }
                    else
                    {
                        this.Response.Write("Invalid username or password");
                    }
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