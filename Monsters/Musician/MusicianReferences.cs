using UnityEngine;
using UnityEngine.AI;

namespace CSE5912.PenguinProductions
{
    public class MusicianReferences : MonsterReferences
    {
        [SerializeField] AudioClip musicianAudioClip;
        [HideInInspector] public NavMeshAgent PlayerNavAgent;

        protected override void GetSpecificReferences()
        {
            NavAgent.enabled = false;
        }
    }
}
