using ScanWeb;
using System;
using System.Collections.Generic;
using System.Data;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using Newtonsoft.Json;

namespace YouDaoEnglishWord
{
    class Program
    {
        static void Main(string[] args)
        {
            string ip = "";
            int port = 0;
            GetMyIpPort(ref ip, ref port);

            string filePath = Environment.CurrentDirectory + "\\sat.txt";
            List<string> wordList = DataHelper.ReadWords(filePath);

            Dictionary<string, string> dic = new Dictionary<string, string>();
            for (int i = 0; i < wordList.Count; i++)
            {
                if (i % 40 == 0)
                    GetMyIpPort(ref ip, ref port);

                int count = 0;
                string word = wordList[i];
                string result = "\"errorCode\":50";
                while ((string.IsNullOrEmpty(result) || result.Contains("\"errorCode\":50"))
                    && count <= 5)
                {

                    Thread.Sleep(100);
                    result = GetTrasnResult(word, ip, port);
                    if (string.IsNullOrEmpty(result) || result.Contains("\"errorCode\":50"))
                        GetMyIpPort(ref ip, ref port);
                    count++;
                }

                //Thread.Sleep(1000);
                Console.WriteLine(result);
                dic[word] = result;
                DataHelper.WriteLog(result);

                //// 移除
                //wordList.Remove(word);
                //i--;
                //DataHelper.WriteList(wordList);
            }

            //DataTable dt = new DataTable();
            //dt.Columns.Add("word");
            //dt.Columns.Add("new");
            //foreach (string key in dic.Keys)
            //{
            //    DataRow dr = dt.NewRow();
            //    dr["word"] = key;
            //    dr["new"] = dic[key];
            //    dt.Rows.Add(dr);
            //}
            //DataHelper.ExportData(dt, Environment.CurrentDirectory + "\\sat22.xlsx");

            //Console.WriteLine(GetTrasnResult("client"));
            //Console.Read();
        }

        public static void GetMyIpPort(ref string ip, ref int port)
        {
            string url =
                "http://api.ip.data5u.com/dynamic/get.html?order=7000cbf17bb5c8349ee19886bf92906e&json=1&sep=3";
            HttpRequestClient s = new HttpRequestClient(true);
            string result = s.httpGet(url, "", "", "", 0).Replace("\"", "");

            if (!string.IsNullOrEmpty(result) && result.Contains("ip"))
            {
                // var cls = JsonConvert.DeserializeObject<EntityModel>(result);
                int indexIp = result.IndexOf("ip") + 3;
                int indexNum = result.IndexOf(",");
                ip = result.Substring(indexIp, indexNum - indexIp);

                int indexPort = result.IndexOf("port") + 5;
                indexNum = result.IndexOf("ttl") - 1;
                port = int.Parse(result.Substring(indexPort, indexNum - indexPort));
            }
        }

        public static string GetTimeStamp(System.DateTime time)
        {
            long ts = ConvertDateTimeToInt(time);
            return ts.ToString();
        }

        /// <summary>  
        /// 将c# DateTime时间格式转换为Unix时间戳格式  
        /// </summary>  
        /// <param name="time">时间</param>  
        /// <returns>long</returns>  
        public static long ConvertDateTimeToInt(System.DateTime time)
        {
            System.DateTime startTime =
                TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1, 0, 0, 0, 0));
            long t = (time.Ticks - startTime.Ticks) / 10000; //除10000调整为13位      
            return t;

        }

        public static string MD5Encrypt32(string password)
        {
            string cl = password;
            string pwd = "";
            MD5 md5 = MD5.Create();

            byte[] s = md5.ComputeHash(Encoding.UTF8.GetBytes(cl));
            for (int i = 0; i < s.Length; i++)
            {
                pwd = pwd + s[i].ToString("X");
            }

            return pwd.ToLower();
        }

        static (string salt, string sign, string ts, string bv) GetTuple(string word)
        {

            string bv = MD5Encrypt32(
                "5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/72.0.3626.121 Safari/537.36");
            DateTime now = DateTime.Now;
            long inow = ConvertDateTimeToInt(now);
            string ts = GetTimeStamp(now);
            Random ra = new Random(unchecked((int)DateTime.Now.Ticks));

            string salt = (inow + 10).ToString();
            string sign = MD5Encrypt32("fanyideskweb" + word + salt.ToString() + "1L5ja}w$puC.v_Kz3@yYn");
            return (salt, sign, ts, bv);
        }

        static string GetTrasnResult(string word, string ip, int port)
        {
            string headers = @"
Accept: application/json, text/javascript, */*; q=0.01
Accept-Encoding: gzip, deflate
Accept-Language: zh-CN,zh;q=0.9
Connection: keep-alive
Content-Length: 260
Content-Type: application/x-www-form-urlencoded; charset=UTF-8
Cookie: OUTFOX_SEARCH_USER_ID=800710354@10.168.8.63; JSESSIONID=aaa_x27J5TyHngke62VLw; OUTFOX_SEARCH_USER_ID_NCOO=1318048763.8392925; ___rl__test__cookies=1552374975385
Host: fanyi.youdao.com
Origin: http://fanyi.youdao.com
Referer: http://fanyi.youdao.com/
User-Agent: Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/72.0.3626.121 Safari/537.36
X-Requested-With: XMLHttpRequest
";
            string url = "http://fanyi.youdao.com/translate_o?smartresult=dict&smartresult=rule";
            HttpRequestClient s = new HttpRequestClient(true);

            var tup = GetTuple(word);
            string content = $"i={word}&" +
                             "from=AUTO&" +
                             "to=AUTO&" +
                             "smartresult=dict&" +
                             "client=fanyideskweb&" +
                             $"salt={tup.salt}&" +
                             $"sign={tup.sign}&" +
                             $"ts={tup.ts}&" +
                             $"bv={tup.bv}&" +
                             "doctype=json&" +
                             "version=2.1&" +
                             "keyfrom=fanyi.web&" +
                             "action=FY_BY_CLICKBUTTION&" +
                             "typoResult=false";
            return s.httpPost(url, headers, content, Encoding.UTF8, ip, port);

        }
    }

    public class EntityModel
    {
        public string ip { get; set; }

        public string port { get; set; }

        public string tt1 { get; set; }
    }

}
