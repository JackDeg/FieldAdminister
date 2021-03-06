using System;
using System.Collections.Generic;
using HarmonyLib;
using Verse;
using RimWorld;
using UnityEngine;
using Verse.AI;
using System.Linq;

namespace JackDeg_FieldAdminister
{
    [HarmonyPatch(typeof(FloatMenuMakerMap))]
    [HarmonyPatch("AddHumanlikeOrders")]
    [HarmonyPatch(new Type[] { typeof(Vector3), typeof(Pawn), typeof(List<FloatMenuOption>) })]
    static class FloatMenuMakerMap_Modify_AddHumanlikeOrders
    {
        public static Action MakeAction(Thing drug, Pawn pawn, Pawn patient)
        {
            Action action = delegate
            {
                Job job = new Job(FieldAdminister_JobDefOf.AdministerDrugs, patient, drug);
                job.count = 1;
                pawn.jobs.TryTakeOrderedJob(job);
            };
            return action;
        }
        [HarmonyPostfix]
        static void AddMenuItems(Vector3 clickPos, Pawn pawn, List<FloatMenuOption> opts)
        {
            // Stabilize
            if (pawn.health.capacities.CapableOf(PawnCapacityDefOf.Manipulation))
            {
                foreach (LocalTargetInfo curTarget in GenUI.TargetsAt(clickPos, TargetingParameters.ForRescue(pawn), true))
                {
                    Pawn patient = (Pawn)curTarget.Thing;
                    if (patient.Downed
                        && pawn.CanReach(patient, PathEndMode.InteractionCell, Danger.Deadly))
                    {
                        IEnumerable<Thing> drugItems = new List<Thing>();
                        if (FASettings.alldrugs)
                        {
                            drugItems = pawn.inventory.GetDrugs();
                        } 
                        else
                        {
                            List<String> possibleDrugs = Drugs.druglist;

                            foreach (var i in pawn.inventory.GetDrugs())
                            {
                                if (possibleDrugs.Contains(i.def.defName))
                                {
                                    drugItems = drugItems.Concat(i);
                                }
                            }
                        }
                        
                        if (pawn.WorkTypeIsDisabled(WorkTypeDefOf.Doctor))
                        {
                            opts.Add(new FloatMenuOption("FA_DoctorDisabled".Translate(), null, MenuOptionPriority.Default));
                            return;
                        }
                        string label;
                        foreach (Thing drug in drugItems)
                        {
                            label = "FA_Administer".Translate(drug.LabelNoCount, patient.LabelCap);
                            opts.Add(FloatMenuUtility.DecoratePrioritizedTask(new FloatMenuOption(label, MakeAction(drug, pawn, patient), MenuOptionPriority.Default, null, patient), pawn, patient, "ReservedBy"));
                        }
                    }
                }
            }
        }
    }
}
