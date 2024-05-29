using UnityEngine;
using UnityEngine.AI;

namespace CSE5912.PenguinProductions
{
    public class GargoyleState_Wander : IState
    {

        private readonly GargoyleReferences gargoyleReferences;

        public GargoyleState_Wander(GargoyleReferences gargoyleReferences)
        {
            this.gargoyleReferences = gargoyleReferences;
        }

        public Color GizmoColor()
        {
            return Color.blue;
        }

        public void OnEnter()
        {
            //RoomPosition nextRoom = this.rooms.GetRandomRoomPosition(gargoyleReferences.transform.position);
            gargoyleReferences.NavAgent.SetDestination(gargoyleReferences.home);

        }

        public void OnExit()
        {

        }

        public void Tick()
        {

        }

        public bool HasArrivedAtDestination()
        {
            return gargoyleReferences.NavAgent.remainingDistance < 0.1f;
        }


        public Vector3 RandomNavmeshLocation(float radius)
        {
            Vector3 randomDirection = Random.onUnitSphere * radius;

            randomDirection += gargoyleReferences.transform.position;


            Vector3 finalPosition = Vector3.zero;

            if (NavMesh.SamplePosition(randomDirection, out NavMeshHit hit, radius, 1))
            {
                finalPosition = hit.position;
            }

            return finalPosition;
        }
    }
}
