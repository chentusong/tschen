using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace JsonTestNet
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            Regex();

            //var obj = JsonConvert.DeserializeObject(result);
            //Newtonsoft.Json.Linq.JObject newObj = obj as Newtonsoft.Json.Linq.JObject;
            //var children = newObj.Children()[0];

            ////解析
            //Newtonsoft.Json.Linq.JObject resultObject = Newtonsoft.Json.Linq.JObject.Parse(result);
            ////转换成列表（取得Content）
            //List<Newtonsoft.Json.Linq.JToken> listJToken = resultObject["content"].Children().ToList();
            ////遍历
            //foreach (var item in listJToken)
            //{
            //}
        }

        public static void Regex()
        {
            string content = "";
            string filePath = Environment.CurrentDirectory + "\\content.txt";
            ReadFile(ref content, filePath);

            string str = "";
            filePath = Environment.CurrentDirectory + "\\sat.txt";
            List<string> wordList = ReadFile(ref str, filePath);

            DataTable dt = new DataTable();
            dt.Columns.Add("word");
            dt.Columns.Add("main");
            dt.Columns.Add("all");

            string patten1 = "tgt\":\"(?<tgt>.*?)\",\"src\":\"(?<src>.*?)\"}";
            Regex reg = new Regex(patten1);

            Dictionary<string, DataRow> dic = new Dictionary<string, DataRow>();

            MatchCollection result = reg.Matches(content);
            foreach (Match temp in result)
            {
                DataRow dr = dt.NewRow();
                dt.Rows.Add(dr);
                dr["word"] = temp.Groups["src"].Value;
                dr["main"] = temp.Groups["tgt"].Value;
                dic[temp.Groups["src"].Value] = dr;

                if (wordList.Contains(temp.Groups["src"].Value))
                {
                    wordList.Remove(temp.Groups["src"].Value);
                }
            }

            string patten2 = "tgt\":\"(?<tgt>.*?)\",\"src\":\"(?<src>.*?)\"}.*?entries\":\\[\"\",(?<entries>.*?)\\],\"type";
            reg = new Regex(patten2);
            result = reg.Matches(content);
            foreach (Match temp in result)
            {
                dic[temp.Groups["src"].Value]["all"] = temp.Groups["entries"].Value;
            }

            str = "";
            foreach (string word in wordList)
            {
                str += word + "\r\n";
            }

            ExcelOperator.ExportData(dt, @"C:\Users\chent\Desktop\youdao.xlsx");
        }

        public static List<string> ReadFile(ref string content, string filePath)
        {

            List<string> wordsList = new List<string>();
            try
            {
                StreamReader sr = new StreamReader(filePath, Encoding.Default);
                String line;
                while ((line = sr.ReadLine()) != null)
                {
                    string word = line.ToString();
                    content += word + "\r\n";
                    if (!wordsList.Contains(word))
                    {
                        wordsList.Add(word);
                    }
                }

                sr.Close();
            }
            catch (Exception ex)
            {
            }

            return wordsList;

        }

    }
}
