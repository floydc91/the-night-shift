using UnityEngine;

namespace CSE5912.PenguinProductions
{
    public class GargoyleState_Idle : IState
    {

        private readonly GargoyleReferences gargoyleReferences;
        private int _goGround;
        private int _statue1;
        private int _goAir;


        public GargoyleState_Idle(GargoyleReferences gargoyleReferences)
        {
            this.gargoyleReferences = gargoyleReferences;
            _goGround = Animator.StringToHash("goGround");
            _statue1 = Animator.StringToHash("Statue1");
            _goAir = Animator.StringToHash("goAir");
        }

        public Color GizmoColor()
        {
            return Color.yellow;
        }

        public void OnEnter()
        {
            gargoyleReferences.Animator.SetTrigger(_goGround);
            gargoyleReferences.Animator.SetBool(_statue1, true);
        }

        public void OnExit()
        {
            gargoyleReferences.Animator.SetBool(_statue1, false);
            gargoyleReferences.Animator.SetTrigger(_goAir);
        }

        public void Tick()
        {
        }
    }
}
