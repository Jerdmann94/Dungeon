using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class EnemyAttackController : NetworkBehaviour
{
    
    public LayerMask playerLayer;
    public LayerMask wallLayer;
    private List<EnemyAttack> attackOptions;
    private List<AttackGameInfo> attackInfo;
    

    private float GCD;
    private PlayerController playerController;
    private GameObject target;

    private float timer;


    private bool isInited;
    
    public GameObject Target
    {
        get => target;
        set
        {
            target = value;
        }
    }


    public void Init(List<EnemyAttack> datas)
    {
        attackInfo = new List<AttackGameInfo>();
        isInited = true;
        attackOptions = datas;
        foreach (var VARIABLE in attackOptions)
        {
            attackInfo.Add(new AttackGameInfo(VARIABLE.coolDown));
        }

    }
    
    private void Update()
    {
        if (!isInited) return;
        if (!NetworkManager.Singleton.IsServer) return;
        if (!GameMaster.gameStartStatic) return;
        if (Target == null) return;
        foreach (var info in attackInfo)
        {
            info.IncrementTimer();
        }
        //MANAGING THE GLOBAL COOL DOWN OF ALL ATTACKS HERE
        if (GCD < 2f)
        {
            GCD += Time.deltaTime;
            return;
        }
        //DOING INTERNAL CHECKING FOR EACH ATTACK. IF AN ATTACK HAS LESS 
        // THAN 2 SECOND CD IT WILL ALWAYS BE READY AFTER THE GCD
        for (int i = 0; i < attackOptions.Count; i++)
        {
            if (!attackInfo[i].CheckCoolDown()) continue;
            if (!attackOptions[i].CheckRangeForAttack
                    (Target, transform.position, playerLayer,wallLayer)) continue;
            attackOptions[i].DoAttack(Target,transform);
            GCD = 0;
            return;
        }

        
    }
}

public class AttackGameInfo
{
   
    private readonly float startingTimer;
    private float currentTimer;

    public void IncrementTimer()
    {
        currentTimer += Time.deltaTime;
    }
    public AttackGameInfo(float sTimer)
    {
        startingTimer = sTimer;
    }

    public bool CheckCoolDown()
    {
        if ((currentTimer < startingTimer)) return false;
        currentTimer = 0;
        return true;
    }
}
