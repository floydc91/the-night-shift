using UnityEngine;

namespace CSE5912.PenguinProductions
{
    public class TransformedState_Idle : IState
    {

        private readonly TransformedReferences transformedReferences;
        private float deadline;
        private int _animIDMotionVelocityX = Animator.StringToHash("VelocityX");
        private int _animIDMotionVelocityZ = Animator.StringToHash("VelocityZ");

        public TransformedState_Idle(TransformedReferences transformedReferences)
        {
            this.transformedReferences = transformedReferences;
        }

        public Color GizmoColor()
        {
            return Color.yellow;
        }

        public void OnEnter()
        {
            Debug.Log("Entered Mimic Idle State");
            deadline = Time.time + 5f;

            transformedReferences.Animator.SetFloat(_animIDMotionVelocityX, 0f);
            transformedReferences.Animator.SetFloat(_animIDMotionVelocityZ, 0f);
        }

        public void OnExit()
        {
            transformedReferences.Animator.SetFloat(_animIDMotionVelocityX, 0f);
            transformedReferences.Animator.SetFloat(_animIDMotionVelocityZ, 4f);
        }

        public void Tick() { }

        public bool DoneWaiting()
        {
            return Time.time >= deadline;
        }
    }
}
