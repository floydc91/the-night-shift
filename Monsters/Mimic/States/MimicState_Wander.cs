using UnityEngine;
using UnityEngine.AI;

namespace CSE5912.PenguinProductions
{
    public class MimicState_Wander : IState
    {

        private readonly MimicReferences mimicReferences;
        private readonly Rooms rooms;

        public MimicState_Wander(MimicReferences mimicReferences, Rooms rooms)
        {
            this.mimicReferences = mimicReferences;
            this.rooms = rooms;
        }

        public Color GizmoColor()
        {
            return Color.blue;
        }

        public void OnEnter()
        {
            //RoomPosition nextRoom = this.rooms.GetRandomRoomPosition(mimicReferences.transform.position);
            mimicReferences.NavAgent.SetDestination(RandomNavmeshLocation(10f));
            mimicReferences.MonsterSFXSource.Play();
        }

        public void OnExit()
        {
            mimicReferences.MonsterSFXSource.Stop();
        }

        public void Tick() { }

        public bool HasArrivedAtDestination()
        {
            return mimicReferences.NavAgent.remainingDistance < 0.1f;
        }


        public Vector3 RandomNavmeshLocation(float radius)
        {
            Vector3 randomDirection = Random.insideUnitSphere * radius;

            randomDirection += mimicReferences.transform.position;


            Vector3 finalPosition = Vector3.zero;

            if (NavMesh.SamplePosition(randomDirection, out NavMeshHit hit, radius, 1))
            {
                finalPosition = hit.position;
            }

            return finalPosition;
        }
    }
}
