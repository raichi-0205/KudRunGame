using UnityEngine;
using System.Collections;

namespace Kud.MainGame
{
    public class BackGround : MonoBehaviour
    {
        [SerializeField] GameObject backGround;
        public void Initialize()
        {
            if (backGround != null)
            {
                backGround.transform.localScale = Camera.main.ScreenToWorldPoint(new Vector2(Camera.main.pixelWidth, Camera.main.pixelHeight)) * 2;
            }
        }
    }
}