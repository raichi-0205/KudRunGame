using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

namespace Kud.MainGame
{
    public class ResultMenu : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI score;
        [SerializeField] TextMeshProUGUI hiScore;
        [SerializeField] TextMeshProUGUI socialHiScore;
        [SerializeField] Button retryButton;

        /// <summary>
        /// 初期化処理
        /// </summary>
        public void Initialize()
        {
            gameObject.SetActive(false);
            retryButton.onClick.AddListener(() =>
            {
                SceneManager.LoadScene(0);          // このシーンを読み込みなおす
            });
        }

        /// <summary>
        /// リザルト画面を開く
        /// </summary>
        public void Open()
        {
            ScoreManager.Instance.ScoreUpdate();

            gameObject.SetActive(true);
            score.text = ScoreManager.Instance.Score.ToString();
            hiScore.text = ScoreManager.Instance.HiScore.ToString();
            socialHiScore.text = ScoreManager.Instance.SocialHiScore.ToString();
        }
    }
}