using System;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

namespace NodeLogicSystem
{
    public class LogicGraph : ScriptableObject
    {
        [NonSerialized] public Dictionary<string, LogicComponent> components = new Dictionary<string, LogicComponent>();
        [NonSerialized] public Dictionary<string, string> nameToGuid = new Dictionary<string, string>();
        public TextAsset jsonComponents;
        public TextAsset jsonNameToGuid;

        public List<string> InputNames { get; private set; }
        public List<string> OutputNames { get; private set; }

        [NonSerialized] private Action<string, bool> OutputCallback;

        public void Initialize(Action<string, bool> outputCallback)
        {
            components = JsonConvert.DeserializeObject<Dictionary<string, LogicComponent>>(jsonComponents.text);
            nameToGuid = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonNameToGuid.text);
            OutputCallback = outputCallback;

            InputNames = new List<string>();
            OutputNames = new List<string>();

            foreach (var pair in nameToGuid)
            {
                if (components[pair.Value].type == LogicType.OUTPUT)
                {
                    OutputNames.Add(pair.Key);
                }
                else if (components[pair.Value].type == LogicType.INPUT)
                {
                    InputNames.Add(pair.Key);
                }
            }
        }

        public void SetInput(string guid, bool state)
        {
            if (components[guid].type == LogicType.INPUT)
            {
                Debug.Log($"Set {guid} to {state}");
                components[guid].State = state;
                UpdateLogicGraphComponent(components[guid].OutputPort1);
                UpdateLogicGraphComponent(components[guid].OutputPort2);
            }
            else
                Debug.LogError($"Attempt to write to read only value, {guid} is readonly");
        }

        private void UpdateLogicGraphComponent(string guid)
        {
            if (guid.Length > 0)
            {
                bool input1 = ReadInputIfAvailable(components[guid].InputPort1);
                bool input2 = ReadInputIfAvailable(components[guid].InputPort2);

                Debug.Log($"Updating {guid} with inputs: {input1} and {input2}, logic type is {Enum.GetName(typeof(LogicType), components[guid].type)}");

                components[guid].LogicUpdate(input1, input2);

                switch (components[guid].type)
                {
                    case LogicType.OUTPUT:
                        OutputCallback(guid, components[guid].State);
                        break;

                    default:
                        UpdateLogicGraphComponent(components[guid].OutputPort1);
                        UpdateLogicGraphComponent(components[guid].OutputPort2);
                        break;
                }
            }
        }

        private bool ReadInputIfAvailable(string guid)
        {
            if (guid.Length > 0)
            {
                return components[guid].State;
            }
            return false;
        }

        public bool GetOutput(string guid)
        {
            if (components[guid].type == LogicType.OUTPUT)
            {
                return components[guid].State;
            }
            Debug.LogError("Attempt to read private value");
            return false;
        }

        public bool GetComponentState(string guid)
        {
            return components[guid].State;
        }

        public interface ILogicGraphInterfaceMethods
        {
            public void SetInput(string guid, bool state);
            public bool GetOutput(string guid);
            public bool GetComponentState(string guid);
        }
    }

    [Serializable]
    public class LogicComponent
    {
        public LogicType type;

        public string GUID = "";

        public string InputPort1 = "";
        public string InputPort2 = "";

        public string OutputPort1 = "";
        public string OutputPort2 = "";

        public bool State = false;

        public void LogicUpdate(bool input1, bool input2)
        {
            // Do logic behavior based on the set type
            switch (type)
            {
                case LogicType.AND:
                    State = (input1 == true && input2 == true);
                    break;

                case LogicType.OR:
                    State = (input1 == true || input2 == true);
                    break;

                case LogicType.XOR:
                    State = ((input1 == true && input2 == false) || (input1 == false && input2 == true));
                    break;

                case LogicType.NOT:
                    State = !input1;
                    break;

                case LogicType.NAND:
                    State = (input1 == false || input2 == false);
                    break;

                case LogicType.NOR:
                    State = (input1 == false && input2 == false);
                    break;

                case LogicType.XNOR:
                    State = input1 == input2;
                    break;

                case LogicType.OUTPUT:
                    State = input1;
                    break;

                case LogicType.SPLITTER:
                    State = input1;
                    break;
            }
        }
    }

    public enum LogicType
    {
        AND,
        OR,
        XOR,
        NOT,
        NAND,
        NOR,
        XNOR,
        INPUT,
        OUTPUT,
        CONST,
        SPLITTER
    }
}