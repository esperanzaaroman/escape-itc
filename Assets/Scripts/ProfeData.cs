using UnityEngine;

/// <summary>
/// Datos de configuración de cada profe.
/// Usado por MenuPrincipal.cs y ProfeSpawner.cs
/// </summary>
[System.Serializable]
public class ProfeData
{
    public string nombre;

    [TextArea(2, 3)]
    public string descripcion;

    public Sprite foto;

    [Header("Stats")]
    public float speed            = 5f;
    public int   capacidadMemoria = 5;
    public float tiempoRecalculo  = 0.5f;

    // Alias para que ProfeSpawner pueda usar d.velocidad
    public float velocidad => speed;
}
