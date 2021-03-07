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
            string username = ""; string password = ""; string mobileno = ""; string senderid = ""; string PEId = ""; string TMId = ""; string templateId = ""; string cdmaheader = ""; string message = "";
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
                    PEId = Request.QueryString["dlt_peid"];
                    TMId = Request.QueryString["dlt_tmid"];
                    templateId = Request.QueryString["dlt_templateid"];
                }
                if (this.Request.HttpMethod == "POST")
                {
                    username = this.Request.Form["UserName"];
                    password = this.Request.Form["password"];
                    mobileno = this.Request.Form["MobileNo"];
                    senderid = this.Request.Form["SenderID"];
                    cdmaheader = senderid;
                    message = this.Request.Form["Message"];
                    PEId = this.Request.Form["dlt_peid"];
                    TMId = this.Request.Form["dlt_tmid"];
                    templateId = this.Request.Form["dlt_templateid"];
                }
                if (username == null || password == null || (mobileno == null || senderid == null) || message == null)
                {
                    this.Response.Write("Code=0 SendSMS Pageload");
                }
                else
                {

                    //var res = Util.ValidateMobileNumbers(mobileno);
                    ____logconfig.Log_Write(____logconfig.LogLevel.DEBUG, 77, "username==>" + username + "password==>" + password + "mobileno===>" + mobileno + "semderID===>" + senderid + "cdmaheader==>" + cdmaheader + "message==>" + message);
                    var sql = "select user_id,username,passkey,Activated,checksenderid,checktemplate,is_dlt_pe_id_check,is_default_sid_check from user_master where username like binary ?global.username and passkey like binary ?global.passkey";
                    var param = new string[2] { username, password };
                    DataSet dataSet = DL.DL_ExecuteQuery(sql, param);
                    if (dataSet.Tables[0].Rows.Count > 0)
                    {
                        var userId = dataSet.Tables[0].Rows[0]["user_id"].ToString();
                        var checksenderId = int.Parse(dataSet.Tables[0].Rows[0]["checksenderid"].ToString());
                        var peIdCheck = int.Parse(dataSet.Tables[0].Rows[0]["is_dlt_pe_id_check"].ToString());
                        var checkTemplate = int.Parse(dataSet.Tables[0].Rows[0]["checktemplate"].ToString());
                        var isdefaultSenderIdCheck = int.Parse(dataSet.Tables[0].Rows[0]["is_default_sid_check"].ToString());
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
                                var dltDetails = Util.CheckSenderIdApproved(checksenderId, username, senderid, isdefaultSenderIdCheck);
                                senderid = dltDetails.Item3;

                                if (dltDetails.Item1)
                                {
                                    if (Util.checkPEId(peIdCheck, PEId, dltDetails.Item2.ToString(), out long validPEID))
                                    {
                                        List<string> getDLTresponse = new List<string>();
                                        getDLTresponse = Util.DltResponse(new string[] { senderid }, new string[] { message }, new string[] { validPEID.ToString() }, new string[] { username });
                                        if (Util.checkTemplateId(checkTemplate, templateId, getDLTresponse.Count > 0 ? getDLTresponse[0] : "0", out long validTemplateId))
                                        {
                                            int num = importnum.importdata(username, password, senderid, cdmaheader, message, mobileno, messageid, 0, validPEID, validTemplateId);
                                            if (num > 0)
                                                this.Response.Write("MessageSent GUID=\"" + messageid + "\" SUBMITDATE=\"" + DateTime.Now.GetDateTimeFormats()[84] + "\"");
                                            else
                                                this.Response.Write("Code=0 SendSMS Pageload");


                                        }
                                        else
                                        {
                                            importnum.ImportInvalidRequest(userId, username, messageid, senderid, message, DateTime.Now.ToString("yyyyMMddHHmmss"), mobileno, "TEMPLATE FAILED.", isUnicode ? "2" : "",1);
                                            this.Response.Write("Code=0 Template Matching failed");
                                        }
                                    }
                                    else
                                    {
                                        importnum.ImportInvalidRequest(userId, username, messageid, senderid, message, DateTime.Now.ToString("yyyyMMddHHmmss"), mobileno, "INVALID PE ID.", isUnicode ? "2" : "",3);
                                        this.Response.Write("Code=0 Invalid PE Id");
                                       
                                    }
                                }
                                else
                                {
                                    importnum.ImportInvalidRequest(userId, username, messageid, senderid, message, DateTime.Now.ToString("yyyyMMddHHmmss"), mobileno, "INVALID SENDER ID.", isUnicode ? "2" : "",3);
                                    this.Response.Write("Code=0 Invalid SenderId");
                                    
                                }
                            }
                            else
                            {
                                importnum.ImportInvalidRequest(userId, username, messageid, senderid, message, DateTime.Now.ToString("yyyyMMddHHmmss"), mobileno, "INVALID MOBILE FAILED.", isUnicode ? "2" : "",2);
                                Response.Write("Code=0 Invalid Mobile Number");
                            }
                        }
                        else
                        {
                            importnum.ImportInvalidRequest(userId, username, messageid, senderid, message, DateTime.Now.ToString("yyyyMMddHHmmss"), mobileno, "USED DEACTIVE FAILED.", isUnicode ? "2" : "",3);
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