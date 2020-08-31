using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SC_Attacks : MonoBehaviour
{
    SC_AttackManager attackMan;
    [Header("LightnigCircleAttack")]
    [Range(2f, 10f)]
    [SerializeField] float damageOverTimeAmount = 3f;
    [SerializeField] float duration = 2f;
    public float delay = 1.5f;
    public GameObject lightningCircleVisual;
    [Header("HammerAttack")]
    [Range(5f, 20f)]
    [SerializeField] float hammerDamageAmount;
    [SerializeField] float maxForce;
    public GameObject hammer;
    private float forceAmount = 20f;

    private void Awake()
    {
        attackMan = FindObjectOfType<SC_AttackManager>();
    }

    public void LightningCircle()
    {
        GameObject lightningCircle = Instantiate(lightningCircleVisual, attackMan.attackPos, lightningCircleVisual.transform.rotation);
        //Todo Add Fading Effect to Particle
        Destroy(lightningCircle, duration);
        
    }

    public void HammerThrow()
    {
        Rigidbody hammerRB = hammer.GetComponent<Rigidbody>();
        hammerRB.isKinematic = false;
        hammerRB.transform.parent = null;
        hammerRB.AddForce(attackMan.player.transform.forward * forceAmount, ForceMode.Impulse);
       
    }

    public void HammerReturn()
    {
        attackMan.isAttacking = false;
    }
}
