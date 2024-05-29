using System;
using UnityEngine;

namespace CSE5912.PenguinProductions
{
    public class GargoyleStateMachine : MonsterStateMachine
    {
        private GargoyleReferences gargoyleReferences;

        private bool foundPlayer;

        protected override void Start()
        {
            base.Start();
            gargoyleReferences = GetComponent<GargoyleReferences>();
            Rooms rooms = FindAnyObjectByType<Rooms>();

            var idle = new GargoyleState_Idle(gargoyleReferences);
            var pursue = new GargoyleState_Pursue(gargoyleReferences);
            var wander = new GargoyleState_Wander(gargoyleReferences);
            var attack = new GargoyleState_Attack(gargoyleReferences);

            At(idle, pursue, () => foundPlayer);
            At(wander, pursue, () => foundPlayer);
            At(pursue, wander, () => !foundPlayer);
            At(pursue, attack, () => gargoyleReferences.NavAgent.remainingDistance < 3.0f && foundPlayer);
            At(attack, pursue, () => gargoyleReferences.NavAgent.remainingDistance > 3.0f || !foundPlayer);
            At(wander, idle, () => gargoyleReferences.NavAgent.remainingDistance < 0.5f && !foundPlayer);

            stateMachine.SetState(idle);

            void At(IState from, IState to, Func<bool> condition) => stateMachine.AddTransition(from, to, condition);
            void Any(IState to, Func<bool> condition) => stateMachine.AddAnyTransition(to, condition);
        }

        protected override void Look()
        {
            foundPlayer = false;
            for (int i = -180; i <= 180; i += 5)
            {
                for (int j = -10; j < 10; j += 2)
                {
                    Quaternion rotation = Quaternion.Euler(j, i, 0);
                    if (Physics.Raycast(transform.position + Vector3.up, rotation * transform.forward, out hit, 12f, layerMask))
                    {
                        foundPlayer = true;
                        gargoyleReferences.NavAgent.destination = hit.transform.position;
                        playerStats = hit.collider.gameObject.GetComponent<PlayerStatsSystem>();
                        gargoyleReferences.PlayerStats = playerStats;
                    }

                    Debug.DrawLine(transform.position + Vector3.up, transform.position + Vector3.up + rotation * transform.forward * 12f, Color.red);
                }
            }
        }
    }
}
