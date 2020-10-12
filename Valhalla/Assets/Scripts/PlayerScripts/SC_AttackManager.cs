using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SC_AttackManager : MonoBehaviour
{
    public static SC_AttackManager single;

    public float maxLightningCooldown;

    public bool isAttacking;
    [SerializeField] float hammerAnimationTimeDelay;

    [HideInInspector]
    public Vector3 attackPos;
    public float currLightningCooldown;
    private bool checkIfPressed = false;

    private void Awake()
    {
        single = this;
    }
    private void Update()
    {
        if (SC_CharacterAnimation.single == null){ return; }
        ButtonSelect();
        if(currLightningCooldown > 0)
        {
            currLightningCooldown -= Time.deltaTime;
        }
    }

    public void ButtonSelect()
    {
        if (SC_CharacterAnimation.single.anime.GetCurrentAnimatorStateInfo(0).IsTag("Attack")) { return; }
        if (isAttacking) { return; }
        if (!SC_UiManager.single.getAttackInput) { return; }

        if (Input.GetButtonDown("Fire1"))
        {
            if (SC_Attacks.single.inHand == true)
            {
                StartCoroutine(CheckButtonHold());
                return;
            }
            else
            {
                if (SC_Attacks.single.hitObject == true)
                {
                    isAttacking = true;
                    SC_CharacterAnimation.single.PullBack();
                }
                return;
            }
        }

        if (Input.GetButton("Fire1"))
        {
            if (SC_Attacks.single.inHand == true && checkIfPressed == false)
            {
                StartCoroutine(CheckButtonHold());
                return;
            }
        }

        if (Input.GetButtonUp("Fire1") && SC_Attacks.single.inHand == true)
        {
            checkIfPressed = false;
            isAttacking = true;
            attackPos = new Vector3(SC_TopDownController.single.GetAimTargetPos().x, SC_TopDownController.single.GetAimTargetPos().y + 0.2f, SC_TopDownController.single.GetAimTargetPos().z);
            Animator hammerAnime = SC_Attacks.single.hammerRB.GetComponent<Animator>();
            if (hammerAnime.enabled == true)
            {
                SC_Attacks.single.hammerRB.transform.localPosition = Vector3.zero;
                SC_Attacks.single.hammerRB.transform.localRotation = Quaternion.identity;
                hammerAnime.SetFloat("SpeedIncreasing", 1.0f);
                hammerAnime.SetTrigger("Reset");
                hammerAnime.enabled = false;
                SC_CharacterAnimation.single.HammerThrow();
            }
            else
            {
                SC_CharacterAnimation.single.MeleeAttack();
                SC_Attacks.single.hammerRB.GetComponent<SC_HammerStats>().melee = true;
            }
        }

        if (Input.GetButtonDown("Fire2"))
        {
            if (SC_Attacks.single.inHand)
            {
                if (!IsInvoking() && currLightningCooldown <= 0)
                {
                    isAttacking = true;
                    SC_CharacterAnimation.single.LightningAttack();

                    currLightningCooldown = maxLightningCooldown;

                    attackPos = new Vector3(SC_TopDownController.single.GetAimTargetPos().x, 
                        SC_TopDownController.single.GetAimTargetPos().y + 0.2f, 
                        SC_TopDownController.single.GetAimTargetPos().z);

                }
            }
            else { return; }
        }
    }

    IEnumerator CheckButtonHold()
    {
        checkIfPressed = true;
        float holdTimer = 0.0f;
        while (Input.GetButton("Fire1"))
        {
            if(holdTimer >= 0.5f)
            {
                StartCoroutine(HammerCharge());
                yield break;
            }
            holdTimer += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
    }
    IEnumerator HammerCharge()
    {
        float forceAdd = 0.5f;
        float forceDelay = 0.5f;
        float speedIncrease = 1.0f;
        Animator hammerAnimator = SC_Attacks.single.hammerRB.GetComponent<Animator>();
        hammerAnimator.enabled = true;
        yield return new WaitForSeconds(hammerAnimationTimeDelay);
        while (Input.GetButton("Fire1"))
        {
            SC_Attacks.single.forceAmount += forceAdd;
            if(forceAdd <= 7.5f)
            {
                forceAdd *= 2f;
            }
            if(SC_Attacks.single.forceAmount <= SC_Attacks.single.maxForce)
            {
                if (SC_Attacks.single.hammerDamageAmount <= SC_Attacks.single.maxhammerDamageAmount)
                {
                    SC_Attacks.single.hammerDamageAmount *= 2;
                }
                else
                {
                    SC_Attacks.single.hammerDamageAmount = SC_Attacks.single.maxhammerDamageAmount;
                }
                SC_Attacks.single.hammerRB.GetComponent<SC_HammerStats>().myHammerAnimation.SetFloat("SpeedIncreasing", speedIncrease);
                speedIncrease++;
                yield return new WaitForSeconds(forceDelay/2f);
            }
            else
            {
                SC_Attacks.single.forceAmount = SC_Attacks.single.maxForce;
                yield break;
            }
        }
    }
}
