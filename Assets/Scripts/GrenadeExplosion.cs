using UnityEngine;

public class GrenadeExplosion : MonoBehaviour
{

    public float blastRadius = 3f;
    public float explosionForce = 20f;
    public float damage = 0f;
    public GameObject explosionPrefab;

    private Collider[] hitColliders;

    void OnCollisionEnter(Collision col) {
        //instantiate explosion prefab at collision point
        Destroy(gameObject);
        GameObject explosion = Instantiate(explosionPrefab, col.contacts[0].point, Quaternion.identity);
        Destroy(explosion, 2f);
        doExplosion(col.contacts[0].point);
    }

    void doExplosion(Vector3 explosionPoint) {
        FMODUnity.RuntimeManager.PlayOneShot("event:/Project/Objects/Guns/explosion_gun4", transform.position);
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
                    target.TakeDamage(damage);
                }
            }
            Rigidbody rb = hit.GetComponent<Rigidbody>();
            if(rb != null){
                if(!rb.isKinematic){
                    // verify if the object is tagged enemy
                }
            }
        }
    }
}
