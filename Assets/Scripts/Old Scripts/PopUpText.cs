using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using DG.Tweening;
using TMPro;
using Unity.Mathematics;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using Cache = UnityEngine.Cache;

public class PopUpText : NetworkBehaviour
{
    private static GameObject _textPrefab;
    public TMP_Text textMeshPro;
    private static GameObject _canvas;
    private static readonly float TextDelay = 6f;

    private NetworkVariable<int> test = new NetworkVariable<int>();
    

    private void Awake()
    {
        textMeshPro = GetComponent<TextMeshPro>();
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if (textMeshPro != null)
        {
            textMeshPro.text = test.Value.ToString();
        }
        
    }


    public void Setup(int amount)
    {
        test.Value = amount;
        textMeshPro.text = test.Value.ToString();
        Debug.Log(test.Value);

    }

    

    /*[ClientRpc]
    private static void SpawnTextClientRpc(Vector3 position, int amount,Color color)
    {
        if (_textPrefab == null)
        {
            _textPrefab = Resources.Load<GameObject>("Text/textPrefab");
        }
        var obj = Instantiate(_textPrefab, position, quaternion.identity);
        obj.GetComponent<NetworkObject>().Spawn();
        var popUp = obj.GetComponent<PopUpText>();
        popUp.Setup(amount);
        popUp.textMeshPro.color = color;
        var rect = popUp.GetComponent<RectTransform>();
        obj.GetComponent<RectTransform>().DOAnchorPos(rect.anchoredPosition +Vector2.up, .1f).SetEase(Ease.OutFlash);
        Destroy(popUp.gameObject,.5f);
    }*/
    
    
}
