using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using Newtonsoft.Json;

namespace NodeLogicSystem
{
    public class LogicNodeGraphView : GraphView
    {
        private readonly Vector2 defaultNodeSize = new Vector2(150, 200);

        public LogicNodeGraphView()
        {
            styleSheets.Add(Resources.Load<StyleSheet>("Graph"));

            SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);

            this.AddManipulator(new ContentZoomer());
            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());

            var gridBack = new GridBackground();
            Insert(0, gridBack);
            gridBack.StretchToParentSize();
        }

        public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
        {
            foreach (var type in Enum.GetNames(typeof(LogicType)))
            {
                evt.menu.AppendAction($"Create {type}", (action) => { CreateNode((LogicType)Enum.Parse(typeof(LogicType), type), type, contentViewContainer.WorldToLocal(action.eventInfo.mousePosition)); });
            }
            evt.menu.AppendSeparator();
            evt.menu.AppendAction("Clear graph", (action) => { ClearGraph(); });
            evt.menu.AppendSeparator();
            base.BuildContextualMenu(evt);
        }

        public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
        {
            var compatiblePorts = new List<Port>();
            ports.ForEach(port =>
            {
                if (startPort != port && startPort.node != port.node && startPort.direction != port.direction)
                    compatiblePorts.Add(port);
            });

            return compatiblePorts;
        }

        private void SerializeGraph(string filename)
        {
            Dictionary<string, LogicComponent> components = new Dictionary<string, LogicComponent>();
            Dictionary<string, string> nameToGuid = new Dictionary<string, string>();

            if (!edges.ToList().Any()) return;

            var connectedPorts = edges.ToList().Where(x => x.input.node != null).ToArray();
            foreach (var edge in connectedPorts)
            {
                var inputNode = edge.input.node as LogicNode;
                var outputNode = edge.output.node as LogicNode;

                if (inputNode.NodeType == LogicType.OUTPUT)
                {
                    if (nameToGuid.ContainsKey(inputNode.NodeName) is false)
                    {
                        nameToGuid.Add(inputNode.NodeName, inputNode.GUID);
                    }
                }

                if (components.ContainsKey(inputNode.GUID))
                {
                    if (components[inputNode.GUID].InputPort1.Length > 0)
                    {
                        components[inputNode.GUID].InputPort2 = outputNode.GUID;
                    }
                    else
                    {
                        components[inputNode.GUID].InputPort1 = outputNode.GUID;
                    }
                }
                else
                {
                    components.Add(inputNode.GUID, new LogicComponent() { type = inputNode.NodeType, GUID = inputNode.GUID, InputPort1 = outputNode.GUID, State = inputNode.State });
                }

                if (outputNode.NodeType == LogicType.INPUT)
                {
                    if (nameToGuid.ContainsKey(outputNode.NodeName) is false)
                    {
                        nameToGuid.Add(outputNode.NodeName, outputNode.GUID);
                    }
                }

                if (components.ContainsKey(outputNode.GUID))
                {
                    if (components[outputNode.GUID].OutputPort1.Length > 0)
                    {
                        components[outputNode.GUID].OutputPort2 = inputNode.GUID;
                    }
                    else
                    {
                        components[outputNode.GUID].OutputPort1 = inputNode.GUID;
                    }
                }
                else
                {
                    components.Add(outputNode.GUID, new LogicComponent() { type = outputNode.NodeType, GUID = outputNode.GUID, OutputPort1 = inputNode.GUID, State = outputNode.State });
                }
            }

            if (!AssetDatabase.IsValidFolder("Assets/Resources"))
                AssetDatabase.CreateFolder("Assets", "Resources");

            var logicGraph = ScriptableObject.CreateInstance<LogicGraph>();
            AssetDatabase.CreateAsset(logicGraph, "Assets/Resources/" + filename + ".asset");
            var comps = new TextAsset(JsonConvert.SerializeObject(components, Formatting.Indented));
            AssetDatabase.CreateAsset(comps, "Assets/Resources/" + filename + " components.json");
            var names = new TextAsset(JsonConvert.SerializeObject(nameToGuid, Formatting.Indented));
            AssetDatabase.CreateAsset(names, "Assets/Resources/" + filename + " names.json");
            logicGraph.jsonComponents = comps;
            logicGraph.jsonNameToGuid = names;
            AssetDatabase.SaveAssets();
            Debug.Log("Saved graph");
        }

        public void SaveGraph()
        {
            string file = EditorUtility.SaveFilePanel("Save graph", "Assets", "My graph", "graph");
            if (file.Length > 0)
            {
                NodeGraphStorage graphStorage = new NodeGraphStorage();

                var logicNodes = nodes.ToList().Cast<LogicNode>();
                foreach (var node in logicNodes)
                {
                    var position = node.GetPosition().position;
                    graphStorage.AddNode(new NodeGraphStorageItem(node.GUID, position.x, position.y, node.NodeType, node.NodeName, node.title, node.State));
                }

                var connectedPorts = edges.ToList().Where(x => x.input.node != null && x.output.node != null).ToArray();
                foreach (var edge in connectedPorts)
                {
                    var input = edge.input.node as LogicNode;
                    var output = edge.output.node as LogicNode;

                    graphStorage.AddConnection(new NodeGraphStorageConnection(output.GUID, input.GUID));
                }

                graphStorage.SaveToFile(file);
                EditorUtility.DisplayDialog("Save graph", "Successfully saved graph", "Ok");
            }
            else
            {
                EditorUtility.DisplayDialog("Save graph", "Operation canceld", "Ok");
            }
        }

        public void LoadGraph()
        {
            string file = EditorUtility.OpenFilePanel("Load graph", "Assets", "graph");
            if (File.Exists(file))
            {
                NodeGraphStorage graphStorage = NodeGraphStorage.LoadFromFile(file);

                ClearGraph();
                CreateNodes(graphStorage.Nodes);
                ConnectNodes(graphStorage.Connections);

                EditorUtility.DisplayDialog("Load graph", "Loaded", "Ok");
            }
            else
            {
                EditorUtility.DisplayDialog("Load graph", "Operation canceld", "Ok");
            }
        }

        private void ClearGraph()
        {
            var Nodes = nodes.ToList().Cast<LogicNode>().ToList();

            foreach (var node in Nodes)
            {
                var Edges = edges.ToList();
                Edges.Where(x => x.input.node == node || x.output.node == node).ToList().ForEach(edge => RemoveElement(edge));

                RemoveElement(node);
            }
        }

        private void CreateNodes(List<NodeGraphStorageItem> nodeItems)
        {
            foreach (var node in nodeItems)
            {
                var tempNode = new LogicNode()
                {
                    GUID = node.NodeGUID,
                    title = node.NodeDisplayName,
                    NodeName = node.NodeName,
                    NodeType = node.NodeType,
                    State = node.NodeState
                };

                tempNode.SetupPorts();

                tempNode.RefreshExpandedState();
                tempNode.RefreshPorts();
                tempNode.SetPosition(new Rect(new Vector2(node.NodePositionX, node.NodePositionY), defaultNodeSize));

                AddElement(tempNode);
            }
        }

        private void ConnectNodes(List<NodeGraphStorageConnection> nodeConnections)
        {
            var Nodes = nodes.ToList().Cast<LogicNode>().ToList();

            for (var i = 0; i < Nodes.Count; i++)
            {
                var connections = nodeConnections.Where(x => x.OutputPort == Nodes[i].GUID).ToList();
                for (int j = 0; j < connections.Count; j++)
                {
                    var targetNodeGuid = connections[j].InputPort;
                    var targetNode = Nodes.First(x => x.GUID == targetNodeGuid);

                    var targetPort = (Port)targetNode.inputContainer[0];
                    if (targetPort.connected)
                        targetPort = (Port)targetNode.inputContainer[1];

                    LinkNodes(Nodes[i].outputContainer[j].Q<Port>(), targetPort);
                }
            }
        }

        private void LinkNodes(Port output, Port input)
        {
            var tempEdge = new Edge
            {
                output = output,
                input = input
            };

            tempEdge?.input.Connect(tempEdge);
            tempEdge?.output.Connect(tempEdge);
            Add(tempEdge);
        }

        public void CompileGraph(string fileName)
        {
            SerializeGraph(fileName);
        }

        public void CreateNode(LogicType type, string nodename)
        {
            var node = CreateDialogeNode(type, nodename);
            AddElement(node);
        }

        private LogicNode CreateDialogeNode(LogicType type, string nodeName)
        {
            var node = new LogicNode
            {
                title = nodeName,
                GUID = Guid.NewGuid().ToString(),
                NodeType = type
            };

            node.SetupPorts();

            node.RefreshExpandedState();
            node.RefreshPorts();
            node.SetPosition(new Rect(Vector2.zero, defaultNodeSize));

            return node;
        }

        private void CreateNode(LogicType type, string nodeName, Vector2 position)
        {
            Debug.Log($"{position.x}, {position.y}");

            var node = new LogicNode
            {
                title = nodeName,
                GUID = Guid.NewGuid().ToString(),
                NodeType = type
            };

            node.SetupPorts();

            node.RefreshExpandedState();
            node.RefreshPorts();
            node.SetPosition(new Rect(position, defaultNodeSize));

            AddElement(node);
        }
    }
}