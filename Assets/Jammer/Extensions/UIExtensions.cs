using UnityEngine;
using UnityEngine.UI;

namespace Jammer.Extensions {

  /// <summary>
  /// Unity has an unfortunate design choice, you can't set UI value state
  /// without the UI event firing.  Ack!
  /// </summary>
  /// <remarks>
  /// https://forum.unity3d.com/threads/change-the-value-of-a-toggle-without-triggering-onvaluechanged.275056/
  /// </remarks>
  public static class UIEventSyncExtensions {
    static Slider.SliderEvent emptySliderEvent = new Slider.SliderEvent();
    public static void SetValue(this Slider instance, float value) {
      var originalEvent = instance.onValueChanged;
      instance.onValueChanged = emptySliderEvent;
      instance.value = value;
      instance.onValueChanged = originalEvent;
    }

    static Toggle.ToggleEvent emptyToggleEvent = new Toggle.ToggleEvent();
    public static void SetValue(this Toggle instance, bool value) {
      var originalEvent = instance.onValueChanged;
      instance.onValueChanged = emptyToggleEvent;
      instance.isOn = value;
      instance.onValueChanged = originalEvent;
    }

    static InputField.OnChangeEvent emptyInputFieldEvent = new InputField.OnChangeEvent();
    public static void SetValue(this InputField instance, string value) {
      var originalEvent = instance.onValueChanged;
      instance.onValueChanged = emptyInputFieldEvent;
      instance.text = value;
      instance.onValueChanged = originalEvent;
    }
  }
}
