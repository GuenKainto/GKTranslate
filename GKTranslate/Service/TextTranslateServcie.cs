using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Net;
using System.Windows;
using Newtonsoft.Json;

namespace GKTranslate.Service
{
    internal class TextTranslateServcie
    {
        public string TextTranslated(string input, string from, string to) 
        {
            string rs = "";
            if (!string.IsNullOrEmpty(input))
            {
                var fromLanguage = from;
                var toLanguage = to;
                var url = $"https://translate.googleapis.com/translate_a/single?client=gtx&sl={fromLanguage}&tl={toLanguage}&dt=t&q={HttpUtility.UrlEncode(input)}";
                var webclient = new WebClient{Encoding = Encoding.UTF8};

                var result = webclient.DownloadString(url);
                try
                {
                    var parsedResult = JsonConvert.DeserializeObject<dynamic>(result);
                    foreach (var element in parsedResult[0])
                    {
                        rs += element[0];
                    }
                    return rs;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    return "ERROR";
                }
            }
            return rs;
        }
    }
}
