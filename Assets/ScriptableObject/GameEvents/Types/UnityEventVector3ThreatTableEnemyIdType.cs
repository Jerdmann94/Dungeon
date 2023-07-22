using System;
using System.Collections;
using System.Collections.Generic;
using Core_Scripts.EnemyScripts;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class UnityEventVector3ThreatTableEnemyIdType : UnityEvent<Vector3,List<PlayerThreat>,string,bool> { }

