using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class MyItemValues
{
    public Item.ItemType itemType { get; set; }
    public int itemValue { get; set; }
}
public class Inventory : MonoBehaviour
{
    public GameObject slotsParent;
    public GameObject slotsParentShortcut;
    public int stackValue = 64;
    Font arial;
    //_________________________
    //DRAG
    //REAL OBJECT FOLLOW MOUSE
    private GameObject dragItem;
    //CLONE STAY ON OLD SLOT
    private GameObject ghostItem;
    //CLONE STAY ON OLD SLOT
    private GameObject splitItem;
    //SPLIT VALUE FROM ITEM THAT FOLLOWS MOUSE
    private int splitValue1, splitValue2;
    private bool drag = false;
    //_________________________
    private List<GameObject> slots = new List<GameObject>();
    //0 - 8 => SHORTCUT SLOTS
    public static Dictionary<int, MyItemValues> myItems = new Dictionary<int, MyItemValues>(){
     {0, new MyItemValues{itemType = Item.ItemType.Wood, itemValue = 10}},
     {1, new MyItemValues{ itemType = Item.ItemType.Stone, itemValue = 26}},
     {2, new MyItemValues{ itemType = Item.ItemType.Stone, itemValue = 26}},
     {3, new MyItemValues{ itemType = Item.ItemType.Grass, itemValue = 4}},
     {20, new MyItemValues{ itemType = Item.ItemType.Wood, itemValue = 60}},
    };
    void Start()
    {
        arial = (Font)Resources.GetBuiltinResource(typeof(Font), "Arial.ttf");

        GetSlots();
    }
    void GetSlots()
    {
        //SHORTCUT SLOTS
        for (int i = 0; i < slotsParentShortcut.transform.childCount; i++)
        {
            slots.Add(slotsParentShortcut.transform.GetChild(i).gameObject);
            Button slotBtn = slots[i].AddComponent<Button>();
            slotBtn.onClick.AddListener(() => OnSlotClick(slotBtn));
            //SLOT COLOR
            ColorBlock colors = slotBtn.colors;
            slotBtn.colors = colors;
        }
        int slotOffset = 9;
        //NORMAL SLOTS
        for (int i = 0; i < slotsParent.transform.childCount; i++)
        {
            slots.Add(slotsParent.transform.GetChild(i).gameObject);
            Button slotBtn = slots[slotOffset].AddComponent<Button>();
            slotOffset++;
            slotBtn.onClick.AddListener(() => OnSlotClick(slotBtn));
            //SLOT COLOR
            ColorBlock colors = slotBtn.colors;
            slotBtn.colors = colors;
        }
        SetItems();
    }
    void OnSlotClick(Button slotBtn)
    {
        if (Input.GetKey(KeyCode.LeftShift))
        {
            //CHECK IF IT CAN BE SPLITED
            if (slotBtn.transform.childCount != 0 && !drag)
            {
                bool isSplitable = myItems[slots.IndexOf(slotBtn.gameObject)].itemValue > 1;
                if (!isSplitable) return;
                SplitItems(slotBtn);
            }
            else
            {
                DragAndDrop(slotBtn);
            }
        }
        else
        {
            DragAndDrop(slotBtn);
        }
    }
    void UpdateItems()
    {
        //DESTROY ALL SLOT CHILDS
        foreach (KeyValuePair<int, MyItemValues> myItem in myItems)
        {
            Destroy(slots[myItem.Key].transform.GetChild(0).gameObject);
        }
        //SET ALL SLOTS WITH VALUE
        SetItems();
    }
    void SetItems()
    {
        foreach (KeyValuePair<int, MyItemValues> myItem in myItems)
        {
            Item.ItemType itemType_ = myItem.Value.itemType;
            int itemValue_ = myItem.Value.itemValue;
            GameObject itemChild = new GameObject();
            itemChild.name = "Item => " + itemType_.ToString();
            itemChild.transform.SetParent(slots[myItem.Key].transform);
            Image itemSprite = itemChild.AddComponent<Image>();
            itemSprite.sprite = Item.items[itemType_].itemSprite;
            itemSprite.raycastTarget = false;
            RectTransform rect = itemChild.GetComponent<RectTransform>();
            rect.localPosition = Vector2.zero;
            rect.localScale = new Vector2(.5f, .47f);
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
            valueTxt.text = itemValue_.ToString();

            //  Debug.Log(myItem.Key + " " + itemType_.ToString() + " " + itemValue_.ToString());
        }
        Debug.Log("INVENTORY UPDATED");
        ToolBar.UpdateToolBarItems();
    }
    void DragAndDrop(Button slotBtn)
    {
        //DRAG
        if (slotBtn.transform.childCount != 0 && !drag)
        {
            DragItem(slotBtn);
        }
        //DROP
        else if (slotBtn.transform.childCount == 0 && drag)
        {
            DropItem(slotBtn);
        }
        //STACK / DROP BACK
        else if (slotBtn.transform.childCount > 0 && drag)
        {
            bool isSameSlot = slotBtn == dragItem.GetComponentInParent<Button>();
            if (isSameSlot) { SetSlot(); return; }
            //CHECK ITEM TYPE --> SAME SPRITE NAME
            bool isSameSprite = slotBtn.transform.GetChild(0).gameObject.transform.GetComponent<Image>().sprite == dragItem.GetComponent<Image>().sprite;
            if (isSameSprite)
            {   //STACK
                StackItems(slotBtn);
            }
            else
            {
                //DROP BACK
                SetSlot(splitItem != null);
            }
        }
    }
    void DragItem(Button slotBtn, bool isSplited = false)
    {
        GameObject cloneItem;
        //SET GHOST ITEM
        cloneItem = Instantiate<GameObject>(slotBtn.transform.GetChild(0).gameObject);
        cloneItem.transform.SetParent(slotBtn.transform);
        Image itemSprite = cloneItem.GetComponent<Image>();
        itemSprite.raycastTarget = false;
        RectTransform rect = cloneItem.GetComponent<RectTransform>();
        rect.localPosition = Vector2.zero;
        rect.localScale = new Vector2(.5f, .47f);
        if (!isSplited)
        {
            cloneItem.name = "ItemGhost => " + slotBtn.transform.GetChild(0).gameObject.name;
            itemSprite.color = new Color(1f, 1f, 1f, .8f);
            cloneItem.transform.GetChild(0).GetComponent<Text>().color = new Color(1f, 1f, 1f, .5f);
            ghostItem = cloneItem;
        }
        else
        {
            cloneItem.name = slotBtn.transform.GetChild(0).gameObject.name;
            Text cloneText = cloneItem.transform.GetChild(0).GetComponent<Text>();
            cloneText.color = new Color(1f, 1f, 1f, 1f);
            cloneText.text = splitValue1.ToString();
            splitItem = cloneItem;
        }
        //SET TO TOP LAYER
        slotBtn.GetComponent<RectTransform>().SetAsLastSibling();
        dragItem = slotBtn.transform.GetChild(0).gameObject;
        drag = true;
    }
    void DropItem(Button slotBtn)
    {
        int slotIndex1 = slots.IndexOf(dragItem.transform.GetComponentInParent<Button>().gameObject);
        int slotIndex2 = slots.IndexOf(slotBtn.gameObject);
        //CHECK IF I DROP THE SPLIT ITEM
        if (splitItem == null)
        {
            myItems[slotIndex2] = new MyItemValues { itemType = myItems[slotIndex1].itemType, itemValue = myItems[slotIndex1].itemValue };
            myItems.Remove(slotIndex1);
        }
        else
        {
            myItems[slotIndex2] = new MyItemValues { itemType = myItems[slotIndex1].itemType, itemValue = splitValue2 };
            splitItem = null;
        }
        dragItem.transform.SetParent(slotBtn.transform);
        SetSlot();
    }
    void StackItems(Button slotBtn)
    {
        GameObject usedItem = dragItem;
        if (splitItem != null)
        {
            usedItem = splitItem;
        }
        //REMOVE OLD SLOT FROM DIC AND ADD VALUE TO NEW SLOT
        int slotIndex1 = slots.IndexOf(usedItem.transform.GetComponentInParent<Button>().gameObject);
        int slotIndex2 = slots.IndexOf(slotBtn.gameObject);
        //OLD SLOT
        int itemValue1 = myItems[slotIndex1].itemValue;
        //NEW SLOT
        int itemValue2 = myItems[slotIndex2].itemValue;
        int changedValue = 0;
        //OVERWRITE NEW SLOT
        if (splitItem == null)
        {
            if (itemValue1 + itemValue2 > stackValue)
            {
                changedValue = (itemValue1 + itemValue2) - stackValue;
                myItems[slotIndex2].itemValue = stackValue;
                myItems[slotIndex1].itemValue = changedValue;
                Destroy(ghostItem);
            }
            else
            {
                myItems[slotIndex2].itemValue += itemValue1;
                myItems.Remove(slotIndex1);
                //DELETE OLD SLOT INFO
                Destroy(usedItem);
                Destroy(ghostItem);
            }
        }
        else
        {
            print(itemValue1);
            print(splitValue2);
            if (itemValue2 + splitValue2 > stackValue)
            {
                changedValue = ((itemValue2 + splitValue2) - stackValue) + splitValue1;
                print(splitValue1);
                print(splitValue2);
                //OVERWRITE OLD SLOT
                myItems[slotIndex1] = new MyItemValues { itemType = myItems[slotIndex2].itemType, itemValue = changedValue };
                myItems[slotIndex2].itemValue = stackValue;
                Destroy(dragItem);
                Destroy(splitItem);
            }
            else
            {
                myItems[slotIndex2].itemValue += itemValue1;
                Destroy(splitItem);
            }
        }
        drag = false;
        UpdateItems();
    }
    void SplitItems(Button slotBtn)
    {
        int slotIndex1 = slots.IndexOf(slotBtn.gameObject);
        //VALUE INSIDE SLOT
        splitValue1 = Mathf.FloorToInt(myItems[slotIndex1].itemValue / 2f);
        //VALUE DRAG
        splitValue2 = Mathf.CeilToInt(myItems[slotIndex1].itemValue / 2f);
        myItems[slotIndex1].itemValue = splitValue1;
        //DRAG ITEM
        slotBtn.gameObject.transform.GetChild(0).GetChild(0).GetComponent<Text>().text = splitValue2.ToString();
        DragItem(slotBtn, true);
    }
    void SetSlot(bool isSplitedAndDropBack = false)
    {
        drag = false;
        if (!isSplitedAndDropBack)
        {
            dragItem.transform.GetComponent<RectTransform>().localPosition = Vector2.zero;
            Destroy(ghostItem);
        }
        else
        {
            //DROP SPLIT ITEM BACK
            int slotIndex1 = slots.IndexOf(splitItem.GetComponentInParent<Button>().gameObject);
            myItems[slotIndex1].itemValue += splitValue2;
            Destroy(dragItem);
            Destroy(splitItem);
        }
        UpdateItems();
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            print(splitValue1);
            print(splitValue2);
            UpdateItems();
        }
        //DRAG
        if (drag)
        {
            dragItem.transform.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        }

    }
}
