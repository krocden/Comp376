using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour
{
    private float speed;
    [SerializeField] private float walkSpeed;
    [SerializeField] private float runningSpeed;
    [SerializeField] private float gravity;
    [SerializeField] private float jumpHeight;
    [SerializeField] private float verticalVelocity;
    
    private Vector3 movement;
    
    private CharacterController controller;
    private Camera firstPersonCamera;

    public bool grounded;
    [SerializeField] private int buffZones = 0;
    [SerializeField] private float buffModifier = 0f;
    private bool isRunning;

    [SerializeField] private Slider playerHealthSlider;
    public bool isInvincible;
    public float invulnerableDuration;
    int maxHealth = 100;
    float currentHealth;
    public float healthRegenRate;
    public float respawnTime;

    public int deadPenalty;
    public bool isDead = false;
    Vector3 knockback = Vector3.zero;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        firstPersonCamera = Camera.main;
        speed = walkSpeed;
        currentHealth = maxHealth;
    }

    void Update()
    {
        if (GameStateManager.Instance.BlockInput)
            return;

        if (isDead)
            return;

        if (!isInvincible)
        {
            if (currentHealth < 100)
            {
                currentHealth += healthRegenRate;
                updateHealthSlider();
            }
        }

        grounded = controller.isGrounded;

        if (Input.GetKeyDown(KeyCode.LeftShift) && grounded)
        {
            isRunning = true;
        } 
        if (Input.GetKeyUp(KeyCode.LeftShift)) 
        {
            if (grounded)
            {
                isRunning = false;
            } 
            else 
            {
                StartCoroutine(landing());
            }
        }

        speed = (buffZones > 0) ? buffModifier * (isRunning ? runningSpeed : walkSpeed) : (isRunning ? runningSpeed : walkSpeed);

        if (grounded)
        {
            verticalVelocity = 0;
        }

        verticalVelocity += gravity * Time.deltaTime;

        if (Input.GetButton("Jump"))
        {
            if (grounded)
            {
                verticalVelocity += Mathf.Sqrt(jumpHeight * -2 * gravity);
            }
        }

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 forward = firstPersonCamera.transform.forward;
        Vector3 right = firstPersonCamera.transform.right;

        forward.y = 0;
        right.y = 0;
        forward.Normalize();
        right.Normalize();

        movement = forward * z + right * x;
        
        movement.y = verticalVelocity;

        if (knockback.magnitude >= 0.5)
        {
            knockback.y = verticalVelocity;
            controller.Move(knockback * Time.deltaTime);
            knockback = Vector3.Lerp(knockback, Vector3.zero, 10 * Time.deltaTime);
        }
        else
        {
            controller.Move(movement * Time.deltaTime * speed);
        }
    }

    void handleInput()
    {
        //Move code here
    }
    public void takeDamage(int damage)
    {
        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            currentHealth = 0;
            StartCoroutine(die());
        }

        updateHealthSlider();

        StartCoroutine(isHit());
    }

    IEnumerator die()
    {
        isDead = true;

        CurrencyManager.Instance.SubtractCurrency(deadPenalty);

        transform.position = new Vector3(0, 3, 0);

        yield return new WaitForSeconds(respawnTime);

        isDead = false;
        currentHealth = 100;
        updateHealthSlider();
    }

    void updateHealthSlider()
    {
        playerHealthSlider.value = (float)currentHealth / maxHealth;
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.gameObject.tag == "Enemy")
        {
            Monster enemy = hit.gameObject.GetComponent<Monster>();

            if (!isInvincible)
            {
                takeDamage(enemy.damage);

                /*if (hit.moveDirection.y < -0.3)
                {
                    return;
                }*/

                Vector3 dir = transform.position - enemy.transform.position;
                //Vector3 pushDir = new Vector3(hit.moveDirection.x, 0, hit.moveDirection.z);
                //float force = Mathf.Clamp(enemy.pushPower, 0, hit.moveDirection.z);
                float force = enemy.pushPower;

                knockback = dir.normalized * force;
                //controller.velocity = pushDir * enemy.pushPower * 2000;
            }
        }
    }

    private IEnumerator isHit()
    {
        isInvincible = true;

        yield return new WaitForSeconds(invulnerableDuration);

        isInvincible = false;
    }

    IEnumerator landing()
    {
        yield return new WaitUntil(() => grounded);
        speed = walkSpeed;
    }

    public void ApplyBuff(float modifier) {
        buffZones++;
        buffModifier = modifier;
    }

    public void RemoveBuff() {
        buffZones--;
        if (buffZones == 0)
            buffModifier = 0;
    }

    public void SetPosition(Vector3 newPos) {
        controller.enabled = false;
        this.transform.position = newPos;
        controller.enabled = true;
    }

}
