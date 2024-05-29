using UnityEngine;

namespace CSE5912.PenguinProductions
{
    public class Animations : MonoBehaviour
    {
        public static Animations Singleton { get; private set; }

        public AnimationClip OriginalDanceClip;
        public AnimationClip[] OverrideDanceClips;

        private void Awake()
        {
            if (Singleton == null)
            {
                Singleton = this;
                // This script is under the InputManager, so it won't be destroyed
            }
            else
            {
                Debug.Log("There are multiple Animations objects in the scene, but there should only be one. Please remove all but one Animations object.");
            }
        }
    }
}
