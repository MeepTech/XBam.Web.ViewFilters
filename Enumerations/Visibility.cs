using Meep.Tech.Data;

namespace Meep.Tech.Web.ViewFilters {

  /// <summary>
  /// The visibility settings
  /// </summary>
  /*public enum Visibility {
    Hidden, // not visible to anyone but the creator. Redirects. For WIPs usually.
    //AdminOnly, // not visible to anyone except admins. Used for testing.
    Public, // can be viewd by anyone, visible in all lists
    Unlisted, // publicly findable by name but doesn't show up in lists
    Obfuscated, // visible to anyone with the randomly generated link
    WhitelistOnly, // visible only to members who own a character in a whitelist, you must add them or approve a user request.
    //FollowedOnly, // only visible to people you follow a character of. Redirects for other people.
    //MutualsOnly // Visible to mutuals only. Redirects for other people.
  }*/

  public class Visibility : Enumeration<Visibility> {

    public static Visibility Hidden {
      get;
    } = new Visibility("Hidden", "Included");

    public static Visibility Public {
      get;
    } = new Visibility("Public", "Included");

    public static Visibility Unlisted {
      get;
    } = new Visibility("Unlisted", "Included");

    public static Visibility Obfuscated {
      get;
    } = new Visibility("Obfuscated", "Included");

    public static Visibility WhitelistOnly {
      get;
    } = new Visibility("WhitelistOnly", "Included");

    protected Visibility(string uniqueIdentifier, string packageName, Universe universe = null)
      : base(packageName + "." + uniqueIdentifier, universe) { }
  }
}
