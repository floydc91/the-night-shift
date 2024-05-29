using Unity.Netcode;
using UnityEngine;

namespace CSE5912.PenguinProductions
{
    public class Spawner : MonoBehaviour
    {
        [SerializeField] private GameObject prefab;

        // Start is called before the first frame update
        void Start()
        {
            Spawn();
        }

        public void Spawn()
        {
            var instance = Instantiate(prefab, transform.position, transform.rotation);
            var instanceNetworkObject = instance.GetComponent<NetworkObject>();
            instanceNetworkObject.Spawn();
            Destroy(gameObject);
        }
    }
}