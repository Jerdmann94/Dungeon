using UnityEngine;

public class SetUpStaticReference : MonoBehaviour
{
    public StaticReference reference;

    // Start is called before the first frame update
    private void Start()
    {
        reference.Target = gameObject;
    }
}