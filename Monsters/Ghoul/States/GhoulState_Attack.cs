using UnityEngine;

namespace CSE5912.PenguinProductions
{
    public class GhoulState_Attack : IState
    {

        private readonly GhoulReferences ghoulReferences;
        private float timeSinceAttack;
        private int _idleOne;
        
        public GhoulState_Attack(GhoulReferences ghoulReferences)
        {
            this.ghoulReferences = ghoulReferences;
            timeSinceAttack = 1f;
            _idleOne = Animator.StringToHash("IdleOne");
        }

        public Color GizmoColor()
        {
            return Color.black;
        }

        public void OnEnter()
        {
            
            ghoulReferences.NavAgent.destination = ghoulReferences.transform.position;
            ghoulReferences.Animator.SetBool(_idleOne, false);
        }

        public void OnExit()
        {
            ghoulReferences.Animator.SetBool(_idleOne, true);
        }

        public void Tick()
        {
            if (ghoulReferences.NavAgent.remainingDistance < 1.0f && timeSinceAttack > 1.0f)
            {
                if(ghoulReferences.PlayerStats != null)ghoulReferences.PlayerStats.Damage(25);
                timeSinceAttack = 0;

            }
            else
            {
                timeSinceAttack += Time.deltaTime;
            }
        }
    }
}
