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
    /// Main menu container. Set in IDE.
    /// </summary>
    public GameObject menuContainer;

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

    /// <summary>
    /// Menu state
    /// </summary>
    public MenuState State { get { return GetState(); } set { SetState(value); }}

    /// <summary>
    /// The name of the currently loaded scene or nil none
    /// </summary>
    public string CurrentScene { get; set; }

    public override void SubscribeEvents() {
      Log.Debug(string.Format("MenuManager.SubscribeEvents()"));

      Events.AddListener<MainMenuCommandEvent>(OnMainMenuCommand);
      Events.AddListener<LoadSceneCommandEvent>(OnLoadSceneCommand);
      Events.AddListener<UnloadSceneCommandEvent>(OnUnloadSceneCommand);
    }

    public override void UnsubscribeEvents() {
      Log.Debug(string.Format("MenuManager.UnsubscribeEvents()"));

      Events.RemoveListener<MainMenuCommandEvent>(OnMainMenuCommand);
      Events.RemoveListener<LoadSceneCommandEvent>(OnLoadSceneCommand);
      Events.RemoveListener<UnloadSceneCommandEvent>(OnUnloadSceneCommand);
    }

    public void OnMainMenuCommand(MainMenuCommandEvent e) {
      if (!e.Handled) {
        Log.Debug(string.Format("GameScene.OnMainMenuCommand({0})", e));

        // MenuCommand is a toggle
        if (State == MenuState.Closed) {
          State = MenuState.Open;
        }
        else {
          State = MenuState.Closed;
        }
      }
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

        // turn off menus
        State = MenuState.Closed;

        if (CurrentScene == e.SceneName) {
          // TODO: Add e.Force to reload if needed
          Log.Debug(string.Format("MenuManager.OnLoadSceneCommand({0}) skipping, already loaded", e));
          return;
        }

        switch (e.Mode) {

          case LoadSceneMode.Additive:
            UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName: e.SceneName, mode: UnityEngine.SceneManagement.LoadSceneMode.Additive);
            break;

          default:
            UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName: e.SceneName, mode: UnityEngine.SceneManagement.LoadSceneMode.Single);
            break;
        }

        CurrentScene = e.SceneName;

        // notify event
        Events.Raise(new LoadSceneCommandEvent() { Handled=true, SceneName=CurrentScene, Mode=e.Mode });
      }
    }

    private MenuState GetState() {
      if (menuContainer.activeSelf) {
        return MenuState.Open;
      }
      else {
        return MenuState.Closed;
      }
    }

    private void SetState(MenuState value) {
      Log.Verbose(string.Format("MenuManager.SetState(value: {0})", value));

      switch(value) {

        case MenuState.Closed:
          menuContainer.SetActive(false);
          break;

        case MenuState.Open:
          menuContainer.SetActive(true);
          break;

        default:
          Log.Error(string.Format("MenuManager.SetState(value: {0}) unhandled state", value));
          break;
      }

      // notify event
      Events.Raise(new MainMenuCommandEvent() { Handled=true, State=State });
    }

  }
}
