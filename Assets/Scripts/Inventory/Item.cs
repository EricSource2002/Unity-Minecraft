using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemProperties
{
    public BlockType blockType;
    public Sprite itemSprite;
}
public class Item
{
    static Sprite getItemSprite(string spriteName)
    {
        if (spriteName == null) spriteName = "404";
        Sprite sprite = Resources.Load<Sprite>($"items/{spriteName}");
        return sprite;
    }
    public static Dictionary<ItemType, ItemProperties> items = new Dictionary<ItemType, ItemProperties>(){
        {ItemType.Wood, new ItemProperties{blockType = BlockType.Wood, itemSprite = getItemSprite("log_oak_top")}},
        {ItemType.Dirt, new ItemProperties{blockType = BlockType.Dirt, itemSprite = getItemSprite("dirt")}},
        {ItemType.Grass, new ItemProperties{blockType = BlockType.Grass, itemSprite = getItemSprite(null)}},
        {ItemType.Stone, new ItemProperties{blockType = BlockType.Stone, itemSprite = getItemSprite("stone")}},
    };
    public enum ItemType { Wood, Stone, Dirt, Grass }
}