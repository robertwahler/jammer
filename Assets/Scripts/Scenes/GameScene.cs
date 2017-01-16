using UnityEngine;
using System.Collections;
using DG.Tweening;
using SDD;
using SDD.Events;

namespace Jammer.Scenes {

  public class GameScene : BaseScene {

    /// <summary>
    /// Main menu container. Set in IDE.
    /// </summary>
    public GameObject menuContainer;

    /// <summary>
    ///  Game container. Set in IDE.
    /// </summary>
    public GameObject gameContainer;

    protected override void OnEnable() {
      Log.Debug(string.Format("GameScene.OnEnable()"));
      base.OnEnable();

    }

    public override void SubscribeEvents() {
      Log.Debug(string.Format("GameScene.SubscribeEvents()"));
      base.SubscribeEvents();

      Events.AddListener<MainMenuCommandEvent>(OnMainMenuCommand);
    }


    public override void UnsubscribeEvents() {
      Log.Debug(string.Format("GameScene.UnsubscribeEvents()"));
      base.UnsubscribeEvents();

      Events.RemoveListener<MainMenuCommandEvent>(OnMainMenuCommand);
    }

    public void OnMainMenuCommand(MainMenuCommandEvent e) {
      if (!e.Handled) {
        Log.Debug(string.Format("GameScene.OnMainMenuCommand({0})", e));

        menuContainer.SetActive(true);
      }
    }

    /// <summary>
    /// Play game!
    /// </summary>
    public void OnPlay() {
      Log.Debug(string.Format("GameScene.OnPlay() this={0}", this));

      if (!gameContainer.activeSelf) {
        gameContainer.SetActive(true);
      }
      menuContainer.SetActive(false);
    }

    /// <summary>
    /// Exit the application
    /// </summary>
    public void OnExit() {
      Log.Debug(string.Format("GameScene.OnExit()"));

      ApplicationHelper.Quit();
    }

    protected override void HandleInput() {
      // game scene specific input handler
    }


  }
}

