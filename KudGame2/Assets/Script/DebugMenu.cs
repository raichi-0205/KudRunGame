using UnityEngine;
using UnityEditor;

namespace Kud.Editor
{
#if UNITY_EDITOR
    public class DebugMenu : MonoBehaviour
    {
        private void OnGUI()
        {
            if(GUI.Button(new Rect(10,10,100,50),"HiScore Delete"))
            {
                MainGame.ScoreManager.Instance.HiScoreDelete();
            }
        }
    }

    [CustomEditor(typeof(DebugMenu))]
    public class DebugMenuEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            if(GUILayout.Button("HiScore Delete"))
            {
                MainGame.ScoreManager.Instance.HiScoreDelete();
            }
        }
    }
#endif
}