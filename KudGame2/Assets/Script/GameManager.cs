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

        [Header("Speed")]
        [SerializeField] float speed = 1;               // 現在の速度
        [SerializeField] float maxSpeed = 10;           // 最大速度
        [SerializeField] float second = 60;             // 最高速度に到達するまでの秒数
        [SerializeField] float accele = 1 / 6;          // 加速値      計算で求める

        [Header("Create Object")]
        [SerializeField, Kud.Editor.NamedArray(typeof(OBJECT_TYPE))] HitObject[] hitObjects;        // 当たり判定のあるオブジェクト
        [SerializeField] float warnigTime = 3;          // 警告秒数
        [SerializeField] int humanMaxNum = 6;           // 画面に出てくるヒューマンオブジェクトの最大数
        [SerializeField] int proteinMaxNum = 1;         // 画面に出てくるプロテインの最大数
        [SerializeField] int hurdleMaxNum = 1;          // 画面に出てくるハードルの最大数
        [SerializeField] float hurdleMinLength = 3;     // ハードルの最低の長さ
        [SerializeField] float hurdleMaxLength = 10;    // ハードルの最高の長さ
        [SerializeField] float nextThinkingTime = 0;    // 次に生成を判断する時間
        [SerializeField] float thinkingMinTime = 0.5f;  // 次に生成を判断する時間の最小時間
        [SerializeField] float thinkingMaxTime = 3;     // 次に生成を判断する時間の最大時間
        [SerializeField] float addThinkingTime = 1;     // 生成しないときに次生成判断する時間の指定
        [SerializeField] GameObject content;            // 生成したオブジェクトの保管

        List<HumanObject> humanObjecs = new List<HumanObject>();
        List<ProteinObject> proteinObjecs = new List<ProteinObject>();
        List<HurdleObject> hurdleObjecs = new List<HurdleObject>();

        [Header("Created Object")]
        [SerializeField] int humanCurrentNum = 0;       // 画面に出ているヒューマンオブジェクトの数
        [SerializeField] int proteinCurrentNum = 0;     // 画面に出ているプロテインの数
        [SerializeField] int hurdleCurrentNum = 0;      // 画面に出ているハードルの数

        void Initialize()
        {
            accele = (maxSpeed - speed) / second;       // 最高速度から初期速度を引いて最高速度に到達するまでの時間で割った数値を加速値にする

            MapManager.Instance.Initialize();
            player.Initialize();

            // オブジェクト生成
            CreateObject();
        }

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            Initialize();
        }

        // Update is called once per frame
        void Update()
        {
            UpdateSpeed();
            ThinkingCreateObject();
        }

        /// <summary>
        /// オブジェクトの生成判断
        /// </summary>
        private void ThinkingCreateObject()
        {
            if(nextThinkingTime > 0)
            {
                nextThinkingTime -= Time.deltaTime;
                return;
            }

            OBJECT_TYPE objectNum = (OBJECT_TYPE)Random.Range(0, (int)OBJECT_TYPE.Num);
            switch (objectNum)
            {
                case OBJECT_TYPE.Human:
                    if(humanCurrentNum < humanMaxNum)
                    {
                        Debug.Log($"[Create] Human:{humanCurrentNum}");
                        humanCurrentNum++;
                        StartObject(humanObjecs);
                    }
                    break;
                case OBJECT_TYPE.Protein:
                    if(proteinCurrentNum < proteinMaxNum)
                    {
                        Debug.Log($"[Create] Protein:{proteinCurrentNum}");
                        proteinCurrentNum++;
                        StartObject(proteinObjecs);
                    }
                    break;
                case OBJECT_TYPE.Hurdle:
                    if(hurdleCurrentNum < hurdleMaxNum)
                    {
                        Debug.Log($"[Create] Hurdle:{hurdleCurrentNum}");
                        hurdleCurrentNum++;
                        StartObject(hurdleObjecs);
                    }
                    break;
                default:
                    // 生成しない
                    nextThinkingTime = addThinkingTime;        // 指定秒後に生成判断 
                    return;
            }
            nextThinkingTime = Random.Range(thinkingMinTime, thinkingMaxTime);      // 指定範囲秒を次の生成判断の時間にする
        }

        /// <summary>
        /// オブジェクトの開始
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="_objects"></param>
        private void StartObject<T>(List<T> _objects) where T : HitObject
        {
            for (int i = 0; i < _objects.Count; i++)
            {
                if (!_objects[i].gameObject.activeSelf)
                {
                    _objects[i].Initialize(Random.Range(0, 100) % 4, Random.Range(1.0f, 3.0f));
                    break;
                }
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