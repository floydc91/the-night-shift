using UnityEngine;

namespace CSE5912.PenguinProductions
{
    public interface IState
    {
        void Tick();
        void OnEnter();
        void OnExit();
        Color GizmoColor();
    }
}
