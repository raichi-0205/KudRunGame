using UnityEngine;
using System.Collections;

namespace Kud.MainGame
{
    public class HurdleObject : HitObject
    {
        public override void Initialize(int _col, float _speed)
        {
            objectType = GameManager.OBJECT_TYPE.Hurdle;
            base.Initialize(_col, _speed);
        }

        // Use this for initialization
        protected override void Start()
        {
            base.Start();
        }

        // Update is called once per frame
        protected override void Update()
        {
            base.Update();
        }

        /// <summary>
        /// 長さの設定
        /// </summary>
        /// <param name="_length"></param>
        public void SetLength(float _length)
        {
            transform.localScale = new Vector2(transform.localScale.x, _length);
        }
    }
}