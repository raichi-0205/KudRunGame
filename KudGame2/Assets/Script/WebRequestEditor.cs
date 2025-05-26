using UnityEngine;
using UnityEditor;

namespace Kud.Network
{
    public class WebRequestEditor : MonoBehaviour
    {
        public enum API
        {
            UpdateScore,
            GetScore,
        }
        [SerializeField] API api;
        public API Api { get { return api; } }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(WebRequestEditor))]
    public class WebRequestEditorEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            WebRequestEditor origin = target as WebRequestEditor;

            if (GUILayout.Button("送信"))
            {
                switch (origin.Api)
                {
                    case WebRequestEditor.API.UpdateScore:
                        {
                            UpdateScore api = new UpdateScore();
                            api.SendStart(0);
                            break;
                        }
                    case WebRequestEditor.API.GetScore:
                        {
                            GetScore api = new GetScore();
                            api.SendStart(0);
                            break;
                        }
                }
            }
        }
    }
#endif
}