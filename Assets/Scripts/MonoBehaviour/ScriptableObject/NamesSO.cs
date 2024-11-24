using System.Linq;
using Sirenix.OdinInspector;
using Unity.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "NameDatabase", menuName = "Name Database")]
public class NamesSO : ScriptableObject
{
    public string[] FirstNames;
    public string[] LastNames;

    [Button]
    public void OrganizeNames()
    {
        FirstNames = FirstNames.Distinct().OrderBy(name => name).ToArray();
        LastNames = LastNames.Distinct().OrderBy(name => name).ToArray();
    }
}
