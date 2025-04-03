using UnityEngine;

namespace Kud.MainGame
{
    public class HitObject : MonoBehaviour
    {
        [SerializeField] protected Collider2D collider;
        [SerializeField] protected float speed;
        public float Speed { get { return speed; } }
        protected float overUnderY = 0;
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
        }

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        protected virtual void Start()
        {

        }

        // Update is called once per frame
        protected virtual void Update()
        {
            transform.position += Vector3.down * speed * Time.deltaTime;
            if(transform.position.y < overUnderY)
            {
                gameObject.SetActive(false);
                GameManager.Instance.UnActiveObject(objectType);
            }
        }
    }
}