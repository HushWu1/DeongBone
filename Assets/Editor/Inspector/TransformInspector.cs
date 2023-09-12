
using UnityEditor;
using UnityEngine;


    [CustomEditor(typeof(Transform))]
    class TransformInspector : Editor
    {
        public const string KEY_CONSTRAINT_ROTATION = "TRANSFORM_CONSTRAINT_ROTATION";

        private Transform transform;
        private bool constraintRotation;

        private void OnEnable()
        {
            transform = target as Transform;
            constraintRotation = EditorPrefs.GetBool(KEY_CONSTRAINT_ROTATION, true);
        }

        private void OnDisable()
        {
            EditorPrefs.SetBool(KEY_CONSTRAINT_ROTATION, constraintRotation);
        }

        public override void OnInspectorGUI()
        {
            bool reset;

            //Position
            EditorGUILayout.BeginHorizontal();
            {
                Vector3 newPos;
                EditorGUI.BeginChangeCheck();
                {
                    reset = GUILayout.Button("P", GUILayout.MaxWidth(20));
                    newPos = EditorGUILayout.Vector3Field(GUIContent.none, transform.localPosition);

                    //reset
                    if (reset)
                    {
                        newPos = Vector3.zero;
                    }
                    else
                    {
                        //限定两位小数
                        newPos.x = Utility.Math.Round(newPos.x, 2);
                        newPos.y = Utility.Math.Round(newPos.y, 2);
                        newPos.z = Utility.Math.Round(newPos.z, 2);
                    }
                }
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(transform, "Position Change");
                    transform.localPosition = newPos;
                    EditorUtility.SetDirty(target);
                }
            }
            EditorGUILayout.EndHorizontal();

            //Rotation
            EditorGUILayout.BeginHorizontal();
            {
                Vector3 newLocalEulerAngles;
                EditorGUI.BeginChangeCheck();
                {
                    reset = GUILayout.Button("R", GUILayout.MaxWidth(20));
                    newLocalEulerAngles = EditorGUILayout.Vector3Field("", transform.localEulerAngles);

                    //reset
                    if (reset)
                    {
                        newLocalEulerAngles = Vector3.zero;
                    }
                    else
                    {
                        if (constraintRotation)
                        {
                            newLocalEulerAngles.x = Utility.Math.NormalizeAngle180(newLocalEulerAngles.x);
                            newLocalEulerAngles.y = Utility.Math.NormalizeAngle180(newLocalEulerAngles.y);
                            newLocalEulerAngles.z = Utility.Math.NormalizeAngle180(newLocalEulerAngles.z);
                        }
                        newLocalEulerAngles.x = Utility.Math.Round(newLocalEulerAngles.x, 2);
                        newLocalEulerAngles.y = Utility.Math.Round(newLocalEulerAngles.y, 2);
                        newLocalEulerAngles.z = Utility.Math.Round(newLocalEulerAngles.z, 2);
                    }
                }
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(transform, "Rotation Change");
                    transform.localEulerAngles = newLocalEulerAngles;
                    EditorUtility.SetDirty(target);
                }
            }
            EditorGUILayout.EndHorizontal();

            //Scale
            EditorGUILayout.BeginHorizontal();
            {
                Vector3 newLocalScale;
                EditorGUI.BeginChangeCheck();
                {
                    reset = GUILayout.Button("S", GUILayout.MaxWidth(20));
                    newLocalScale = EditorGUILayout.Vector3Field("", transform.localScale);

                    if (reset)
                    {
                        newLocalScale = Vector3.one;
                    }
                    else
                    {
                        newLocalScale.x = Utility.Math.Round(newLocalScale.x, 2);
                        newLocalScale.y = Utility.Math.Round(newLocalScale.y, 2);
                        newLocalScale.z = Utility.Math.Round(newLocalScale.z, 2);
                    }
                }
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(transform, "Scale Change");
                    transform.localScale = newLocalScale;
                    EditorUtility.SetDirty(target);
                }
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();
            constraintRotation = EditorGUILayout.ToggleLeft(new GUIContent("ConstraintRotation", "将旋转约束在(-180,180)范围内.若进行约束可能会影响动画制作"), constraintRotation);
        }
    }

