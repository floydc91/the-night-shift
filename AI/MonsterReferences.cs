using CSE5912.PenguinProductions.Networking;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.AI;

namespace CSE5912.PenguinProductions
{
    /// <summary>
    /// Base class for all monsters to extend. Holds references
    /// </summary>
    [RequireComponent(typeof(NavMeshAgent), typeof(Animator), typeof(AudioSource))]
    [RequireComponent(typeof(MonsterStateMachine))]
    public abstract class MonsterReferences : ExtendedNetworkBehaviour
    {
        [HideInInspector] public NavMeshAgent NavAgent { get; set; }
        [HideInInspector] public Animator Animator { get; set; }
        [HideInInspector] public PlayerStatsSystem PlayerStats { get; set; }
        [HideInInspector] public AudioSource MonsterSFXSource { get; set; }

        protected void Awake()
        {
            NavAgent = GetComponent<NavMeshAgent>();
            Animator = GetComponent<Animator>();
            MonsterSFXSource = GetComponent<AudioSource>();
            MonsterStateMachine stateMachine = GetComponent<MonsterStateMachine>();

            // Disable unneeded stuff on clients.
            if (!NetworkManager.Singleton.IsServer)
            {
                stateMachine.enabled = false;
                NavAgent.enabled = false;
            }

            // Get the additional references each monster needs.
            GetSpecificReferences();
        }

        /// <summary>
        /// Gets the additional references each monster needs.
        /// </summary>
        protected virtual void GetSpecificReferences() { }
    }
}