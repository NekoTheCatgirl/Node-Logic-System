using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor;
using System;
using UnityEditor.UIElements;
using UnityEditor.Experimental.GraphView;

namespace NodeLogicSystem
{
    public class LogicNodeEditor : EditorWindow
    {
        private LogicNodeGraphView _graphView;

        [MenuItem("Window/Logic editor")]
        public static void Init()
        {
            var window = GetWindow<LogicNodeEditor>();
            window.titleContent = new GUIContent("Node editor");
        }

        private void OnEnable()
        {
            ConstructGraphView();
            GenerateToolbar();
            GenerateMiniMap();
        }

        private void GenerateMiniMap()
        {
            var miniMap = new MiniMap { anchored = true };
            miniMap.SetPosition(new Rect(10, 30, 200, 140));
            _graphView.Add(miniMap);
        }

        private void ConstructGraphView()
        {
            _graphView = new LogicNodeGraphView()
            {
                name = "Graph"
            };

            _graphView.StretchToParentSize();

            rootVisualElement.Add(_graphView);
        }

        private string FileName = "LogicGraph";

        private void GenerateToolbar()
        {
            var toolbar = new Toolbar();

            var fieldLabel = new TextElement();
            fieldLabel.text = "File name: ";
            toolbar.Add(fieldLabel);

            var graphNameField = new TextField();
            graphNameField.value = FileName;
            graphNameField.MarkDirtyRepaint();
            graphNameField.RegisterValueChangedCallback(val => { FileName = val.newValue; });
            toolbar.Add(graphNameField);

            var graphCompileButton = new ToolbarButton(() => { _graphView.CompileGraph(FileName); });
            graphCompileButton.text = "Compile";
            graphCompileButton.tooltip = "Compiles the graph for use ingame";
            toolbar.Add(graphCompileButton);

            toolbar.Add(new ToolbarSpacer());

            var graphSaveButton = new ToolbarButton(() => { _graphView.SaveGraph(); });
            graphSaveButton.text = "Save";
            graphSaveButton.tooltip = "Saves the node graph for later editing";
            toolbar.Add(graphSaveButton);

            var graphLoadButton = new ToolbarButton(() => { _graphView.LoadGraph(); });
            graphLoadButton.text = "Load";
            graphLoadButton.tooltip = "Loads a previously saved node graph";
            toolbar.Add(graphLoadButton);

            toolbar.Add(new ToolbarSpacer() { flex = true }); ;

            var infoText = new TextElement();
            infoText.text = "Use RMB to create nodes. MMB to navigate canvas, and  LMB to manipulate nodes";
            toolbar.Add(infoText);

            rootVisualElement.Add(toolbar);
        }

        private void OnDisable()
        {
            rootVisualElement.Remove(_graphView);
        }

    }
}