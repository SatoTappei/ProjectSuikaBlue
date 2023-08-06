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

                // �w�肵��URI�ɑ΂��ă��N�G�X�g�𑗐M���A�����𕶎���œǂݎ��
                using HttpResponseMessage response = await _client.GetAsync(uri);
                response.EnsureSuccessStatusCode(); // �ʐM�G���[�ŗ�O��throw���Ă����B
                string content = await response.Content.ReadAsStringAsync();

                IsProcessing = false;
                return content;
            }
            catch (HttpRequestException e)
            {
                Debug.LogError("\n�ʐM�G���[: " + e.Message);
                IsProcessing = false;
                return null;
            }
        }
    }
}
