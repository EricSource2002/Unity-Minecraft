using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToolBar : MonoBehaviour
{
    public GameObject invHighlighted;
    private int startPosX;
    private int currPosX;
    private int lastPosX;
    private const int space = 60;
    private int invIndex = 0;
    public BlockType selectedBlock;

    public GameObject toolBarSlots;
    static Font arial;
    private static List<Image> slots = new List<Image>();
    //TODO CENTER TOOLBAR UI => BOTTOM_CENTER 
    void Start()
    {
        arial = (Font)Resources.GetBuiltinResource(typeof(Font), "Arial.ttf");
        selectedBlock = BlockType.Stone;
        startPosX = (int)invHighlighted.transform.position.x;
        currPosX = (int)invHighlighted.transform.position.x;
        lastPosX = (int)invHighlighted.transform.position.x;
        for (int i = 0; i < toolBarSlots.transform.childCount; i++)
        {
            slots.Add(toolBarSlots.transform.GetChild(i).GetComponent<Image>());
        }
        UpdateToolBarItems();
    }
    //CALL FROM INVENTORY
    public static void UpdateToolBarItems()
    {
        int i = 0;
        foreach (Image slot in slots)
        {
            if (slot.gameObject.transform.childCount > 0)
            {
                Debug.Log(slot.gameObject.transform.GetChild(0).gameObject.name );
                Destroy(slot.transform.GetChild(0).gameObject);
            }
            if (Inventory.myItems.ContainsKey(i))
            {
                Item.ItemType itemType_ = Inventory.myItems[i].itemType;
                int itemValue_ = Inventory.myItems[i].itemValue;
                GameObject itemChild = new GameObject();
                itemChild.name = "Item => " + itemType_.ToString();
                itemChild.transform.SetParent(slot.gameObject.transform);
                Image itemSprite = itemChild.AddComponent<Image>();
                itemSprite.sprite = Item.items[itemType_].itemSprite;
                itemSprite.raycastTarget = false;
                RectTransform rect = itemChild.GetComponent<RectTransform>();
                rect.localPosition = Vector2.zero;
                rect.localScale = new Vector2(.26f, .47f);
                //_______________________________________
                GameObject valueGameObject = new GameObject();
                valueGameObject.name = "Value => " + itemType_.ToString();
                valueGameObject.transform.SetParent(itemChild.transform);
                Text valueTxt = valueGameObject.AddComponent<Text>();
                RectTransform rectTransform = valueGameObject.GetComponent<RectTransform>();
                rectTransform.localScale = new Vector2(.12f, .12f);
                rectTransform.sizeDelta = new Vector2(3240, 2240);
                rectTransform.localPosition = new Vector2(45, -45);
                valueTxt.raycastTarget = false;
                valueTxt.color = Color.white;
                valueTxt.fontStyle = FontStyle.Bold;
                valueTxt.alignment = TextAnchor.MiddleCenter;
                valueTxt.fontSize = 300;
                valueTxt.font = arial;
                valueTxt.text = Inventory.myItems[i].itemValue.ToString();
            }
            i++;
        }
    }
    void Update()
    {
        if (Input.GetAxis("Mouse ScrollWheel") > 0)
        {
            if (currPosX != startPosX)
            {
                currPosX -= space;
            }
            else
            {
                currPosX = startPosX + space * 8;
            }
        }
        if (Input.GetAxis("Mouse ScrollWheel") < 0)
        {
            if (currPosX != startPosX + space * 8)
            {
                currPosX += space;
            }
            else
            {
                currPosX = startPosX;
            }
        }
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            currPosX = startPosX; //#0
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            currPosX = startPosX + space; //#1
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            currPosX = startPosX + space * 2; //#2
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            currPosX = startPosX + space * 3; //#3

        }
        else if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            currPosX = startPosX + space * 4; //#4
        }
        else if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            currPosX = startPosX + space * 5; //#5
        }
        else if (Input.GetKeyDown(KeyCode.Alpha7))
        {
            currPosX = startPosX + space * 6; //#6
        }
        else if (Input.GetKeyDown(KeyCode.Alpha8))
        {
            currPosX = startPosX + space * 7; //#7
        }
        else if (Input.GetKeyDown(KeyCode.Alpha8))
        {
            currPosX = startPosX + space * 8; //#7
        }
        if (currPosX != lastPosX)
        {
            lastPosX = currPosX;
            /*
             currPosY = startPosY - space * x  | - startPosY
             currPosY - startPosY = -space * x | :(-space)
             x = (currPosY - startPosY) / (-space)
            */
            invHighlighted.transform.position = new Vector2(currPosX, invHighlighted.transform.position.y);
            invIndex = ((currPosX - startPosX) / (space));
            if (Inventory.myItems.ContainsKey(invIndex))
            {
                selectedBlock = Item.items[Inventory.myItems[invIndex].itemType].blockType;
            }
            else
            {
                selectedBlock = BlockType.Air;
            }
        }
    }
}
