using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace EnterpriseBot.Service
{
    public class BertClient
    {
        public class BertQuery
        {
            [JsonProperty("question")]
            public string question { get; set; }

            [JsonProperty("context")]
            public string context { get; set; }
        }
        public class BertAnswer
        {
            [JsonProperty("answer")]
            public string answer { get; set; }
        }
        public async Task<string> PassageQueryAsync(string query)
        {
            var url = "https://hackathon2019.blob.core.windows.net/bert/HackathonWinnerFAQ.txt";
            var httpClient = new HttpClient();
            var response = await httpClient.GetStringAsync(url).ConfigureAwait(false);
            var bertQuery = new BertQuery()
            {
                question = query,
                context = response
            };

            var answer = await BertRequestAsync(bertQuery);
            return answer;
        }

        public async Task<string> HttpPostRequestAsync(string url, string postData)
        {
            string strPostReponse = string.Empty;
            try
            {
                var postRequest = CreatePostHttpWebRequest(url, postData);
                var postResponse = await postRequest.GetResponseAsync() as HttpWebResponse;
                strPostReponse = GetHttpResponse(postResponse, "POST");
            }
            catch (Exception ex)
            {
                strPostReponse = ex.Message;
            }
            return strPostReponse;
        }

        private static HttpWebRequest CreatePostHttpWebRequest(string url, string postData)
        {
            var postRequest = HttpWebRequest.Create(url) as HttpWebRequest;
            postRequest.KeepAlive = false;
            postRequest.Timeout = 5000;
            postRequest.Method = "POST";
            postRequest.ContentType = "application/json";
            postRequest.ContentLength = postData.Length;
            postRequest.AllowWriteStreamBuffering = false;
            StreamWriter writer = new StreamWriter(postRequest.GetRequestStream(), Encoding.ASCII);
            writer.Write(postData);
            writer.Flush();
            return postRequest;
        }

        private static string GetHttpResponse(HttpWebResponse response, string requestType)
        {
            var responseResult = "";
            const string post = "POST";
            string encoding = "UTF-8";
            if (string.Equals(requestType, post, StringComparison.OrdinalIgnoreCase))
            {
                encoding = response.ContentEncoding;
                if (encoding == null || encoding.Length < 1)
                {
                    encoding = "UTF-8";
                }
            }
            using (StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.GetEncoding(encoding)))
            {
                responseResult = reader.ReadToEnd();
            }
            return responseResult;
        }

        public async Task<string> BertRequestAsync(BertQuery query)
        {
            var url = "http://10.139.139.82:5001/BERT";

            var response = await HttpPostRequestAsync(url, JsonConvert.SerializeObject(query));
            return JsonConvert.DeserializeObject<BertAnswer>(response).answer;
        }
    }
}
