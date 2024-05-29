using UnityEngine;
using Random = System.Random;

namespace CSE5912.PenguinProductions
{
    [DisallowMultipleComponent]
    public class GargoyleReferences : MonsterReferences
    {
        [HideInInspector] public Random random;
        [HideInInspector] public Vector3 home;
        public AudioClip gargoyleClip;

        protected override void GetSpecificReferences()
        {
            random = new Random();
            home = this.transform.position;
        }
    }
}
