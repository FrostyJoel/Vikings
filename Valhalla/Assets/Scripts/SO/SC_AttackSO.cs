using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="New_Attack",menuName ="Attacks")]
public class SC_AttackSO : ScriptableObject
{
    public float damageAmount;
    public GameObject visualPrefab;
    public float duration;
    public float delay;
}
