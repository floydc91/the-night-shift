using UnityEngine;

namespace CSE5912.PenguinProductions
{
    public class MusicianState_Attack : IState
    {
        private readonly MusicianReferences musicianReferences;
        private int _attack;

        public MusicianState_Attack(MusicianReferences musicianReferences)
        {
            this.musicianReferences = musicianReferences;
            _attack = Animator.StringToHash("attack");
        }

        public Color GizmoColor()
        {
            return Color.magenta;
        }

        public void OnEnter()
        {
            musicianReferences.Animator.SetBool(_attack, true);
            musicianReferences.PlayerStats.Damage(999999);
        }

        public void OnExit()
        {
            musicianReferences.Animator.SetBool(_attack, false);
        }

        public void Tick() { }
    }
}
