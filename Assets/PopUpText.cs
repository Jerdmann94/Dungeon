using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using DG.Tweening;
using TMPro;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using Cache = UnityEngine.Cache;

public class PopUpText : MonoBehaviour
{
    private static GameObject _textPrefab;
    private TMP_Text textMeshPro;
    private static GameObject _canvas;
    private static readonly float TextDelay = 6f;

    private void Awake()
    {
        textMeshPro = GetComponent<TextMeshPro>();
    }

    private void Setup(int amount)
    {
        textMeshPro.text = amount.ToString();
    }

    public static PopUpText CreatePopUp(Vector3 position, int amount,Color color)
    {
        if (_textPrefab == null)
        {
            _textPrefab = Resources.Load<GameObject>("Text/textPrefab");
            Debug.Log("Loading Text Resource");
        }
        _canvas = GameObject.FindWithTag("Canvas");
        var obj = Instantiate(_textPrefab, position, quaternion.identity);
        obj.transform.SetParent(_canvas.transform);
        
        var popUp = obj.GetComponent<PopUpText>();
        //Debug.Log(popUp);
        popUp.Setup(amount);
        popUp.textMeshPro.color = color;
        //MoveText(obj.GetComponent<RectTransform>());
        var rect = popUp.GetComponent<RectTransform>();
        obj.GetComponent<RectTransform>().DOAnchorPos(rect.anchoredPosition +Vector2.up, .1f).SetEase(Ease.OutFlash);
        Destroy(popUp.gameObject,.5f);
        return popUp;
    }
    
}
