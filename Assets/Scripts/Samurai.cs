using UnityEngine;

public class Samurai : MonoBehaviour
{
    public enum States
    {
        Idle,
        Attack,
        Run
    }

    public delegate void OnStateChanged(States state);
    public event OnStateChanged onStateChanged;

    public delegate void OnAttackHit(float remainingTime);
    public event OnAttackHit onAttackHit;

    public float runSpeed = 3f;
    [Range(0f, 1f)] public float attackFxTiming = 0.5f;

    States state = States.Idle;
    Animator animator;
    bool fxFired = false;

    void Start()
    {
        animator = GetComponent<Animator>();
        PlayRun();
    }

    void Update()
    {
        if (state == States.Attack)
        {
            AnimatorStateInfo info = animator.GetCurrentAnimatorStateInfo(0);

            if (!fxFired && info.IsName("Samurai_Attack") && info.normalizedTime >= attackFxTiming)
            {
                fxFired = true;
                float remainingTime = info.length * (1f - info.normalizedTime);
                onAttackHit?.Invoke(remainingTime);
            }

            if (info.IsName("Samurai_Attack") && info.normalizedTime >= 1f)
            {
                fxFired = false;
                PlayIdle();
            }
        }

        if (state == States.Run)
        {
            Vector3 target = new Vector3(0f, -3.5f, 0f);
            transform.position = Vector3.MoveTowards(transform.position, target, runSpeed * Time.deltaTime);

            if (transform.position == target)
            {
                PlayIdle();
            }
        }
    }

    public bool PlayAttack()
    {
        if (state == States.Attack) return false;

        state = States.Attack;
        animator.SetInteger("State", (int)States.Attack);
        onStateChanged?.Invoke(state);
        return true;
    }

    public void PlayIdle()
    {
        state = States.Idle;
        animator.SetInteger("State", (int)States.Idle);
        onStateChanged?.Invoke(state);
    }

    public void PlayRun()
    {
        state = States.Run;
        animator.SetInteger("State", (int)States.Run);
        onStateChanged?.Invoke(state);
    }
}
