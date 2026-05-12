using System.Collections;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    [Header("AR Settings")]
    public GameObject imageTarget;

    [Header("Health System")]
    public int maxHealth = 100;
    private int currentHealth;
    private bool isDead = false;

    [Header("Combat Settings")]
    public float attackRange = 1.5f;
    public float attackCooldown = 2.0f;
    public int attackDamage = 20;
    public float moveSpeed = 2.0f;

    [Header("Physics")]
    public float gravity = 5f;
    private Vector3 velocity;

    [Header("Input Settings")]
    public FixedJoystick joystick;   // ✅ Joystick

    private float attackTimer = 0f;
    private Animator animator;
    private Transform targetEnemy;
    private CharacterController characterController;

    void Start()
    {
        animator = GetComponent<Animator>();
        characterController = GetComponent<CharacterController>();
        currentHealth = maxHealth;

        // Tìm joystick nếu chưa gán
        if (joystick == null)
        {
            joystick = FindFirstObjectByType<FixedJoystick>();
        }

        // Neo Player vào ImageTarget
        if (imageTarget == null)
        {
            imageTarget = GameObject.Find("ImageTarget");
        }

        if (imageTarget != null)
        {
            transform.SetParent(imageTarget.transform);
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
        }

        gameObject.tag = "Player";
    }

    void Update()
    {
        if (isDead) return;

        // ✅ Điều khiển bằng joystick
        float horizontal = joystick.Horizontal;
        float vertical = joystick.Vertical;
        Vector3 inputDirection = new Vector3(horizontal, 0, vertical);

        if (inputDirection.magnitude > 0.1f)
        {
            // Xoay Player theo hướng joystick
            transform.rotation = Quaternion.LookRotation(inputDirection);

            // Di chuyển
            Vector3 movement = inputDirection.normalized * moveSpeed;
            SafeMove(movement * Time.deltaTime);

            if (animator != null) animator.SetFloat("Blend", movement.magnitude);
        }
        else
        {
            if (animator != null) animator.SetFloat("Blend", 0f);
        }

        // ✅ Combat tự động tìm Enemy
        FindNearestEnemy();
        if (targetEnemy != null)
        {
            float distanceToEnemy = Vector3.Distance(transform.position, targetEnemy.position);
            if (distanceToEnemy <= attackRange)
            {
                attackTimer += Time.deltaTime;
                if (attackTimer >= attackCooldown)
                {
                    Attack();
                }
            }
        }

        // ✅ Gravity
        if (characterController != null && characterController.enabled && characterController.gameObject.activeInHierarchy)
        {
            if (characterController.isGrounded)
            {
                velocity.y = 0f;
            }
            else
            {
                velocity.y -= gravity * Time.deltaTime;
            }
            SafeMove(velocity * Time.deltaTime);
        }
    }

    void SafeMove(Vector3 move)
    {
        if (characterController != null && characterController.enabled && characterController.gameObject.activeInHierarchy)
        {
            characterController.Move(move);
        }
    }

    void FindNearestEnemy()
    {
        GameObject enemyObj = GameObject.FindGameObjectWithTag("Enemy");
        if (enemyObj != null)
        {
            EnemyManager enemyScript = enemyObj.GetComponent<EnemyManager>();
            if (enemyScript != null && !enemyScript.IsDead())
            {
                targetEnemy = enemyObj.transform;
                return;
            }
        }
        targetEnemy = null;
    }

    void Attack()
    {
        attackTimer = 0f;
        animator.SetTrigger("Attack");
        StartCoroutine(DealDamageAfterDelay(0.5f));
    }

    IEnumerator DealDamageAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (targetEnemy != null)
        {
            EnemyManager enemy = targetEnemy.GetComponent<EnemyManager>();
            if (enemy != null) enemy.TakeDamage(attackDamage);
        }
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;
        currentHealth -= damage;
        Debug.Log("Player HP: " + currentHealth);
        if (currentHealth <= 0) Die();
    }

    void Die()
    {
        isDead = true;
        animator.SetTrigger("Death");
        Debug.Log("Player đã chết!");
    }

    public bool IsDead() => isDead;
}
