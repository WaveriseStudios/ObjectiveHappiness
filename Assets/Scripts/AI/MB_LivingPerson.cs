using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MB_LivingPerson : MonoBehaviour
{
    [SerializeField] private SO_Person personData;

    [SerializeField] private int currentEnergy = 100;
    [SerializeField] private int maxEnergy = 100;

    [SerializeField] private int currentHunger = 0;
    [SerializeField] private int maxHunger = 100;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
