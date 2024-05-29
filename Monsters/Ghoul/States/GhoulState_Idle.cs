using UnityEngine;

namespace CSE5912.PenguinProductions
{
    public class GhoulState_Idle : IState
    {

        private readonly GhoulReferences ghoulReferences;
        private float deadline;
        private int _idleOne;

        public GhoulState_Idle(GhoulReferences ghoulReferences)
        {
            this.ghoulReferences = ghoulReferences;
            _idleOne = Animator.StringToHash("IdleOne");
        }

        public Color GizmoColor()
        {
            return Color.yellow;
        }

        public void OnEnter()
        {
            deadline = Time.time + 5f;
            ghoulReferences.Animator.SetBool(_idleOne, true);


        }

        public void OnExit()
        {
            ghoulReferences.Animator.SetBool(_idleOne, false);
        }

        public void Tick()
        {

        }

        public bool DoneWaiting()
        {
            return Time.time >= deadline;
        }
    }
}
