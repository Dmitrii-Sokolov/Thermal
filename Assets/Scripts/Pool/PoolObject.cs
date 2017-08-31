using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[AddComponentMenu("Pool/PoolObject")]
public class PoolObject : MonoBehaviour
{
    private bool end = false;
    private float endTime;

    #region Interface
    public void ReturnToPool()
    {
        end = false;
        gameObject.SetActive(false);
    }

    public void ReturnToPool(float LifeTime)
    {
        end = true;
        endTime = Time.time + LifeTime;
    }
    #endregion

    void FixedUpdate()
    {
        if (end)
        {
            if (Time.time > endTime)
            {
                end = false;
                gameObject.GetComponent<PoolObject>().ReturnToPool();
            }
        }
    }
}