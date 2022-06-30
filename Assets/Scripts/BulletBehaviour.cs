
using UnityEngine;

public class BulletBehaviour : MonoBehaviour
{

    private StarterAssets.ThirdPersonController player;

    private float damage = 0f;

    private float livingTime = 3f;
    private float currentTime = 0f;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<StarterAssets.ThirdPersonController>();
    }

    void Update()
    {
        currentTime += Time.deltaTime;
        if (currentTime >= livingTime)
        {
            Destroy(this.gameObject);
        }
    }

    public void setDamage(float d){
        damage = d;
    }

    void OnTriggerEnter(Collider other){
        if (other.gameObject.tag == "RangedEnemy" || other.gameObject.tag == "Boss" || other.gameObject.tag == "Enemy" || other.gameObject.tag == "BossHead" || other.gameObject.tag == "BossBullet"){
            return;
        } else if (other.gameObject.tag == "Player"){
            player.TakeDamage(damage, "Robo");
            Destroy(this.gameObject);
        } else {
            Destroy(this.gameObject);
        }
    }
}
