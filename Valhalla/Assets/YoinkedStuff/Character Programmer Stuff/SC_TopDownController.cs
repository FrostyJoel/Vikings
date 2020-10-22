using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]

public class SC_TopDownController : MonoBehaviour
{
    public static SC_TopDownController single;
    public enum CameraDirection { x, z }

    [Header("Camera Settings")]
    public CameraDirection cameraDirection = CameraDirection.x;
    public float cameraHeight = 20f;
    public float cameraDistance = 7f;
    public float cameraXYOffset = 2;
    public Camera playerCamera;
    public GameObject targetIndicatorPrefab;

    [Header("PlayerMovement Settings")]
    public float speed = 5.0f;
    public float gravity = 14.0f;
    public float maxVelocityChange = 10.0f;
    public float jumpHeight = 2.0f;
    public float turnSpeed = 90;
    public bool canJump = true;
    [SerializeField]bool walking = false;

    [Header("Player Stats")]
    //[Range(20f,60f)]
    public float maxHealth = 20f;
    //[Range(1f,10f)]
    public float invulnerableTimer = 1f;

    public ParticleSystem hitEffect;

    [Header("HideInInspector")]
    public float curHealth = 0.0f;
    bool gotHit = false;
    bool grounded = false;
    public bool canMove = true;
    public bool dead;
    Rigidbody r;
    GameObject targetObject;

    //Mouse cursor Camera offset effect
    Vector2 playerPosOnScreen;
    Vector2 cursorPosition;
    Vector2 offsetVector;

    //Plane that represents imaginary floor that will be used to calculate Aim target position
    Plane surfacePlane = new Plane();

    //Animator and AttackManager
    Vector3 targetVelocity;


    void Awake()
    {
        if (single != null)
        {
            Destroy(this.gameObject);
            return;
        }
        else
        {
            single = this;
        }

        if(playerCamera.transform.parent != null)
        {
            playerCamera.transform.parent = null;
        }
        r = GetComponent<Rigidbody>();
        r.freezeRotation = true;
        r.useGravity = false;
        curHealth = maxHealth;

        //Instantiate aim target prefab
        if (targetIndicatorPrefab)
        {
            targetObject = Instantiate(targetIndicatorPrefab, Vector3.zero, Quaternion.identity) as GameObject;
        }
    }

    private void Update()
    {
        if(curHealth <= 0 && !dead)
        {
            dead = true;
            curHealth = 0f;
            SC_UiManager.single.HealthBarUpdate();
            Die();
        }
        if (gotHit)
        {
            if (!IsInvoking())
            {
                Invoke(nameof(InvulnerableReset), invulnerableTimer);
            }
        }
    }

    void FixedUpdate()
    {
        if(SC_GameManager.single == null) { return; }
        if (!SC_GameManager.single.gameStart || SC_UiManager.single.loading) 
        {
            SC_CharacterAnimation.single.SetHorizontalAnime(0);
            SC_CharacterAnimation.single.SetVerticalAnime(0);
            r.velocity = Vector3.zero;
            return; 
        }
        //Setup camera offset
        Vector3 cameraOffset = Vector3.zero;
        if (cameraDirection == CameraDirection.x)
        {
            cameraOffset = new Vector3(cameraDistance, cameraHeight, 0);
        }
        else if (cameraDirection == CameraDirection.z)
        {
            cameraOffset = new Vector3(0, cameraHeight, cameraDistance);
        }

        if (grounded && !SC_AttackManager.single.isAttacking)
        {
            walking = false;
            targetVelocity = Vector3.zero;
            MovementCalculations();
            AccelerationAndJumping();
        }

        // We apply gravity manually for more tuning control
        r.AddForce(new Vector3(0, -gravity * r.mass, 0));

        grounded = false;
        CameraOffsetFollowAndAim(cameraOffset);
        PlayerRotation();
    }

    private void MovementCalculations()
    {
        // Calculate how fast we should be moving
        if (cameraDirection == CameraDirection.x)
        {
            targetVelocity = new Vector3(Input.GetAxis("Vertical") * (cameraDistance >= 0 ? -1 : 1), 0, Input.GetAxis("Horizontal") * (cameraDistance >= 0 ? 1 : -1));
        }
        else if (cameraDirection == CameraDirection.z)
        {
            targetVelocity = new Vector3(Input.GetAxis("Horizontal") * (cameraDistance >= 0 ? -1 : 1), 0, Input.GetAxis("Vertical") * (cameraDistance >= 0 ? -1 : 1));
        }
        targetVelocity *= speed;

        if (targetVelocity != Vector3.zero)
        {
            walking = true;
            SC_CharacterAnimation.single.SetHorizontalAnime(Input.GetAxis("Horizontal"));
            SC_CharacterAnimation.single.SetVerticalAnime(Input.GetAxis("Vertical"));
        }
        else
        {
            SC_CharacterAnimation.single.SetHorizontalAnime(0);
            SC_CharacterAnimation.single.SetVerticalAnime(0);
        }
    }

    private void AccelerationAndJumping()
    {
        // Apply a force that attempts to reach our target velocity
        Vector3 velocity = r.velocity;
        Vector3 velocityChange = (targetVelocity - velocity);
        velocityChange.x = Mathf.Clamp(velocityChange.x, -maxVelocityChange, maxVelocityChange);
        velocityChange.z = Mathf.Clamp(velocityChange.z, -maxVelocityChange, maxVelocityChange);
        velocityChange.y = 0;
        r.AddForce(velocityChange, ForceMode.VelocityChange);

        // Jump
        if (canJump && Input.GetButton("Jump"))
        {
            r.velocity = new Vector3(velocity.x, CalculateJumpVerticalSpeed(), velocity.z);
        }
    }

    private void CameraOffsetFollowAndAim(Vector3 cameraOffset)
    {
        //Mouse cursor offset effect
        playerPosOnScreen = playerCamera.WorldToViewportPoint(transform.position);
        cursorPosition = playerCamera.ScreenToViewportPoint(Input.mousePosition);
        offsetVector = cursorPosition - playerPosOnScreen;

        //Camera follow
        playerCamera.transform.position = Vector3.Lerp(playerCamera.transform.position, transform.position + cameraOffset, Time.deltaTime * 7.4f);
        playerCamera.transform.LookAt(transform.position + new Vector3(-offsetVector.y * cameraXYOffset, 0, offsetVector.x * cameraXYOffset));

        //Aim target position and rotation
        targetObject.transform.position = GetAimTargetPos();
        targetObject.transform.LookAt(new Vector3(transform.position.x, targetObject.transform.position.y, transform.position.z));
    }

    private void PlayerRotation()
    {
        //Player rotation
        if (SC_AttackManager.single.isAttacking)
        {
            ResetMovement();
            Vector3 dir = SC_AttackManager.single.attackPos - transform.position;
            DetermineRotation(dir);
            //transform.LookAt(new Vector3(targetObject.transform.position.x, transform.position.y, targetObject.transform.position.z));
        }
        else if (walking == false)
        {
            Vector3 dir = targetObject.transform.position - transform.position;
            DetermineRotation(dir);
        }
        else
        {
            DetermineRotation(targetVelocity);
        }
    }

    private void DetermineRotation(Vector3 dir)
    {
        Quaternion lookRotation = Quaternion.LookRotation(dir);
        Vector3 rotation = Quaternion.Lerp(transform.rotation, lookRotation, Time.deltaTime * turnSpeed).eulerAngles;
        transform.rotation = Quaternion.Euler(0f, rotation.y, 0f);
    }

    private void ResetMovement()
    {
        SC_CharacterAnimation.single.SetHorizontalAnime(0);
        SC_CharacterAnimation.single.SetVerticalAnime(0);
        GetComponent<Rigidbody>().velocity = Vector3.zero;
    }

    public Vector3 GetAimTargetPos()
    {
        //Update surface plane
        surfacePlane.SetNormalAndPosition(Vector3.up, transform.position);

        //Create a ray from the Mouse click position
        Ray ray = playerCamera.ScreenPointToRay(Input.mousePosition);

        //Initialise the enter variable
        if (surfacePlane.Raycast(ray, out float enter))
        {
            //Get the point that is clicked
            Vector3 hitPoint = ray.GetPoint(enter);

            //Move your cube GameObject to the point where you clicked
            return hitPoint;
        }

        //No raycast hit, hide the aim target by moving it far away
        return new Vector3(-5000, -5000, -5000);
    }

    void OnCollisionStay()
    {
        grounded = true;
    }

    float CalculateJumpVerticalSpeed()
    {
        // From the jump height and gravity we deduce the upwards speed 
        // for the character to reach at the apex.
        return Mathf.Sqrt(2 * jumpHeight * gravity);
    }

    public void Die()
    {
        SC_UiManager uiMan = SC_UiManager.single;
        SC_CharacterAnimation charAnim = SC_CharacterAnimation.single;
        if (!uiMan.IsInvoking(nameof(uiMan.GetGameLostScreen)))
        {
            charAnim.Die();
            SC_GameManager.single.gameStart = false;
        }
    }

    private void InvulnerableReset()
    {
        gotHit = false;
    }

    public void DealDamage(float damage)
    {
        if (!gotHit)
        {
            hitEffect.Play();
            SC_AudioManager.single.PlaySound(AudioType.PlayerTakeDamage);
            curHealth -= damage;
            gotHit = true;
        }
        else
        {
            Debug.Log("invulnerable");
        }
    }
}