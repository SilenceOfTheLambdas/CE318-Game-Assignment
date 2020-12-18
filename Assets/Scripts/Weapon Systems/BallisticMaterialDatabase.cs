using UnityEngine;

namespace Weapon_Systems
{
    [CreateAssetMenu(fileName = "BallisticData", menuName = "MaterialDatabase", order = 1)]
    public class BallisticMaterialDatabase : ScriptableObject
    {
        #region Variables

        public BallisticMaterial[] materials;

        #endregion
    }

    [System.Serializable]
    public class BallisticMaterial
    {
        #region Variables

        public string name;
        public float  yieldStrength;
        public float  density;
        public float  rndSpread;
        public float  rndSpreadRic;

        #endregion
    }
}