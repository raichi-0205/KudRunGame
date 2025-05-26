using System;
using System.Collections.Generic;
using UnityEngine;

namespace Kud.Network
{
    public class UpdateScore
    {
        const string apiName = "UpdateScore.php";
        Response response = null;

        public Action<Response> CallBack;

        public async System.Threading.Tasks.Task SendStart(int _score)
        {
            WWWForm data = new WWWForm();
            data.AddField("score", _score);

            WebRequestManager webRequest = new WebRequestManager();
            Debug.Log($"送信開始");
            await webRequest.StartRequest(apiName, data, Recv);
        }

        public void Recv(string _result)
        {
            response = JsonUtility.FromJson<Response>(_result);
            Debug.Log($"[Net] score:{response.__dat.DataValue}");
            if(CallBack != null)
            {
                CallBack.Invoke(response);
            }
        }

        [System.Serializable]
        public class Response
        {
            public ResponseEntity __dat;
        }

        [System.Serializable]
        public class ResponseEntity
        {
            public int DataValue;
            public bool isUpdate;
        }
    }
}