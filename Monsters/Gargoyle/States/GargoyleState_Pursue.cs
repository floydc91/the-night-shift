using UnityEngine;

namespace CSE5912.PenguinProductions
{
    public class GargoyleState_Pursue : IState
    {

        private readonly GargoyleReferences gargoyleReferences;
        //private float timeSinceAttack;
        private float distance;
        //private bool isIdle;

        public GargoyleState_Pursue(GargoyleReferences gargoyleReferences)
        {
            this.gargoyleReferences = gargoyleReferences;
            //timeSinceAttack = 0;
            //this.isIdle = isIdle;
        }

        public Color GizmoColor()
        {
            return Color.red;
        }

        public void OnEnter()
        {   
           gargoyleReferences.MonsterSFXSource.Play();
        }

        public void OnExit()
        {
            gargoyleReferences.MonsterSFXSource.Stop();
        }

        public void Tick()
        {
            distance = gargoyleReferences.NavAgent.remainingDistance;
        }

        public float Distance()
        {
            return distance;
        }
    }
}
