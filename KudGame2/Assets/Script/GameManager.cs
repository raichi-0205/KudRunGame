using UnityEngine;
using System.Collections.Generic;
using Kud.Common;

namespace Kud.MainGame
{
    public class GameManager : SingletonMonoBehaviour<GameManager>
    {
        public enum OBJECT_TYPE
        {
            Human,          // 人
            Protein,        // プロテイン
            Hurdle,         // ハードル
            Num
        }

        [Header("Player")]
        [SerializeField] Player player;                 // プレイヤー
        public Player Player { get { return player; } }

        [Header("BackGround")]
        [SerializeField] BackGround backGround;

        [Header("Speed")]
        [SerializeField] float speed = 1;               // 現在の速度
        [SerializeField] float maxSpeed = 10;           // 最大速度
        [SerializeField] float second = 60;             // 最高速度に到達するまでの秒数
        [SerializeField] float accele = 1 / 6;          // 加速値      計算で求める
        public float Speed { get { return speed; } }

        [Header("Create Object")]
        [SerializeField, Kud.Editor.NamedArray(typeof(OBJECT_TYPE))] HitObject[] hitObjects;        // 当たり判定のあるオブジェクト
        [SerializeField] int humanMaxNum = 6;           // 画面に出てくるヒューマンオブジェクトの最大数
        [SerializeField] int proteinMaxNum = 1;         // 画面に出てくるプロテインの最大数
        [SerializeField] int hurdleMaxNum = 1;          // 画面に出てくるハードルの最大数
        [SerializeField] CreateThinkingSystem createThinkingSystem;
        [SerializeField] GameObject content;            // 生成したオブジェクトの保管
        public HitObject[] HitObjects { get { return hitObjects; } }

        List<HumanObject> humanObjecs = new List<HumanObject>();
        List<ProteinObject> proteinObjecs = new List<ProteinObject>();
        List<HurdleObject> hurdleObjecs = new List<HurdleObject>();
        public List<HumanObject> HumanObjects { get { return humanObjecs; } }
        public List<ProteinObject> ProteinObjects { get { return proteinObjecs; } }
        public List<HurdleObject> HurdleObjects { get { return hurdleObjecs; } }

        [Header("Created Object")]
        [SerializeField] int humanCurrentNum = 0;       // 画面に出ているヒューマンオブジェクトの数
        [SerializeField] int proteinCurrentNum = 0;     // 画面に出ているプロテインの数
        [SerializeField] int hurdleCurrentNum = 0;      // 画面に出ているハードルの数
        public int HumanCurrentNum { get { return humanCurrentNum; } set { humanCurrentNum = value; } }
        public int ProteinCurrentNum { get { return proteinCurrentNum; } set { proteinCurrentNum = value; } }
        public int HurdleCurrentNum { get { return hurdleCurrentNum; } set { hurdleCurrentNum = value; } }

        [Header("Score")]
        [SerializeField] int lengthAdditive = 1;        // 経過時間ごとの増加数
        [SerializeField] int blowAway = 5;              // 人を飛ばしたときの加算
        private float scoreCount = 0;
        [SerializeField] ScoreUI scoreUI;
        public ScoreUI ScoreUI { get { return scoreUI; } }

        [Header("Result")]
        [SerializeField] ResultMenu resultMenu;

        private bool isGameStart = false;
        public bool IsGameStart { get { return isGameStart; } }

        void Initialize()
        {
            accele = (maxSpeed - speed) / second;       // 最高速度から初期速度を引いて最高速度に到達するまでの時間で割った数値を加速値にする

            MapManager.Instance.Initialize();
            player.Initialize();
            backGround.Initialize();

            // オブジェクト生成
            CreateObject();

            createThinkingSystem.Initialize(humanObjecs, proteinObjecs, hurdleObjecs);

            scoreUI.Initialize();

            ScoreManager.Instance.Score = 0;

            resultMenu.Initialize();
        }

        protected override void Awake()
        {
            base.Awake();
            Initialize();
        }

        // Update is called once per frame
        void Update()
        {
            if (!isGameStart)
            {
                return;
            }

            UpdateSpeed();
            ThinkingCreateObject();
            UpdateScore();
        }

        /// <summary>
        /// ゲーム開始
        /// </summary>
        public void GameStart()
        {
            isGameStart = true;
        }

        /// <summary>
        /// スコア更新
        /// </summary>
        private void UpdateScore()
        {
            if (scoreCount < maxSpeed)
            {
                scoreCount += speed * Time.deltaTime;
            }
            else
            {
                ScoreManager.Instance.Score += lengthAdditive;
                scoreCount = 0;
            }
        }

        /// <summary>
        /// オブジェクトの生成判断
        /// </summary>
        private void ThinkingCreateObject()
        {
            createThinkingSystem.Thinking();
        }

        /// <summary>
        /// オブジェクトの開始
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="_objects"></param>
        public HitObject FindCreateActiveObject<T>(List<T> _objects) where T : HitObject
        {
            for (int i = 0; i < _objects.Count; i++)
            {
                if (!_objects[i].gameObject.activeSelf)
                {
                    return _objects[i];
                }
            }
            return null;
        }

        public void StartObject(HitObject _object, int _col, float _speed)
        {
            _object.Initialize(_col, _speed);
        }

        public void HitObjectCount(OBJECT_TYPE _type)
        {
            switch (_type)
            {
                case OBJECT_TYPE.Human:
                    humanCurrentNum++;
                    break;
                case OBJECT_TYPE.Protein:
                    proteinCurrentNum++;
                    break;
                case OBJECT_TYPE.Hurdle:
                    hurdleCurrentNum++;
                    break;
            }
        }

        /// <summary>
        /// 生成クラス
        /// </summary>
        private void CreateObject()
        {
            float setSpeed = Random.Range(speed / 2, speed);
            for(int i = 0; i < (int)OBJECT_TYPE.Num; i++)
            {
                switch ((OBJECT_TYPE)i)
                {
                    case OBJECT_TYPE.Human:
                        for(int j = 0; j < humanMaxNum; j++)
                        {
                            HumanObject human = (HumanObject)Instantiate(hitObjects[i]);
                            human.transform.SetParent(content.transform, true);
                            human.transform.position = new Vector3(MapManager.Instance.LinePosxs[1], MapManager.Instance.GetOverDisplayTop(human.transform.localScale.y), 0);
                            human.gameObject.SetActive(false);
                            humanObjecs.Add(human);
                        }
                        break;
                    case OBJECT_TYPE.Protein:
                        for(int j= 0; j < proteinMaxNum; j++)
                        {
                            ProteinObject protein = (ProteinObject)Instantiate(hitObjects[i]);
                            protein.transform.SetParent(content.transform, true);
                            protein.transform.position = new Vector3(MapManager.Instance.LinePosxs[1], MapManager.Instance.GetOverDisplayTop(protein.transform.localScale.y), 0);
                            protein.gameObject.SetActive(false);
                            proteinObjecs.Add(protein);
                        }
                        break;
                    case OBJECT_TYPE.Hurdle:
                        for(int j = 0; j < hurdleMaxNum; j++)
                        {
                            HurdleObject hurdle = (HurdleObject)Instantiate(hitObjects[i]);
                            hurdle.transform.SetParent(content.transform, true);
                            hurdle.transform.position = new Vector3(MapManager.Instance.LinePosxs[1], MapManager.Instance.GetOverDisplayTop(hurdle.transform.localScale.y), 0);
                            hurdle.gameObject.SetActive(false);
                            hurdleObjecs.Add(hurdle);
                        }
                        break;
                }
            }
        }

        /// <summary>
        /// 加速
        /// </summary>
        private void UpdateSpeed()
        {
            if(speed >= maxSpeed)
            {
                speed = maxSpeed;
                return;
            }

            speed += accele * Time.deltaTime;
        }

        public void UnActiveObject(OBJECT_TYPE _type)
        {
            switch (_type)
            {
                case OBJECT_TYPE.Human:
                    humanCurrentNum--;
                    break;
                case OBJECT_TYPE.Protein:
                    proteinCurrentNum--;
                    break;
                case OBJECT_TYPE.Hurdle:
                    hurdleCurrentNum--;
                    break;
            }
        }

        /// <summary>
        /// オブジェクトの配置をリセット
        /// </summary>
        public void ObjectReset()
        {
            humanCurrentNum = 0;
            proteinCurrentNum = 0;
            hurdleCurrentNum = 0;
        }

        /// <summary>
        /// 吹っ飛ばし時のスコア加算
        /// </summary>
        public void BlowAddScore()
        {
            ScoreManager.Instance.Score += blowAway;
        }

        /// <summary>
        /// 全てのオブジェクトを停止
        /// </summary>
        public void AllObjectStop()
        {
            ObjectAllStop(humanObjecs);
            ObjectAllStop(proteinObjecs);
            ObjectAllStop(hurdleObjecs);
            player.Stop();
            isGameStart = false;

            resultMenu.Open();

            void ObjectAllStop<T>(List<T> _objects) where T : HitObject
            {
                foreach (HitObject hitObject in _objects)
                {
                    hitObject.ActiveStop();
                }
            }
        }
    }

#if UNITY_EDITOR
    [UnityEditor.CustomEditor(typeof(GameManager))]
    public class GameManagerEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            GameManager gameManager = target as GameManager;
            if (GUILayout.Button("オブジェクトリセット"))
            {
                gameManager.ObjectReset();
            }
        }
    }
#endif
}