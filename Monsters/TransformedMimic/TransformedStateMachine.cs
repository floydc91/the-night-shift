using System;
using UnityEngine;

namespace CSE5912.PenguinProductions
{
    public class TransformedStateMachine : MonsterStateMachine
    {
        private TransformedReferences transformedReferences;

        private bool foundPlayer;

        protected override void Start()
        {
            foundPlayer = false;
            base.Start();
            transformedReferences = GetComponent<TransformedReferences>();
            Rooms rooms = FindObjectOfType<Rooms>();

            var idle = new TransformedState_Idle(transformedReferences);
            var pursue = new TransformedState_Pursue(transformedReferences);
            var wander = new TransformedState_Wander(transformedReferences, rooms);
            var attack = new TransformedState_Attack(transformedReferences);

            At(idle, wander, () => idle.DoneWaiting());
            At(wander, pursue, () => foundPlayer);
            At(idle, pursue, () => foundPlayer);
            At(pursue, idle, () => !foundPlayer);
            At(wander, idle, () => wander.HasArrivedAtDestination());
            At(pursue, attack, () => transformedReferences.NavAgent.remainingDistance < 0.5f && foundPlayer);
            At(attack, pursue, () => transformedReferences.NavAgent.remainingDistance >= 0.5f || !foundPlayer);

            stateMachine.SetState(idle);

            void At(IState from, IState to, Func<bool> condition) => stateMachine.AddTransition(from, to, condition);
            void Any(IState to, Func<bool> condition) => stateMachine.AddAnyTransition(to, condition);
        }

        protected override void Look()
        {
            foundPlayer = false;
            for (int i = -180; i <= 180; i += 5)
            {
                for (int j = -50; j < 50; j += 5)
                {
                    Quaternion rotation = Quaternion.Euler(j, i, 0);
                    if (Physics.Raycast(transform.position, rotation * transform.forward, out hit, 5f, layerMask))
                    {
                        foundPlayer = true;
                        transformedReferences.NavAgent.destination = hit.transform.position;
                        playerStats = hit.collider.gameObject.GetComponent<PlayerStatsSystem>();
                        transformedReferences.PlayerStats = playerStats;
                    }

                    Debug.DrawLine(transform.position, transform.position + rotation * transform.forward * 5f, Color.red);
                }
            }
        }
    }
}
