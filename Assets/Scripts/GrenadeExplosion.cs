using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrenadeExplosion : MonoBehaviour
{

    public float blastRadius = 5f;
    public float explosionForce = 20f;
    public float damage = 0f;
    public GameObject explosionPrefab;

    private Collider[] hitColliders;

    void OnCollisionEnter(Collision col) {
        //Debug.Log(col.contacts[0].point.ToString());
        //instantiate explosion prefab at collision point
        Destroy(gameObject);
        GameObject explosion = Instantiate(explosionPrefab, col.contacts[0].point, Quaternion.identity);
        Destroy(explosion, 2f);
        doExplosion(col.contacts[0].point);
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
            if(hit.gameObject.tag == "Enemy"){
                // deal damage
                EnemyBehaviour enemy = hit.GetComponent<EnemyBehaviour>();
                if(enemy != null){
                    enemy.TakeDamage(damage);
                }
            }
            else if(hit.gameObject.tag == "RangedEnemy"){
                // deal damage
                RangedEnemyBehaviour ranged_enemy = hit.GetComponent<RangedEnemyBehaviour>();
                if(ranged_enemy != null){
                    ranged_enemy.TakeDamage(damage);
                }
            }
            else if(hit.gameObject.tag == "Boss"){
                // deal damage
                BossBehaviour boss = hit.GetComponent<BossBehaviour>();
                if(boss != null){
                    boss.TakeDamage(damage);
                }
            }
            else if(hit.gameObject.name == "target_test"){
                // deal damage
                TrainingTargetBehaviour target = hit.transform.parent.GetComponent<TrainingTargetBehaviour>();
                if(target != null){
                    //Debug.Log("dealing " + damage);
                    target.TakeDamage(damage);
                }
            }
            Rigidbody rb = hit.GetComponent<Rigidbody>();
            if(rb != null){
                if(!rb.isKinematic){
                    //rb.AddExplosionForce(explosionForce, explosionPoint, blastRadius, .04f, ForceMode.Impulse);
                    // verify if the object is tagged enemy
                }
            }
        }
    }
}
