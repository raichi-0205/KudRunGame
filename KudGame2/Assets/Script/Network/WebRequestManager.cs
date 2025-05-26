using UnityEngine;
using UnityEngine.Events;
using System.Threading.Tasks;
using UnityEngine.Networking;
using System;

namespace Kud.Network
{
    public class WebRequestManager
    {
        string url = "https://ricproject.stars.ne.jp/KudRunGame/";
        float timeout = 3.0f;

        public async Task StartRequest(string _apiName, WWWForm _data, Action<string> _callBack)
        {
            // 送信先とデータを決める
            UnityWebRequest www = UnityWebRequest.Post(url + _apiName, _data);
            Debug.Log($"送信先:{url + _apiName}");

            float time = 0;

            var op = www.SendWebRequest();
            while (!op.isDone)
            {
                time += Time.deltaTime;
                if (time > timeout)
                {
                    Debug.LogError("タイムアウト");
                    return;
                }
                await Task.Yield();
            }
            Debug.Log("送信完了");

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.LogError(www.error);
            }
            else
            {
                _callBack(www.downloadHandler.text);
            }
        }
    }
}