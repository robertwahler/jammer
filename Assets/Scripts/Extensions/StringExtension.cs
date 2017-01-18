using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;

namespace Jammer.Extensions {

  public static class StringExtension {

    /// <summary>
    /// Return a camelCaseString
    /// </summary>
    public static string CamelCase(this string source) {
      return System.Text.RegularExpressions.Regex.Replace(source, "([a-z](?=[A-Z])|[A-Z](?=[A-Z][a-z]))", "$1 ").Trim();
    }

    /// <summary>
    /// Strip blank lines
    /// </summary>
    public static string StripBlankLines(this string source) {
      return System.Text.RegularExpressions.Regex.Replace(source, @"^\s+$[\r\n]*", "", RegexOptions.Multiline).Trim();
    }

    public static string Base64Encode(this string source) {
      var bytes = System.Text.Encoding.UTF8.GetBytes(source);
      return Convert.ToBase64String(bytes);
    }

    public static string Base64Decode(this string source) {
      var bytes = Convert.FromBase64String(source);
      return System.Text.Encoding.UTF8.GetString(bytes);
    }

    public static byte[] AsBytes(this string source) {
      return System.Text.Encoding.UTF8.GetBytes(source);
    }

    public static string AsString(this byte[] source) {
      return System.Text.Encoding.UTF8.GetString(source);
    }

    public static string Capitalize(this string text) {
      return Regex.Replace(text, "^[a-z]", m => m.Value.ToUpper());
    }

    /// <summary>
    /// Platform independent, language independent, non-cryptographic hash code
    /// </summary>
    public static int Hash(this string text) {
      // unchecked => overflows will not cause error
      unchecked {
        int hash = 23;
        foreach (char c in text) {
          hash = hash * 31 + c;
        }
        return hash;
      }
    }

    /// <summary>
    /// Platform independent, language independent, CRC. Ignores EOL characters CR, LF.
    /// </summary>
    public static int CRC(this string text) {
      return Regex.Replace(text, @"\r\n?|\n", "").Hash();
    }

    /// <summary>
    /// Truncate string with "..." at maxChars
    /// </summary>
    public static string Truncate(this string value, int maxChars) {
      return value.Length <= maxChars ? value : value.Substring(0, maxChars) + "...";
    }

  }
}
