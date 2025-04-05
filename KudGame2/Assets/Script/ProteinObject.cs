using UnityEngine;
using System.Collections;

namespace Kud.MainGame
{
    public class ProteinObject : HitObject
    {

        public override void Initialize(int _col, float _speed)
        {
            objectType = GameManager.OBJECT_TYPE.Protein;
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

        protected override void OnHitPlayer(Player _player)
        {
            base.OnHitPlayer(_player);
            _player.CatchProtein();
            gameObject.SetActive(false);
            GameManager.Instance.UnActiveObject(objectType);
        }
    }
}