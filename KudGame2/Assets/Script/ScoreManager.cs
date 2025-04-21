using Kud.Common;

namespace Kud.MainGame
{
    class ScoreManager : Singleton<ScoreManager>
    {
        public int Score { get; set; } = 0;
        public int HiScore { get; set; } = 0;
        public int SocialHiScore { get; set; } = 0;

    }
}
