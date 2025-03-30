using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace Kud.MainGame
{
    public class Player : MonoBehaviour
    {
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
            for(int i = 0; i < MapManager.Instance.Split; i++)
            {
                linePosxs.Add(MapManager.Instance.ScreenToWorldColumXPos(i));
            }

            leftButton.onClick.AddListener(() => { OnButton(true); });
            rightButton.onClick.AddListener(() => { OnButton(false); });

            transform.position = new Vector3(linePosxs[colum], transform.position.y, transform.position.z);
        }

        // Update is called once per frame
        void Update()
        {
            Move();

            // debug
            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                colum--;
                colum = Mathf.Clamp(colum, 0, MapManager.Instance.Split - 1);
                transform.position = new Vector3(linePosxs[colum], transform.position.y, transform.position.z);
            }
            else if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                colum++;
                colum = Mathf.Clamp(colum, 0, MapManager.Instance.Split - 1);
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
            colum = Mathf.Clamp(colum, 0, MapManager.Instance.Split - 1);
            length = Mathf.Abs(linePosxs[colum] - transform.position.x);
            speed = length / moveTime;        // 必要な速度を求める
        }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(Player))]
    public class PlayerEditor : UnityEditor.Editor
    {
        float posx = 0;
        public override void OnInspectorGUI()
        {
            Player player = target as Player;

            base.OnInspectorGUI();
            if (GUILayout.Button("指定列の座標を計算"))
            {
                posx = MapManager.Instance.ColumXPos(player.Colum);
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