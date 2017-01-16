using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using SDD;
using SDD.Events;

namespace Jammer.Scenes {

  public class BaseScene : EventHandler {

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

    public override void SubscribeEvents() {
    }

    public override void UnsubscribeEvents() {
    }

    protected virtual void Update() {
      //if ((Game != null) && !Game.Focused) return;

      HandleInput();
    }

    protected virtual void HandleInput() {
    }

  }
}

