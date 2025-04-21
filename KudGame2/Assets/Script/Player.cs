using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace Kud.MainGame
{
    public class Player : MonoBehaviour
    {
        [SerializeField] BoxCollider2D collider;
        public BoxCollider2D Collider { get { return collider; } }

        [SerializeField] SpriteRenderer spriteRenderer;
        [SerializeField] Color defaultColor = Color.white;
        [SerializeField] Color awaikingColor = Color.yellow;
        [SerializeField] bool isAwaiking = false;
        [SerializeField] float awaikingTimer = 10.0f;
        [SerializeField] float currentAwaikingTime = 0;

        [SerializeField] int colum;                 // 初期列
        
        public bool IsAwaiking { get { return isAwaiking; } }
        public int Colum { get { return colum; } }
        private List<float> linePosxs;

        [SerializeField] float moveTime = 0.25f;
        float length = 0;
        float speed = 0;

        // コントローラー
        [SerializeField] Button leftButton;
        [SerializeField] Button rightButton;

        private bool isStop = false;

        public void Initialize()
        {
            leftButton.onClick.AddListener(() => { OnButton(true); });
            rightButton.onClick.AddListener(() => { OnButton(false); });

            linePosxs = MapManager.Instance.LinePosxs;
            transform.position = new Vector3(linePosxs[colum], transform.position.y, transform.position.z);
        }

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            if (isStop)
            {
                return;
            }

            Move();

            Awaiking();

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

        private void Awaiking()
        {
            if (isAwaiking)
            {
                currentAwaikingTime -= Time.deltaTime;
                if (currentAwaikingTime < 0)
                {
                    isAwaiking = false;
                    spriteRenderer.color = defaultColor;
                }
                else if (currentAwaikingTime < awaikingTimer / 4)
                {
                    if ((int)(currentAwaikingTime * 10) % 2 == 0)
                    {
                        spriteRenderer.color = defaultColor;
                    }
                    else
                    {
                        spriteRenderer.color = awaikingColor;
                    }
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

        public void CatchProtein()
        {
            spriteRenderer.color = awaikingColor;
            isAwaiking = true;
            currentAwaikingTime = awaikingTimer;
        }

        public void Stop()
        {
            isStop = true;
        }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(Player))]
    public class PlayerEditor : UnityEditor.Editor
    {
        float posx = 0;
        List<float> linePosxs;

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