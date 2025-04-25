using UnityEngine;
using TMPro;

namespace Kud.MainGame
{
    public class ScoreUI : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI hiScoreText;
        [SerializeField] TextMeshProUGUI scoreText;
        string hiScoreTitle;
        string scoreTitle;

        public void Initialize()
        {
            hiScoreTitle = hiScoreText.text;
            scoreTitle = scoreText.text;

            UpdateHiScore();
        }

        public void UpdateHiScore()
        {
            hiScoreText.text = $"{hiScoreTitle}\n{ScoreManager.Instance.HiScore}";
        }

        public void UpdateScore()
        {
            scoreText.text = $"{scoreTitle}\n{ScoreManager.Instance.Score}";
        }

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}