using UnityEngine;

namespace Kud.MainGame
{
    public class HitObject : MonoBehaviour
    {
        [SerializeField] protected BoxCollider2D collider;
        public BoxCollider2D Collider { get { return collider; } }
        [SerializeField] protected float speed;
        public float Speed { get { return speed; } }
        [SerializeField] protected int col;         // 生成された列
        public int Col { get { return col; } }
        [SerializeField] protected bool isInDisplay = false;
        public bool IsInDisplay { get { return isInDisplay; } }
        protected float overUnderY = 0;
        protected float overTopY = float.MaxValue;
        protected GameManager.OBJECT_TYPE objectType;

        /// <summary>
        /// 初期化処理
        /// </summary>
        /// <param name="_col">生成列</param>
        /// <param name="_speed">移動速度</param>
        public virtual void Initialize(int _col, float _speed)
        {
            gameObject.SetActive(true);
            transform.position = new Vector3(MapManager.Instance.LinePosxs[_col], MapManager.Instance.GetOverDisplayTop(transform.localScale.y), 0);
            speed = _speed;
            overUnderY = MapManager.Instance.GetOverDisplayUnder(transform.localScale.y);
            overTopY = MapManager.Instance.GetOverDisplayTop(0);
            col = _col;
            isInDisplay = false;
        }

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        protected virtual void Start()
        {

        }

        // Update is called once per frame
        protected virtual void Update()
        {
            transform.position += Vector3.down * speed * Time.deltaTime;
            if (transform.position.y < overUnderY)
            {
                // オブジェクトを不活性にする
                gameObject.SetActive(false);
                GameManager.Instance.UnActiveObject(objectType);
            }
            else
            {
                // 画面内に入っているか
                if(!isInDisplay && transform.position.y + transform.localScale.y / 2 < overTopY)
                {
                    isInDisplay = true;
                }

                float left = transform.position.x - transform.localScale.x / 2;
                float right = transform.position.x + transform.localScale.x / 2;
                float top = transform.position.y + transform.localScale.y / 2;
                float bottom = transform.position.y - transform.localScale.y / 2;

                // プレイヤーとの当たり判定
                Player player = GameManager.Instance.Player;
                if(CheckSquareCollision(player.gameObject, left, right, top, bottom))
                {
                    OnHitPlayer(player);
                    return;
                }

                foreach (HumanObject humanObj in GameManager.Instance.HumanObjects)
                {
                    if(humanObj == this)
                    {
                        continue;
                    }

                    if (CheckThinkingCollision(humanObj.gameObject, transform.position.x, top))
                    {
                        if (CheckSquareCollision(humanObj.gameObject, left, right, top, bottom))
                        {
                            OnHitObject(humanObj);
                        }
                    }
                }

                foreach (ProteinObject proteinObj in GameManager.Instance.ProteinObjects)
                {
                    if (proteinObj == this)
                    {
                        continue;
                    }

                    if (CheckThinkingCollision(proteinObj.gameObject, transform.position.x, top))
                    {
                        if (CheckSquareCollision(proteinObj.gameObject, left, right, top, bottom))
                        {
                            OnHitObject(proteinObj);
                        }
                    }
                }

                foreach (HurdleObject hurdleObj in GameManager.Instance.HurdleObjects)
                {
                    if (hurdleObj == this)
                    {
                        continue;
                    }

                    if(CheckThinkingCollision(hurdleObj.gameObject, transform.position.x, top))
                    {
                        if (CheckSquareCollision(hurdleObj.gameObject, left, right, top, bottom))
                        {
                            OnHitObject(hurdleObj);
                        }
                    }
                }

            }
        }

        /// <summary>
        /// 四頂点の当たり判定
        /// </summary>
        /// <param name="_gameObject"></param>
        /// <param name="_left"></param>
        /// <param name="_right"></param>
        /// <param name="_top"></param>
        /// <param name="_bottom"></param>
        /// <returns></returns>
        protected bool CheckSquareCollision(GameObject _gameObject, float _left, float _right, float _top, float _bottom)
        {
            float left = _gameObject.transform.position.x - _gameObject.transform.localScale.x / 2;
            float right = _gameObject.transform.position.x + _gameObject.transform.localScale.x / 2;
            float top = _gameObject.transform.position.y + _gameObject.transform.localScale.y / 2;
            float bottom = _gameObject.transform.position.y - _gameObject.transform.localScale.y / 2;

            if ((left <= _right && right >= _right) || (left <= _left && right >= _left))
            {
                if ((top >= _top && bottom <= _top) || (top >= _bottom && bottom <= _bottom))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 当たり判定を取るかの判定
        /// </summary>
        /// <param name="_gameObject"></param>
        /// <param name="_x"></param>
        /// <param name="_top"></param>
        /// <returns>当たり判定を取る</returns>
        protected bool CheckThinkingCollision(GameObject _gameObject, float _x, float _top)
        {
            // 違うX軸にいるか
            if(_gameObject.transform.position.x != _x)
            {
                return false;
            }

            // 自分より上か
            float bottom = _gameObject.transform.position.y - _gameObject.transform.localScale.y / 2;
            if(bottom > _top)
            {
                return false;
            }

            return _gameObject.activeSelf;
        }

        /// <summary>
        /// プレイヤーとの当たり判定時の処理
        /// </summary>
        protected virtual void OnHitPlayer(Player _player)
        {
            Debug.Log($"[Hit] Player");
        }

        /// <summary>
        /// オブジェクト同士の当たり判定時の処理
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="_obj"></param>
        protected virtual void OnHitObject<T>(T _obj) where T : HitObject
        {
            Debug.Log($"[Hit] Object");
            transform.position += Vector3.up * speed * Time.deltaTime;
            speed = _obj.Speed;
        }
    }
}