using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using DG.Tweening;
using SDD;
using SDD.Events;

using Jammer.Scenes;
using Jammer.Extensions;

namespace Jammer {

  public class MenuActions : EventHandler {

    /// <summary>
    /// Audio mute checkbox
    /// </summary>
    public Toggle toggleMuteAudio;

    // main level
    const string MAINSCENE = "Level1";

    protected override void OnEnable() {
      Log.Debug(string.Format("MenuActions.OnEnable()"));
      base.OnEnable();

      // set value without firing an event
      toggleMuteAudio.SetValue(AudioManager.Instance.MuteAudio);
    }

    public override void SubscribeEvents() {
      Log.Debug(string.Format("MenuActions.SubscribeEvents()"));

      Events.AddListener<AudioSettingsCommandEvent>(OnAudioSettingsCommand);
    }

    public override void UnsubscribeEvents() {
      Log.Debug(string.Format("MenuActions.UnsubscribeEvents()"));

      Events.RemoveListener<AudioSettingsCommandEvent>(OnAudioSettingsCommand);
    }

    public void OnAudioSettingsCommand(AudioSettingsCommandEvent e) {
      if (e.Handled) {
        Log.Debug(string.Format("MenuActions.OnAudioSettingsCommand({0})", e));
        // set value without firing an event
        toggleMuteAudio.SetValue(e.MuteAudio);
      }
    }

    /// <summary>
    /// Play game!
    /// </summary>
    public void OnPlay() {
      Log.Debug(string.Format("MenuActions.OnPlay() this={0}", this));

      Events.Raise(new LoadSceneCommandEvent() { Handled=false, SceneName=MAINSCENE, Mode=LoadSceneMode.Single });
    }

    /// <summary>
    /// Exit the application
    /// </summary>
    public void OnExit() {
      Log.Debug(string.Format("MenuActions.OnExit()"));

      ApplicationHelper.Quit();
    }

    /// <summary>
    /// Audio mute checkbox handler
    /// </summary>
    public void OnMuteAudio(Toggle sender) {
      Log.Debug(string.Format("MenuActions.OnMuteAudio(sender: {0}), sender.isOn {1}", sender, sender.isOn));
      Debug.Log(AudioManager.Instance.MuteAudio);

      Events.Raise(new AudioSettingsCommandEvent() { Handled=false, MuteAudio=sender.isOn });
    }

  }
}

