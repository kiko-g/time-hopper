using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrenadeExplosion : MonoBehaviour
{

    public float blastRadius = 10;
    public float explosionForce = 20;

    private Collider[] hitColliders;

    void OnCollisionEnter(Collision col) {
        doExplosion(col.contacts[0].point);
        Destroy(gameObject, 1.0f);
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void doExplosion(Vector3 explosionPoint) {
        hitColliders = Physics.OverlapSphere(explosionPoint, blastRadius);

        //for each collider
        foreach (Collider hit in hitColliders) {
            //Debug.Log(hit.gameObject.name);
            if (hit.GetComponent<Rigidbody>() != null) {
                hit.GetComponent<Rigidbody>().isKinematic = false;
                hit.GetComponent<Rigidbody>().AddExplosionForce(explosionForce, explosionPoint, blastRadius, .04f , ForceMode.Impulse);
            }
        }
    }
}
