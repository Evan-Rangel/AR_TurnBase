using UnityEngine;
using System.Collections;

public class EnemyDeathEndGame : MonoBehaviour
{
    [Tooltip("Referencia al componente que lleva la vida")]
    public HealthSystem healthSystem;

    [Tooltip("Segundos que se esperan antes de cerrar el juego")]
    public float delay = 5f;

    private void Awake()
    {
        if (healthSystem == null)
            healthSystem = GetComponent<HealthSystem>();
    }

    private void OnEnable()
    {
        if (healthSystem != null)
            healthSystem.OnDead += HandleDeath;
    }

    private void OnDisable()
    {
        if (healthSystem != null)
            healthSystem.OnDead -= HandleDeath;
    }

    private void HandleDeath(object sender, System.EventArgs e)
    {
        StartCoroutine(EndGameAfterDelay(delay));
    }

    private IEnumerator EndGameAfterDelay(float seconds)
    {
        yield return new WaitForSeconds(seconds);

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;   // Detiene el Play Mode
#else
        Application.Quit();                                // Cierra la app compilada
#endif
    }
}
