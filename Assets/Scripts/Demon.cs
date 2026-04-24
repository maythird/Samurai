using System.Collections;
using UnityEngine;

public class Demon : MonoBehaviour
{
    public enum States
    {
        Idle,
        Hit,
        Death
    }

    public delegate void OnStateChanged(States state);
    public event OnStateChanged onStateChanged;

    public delegate void OnHpChanged(int hp, int maxHp);
    public event OnHpChanged onHpChanged;

    public int MaxHp = 100;
    int hp;

    States state = States.Idle;
    Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
        hp = MaxHp;
        onHpChanged?.Invoke(hp, MaxHp);
    }

    public void TakeDamage(int damage, float hitDuration = 0.5f)
    {
        if (state == States.Death) return;

        hp = Mathf.Max(0, hp - damage);
        onHpChanged?.Invoke(hp, MaxHp);

        if (hp == 0)
        {
            PlayDeath();
        }
        else
        {
            PlayHit(hitDuration);
        }
    }

    public void PlayHit(float duration = 0.5f)
    {
        if (state == States.Death) return;

        state = States.Hit;
        animator.SetInteger("State", (int)States.Hit);
        onStateChanged?.Invoke(state);
        StartCoroutine(ReturnToIdleAfterDelay(duration));
    }

    public void PlayIdle()
    {
        if (state == States.Death) return;

        state = States.Idle;
        animator.SetInteger("State", (int)States.Idle);
        onStateChanged?.Invoke(state);
    }

    public void PlayDeath()
    {
        if (state == States.Death) return;

        state = States.Death;
        animator.SetTrigger("Death");
        onStateChanged?.Invoke(state);
    }

    public void Recovery()
    {
        animator.enabled = true;
        hp = MaxHp;
        onHpChanged?.Invoke(hp, MaxHp);
        state = States.Idle;
        animator.ResetTrigger("Death");
        animator.SetInteger("State", (int)States.Idle);
        onStateChanged?.Invoke(state);
    }

    IEnumerator ReturnToIdleAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (state != States.Death)
            PlayIdle();
    }
}
