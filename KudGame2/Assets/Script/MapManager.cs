using UnityEngine;
using System.Collections.Generic;
using Kud.Common;

namespace Kud.MainGame
{
    public class MapManager : SingletonMonoBehaviour<MapManager>
    {
        [SerializeField] GameObject backGround;
        [SerializeField] float bothEndsLineWidth;   // 端の線の幅
        [SerializeField] float lineWidth;           // 線の幅
        [SerializeField] int split;                 // 分割数
        private List<float> linePosxs;              // 各列の中心点のワールド座標

        public float BothEndsLineWidth { get { return bothEndsLineWidth; } }
        public float LineWidth { get { return lineWidth; } }
        public int Split { get { return split; } }
        public List<float> LinePosxs { get { return linePosxs; } }

        public void Initialize()
        {
            linePosxs = new List<float>();
            for (int i = 0; i < MapManager.Instance.Split; i++)
            {
                linePosxs.Add(MapManager.Instance.ScreenToWorldColumXPos(i));
            }
        }

        protected override void Awake()
        {
            if (Instance != this)
            {
                Destroy(this.gameObject);
            }

            base.Awake();
            DontDestroyOnLoad(this.gameObject);
        }

        /// <summary>
        /// 画面外の上座標を取得
        /// </summary>
        /// <param name="_height">オブジェクトの縦サイズ</param>
        /// <returns></returns>
        public float GetOverDisplayTop(float _height)
        {
            float topPos = Camera.main.pixelHeight;
            return Camera.main.ScreenToWorldPoint(new Vector3(0, topPos, 0)).y + _height / 2;
        }

        public float GetOverDisplayUnder(float _height)
        {
            return Camera.main.ScreenToWorldPoint(new Vector3(0, 0, 0)).y - _height / 2;
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