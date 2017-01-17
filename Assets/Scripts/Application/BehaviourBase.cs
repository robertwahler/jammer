using UnityEngine;
using SDD.Events;

namespace Jammer {

  public class BehaviourBase : MonoBehaviour {

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
    /// return this behaviour as a string
    /// </summary>
    public override string ToString() {
      return string.Format("{0}, InstanceID {1}", GetType().ToString(), GetInstanceID());
    }
  }
}
