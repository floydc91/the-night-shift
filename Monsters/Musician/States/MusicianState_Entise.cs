using UnityEngine;

namespace CSE5912.PenguinProductions
{
    public class MusicianState_Entise : IState
    {

        private readonly MusicianReferences musicianReferences;
        private int _entise;
        //Add player event depending on Player implementation

        public MusicianState_Entise(MusicianReferences musicianReferences)
        {
            this.musicianReferences = musicianReferences;
            
        }

        public Color GizmoColor()
        {
            return Color.green;
        }

        public void OnEnter()
        {
            musicianReferences.MonsterSFXSource.Play();
            
            musicianReferences.PlayerStats.IsEnticed = true;

            musicianReferences.PlayerNavAgent.enabled = true;
            musicianReferences.PlayerNavAgent.destination = musicianReferences.transform.position;
        }

        public void OnExit()
        {
            musicianReferences.MonsterSFXSource.Stop();
            
            if (musicianReferences.PlayerStats != null) musicianReferences.PlayerStats.IsEnticed = false;

            if (musicianReferences.PlayerNavAgent != null) musicianReferences.PlayerNavAgent.enabled = false;
        }

        public void Tick() { }
    }
}
