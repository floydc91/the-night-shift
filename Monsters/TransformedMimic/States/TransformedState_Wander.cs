using UnityEngine;
using UnityEngine.AI;

namespace CSE5912.PenguinProductions
{
    public class TransformedState_Wander : IState
    {

        private readonly TransformedReferences transformedReferences;
        private readonly Rooms rooms;

        private int _animIDMotionVelocityX = Animator.StringToHash("VelocityX");
        private int _animIDMotionVelocityZ = Animator.StringToHash("VelocityZ");

        public TransformedState_Wander(TransformedReferences transformedReferences, Rooms rooms)
        {
            this.transformedReferences = transformedReferences;
            this.rooms = rooms;
        }

        public Color GizmoColor()
        {
            return Color.green;
        }

        public void OnEnter()
        {
            //RoomPosition nextRoom = this.rooms.GetRandomRoomPosition(transformedReferences.transform.position);
            //transformedReferences.navMeshAgent.SetDestination(nextRoom.transform.position);
            Debug.Log("Entered Mimic wander State");
            transformedReferences.Animator.SetFloat(_animIDMotionVelocityX, 0f);
            transformedReferences.Animator.SetFloat(_animIDMotionVelocityZ, 4f);
            transformedReferences.NavAgent.SetDestination(RandomNavmeshLocation(10f));
        }

        public void OnExit()
        {
            transformedReferences.Animator.SetFloat(_animIDMotionVelocityX, 0f);
            transformedReferences.Animator.SetFloat(_animIDMotionVelocityZ, 0f);

        }

        public void Tick()
        {
        }

        public bool HasArrivedAtDestination()
        {
            return transformedReferences.NavAgent.remainingDistance < 0.1f;
        }
        public Vector3 RandomNavmeshLocation(float radius)
        {

            Vector3 randomDirection = Random.insideUnitSphere * radius;

            randomDirection += transformedReferences.transform.position;

            NavMeshHit hit;

            Vector3 finalPosition = Vector3.zero;

            if (NavMesh.SamplePosition(randomDirection, out hit, radius, 1))

            {

                finalPosition = hit.position;

            }

            return finalPosition;

        }
    }
}
