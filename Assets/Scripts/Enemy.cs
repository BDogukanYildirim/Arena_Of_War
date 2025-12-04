
using System.Collections;
using UnityEngine;

public class Enemy_sc : MonoBehaviour
{
    [Header("Target")]
    public GameObject playerTarget; // Player'ı tutacağımız değişken (GameObject olarak güncellendi)

    [Header("Stats")]
    [SerializeField] int health = 2; // 2 vuruşluk can
    [SerializeField] bool isDead = false;

    [Header("Animasyon")]
    [SerializeField] bool walk;
    [SerializeField] bool run;
    [SerializeField] bool hurt;
    [SerializeField] bool attack;
    Animator anim;

    Rigidbody rb;
    BoxCollider hitCollider; // Düşmanın kendi collider'ı

    void Start()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        hitCollider = GetComponent<BoxCollider>();
        rb.freezeRotation = true;

        // Player nesnesini Tag üzerinden bul ve Ata konumuna ihtiyacımız olacak 
        GameObject foundPlayer = GameObject.FindGameObjectWithTag("Player");
        if (foundPlayer != null)
        {
            playerTarget = foundPlayer; // Artık direkt GameObject olarak atıyoruz
        }
        else
        {
            Debug.LogError("Sahne'de 'Player' tagine sahip bir obje bulunamadı!");
        }
    }

    void Update()
    {
        if (isDead) return; // Ölü ise hiçbir şey yapma

        EnemyRotate();
        ChooseAnimasyon();
    }

    void FixedUpdate()
    {
        if (isDead) return;
        
        EnemyMove();
    }

    // İsteğin üzerine şimdilik boş bırakıldı
    private void EnemyMove()
    {
        // İleride buraya NavMeshAgent veya Rigidbody hareketi yazılacak yapay zeka entegrasyonu hareket fonksiyonu gelecek 
    }

    private void EnemyRotate()
    {
        // yapay zeka eklenince düzenleyecegim
    }

    // Player'ın kılıcı çarptığında çalışır
    private void OnTriggerEnter(Collider other)
    {
        if (isDead) return;

        // Player'ın kılıcı Daha demoda player da Kılıc su anlık ayrı degil
        if (other.CompareTag("Weapon")) 
        {
            TakeDamage(1);
            Debug.Log("Düşman hasar aldı! Kalan Can: " + health);
        }
    }

    public void TakeDamage(int dmg)
    {
        health -= dmg;

        if (health <= 0)
        {
            Die();
        }
        else
        {
            // Hasar alma animasyonu (Hurt)
            StopCoroutine("CoroutineHurt");
            StartCoroutine("CoroutineHurt");
        }
    }

    void Die()
    {
        health = 0;
        isDead = true;
        anim.SetBool("die", true);
        
        // Collider'ı kapat ki öldükten sonra tekrar vurulamasın veya içinden geçilebilsin
        if(hitCollider != null) hitCollider.enabled = false;

        // 3 saniye sonra sahneden sil (Animasyonun bitmesi için süre)
        Destroy(gameObject, 3f);
    }

    IEnumerator CoroutineHurt()
    {
        hurt = true;
        // Animasyonun geçiş süresi kadar bekle
        yield return new WaitForSeconds(0.2f); 
        hurt = false;
    }

    private void ChooseAnimasyon()
    {
        // Animasyon parametrelerini güncelle
        anim.SetBool("walk", walk);
        anim.SetBool("run", run);
        anim.SetBool("hurt", hurt);
        anim.SetBool("attack", attack);
    }
}
