using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PastIndicatorController : MonoBehaviour
{
    public void Rewind()
    {   
        DataHub.Instance.DataHubRewind();
    }
}
