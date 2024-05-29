using UnityEngine;
using Random = System.Random;

namespace CSE5912.PenguinProductions
{
    [DisallowMultipleComponent]
    public class GhoulReferences : MonsterReferences
    {
        [HideInInspector] public Random random;
        public AudioClip ghoulSFX;
    }
}
