using UnityEngine;
using Random = System.Random;

namespace CSE5912.PenguinProductions
{
    [DisallowMultipleComponent]
    public class TransformedReferences : MonsterReferences
    {
        [HideInInspector] public Random random;

        protected override void GetSpecificReferences()
        {
            random = new Random();
        }
    }
}
