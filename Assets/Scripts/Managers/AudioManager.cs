using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class Audio {
    public AudioClip clip;
    public float volume = 1f;
}
public class AudioManager : MonoBehaviour {
	public static AudioManager s;

	private AudioSource[] effectsSources;
	private AudioSource musicSource;
    public Audio menuMusic,
                 gameMusic;
    public Audio[] meow,
                   ground,
                   splash,
                   rustle,
                   whoosh,
                   explosion,
                   pickup,
                   hover,
                   press,
                   gong,
                   paper,
                   win,
                   lose,
                   money;
	
	private void Awake() {
        s = this;
        effectsSources = new AudioSource[ConstantsManager.s.audioSources];
        for (int i=0; i<effectsSources.Length; i++) {
            effectsSources[i] = gameObject.AddComponent<AudioSource>();
        }
        musicSource = gameObject.AddComponent<AudioSource>();
        PlayMusic(menuMusic);
	}
    private AudioSource GetEffectsSource() {
        foreach (AudioSource source in effectsSources) {
            if (!source.isPlaying) return source;
        }
        return effectsSources[0];
    }
	public void PlayEffect(Audio audio) {
        AudioSource source = GetEffectsSource();
		float randomPitch = Random.Range(ConstantsManager.s.randomPitchMin, ConstantsManager.s.randomPitchMax);
        source.pitch = randomPitch;
		source.clip = audio.clip;
        source.volume = audio.volume;
		source.Play();
	}
	public void PlayEffect(params Audio[] audios) {
        PlayEffect(audios[Random.Range(0, audios.Length)]);
	}
    public void PlayCorrespondingTileEffect(Tile tile) {
        if (tile is MossyTile) {
            PlayEffect(rustle);
        } else if (tile is Puddle) {
            PlayEffect(splash);
        } else {
            PlayEffect(ground);
        }
    }
	public void PlayMusic(Audio audio) {
        if (musicSource.clip == audio.clip) return;
        LeanTween.value(gameObject, (float f) => { musicSource.volume = f; }, musicSource.volume, 0, ConstantsManager.s.musicFadeDuration).setOnComplete(() => {
            musicSource.clip = audio.clip;
            musicSource.volume = 0;
            musicSource.loop = true;
            musicSource.Play();
            LeanTween.value(gameObject, (float f) => { musicSource.volume = f; }, 0, audio.volume, ConstantsManager.s.musicFadeDuration);
        });
	}
    private void OnPlayerDie() {
        PlayEffect(explosion);
    }
    private void OnPlayerMeow() {
        PlayEffect(meow);
    }
    private void OnPlayerArriveAtCoord(int x, int y) {
        Vector2Int coord = Player.s.GetCoord();
        if (Floor.s.TileExistsAt(coord.x, coord.y)) {
            Tile tile = Floor.s.GetTile(coord.x, coord.y).GetComponent<Tile>();
            PlayCorrespondingTileEffect(tile);
        }
    }
    private void OnGameWin() {
        PlayMusic(menuMusic);
        //PlayEffect(win);
    }
    private void OnGameLose() {
        PlayMusic(menuMusic);
        //PlayEffect(lose);
    }
    private void OnGameStart() {
        PlayMusic(gameMusic);
    }
    private void OnGameExit() {
        PlayMusic(menuMusic);
    }
    private void OnEnable() {
        EventManager.s.OnPlayerDie += OnPlayerDie;
        EventManager.s.OnPlayerMeow += OnPlayerMeow;
        EventManager.s.OnPlayerArriveAtCoord += OnPlayerArriveAtCoord;
        EventManager.s.OnGameWin += OnGameWin;
        EventManager.s.OnGameLose += OnGameLose;
        EventManager.s.OnGameStart += OnGameStart;
        EventManager.s.OnGameExit += OnGameExit;
    }
    private void OnDisable() {
        EventManager.s.OnPlayerDie -= OnPlayerDie;
        EventManager.s.OnPlayerMeow -= OnPlayerMeow;
        EventManager.s.OnPlayerArriveAtCoord -= OnPlayerArriveAtCoord;
        EventManager.s.OnGameWin -= OnGameWin;
        EventManager.s.OnGameLose -= OnGameLose;
        EventManager.s.OnGameStart -= OnGameStart;
        EventManager.s.OnGameExit -= OnGameExit;
    }
}
