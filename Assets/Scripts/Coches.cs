using System.Collections.Generic;
using UnityEngine;

public class Coches : MonoBehaviour
{
    public GameObject Coche;
    public int NumeroCoches = 10;
    List<GameObject> losCoches;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        float x, z, y = 0f;
        int i;
        
        losCoches = new List<GameObject>();
        for (i=0; i < NumeroCoches; i++)
        {
            x = Random.Range(-10, 10);
            z = Random.Range(-10, 10);
            losCoches.Add(Instantiate(Coche, new Vector3(x, y, z), Quaternion.identity));
        }
    }

    // Update is called once per frame
    void Update()
    {

        // Rotar cada coche alrededor del eje Y
        foreach (GameObject coche in losCoches)
        {
            coche.transform.Rotate(0, 50 * Time.deltaTime, 0); // 50 grados por segundo
        }
    
    }
}
