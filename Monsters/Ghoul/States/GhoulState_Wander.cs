using UnityEngine;
using UnityEngine.AI;

namespace CSE5912.PenguinProductions
{
    public class GhoulState_Wander : IState
    {

        private readonly GhoulReferences ghoulReferences;
        private Rooms rooms;
        private int _idleOne;
        private int _run;

        public GhoulState_Wander(GhoulReferences ghoulReferences, Rooms rooms)
        {
            this.ghoulReferences = ghoulReferences;
            this.rooms = rooms;
            _idleOne = Animator.StringToHash("IdleOne");
            _run = Animator.StringToHash("Run");
        }

        public Color GizmoColor()
        {
            return Color.blue;
        }

        public void OnEnter()
        {
            //RoomPosition nextRoom = this.rooms.GetRandomRoomPosition(ghoulReferences.transform.position);
            ghoulReferences.NavAgent.SetDestination(RandomNavmeshLocation(15f));
            ghoulReferences.Animator.SetBool(_idleOne, true);
            ghoulReferences.Animator.SetBool(_run, true);
        }

        public void OnExit()
        {            
            ghoulReferences.Animator.SetBool(_run, false);
        }

        public void Tick()
        {

        }

        public bool HasArrivedAtDestination()
        {
            return ghoulReferences.NavAgent.remainingDistance < 0.1f;
        }


        public Vector3 RandomNavmeshLocation(float radius)
        {
            Vector3 randomDirection = Random.onUnitSphere * radius;

            randomDirection += ghoulReferences.transform.position;


            Vector3 finalPosition = Vector3.zero;

            if (NavMesh.SamplePosition(randomDirection, out NavMeshHit hit, radius, 1))

            {

                finalPosition = hit.position;

            }

            return finalPosition;
        }
    }
}
