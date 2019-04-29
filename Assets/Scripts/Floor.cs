using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Floor : MonoBehaviour
{
    public float disAmount = 0.1f;
    public GameObject target;
    Vector3 distance;
    float killDist;
    Vector3 destroyDistance;
    private Renderer mat;
    
    private void Start()
    {
        mat = GetComponent<Renderer>();
        mat.material.SetTextureScale("_MainTex", new Vector2(Random.Range(-2f, -1f), Random.Range(1f, 2f)));
    }
    
    private void Update()
    {
        killDist = target.transform.position.x - transform.position.x;
        if (killDist > 0)
        {
            distance = target.transform.position - transform.position;
            disAmount = (distance.magnitude / 2f) / 10;
            mat.material.SetFloat("_Level", disAmount);
        }


        if(killDist > 11)
        {
            Destroy(gameObject);
        }
    }
}
