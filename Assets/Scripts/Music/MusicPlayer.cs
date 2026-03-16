using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class MusicPlayer : MonoBehaviour
{
    public static MusicPlayer Instance;

    private AudioSource audioSource;
    public List<AudioClip> musicTracks;
    
    public TextMeshProUGUI songText; 
    public GameObject musicPanel;
    private int currentTrackIndex = 0;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            // Setup AudioSource
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.loop = false;
            audioSource.volume = 0.1f;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    void Start()
    {
        PlayNextTrack();
    }

    void PlayNextTrack()
    {
        if (musicTracks.Count == 0) return;

        int newTrackIndex;
        do
        {
            newTrackIndex = Random.Range(0, musicTracks.Count);
        } while (newTrackIndex == currentTrackIndex); 

        currentTrackIndex = newTrackIndex;
        audioSource.clip = musicTracks[currentTrackIndex];
        audioSource.Play();
        musicPanel.SetActive(true);
        UpdateSongNameUI(musicTracks[currentTrackIndex].name);

        StartCoroutine(WaitForSongToEnd());
    }

    IEnumerator WaitForSongToEnd()
    {
        yield return new WaitForSeconds(audioSource.clip.length);
        PlayNextTrack();
    }

    void UpdateSongNameUI(string songName)
    {
        if (songText != null)
        {
            songText.text = "Now Playing: " + songName;
        }
        else
        {
            Debug.LogWarning("No TMP Text assigned or found in the scene!");
        }
    }

	public void SetVolume(float volume)
	{
		audioSource.volume = volume;
		PlayerPrefs.SetFloat("MusicVolume", volume);
		PlayerPrefs.Save();
	}

	public void ToggleMusic()
	{
		if (audioSource.isPlaying)
		{
			audioSource.Pause();
			PlayerPrefs.SetInt("MusicState", 0);
		}
		else
		{
			audioSource.Play();
			PlayerPrefs.SetInt("MusicState", 1);
		}
		PlayerPrefs.Save();
	}

}
