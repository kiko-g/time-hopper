using System.Collections;
using UnityEngine.UI;
using UnityEngine;

public class GunBehaviour : MonoBehaviour
{

    public float damage;

    public float propulsionForce;

    public float range;

    public int weaponPrice;

    public int ammoPrice;

    public float shootingSpread;

    public int numBulletsPerMagazine;

    public int defaultNumberOfExtraMagazines;

    public int currentAmmo;

    public int availableAmmo;

    private string availableAmmoString = "0";

    public bool is_pistol;

    public float fireRateTimer;

    private float lastShotTime;

    public float reloadTime;

    public bool reloading = false;

    private float reloadStart;
    private StarterAssets.ThirdPersonController player;
    
    private Transform cam;

    //[FMODUnity.EventRef]
    //public string ShootingEvent = "";

    private Vector2 screenCenter;

    public ParticleSystem[] impactEffects;

    [SerializeField]
    private ParticleSystem muzzleFlash;

    [SerializeField]
    private Text currentAmmoUI;
 
    [SerializeField]
    private Text availableAmmoUI;

    [SerializeField]
    private LayerMask aimColliderMask = new LayerMask();

    WeaponRecoil recoil;

    [Header("Bullet")]
    public GameObject bullet;
    public GameObject grenadePrefab;
    public Transform muzzle;
    public float fadeDuration = 0.2f;


    [Header("Shotgun")]
    public int numBulletsPerShot;
    public bool is_shotgun;
    public bool is_rocketlauncher;
    public float innacuracyDistance;

    public string gun_sound = "shot_gun_1";
    
    void Start()
    {
        recoil = GetComponent<WeaponRecoil>();
        //availableAmmoString = "0";
        if(is_pistol){
            availableAmmoString = "∞";
            FillAmmo();
        }
        updateReloadUI();
        screenCenter = new Vector2(Screen.width / 2f, Screen.height / 2f);
        cam = Camera.main.transform;
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<StarterAssets.ThirdPersonController>();
    }

    void Update(){
        if(reloading){
            if(Time.time - reloadStart >= reloadTime){
                reloading = false;
                finishReload();
            }
        }
    }

    public void setGunSound(string gunSound){
        gun_sound = gunSound;
    }

    public void Shoot(Vector2 shootingSpreadVec)
    {
        if (currentAmmo <= 0 || reloading){
            return;
        }
        if(Time.time - lastShotTime <= fireRateTimer){
            return;
        }
        lastShotTime = Time.time;
        currentAmmo--;
        muzzleFlash.Play();
        FMODUnity.RuntimeManager.PlayOneShot("event:/Project/Objects/Guns/" + gun_sound, transform.position);
        recoil.TriggerRecoil();
        Ray ray = Camera.main.ScreenPointToRay(screenCenter + shootingSpreadVec);
        if (is_shotgun){
            for (int i = 0; i < numBulletsPerShot; i++){ 
                RaycastHit hit;
                Vector3 shootingDir = GetShootingDirection();
                if (Physics.Raycast(cam.position, shootingDir, out hit, range, aimColliderMask)){
                    ParticleSystem effect = Instantiate(impactEffects[0], hit.point, Quaternion.identity);
                    effect.transform.forward = -transform.forward;
                    Destroy(effect.gameObject, effect.main.duration);
                    if (hit.transform.tag == "Enemy"){
                        float actualDamage = damage;
                        if (hit.collider.GetType() == typeof(SphereCollider)){
                            actualDamage *= 2;
                        }
                        EnemyBehaviour enemy = hit.transform.GetComponent<EnemyBehaviour>();
                        if (enemy != null){
                            enemy.TakeDamage(actualDamage);
                        }
                    } else if (hit.transform.tag == "RangedEnemy"){
                        float actualDamage = damage;
                        if (hit.collider.GetType() == typeof(SphereCollider)){
                            actualDamage *= 2;
                        }
                        RangedEnemyBehaviour enemy = hit.transform.GetComponent<RangedEnemyBehaviour>();
                        if (enemy != null){
                            enemy.TakeDamage(actualDamage);
                        }
                    } else if(hit.transform.name == "target_test"){
                        TrainingTargetBehaviour target = hit.transform.parent.GetComponent<TrainingTargetBehaviour>();
                        if (target != null){
                            target.TakeDamage(damage);
                        }
                    } else if(hit.transform.tag == "Boss"){
                        float actualDamage = damage;
                        if (hit.collider.GetType() == typeof(SphereCollider)){
                            actualDamage *= 2;
                        }
                        BossBehaviour enemy = hit.transform.GetComponent<BossBehaviour>();
                        if (enemy != null){
                            enemy.TakeDamage(actualDamage);
                        }
                    }
                }
                else{
                    CreateBullet(cam.position + shootingDir * range);
                }   
            }
        }
        else if (is_rocketlauncher) {
            //instantiate grenade
            GameObject grenadeInstance = Instantiate(grenadePrefab, muzzle.position, muzzle.rotation);
            //addForce
            grenadeInstance.GetComponent<GrenadeExplosion>().damage = damage;
            RaycastHit hit;
            if(Physics.Raycast(ray, out hit, range, aimColliderMask)){
                //add force to the hit direction
                Vector3 direction = hit.point - grenadeInstance.transform.position;
                grenadeInstance.GetComponent<Rigidbody>().AddForce(direction.normalized * propulsionForce, ForceMode.Impulse);
            }
        }
        else{
            RaycastHit hit;
        
            if (Physics.Raycast(ray, out hit, range, aimColliderMask)){
                ParticleSystem effect = Instantiate(impactEffects[0], hit.point, Quaternion.identity);
                effect.transform.forward = hit.normal;
                Destroy(effect.gameObject, effect.main.duration);

                if (hit.transform.tag == "Enemy"){
                    float actualDamage = damage;
                    if (hit.collider.GetType() == typeof(SphereCollider)){
                        actualDamage *= 2;
                    }
                    if(hit.collider.GetType() == typeof(BoxCollider)){
                        actualDamage = 0;
                    }
                    EnemyBehaviour enemy = hit.transform.GetComponent<EnemyBehaviour>();
                    if (enemy != null){
                        enemy.TakeDamage(actualDamage);
                    }
                } else if (hit.transform.tag == "RangedEnemy"){
                    float actualDamage = damage;
                    if (hit.collider.GetType() == typeof(SphereCollider)){
                        actualDamage *= 2;
                    }
                    RangedEnemyBehaviour enemy = hit.transform.GetComponent<RangedEnemyBehaviour>();
                    if (enemy != null){
                        enemy.TakeDamage(actualDamage);
                    }
                } else if(hit.transform.tag == "Boss"){
                    float actualDamage = damage;
                    if (hit.collider.GetType() == typeof(SphereCollider)){
                        actualDamage *= 2;
                    }
                    BossBehaviour enemy = hit.transform.GetComponent<BossBehaviour>();
                    if (enemy != null){
                        enemy.TakeDamage(actualDamage);
                    }
                } else if(hit.transform.name == "target_test"){
                    TrainingTargetBehaviour target = hit.transform.parent.GetComponent<TrainingTargetBehaviour>();
                    if (target != null){
                        target.TakeDamage(damage);
                    }
                }
                CreateBullet(hit.point);
            }
            else{
                CreateBullet(cam.position + ray.direction * range);
            }
        }
        updateReloadUI();
        if(currentAmmo == 0){
            Reload();
        }
    }

    public void updateReloadUI()
    {
        currentAmmoUI.text = currentAmmo.ToString();
        availableAmmoUI.text = availableAmmoString;
    }

    public void Reload()
    {
        if(!reloading){
            if(currentAmmo < numBulletsPerMagazine){
                reloadStart = Time.time;
                reloading = true;
            }
        }
    }

    private void finishReload(){
        if(is_pistol){
            currentAmmo = numBulletsPerMagazine;
        }
        else{
            int ammoNeeded = numBulletsPerMagazine - currentAmmo;
            if (ammoNeeded <= availableAmmo){
                availableAmmo -= ammoNeeded;
                currentAmmo += ammoNeeded;
            } else {
                currentAmmo += availableAmmo;
                availableAmmo = 0;
            }
            availableAmmoString = availableAmmo.ToString();
        }
        player._animator.SetBool("Reloading", false);
        updateReloadUI();
    }

    public bool BuyAmmo(int ammount){
        if(numBulletsPerMagazine * defaultNumberOfExtraMagazines >= availableAmmo + ammount){
            availableAmmo += ammount;
        }
        else{
            return false;
        }
        availableAmmoString = availableAmmo.ToString();
        updateReloadUI();
        return true;
    }

    public void FillAmmo(){
        //Debug.Log("Filling Ammo...");
        currentAmmo = numBulletsPerMagazine;
        if(!is_pistol){
            availableAmmo = numBulletsPerMagazine * defaultNumberOfExtraMagazines;
            availableAmmoString = availableAmmo.ToString();
        }
        updateReloadUI();
    }

    Vector3 GetShootingDirection(){
        Vector3 targetPos = cam.position + cam.forward * range;
        if (!is_rocketlauncher) {
            targetPos = new Vector3(
                targetPos.x + Random.Range(-innacuracyDistance, innacuracyDistance),
                targetPos.y + Random.Range(-innacuracyDistance, innacuracyDistance),
                targetPos.z + Random.Range(-innacuracyDistance, innacuracyDistance)
            );
        }

        Vector3 direction = targetPos - cam.position;
        return direction.normalized;
    }


    void CreateBullet(Vector3 end){
        LineRenderer lr = Instantiate(bullet).GetComponent<LineRenderer>();
        lr.SetPositions(new Vector3[2] {muzzle.position, end});
        StartCoroutine(FadeBullet(lr));
    }


    IEnumerator FadeBullet(LineRenderer lr){
        float alpha = 1;
        while(alpha > 0){
            alpha -= Time.deltaTime / fadeDuration;
            lr.startColor = new Color(lr.startColor.r, lr.startColor.g, lr.startColor.b, alpha);
            lr.endColor = new Color(lr.endColor.r, lr.endColor.g, lr.endColor.b, alpha);
            if (alpha <= 0){
                Destroy(lr.gameObject);
            }
            yield return null;
        }
    }
}
