using System;
using UnityEngine;

namespace CSE5912.PenguinProductions
{
    public class MimicStateMachine : MonsterStateMachine
    {
        private MimicReferences mimicReferences;

        private bool foundPlayer;
        private bool killedPlayer;

        protected override void Start()
        {
            base.Start();
            mimicReferences = GetComponent<MimicReferences>();
            Rooms rooms = FindObjectOfType<Rooms>();

            var idle = new MimicState_Idle(mimicReferences);
            var pursue = new MimicState_Pursue(mimicReferences);
            var wander = new MimicState_Wander(mimicReferences, rooms);
            var transform = new MimicState_Transform(mimicReferences);
            var attack = new MimicState_Attack(mimicReferences);

            At(idle, wander, () => idle.DoneWaiting());
            At(wander, pursue, () => foundPlayer);
            At(idle, pursue, () => foundPlayer);
            At(pursue, idle, () => !foundPlayer);
            At(wander, idle, () => mimicReferences.NavAgent.remainingDistance < 0.5f && !foundPlayer);
            At(pursue, attack, () => mimicReferences.NavAgent.remainingDistance < 0.5f && foundPlayer);
            At(attack, pursue, () => mimicReferences.NavAgent.remainingDistance >= 0.5f || !foundPlayer);
            Any(transform, () => killedPlayer);

            stateMachine.SetState(idle);

            void At(IState from, IState to, Func<bool> condition) => stateMachine.AddTransition(from,to, condition);
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
                        if(mimicReferences.NavAgent != null) mimicReferences.NavAgent.destination = hit.transform.position;
                        playerStats = hit.collider.gameObject.GetComponent<PlayerStatsSystem>();
                        mimicReferences.PlayerStats = playerStats;
                    }

                    Debug.DrawLine(transform.position, transform.position + rotation * transform.forward * 5f, Color.red);
                }
            }
        }
    }
}
