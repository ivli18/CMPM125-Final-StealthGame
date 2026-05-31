using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public GameObject winPanel;        // Win UI panel here
    private CharacterController cc;

    // sprint handling
    public float sprintMult = 1.5f;
    public float maxStamina = 100f;
    public float drainRate = 30f;
    public float regenRate = 10f;
    private float stamina;
    public RectTransform staminaBarFill;

    void Start()
    {
        cc = GetComponent<CharacterController>();
        stamina = maxStamina;
    }

    void FixedUpdate()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        // sprint check
        bool isSprinting = Input.GetKey(KeyCode.LeftShift) && stamina > 0;
        if (isSprinting)
        {
            stamina -= drainRate * Time.deltaTime;
        } else {
            stamina += regenRate * Time.deltaTime;
        }
        stamina = Mathf.Clamp(stamina, 0, maxStamina);

        // update stamina bar
        if (staminaBarFill != null)
        {
            float fill = stamina / maxStamina;
            staminaBarFill.localScale = new Vector3(fill, 1, 1);
        }   

        float currentSpeed = moveSpeed * (isSprinting ? sprintMult : 1f);
        Vector3 move = new Vector3(h, 0, v) * currentSpeed;
        cc.Move(move * Time.deltaTime);
    }

    public void TriggerWin()
    {
        winPanel.SetActive(true);
        Time.timeScale = 0f;
    }

    public void TriggerLose()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}