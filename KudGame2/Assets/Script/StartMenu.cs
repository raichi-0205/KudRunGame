using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace Kud.MainGame
{
    public class StartMenu : MonoBehaviour
    {
        [SerializeField] Button startButton;

        private void Awake()
        {
            Initialize();
        }

        private void Initialize()
        {
            gameObject.SetActive(true);
            startButton.onClick.AddListener(() =>
            {
                gameObject.SetActive(false);
                GameManager.Instance.GameStart();
            });
        }

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}