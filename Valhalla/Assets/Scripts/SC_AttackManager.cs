using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SC_AttackManager : MonoBehaviour
{
    public List<SC_AttackSO> attackList = new List<SC_AttackSO>();
    public SC_AttackSO curAttack;
    public SC_TopDownController player;
    public bool isAttacking;
    [HideInInspector]
    public Vector3 attackPos;

    private void Awake()
    {
        player = GetComponent<SC_TopDownController>();
    }
    private void Update()
    {
        ButtonSelect();
    }

    public void ButtonSelect()
    {
        if (attackList.Count <= 0)
        {
            return;
        }
        if (!curAttack)
        {
            curAttack = attackList[0];
        }
        int selectedButton = 0;
        if(Input.GetButtonDown("Fire1"))
        {
            selectedButton = 0;
            curAttack = attackList[selectedButton];
            if (!IsInvoking())
            {
                GetComponent<SC_CharacterAnimation>().Attack1();
                isAttacking = true;
                attackPos = new Vector3(player.GetAimTargetPos().x, player.GetAimTargetPos().y + 0.2f, player.GetAimTargetPos().z);
                Invoke("SummonSelectedAttack", curAttack.delay);
            }
        }
        if (Input.GetButtonDown("Fire2"))
        {
            selectedButton = 1;
            curAttack = attackList[selectedButton];
            if (!IsInvoking())
            {
                GetComponent<SC_CharacterAnimation>().Attack2();
                isAttacking = true;
                attackPos = new Vector3(player.GetAimTargetPos().x, player.GetAimTargetPos().y + 0.2f, player.GetAimTargetPos().z);
                Invoke("SummonSelectedAttack", curAttack.delay);
            }
        }
    }
    

    public void SummonSelectedAttack()
    {
        GameObject actualAttack = Instantiate(curAttack.visualPrefab, attackPos, curAttack.visualPrefab.transform.rotation);
        Destroy(actualAttack, curAttack.visualPrefab.GetComponent<ParticleSystem>().main.duration);
        isAttacking = false;
    }
}
