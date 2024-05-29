using UnityEngine;

namespace CSE5912.PenguinProductions
{
    public class MimicState_Attack : IState
    {

        private readonly MimicReferences mimicReferences;
        private float timeSinceAttack;

        public MimicState_Attack(MimicReferences mimicReferences)
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
            if (mimicReferences.NavAgent.remainingDistance < 0.5f && timeSinceAttack > 0.75f)
            {
                mimicReferences.PlayerStats.Damage(50);
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
