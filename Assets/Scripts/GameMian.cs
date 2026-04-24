using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GameMian : MonoBehaviour
{
    public Button btnAttack;
    public Samurai samurai;
    

    void Start()
    {
        btnAttack.gameObject.SetActive(false);
        btnAttack.onClick.AddListener(() =>
        {
            StartCoroutine(AttackRoutine());
        });
        samurai.onAttackStarted += OnAttackStarted;
        StartCoroutine(MoveAndShowButton());
    }

    void OnAttackStarted()
    {
        Debug.Log("사무라이 공격 시작!");
    }
    
    
    IEnumerator MoveAndShowButton()
    {
        yield return StartCoroutine(samurai.Move(Vector3.zero));
        btnAttack.gameObject.SetActive(true);
    }

    IEnumerator AttackRoutine()
    {
        btnAttack.interactable = false;
        yield return StartCoroutine(samurai.Attack());
        btnAttack.interactable = true;
    }
}
