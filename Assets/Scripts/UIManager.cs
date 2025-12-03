using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    [SerializeField] GameObject player;
    [SerializeField] GameObject SpawnManager;

    [Header("UI Panelleri")]
    [SerializeField] GameObject menuPanel;
    [SerializeField] GameObject settings;
    [SerializeField] GameObject Credits;
    [SerializeField] GameObject PhotoCam;

    [Header("Butonlar")]
    [SerializeField] Button[] buttons;

    [SerializeField] bool menuState =true;

    private Vector3 startPosition = new Vector3(50, 0, 50);

    // Oyunun başlayıp başlamadığını takip eden değişken
    private bool gameHasStarted = false;

    private void OnEnable()
    {
        if (GameManager.Instance != null)
            menuState = true;
            GameManager.OnPauseStateChanged += PauseMenuIslemleri;
    }

    private void OnDisable()
    {
        if (GameManager.Instance != null)
            menuState = false;
            GameManager.OnPauseStateChanged -= PauseMenuIslemleri;
    }

    void PauseMenuIslemleri(bool isPaused)
    {
        if (menuPanel != null)
        {
            menuState = isPaused;

            // Eğer menü açıldıysa (isPaused = true), butonları güncelle
            if (isPaused)
            {
                UpdateMenuButtons();
            }
        }

        if (!isPaused)
        {
            settings.SetActive(false);
            Credits.SetActive(false);
        }
    }

    void Start()
    {
        // Buton Dinleyicileri
        buttons[0].onClick.AddListener(StartGameButton);
        buttons[1].onClick.AddListener(Settings);
        buttons[2].onClick.AddListener(Exit);
        buttons[3].onClick.AddListener(() => StartCoroutine(CoroutineCredits()));
        buttons[4].onClick.AddListener(ExitSettings);
        buttons[5].onClick.AddListener(ResumeGame);
        buttons[6].onClick.AddListener(StartNewGameWithReload);

        // Başlangıç Ayarları
        gameHasStarted = false; // Oyun henüz başlamadı
        menuState = true;
        if (menuPanel != null) menuPanel.SetActive(true);
        if (player != null) player.SetActive(false);
        if (SpawnManager != null) SpawnManager.SetActive(false);

        // İlk açılışta butonları ayarla
        UpdateMenuButtons();
    }

    private void Update()
    {
        menuPanel.SetActive(menuState);
    }

    // --- BUTON GİZLEME/AÇMA MANTIĞI ---
    void UpdateMenuButtons()
    {
        // Çıkış butonu (buttons[2]) WebGL olduğu için hep kapalı kalsın
        buttons[2].gameObject.SetActive(false);
        
        if (!gameHasStarted)
        {
            // DURUM 1: Oyun Henüz Başlamadı (İlk Açılış)
            buttons[0].gameObject.SetActive(true);  // "Başla" AÇIK
            buttons[5].gameObject.SetActive(false); // "Devam Et" KAPALI
            buttons[6].gameObject.SetActive(false); // "Yeni Oyun" KAPALI
        }
        else
        {
            // DURUM 2: Oyun Başladı ve P ile Menüye Girildi
            buttons[0].gameObject.SetActive(false); // "Başla" KAPALI
            buttons[5].gameObject.SetActive(true);  // "Devam Et" AÇIK
            buttons[6].gameObject.SetActive(true);  // "Yeni Oyun" AÇIK
        }
    }

    public void ResumeGame()
    {
        // Sadece Pause durumunu değiştiriyoruz.
        // Event sistemi (PauseMenuIslemleri) otomatik olarak menüyü kapatacak.
        GameManager.Instance.TogglePause();
    }

    // Bu, Button 0 (Başla) fonksiyonu
    public void StartGameButton()
    {
        // Oyunu başlattığımızı işaretliyoruz
        gameHasStarted = true;
        menuState = false;
        player.transform.position = startPosition;

        // Görsel objeleri aç
        if (PhotoCam != null) PhotoCam.SetActive(false);
        if (player != null) player.SetActive(true);
        if (SpawnManager != null) SpawnManager.SetActive(true);

        // Menüyü kapat
        if (menuPanel != null) menuPanel.SetActive(false);
    }

    public void StartNewGameWithReload()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void Settings()
    {
        settings.SetActive(true);
    }

    public void ExitSettings()
    {
        settings.SetActive(false);
    }

    public void Exit()
    {
        Application.Quit();
    }

    IEnumerator CoroutineCredits()
    {
        Credits.SetActive(true);
        yield return new WaitForSeconds(2.5f);
        Credits.SetActive(false);
    }
}