using CSE5912.PenguinProductions.Utility;
using UnityEngine;

namespace CSE5912.PenguinProductions
{
    public abstract class MonsterStateMachine : MonoBehaviour
    {
        protected StateMachine stateMachine;

        protected int layerNumber = (int)LayerEnum.Player;
        protected int layerMask;
        protected RaycastHit hit;

        protected PlayerStatsSystem playerStats;

        protected virtual void Start()
        {
            stateMachine = new StateMachine();
            layerMask = 1 << layerNumber;
        }

        // Update is called once per frame
        protected virtual void Update()
        {
            stateMachine.Tick();
            Look();
        }

        protected abstract void Look();

        protected virtual void OnDrawGizmos()
        {
            if (stateMachine != null)
            {
                Gizmos.color = stateMachine.GetGizmoColor();
                Gizmos.DrawSphere(transform.position + Vector3.up * 3, 0.4f);
            }
        }
    }
}