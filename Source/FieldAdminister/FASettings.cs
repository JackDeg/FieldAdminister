using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace JackDeg_FieldAdminister
{
    public class FASettings : ModSettings
    {
        public static bool alldrugs;

        public override void ExposeData()
        {
            Scribe_Values.Look(ref alldrugs, "alldrugs");
            base.ExposeData();
        }
    }

    public class FAMod : Mod
    {
        FASettings settings;

        public FAMod(ModContentPack content) : base(content)
        {
            this.settings = GetSettings<FASettings>();
        }

        public override void DoSettingsWindowContents(Rect inRect)
        {
            Listing_Standard listing_Standard = new Listing_Standard();
            listing_Standard.Begin(inRect);
            listing_Standard.CheckboxLabeled("Administer any drug?", ref FASettings.alldrugs, "Administer any drug");
            listing_Standard.End();
            base.DoSettingsWindowContents(inRect);
        }

        public override string SettingsCategory()
        {
            return "Field Administer";
        }
    }
}
