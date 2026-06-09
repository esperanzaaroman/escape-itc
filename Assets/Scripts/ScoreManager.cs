using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; }

    [Header("Monedas para ganar")]
    public int monedasParaGanar = 5;

    public static event System.Action OnJugadorGana;
    public static event System.Action OnProfeGana;

    public int MonedasJugador { get; private set; } = 0;
    public int MonedasProfe   { get; private set; } = 0;

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
    }

    public void RegistrarMonedaJugador()
    {
        MonedasJugador++;
        if (MonedasJugador >= monedasParaGanar)
            OnJugadorGana?.Invoke();
    }

    public void RegistrarMonedaProfe()
    {
        MonedasProfe++;
        if (MonedasProfe >= monedasParaGanar)
            OnProfeGana?.Invoke();
    }
}