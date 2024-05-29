using System;
using UnityEngine;

namespace CSE5912.PenguinProductions
{
    public class GhoulStateMachine : MonsterStateMachine
    {
        private GhoulReferences ghoulReferences;

        private bool foundPlayer;

        protected override void Start()
        {
            base.Start();
            ghoulReferences = GetComponent<GhoulReferences>();
            Rooms rooms = FindAnyObjectByType<Rooms>();

            var idle = new GhoulState_Idle(ghoulReferences);
            var pursue = new GhoulState_Pursue(ghoulReferences);
            var wander = new GhoulState_Wander(ghoulReferences, rooms);
            var attack = new GhoulState_Attack(ghoulReferences);

            At(idle, wander, () => idle.DoneWaiting());
            At(idle, pursue, () => foundPlayer);
            At(wander, pursue, () => foundPlayer);
            At(pursue, wander, () => !foundPlayer);
            At(pursue, attack, () => ghoulReferences.NavAgent.remainingDistance < 1.0f && foundPlayer);
            At(attack, pursue, () => ghoulReferences.NavAgent.remainingDistance > 1.0f || !foundPlayer);
            At(wander, idle, () => ghoulReferences.NavAgent.remainingDistance < 0.5f && !foundPlayer);

            stateMachine.SetState(idle);

            void At(IState from, IState to, Func<bool> condition) => stateMachine.AddTransition(from,to, condition);
            void Any(IState to, Func<bool> condition) => stateMachine.AddAnyTransition(to, condition);
        }

        protected override void Look()
        {
            foundPlayer = false;
            for (int i = -180; i <= 180; i += 5)
            {
                for (int j = -10; j < 10; j+= 2)
                {
                    Quaternion rotation = Quaternion.Euler(j, i, 0);
                    if (Physics.Raycast(transform.position, rotation * transform.forward, out hit, 12f, layerMask))
                    {
                        foundPlayer = true;
                        ghoulReferences.NavAgent.destination = hit.transform.position;
                        playerStats = hit.collider.gameObject.GetComponent<PlayerStatsSystem>();
                        ghoulReferences.PlayerStats = playerStats;
                    }

                    Debug.DrawLine(transform.position, transform.position + rotation * transform.forward * 12f, Color.red);
                }
            }
        }
    }
}
