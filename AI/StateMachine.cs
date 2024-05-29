using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Unity.VisualScripting;

namespace CSE5912.PenguinProductions
{
    public class StateMachine
    {
        private IState currentState;

        private Dictionary<Type, List<Transition>> transitions = new Dictionary<Type, List<Transition>>();
        private List<Transition> currentTransitions = new List<Transition> ();
        private List<Transition> anyTransition = new List<Transition>();

        private static List<Transition> EmptyTransitions = new List<Transition>(0);

        private class Transition
        {
            public Func<bool> Condition { get; }
            public IState To { get;  }
            public Transition(IState to, Func<bool> condition)
            {
                To = to;
                Condition = condition;
            }
        }

        public void Tick()
        {
            var transition = GetTransition();
            if (transition != null)
            {
                SetState(transition.To);
            }
            currentState?.Tick();
        }

        public void SetState(IState state)
        {
            if (state == currentState)
                return;

            currentState?.OnExit();
            currentState = state;

            transitions.TryGetValue(currentState.GetType(), out currentTransitions);
            if(currentTransitions == null)
            {
                currentTransitions = EmptyTransitions;
            }

            currentState.OnEnter();
        }

        public void AddTransition(IState from, IState to, Func<bool> predicate)
        {
            if(transitions.TryGetValue(from.GetType(), out var targetTransitions) == false)
            {
                targetTransitions = new List<Transition>();
                transitions[from.GetType()] = targetTransitions;
            }

            targetTransitions.Add(new Transition(to, predicate));
        }

        public void AddAnyTransition(IState to, Func<bool> predicate)
        {
            anyTransition.Add(new Transition(to, predicate));
        }

        public Color GetGizmoColor()
        {
            if(currentState != null)
            {
                return currentState.GizmoColor();
            }
            return Color.grey;
        }

        private Transition GetTransition()
        {
            foreach(Transition transition in anyTransition)
            {
                if (transition.Condition())
                {
                    return transition;
                }
            }

            foreach(Transition transition in currentTransitions)
            {
                if (transition.Condition())
                {
                    return transition;
                }
            }
            return null;
        }
    }
}
