using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Net.Http;
using UnityEngine;
using System.Net.Http.Headers;

namespace DataBinding
{
    public class RequestWorker
    {
        HttpClient _client = new();

        public bool IsProcessing { get; private set; }

        public async Task<string> GetRequest(string uri)
        {
            try
            {
                IsProcessing = true;

                _client.DefaultRequestHeaders.Accept.Clear();
                _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                // 指定したURIに対してリクエストを送信し、応答を文字列で読み取る
                using HttpResponseMessage response = await _client.GetAsync(uri);
                response.EnsureSuccessStatusCode(); // 通信エラーで例外をthrowしてくれる。
                string content = await response.Content.ReadAsStringAsync();

                IsProcessing = false;
                return content;
            }
            catch (HttpRequestException e)
            {
                Debug.LogError("\n通信エラー: " + e.Message);
                IsProcessing = false;
                return null;
            }
        }
    }
}
