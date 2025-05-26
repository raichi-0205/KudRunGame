using System;
using System.Collections.Generic;
using UnityEngine;

namespace Kud.Network
{
    public class GetScore
    {
        const string apiName = "GetScore.php";
        Response response = null;

        public void SendStart(int _score)
        {
            WWWForm data = new WWWForm();

            WebRequestManager webRequest = new WebRequestManager();
            Debug.Log($"送信開始");
            webRequest.StartRequest(apiName, data, Recv);
        }

        public void Recv(string _result)
        {
            response = JsonUtility.FromJson<Response>(_result);
            Debug.Log($"[Net] score:{response.__dat[0].DataValue}");
        }

        [Serializable]
        public class Response
        {
            public List<ResponseEntity> __dat;
        }

        [Serializable]
        public class ResponseEntity
        {
            public int DataValue;
        }
    }
}
