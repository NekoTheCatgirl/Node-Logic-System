using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor;
using UnityEditor.Experimental.GraphView;

namespace NodeLogicSystem
{
    public class LogicNode : Node
    {
        public LogicType NodeType;

        public string GUID;

        public string NodeName;

        public bool State = false;

        private Port GeneratePort(Direction portDirection, Port.Capacity capacity = Port.Capacity.Single)
        {
            return InstantiatePort(Orientation.Horizontal, portDirection, capacity, typeof(bool));
        }

        public void SetupPorts()
        {
            Port port1;
            Port port2;
            Port port3;
            Port port4;
            switch (NodeType)
            {
                case LogicType.AND:
                    styleSheets.Add(Resources.Load<StyleSheet>("NodeAnd"));
                    port1 = GeneratePort(Direction.Input);
                    port1.portName = "Input A";
                    port2 = GeneratePort(Direction.Input);
                    port2.portName = "Input B";
                    port3 = GeneratePort(Direction.Output);
                    port3.portName = "Output";
                    
                    inputContainer.Add(port1);
                    inputContainer.Add(port2);
                    outputContainer.Add(port3);
                    break;

                case LogicType.OR:
                    styleSheets.Add(Resources.Load<StyleSheet>("NodeOr"));
                    port1 = GeneratePort(Direction.Input);
                    port1.portName = "Input A";
                    port2 = GeneratePort(Direction.Input);
                    port2.portName = "Input B";
                    port3 = GeneratePort(Direction.Output);
                    port3.portName = "Output";
                    inputContainer.Add(port1);
                    inputContainer.Add(port2);
                    outputContainer.Add(port3);
                    break;

                case LogicType.XOR:
                    styleSheets.Add(Resources.Load<StyleSheet>("NodeXor"));
                    port1 = GeneratePort(Direction.Input);
                    port1.portName = "Input A";
                    port2 = GeneratePort(Direction.Input);
                    port2.portName = "Input B";
                    port3 = GeneratePort(Direction.Output);
                    port3.portName = "Output";
                    inputContainer.Add(port1);
                    inputContainer.Add(port2);
                    outputContainer.Add(port3);
                    break;

                case LogicType.NAND:
                    styleSheets.Add(Resources.Load<StyleSheet>("NodeNand"));
                    port1 = GeneratePort(Direction.Input);
                    port1.portName = "Input A";
                    port2 = GeneratePort(Direction.Input);
                    port2.portName = "Input B";
                    port3 = GeneratePort(Direction.Output);
                    port3.portName = "Output";
                    inputContainer.Add(port1);
                    inputContainer.Add(port2);
                    outputContainer.Add(port3);
                    break;

                case LogicType.NOR:
                    styleSheets.Add(Resources.Load<StyleSheet>("NodeNor"));
                    port1 = GeneratePort(Direction.Input);
                    port1.portName = "Input A";
                    port2 = GeneratePort(Direction.Input);
                    port2.portName = "Input B";
                    port3 = GeneratePort(Direction.Output);
                    port3.portName = "Output";
                    inputContainer.Add(port1);
                    inputContainer.Add(port2);
                    outputContainer.Add(port3);
                    break;

                case LogicType.XNOR:
                    styleSheets.Add(Resources.Load<StyleSheet>("NodeXnor"));
                    port1 = GeneratePort(Direction.Input);
                    port1.portName = "Input A";
                    port2 = GeneratePort(Direction.Input);
                    port2.portName = "Input B";
                    port3 = GeneratePort(Direction.Output);
                    port3.portName = "Output";
                    inputContainer.Add(port1);
                    inputContainer.Add(port2);
                    outputContainer.Add(port3);
                    break;

                case LogicType.NOT:
                    styleSheets.Add(Resources.Load<StyleSheet>("NodeNot"));
                    port1 = GeneratePort(Direction.Input);
                    port1.portName = "Input";
                    port3 = GeneratePort(Direction.Output);
                    port3.portName = "Output";
                    inputContainer.Add(port1);
                    outputContainer.Add(port3);
                    break;

                case LogicType.INPUT:
                    styleSheets.Add(Resources.Load<StyleSheet>("NodeInput"));
                    port3 = GeneratePort(Direction.Output);
                    port3.portName = "Output";
                    outputContainer.Add(port3);
                    break;

                case LogicType.OUTPUT:
                    styleSheets.Add(Resources.Load<StyleSheet>("NodeOutput"));
                    port1 = GeneratePort(Direction.Input);
                    port1.portName = "Input";
                    inputContainer.Add(port1);
                    break;

                case LogicType.SPLITTER:
                    styleSheets.Add(Resources.Load<StyleSheet>("NodeSplitter"));
                    port1 = GeneratePort(Direction.Input);
                    port1.portName = "Input";
                    port3 = GeneratePort(Direction.Output);
                    port3.portName = "Output A";
                    port4 = GeneratePort(Direction.Output);
                    port4.portName = "Output B";
                    inputContainer.Add(port1);
                    outputContainer.Add(port3);
                    outputContainer.Add(port4);
                    break;

                case LogicType.CONST:
                    styleSheets.Add(Resources.Load<StyleSheet>("NodeConst"));
                    port3 = GeneratePort(Direction.Output);
                    port3.portName = "Output";
                    outputContainer.Add(port3);
                    var toggle = new Toggle();
                    toggle.tooltip = "State";
                    toggle.RegisterValueChangedCallback((val) => { State = val.newValue; });
                    this.titleContainer.Add(toggle);
                    break;
            }
            if (NodeType == LogicType.INPUT || NodeType == LogicType.OUTPUT)
            {
                var nameField = new TextField(8, false, false, ' ');
                nameField.tooltip = "Name";
                nameField.SetValueWithoutNotify(NodeName);
                nameField.MarkDirtyRepaint();
                nameField.RegisterValueChangedCallback((val) => { NodeName = val.newValue; });
                nameField.name = "IOField";
                this.titleContainer.Add(nameField);
            }
        }
    }
}