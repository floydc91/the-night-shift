using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CSE5912.PenguinProductions
{
    public class Rooms : MonoBehaviour
    {
        private RoomPosition[] roomPositions;

        private void Awake()
        {
            roomPositions = GetComponentsInChildren<RoomPosition>();
        }

        public RoomPosition GetRandomRoomPosition(Vector3 agentLocation)
        {
            return roomPositions[Random.Range(0, roomPositions.Length-1)];
        }
    }
}
