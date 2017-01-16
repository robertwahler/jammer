using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using SDD;
using SDD.Events;

namespace Jammer.Scenes {

  public class SplashScene : BaseScene {

    /// <summary>
    /// Main logo canvas. Set in IDE.
    /// </summary>
    public CanvasGroup canvasGroupLogo;

    // scene to load after splash shown
    const string MAINSCENE = "Game";

    protected override void OnEnable() {
      Log.Debug(string.Format("SplashScene.OnEnable()"));
      base.OnEnable();

      if (canvasGroupLogo != null) {
        // immediate fade out
        canvasGroupLogo.alpha = 0f;
      }
      else {
        Log.Error(string.Format("SplashScene.OnEnable() canvasGroupLogo not set in IDE"));
      }
    }

    protected IEnumerator Start() {
      Log.Debug(string.Format("SplashScene.Start()"));

      // max wait for game to start before animation
      float wait = 10.0f;
      while (wait > 0) {
        wait -= Time.deltaTime;
        yield return null;

        if ((Game.State != GameState.New) && (Game.State != GameState.Loading)) {
          // game ready, start animation
          StartCoroutine(AnimateStart());
          yield break;
        }
      }

      // fail safe, this should never run but if the timer expires, load the main scene
      yield return WaitFor.Seconds(10.0f);
      StopAllCoroutines();
      Log.Warning(string.Format("SplashScene.Start() fail-safe timer expired"));
      Events.Raise(new LoadSceneCommandEvent() { Handled=false, SceneName=MAINSCENE, Mode=LoadSceneMode.Single });
    }

    private IEnumerator AnimateStart() {
      Log.Debug(string.Format("SplashScene.AnimateStart() this={0}", this));

      // wait for the Unity splash screen to go away
      while (Application.isShowingSplashScreen) {
        yield return null;
      }

      // Animate logo
      float duration = 4f;
      canvasGroupLogo.DOFade(1f, duration: duration);

      // wait for animation, can be cut short via keypress
      while ((duration > 0) && (!Input.anyKeyDown)) {
        duration -= Time.deltaTime;
        yield return null;
      }

      Events.Raise(new LoadSceneCommandEvent() { Handled=false, SceneName=MAINSCENE, Mode=LoadSceneMode.Single });
    }
  }
}

