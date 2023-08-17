using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;

public class SpawnOnDestroy : MonoBehaviour
{
    [SerializeField] private GameObject preFab;

    private void OnDestroy()
    {
       Instantiate(preFab, transform.position, Quaternion.identity);
    }
}
