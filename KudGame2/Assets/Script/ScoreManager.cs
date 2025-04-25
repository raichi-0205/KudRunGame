using UnityEngine;
using Kud.Common;

namespace Kud.MainGame
{
    class ScoreManager : Singleton<ScoreManager>
    {
        private int score = 0;
        public int Score { get { return score; } set { score = value; GameManager.Instance.ScoreUI.UpdateScore(); } }
        public int HiScore { get; set; } = 0;
        public int SocialHiScore { get; set; } = 0;

        const string dataKey = "HiScore";

        public ScoreManager()
        {
            ScoreLoad();
        }

        /// <summary>
        /// ハイスコアの読み込み
        /// </summary>
        public void ScoreLoad()
        {
            HiScore = PlayerPrefs.GetInt(dataKey);
        }

        /// <summary>
        /// スコア更新
        /// </summary>
        public void ScoreUpdate()
        {
            // スコアを更新する
            if(Score > HiScore)
            {
                PlayerPrefs.SetInt(dataKey, score);
                HiScore = score;
            }
        }

        /// <summary>
        /// ハイスコア削除
        /// </summary>
        public void HiScoreDelete()
        {
            PlayerPrefs.DeleteKey(dataKey);
        }
    }
}
