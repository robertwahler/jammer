using UnityEngine;

namespace Jammer {

  /// <summary>
  /// MonoBehavior singleton abstract class
  ///
  /// Usage: Create a child of this behaviour and attach the child script to a
  /// GameObject.
  /// </summary>
  public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour {

    /// <summary>
    /// Controls DontDestroyOnLoad flag. Needed by test suite.
    /// </summary>
    public bool dontDestroyOnLoad = true;

    private static T _instance = null;

    // NOTE: Call base.Awake() in child classes that override Awake() or the
    //       Singleton behaviour will fail.  Better yet, override Initalize()
    //       instead.
    protected virtual void Awake() {
      if (dontDestroyOnLoad) {
        DontDestroyOnLoad(gameObject);
      }
      if (_instance == null) {
        _instance = this as T;
        Initialize();
      }
      else {
        Destroy(gameObject);
      }
    }

    /// <summary>
    /// Initialize child classes by overriding.  Initialize() will only be called
    /// once, when the persistent class Awake() is called, Initialize() will not
    /// be called if an instance already exists.
    /// </summary>
    protected virtual void Initialize() {
    }

    /// <summary>
    /// gets the instance of this Singleton, returns null if it doesn't exist
    ///
    /// MyClass.Instance.MyMethod(); or make your public methods static and have
    /// them use Instance
    /// </summary>
    public static T Instance {
      get {
        return _instance;
      }
    }

    /// <summary>
    /// for garbage collection
    /// </summary>
    public virtual void OnApplicationQuit () {
      // release reference on exit
      _instance = null;
    }

    /// <summary>
    /// return the board as a string
    /// </summary>
    public override string ToString() {
      return string.Format("{0} (Singleton),  InstanceID={1}", GetType().ToString(), GetInstanceID());
    }

  }
}

