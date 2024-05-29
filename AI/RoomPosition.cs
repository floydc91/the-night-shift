using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CSE5912.PenguinProductions
{
    public class RoomPosition : MonoBehaviour
    {
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawCube(transform.position, Vector3.one * 0.3f);
        }
    }
}
