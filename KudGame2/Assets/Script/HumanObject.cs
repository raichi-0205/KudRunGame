using UnityEngine;
using System.Collections;

namespace Kud.MainGame
{
    public class HumanObject : HitObject
    {
        bool isHit = false;
        Vector3 displayOverPos = Vector3.zero;
        Vector3 displayOverNormal = Vector3.zero;
        [SerializeField] float rotationAxis = 1.0f;

        public override void Initialize(int _col, float _speed)
        {
            objectType = GameManager.OBJECT_TYPE.Human;
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
            if (isHit) 
            {
                transform.localEulerAngles += new Vector3(0, 0, rotationAxis);
                transform.position += displayOverNormal * 10.0f * Time.deltaTime;
                Vector3 pos = Camera.main.WorldToScreenPoint(transform.position);
                if ((transform.position.x < displayOverPos.x && displayOverNormal.x < 0) || (transform.position.x > displayOverPos.x && displayOverNormal.x > 0))
                {
                    isHit = false;
                    gameObject.SetActive(false);
                    GameManager.Instance.UnActiveObject(objectType);
                    transform.localRotation = Quaternion.identity;
                }
            }
            else
            {
                base.Update();
            }
        }

        protected override void OnHitPlayer(Player _player)
        {
            base.OnHitPlayer(_player);
            if (_player.IsAwaiking)
            {
                Vector2 scale = Camera.main.WorldToScreenPoint(transform.localScale);                       // オブジェクトのスクリーンサイズ
                float width = transform.position.x < 0 ? 0 - scale.x : Camera.main.pixelWidth + scale.x;    // 飛ばす左右方向
                displayOverPos = Camera.main.ScreenToWorldPoint(new Vector3(width, 0, 0));                  // 飛ばす先のワールド座標
                displayOverNormal = (displayOverPos - _player.transform.position).normalized;               // 飛ばす方向の単位ベクトル
                displayOverNormal = new Vector3(displayOverNormal.x, Mathf.Abs(displayOverNormal.y));       // 飛ばす方向の単位ベクトルのy軸反転
                GameManager.Instance.BlowAddScore();                // スコア加算
                isHit = true;
                Sound.SoundManager.Instance.PlaySound(Sound.SoundManager.SE.Critical);
            }
            else
            {
                GameManager.Instance.AllObjectStop();
                Sound.SoundManager.Instance.PlaySound(Sound.SoundManager.SE.BadHit);
            }
        }
    }
}