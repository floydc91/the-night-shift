using System;
using UnityEngine;
using UnityEngine.AI;

namespace CSE5912.PenguinProductions
{
    public class MusicianStateMachine : MonsterStateMachine
    {
        private MusicianReferences musicianReferences;

        private bool foundPlayer;
        private bool killPlayer;

        protected override void Start()
        {
            base.Start();
            musicianReferences = GetComponent<MusicianReferences>();
            Rooms rooms = FindObjectOfType<Rooms>();

            //States
            var idle = new MusicianState_Idle(musicianReferences);
            var delay = new ZombieState_Delay(3f);
            var entise = new MusicianState_Entise(musicianReferences);
            var attack = new MusicianState_Attack(musicianReferences);

            //Transitions
            At(idle, entise, () => foundPlayer);
            At(entise, delay, () => !foundPlayer);
            At(delay, idle, () => delay.IsDone());
            At(entise, attack, () => killPlayer);
            At(attack, delay, () => !killPlayer);

            //Start State
            stateMachine.SetState(idle);

            //Functions and Conditions
            void At(IState from, IState to, Func<bool> condition) => stateMachine.AddTransition(from, to, condition);
            void Any(IState to, Func<bool> condition) => stateMachine.AddAnyTransition(to, condition);
        }

        protected override void Update()
        {
            base.Update();
            Kill();
        }

        protected override void Look()
        {
            foundPlayer = false;
            for (int i = -180; i <= 180; i += 2)
            {
                for (int j = -5; j < 5; j += 2)
                {
                    Quaternion rotation = Quaternion.Euler(j, i, 0);
                    if (Physics.Raycast(transform.position + Vector3.up, rotation * transform.forward, out hit, 10f, layerMask))
                    {
                        foundPlayer = true;
                        playerStats = hit.collider.gameObject.GetComponent<PlayerStatsSystem>();
                        musicianReferences.PlayerStats = playerStats;
                        musicianReferences.PlayerNavAgent = hit.collider.gameObject.GetComponent<NavMeshAgent>();
                    }

                    Debug.DrawLine(transform.position + Vector3.up, transform.position + Vector3.up + rotation * transform.forward * 10f, Color.blue);
                }
            }
        }

        private void Kill()
        {
            killPlayer = false;

            for (int i = -180; i <= 180; i += 5)
            {
                for (int j = -10; j < 10; j += 2)
                {
                    Quaternion rotation = Quaternion.Euler(j, i, 0);
                    if (Physics.Raycast(transform.position + Vector3.up, rotation * transform.forward, out hit, 1.5f, layerMask))
                    {
                        killPlayer = true;
                    }

                    Debug.DrawLine(transform.position + Vector3.up, transform.position + Vector3.up + rotation * transform.forward * 1.5f, Color.red);
                }
            }
        }
    }
}
