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

        public bool IsUpdateSocialHiScore { get; private set; } = false;

        const string dataKey = "HiScore";

        Network.UpdateScore updateScore = new Network.UpdateScore();

        public ScoreManager()
        {
            ScoreLoad();
            updateScore.CallBack = (res) =>
            {
                SocialHiScore = res.__dat.DataValue;
                IsUpdateSocialHiScore = res.__dat.isUpdate;
            };
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
        /// スコア送信
        /// </summary>
        /// <returns></returns>
        public async System.Threading.Tasks.Task SendScore()
        {
            await updateScore.SendStart(score);
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
