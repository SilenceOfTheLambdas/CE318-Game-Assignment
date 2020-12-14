using UnityEngine;

namespace AI
{
    public class Cover : MonoBehaviour
    {
        [SerializeField] private Transform[] coverSpots;

        public Transform[] GetCoverSpots()
        {
            return coverSpots;
        }
    }
}