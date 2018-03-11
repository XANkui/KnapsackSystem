using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System;

public class KnapsackManager : MonoBehaviour
{
    private static KnapsackManager _instance;
    public static KnapsackManager Instance { get { return _instance; } }

    public GridPanelUI GridPanelUI;
    public TooltipUI TooltipUI;
    public DragItemUI DragItemUI;

    private bool isDrag = false;
    private bool isShow = false;

    public Dictionary<int, Item> ItemList;

    void Awake()
    {
        //单例
        _instance = this;
        //数据
        Load();
        //事件
        GridUI.OnEnter += GridUI_OnEnter;
        GridUI.OnExit += GridUI_OnExit;
        GridUI.OnLeftBeginDrag += GridUI_OnLeftBeginDrag;
        GridUI.OnLeftEndDrag += GridUI_OnLeftEndDrag;
    }

    void Update()
    {
        Vector2 position;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(GameObject.Find("KnapsackUI").transform as RectTransform, Input.mousePosition, null, out position);

        if (isDrag)
        {
            DragItemUI.Show();
            DragItemUI.SetLocalPosition(position);
        }
        else if (isShow)
        {
            TooltipUI.Show();
            TooltipUI.SetLocalPosition(position);
        }
    }

    public void StoreItem(int itemId)
    {
        if (!ItemList.ContainsKey(itemId))
            return;

        Transform emptyGrid = GridPanelUI.GetEmptyGrid();
        if (emptyGrid == null)
        {
            Debug.LogWarning("背包已满!!");
            return;
        }

        Item temp = ItemList[itemId];
        this.CreatNewItem(temp, emptyGrid);
    }

    private void Load()
    {
        ItemList = new Dictionary<int, Item>();

        Weapon w1 = new Weapon(0, "牛刀", "牛B的刀！", 20, 10, "", 100);
        Weapon w2 = new Weapon(1, "羊刀", "杀羊刀。", 15, 10, "", 20);
        Weapon w3 = new Weapon(2, "宝剑", "大宝剑！", 120, 50, "", 500);
        Weapon w4 = new Weapon(3, "军枪", "可以对敌人射击，很厉害的一把枪。", 1500, 125, "", 720);

        Consumable c1 = new Consumable(4, "红瓶", "加血", 25, 11, "", 20, 0);
        Consumable c2 = new Consumable(5, "蓝瓶", "加蓝", 39, 19, "", 0, 20);

        Armor a1 = new Armor(6, "头盔", "保护脑袋！", 128, 83, "", 5, 40, 1);
        Armor a2 = new Armor(7, "护肩", "上古护肩，锈迹斑斑。", 1000, 0, "", 15, 40, 11);
        Armor a3 = new Armor(8, "胸甲", "皇上御赐胸甲。", 153, 0, "", 25, 30, 11);
        Armor a4 = new Armor(9, "护腿", "预防风寒，从腿做起", 999, 60, "", 19, 30, 51);

        ItemList.Add(w1.Id, w1);
        ItemList.Add(w2.Id, w2);
        ItemList.Add(w3.Id, w3);
        ItemList.Add(w4.Id, w4);
        ItemList.Add(c1.Id, c1);
        ItemList.Add(c2.Id, c2);
        ItemList.Add(a1.Id, a1);
        ItemList.Add(a2.Id, a2);
        ItemList.Add(a3.Id, a3);
        ItemList.Add(a4.Id, a4);

    }

    #region 事件回调
    private void GridUI_OnEnter(Transform girdTransform)
    {
        Item item = ItemModel.GetItem(girdTransform.name);
        if (item == null)
            return;
        string text = GetTooltipText(item);
        TooltipUI.UpdateTooltip(text);
        isShow = true;
    }

    private void GridUI_OnExit()
    {
        isShow = false;
        TooltipUI.Hide();
    }

    private void GridUI_OnLeftBeginDrag(Transform girdTransform)
    {
        if (girdTransform.childCount == 0)
            return;
        else
        {
            Item item = ItemModel.GetItem(girdTransform.name);
            DragItemUI.UpdateItem(item.Name);
            Destroy(girdTransform.GetChild(0).gameObject);
            isDrag = true;
        }
    }

    private void GridUI_OnLeftEndDrag(Transform prevTransform, Transform enterTransform)
    {
        isDrag = false;
        DragItemUI.Hide();

        if (enterTransform == null)//扔东西
        {
            ItemModel.DeleteItem(prevTransform.name);
            Debug.LogWarning("物品已扔");
        }
        else if (enterTransform.tag == "Grid")//拖到另一个或者当前格子里
        {
            if (enterTransform.childCount == 0)//直接扔进去
            {
                Item item = ItemModel.GetItem(prevTransform.name);
                this.CreatNewItem(item, enterTransform);
                ItemModel.DeleteItem(prevTransform.name);
            }
            else //交换
            {
                //删除原来的物品
                Destroy(enterTransform.GetChild(0).gameObject);
                //获取数据
                Item prevGirdItem = ItemModel.GetItem(prevTransform.name);
                Item enterGirdItem = ItemModel.GetItem(enterTransform.name);
                //交换的两个物体
                this.CreatNewItem(prevGirdItem, enterTransform);
                this.CreatNewItem(enterGirdItem, prevTransform);
            }
        }
        else//拖到UI的其他地方,让他还原
        {
            Item item = ItemModel.GetItem(prevTransform.name);
            this.CreatNewItem(item, prevTransform);
        }

    }

    #endregion

    private string GetTooltipText(Item item)
    {
        if (item == null)
            return "";
        StringBuilder sb = new StringBuilder();
        sb.AppendFormat("<color=red>{0}</color>\n\n", item.Name);
        switch (item.ItemType)
        {
            case "Armor":
                Armor armor = item as Armor;
                sb.AppendFormat("力量:{0}\n防御:{1}\n敏捷:{2}\n\n", armor.Power, armor.Defend, armor.Agility);
                break;
            case "Consumable":
                Consumable consumable = item as Consumable;
                sb.AppendFormat("HP:{0}\nMP:{1}\n\n", consumable.BackHp, consumable.BackMp);
                break;
            case "Weapon":
                Weapon weapon = item as Weapon;
                sb.AppendFormat("攻击:{0}\n\n", weapon.Damage);
                break;
            default:
                break;
        }
        sb.AppendFormat("<size=25><color=white>购买价格：{0}\n出售价格：{1}</color></size>\n\n<color=yellow><size=20>描述：{2}</size></color>", item.BuyPrice, item.SellPrice, item.Description);
        return sb.ToString();
    }

    private void CreatNewItem(Item item, Transform parent)
    {
        GameObject itemPrefab = Resources.Load<GameObject>("Prefabs/Item");
        itemPrefab.GetComponent<ItemUI>().UpdateItem(item.Name);
        GameObject itemGo = GameObject.Instantiate(itemPrefab);
        itemGo.transform.SetParent(parent);
        itemGo.transform.localPosition = Vector3.zero;
        itemGo.transform.localScale = Vector3.one;
        //存储数据
        ItemModel.StoreItem(parent.name, item);
    }

}
