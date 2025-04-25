using UnityEngine;
using System.Collections;

namespace Kud.MainGame
{
    public class BackGround : MonoBehaviour
    {
        [SerializeField] GameObject backGround;
        [SerializeField] SpriteRenderer spriteRenderer;
        private float time = 0;
        public void Initialize()
        {
            if (backGround != null)
            {
                backGround.transform.localScale = Camera.main.ScreenToWorldPoint(new Vector2(Camera.main.pixelWidth, Camera.main.pixelHeight)) * 2;
            }
        }

        private void Update()
        {
            if (GameManager.Instance.IsGameStart)
            {
                spriteRenderer.material.SetFloat("_Speed", GameManager.Instance.Speed * 2);
                time += Time.deltaTime;
            }
            else
            {
                spriteRenderer.material.SetFloat("_Speed", 0);
                time = 0;
            }
            spriteRenderer.material.SetFloat("_DeltaTime", time);
        }
    }
}