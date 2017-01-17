using UnityEngine;
using System.Collections;
using DG.Tweening;
using SDD;
using SDD.Events;

namespace Jammer.Scenes {

  public class GameScene : BaseScene {

    public override void Awake() {
      Log.Debug(string.Format("GameScene.Awake()"));

      // Don't call base.Awake(), this scene has it loaded already
    }

    protected override void OnEnable() {
      Log.Debug(string.Format("GameScene.OnEnable()"));
      base.OnEnable();

    }

    protected override void HandleInput() {
      base.HandleInput();;

      // scene specific input handler
    }

  }
}

