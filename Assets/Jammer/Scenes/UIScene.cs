using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

using Jammer.UI;

namespace Jammer.Scenes {

  /// <summary>
  /// Contains menus, deletes its own scene hierarchy if it isn't the main scene
  /// </summary>
  public class UIScene : BaseScene {

    /// <summary>
    /// Show on start in the Unity editor if this scene is the active scene.
    /// Assign ref in IDE.
    /// </summary>
    [Header("Design Time Only")]
    public Menu designTimeMenu;

    protected override void OnEnable() {
      Log.Verbose(string.Format("UIScene.OnEnable()"));
      base.OnEnable();

      if (ActiveScene.name != ApplicationConstants.UIScene) {
        // throw away design-time scene hierarchy
        GameObject.DestroyImmediate(gameObject);
      }
    }

    // show menu automatically if this is the editor scene
    #if UNITY_EDITOR
      protected void Start() {
        Log.Debug(string.Format("UIScene.Start()"));

        MenuId id;

        if (ActiveScene.name == ApplicationConstants.UIScene) {
          if (designTimeMenu != null) {
            id = designTimeMenu.Id;
          }
          else {
            id = MenuId.Main;
          }
          Events.Raise(new MenuCommandEvent() { Handled=false, MenuId=id, State=MenuState.Open });
        }
      }
    #endif

  }
}

