using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using SDD;

namespace Jammer.Scenes {

  public class SplashScene : BaseScene {

    /// <summary>
    /// Main logo canvas. Set in IDE.
    /// </summary>
    public CanvasGroup canvasGroupLogo;

    public override void Awake() {
      Log.Debug(string.Format("SplashScene.Awake()"));

      // Don't call base.Awake(), this is the only scene that doesn't
      // need to support hot-loading of the GameManager
    }

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

    protected void Start() {
      Log.Debug(string.Format("SplashScene.Start()"));

      StartCoroutine(AnimateStart());
    }

    private IEnumerator AnimateStart() {
      Log.Debug(string.Format("SplashScene.AnimateStart() this={0}", this));

      // wait for the Unity splash screen to go away
      while (Application.isShowingSplashScreen) {
        yield return null;
      }

      // Animate logo
      float duration = 3f;
      canvasGroupLogo.DOFade(1f, duration: duration);

      // wait for animation, short-circuit via any keypress
      while ((duration > 0) && (!Input.anyKeyDown)) {
        duration -= Time.deltaTime;
        yield return null;
      }

      // load the start game scene, it handles the GameManager, Menus, and loading levels
      UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName: ApplicationConstants.StartScene, mode: UnityEngine.SceneManagement.LoadSceneMode.Single);
    }
  }
}

