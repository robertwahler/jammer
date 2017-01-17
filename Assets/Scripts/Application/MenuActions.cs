using UnityEngine;
using System.Collections;
using DG.Tweening;
using SDD;
using SDD.Events;

using Jammer.Scenes;

namespace Jammer {

  public class MenuActions : Jammer.BehaviourBase {

    // main level
    const string MAINSCENE = "Level1";

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

  }
}

