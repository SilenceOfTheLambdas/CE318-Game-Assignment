using System;
using System.Linq;
using UnityEngine;

namespace Weapon_Systems
{
    public class BallisticMaterialManager : MonoBehaviour
    {
        public static BallisticMaterialManager  Instance;
        public        BallisticMaterialDatabase database;

        private void Start()
        {
            if (Instance == null) Instance = this;
        }

        public BallisticMaterial GetMaterialFromName(string p_name)
        {
            return database.materials.FirstOrDefault(ballisticMaterial => ballisticMaterial.name == p_name);
        }
    }
}