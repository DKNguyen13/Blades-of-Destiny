using System.Collections;
using UnityEngine;

public class E_Leaf_Arrow : MonoBehaviour
{

    [Header("Arrow")]
    [SerializeField] private GameObject arrowPrefab;
    [SerializeField] private float arrowSpeed = 20f; // arrow speed
    [SerializeField] private float offsetX = 6f;
    private Transform playerTransform;
    private Vector3 enemyPosition;
    Animator animator;
    private GameObject arrow;

    // Start is called before the first frame update
    void Start()
    {
        playerTransform = GetComponent<Transform>();
    }

    //Shoot arrow
    public void ShootArrow()
    {
        Vector3 spawnPosition = playerTransform.position + new Vector3(offsetX, 0.3f, 0);
        arrow = Instantiate(arrowPrefab, spawnPosition, Quaternion.identity);// Create arrow
        StartCoroutine(MoveArrowToEnemy(arrow.transform, enemyPosition));// Make the arrow move toward the enemy
    }

    // Coroutine để di chuyển mũi tên đến vị trí kẻ thù
    private IEnumerator MoveArrowToEnemy(Transform arrowTransform, Vector3 targetPosition)
    {
        while (arrowTransform != null)
        {
            Vector3 direction = (targetPosition - arrowTransform.position).normalized;// Tính toán hướng di chuyển
            arrowTransform.position += direction * arrowSpeed * Time.deltaTime;// Di chuyển mũi tên theo hướng đó

            // Kiểm tra nếu mũi tên đến gần đủ vị trí mục tiêu
            if (Vector3.Distance(arrowTransform.position, targetPosition) < 0.1f)
            {
                animator = arrow.GetComponent<Animator>();
                animator.SetTrigger("skill2");
                StartCoroutine(DestroyArrowAfterAnimation());
                break;
            }
            yield return null;
        }
    }
    private IEnumerator DestroyArrowAfterAnimation()
    {
        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);
        Destroy(arrow);
    }
    public Vector3 EnemyPosition { get => enemyPosition; set => enemyPosition = value;}
}
