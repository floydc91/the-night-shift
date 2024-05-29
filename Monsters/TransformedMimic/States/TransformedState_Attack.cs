using UnityEngine;

namespace CSE5912.PenguinProductions
{
    public class TransformedState_Attack : IState
    {
        private readonly TransformedReferences transformedReferences;
        private float timeSinceAttack;
        //private float deadline;

        public TransformedState_Attack(TransformedReferences transformedReferences)
        {
            this.transformedReferences = transformedReferences;
            timeSinceAttack = 0;
        }

        public Color GizmoColor()
        {
            return Color.red;
        }

        public void OnEnter()
        {
            Debug.Log("Entered Mimic attack State");
        }

        public void OnExit()
        {

        }

        public void Tick()
        {
            if (transformedReferences.NavAgent.remainingDistance < 0.5f && timeSinceAttack > 0.1f)
            {
                transformedReferences.PlayerStats.Damage(25);
                timeSinceAttack = 0;

            }
            else
            {
                timeSinceAttack += Time.deltaTime;
            }
        }
    }
}
