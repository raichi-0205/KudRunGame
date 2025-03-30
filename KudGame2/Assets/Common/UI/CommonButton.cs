using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEditor;

namespace Star.Common.UI
{
    public class CommonButton : Button
    {
#if UNITY_EDITOR
        [MenuItem("GameObject/Common/UI/CommonButton", priority = 2)]
        public static void Commonbutton()
        {
            var obj = new GameObject();
            obj.transform.SetParent(Selection.activeGameObject.transform, false);
            var image = obj.AddComponent<Image>();
            var button = obj.AddComponent<CommonButton>();
            button.image = image;
        }
#endif
    }
}