using UnityEngine;
using TMPro;
using UnityEngine.AI;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class BossBehaviour : MonoBehaviour
{
    private Transform playerTransform;

    private Animator animator;

    private float moveSpeed;

    Vector3 offset;

    [Header("Enemy Stats")]
    public float baseHealth = 100f;
    public float baseDamage = 5f;
    public float healthIncreasePerRound = 5f;
    public float damageIncreasePerRound = 2f;
    private float health;
    private float damage;
    public int dropPercentage;

    bool dropped = false, alreadyAttacked = false, registeredHit = false;
    
    [SerializeField]
    private GameObject currencyPrefab;

    private GameObject currencyHolder;
    
    private GameObject enemies;

    private NavMeshAgent navMeshAgent;

    public ObstacleAvoidanceType AvoidanceType;

    // textmeshprougui with damage on hit
    [SerializeField]
    private TextMeshProUGUI damageText;

    private bool enableNavMesh = false;

    private Rigidbody body;

    [SerializeField]
    private ParticleSystem slamParticles;

    private bool arrivalSlam = false, alreadySlammed = false, choseAttack = false;
    private bool slammedGround = false, pain = false;
    private int rng = -1;

    private float elapsedTime = 0f;
    private float upDir =  0f;

    [SerializeField]
    private GameObject bulletPrefab;
    private Vector3 bulletOffset = new Vector3(-0.85f, 1.6f, 0.4f);
    [SerializeField]
    private Slider bossHealth;
    private bool inPain1 = false, inPain2 = false;
    private string colliseum_base = "footstep_coliseu_boss_";
    private string factory_base = "footstep_factory_boss_";
    private string forest_base = "footstep_newworld_boss_";

    private string footstepsBase;

    // Start is called before the first frame update
    void Start()
    {
        body = GetComponent<Rigidbody>();
        //navMeshAgent = GetComponent<NavMeshAgent>();
        health = baseHealth;
        damage = baseDamage;
        playerTransform = GameObject.Find("PlayerArmature").transform;
        animator = GetComponentInChildren<Animator>();
        currencyHolder = GameObject.Find("CurrencyHolder");
        bossHealth = GameObject.FindGameObjectWithTag("BossStats").GetComponent<Slider>();
        bossHealth.value = 100;

        // Generate random float move speed between 1 and 3 with different random seed for each enemy
        moveSpeed = 4f;

        switch(SceneManager.GetActiveScene().name)
        {
            case "Colliseum":
                footstepsBase = colliseum_base;
                break;
            case "Factory":
                footstepsBase = factory_base;
                break;
            case "Forest":
                footstepsBase = forest_base;
                break;
            default:
                footstepsBase = colliseum_base;
                break;
        }
        
    }

    void initNavMeshAgent()
    {
        navMeshAgent.obstacleAvoidanceType = AvoidanceType;
        navMeshAgent.avoidancePriority = Random.Range(0, 100);
        navMeshAgent.speed = 2.5f;
    }

    void Step(){
        int id = Random.Range(1, 3);
        FMODUnity.RuntimeManager.PlayOneShot("event:/Project/Character Related/Footstep/" + footstepsBase + id, transform.position);
    }
    
    // Update is called once per frame
    void Update()
    {   
        if (transform == null)
            return;

        checkForPain();

        if (navMeshAgent != null)
        {
            // if animation is not run, then movespeed is 0
            if (!animator.GetCurrentAnimatorStateInfo(0).IsName("Run"))
            {
                moveSpeed = 0f;
                navMeshAgent.speed = moveSpeed;
            }
            else
            {
                moveSpeed = 4f;
                navMeshAgent.speed = moveSpeed;
            }
        }

        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Armature|Zombie Pain"))
        {
            animator.SetBool("in_pain", false);
        }

        if (!arrivalSlam && transform.position.y < 3.5f)
        {
            SlamAttack();
            arrivalSlam = true;
        }

        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Armature|Zombie Slam") && animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.3f)
        {
            if(!slammedGround)
            {
                slamParticles.Play();
                slammedGround = true;

                if (Vector3.Distance(transform.position, playerTransform.position) <= 8)
                {   
                    playerTransform.GetComponent<StarterAssets.ThirdPersonController>().TakeDamage(50 - Vector3.Distance(transform.position, playerTransform.position)*5, "Zombie");
                }
            }
        }
        else
        {
            animator.SetBool("slamming", false);
            animator.SetBool("is_running", true);
            alreadySlammed = false;
        }

        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Armature|Zombie scream"))
        {
            transform.LookAt(playerTransform);
            upDir = transform.forward.y;
            if(animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.35f && animator.GetCurrentAnimatorStateInfo(0).normalizedTime <= 0.65f){
                Vector3 offset = new Vector3(-0.4f, 0, 0);
                Vector3 dest = transform.forward;
                dest.y = upDir;
                Transform pos = GameObject.FindGameObjectWithTag("BossHead").transform;

                GameObject bullet = Instantiate(bulletPrefab, pos.position, new Quaternion(0, 0, 0, 0));
                //bullet.transform.rotation = pos.rotation;
                bullet.transform.Translate(offset);
                //bullet.transform.LookAt(playerTransform);
                //bullet.transform.Translate(bullet.transform.forward);
                bullet.GetComponent<BulletBehaviour>().setDamage(damage);
                bullet.GetComponent<Rigidbody>().mass = 0.2f;

                //bullet.GetComponent<Rigidbody>().velocity = transform.forward * 3f;
                bullet.GetComponent<Rigidbody>().AddForce(dest * 200f);
            }
            
            if(animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.65f)
            {
                elapsedTime = 0f;
            }
        }

        if (transform.position.y <= 3 && !enableNavMesh && IsOnNavMesh()){
            gameObject.AddComponent<NavMeshAgent>();
            navMeshAgent = GetComponent<NavMeshAgent>();
            initNavMeshAgent();
            enableNavMesh = true;
            body.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
            body.isKinematic = true;
        }

        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Death")){
            if (enableNavMesh)
                navMeshAgent.enabled = false;
            moveSpeed = 0;
            float animTime = animator.GetCurrentAnimatorStateInfo(0).length;
            if(animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.2f){
                DropCurrency();
            }
            Destroy(transform.parent.gameObject, animTime - 0.5f);
            return;
        }

        if (animator.GetCurrentAnimatorClipInfo(0)[0].clip.name.Contains("atack"))
        {
            if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime <= 0.1f )
            {
                registeredHit = false;
            }
            
            if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f)
            {
                alreadyAttacked = false;
                registeredHit = false;
            }
            else if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.4f && !registeredHit)
            {
                if (Vector3.Distance(transform.position, playerTransform.position) < 1.5)
                {
                    playerTransform.GetComponent<StarterAssets.ThirdPersonController>().TakeDamage(damage, "Zombie");
                }
                registeredHit = true;
                animator.SetBool("is_attacking", false);
                animator.SetBool("is_running", true);
            }
        }

        animator.speed = 1;
        if (animator.GetCurrentAnimatorClipInfo(0)[0].clip.name.Contains("Run") && moveSpeed > 0)
        {
            animator.speed = moveSpeed / 3.0f;
        }

        if (!animator.GetCurrentAnimatorClipInfo(0)[0].clip.name.Contains("Slam") && !animator.GetCurrentAnimatorClipInfo(0)[0].clip.name.Contains("scream") && !animator.GetBool("in_pain"))
        {
            if (elapsedTime > 2.0f)
            {
                Scream();
            }
            else if (Vector3.Distance(transform.position, playerTransform.position) > 1.5){
                if (enableNavMesh){
                    navMeshAgent.enabled = true;
                    navMeshAgent.SetDestination(playerTransform.position);
                    //Debug.Log("NavMeshAgent: " + navMeshAgent.enabled);
                }
                            
                transform.LookAt(playerTransform);

                Vector3 eulerAngles = transform.rotation.eulerAngles;
                eulerAngles = new Vector3(0, eulerAngles.y, 0);
                transform.rotation = Quaternion.Euler(eulerAngles);

                if (!enableNavMesh)
                    transform.position += transform.forward * moveSpeed * Time.deltaTime;

                animator.SetBool("is_running", true);
                animator.SetBool("is_attacking", false);
                rng = -1;
                
                elapsedTime += Time.deltaTime;
            } else {
                if (enableNavMesh)
                    navMeshAgent.enabled = false; 
                animator.SetBool("is_running", false);

                Attack();
            }
        }
        else 
        {
            animator.SetBool("is_running", false);
        }

        // damage text position equal to transform position with y offset of 0.5
        damageText.transform.position = new Vector3(transform.position.x, transform.position.y + 0.6f, transform.position.z);

        // damage text rotation equal to transform rotation with 180 degree offset
        damageText.transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles + new Vector3(0, 180, 0));

        //get all objects with tag bossbullet
        GameObject[] bullets = GameObject.FindGameObjectsWithTag("BossBullet");

        //iterate bullets
        foreach (GameObject bullet in bullets)
        {
            bullet.transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles);
        }
    
        slamParticles.gameObject.transform.position = new Vector3(transform.position.x, transform.position.y + 0.2f, transform.position.z);
    }

    void checkForPain()
    {
        if (health <= 2000 && !inPain1)
        {
            animator.SetBool("in_pain", true);
            animator.SetBool("is_running", false);
            inPain1 = true;
        }
        else if (health <= 1000 && !inPain2)
        {
            animator.SetBool("in_pain", true);
            animator.SetBool("is_running", false);
            inPain2 = true;
        }
    }

    public void Attack()
    {
        if (rng <= -1)
        {
            //rng = Random.Range(1, 101);
            rng = 1;
        }

        StarterAssets.ThirdPersonController player = playerTransform.GetComponent<StarterAssets.ThirdPersonController>();
        
        if(rng <= 10 + player.waveSpawner.roundNr * 10){
            animator.SetBool("slamming", true);
            SlamAttack();
        }
        else
            animator.SetBool("is_attacking", true);
    }

    public void Scream()
    {
        animator.SetBool("screaming", true);
        animator.SetBool("is_running", false);
    }

    public void SlamAttack()
    {
        if (alreadySlammed) return;
        slamParticles.Clear();
        slammedGround = false;
        animator.SetBool("slamming", true);
        alreadySlammed = true;
    }

    public bool checkIfInPain()
    {
        if (animator.GetBool("in_pain") && !animator.GetBool("is_running"))
        {
            return true;
        }

        return false;
    }

    public void TakeDamage(float damage)
    {
        damage = 500;
        if (checkIfInPain())
        {
            damage = 0;
        }
        
        if(!animator.GetBool("is_dead")){
            health -= damage;

            // change text o textmeshprougui with damage on hit
            damageText.text = damage.ToString();
            damageText.GetComponent<Animator>().Play("EnemyDamageOnHit", -1, 0f);

            bossHealth.value = health / 30.0f;

            if (health <= 0){
                Die();
            }
        }
    }

    void OnParticleCollision(){
            TakeDamage(1);
    }

    private void Die()
    {
        moveSpeed = 0;
        animator.SetBool("is_dead", true);
        // deactivate the colliders
        GetComponent<CapsuleCollider>().enabled = false;
        GetComponent<SphereCollider>().enabled = false;
        GetComponent<Rigidbody>().isKinematic = true;
        StarterAssets.ThirdPersonController player = playerTransform.GetComponent<StarterAssets.ThirdPersonController>();
        player.AddWeaponCurrency(10);
        player.waveSpawner.decreaseEnemiesToDefeat();
        player.waveSpawner.clearAllEnemies();
        player.waveSpawner.resetRound();

        animator.Play("Death", -1, 0f);
        GameObject bossStats = GameObject.FindGameObjectWithTag("BossStats");
        bossStats.GetComponent<Animator>().Play("BossStatsFadeOut", -1, 0f);
    }

    private void DropCurrency(){
        if(!dropped){
            dropped = true;
            int dropRng = Random.Range(1, 101);
            if(dropRng <= dropPercentage){
                Vector3 spawnPos = new Vector3(transform.position.x, transform.position.y + 2.0f, transform.position.z);
                GameObject currency = Instantiate(currencyPrefab, spawnPos, new Quaternion(0, 0, 0, 0));
                Debug.Log("Dropped currency");
                currency.transform.SetParent(currencyHolder.transform);
            }
        }
    }

    //on collision enter, if tag is "Player" then ignore collision
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player" || collision.gameObject.tag == "BossBullet")
        {
            Physics.IgnoreCollision(collision.collider, GetComponent<Collider>());
        }
    }

    public bool IsOnNavMesh()
    {
        NavMeshHit hit;

        // Check for nearest point on navmesh to agent, within onMeshThreshold
        if (NavMesh.SamplePosition(transform.position, out hit, 1f, NavMesh.AllAreas))
        {
            // Check if the positions are vertically aligned
            if (Mathf.Approximately(transform.position.x, hit.position.x)
                && Mathf.Approximately(transform.position.z, hit.position.z))
            {
                // Lastly, check if object is below navmesh
                return transform.position.y >= hit.position.y;
            }
        }

        return false;
    }

    public void setStats(int roundNum){
        health = baseHealth + healthIncreasePerRound * (roundNum - 1);
        damage = baseDamage + damageIncreasePerRound * (roundNum - 1);
    }
}
