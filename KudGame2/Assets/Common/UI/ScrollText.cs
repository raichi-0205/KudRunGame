using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

namespace Kud.Common.UI
{
    public class ScrollText : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] TextMeshProUGUI text;      // 流したいテキスト
        [SerializeField] bool updateFlag = false;   // カーソルが当たっているかどうか
        [SerializeField] float speed = 1;           // スクロール速度
        [SerializeField] float wait = 100;          // 画面外に出てからの余白分
        Vector3 initPos;        // 初期位置

        private void Start()
        {
            Init();
        }

        private void Update()
        {
            UpdatePos();
        }

        /// <summary>
        /// 初期化処理
        /// </summary>
        public void Init()
        {
            initPos = text.rectTransform.localPosition;
        }

        /// <summary>
        /// 座標更新
        /// </summary>
        private void UpdatePos()
        {
            if (!updateFlag) return;
            
            if (-(text.rectTransform.sizeDelta.x + speed + wait) > text.rectTransform.localPosition.x)
            {
                ResetPos();
            }
            else
            {
                text.rectTransform.localPosition += new Vector3(-1 * speed * Time.deltaTime, 0, 0);
            }
        }

        /// <summary>
        /// 初期位置に戻す
        /// </summary>
        public void ResetPos()
        {
            text.rectTransform.localPosition = initPos;
        }

        /// <summary>
        /// カーソルが当たった時に呼ぶ
        /// </summary>
        /// <param name="_pointerEventData"></param>
        public void OnPointerEnter(PointerEventData _pointerEventData)
        {
            updateFlag = true;
        }

        /// <summary>
        /// カーソルが外れた時に呼ぶ
        /// </summary>
        /// <param name="_pointerEventData"></param>
        public void OnPointerExit(PointerEventData _pointerEventData)
        {
            updateFlag = false;
            ResetPos();
        }
    }
}