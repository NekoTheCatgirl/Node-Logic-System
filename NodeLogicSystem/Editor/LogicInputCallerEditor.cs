using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace NodeLogicSystem
{
    [CustomEditor(typeof(LogicInputCaller))]
    public class LogicInputCallerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.DrawDefaultInspector();
            var item = target as LogicInputCaller;

            if (item.InputNodes.Length > 0)
            {
                if (item.InputNodes.Contains(item.InputName) is false) item.InputName = item.InputNodes.First();
                int selectedIndex = item.InputNodes.ToList().IndexOf(item.InputName);

                EditorGUI.BeginChangeCheck();
                selectedIndex = EditorGUILayout.Popup(selectedIndex, item.InputNodes);
                item.InputName = item.InputNodes[selectedIndex];
                EditorGUI.EndChangeCheck();
            }
        }
    }
}
