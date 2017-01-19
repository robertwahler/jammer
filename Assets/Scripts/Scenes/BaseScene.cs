using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using SDD;
using SDD.Events;

namespace Jammer.Scenes {

  public class BaseScene : EventHandler {

    public Scene ActiveScene {
      get {
        return SceneManager.GetActiveScene();
      }
    }

    public virtual void Awake() {
      Log.Debug(string.Format("BaseScene.Awake()"));

      // If the GameManager instance is not available, then it needs to be created.
      if (!GameManager.Instance) {
        Log.Debug(string.Format("BaseScene.Awake() GameManager singleton not found, loading GameManager prefab..."));
        GameObject prefab = (GameObject) Resources.Load("GameManager");
        prefab.name = "GameManager";
        UnityEngine.Object.Instantiate(prefab);
      }
    }

    protected override void OnEnable() {
      Log.Debug(string.Format("BaseScene.OnEnable()"));
      base.OnEnable();

      // notify event
      Events.Raise(new LoadSceneCommandEvent() { Handled=true, SceneName=ActiveScene.name });
    }

    public override void SubscribeEvents() {
    }

    public override void UnsubscribeEvents() {
    }

    protected virtual void Update() {
      if ((Game != null) && !Game.Focused) return;

      HandleInput();
    }

    protected virtual void HandleInput() {
      if (Input.GetKeyDown(KeyCode.Escape)) {
        Log.Debug(string.Format("BaseScene.HandleInput() KeyCode.Escape"));

        // toggle main menu, unless we are on the start scene
        if (ActiveScene.name != ApplicationConstants.StartScene) {
          EventManager.Instance.Raise(new MainMenuCommandEvent(){ Handled=false });
        }
      }
    }

  }
}

