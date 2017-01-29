using UnityEngine;

namespace Jammer {

  public static class ApplicationConstants {

    /// <summary>
    /// Application product name. Used as namespace. Application exe name. Save
    /// folder name. etc.
    /// </summary>
    public static readonly string ProductCode = Application.productName;

    /// <summary>
    /// Scene to load after splash shown
    /// </summary>
    public static readonly string StartScene = "Start";

    /// <summary>
    /// Main game play scene
    /// </summary>
    public static readonly string MainScene = "Level1";

    /// <summary>
    /// Scene that contains the menus
    /// </summary>
    public static readonly string UIScene = "UI";

  }
}

