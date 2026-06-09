using UnityEngine;

public class ProfeSpawner : MonoBehaviour
{
    [Header("GameObjects de los 3 profes en escena")]
    public GameObject[] profeObjects = new GameObject[3];

    [Header("Datos de stats (mismo array que ProfeSelectionMenu)")]
    public ProfeData[] profeDatos = new ProfeData[3];

    void Awake()
    {
        int elegido = GameConfig.ProfeElegido;

        for (int i = 0; i < profeObjects.Length; i++)
        {
            if (profeObjects[i] == null) continue;

            bool activo = (i == elegido);
            profeObjects[i].SetActive(activo);

            // Aplicar stats del profe elegido
            if (activo && profeDatos != null && profeDatos.Length > i)
            {
                profe p = profeObjects[i].GetComponent<profe>();
                ProfeData d = profeDatos[i];

                if (p != null && d != null)
                {
                    p.speed                  = d.velocidad;
                    p.capacidadMemoria       = d.capacidadMemoria;
                    p.tiempoEntreRecalculos  = d.tiempoRecalculo;
                }
            }
        }
    }
}