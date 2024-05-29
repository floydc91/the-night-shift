using UnityEngine;

namespace CSE5912.PenguinProductions
{
    public class MimicState_Pursue : IState
    {

        private readonly MimicReferences mimicReferences;
        private float timeSinceAttack;

        public MimicState_Pursue(MimicReferences mimicReferences)
        {
            this.mimicReferences = mimicReferences;
            timeSinceAttack = 0;
        }

        public Color GizmoColor()
        {
            return Color.red;
        }

        public void OnEnter() { }

        public void OnExit() { }

        public void Tick()
        {
            if (mimicReferences.NavAgent.remainingDistance < 0.5f && timeSinceAttack > 0.1f)
            {
                mimicReferences.PlayerStats.Damage(3);
                timeSinceAttack = 0;

                if (mimicReferences.PlayerStats.IsDead)
                {
                    mimicReferences.Transform();
                }
            } 
            else
            {
                timeSinceAttack += Time.deltaTime;
            }
        }
    }
}
