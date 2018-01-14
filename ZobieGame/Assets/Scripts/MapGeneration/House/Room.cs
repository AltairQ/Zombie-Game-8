using UnityEngine;

public class Room : MapObject
{
    private bool _hasLoot = false;
    public Room(Rect rect) : base(rect)
    {
    }

    public override void Generate()
    {
        _hasLoot = GeneratorAssets.Get().HouseSettings.RoomSpawnChance >= Random.Range(0f, 1f);
    }

    public override GameObject Make()
    {
        GameObject item = null;
        if(_hasLoot)
        {
            item = GeneratorAssets.Get().LootSettings.GetRandomItem();
            item.transform.position = Rect.Center(ObjectHeight.Floor) + new Vector3(0, item.transform.lossyScale.y, 0);
        }
        return item;
    }
}
