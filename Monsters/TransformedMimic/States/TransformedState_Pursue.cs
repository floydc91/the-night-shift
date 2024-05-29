using UnityEngine;

namespace CSE5912.PenguinProductions
{
    public class TransformedState_Pursue : IState
    {
        private TransformedReferences transformedReferences;

        private float timeSinceAttack;
        //private float deadline;

        public TransformedState_Pursue(TransformedReferences transformedReferences)
        {
            this.transformedReferences = transformedReferences;
            timeSinceAttack = 0;
        }

        public Color GizmoColor()
        {
            return Color.yellow;
        }

        public void OnEnter()
        {
            Debug.Log("Entered Mimic pursue State");
        }

        public void OnExit()
        {
        }

        public void Tick()
        {
            if (transformedReferences.NavAgent.remainingDistance < 0.5f && timeSinceAttack > 0.1f)
            {
                transformedReferences.PlayerStats.Damage(3);
                timeSinceAttack = 0;
            }
            else
            {
                timeSinceAttack += Time.deltaTime;
            }
        }
    }
}
