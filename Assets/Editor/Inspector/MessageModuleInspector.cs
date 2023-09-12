
using UnityEditor;
using UnityEngine;



    [CustomEditor(typeof(EventModule))]
    public class MessageModuleInspector : BaseInspector
    {
        protected override void OnInspectorUpdateInEditor()
        {
            base.OnInspectorUpdateInEditor();
            Repaint();
        }

        private string searchText;

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (Application.isPlaying)
            {
                searchText = EditorGUILayout.TextField("Search Message", searchText);
                int showCount = 0;
                foreach (var handlerPair in TGameFrameWork.Instance.GetModule<EventModule>().Handlers)
                {
                    if (!string.IsNullOrEmpty(searchText) && !handlerPair.Key.FullName.ToLower().Contains(searchText.ToLower()))
                        continue;

                    showCount++;
                    EditorGUILayout.BeginVertical(GUI.skin.box);
                    {
                        EditorGUILayout.LabelField("Message", $"{handlerPair.Key}({handlerPair.Value.Count})");
                        EditorGUI.indentLevel++;
                        {
                            foreach (var handler in handlerPair.Value)
                            {
                                EditorGUILayout.BeginVertical(GUI.skin.box);
                                {
                                    EditorGUILayout.LabelField($"{handler.Target.GetType().FullName }.{handler.Method.Name}");
                                }
                                EditorGUILayout.EndVertical();
                            }
                        }
                        EditorGUI.indentLevel--;
                    }
                    EditorGUILayout.EndVertical();
                    EditorGUILayout.Space();
                }

                if (!string.IsNullOrEmpty(searchText) && showCount == 0)
                {
                    //没有搜索到任何内容
                    EditorGUILayout.HelpBox("Can't match any message", UnityEditor.MessageType.Info);
                }
            }
        }
    }
