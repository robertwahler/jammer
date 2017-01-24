using UnityEngine;
using SDD.Events;

namespace Jammer {

  public class BaseBehaviour : MonoBehaviour {

    /// <summary>
    /// The GameManager singleton, lazy loaded when required, but not forced on.
    /// </summary>
    public GameManager Game {
      get {
        return GameManager.Instance;
      }
    }

    /// <summary>
    /// The EventManager instance, a convenience property
    /// </summary>
    public EventManager Events {
      get {
        return EventManager.Instance;
      }
    }

    /// <summary>
    /// Convenience getter for the active scene
    /// </summary>
    public UnityEngine.SceneManagement.Scene ActiveScene {
      get {
        return UnityEngine.SceneManagement.SceneManager.GetActiveScene();
      }
    }


    /// <summary>
    /// return this behaviour as a string
    /// </summary>
    public override string ToString() {
      return string.Format("{0}, InstanceID {1}", GetType().ToString(), GetInstanceID());
    }
  }
}
