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
        var settings = GeneratorAssets.Get().HouseSettings;
        if(_hasLoot)
        {
            item = GeneratorAssets.Get().LootSettings.GetRandomItem();
            item.transform.position = Rect.Center(ObjectHeight.Floor) + new Vector3(0, item.transform.lossyScale.y, 0);
        }
        else if(Rect.Area() > (settings.MinRoomArea + settings.MaxRoomArea)*3/4)
        {
            var furniture = GeneratorAssets.Get().FurnitureSettings.GetRandomCentralFurniture();
            item = furniture.gameObject;
            float height = item.transform.lossyScale.y + furniture.yShift;
            item.transform.position = Rect.Center(ObjectHeight.Floor) + new Vector3(0, height, 0);
        }
        return item;
    }
}
