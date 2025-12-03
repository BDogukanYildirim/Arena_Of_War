using UnityEngine;
using System;
using UnityEngine.UI; // Action için gerekli

public class GameManager : MonoBehaviour
{
    // Singleton: Her yerden GameManager.Instance diyerek ulaşmak için
    public static GameManager Instance;

    // Olay Sistemi: Abone olanlara "Oyun Durdu mu?" (bool) bilgisini yollar
    public static event Action<bool> OnPauseStateChanged;

    private bool isPaused = false;

    AudioSource audio;
    [SerializeField]  Slider slider;

    private void Start()
    {
        audio = GetComponent<AudioSource>();
        audio.Play();
    }
    void Awake()
    {
        // Singleton ayarı
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    void Update()
    {
        audio.volume = slider.value;
        // ESC tuşu kontrolü sadece burada yapılır
        if (Input.GetKeyDown(KeyCode.P))
        {
            TogglePause();
        }
    }

    public void TogglePause()
    {
        isPaused = !isPaused;
        Debug.Log("UIManager: Menu Panel açılmaya/kapanmaya çalışıldı.");

        // 1. Zamanı durdur veya devam ettir
        // TimeScale 0 olunca SpawnManager'daki WaitForSeconds da donar.
        Time.timeScale = isPaused ? 0 : 1;

        // 2. Abone olan herkese (UI ve Player) haber ver
        OnPauseStateChanged?.Invoke(isPaused);
    }

    // Oyun bittiğinde veya sahne değiştiğinde zamanın takılı kalmaması için
    private void OnDestroy()
    {
        Time.timeScale = 1;
    }
}