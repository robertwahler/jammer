using UnityEngine;

namespace SDD.Events {

  /// <summary>
  /// Event handler
  /// </summary>
  public abstract class EventHandler : Jammer.BehaviourBase, IEventHandler {

    /// <summary>
    /// Subscribe to events
    ///
    /// @example
    ///   EventManager.Instance.AddListener<MoveResolvedEvent>(OnMoveResolved);
    /// </summary>
    public abstract void SubscribeEvents();

    /// <summary>
    /// Unsubscribe from events
    ///
    /// @example
    ///   EventManager.Instance.RemoveListener<MoveResolvedEvent>(OnMoveResolved);
    /// </summary>
    public abstract void UnsubscribeEvents();

    protected virtual void OnEnable() {
      SubscribeEvents();
    }

    protected virtual void OnDisable() {
      UnsubscribeEvents();
    }

  }
}
