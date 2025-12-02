using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum JobType
{
    None,
    Bucheron,
    Recolteur,
    Mineur,
    Maçon,
}


[CreateAssetMenu(fileName = "New Person", menuName = "ScriptableObjects/People", order = 1)]
public class SO_Person : ScriptableObject
{
    public string currentName = "Nom";
    public JobType currentJob = JobType.None;
    
}
