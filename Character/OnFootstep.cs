using UnityEngine;

namespace CSE5912.PenguinProductions
{
    public class OnFootstep : MonoBehaviour
    {
        [SerializeField]
        AudioClip _footstep;
        [SerializeField]
        AudioClip _jumpSound;
        [SerializeField]
        AudioClip _landSound;
        [SerializeField]
        AudioClip _crouchStep;
        [SerializeField]
        AudioSource _audioSource;

        public void OnFootStep()
        {
            _audioSource.PlayOneShot(_footstep);
        }

        public void OnJumpSound()
        {
            _audioSource.PlayOneShot(_jumpSound);
        }
        public void OnLandSound()
        {
            _audioSource.PlayOneShot(_landSound);
        }
        public void OnSneakSound()
        {
            _audioSource.PlayOneShot(_crouchStep);

        }
    }
}