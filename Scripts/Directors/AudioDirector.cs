using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class AudioDirector : MonoBehaviour
{
    public static AudioDirector Instance;

    [Header("Listener")]
    public AudioListener listener;          // Persistent listener in RootScene
    private Transform earTarget;            // Found in each scene (tagged "Ear")
    public float followSmooth = 15f;        // How smoothly listener follows

    [Header("Music")]
    public AudioSource mainMenuMusic;
    public AudioSource dayMusic;
    public AudioSource nightMusic;

    [Header("Settings")]
    public float fadeDuration = 2f;

    // Track ongoing fade coroutines per source
    private Coroutine mainMenuFade;
    private Coroutine dayFade;
    private Coroutine nightFade;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void Start()
    {
        FindEarTarget();
        UpdateMusicState();
    }

    private void LateUpdate()
    {
        FollowEarTarget();
    }

    // --------------------------------------------------------
    //   FIND EAR TARGET
    // --------------------------------------------------------
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        FindEarTarget();
        UpdateMusicState();   // Fade correctly when changing scenes
    }

    void FindEarTarget()
    {
        GameObject earObj = GameObject.FindGameObjectWithTag("Ear");

        if (earObj != null)
        {
            earTarget = earObj.transform;
            Debug.Log("<color=cyan>[AudioDirector] Ear target set to " + earObj.name + "</color>");
        }
        else
        {
            Debug.LogWarning("[AudioDirector] No Ear object found in scene.");
        }
    }

    // --------------------------------------------------------
    //   MOVE LISTENER (SMOOTH FOLLOW)
    // --------------------------------------------------------
    void FollowEarTarget()
    {
        if (earTarget == null) return;

        listener.transform.position = Vector3.Lerp(
            listener.transform.position,
            earTarget.position,
            Time.deltaTime * followSmooth
        );

        listener.transform.rotation = Quaternion.Slerp(
            listener.transform.rotation,
            earTarget.rotation,
            Time.deltaTime * followSmooth
        );
    }

    // --------------------------------------------------------
    //   MUSIC LOGIC
    // --------------------------------------------------------
    public void UpdateMusicState()
    {
        string scene = SceneManager.GetActiveScene().name;
        bool isMenu = scene == "MainMenu";

        float time = SimulationManager.Instance.currentTime; // 0–24
        bool isDay = time >= 6 && time < 18;
        bool isNight = !isDay;

        if (isMenu)
        {
            FadeMusic(mainMenuMusic, ref mainMenuFade, 0.9f);
            FadeMusic(dayMusic, ref dayFade, isDay ? 0.4f : 0f);
            FadeMusic(nightMusic, ref nightFade, isNight ? 0.4f : 0f);
        }
        else
        {
            // Fade out menu music smoothly instead of stopping
            FadeMusic(mainMenuMusic, ref mainMenuFade, 0f);
            FadeMusic(dayMusic, ref dayFade, isDay ? 0.9f : 0f);
            FadeMusic(nightMusic, ref nightFade, isNight ? 0.9f : 0f);
        }
    }

    // --------------------------------------------------------
    //   Fade Helper (per-source coroutine)
    // --------------------------------------------------------
    void FadeMusic(AudioSource source, ref Coroutine fadeRoutine, float targetVolume)
    {
        if (source == null) return;

        if (fadeRoutine != null)
            StopCoroutine(fadeRoutine);

        fadeRoutine = StartCoroutine(FadeRoutine(source, targetVolume));
    }

    IEnumerator FadeRoutine(AudioSource source, float targetVolume)
    {
        float start = source.volume;
        float t = 0f;

        if (!source.isPlaying)
            source.Play();

        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            source.volume = Mathf.Lerp(start, targetVolume, t / fadeDuration);
            yield return null;
        }

        source.volume = targetVolume;

        if (Mathf.Approximately(targetVolume, 0f))
            source.Stop();
    }
}
