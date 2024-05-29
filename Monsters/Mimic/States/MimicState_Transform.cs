using UnityEngine;

namespace CSE5912.PenguinProductions
{
    public class MimicState_Transform : IState
    {

        private readonly MimicReferences mimicReferences;

        public MimicState_Transform(MimicReferences mimicReferences)
        {
            this.mimicReferences = mimicReferences;
        }

        public Color GizmoColor()
        {
            return Color.green;
        }

        public void OnEnter()
        {
            mimicReferences.Transform();
        }

        public void OnExit() { }

        public void Tick() { }
    }
}
