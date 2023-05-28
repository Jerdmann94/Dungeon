using DG.Tweening;
using Unity.Mathematics;
using Unity.Netcode;
using UnityEngine;

public class TextManager : NetworkBehaviour
{
    public GameObject _textPrefab;

    public StaticReference textRefernce;

    // Start is called before the first frame update
    private void Start()
    {
        textRefernce.target = gameObject;
    }

    public void testMethod()
    {
        // Debug.Log("test method");
    }

    [ServerRpc(RequireOwnership = false)]
    public void CreatePopUpServerRpc(Vector3 position, int amount, Color color)
    {
        Debug.Log("inside creating pop up");
        if (_textPrefab == null) _textPrefab = Resources.Load<GameObject>("Text/textPrefab");
        var obj = Instantiate(_textPrefab, position, quaternion.identity);

        var popUp = obj.GetComponent<PopUpText>();
        popUp.Setup(amount);
        //popUp.textMeshPro.color = color;
        var rect = popUp.GetComponent<RectTransform>();
        obj.GetComponent<RectTransform>().DOAnchorPos(rect.anchoredPosition + Vector2.up, .1f).SetEase(Ease.OutFlash);
        //Destroy(popUp.gameObject,.5f);
        obj.GetComponent<NetworkObject>().Spawn();
    }

    public void CreatePopUp(Vector3 position, int amount, Color color)
    {
        if (_textPrefab == null) _textPrefab = Resources.Load<GameObject>("Text/textPrefab");
        var obj = Instantiate(_textPrefab, position, quaternion.identity);

        var popUp = obj.GetComponent<PopUpText>();
        popUp.Setup(amount);
        //popUp.textMeshPro.color = color;
        var rect = popUp.GetComponent<RectTransform>();
        obj.GetComponent<RectTransform>().DOAnchorPos(rect.anchoredPosition + Vector2.up, .1f).SetEase(Ease.OutFlash);
        Destroy(popUp.gameObject, .5f);
        obj.GetComponent<NetworkObject>().Spawn();
    }
}