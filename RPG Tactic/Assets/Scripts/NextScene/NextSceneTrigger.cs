using UnityEngine;
using UnityEngine.SceneManagement;

public class NextSceneTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        // Verifica si el otro objeto tiene alguno de los tags v�lidos
        if (other.CompareTag("Warrior") ||
            other.CompareTag("Archer") ||
            other.CompareTag("Mage") ||
            other.CompareTag("Healer"))
        {
            // Cargar la siguiente escena por �ndice
            int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;
            SceneManager.LoadScene(nextSceneIndex);
        }
    }
}
