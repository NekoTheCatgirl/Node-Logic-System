using System;
using System.IO;
using System.Collections.Generic;

using Newtonsoft.Json;

namespace NodeLogicSystem
{
    /// <summary>
    /// This class is used by the node editor to save and load node info. Not usable at runtime
    /// </summary>
    internal class NodeGraphStorage
    {
        public List<NodeGraphStorageItem> Nodes = new List<NodeGraphStorageItem>();
        public List<NodeGraphStorageConnection> Connections = new List<NodeGraphStorageConnection>();

        internal void AddNode(NodeGraphStorageItem n) => Nodes.Add(n);
        internal void AddConnection(NodeGraphStorageConnection c) => Connections.Add(c);

        internal static NodeGraphStorage LoadFromFile(string file)
        {
            return JsonConvert.DeserializeObject<NodeGraphStorage>(File.ReadAllText(file));
        }

        internal void SaveToFile(string file)
        {
            File.WriteAllText(file, JsonConvert.SerializeObject(this, Formatting.Indented));
        }
    }

    /// <summary>
    /// This class stores node information about position, type and extra variables.
    /// </summary>
    internal class NodeGraphStorageItem
    {
        public NodeGraphStorageItem(string nodeGUID, float nodePositionX, float nodePositionY, LogicType nodeType, string nodeName, string nodeDisplayName, bool nodeState)
        {
            NodeGUID = nodeGUID ?? throw new ArgumentNullException(nameof(nodeGUID));
            NodePositionX = nodePositionX;
            NodePositionY = nodePositionY;
            NodeType = nodeType;
            NodeName = nodeName;
            NodeDisplayName = nodeDisplayName ?? throw new ArgumentNullException(nameof(nodeDisplayName));
            NodeState = nodeState;
        }

        public string NodeGUID { get; set; }
        public float NodePositionX { get; set; }
        public float NodePositionY { get; set; }
        public LogicType NodeType { get; set; }
        public string NodeName { get; set; }
        public string NodeDisplayName { get; set; }
        public bool NodeState { get; set; }
    }

    /// <summary>
    /// This class stores node connection information, linking nodes together
    /// </summary>
    internal class NodeGraphStorageConnection
    {
        public NodeGraphStorageConnection(string outputPort, string inputPort)
        {
            OutputPort = outputPort ?? throw new ArgumentNullException(nameof(outputPort));
            InputPort = inputPort ?? throw new ArgumentNullException(nameof(inputPort));
        }

        public string OutputPort { get; set; }
        public string InputPort { get; set; }
    }
}
