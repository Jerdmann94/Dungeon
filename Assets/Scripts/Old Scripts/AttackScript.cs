using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackScript : MonoBehaviour
{
    public AttackData attackData;
    public int damage;
    public List<GameObject> hitList = new List<GameObject>();
    public GameObject losSpawn; 

    private void Start()
    {
        damage = attackData.damage;
    }

    public void CheckForLos()
    {
        var transform1 = transform;
        var transform2 = transform1.parent.transform;
        var position = transform1.position;
        var position2 = transform2.position;
        var distance = position - position2;
        var direction = distance/distance.magnitude;
       
        float distF = Vector3.Distance(new Vector3(position.x,position.y,0), position2);
        RaycastHit2D hit = Physics2D.Raycast(position2, direction,.9f, 1);
        if (hit)
        {
           //Instantiate(losSpawn, position, Quaternion.identity);
            //Debug.Log(hit.collider.gameObject.name+ "   " + distF);
           gameObject.SetActive(false);
            
        }
        else
        {
            Debug.Log(" raycast did not hit");
        }
    }
    
}
