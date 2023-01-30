using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetUpStaticReference : MonoBehaviour
{
   public  StaticReference reference;
    // Start is called before the first frame update
    void Start()
    {
        reference.target = this.gameObject;
    }

   
}
