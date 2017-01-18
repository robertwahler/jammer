using UnityEngine;
using UnityEngine.Audio;
using System.Collections;
using Newtonsoft.Json;
using SDD;
using SDD.Events;

namespace Jammer {

  /// <summary>
  /// Audio manager. Attached to Game Manager
  /// </summary>
  public class AudioManager : EventHandler {

    /// <summary>
    /// There should be just one manager. If not found return null
    /// </summary>
    public static AudioManager Instance {
      get {
        if (instance == null) {
          Log.Debug(string.Format("AudioManager.Instance.get looking for object"));
          instance = (AudioManager) GameObject.FindObjectOfType(typeof(AudioManager));
        }

        return instance;
      }
    }
    private static AudioManager instance = null;

    /// <summary>
    /// Master maixer ref. Set in IDE.
    /// </summary>
    public AudioMixer mixerMaster;

    /// <summary>
    /// Audiosource for music. Assign in IDE.
    /// </summary>
    public AudioSource musicAudioSource;

    /// <summary>
    /// Mute all audio. This will actually stop all audio sources.
    /// </summary>
    public bool MuteAudio { get { return GetMuteAudio(); } set { SetMuteAudio(value); }}

    /// <summary>
    /// Audio settings. These are serialized to audio.txt in JSON format.
    /// </summary>
    private AudioSettings audioSettings = new AudioSettings();

    protected void Awake() {
      Log.Verbose(string.Format("AudioManager.Awake()"));

      Load();

      // sync subscribers to our initial state, if subscribers are not awake
      // yet, they will need to ask for the settings
      Events.Raise(new AudioSettingsCommandEvent() { Handled=true, MuteAudio=MuteAudio });
    }

    private IEnumerator Start() {
      Log.Debug(string.Format("AudioManager.Start()"));

      // wait one frame for components to fully settle out
      yield return null;

      if (!audioSettings.MuteAudio) {
        musicAudioSource.Play();
      }
    }

    public override void SubscribeEvents() {
      Log.Debug(string.Format("AudioManager.SubscribeEvents()"));

      Events.AddListener<MainMenuCommandEvent>(OnMainMenuCommand);
      Events.AddListener<AudioSettingsCommandEvent>(OnAudioSettingsCommand);
    }

    public override void UnsubscribeEvents() {
      Log.Debug(string.Format("AudioManager.UnsubscribeEvents()"));

      Events.RemoveListener<MainMenuCommandEvent>(OnMainMenuCommand);
      Events.RemoveListener<AudioSettingsCommandEvent>(OnAudioSettingsCommand);
    }

    public void OnAudioSettingsCommand(AudioSettingsCommandEvent e) {
      if (!e.Handled) {
        Log.Debug(string.Format("AudioManager.OnAudioSettingsCommand({0})", e));
        MuteAudio = e.MuteAudio;
      }
    }

    public void OnMainMenuCommand(MainMenuCommandEvent e) {
      if (!e.Handled) {
        Log.Debug(string.Format("AudioManager.OnMainMenuCommand({0})", e));
        // TODO: lower vol/freq using filter when menus are open
      }
    }

    /// <summary>
    /// Load audio settings from JSON, if param null, will load from default
    /// </summary>
    public void Load(string json=null) {
      Log.Verbose(string.Format("AudioManager.Load(json: {0})", json));

      try {
        if (string.IsNullOrEmpty(json)) {
          Log.Verbose(string.Format("AudioManager.Load(json: {0}) loading from Audio.txt", json));
          // load the data, defaults to empty string
          json = Settings.GetString(SettingsKey.Audio);
        }
        if (!string.IsNullOrEmpty(json)) {
          audioSettings = JsonConvert.DeserializeObject<AudioSettings>(json);
          Log.Debug(string.Format("AudioManager.Load(json: {0}) loaded audioSettings {1}", json, audioSettings));
        }
        else {
          Log.Verbose(string.Format("AudioManager.Load(json: {0}) Audio.txt empty, using audio defaults", json));
        }
      }
      catch (System.Exception e) {
        Log.Error(string.Format("AudioManager.Load() failed with {0}", e.ToString()));
      }
    }
    /// <summary>
    /// Save audio settings to JSON, if param null, will save from memory
    /// </summary>
    public void Save(string json=null) {
      Log.Debug(string.Format("AudioManager.Save(json: {0})", json));

      try {
        if (string.IsNullOrEmpty(json)) {
          Log.Verbose(string.Format("AudioManager.Save(json: {0}) serializing AudioSettings", json));
          json = JsonConvert.SerializeObject(value: audioSettings, formatting: Formatting.Indented);
        }
        Settings.SetString(SettingsKey.Audio, json);
        Settings.Save();
      }
      catch (System.Exception e) {
        Log.Error(string.Format("AudioManager.Save() failed with {0}", e.ToString()));
      }
    }

    /// <summary>
    /// Exposed parameter for the master volume;
    /// </summary>
    public void SetMusicVolume(float volume) {
      mixerMaster.SetFloat("volumeMaster", volume);
    }

    private bool GetMuteAudio() {
      return audioSettings.MuteAudio;
    }

    private void SetMuteAudio(bool value) {
      Log.Verbose(string.Format("AudioManager.SetMuteAudio(value: {0})", value));

      if (value != audioSettings.MuteAudio) {
        audioSettings.MuteAudio = value;
        if (audioSettings.MuteAudio) {
          musicAudioSource.Stop();
        }
        else {
          musicAudioSource.Play();
        }
        Save();
        // notify event
        Events.Raise(new AudioSettingsCommandEvent() { Handled=true, MuteAudio=MuteAudio });
      }
    }

  }
}
