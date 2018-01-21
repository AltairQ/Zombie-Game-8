using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "FurnitureSettings", menuName = "Generator/FurnitureSettings")]
public class FurnitureSettings : ScriptableObject
{
    [System.Serializable]
    public class FurnitureSetting
    {
        public GameObject gameObject = null;
        public float yShift;
        public FurnitureSetting Clone()
        {
            var clone = new FurnitureSetting {
                gameObject = Instantiate(gameObject),
                yShift = yShift
            };

            return clone;
        }
    }

    [SerializeField]
    private List<FurnitureSetting> _wallFurnitures; // spawn near walls

    [SerializeField]
    private List<FurnitureSetting> _centralFurnitures; // spawn near center of the room

    private FurnitureSetting GetRandom(List<FurnitureSetting> list)
    {
        int idx = Random.Range(0, list.Count);
        return list[idx].Clone();
    }

    public FurnitureSetting GetRandomWallFurniture()
    {
        return GetRandom(_wallFurnitures);
    }

    public FurnitureSetting GetRandomCentralFurniture()
    {
        return GetRandom(_centralFurnitures);
    }
}
