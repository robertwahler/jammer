using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using SDD;

namespace Jammer.Scenes {

  /// <summary>
  /// Contains menus, deletes its own scene hierarchy if it isn't the main scene
  /// </summary>
  public class UIScene : BaseScene {

    protected override void OnEnable() {
      Log.Debug(string.Format("UIScene.OnEnable()"));
      base.OnEnable();

      if (ActiveScene.name != ApplicationConstants.UIScene) {
        // throw away design-time scene hierarchy
        GameObject.DestroyImmediate(gameObject);
      }
    }

    protected void Start() {
      Log.Debug(string.Format("UIScene.Start()"));

      if (ActiveScene.name == ApplicationConstants.UIScene) {
        // we are design time editing, show the menus
        Events.Raise(new MainMenuCommandEvent() { Handled=false, State=MenuState.Open });
      }

    }

  }
}

