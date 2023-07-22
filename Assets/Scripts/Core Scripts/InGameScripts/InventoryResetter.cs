using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryResetter : MonoBehaviour
{
    public StaticReference uiDump;
    public GameObject uiPrefab;
    public OnDropType myType;


    public void DoReset(List<GameItem> items)
    {
        var children = transform.GetComponentsInChildren<DragAndDropLoot>();
        for (var index = 0; index < items.Count; index++)
        {
            GameObject go = null;
            DragAndDropLoot dd = null;
            if (children.Length >= index)
            {
                go = SpawnUIITem();
                dd = go.GetComponent<DragAndDropLoot>();
            }
            else
            {
                go = children[index].gameObject;
                dd = children[index];
            }


            var gi = items[index];
            go.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = gi.amountInThisStack.ToString();
            go.transform.GetChild(1).GetComponent<Image>().sprite = SpriteUtil.GetSprite(gi.sprite);
            //DOING SPRITES SOME OTHER WAY, WITH STRINGS TO FIND THEM
            dd.item = gi;
            dd.SetParentTransform(transform, myType);
        }

        children = transform.GetComponentsInChildren<DragAndDropLoot>();
        if (children.Length <= items.Count) return;
        for (var i = 0; i < items.Count; i++)
            children[i].SetParentTransform(uiDump.Target.transform, OnDropType.Nulling);
    }

    private GameObject SpawnUIITem()
    {
        GameObject go = null;
        go = uiDump.Target.transform.childCount == 0
            ? Instantiate(uiPrefab)
            : uiDump.Target.transform.GetChild(0).gameObject;
        return go;
    }
}