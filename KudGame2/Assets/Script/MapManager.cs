using UnityEngine;
using Kud.Common;

namespace Kud.MainGame
{
    public class MapManager : SingletonMonoBehaviour<MapManager>
    {
        [SerializeField] float bothEndsLineWidth;   // 端の線の幅
        [SerializeField] float lineWidth;           // 線の幅
        [SerializeField] int split;                 // 分割数

        public float BothEndsLineWidth { get { return bothEndsLineWidth; } }
        public float LineWidth { get { return lineWidth; } }
        public int Split { get { return split; } }

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        /// <summary>
        /// 列の中心座標を求める
        /// </summary>
        /// <param name="_colum"></param>
        /// <returns></returns>
        public float ColumXPos(int _colum)
        {
            float windowWidth = Camera.main.pixelWidth;
            float leftSpace = BothEndsLineWidth;
            float nextSpace = LineWidth;
            int split = Split;
            Debug.Log($"cameraWidth:{windowWidth}");
            float width = (windowWidth - leftSpace * 2 - nextSpace * (split - 1)) / split;
            Debug.Log($"width:{width}");
            float posx = nextSpace * _colum + width * _colum + leftSpace + width / 2;
            Debug.Log($"pos x:{posx}");
            return posx;
        }

        /// <summary>
        /// 列の中心座標を求めてワールド座標を返す
        /// </summary>
        /// <param name="_colum"></param>
        /// <returns></returns>
        public float ScreenToWorldColumXPos(int _colum)
        {
            Vector2 pos = Camera.main.ScreenToWorldPoint(new Vector3(ColumXPos(_colum), 0, 0));
            return pos.x;
        }
    }
}