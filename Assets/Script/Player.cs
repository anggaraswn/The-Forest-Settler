using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{

    private CharacterController controller;
    private Animator animator;
    private CapsuleCollider capsuleCollider;
    public Interactable focus;
    public HealthBar healthBar;

    [SerializeField] float speed = 6f;
    [SerializeField] float maxRotateSpeed = 0.1f;
    [SerializeField] Transform camera;


    private float currentVelocity;
    private float gravity = 9.8f;
    public int maxHealth = 100;
    public int currentHealth;
    private bool canAttack = false;


    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponentInChildren<Animator>();
        capsuleCollider = GetComponent<CapsuleCollider>();
        currentHealth = maxHealth;
        healthBar.SetMaxHealth(maxHealth);
    }

    // Update is called once per frame
    void Update()
    {
        Movement();
        Animation();

        if (Input.GetMouseButtonDown(0))
        {
            TakeDamage(20);
        }
    }

    private void Movement()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        Vector3 direction = new Vector3(horizontal, 0, vertical).normalized;
        Vector3 moveDirection = new Vector3();
        if (direction.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + camera.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref currentVelocity, maxRotateSpeed);

            moveDirection = Quaternion.Euler(0, angle, 0) * Vector3.forward;

            moveDirection = moveDirection.normalized;

            transform.rotation = Quaternion.Euler(0, angle, 0);
        }

        moveDirection.y += (gravity * -1);
        moveDirection.x *= speed;
        moveDirection.z *= speed;
        controller.Move(moveDirection * Time.deltaTime);


        if (Input.GetKey(KeyCode.LeftShift))
        {
            speed = 15f;
        }
        else if (!Input.GetKey(KeyCode.LeftShift))
        {
            speed = 6f;
        }
    }

    private void Animation()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        Vector3 direction = new Vector3(horizontal, 0, vertical).normalized;


        if (direction != Vector3.zero && !Input.GetKey(KeyCode.LeftShift))
        {
            //walk
            animator.SetBool("IsIdle", false);
            animator.SetBool("IsWalking", true);
            animator.SetBool("IsSprinting", false);

            animator.SetFloat("DirectionX", direction.x);
            animator.SetFloat("DirectionY", Mathf.Abs(direction.z));

            if (Input.GetMouseButton(1))
            {
                animator.SetBool("IsRolling", true);
                animator.SetBool("IsWalking", false);
            }
            else
            {
                animator.SetBool("IsRolling", false);
            }
        }
        else if (direction != Vector3.zero && Input.GetKey(KeyCode.LeftShift))
        {
            //run
            animator.SetBool("IsIdle", false);
            animator.SetBool("IsSprinting", true);
            animator.SetBool("IsWalking", false);

            animator.SetFloat("DirectionX", 0f);
            animator.SetFloat("DirectionY", 2);

            if (Input.GetMouseButton(1))
            {
                animator.SetBool("IsRolling", true);
                animator.SetBool("IsSprinting", false);
            }
            else
            {
                animator.SetBool("IsRolling", false);
            }
        }
        else if (direction != Vector3.zero && !Input.GetKey(KeyCode.LeftShift) && direction.z == 0)
        {
            //strafe
            animator.SetBool("IsIdle", false);
            animator.SetBool("IsWalking", true);
            animator.SetBool("IsSprinting", false);

            animator.SetFloat("DirectionX", direction.x);
            animator.SetFloat("DirectionY", 0f);

        }
        else
        {
            animator.SetBool("IsIdle", true);
            animator.SetBool("IsWalking", false);
            animator.SetBool("IsSprinting", false);


            animator.SetFloat("DirectionX", 0f);
            animator.SetFloat("DirectionY", 0f);

        }
    }

    private void Combat()
    {
        
    }

    private void SetFocus(Interactable newFocus)
    {
        if (newFocus != focus)
        {
            if (focus != null) focus.OnDefocused();
            focus = newFocus;
        }
        newFocus.OnFocused(transform);

    }

    private void RemoveFocus()
    {
        if(focus != null) focus.OnDefocused();
        focus = null;
    }

    private void OnTriggerEnter(Collider other)
    {
        Interactable interactable = other.GetComponent<Interactable>();
        if (interactable != null) SetFocus(interactable);

        if(other.gameObject.tag == "Enemy")
        {
            canAttack = true;
            Debug.Log("Can Attack " + other.name);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Interactable interactable = other.GetComponent<Interactable>();
        if (interactable != null) RemoveFocus();
    }

    void TakeDamage(int damage)
    {
        currentHealth -= damage;
        healthBar.SetHealth(currentHealth);
    }

}