using UnityEngine;
using UnityEngine.SceneManagement;

public class NextSceneTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        // Verifica si el otro objeto tiene alguno de los tags válidos
        if (other.CompareTag("Warrior") ||
            other.CompareTag("Archer") ||
            other.CompareTag("Mage") ||
            other.CompareTag("Healer"))
        {
            // Cargar la siguiente escena por índice
            int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;
            SceneManager.LoadScene(nextSceneIndex);
        }
    }
}
