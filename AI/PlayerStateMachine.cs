using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CSE5912.PenguinProductions
{
    public class PlayerStateMachine : MonoBehaviour
    {
        bool seesEntity;
        int firstRay;
        [SerializeField] float speed;
        float rotationSpeed;
        float health;

        Rigidbody rigidbody;

        //Start is called before the first frame update
        void Start()
        {
            seesEntity = false;
            speed = 2f;
            health = 100;
            rotationSpeed = 700;
            rigidbody = GetComponent<Rigidbody>();
        }

        //Update is called once per frame
        void Update()
        {
            look();
            move();
            rotate();
        }

        //Scans the field of view for any entity and records the position of the first ray that found something
        public void look()
        {
            seesEntity = false;
            for (int i = -100; i <= 100; i += 5)
            {
                Quaternion rotation = Quaternion.Euler(0, i, 0);
                if (Physics.Raycast(transform.position, rotation * transform.forward, 2f))
                {
                    seesEntity = true;
                    firstRay = i;
                }
                Debug.DrawLine(transform.position, transform.position + rotation * transform.forward * 2f, Color.red);
            }
        }

        //Moves forward, while resetting the y-value and velocity in case they were affected by collisions
        public void move()
        {
            transform.Translate(Vector3.forward * speed * Time.deltaTime, Space.Self);
            transform.position = new Vector3(transform.position.x, -4, transform.position.z);
            rigidbody.velocity = Vector3.zero;
        }

        //Rotates away from any entity it sees, otherwise rotates randomly
        public void rotate()
        {
            if (seesEntity)
            {
                transform.rotation = Quaternion.RotateTowards(transform.rotation, transform.rotation * Quaternion.Euler(0, -firstRay, 0), rotationSpeed * Time.deltaTime);
            }
            else
            {
                transform.rotation = Quaternion.RotateTowards(transform.rotation, transform.rotation * Quaternion.Euler(0, Random.Range(-10, 10), 0), rotationSpeed * Time.deltaTime);
            }
            transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0);
            rigidbody.angularVelocity = Vector3.zero;
        }

        //Resets prey to starting position upon collision with a predator
        void OnTriggerStay(Collider collision)
        {
            //Debug.Log("collided");
            if (collision.CompareTag("Monster"))
            {
                health -= .2f;
                //Debug.Log(health);
            }
        }
    }
}
