using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BorderScript : MonoBehaviour
{
    private void OnDestroy()
    {
        transform.parent.localPosition += new Vector3(0, 0, 0.25f);
    }
}
