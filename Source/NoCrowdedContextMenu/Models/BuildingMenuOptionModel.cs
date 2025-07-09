using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace NoCrowdedContextMenu.Models
{
    public readonly struct BuildingMenuOptionModel
    {
        public static readonly BuildingMenuOptionModel Empty = new BuildingMenuOptionModel();


        //------------------------------------------------------
        //
        //  Public Fields
        //
        //------------------------------------------------------

        #region Public Fields

        public readonly BuildableDef Building;

        public readonly int Cost;
        public readonly ThingDef Material;

        public readonly bool IsValid;

        #endregion


        public int ResourceCount => Find.CurrentMap?.resourceCounter.GetCount(Material) ?? -1;


        internal BuildingMenuOptionModel(BuildableDef building, ThingDef material)
        {
            Building = building;
            Cost = building.CostStuffCount;

            if (Cost < 1)
            {
                Cost = Building.costList[0].count;
            }
            else if (material.smallVolume)
            {
                Cost *= 10;
            }

            Material = material;
            IsValid = true;
        }
    }
}
