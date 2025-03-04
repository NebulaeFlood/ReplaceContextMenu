using RimWorld;
using UnityEngine;
using Verse;

namespace NoCrowdedContextMenu
{
    internal enum IconStatus : int
    {
        None,
        LayerOnly,
        FromTexture,
        FromDef,
        FromThing
    }


    internal readonly struct OptionInfo
    {
        //------------------------------------------------------
        //
        //  Private Fields
        //
        //------------------------------------------------------

        #region Private Fields

        private readonly Thing _iconThing;
        private readonly Texture2D _itemIcon;
        private readonly ThingDef _shownItem;
        private readonly ThingStyleDef _thingStyle;

        private readonly Color _compositionColor;
        private readonly Color _iconColor;
        private readonly Rect _iconTexCoords;

        private readonly bool _drawLayer;

        private readonly BuildableDef _associatedBuild;

        #endregion


        internal readonly bool HasInfoCard;
        internal readonly bool ShowMaterialInfoWindow;

        internal readonly IconStatus IconStatus;


        //------------------------------------------------------
        //
        //  Internal Properties
        //
        //------------------------------------------------------

        #region Internal Properties

        internal Color Composition => _compositionColor;

        internal int CostCount
        {
            get
            {
                int costCount = 0;

                if (NCCMPatch.IsBuildPickerMenu)
                {
                    costCount = _associatedBuild.CostStuffCount;

                    if (costCount < 1)
                    {
                        costCount = _associatedBuild.costList[0].count;
                    }
                    else if (_shownItem.smallVolume)
                    {
                        costCount *= 10;
                    }
                }
                else if (NCCMPatch.IsMaterialPickerMenu)
                {
                    costCount = NCCMPatch.BuildRequireMaterial.CostStuffCount;

                    if (costCount < 1)
                    {
                        costCount = NCCMPatch.BuildRequireMaterial.costList[0].count;
                    }
                    else if (_shownItem.smallVolume)
                    {
                        costCount *= 10;
                    }
                }

                return costCount;
            }
        }

        internal int CurrentMapCount
        {
            get
            {
                if (Find.CurrentMap is Map map)
                {
                    return map.resourceCounter.GetCount(_shownItem);
                }

                return -1;
            }
        }

        internal string Description => _shownItem.description;

        internal Texture2D Icon => _shownItem.uiIcon;

        internal string Label => _shownItem.LabelCap;

        #endregion


        internal OptionInfo(FloatMenuOption option, int index)
        {
            _drawLayer = FieldAccessUtility.DrawPlaceHolderIconGetter(option);
            _iconThing = FieldAccessUtility.IconThingGetter(option);
            _itemIcon = FieldAccessUtility.ItemIconGetter(option);
            _shownItem = FieldAccessUtility.ShownItemGetter(option);

            _compositionColor = Color.white;
            _iconColor = option.iconColor;
            _iconTexCoords = option.iconTexCoords;
            _thingStyle = null;

            HasInfoCard = option.extraPartOnGUI != null;

            if (_shownItem != null)
            {
                IconStatus = IconStatus.FromDef;

                if (option.forceBasicStyle)
                {
                    _thingStyle = null;
                }
                else
                {
                    _thingStyle = FieldAccessUtility.ThingStyleGetter(option)
                        ?? Faction.OfPlayer.ideos?.PrimaryIdeo.GetStyleFor(_shownItem);
                }

                if (option.forceThingColor.HasValue)
                {
                    _compositionColor = option.forceThingColor.Value;
                }
                else
                {
                    _compositionColor = _shownItem.MadeFromStuff
                         ? _shownItem.GetColorForStuff(GenStuff.DefaultStuffFor(_shownItem))
                         : _shownItem.uiIconColor;
                }
            }
            else if (_itemIcon != null)
            {
                IconStatus = IconStatus.FromTexture;
            }
            else if (_iconThing != null)
            {
                IconStatus = IconStatus.FromThing;

                _shownItem = _iconThing.def;
            }
            else
            {
                IconStatus = _drawLayer
                    ? IconStatus.LayerOnly
                    : IconStatus.None;
            }

            if (IconStatus is IconStatus.None
                || IconStatus is IconStatus.FromTexture
                || IconStatus is IconStatus.LayerOnly)
            {
                _associatedBuild = null;

                ShowMaterialInfoWindow = false;
            }
            else
            {
                _associatedBuild = NCCMPatch.IsBuildPickerMenu
                    ? (NCCMPatch.BuildOptions[index] as Designator_Place)?.PlacingDef
                    : null;

                ShowMaterialInfoWindow =
                    (NCCMPatch.IsMaterialPickerMenu
                        && NCCMPatch.BuildRequireMaterial is BuildableDef def
                        && (def.CostStuffCount > 0
                            || def.costList.Count == 1))
                    ||
                    (NCCMPatch.IsBuildPickerMenu
                        && _associatedBuild != null
                        && _shownItem != null
                        && (_associatedBuild.CostStuffCount > 0
                            || _associatedBuild.costList.Count == 1));
            }
        }


        internal void DrawIcon(Rect iconRect)
        {
            if (_drawLayer)
            {
                Widgets.DrawTextureFitted(iconRect, Widgets.PlaceholderIconTex, 1f);
            }

            if (IconStatus is IconStatus.None
                || IconStatus is IconStatus.LayerOnly)
            {
                return;
            }

            Color color = GUI.color;
            GUI.color = new Color(
                _iconColor.r,
                _iconColor.g,
                _iconColor.b,
                _iconColor.a * color.a);

            if (IconStatus is IconStatus.FromDef)
            {
                Widgets.DefIcon(iconRect, _shownItem, thingStyleDef: _thingStyle, color: new Color(
                    _compositionColor.r,
                    _compositionColor.g,
                    _compositionColor.b,
                    _compositionColor.a * color.a));
            }
            else if (IconStatus is IconStatus.FromTexture)
            {
                Widgets.DrawTextureFitted(iconRect, _itemIcon, 1f, new Vector2(1f, 1f), _iconTexCoords);
            }
            else
            {
                Widgets.ThingIcon(iconRect, _iconThing);
            }

            GUI.color = color;
        }
    }
}
