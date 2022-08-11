using System;
using System.Collections.Generic;
using System.Linq;
using Meep.Tech.Data;

namespace Meep.Tech.Web.ViewFilters {

  /// <summary>
  /// Can be used to filter views for an object based on visibility settings.
  /// </summary>
  public abstract class ViewFilter<TViewingModel>
    : IModel.IComponent<ViewFilter<TViewingModel>>,
      IComponent.IUseDefaultUniverse,
      IComponent.IDoOnAdd
    where TViewingModel : IUnique {
    Func<ViewFilter<TViewingModel>, TViewingModel, bool> _isVisibleFunc { get; init; }
    Func<ViewFilter<TViewingModel>, TViewingModel, View> _viewFetcher { get; init; }
    Visibility _visibility;

    /// <summary>
    /// The default function used to detect if the item should be visible to the given model.
    /// </summary>
    public static Func<ViewFilter<TViewingModel>, TViewingModel, bool> DefaultIsVisibleFunc
      = (@this, viewer)
        => {
          if (@this.Parent.Equals(viewer)) {
            return true;
          }
          if (@this.Blacklist.Contains(viewer.Id) || @this.Visibility == Visibility.Hidden) {
            return false;
          }
          else if (@this.Visibility == Visibility.Public
            || @this.Visibility == Visibility.Obfuscated
            || @this.Visibility == Visibility.Unlisted
          ) {
            return true;
          }
          else if (@this.Visibility == Visibility.WhitelistOnly) {
            return @this.Whitelist.Contains(viewer.Id);
          }

          return true;
        };

    /// <summary>
    /// The parent model.
    /// </summary>
    public IReadableComponentStorage Parent {
      get;
      private set;
    }

    /// <summary>
    /// The overall visibility setting
    /// </summary>
    [AutoBuild(ParameterName = "ViewFilter" + "." + nameof(TViewingModel) + "." + nameof(Visibility))]
    public Visibility Visibility {
      get => _visibility ??= Visibility.Hidden;
      set => _visibility = value;
    }

    /// <summary>
    /// Allow other users to request access to this item's whitelist
    /// </summary>
    [AutoBuild(ParameterName = "ViewFilter" + "." + nameof(TViewingModel) + "." + nameof(AllowWhitelistRequests))]
    public bool AllowWhitelistRequests {
      get;
      set;
    }

    /// <summary>
    /// Any whitelisted characters
    /// </summary>
    [AutoBuild(ParameterName = "ViewFilter" + "." + nameof(TViewingModel) + "." + nameof(Whitelist))]
    public virtual HashSet<string> Whitelist {
      get;
      private set;
    } = new();

    /// <summary>
    /// Any blacklisted characters
    /// </summary>
    [AutoBuild(ParameterName = "ViewFilter" + "." + nameof(TViewingModel) + "." + nameof(Blacklist))]
    public virtual HashSet<string> Blacklist {
      get;
      private set;
    } = new();

    /// <summary>
    /// Can be used to make a generic type of view filter.
    /// </summary>
    /// <returns></returns>
    public static ViewFilter<TViewingModel> Make(Func<ViewFilter<TViewingModel>, TViewingModel, View> getView, Func<ViewFilter<TViewingModel>, TViewingModel, bool> isVisible = null, params (string, object)[] @params) =>
      Components<ViewFilter<TViewingModel>>.BuilderFactory.Make(new[] {
        (nameof(getView), (object)getView),
        (nameof(isVisible), isVisible)
      }.Concat(@params));

    /// <summary>
    /// Check if the current model should be visible at all to the viewer model.
    /// </summary>
    public bool IsVisibleFor(TViewingModel viewer)
      => _isVisibleFunc.Invoke(this, viewer);

    /// <summary>
    /// Get a view given a specific viewer of this model.
    /// </summary>
    public View GetViewFor(TViewingModel viewer)
      => _viewFetcher.Invoke(this, viewer);

    #region XBam Config

    ViewFilter(IBuilder builder) {
      _viewFetcher = builder.GetAndValidateParamAs<Func<ViewFilter<TViewingModel>, TViewingModel, View>>("getView");
      _isVisibleFunc = builder.GetParam("isVisible", DefaultIsVisibleFunc);
    }

    void IComponent.IDoOnAdd.ExecuteWhenAdded(IReadableComponentStorage parent)
      => Parent = parent;

    #endregion
  }
}
