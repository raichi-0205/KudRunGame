using UnityEngine;
using System.Collections;

namespace Kud.MainGame
{
    public class HurdleObject : HitObject
    {
        [SerializeField] SpriteRenderer sprite;

        public override void Initialize(int _col, float _speed)
        {
            objectType = GameManager.OBJECT_TYPE.Hurdle;
            base.Initialize(_col, _speed);
        }

        // Use this for initialization
        protected override void Start()
        {
            base.Start();
            sprite.size = new Vector2(transform.localScale.x, transform.localScale.y);
            sprite.transform.localScale = new Vector2(sprite.transform.localScale.x / transform.localScale.x, sprite.transform.localScale.y / transform.localScale.y);
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

        protected override void OnHitPlayer(Player _player)
        {
            base.OnHitPlayer(_player);
            GameManager.Instance.AllObjectStop();
            Sound.SoundManager.Instance.PlaySound(Sound.SoundManager.SE.BadHit);
        }
    }
}