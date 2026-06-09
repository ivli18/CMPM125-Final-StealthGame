using UnityEngine;
using UnityEngine.SceneManagement;

public class WinItem : MonoBehaviour
{
    [SerializeField] private string nextScene;
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
        {
            return;
        }
        if (!string.IsNullOrEmpty(nextScene))
            {
                SceneManager.LoadScene(nextScene);
            }
            else
            {
                other.GetComponent<PlayerController>().TriggerWin();
            }
        Destroy(gameObject);
    }
}