using UnityEngine;
using System.Collections.Generic;

namespace NodeLogicSystem
{
    public class LogicInputCaller : MonoBehaviour
    {
        [SerializeField] LogicGraphContainer Target;
        public string[] InputNodes
        {
            get
            {
                if (Target != null)
                    return Target.GetInputNames();
                return new List<string>().ToArray();
            }
        }
        public string InputName;

        private void Start()
        {
            if (Target == null)
            {
                Debug.LogError("Target must be set!");
                Debug.Break();
                return;
            }
        }

        public void Toggle()
        {
            bool value = Target.GetComponentState(Target.GetGuidFromName(InputName));
            Target.SetInput(Target.GetGuidFromName(InputName), !value);
        }
    }
}