using UnityEngine;

namespace CSE5912.PenguinProductions
{
    public class GhoulState_Pursue : IState
    {

        private readonly GhoulReferences ghoulReferences;
        //private float timeSinceAttack;
        private float distance;
        private int _idleOne;
        private int _run;

        public GhoulState_Pursue(GhoulReferences ghoulReferences)
        {
            this.ghoulReferences = ghoulReferences;
            _idleOne = Animator.StringToHash("IdleOne");
            _run = Animator.StringToHash("Run");
            //timeSinceAttack = 0;
        }

        public Color GizmoColor()
        {
            return Color.red;
        }

        public void OnEnter()
        {
            ghoulReferences.MonsterSFXSource.Play();
            ghoulReferences.Animator.SetBool(_idleOne, false);
            ghoulReferences.Animator.SetBool(_run, true);
        }

        public void OnExit()
        {
            ghoulReferences.MonsterSFXSource.Stop();
            ghoulReferences.Animator.SetBool(_run, false);
            ghoulReferences.Animator.SetBool(_idleOne, true);

        }

        public void Tick()
        {
            distance = ghoulReferences.NavAgent.remainingDistance;
        }

        public float Distance()
        {
            return distance;
        }
    }
}
