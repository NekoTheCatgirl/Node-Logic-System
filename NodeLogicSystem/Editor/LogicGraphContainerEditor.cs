using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEditor;
using UnityEditor.Events;

namespace NodeLogicSystem
{
    [CustomEditor(typeof(LogicGraphContainer))]
    public class LogicGraphContainerEditor : Editor
    {
        SerializedProperty graphProperty;
        SerializedProperty graphEventArray;

        private void OnEnable()
        {
            graphProperty = serializedObject.FindProperty("graph");
            graphEventArray = serializedObject.FindProperty("graphEvents");
        }

        public override void OnInspectorGUI()
        {
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(graphProperty);
            if (EditorGUI.EndChangeCheck())
                OnGraphChange();

            EditorGUI.BeginChangeCheck();
            if (graphEventArray.isArray)
            {
                if (graphEventArray.arraySize > 0)
                {
                    m_outputEventsUnfolded = EditorGUILayout.Foldout(m_outputEventsUnfolded, "Output events", true);
                    if (m_outputEventsUnfolded)
                    {
                        using (new EditorGUI.IndentLevelScope())
                        {
                            for (int i = 0; i < graphEventArray.arraySize; i++)
                            {
                                string name = graphEventArray.GetArrayElementAtIndex(i).FindPropertyRelative("Name").stringValue;
                                EditorGUILayout.PropertyField(graphEventArray.GetArrayElementAtIndex(i), new GUIContent(name));
                            }
                        }
                    }
                }
                else
                {
                    EditorGUILayout.HelpBox("No output nodes found! Graph is invalid!", MessageType.Error, true);
                }
            }

            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
                serializedObject.Update();
                Repaint();
            }
        }

        private void OnGraphChange()
        {
            serializedObject.ApplyModifiedProperties();
            (target as LogicGraphContainer).SetupContainer();
            serializedObject.Update();
            Repaint();
        }

        [SerializeField] private bool m_outputEventsUnfolded = true;
    }
}
