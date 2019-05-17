using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class particleDeleter : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Delete();
    }

    IEnumerator Delete()
    {
        yield return new WaitForSeconds(1.0f);
        Destroy(gameObject);

    }
}
