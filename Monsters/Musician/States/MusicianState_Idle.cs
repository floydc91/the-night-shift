using UnityEngine;

namespace CSE5912.PenguinProductions
{
    public class MusicianState_Idle : IState
    {
        private readonly MusicianReferences musicianReferences;
        private int _idle;

        public MusicianState_Idle(MusicianReferences musicianReferences)
        {
            this.musicianReferences = musicianReferences;
            _idle = Animator.StringToHash("idle");
        }

        public Color GizmoColor()
        {
            return Color.yellow;
        }

        public void OnEnter()
        {
            musicianReferences.Animator.SetBool(_idle, true);
        }

        public void OnExit()
        {
            musicianReferences.Animator.SetBool(_idle, false);
        }

        public void Tick() { }
    }
}
