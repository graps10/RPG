using System.Collections;
using UnityEngine;

public class ShockStrike_Controller : MonoBehaviour, PooledObject
{
    [SerializeField] private CharacterStats targetStats;
    [SerializeField] private float speed;
    private int damage;

    private Animator anim;
    private bool triggered;

    void Awake()
    {
        anim = GetComponentInChildren<Animator>();
    }

    public void Setup(int _damage, CharacterStats _targetStats)
    {
        damage = _damage;
        targetStats = _targetStats;
    }

    public void OnSpawn() { }

    public void OnReturnToPool()
    {
        triggered = false;
        targetStats = null;

        transform.position = Vector2.zero;
        transform.localScale = Vector3.one;

        anim.transform.localPosition = Vector3.zero;
    }

    void Update()
    {
        if (!targetStats) return;

        if (triggered) return;

        transform.position = Vector2.MoveTowards(transform.position, targetStats.transform.position, speed * Time.deltaTime);
        transform.right = transform.position - targetStats.transform.position;

        if (Vector2.Distance(transform.position, targetStats.transform.position) < 0.1f)
        {
            triggered = true;

            anim.transform.localRotation = Quaternion.identity;
            anim.transform.localPosition = new Vector3(0, 0.5f);

            transform.localRotation = Quaternion.identity;
            transform.localScale = new Vector3(3, 3);

            Invoke("DamageAndSelfDestroy", 0.2f);
            anim.SetTrigger("Hit");
        }
    }

    private void DamageAndSelfDestroy()
    {
        targetStats.ApplyShock(true);

        targetStats.TakeDamage(damage);
        StartCoroutine(ReturnFXAfterDelay("shockStrike", gameObject, 0.4f));
    }
    private IEnumerator ReturnFXAfterDelay(string poolKey, GameObject fx, float delay)
    {
        yield return new WaitForSeconds(delay);
        PoolManager.instance.Return(poolKey, fx);
    }
}
