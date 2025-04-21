using UnityEngine;
using System.Collections.Generic;

namespace Kud.MainGame
{
    public class CreateThinkingSystem : MonoBehaviour
    {
        public class Probability
        {
            public int Range = 0;           // 確率範囲
            public int Colum = -1;          // 列
            public Probability(int _range, int _colum)
            {
                Range = _range;
                Colum = _colum;
            }
        }

        [SerializeField] const int line = 4;
        [SerializeField] float warnigTime = 3;          // 警告秒数
        [SerializeField] float[] currentWarningTimes;   // 現在の警告秒数
        [SerializeField] GameObject warningObject;      // 警告のオブジェクト
        [SerializeField] int onceSetObjectMax = 2;      // 一度に生成するオブジェクトの数
        [SerializeField] int humanMaxNum = 6;           // 画面に出てくるヒューマンオブジェクトの最大数
        [SerializeField] int proteinMaxNum = 1;         // 画面に出てくるプロテインの最大数
        [SerializeField] int hurdleMaxNum = 1;          // 画面に出てくるハードルの最大数
        [SerializeField] float hurdleMinLength = 3;     // ハードルの最低の長さ
        [SerializeField] float hurdleMaxLength = 10;    // ハードルの最高の長さ
        [SerializeField] float nextThinkingTime = 0;    // 次に生成を判断する時間
        [SerializeField] float thinkingMinTime = 0.5f;  // 次に生成を判断する時間の最小時間
        [SerializeField] float thinkingMaxTime = 3;     // 次に生成を判断する時間の最大時間
        [SerializeField] float addThinkingTime = 1;     // 生成しないときに次生成判断する時間の指定

        [Header("オブジェクトの速度")]
        [SerializeField] float humanMaxSpeed = 3.0f;    // 人の最大速度
        [SerializeField] float humanMinSpeed = 1.0f;    // 人の最低速度
        [SerializeField] float proteinMaxSpeed = 1.5f;  // プロテインの最高速度
        [SerializeField] float proteinMinSpeed = 1.5f;  // プロテインの最低速度
        [SerializeField] float hurdleMaxSpeed = 1.5f;   // ハードルの最高速度
        [SerializeField] float hurdleMinSpeed = 1.5f;   // ハードルの最低速度

        [Header("非覚醒状態の生成割合")]
        [SerializeField] float humanCreatePer = 25;
        [SerializeField] float proteinCreatePer = 40;
        [SerializeField] float hurdleCreatePer = 35;
        [Header("覚醒状態の生成割合")]
        [SerializeField] float awakingHumanCreatePer = 70;
        [SerializeField] float awakingProteinCreatePer = 10;
        [SerializeField] float awakingHurdleCreatePer = 20;

        List<HumanObject> humanObjecs;
        List<ProteinObject> proteinObjecs;
        List<HurdleObject> hurdleObjecs;
        bool[] hurdleWarningLine;
        GameObject[] warningObjects;
        Dictionary<int, HitObject> outObjects = new Dictionary<int, HitObject>();       // オブジェクトがかぶって出現することはないので一つでいい

        /// <summary>
        /// 初期化処理
        /// </summary>
        /// <param name="_humanObjects"></param>
        /// <param name="_proteinObejcts"></param>
        /// <param name="_hurdleObjects"></param>
        public void Initialize(List<HumanObject> _humanObjects, List<ProteinObject> _proteinObejcts, List<HurdleObject> _hurdleObjects)
        {
            humanObjecs = _humanObjects;
            proteinObjecs = _proteinObejcts;
            hurdleObjecs = _hurdleObjects;
            hurdleWarningLine = new bool[line];
            currentWarningTimes = new float[line];
            warningObjects = new GameObject[line];

            GameObject warningParent = new GameObject("Warnings");
            for(int i = 0; i < line; i++)
            {
                warningObjects[i] = Instantiate(warningObject);
                warningObjects[i].transform.SetParent(warningParent.transform);
                warningObjects[i].transform.position = new Vector3(MapManager.Instance.LinePosxs[i], 0, 0);
                warningObjects[i].SetActive(false);
            }
        }

        public void Update()
        {
            UpdateWarningTime();
        }

        /// <summary>
        /// 警告時間の更新
        /// </summary>
        private void UpdateWarningTime()
        {
            for(int i = 0; i < hurdleWarningLine.Length; i++)
            {
                if (hurdleWarningLine[i])
                {
                    currentWarningTimes[i] += Time.deltaTime;
                    if(currentWarningTimes[i] >= warnigTime)
                    {
                        hurdleWarningLine[i] = false;
                        currentWarningTimes[i] = 0;
                        warningObjects[i].SetActive(false);
                        HurdleObject hurdleObject = (HurdleObject)GameManager.Instance.FindCreateActiveObject(hurdleObjecs);
                        if (hurdleObject != null)
                        {
                            // 長さを決める
                            hurdleObject.SetLength(Random.Range(hurdleMinLength, hurdleMaxLength));
                            // 警告を出して止める
                            GameManager.Instance.StartObject(hurdleObject, i, Random.Range(hurdleMinSpeed, hurdleMaxSpeed));     // Hack: 速度算出を治す
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 生成内容判断
        /// </summary>
        public void Thinking()
        {
            if (nextThinkingTime > 0)
            {
                nextThinkingTime -= Time.deltaTime;
                return;
            }

            int currentObjectNum = GameManager.Instance.HumanCurrentNum + GameManager.Instance.ProteinCurrentNum + GameManager.Instance.HurdleCurrentNum;
            int settingObjectNum = Random.Range(1, (currentObjectNum % onceSetObjectMax) + 1);
            float speed = 1.5f;
            CheckOutObject();       // 事前に外にあるオブジェクトを洗い出す

            // 一度に設定する個数分設定する
            for(int i = 0; i < settingObjectNum; i++)
            {
                // 配置する列を決める
                bool[] lineFlag = new bool[line];
                // 除外：配置が予約されているラインか（ハードル）
                CheckWarningLine(ref lineFlag);
                // 除外：配置されたオブジェクトが配置位置にまだいないか（オブジェクトの上の座標が画面外なら配置しない）
                CheckObjectOutTop(ref lineFlag);
                // 判断：配置するオブジェクトをどれにするか（優先度を設けて判断する）
                GameManager.OBJECT_TYPE objectType = ThinkingObject();
                Debug.Log($"[Create] ObjectThinking:{objectType}");
                if(objectType == GameManager.OBJECT_TYPE.Num)
                {
                    continue;       // 何らかの不具合で生成できなかった
                }
                int createCol = ThinkingCol(lineFlag, objectType, ref speed);          // 生成する列
                Debug.Log($"[Create] ColumThinking:{createCol}");

                switch (objectType)
                {
                    case GameManager.OBJECT_TYPE.Human:
                        if (GameManager.Instance.HumanCurrentNum < humanMaxNum)
                        {
                            Debug.Log($"[Create] Human:{GameManager.Instance.HumanCurrentNum}");
                            GameManager.Instance.StartObject(GameManager.Instance.FindCreateActiveObject(humanObjecs), createCol, speed);
                            GameManager.Instance.HitObjectCount(GameManager.OBJECT_TYPE.Human);
                        }
                        break;
                    case GameManager.OBJECT_TYPE.Protein:
                        if (GameManager.Instance.ProteinCurrentNum < proteinMaxNum)
                        {
                            Debug.Log($"[Create] Protein:{GameManager.Instance.ProteinCurrentNum}");
                            GameManager.Instance.StartObject(GameManager.Instance.FindCreateActiveObject(proteinObjecs), createCol, speed);
                            GameManager.Instance.HitObjectCount(GameManager.OBJECT_TYPE.Protein);
                        }
                        break;
                    case GameManager.OBJECT_TYPE.Hurdle:
                        if (GameManager.Instance.HurdleCurrentNum < hurdleMaxNum)
                        {
                            Debug.Log($"[Create] Hurdle:{GameManager.Instance.HurdleCurrentNum}");
                            hurdleWarningLine[createCol] = true;
                            warningObjects[createCol].SetActive(true);
                            GameManager.Instance.HitObjectCount(GameManager.OBJECT_TYPE.Hurdle);
                        }
                        break;
                    default:
                        // 生成しない
                        nextThinkingTime = addThinkingTime;        // 指定秒後に生成判断 
                        break;
                }
            }

            nextThinkingTime = Random.Range(thinkingMinTime, thinkingMaxTime);      // 指定範囲秒を次の生成判断の時間にする
        }

        #region 生成オブジェクトの判断
        /// <summary>
        /// どのオブジェクトを生成するか
        /// </summary>
        /// <returns></returns>
        private GameManager.OBJECT_TYPE ThinkingObject()
        {
            // 具体：人は条件問わず配置する。覚醒状態であれば高確率で配置する
            // 具体：覚醒状態でなければプロテインを配置する
            // 具体：覚醒状態であれば指定の確率でプロテインを配置する
            // 具体：開始時間から一定時間経過していて、ハードルが設置されていなければハードルを配置する
            // 具体：最終的に非覚醒状態の優先度は次（プロテイン＞ハードル＞人）
            // 具体：覚醒状態の優先度（人＞ハードル＞プロテイン）

            int humanNum = humanMaxNum - GameManager.Instance.HumanCurrentNum;              // 生成できる人の数
            int proteinNum = proteinMaxNum - GameManager.Instance.ProteinCurrentNum;        // 生成できるプロテインの数
            int hurdleNum = hurdleMaxNum - GameManager.Instance.HurdleCurrentNum;           // 生成できるハードルの数

            int rand = Random.Range(0, 100);
            float humanProvisionalRatio = 0;
            float proteinProvisionalRatio = 0;
            float hurdleProvisionalRatio = 0;
            if (GameManager.Instance.Player.IsAwaiking)
            {
                humanProvisionalRatio = awakingHumanCreatePer;
                proteinProvisionalRatio = awakingProteinCreatePer;
                hurdleProvisionalRatio = awakingHurdleCreatePer;
            }
            else
            {
                humanProvisionalRatio = humanCreatePer;
                proteinProvisionalRatio = proteinCreatePer;
                hurdleProvisionalRatio = hurdleCreatePer;
            }

            // 生成できる数がなければ割合を別のものに振る
            if (humanNum == 0)
            {
                proteinProvisionalRatio += humanProvisionalRatio / 2;
                hurdleProvisionalRatio += humanProvisionalRatio / 2;
            }
            if (proteinNum == 0)
            {
                if(humanNum > 0)
                {
                    // 人に割り振れるなら二分する
                    humanProvisionalRatio += proteinProvisionalRatio / 2;
                    hurdleProvisionalRatio += proteinProvisionalRatio / 2;
                }
                else
                {
                    // 人に割り振れないならハードルにすべて渡す
                    hurdleProvisionalRatio += proteinProvisionalRatio;
                }
            }
            if (hurdleNum == 0)
            {
                if(humanNum > 0 && proteinNum > 0)
                {
                    // 人にもプロテインにも割り当てられるなら二分する
                    humanProvisionalRatio += hurdleProvisionalRatio / 2;
                    proteinProvisionalRatio += hurdleProvisionalRatio / 2;
                }
                else if(humanNum > 0)
                {
                    // 人にだけ割り振れるなら人にすべて渡す
                    humanProvisionalRatio += hurdleProvisionalRatio;
                }
                else if(proteinNum > 0)
                {
                    // プロテイン二だけ割り振れるならプロテインに渡す
                    proteinProvisionalRatio += hurdleProvisionalRatio;
                }
                else
                {
                    Debug.LogWarning($"[Thinkig] どれも生成可能オブジェクトじゃないのでアルゴリズムがおかしい");
                }
            }

            if(rand <= humanProvisionalRatio)
            {
                return GameManager.OBJECT_TYPE.Human;
            }
            else if(rand <= humanProvisionalRatio + proteinProvisionalRatio)
            {
                return GameManager.OBJECT_TYPE.Protein;
            }
            else if(rand <= humanProvisionalRatio + proteinProvisionalRatio + hurdleProvisionalRatio)
            {
                return GameManager.OBJECT_TYPE.Hurdle;
            }
            return GameManager.OBJECT_TYPE.Num;         // 生成失敗
        }
        #endregion
        #region 生成列の判断
        /// <summary>
        /// 生成する列を判断する
        /// </summary>
        /// <param name="_lineFlag"></param>
        /// <param name="_objectType"></param>
        /// <param name="_speed"></param>
        /// <returns></returns>
        private int ThinkingCol(bool[] _lineFlag, GameManager.OBJECT_TYPE _objectType, ref float _speed)
        {
            switch (_objectType)
            {
                case GameManager.OBJECT_TYPE.Human:
                    return ThinkingColPlayer(_lineFlag, ref _speed);
                case GameManager.OBJECT_TYPE.Protein:
                    return ThinkingColProtein(_lineFlag, ref _speed);
                case GameManager.OBJECT_TYPE.Hurdle:
                    return ThinkingColHurdle(_lineFlag, ref _speed);
                    
            }
            return -1;      // -1は生成できる列がなかった時に出す
        }

        /// <summary>
        /// プレイヤーの生成列の判断
        /// </summary>
        /// <param name="_lineFlag"></param>
        /// <param name="_speed"></param>
        /// <returns></returns>
        private int ThinkingColPlayer(bool[] _lineFlag, ref float _speed)
        {
            // 優先：人であればプレイヤーのいる列に優先、左右を第二候補、最も離れた位置を最終候補にする
            int colum = GameManager.Instance.Player.Colum;
            Player player = GameManager.Instance.Player;
            int far = 0;
            int rand = Random.Range(0, 100);
            int candidate = -1;
            List<int> candidateColum = new List<int>();
            List<int> ignoreList = new List<int>();
            List<HumanObject> humanObjects = new List<HumanObject>();
            Dictionary<int, float> speedList = new Dictionary<int, float>();
            List<Probability> probabilities = new List<Probability>();
            int num = 0;

            // 最も離れた位置を探す
            if (colum < line / 2)
            {
                // 左側なので最も離れた位置はline - 1になる
                far = line - 1;
            }
            else
            {
                // 右側なので最も離れた位置は0になる
                far = 0;
            }

            // 候補地リスト
            for (int i = 0; i < _lineFlag.Length; i++)
            {
                if (_lineFlag[i])
                {
                    candidateColum.Add(i);
                }
            }

            // 覚醒状態じゃない
            if (!GameManager.Instance.Player.IsAwaiking) 
            {
                // 各列で候補地を見つける、見つからなければ生成不良として失敗する
                for (int i = 0; i < candidateColum.Count; i++)
                {
                    // 除外：人であれば覚醒状態ではない状態かつ生成済みオブジェクトと隣り合う位置を候補とした時、下記具体条件でプレイヤーが詰まないようにする
                    // 具体：人Aがプレイヤーを通過する時、人Bの位置はプレイヤーの上座標から1人分の空間を開ける
                    // これでも詰むケースはあり得るのでその場合は不幸ということで……

                    // 候補列の左右に人がいるか要ればその人を記録
                    humanObjects.Clear();           // Hack: リファクタ案件
                    foreach (HumanObject human in GameManager.Instance.HumanObjects)
                    {
                        if (candidateColum[i] - 1 == human.Col || candidateColum[i] + 1 == human.Col)
                        {
                            if (human.gameObject.activeSelf)
                            {
                                humanObjects.Add(human);
                            }
                        }
                    }

                    // 対象の人が通過する時間に対して充分距離をとれるのか
                    foreach (HumanObject human in humanObjects)
                    {
                        // 具体：人Aがプレイヤーの下座標を通過するまでの時間を求める（距離/速度）
                        float distanceY = Mathf.Sqrt(Mathf.Pow(human.transform.position.y - (player.transform.position.y - player.transform.localScale.y / 2), 2));     // プレイヤーとの高さに対する距離を測る
                        float transitTime = distanceY / human.Speed;        // プレイヤーを過ぎ去る時間を求める

                        // 具体：この時間をもとに人Bがその時間にプレイヤーの上座標から一人分の空間を開けた位置にたどり着くのに必要な速度を求める（距離/時間）
                        distanceY = Mathf.Sqrt(Mathf.Pow(MapManager.Instance.GetOverDisplayTop(human.transform.localScale.y)
                            - (player.transform.position.y + player.transform.localScale.y / 2 + human.transform.localScale.y * 1.5f), 2));
                        float speed = distanceY / transitTime;
                        // 具体：この時の速度が一定の条件を満たさない場合、今の候補を除外して違う列の候補を出す
                        if (speed < humanMinSpeed)
                        {
                            // 最低速度を下回っているのでこのオブジェクトでは今生成するとどうやっても詰んでしまう可能性があるので除外
                            ignoreList.Add(candidateColum[i]);
                        }
                        else
                        {
                            // この列に対しては詰まない想定なので生成して問題ない
                            speed = Mathf.Clamp(speed, humanMinSpeed, humanMaxSpeed);
                            if (speedList.ContainsKey(candidateColum[i]) && speedList[candidateColum[i]] < speed)
                            {
                                speedList[candidateColum[i]] = speed;
                            }
                        }
                    }
                }
            }

            // 生成可能位置から除外するものを除外する
            foreach (int i in ignoreList)
            {
                candidateColum.Remove(i);
            }

            // 上がっている候補地から決める
            if (candidateColum.Contains(colum))
            {
                // プレイヤーの位置
                candidate = colum;
                num += 50;
                probabilities.Add(new Probability(num, candidate));
                Debug.Log($"[Human]0:{num}");
            }
            if(candidateColum.Contains(colum + 1))
            {
                // プレイヤーの右
                candidate = colum + 1;
                num += 20;
                probabilities.Add(new Probability(num, candidate));
                Debug.Log($"[Human]0:{num}");
            }
            if(candidateColum.Contains(colum - 1))
            {
                // プレイヤーの左
                candidate = colum - 1;
                num += 20;
                probabilities.Add(new Probability(num, candidate));
                Debug.Log($"[Human]0:{num}");
            }
            if(candidateColum.Contains(far))
            {
                // プレイヤーから一番遠い位置
                candidate = far;
                num += 10;
                probabilities.Add(new Probability(num, candidate));
                Debug.Log($"[Human]0:{num}");
            }

            // 速度決める
            if (speedList.ContainsKey(candidate))
            {
                // 速度調整が必要
                _speed = speedList[candidate];
                Debug.Log($"[Create] SetSpeedDic:{_speed}");
            }
            else
            {
                // 速度調整が不要
                _speed = Random.Range(humanMinSpeed, humanMaxSpeed);
                Debug.Log($"[Create] SetSpeedDef:{_speed}");
            }

            return RandomColumResult(probabilities, num, candidate, "Human");
        }

        /// <summary>
        /// プロテインを出す位置を考える
        /// </summary>
        /// <param name="_lineFlag"></param>
        /// <param name="_speed"></param>
        /// <returns></returns>
        private int ThinkingColProtein(bool[] _lineFlag, ref float _speed)
        {
            int colum = GameManager.Instance.Player.Colum;
            int far = 0;
            int candidate = -1;
            int num = 0;
            List<Probability> probabilities = new List<Probability>();

            _speed = Random.Range(proteinMinSpeed, proteinMaxSpeed);

            // 最も離れた位置を探す
            if (colum < line / 2)
            {
                // 左側なので最も離れた位置はline - 1になる
                far = line - 1;
            }
            else
            {
                // 右側なので最も離れた位置は0になる
                far = 0;
            }

            // 優先：プロテインであればプレイヤーのいる列から最も離れた位置を最優先とする。離れた位置が遠いほど優先になる
            // Hack: リファクタできる
            if (_lineFlag[far])
            {
                // 最も離れた位置
                candidate = far;
                num += 40;
                probabilities.Add(new Probability(num, candidate));
                Debug.Log($"[Protein]0:{num}");
            }
            if(far < line - 1 && _lineFlag[far + 1])
            {
                // 離れた位置から右1つ隣
                candidate = far + 1;
                num += 30;
                probabilities.Add(new Probability(num, candidate));
                Debug.Log($"[Protein]1:{num}");
            }
            if(far > 0 && _lineFlag[far - 1])
            {
                // 離れた位置から左1つ隣
                candidate = far - 1;
                num += 30;
                probabilities.Add(new Probability(num, candidate));
                Debug.Log($"[Protein]1:{num}");
            }
            if(colum < line - 1 && _lineFlag[colum + 1])
            {
                // プレイヤーの右隣
                candidate = colum + 1;
                num += 10;
                probabilities.Add(new Probability(num, candidate));
                Debug.Log($"[Protein]2:{num}");
            }
            if(colum > 0 && _lineFlag[colum - 1])
            {
                // プレイヤーの左隣
                candidate = colum - 1;
                num += 10;
                probabilities.Add(new Probability(num, candidate));
                Debug.Log($"[Protein]2:{num}");
            }
            if(_lineFlag[colum])
            {
                // プレイヤーの位置
                candidate = colum;
                num += 10;
                probabilities.Add(new Probability(num, candidate));
                Debug.Log($"[Protein]3:{num}");
            }

            return RandomColumResult(probabilities, num, candidate, "Protein");
        }

        private int ThinkingColHurdle(bool[] _lineFlag, ref float _speed)
        {
            // 優先：ハードルであればプレイヤーのいる列の隣のうち二分しないものを優先とし、次に最も離れた位置を第二候補とする（詰みやすいと面白くない）
            // 優先：ハードルであればプレイヤーのいる列を第三候補とし、プレイヤーのいる列から二つ隣の列を第四候補とし
            // 優先：ハードルであればプレイヤーのいる列の隣のうち二分するものを最終候補とする
            List<Probability> probabilities = new List<Probability>();
            int colum = GameManager.Instance.Player.Colum;
            int far = 0;
            int candidate = -1;
            int num = 0;

            _speed = Random.Range(hurdleMinSpeed, hurdleMaxSpeed);

            // 最も離れた位置を探す
            if (colum < line / 2)
            {
                // 左側なので最も離れた位置はline - 1になる
                far = line - 1;
            }
            else
            {
                // 右側なので最も離れた位置は0になる
                far = 0;
            }

            Debug.Log($"[Hurdle]colum:{colum} far:{far}");
            for(int i = 0; i < _lineFlag.Length; i++)
            {
                Debug.Log($"[Hurdle]line:{i}={_lineFlag[i]}");
            }

            if (colum - 1 == 0 && _lineFlag[colum - 1])
            {
                // プレイヤーの左隣が一番左
                candidate = colum - 1;
                num += 50;
                probabilities.Add(new Probability(num, candidate));
                Debug.Log($"[Hurdle]0:{num}");
            }
            if (colum + 1 == line - 1 && _lineFlag[colum + 1])
            {
                // プレイヤーの右隣が一番右
                candidate = colum + 1;
                num += 50;
                probabilities.Add(new Probability(num, candidate));
                Debug.Log($"[Hurdle]0:{num}");
            }
            if (_lineFlag[far])
            {
                // 一番離れた位置
                candidate = far;
                num += 30;
                probabilities.Add(new Probability(num, candidate));
                Debug.Log($"[Hurdle]1:{num}");
            }
            if (_lineFlag[colum])
            {
                // プレイヤーの位置
                candidate = colum;
                num += 10;
                probabilities.Add(new Probability(num, candidate));
                Debug.Log($"[Hurdle]2:{num}");
            }
            if (colum - 1 >= 0 && _lineFlag[colum - 1])
            {
                // 左隣が有効
                candidate = colum - 1;
                num += 5;
                probabilities.Add(new Probability(num, candidate));
                Debug.Log($"[Hurdle]3:{num}");
            }
            if (colum + 1 < line - 1 && _lineFlag[colum + 1])
            {
                // 右隣が有効
                candidate = colum + 1;
                num += 5;
                probabilities.Add(new Probability(num, candidate));
                Debug.Log($"[Hurdle]4:{num}");
            }

            return RandomColumResult(probabilities, num, candidate, "Hurdle");
        }

        /// <summary>
        /// 指定した確率で列を取得する
        /// </summary>
        /// <param name="_probabilities"></param>
        /// <param name="_num"></param>
        /// <param name="_defaultColum"></param>
        /// <returns></returns>
        private int RandomColumResult(List<Probability> _probabilities, int _num, int _defaultColum = -1, string _type = "")
        {
            foreach (Probability probability in _probabilities)
            {
                Debug.Log($"[Thinking][Colum][{_type}]Range:{probability.Range} Colum:{probability.Colum}");
            }

            int index = Random.Range(0, _num);
            foreach (Probability probability in _probabilities)
            {
                if (index < probability.Range)
                {
                    Debug.Log($"[Thinking][Colum][{_type}]Range:{probability.Range} Index:{index} Colum:{probability.Colum}");
                    return probability.Colum;
                }
            }
            return _defaultColum;
        }
        #endregion
        #region 生成可否チェック
        /// <summary>
        /// 
        /// </summary>
        /// <param name="_line"></param>
        private void CheckWarningLine(ref bool[] _line)
        {
            for (int i = 0; i < hurdleWarningLine.Length; i++)
            {
                _line[i] = !hurdleWarningLine[i];           // フラグが立っていなければ生成可能位置
                Debug.Log($"[Thinking]line:{i}={_line[i]}");
            }
        }

        private void CheckObjectOutTop(ref bool[] _line)
        {
            for(int i = 0; i < _line.Length; i++)
            {
                if (_line[i])
                {
                    // 画面外に残っているオブジェクトがあるか
                    if (outObjects.ContainsKey(i))
                    {
                        // Keyがあれば生成してはいけない列
                        _line[i] = false;
                    }
                }
                Debug.Log($"[Thinking]line:{i}={_line[i]}");
            }
        }

        private void CheckOutObject()
        {
            outObjects.Clear();
            Check(GameManager.Instance.HumanObjects);
            Check(GameManager.Instance.ProteinObjects);
            Check(GameManager.Instance.HurdleObjects);

            void Check<T>(List<T> _hitObjects) where T : HitObject
            {
                // 画面に入っていないオブジェクトがないか
                foreach (HitObject hitObject in _hitObjects)
                {
                    if (!hitObject.IsInDisplay && hitObject.gameObject.activeSelf)
                    {
                        // 画面に入っていない
                        outObjects[hitObject.Col] = hitObject;
                        Debug.Log($"[Thinking][Check]line:{hitObject.Col}");
                    }
                }
            }
        }
#endregion
    }
}