using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public GameObject winPanel;
    private CharacterController cc;

    public float sprintMult = 1.3f;
    public float maxStamina = 100f;
    public float drainRate = 30f;
    public float regenRate = 10f;
    private float stamina;
    private bool exhausted = false;
    public RectTransform staminaBarFill;
    public Image staminaBarImage;

    [SerializeField] private ParticleSystem sprintEffect;
    

    readonly Color fullColor = new Color(1f, 0.85f, 0f);
    readonly Color lowColor  = new Color(0.9f, 0.1f, 0.1f);

    void Start()
    {
        cc = GetComponent<CharacterController>();
        stamina = maxStamina;
    }

    void FixedUpdate()
{
    float h = Input.GetAxis("Horizontal");
    float v = Input.GetAxis("Vertical");

    if (stamina <= 0) exhausted = true;
    if (stamina >= maxStamina * 0.3f) exhausted = false;
    bool isSprinting = Input.GetKey(KeyCode.LeftShift) && stamina > 0 && !exhausted;
    stamina += (isSprinting ? -drainRate : regenRate) * Time.deltaTime;
    stamina = Mathf.Clamp(stamina, 0, maxStamina);

    float fill = stamina / maxStamina;
    staminaBarFill.pivot = new Vector2(0.5f, 0f);
    staminaBarFill.localScale = new Vector3(1, fill, 1);

    if (staminaBarFill != null)
        staminaBarFill.localScale = new Vector3(1, fill, 1);

    if (staminaBarImage != null)
        staminaBarImage.color = Color.Lerp(lowColor, fullColor, fill);

    float currentSpeed = moveSpeed * (isSprinting ? sprintMult : 1f);
    cc.Move(new Vector3(h, 0, v) * currentSpeed * Time.deltaTime);

    Vector3 move = new Vector3(h, 0, v);

    if (isSprinting && (h != 0 || v != 0))
    {
        sprintEffect.transform.forward = -move.normalized;

        if (sprintEffect != null && !sprintEffect.isPlaying)
            sprintEffect.Play();
    }
    else
    {
        if (sprintEffect != null && sprintEffect.isPlaying)
            sprintEffect.Stop();
    }
}

    public void TriggerWin()
    {
        winPanel.SetActive(true);
        Time.timeScale = 0f;
    }

    public void TriggerLose()
    {
        AudioManager.Instance.PlaySFX(AudioManager.SFXType.Caught);
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}