using UnityEngine;

namespace CSE5912.PenguinProductions
{
    public class MimicState_Idle : IState
    {

        private readonly MimicReferences mimicReferences;
        private float deadline;

        public MimicState_Idle(MimicReferences mimicReferences)
        {
            this.mimicReferences = mimicReferences;
        }

        public Color GizmoColor()
        {
            return Color.yellow;
        }

        public void OnEnter()
        {
            deadline = Time.time + 5f;
        }

        public void OnExit() { }

        public void Tick() { }

        public bool DoneWaiting()
        {
            return Time.time >= deadline;
        }
    }
}
