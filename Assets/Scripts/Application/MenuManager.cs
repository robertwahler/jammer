using UnityEngine;
using Newtonsoft.Json;
using SDD;
using SDD.Events;

using Jammer.Scenes;

namespace Jammer {

  /// <summary>
  /// Light weight menu manager fake singleton. Listens for menu events, maps
  /// them, handles loading and unload the menus as needed. Hang this off the
  /// GameManager.
  /// </summary>
  public class MenuManager : EventHandler {

    /// <summary>
    /// There should be just one manager. If not found return null
    /// </summary>
    public static MenuManager Instance {
      get {
        if (instance == null) {
          Log.Debug(string.Format("MenuManager.Instance.get looking for object"));
          instance = (MenuManager) GameObject.FindObjectOfType(typeof(MenuManager));
        }

        return instance;
      }
    }
    private static MenuManager instance = null;

    public override void SubscribeEvents() {
      Log.Debug(string.Format("MenuManager.SubscribeEvents()"));

      Events.AddListener<LoadSceneCommandEvent>(OnLoadSceneCommand);
      Events.AddListener<UnloadSceneCommandEvent>(OnUnloadSceneCommand);
    }

    public override void UnsubscribeEvents() {
      Log.Debug(string.Format("MenuManager.UnsubscribeEvents()"));

      Events.RemoveListener<LoadSceneCommandEvent>(OnLoadSceneCommand);
      Events.RemoveListener<UnloadSceneCommandEvent>(OnUnloadSceneCommand);
    }

    /// <summary>
    /// Request to unload a scene
    /// </summary>
    void OnUnloadSceneCommand(UnloadSceneCommandEvent e) {
      if (!e.Handled) {
        Log.Debug(string.Format("MenuManager.OnUnloadSceneCommand({0})", e));

        UnityEngine.SceneManagement.SceneManager.UnloadScene(sceneName: e.SceneName);

        // notify event
        Events.Raise(new UnloadSceneCommandEvent() { Handled=true, SceneName=e.SceneName });
      }
    }

    /// <summary>
    /// Request to load a scene
    /// </summary>
    void OnLoadSceneCommand(LoadSceneCommandEvent e) {
      if (!e.Handled) {
        Log.Debug(string.Format("MenuManager.OnLoadSceneCommand({0})", e));

        switch (e.Mode) {

          case LoadSceneMode.Additive:
            UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName: e.SceneName, mode: UnityEngine.SceneManagement.LoadSceneMode.Additive);
            break;

          default:
            UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName: e.SceneName, mode: UnityEngine.SceneManagement.LoadSceneMode.Single);
            break;
        }

        // notify event
        Events.Raise(new LoadSceneCommandEvent() { Handled=true, SceneName=e.SceneName, Mode=e.Mode });
      }
    }

  }
}
