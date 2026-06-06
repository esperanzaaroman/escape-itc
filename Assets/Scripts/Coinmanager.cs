using UnityEngine;
using System.Collections.Generic;

public class CoinManager : MonoBehaviour
{
    [Header("IA — reasignación automática de objetivo")]
    public List<profe> profes = new List<profe>();

    [Header("HUD (opcional — se busca automáticamente si se deja vacío)")]
    public GameHUD hud;

    public static event System.Action OnAllCoinsCollected;

    private List<coin> _remaining = new List<coin>();
    private int _total     = 0;
    private int _collected = 0;

    public int Score      => _collected;
    public int TotalCoins => _total;

    void Awake()
    {
        foreach (coin c in FindObjectsOfType<coin>())
            _remaining.Add(c);
        _total = _remaining.Count;

        if (hud == null)
            hud = FindObjectOfType<GameHUD>();
    }

    void OnEnable()  => coin.OnCoinCollected += HandleCoinCollected;
    void OnDisable() => coin.OnCoinCollected -= HandleCoinCollected;

    void Start() => hud?.ActualizarMonedas(_collected, _total);

    private void HandleCoinCollected(coin recogida)
    {
        _collected++;
        _remaining.Remove(recogida);
        hud?.ActualizarMonedas(_collected, _total);

        if (_remaining.Count == 0)
        {
            OnAllCoinsCollected?.Invoke();
            return;
        }

        foreach (profe p in profes)
            if (p != null && p.objetivo == recogida)
                p.objetivo = GetNearestCoin(p.transform.position);
    }

    public coin GetNearestCoin(Vector3 desde)
    {
        coin nearest  = null;
        float minDist = float.MaxValue;
        foreach (coin c in _remaining)
        {
            if (c == null) continue;
            float d = Vector3.Distance(desde, c.transform.position);
            if (d < minDist) { minDist = d; nearest = c; }
        }
        return nearest;
    }

    public List<coin> GetRemainingCoins() => _remaining;
}