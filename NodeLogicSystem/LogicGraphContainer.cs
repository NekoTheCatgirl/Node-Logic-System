using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using NodeLogicSystem.Utilities;

namespace NodeLogicSystem
{
    [DisallowMultipleComponent]
    public class LogicGraphContainer : MonoBehaviour
    {
        [SerializeField] LogicGraph graph;

        public string GetGuidFromName(string name)
        {
            if (graph != null)
                return graph.nameToGuid[name];
            return null;
        }

        public string[] GetAllGuids()
        {
            if (graph != null)
                return graph.components.Keys.ToArray();
            return null;
        }

        public string[] GetInputNames()
        {
            if (graph != null)
                return graph.InputNames.ToArray();
            return null;
        }

        public string[] GetOutputNames()
        {
            if (graph != null)
                return graph.OutputNames.ToArray();
            return null;
        }

        [SerializeField] List<GraphEvent> graphEvents = new List<GraphEvent>();

        [ContextMenu("Print guid's")]
        public void PrintAllGUID()
        {
            foreach (var guid in GetAllGuids())
            {
                Debug.Log(guid);
            }
        }

        /// <summary>
        /// Do not call this function
        /// </summary>
        public void SetupContainer()
        {
            if (graph == null)
            {
                graphEvents = new List<GraphEvent>();
                return;
            }

            graph.Initialize(HandleOutputEvent);

            List<GraphEvent> events = new List<GraphEvent>();

            foreach (string name in graph.OutputNames)
            {
                events.Add(new GraphEvent(graph.nameToGuid[name], name));
            }

            graphEvents = events;
        }

        private void Start()
        {
            graph.Initialize(HandleOutputEvent);
        }

        private void HandleOutputEvent(string guid, bool value)
        {
            graphEvents.Where(x => x.guid == guid).First().Invoke(value);
        }

        public void SetInput(string guid, bool state) => graph.SetInput(guid, state);

        public bool GetOutput(string guid) => graph.GetOutput(guid);

        public bool GetComponentState(string guid) => graph.GetComponentState(guid);
    }
}