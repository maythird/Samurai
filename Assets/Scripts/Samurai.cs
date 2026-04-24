using UnityEngine;
using System.Collections;

public class Samurai : MonoBehaviour
{
   //애니메이터 paramaters
   // int 형식 state
   // 0 : Idle
   // 1 : Attack
   // 2 : Run

   public delegate void OnAttackStarted();
   public event OnAttackStarted onAttackStarted;

   public Transform fxSlashPoint;

   private Animator animator;
   public float moveSpeed = 3f;

   private void Awake()
   {
      animator = GetComponent<Animator>();
   }

   public IEnumerator Move(Vector3 tPos)
   {
      animator.SetInteger("State", 2);

      while (Vector3.Distance(transform.position, tPos) > 0.05f)
      {
         transform.position = Vector3.MoveTowards(transform.position, tPos, moveSpeed * Time.deltaTime);
         yield return null;
      }

      transform.position = tPos;
      animator.SetInteger("State", 0);
   }

   public IEnumerator Attack()
   {
      animator.SetInteger("State", 1);
      onAttackStarted?.Invoke();

      if (fxSlashPoint != null)
         FxManager.Instance?.PlaySlash(fxSlashPoint.position);

      yield return null; // 애니메이터가 Attack 상태로 전환될 때까지 한 프레임 대기
      float clipLength = animator.GetCurrentAnimatorStateInfo(0).length;
      yield return new WaitForSeconds(clipLength);

      animator.SetInteger("State", 0);
   }
}
