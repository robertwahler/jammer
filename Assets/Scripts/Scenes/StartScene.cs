using UnityEngine;
using System.Collections;
using DG.Tweening;
using SDD;
using SDD.Events;

using Jammer.UI;

namespace Jammer.Scenes {

  /// <summary>
  /// Acts as a start screen, designer and singleton loader. This script will
  /// intentionally not survive loading the level scenes.
  /// </summary>
  public class StartScene : BaseScene {

    protected virtual void Start() {
      Log.Debug(string.Format("StartScene.Start()"));

      // show menus first time through
      Events.Raise(new MenuCommandEvent() { Handled=false, MenuId=MenuId.Main, State=MenuState.Open });
    }

    protected override void HandleInput() {
      base.HandleInput();

      // scene specific input handler
    }

  }
}

