using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class GameMain : MonoBehaviour
{
    public Button BtnAttack;
    public Button BtnRecovery;
    public Button BtnDeath;
    public Samurai samurai;
    public Demon demon;
    public FxManager fxManager;
    public Transform fxSpawnPoint;
    public Slider DemonHpGague;
    public Canvas canvas;
    public float hpGagueHeadOffset = 0.2f;
    public int attackDamage = 10;

    Camera mainCamera;
    RectTransform hpGagueRect;
    SpriteRenderer demonSprite;
    float demonHeadHeight;

    void Start()
    {
        mainCamera = Camera.main;
        hpGagueRect = DemonHpGague.GetComponent<RectTransform>();
        demonSprite = demon.GetComponent<SpriteRenderer>();
        demonHeadHeight = demonSprite.bounds.max.y - demon.transform.position.y;

        samurai.onStateChanged += OnSamuraiStateChanged;
        samurai.onAttackHit += OnSamuraiAttackHit;
        demon.onStateChanged += OnDemonStateChanged;
        demon.onHpChanged += OnDemonHpChanged;
        BtnAttack.onClick.AddListener(OnAttackButtonClicked);
        BtnRecovery.onClick.AddListener(OnRecoveryButtonClicked);
        BtnDeath.onClick.AddListener(OnDeathButtonClicked);
        BtnAttack.gameObject.SetActive(false);
        BtnRecovery.gameObject.SetActive(false);
    }

    void Update()
    {
        
    }

    void LateUpdate()
    {
        if (!DemonHpGague.gameObject.activeSelf) return;

        if (!demon.gameObject.activeSelf)
        {
            DemonHpGague.gameObject.SetActive(false);
            return;
        }

        Vector3 headPos = new Vector3(
            demon.transform.position.x,
            demon.transform.position.y + demonHeadHeight + hpGagueHeadOffset,
            demon.transform.position.z
        );
        Vector3 screenPos = mainCamera.WorldToScreenPoint(headPos);
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.GetComponent<RectTransform>(),
            screenPos,
            canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : mainCamera,
            out Vector2 localPos
        );
        hpGagueRect.localPosition = localPos;
    }

    void OnAttackButtonClicked()
    {
        samurai.PlayAttack();
    }

    void OnRecoveryButtonClicked()
    {
        demon.gameObject.SetActive(true);
        demon.Recovery();
        DemonHpGague.gameObject.SetActive(true);
        BtnRecovery.gameObject.SetActive(false);
        BtnDeath.gameObject.SetActive(true);
    }

    void OnDeathButtonClicked()
    {
        Debug.Log("[GameMain] Death button clicked");
        demon.TakeDamage(demon.MaxHp);
    }

    void OnDemonStateChanged(Demon.States state)
    {
        Debug.Log($"[GameMain] Demon state changed: {state}");

        if (state == Demon.States.Death)
        {
            StartCoroutine(HideDemonAfterDeath());
        }
    }

    void OnDemonHpChanged(int hp, int maxHp)
    {
        Debug.Log($"[GameMain] Demon HP: {hp} / {maxHp}");
        DemonHpGague.value = (float)hp / maxHp;

        if (hp == 0)
            DemonHpGague.gameObject.SetActive(false);
    }

    IEnumerator HideDemonAfterDeath()
    {
        Animator demonAnimator = demon.GetComponent<Animator>();

        yield return new WaitUntil(() =>
        {
            AnimatorStateInfo info = demonAnimator.GetCurrentAnimatorStateInfo(0);
            return info.IsName("Demon_Death") && info.normalizedTime >= 0.95f;
        });

        demonAnimator.enabled = false;
        demon.gameObject.SetActive(false);
        BtnDeath.gameObject.SetActive(false);
        BtnRecovery.gameObject.SetActive(true);
    }

    void OnSamuraiStateChanged(Samurai.States state)
    {
        Debug.Log($"[GameMain] Samurai state changed: {state}");

        if (state == Samurai.States.Idle && !BtnAttack.gameObject.activeSelf)
        {
            BtnAttack.gameObject.SetActive(true);
        }

    }

    void OnSamuraiAttackHit(float remainingTime)
    {
        demon.TakeDamage(attackDamage, remainingTime);
        Vector3 spawnPos = fxSpawnPoint != null ? fxSpawnPoint.position : samurai.transform.position;
        fxManager.SpawnSlashFx(spawnPos);
    }
}
