using UnityEngine;

namespace CSE5912.PenguinProductions
{
    public class GargoyleState_Attack : IState
    {

        private readonly GargoyleReferences gargoyleReferences;
        private float timeSinceAttack;
        private int _flyAttack2;

        public GargoyleState_Attack(GargoyleReferences gargoyleReferences)
        {
            this.gargoyleReferences = gargoyleReferences;
            _flyAttack2 = Animator.StringToHash("flyAttack2");
            timeSinceAttack = 4f;
        }

        public Color GizmoColor()
        {
            return Color.black;
        }

        public void OnEnter()
        {
        }

        public void OnExit()
        {
        }

        public void Tick()
        {
            if (timeSinceAttack > 4.0f)
            {
                gargoyleReferences.PlayerStats.Damage(100);
                gargoyleReferences.Animator.SetTrigger(_flyAttack2);
                timeSinceAttack = 0;
            }
            else
            {
                timeSinceAttack += Time.deltaTime;
            }
        }
    }
}
