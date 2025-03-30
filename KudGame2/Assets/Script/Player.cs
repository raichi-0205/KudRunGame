using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace Kud.MainGame
{
    public class Player : MonoBehaviour
    {
        // Todo: これらはここに入れるべきじゃない
        [SerializeField] float bothEndsLineWidth;   // 端の線の幅
        [SerializeField] float lineWidth;           // 線の幅
        [SerializeField] int split;                 // 分割数

        public float BothEndsLineWidth { get { return bothEndsLineWidth; } }
        public float LineWidth { get { return lineWidth; } }
        public int Split { get { return split; } }

        // ここからが処理
        [SerializeField] int colum;                 // 初期列
        public int Colum { get { return colum; } }
        public List<float> linePosxs;

        [SerializeField] float moveTime = 0.25f;
        float length = 0;
        float speed = 0;

        // コントローラー
        [SerializeField] Button leftButton;
        [SerializeField] Button rightButton;

        // Use this for initialization
        void Start()
        {
            //linePosxs = new List<float>(split);            // 分割数で領域確保
            //for(int i = 0; i < linePosxs.Count; i++)
            //{
            //    linePosxs[i] = ColumXPos(i);
            //}
            linePosxs = new List<float>();
            for(int i = 0; i < split; i++)
            {
                linePosxs.Add(ScreenToWorldColumXPos(i));
            }

            leftButton.onClick.AddListener(() => { OnButton(true); });
            rightButton.onClick.AddListener(() => { OnButton(false); });
        }

        // Update is called once per frame
        void Update()
        {
            Move();

            // debug
            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                colum--;
                colum = Mathf.Clamp(colum, 0, split - 1);
                transform.position = new Vector3(linePosxs[colum], transform.position.y, transform.position.z);
            }
            else if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                colum++;
                colum = Mathf.Clamp(colum, 0, split - 1);
                transform.position = new Vector3(linePosxs[colum], transform.position.y, transform.position.z);
            }
        }

        private void Move()
        {
            if (length > 0)
            {
                float posx = speed * Time.deltaTime;    // このフレームの移動量を求める
                length -= posx;                         // 進んだ距離分引く
                posx *= transform.position.x < linePosxs[colum] ? 1 : -1;       // 左右の移動方向を決める
                if (length <= 0)
                {
                    transform.position = new Vector3(linePosxs[colum], transform.position.y, transform.position.z);
                    length = 0;
                }
                else
                {
                    transform.position += new Vector3(posx, 0, 0);
                }
            }
        }

        public void OnButton(bool _isLeft)
        {
            if(length > 0)
            {
                // 移動中
                return;
            }
            if (_isLeft)
            {
                colum--;
            }
            else
            {
                colum++;
            }
            colum = Mathf.Clamp(colum, 0, split - 1);
            length = Mathf.Abs(linePosxs[colum] - transform.position.x);
            speed = length / moveTime;        // 必要な速度を求める
        }

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

        public float ScreenToWorldColumXPos(int _colum)
        {
            Vector2 pos = Camera.main.ScreenToWorldPoint(new Vector3(ColumXPos(_colum), 0, 0));
            return pos.x;
        }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(Player))]
    public class PlayerEditor : Editor
    {
        float posx = 0;
        public override void OnInspectorGUI()
        {
            Player player = target as Player;

            base.OnInspectorGUI();
            if (GUILayout.Button("指定列の座標を計算"))
            {
                posx = player.ColumXPos(player.Colum);
            }
            GUILayout.Label(posx.ToString());
            if (GUILayout.Button("反映"))
            {
                Vector2 pos = Camera.main.ScreenToWorldPoint(new Vector3(posx, 0, 0));
                player.transform.position = new Vector3(pos.x, player.transform.position.y, player.transform.position.z);    
            }
        }
    }

#endif
}