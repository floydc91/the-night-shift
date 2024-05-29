using Unity.Netcode;
using UnityEngine;
using Random = System.Random;

namespace CSE5912.PenguinProductions
{
    [DisallowMultipleComponent]
    public class MimicReferences : MonsterReferences
    {
        [HideInInspector] public Random random;
        public GameObject prefab;
        public AudioClip mimicSFX; //This audio clip is unused

        protected override void GetSpecificReferences()
        {
            random = new();
        }

        /// <summary>
        /// Spawns a transformed mimic.
        /// </summary>
        public void Transform()
        {
            if (!IsServer) return;

            // Spawn transformed mimic
            GameObject instance = Instantiate(prefab, transform.position, transform.rotation);
            if (instance.TryGetComponent(out NetworkObject netObj)) netObj.Spawn(true);

            // Get rid of the mimic
            if (gameObject.TryGetComponent(out netObj)) netObj.Despawn(true);
            else Destroy(gameObject);
        }
    }
}
