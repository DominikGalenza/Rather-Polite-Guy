using System.Collections;
using System.Collections.Generic;
using GameDevTV.Inventories;
using UnityEngine;
using UnityEngine.AI;

namespace RPG.Inventories
{
    public class RandomDropper : ItemDropper
    {
        [Tooltip("How far can the pickup be scatter from the dropper.")]
        [SerializeField] float scatterDistance = 1f;
        [SerializeField] float findPointInRange = 0.1f;
        [SerializeField] InventoryItem[] dropLibrary;
        [SerializeField] int numberOfDrops = 2;

        const int ATTEMPTS = 30;

        public void RandomDrop()
        {
            for (int i = 0; i < numberOfDrops; i++)
            {
                InventoryItem item = dropLibrary[Random.Range(0, dropLibrary.Length)];
                DropItem(item, 1);
            }    
        }

        protected override Vector3 GetDropLocation()
        {
            for (int i = 0; i < ATTEMPTS; i++)
            {
                Vector3 randomPoint = transform.position + Random.insideUnitSphere * scatterDistance;
                NavMeshHit hit;

                if (NavMesh.SamplePosition(randomPoint, out hit, findPointInRange, NavMesh.AllAreas))
                {
                    return hit.position;
                }
            }
            return transform.position;
        }
    }
}
