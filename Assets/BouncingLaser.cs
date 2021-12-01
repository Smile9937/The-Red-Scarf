
using System.Collections;
using UnityEngine;

public class BouncingLaser : MonoBehaviour
{
    [SerializeField] private float speed = 1f;
    [SerializeField] private LayerMask laserHitLayers;
    [SerializeField] private int damage;
    [SerializeField] private Vector2 knockback;
    [SerializeField] private float knockbackLength;
    [SerializeField] private float damageRate;
    [SerializeField] private float length;
    [SerializeField] private int numberOfBouncesBeforeDisappearing;

    private Vector3 beginPos = new Vector3(0f, 0f, 0);
    private Vector3 endPos = new Vector3(-3f, 0f, 0);

    private int numberOfCharges = 0;

    private bool canDamage = true;

    private LineRenderer lineRenderer;
    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
    }

    private void OnEnable()
    {
        beginPos = transform.position;
        endPos = new Vector3(beginPos.x - length, beginPos.y, beginPos.z);
        lineRenderer.SetPosition(0, beginPos);
        lineRenderer.SetPosition(1, endPos);
        numberOfCharges = numberOfBouncesBeforeDisappearing;
        float dir = Random.Range(0, 360);
        Rotate(dir);
    }

    private void Update()
    {
        Vector3 newBeginPos = transform.localToWorldMatrix * new Vector4(beginPos.x, beginPos.y, beginPos.z, 1);
        Vector3 newEndPos = transform.localToWorldMatrix * new Vector4(endPos.x, endPos.y, endPos.z, 1);

        lineRenderer.SetPosition(0, newBeginPos);
        lineRenderer.SetPosition(1, newEndPos);

        transform.Translate(Vector3.left * speed * Time.deltaTime);

        Vector2 direction = lineRenderer.GetPosition(1) - lineRenderer.GetPosition(0);
        RaycastHit2D hit = Physics2D.Raycast(lineRenderer.GetPosition(0), direction.normalized, direction.magnitude, laserHitLayers);

        if (hit.collider == null)
            return;
        if (hit.collider.CompareTag("Ground"))
        {
            HitGround(hit);
        }

        if (!canDamage)
            return;

        Player player = hit.collider.GetComponent<Player>();
        if (player != null)
        {
            canDamage = false;
            StartCoroutine(LaserHitCooldown());
            player.Damage(damage, false);
            player.KnockBack(gameObject, knockback, knockbackLength);
        }

    }

    private IEnumerator LaserHitCooldown()
    {
        yield return new WaitForSeconds(damageRate);
        canDamage = true;
    }


    private void HitGround(RaycastHit2D hit)
    {
        if (numberOfCharges <= 0)
        {
            gameObject.SetActive(false);
        }
        else
        {
            numberOfCharges--;
            Vector2 hitNormal = hit.normal.normalized;

            if (hitNormal.y != 0f)
            {
                float rotation = -transform.rotation.eulerAngles.z;
                Rotate(rotation);
            }
            else if (hit.normal.x != 0f)
            {
                float rotation = -transform.rotation.eulerAngles.z - 180;
                Rotate(rotation);
            }
        }
    }

    private void Rotate(float rotation)
    {
        transform.eulerAngles = Vector3.forward * rotation;
    }
}
