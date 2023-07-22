using Unity.Netcode;
using UnityEngine;
using UnityEngine.EventSystems;

public class DragDropAttackItem : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    public Texture2D texture;
    public GameItem currentRune;

    public StaticReference attackManager;

    //public GameObject attackObjectPrefab;
    private RectTransform rectTransform;

    // Start is called before the first frame update
    private void Start()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        //Debug.Log("begin drag");
        Cursor.SetCursor(texture, Vector2.zero, CursorMode.Auto);
    }

    public void OnDrag(PointerEventData eventData)
    {
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);

        var x = attackManager.Target.GetComponent<AttackManager>();


        //RAYCAST TO DRAG POINT
        attackManager.Target.GetComponent<AttackManager>()
            .SpawnAttackItemServerRpc(Camera.main.ScreenToWorldPoint(Input.mousePosition), currentRune.name);
        //Debug.Log("end drag");
    }

    public void OnPointerDown(PointerEventData eventData)
    {
    }

    [ServerRpc(RequireOwnership = false)]
    private void SpawnAttackItemServerRpc(Vector3 worldPosition, ServerRpcParams rpcParams = default)
    {
        /*Debug.Log("Doing Attack");
        var clientId = rpcParams.Receive.SenderClientId;
        if (!NetworkManager.ConnectedClients.ContainsKey(clientId)) return;
        var client = NetworkManager.ConnectedClients[clientId];
        var cPos = client.PlayerObject.transform.position;
        
        // CHECK IF LOCATION IS CLOSE ENOUGH TO PLAYER
        var distance = worldPosition - cPos;
        var direction = distance/distance.magnitude;
        
        //Debug.Log("raycasting");
        float distF = Vector3.Distance(new Vector3(worldPosition.x,worldPosition.y,0), cPos);
        RaycastHit2D hit = Physics2D.Raycast(cPos, direction, distF, 1);
        if (hit)
        {
            //Debug.Log(hit.collider.gameObject);
            return;
        }
        //Debug.Log("we hit not a wall");
        // SPAWN AN ATTACK OBJECT

        var spawnLoc = new Vector3Int(Mathf.RoundToInt(worldPosition.x), Mathf.RoundToInt(worldPosition.y), 1);
        var attackObject = Instantiate(attackObjectPrefab, spawnLoc,
            quaternion.identity);
        attackObject.GetComponent<NetworkObject>().Spawn();*/
    }
}