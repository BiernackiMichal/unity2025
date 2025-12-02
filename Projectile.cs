using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed = 10f;
    public float rotateSpeed = 10f;

    private Transform target;
    private int damage;

    public void Init(Transform target, int damage)
    {
        this.target = target;
        this.damage = damage;
    }

    private void Update()
    {
        if (target == null)
        {
            Destroy(gameObject);
            return;
        }

        // kierunek
        Vector3 dir = target.position - transform.position;

        // poruszanie
        transform.position += dir.normalized * speed * Time.deltaTime;

        // obrót pocisku w stronę celu
        if (dir != Vector3.zero)
        {
            Quaternion rot = Quaternion.LookRotation(dir);
            transform.rotation = Quaternion.Lerp(transform.rotation, rot, rotateSpeed * Time.deltaTime);
        }

        if (dir.magnitude < 0.2f)
        {
            HitTarget();
        }
    }

    private void HitTarget()
    {
        Enemy e = target.GetComponent<Enemy>();
        if (e != null)
            e.TakeDamage(damage);

        Destroy(gameObject);
    }
}
