using Kud.Common;

namespace Kud.MainGame
{
    class ScoreManager : Singleton<ScoreManager>
    {
        private int score = 0;
        public int Score { get { return score; } set { score = value; GameManager.Instance.ScoreUI.UpdateScore(); } }
        public int HiScore { get; set; } = 0;
        public int SocialHiScore { get; set; } = 0;

    }
}
