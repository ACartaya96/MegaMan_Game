using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnContrroller : MonoBehaviour
{
    public static SpawnContrroller Instance = null;
    [SerializeField] public GameObject[] spawnPoints;
    // Start is called before the first frame update
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }

    }   
  

    // Update is called once per frame
    void Update()
    {
        
    }
}
