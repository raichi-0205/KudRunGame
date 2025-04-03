using UnityEngine;
using System.Collections;

namespace Kud.MainGame
{
    public class HumanObject : HitObject
    {

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
            base.Update();
        }
    }
}