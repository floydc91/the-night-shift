using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;

namespace CSE5912.PenguinProductions
{
    public class NavMeshBaker : MonoBehaviour
    {

        List<NavMeshSurface> navMeshSurfaces;

        // Start is called before the first frame update
        void Start()
        {
            AddToSurfaces(this.transform.root);
            BuildAll();
        }

        void AddToSurfaces(Transform parent)
        {
            foreach (Transform child in parent)
            {
                NavMeshSurface surface = child.GetComponent<NavMeshSurface>();
                if (surface != null)
                {
                    navMeshSurfaces.Add(surface);
                } else
                {
                    AddToSurfaces(child);
                }
            }
        }

        void BuildAll()
        {
            foreach (NavMeshSurface surface in navMeshSurfaces)
            {
                surface.BuildNavMesh();
            }
        }

    }
}
