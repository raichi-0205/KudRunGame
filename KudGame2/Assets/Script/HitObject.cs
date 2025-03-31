using UnityEngine;

namespace Kud.MainGame
{
    public class HitObject : MonoBehaviour
    {
        [SerializeField] Collider2D collider;
        [SerializeField] protected float speed;
        public float Speed { get { return speed; } }

        /// <summary>
        /// 初期化処理
        /// </summary>
        /// <param name="_col">生成列</param>
        /// <param name="_speed">移動速度</param>
        public virtual void Initialize(int _col, float _speed)
        {

            speed = _speed;
        }

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        protected virtual void Start()
        {

        }

        // Update is called once per frame
        protected virtual void Update()
        {

        }
    }
}