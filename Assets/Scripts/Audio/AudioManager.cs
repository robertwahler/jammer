using UnityEngine;
using UnityEngine.Audio;
using Newtonsoft.Json;
using SDD;
using SDD.Events;

namespace Jammer {

  /// <summary>
  /// Audio manager. Attached to Game Manager
  /// </summary>
  public class AudioManager : EventHandler {

    /// <summary>
    /// Master maixer ref. Set in IDE.
    /// </summary>
    public AudioMixer mixerMaster;

    /// <summary>
    /// Audiosource for music. Assign in IDE.
    /// </summary>
    public AudioSource musicAudioSource;

    /// <summary>
    /// Exposed parameter for the master volume;
    /// </summary>
    public void SetMusicVolume(float volume) {
      mixerMaster.SetFloat("volumeMaster", volume);
    }

    public override void SubscribeEvents() {
      Log.Debug(string.Format("MenuManager.SubscribeEvents()"));

      Events.AddListener<MainMenuCommandEvent>(OnMainMenuCommand);
    }

    public override void UnsubscribeEvents() {
      Log.Debug(string.Format("MenuManager.UnsubscribeEvents()"));

      Events.RemoveListener<MainMenuCommandEvent>(OnMainMenuCommand);
    }

    public void OnMainMenuCommand(MainMenuCommandEvent e) {
      if (!e.Handled) {
        Log.Debug(string.Format("GameScene.OnMainMenuCommand({0})", e));

        // TODO: lower vol/freq using filter when menus are open
      }
    }

  }
}
