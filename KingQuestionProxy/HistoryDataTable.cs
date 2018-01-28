using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using Newtonsoft.Json;
using System.IO;

namespace KingQuestionProxy
{
    static class HistoryDataTable
    {
        private static readonly object syncRoot = new object();

        private static readonly string dataFile = "data.json";

        private static readonly Dictionary<int, HistoryData> dictionary = new Dictionary<int, HistoryData>();

        static HistoryDataTable()
        {
            if (File.Exists(dataFile))
            {
                var json = File.ReadAllText(dataFile, Encoding.UTF8);
                var datas = JsonConvert.DeserializeObject<HistoryData[]>(json);
                foreach (var item in datas)
                {
                    var key = item.QuestionData.quiz.GetHashCode();
                    dictionary.Add(key, item);
                }
            }
        }

        public static void Save()
        {
            lock (syncRoot)
            {
                var json = JsonConvert.SerializeObject(dictionary.Values.ToArray());
                File.WriteAllText(dataFile, json, Encoding.UTF8);
            }
        }

        public static HistoryData TryGet(string title)
        {
            lock (syncRoot)
            {
                HistoryData data;
                var key = title.GetHashCode();
                dictionary.TryGetValue(key, out data);
                return data;
            }
        }

        public static HistoryData TryGet(KingRequest request)
        {
            lock (syncRoot)
            {
                return dictionary.Values.FirstOrDefault(item => item.KingRequest.Equals(request));
            }
        }

        public static bool TryAdd(HistoryData data)
        {
            lock (syncRoot)
            {
                var key = data.QuestionData.quiz.GetHashCode();
                if (dictionary.ContainsKey(key) == false)
                {
                    dictionary.Add(key, data);
                    return true;
                }
                return false;
            }
        }
    }
}
