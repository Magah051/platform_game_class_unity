using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public Transform waypointA;
    public Transform waypointB;
    public float movementSpeed = 2f;
    private Animator animator;
    private bool isWalking = false;
    public int enemyHealth = 50;
    public float attackInterval = 1f;

    private Transform currentTarget;
    private Rigidbody2D rb;
    private Vector3 scale;
    private Coroutine attackCoroutine;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        currentTarget = waypointA;
        scale = transform.localScale;
        Debug.Log("Life do Enemy: " + enemyHealth);
    }

    // Update is called once per frame
    void Update()
    {
        MoveTowardsTarget();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("ZoneAttack"))
        {
            Debug.Log("Inimigo entrou na zona de ataque");
        }

        PlayerController player = other.GetComponent<PlayerController>();

        if (player == null)
        {
            player = other.GetComponentInParent<PlayerController>();
        }

        if (player != null)
        {
            if (attackCoroutine == null)
            {
                attackCoroutine = StartCoroutine(AttackPlayer(player));
            }
        }
        else
        {
            Debug.LogWarning("Player Controller n�o encontrado no objeto com a tag ZoneAttack");
        }

    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("ZoneAttack"))
        {
            Debug.Log("Inimigo saiu da zona de ataque.");

            if (attackCoroutine != null)
            {
                StopCoroutine(attackCoroutine);
                attackCoroutine = null;
            }
        }
    }

    private IEnumerator AttackPlayer(PlayerController player)
    {
        while (true)
        {
            player.TakeDamage(10);//Valor pode ser alterado conforme sua necessidade.
            animator.SetTrigger("Attack");
            Debug.Log("Inimigo atacando...");

            yield return new WaitForSeconds(attackInterval);
        }
    }


    private void MoveTowardsTarget()
    {
        Vector3 curTargetHorizontal = new Vector2(currentTarget.position.x, transform.position.y);
        Vector2 direction = (curTargetHorizontal - transform.position).normalized;

        transform.position += (Vector3)direction * movementSpeed * Time.deltaTime;

        if (Vector2.Distance(curTargetHorizontal, transform.position) <= 0.2f)
        {
            SwitchTarget();
        }

        UpdateAnimation();
    }

    private void SwitchTarget()
    {
        if (currentTarget == waypointA)
        {
            currentTarget = waypointB;
            Flip();
        }
        else
        {
            currentTarget = waypointA;
            transform.localScale = scale;
        }
    }

    private void UpdateAnimation()
    {
        isWalking = (Vector2.Distance(transform.position, currentTarget.position) > 0.1f);
        animator.SetBool("isWalking", isWalking);
    }

    private void Flip()
    {
        Vector3 flippedScale = scale;
        flippedScale.x *= -1;
        transform.localScale = flippedScale;
    }
}
