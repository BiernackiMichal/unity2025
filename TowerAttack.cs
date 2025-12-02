using UnityEngine;

public class TowerAttack : MonoBehaviour
{
    [Header("Tower Stats")]
    public float range = 6f;
    public float fireRate = 1f;
    public int damage = 10;

    [Header("Shooting")]
    public GameObject projectilePrefab;
    public Transform shootPoint;

    [Header("Rotation")]
    public Transform turretHead;
    public float rotationSpeed = 6f;

    [Tooltip("Jeśli model jest obrócony o 90°, ustaw offset np. na -90.")]
    public float rotationOffsetY = 0f;

    private float fireCooldown = 0f;
    private Enemy currentTarget;

    private void Update()
    {
        fireCooldown -= Time.deltaTime;

        currentTarget = FindTarget();

        if (currentTarget != null)
        {
            RotateToTarget();

            if (fireCooldown <= 0f)
            {
                Shoot(currentTarget);
                fireCooldown = 1f / fireRate;
            }
        }
    }

    private Enemy FindTarget()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, range);

        Enemy closest = null;
        float closestDist = Mathf.Infinity;

        foreach (var h in hits)
        {
            Enemy e = h.GetComponent<Enemy>();
            if (e != null)
            {
                float d = Vector3.Distance(transform.position, e.transform.position);
                if (d < closestDist)
                {
                    closestDist = d;
                    closest = e;
                }
            }
        }

        return closest;
    }

    private void RotateToTarget()
    {
        if (turretHead == null || currentTarget == null) return;

        Vector3 dir = currentTarget.transform.position - turretHead.position;
        dir.y = 0f;

        if (dir.sqrMagnitude < 0.01f) return;

        Quaternion targetRot =
            Quaternion.LookRotation(dir) *
            Quaternion.Euler(0, rotationOffsetY, 0); // poprawka na model

        turretHead.rotation = Quaternion.Lerp(
            turretHead.rotation,
            targetRot,
            Time.deltaTime * rotationSpeed
        );
    }

    private void Shoot(Enemy target)
    {
        GameObject p = Instantiate(projectilePrefab, shootPoint.position, shootPoint.rotation);
        Projectile proj = p.GetComponent<Projectile>();
        proj.Init(target.transform, damage);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, range);
    }
}
