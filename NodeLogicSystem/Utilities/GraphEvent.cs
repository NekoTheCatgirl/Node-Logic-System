using System;
using UnityEngine.Events;

namespace NodeLogicSystem.Utilities
{
    [Serializable]
    public class GraphEvent : UnityEvent<bool>
    {
        public string guid;
        public string Name;

        public GraphEvent()
        {

        }

        public GraphEvent(string guid, string n)
        {
            this.guid = guid;
            this.Name = n;
        }
    }
}
