using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CSE5912.PenguinProductions
{
    public class AIEventManager : MonoBehaviour
    {
        public delegate void MovementAction();
        public static event MovementAction OnMoved;

        
    }
}
