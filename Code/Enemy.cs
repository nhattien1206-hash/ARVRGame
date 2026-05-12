using UnityEngine;
using System.Collections;

public class EnemyManager : MonoBehaviour
{
    public CharacterController characterController;
    public float speed = 2.0f;
    public GameObject player;
    private Animator animator;

    [Header("Chỉ số Chiến đấu")]
    public float maxHealth = 100f;
    private float currentHealth;
    public float attackRange = 1.5f;
    public float attackCooldown = 2.0f;
    public int damage = 10;

    private float lastAttackTime;
    private bool isDead = false;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        currentHealth = maxHealth;

        if (player == null)
        {
            var playerScript = FindObjectOfType<PlayerManager>();
            if (playerScript != null) player = playerScript.gameObject;
        }

        gameObject.tag = "Enemy";
    }

    void Update()
    {
        if (isDead || player == null) return;

        float distance = Vector3.Distance(transform.position, player.transform.position);

        if (distance > attackRange)
        {
            MoveToPlayer();
            if (animator != null) animator.SetFloat("Blend", 1f);
        }
        else
        {
            if (animator != null) animator.SetFloat("Blend", 0f);
            if (Time.time >= lastAttackTime + attackCooldown) Attack();
        }
    }

    void MoveToPlayer()
    {
        var direction = player.transform.position - transform.position;
        direction.y = 0;

        if (direction != Vector3.zero) transform.forward = direction.normalized;

        var movement = direction.normalized * speed;
        characterController.Move(movement * Time.deltaTime);
    }

    void Attack()
    {
        lastAttackTime = Time.time;
        if (animator != null) animator.SetTrigger("Attack");
        StartCoroutine(DealDamageToPlayer(0.5f));
    }

    IEnumerator DealDamageToPlayer(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (player != null)
        {
            var pManager = player.GetComponent<PlayerManager>();
            if (pManager != null && !pManager.IsDead()) pManager.TakeDamage(damage);
        }
    }

    public void TakeDamage(int amount)
    {
        if (isDead) return;
        currentHealth -= amount;
        Debug.Log("Enemy nhận sát thương! Máu còn: " + currentHealth);
        if (currentHealth <= 0) Die();
    }

    void Die()
    {
        isDead = true;
        if (animator != null) animator.SetTrigger("Death");
        Debug.Log("Enemy đã bị tiêu diệt!");
        Destroy(gameObject, 3f);
    }

    public bool IsDead() => isDead;
}