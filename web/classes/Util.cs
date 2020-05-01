﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Configuration;
using sms_submit;

namespace web.classes
{
    public static class Util
    {
        public static bool MatchTemplate(string username, string senderid, string content)
        {
            var uri = ConfigurationManager.AppSettings["TemplateUri"] + "username=" + username + "&senderid=" + senderid + "&template=" + HttpUtility.UrlEncode(content);
            var response = MakehttpGetCall(uri);
            if (response == "")
                return false;
            if (response.IndexOf("||") == -1)
                return false;
            string[] arr = response.Split(new string[] { "||" }, StringSplitOptions.None);
            if (arr[1].Contains("true") && arr[2].Contains("true"))
                return true;
            return false;
        }

        public static string MakehttpGetCall(string uri)
        {
            try
            {
                var responseFromServer = "";
                WebRequest webreq = WebRequest.Create(uri);
                WebResponse resp = webreq.GetResponse();
                using (Stream dataStream = resp.GetResponseStream())
                {
                    using (StreamReader reader = new StreamReader(dataStream))
                    {
                        responseFromServer = reader.ReadToEnd();
                    }
                }
                resp.Close();
                return responseFromServer;
            }
            catch (Exception ex)
            {
                ____logconfig.Error_Write(____logconfig.LogLevel.EXC, 0, ex);
                return "";
            }

        }
    }
}