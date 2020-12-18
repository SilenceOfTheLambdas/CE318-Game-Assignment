using UnityEngine;

namespace Weapon_Systems
{
    public class BallisticMaterialHolder : MonoBehaviour
    {
        [SerializeField] private string materialName;

        public string Material => materialName;
    }
}