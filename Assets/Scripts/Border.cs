using UnityEngine;
using System.Collections;

public class Border : MonoBehaviour {


    private PoolObject Try_return;

    void OnTriggerExit2D(Collider2D other)
    {
        Try_return = other.GetComponent<PoolObject>();
        if (Try_return)
            Try_return.ReturnToPool();
        else
            Destroy(other.gameObject);
    }
}
