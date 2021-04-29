using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using Verse;
using RimWorld;
using UnityEngine;
using Verse.AI;
using JackDeg_FieldAdminister;

namespace JackDeg_FieldAdminister
{
    [HarmonyPatch(typeof(FloatMenuMakerMap))]
    [HarmonyPatch("AddHumanlikeOrders")]
    [HarmonyPatch(new Type[] { typeof(Vector3), typeof(Pawn), typeof(List<FloatMenuOption>) })]
    static class FloatMenuMakerMap_Modify_AddHumanlikeOrders
    {
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

                        Thing gojuiceItem = null;
                        foreach (var i in pawn.inventory.GetDrugs())
                        {
                            if (i is GoJuice)
                            {
                                gojuiceItem = i;
                            }
                        }

                        if (pawn.WorkTypeIsDisabled(WorkTypeDefOf.Doctor) && (gojuiceItem == null || !(gojuiceItem is GoJuice)))
                        {
                            opts.Add(new FloatMenuOption("FieldMedic_CannotStabilize".Translate() + " (" + "IncapableOfCapacity".Translate(WorkTypeDefOf.Doctor.gerundLabel) + ")", null, MenuOptionPriority.Default));
                            return;
                        }
                        if (pawn.WorkTypeIsDisabled(WorkTypeDefOf.Doctor))
                        {
                            opts.Add(new FloatMenuOption("FieldMedic_CannotStabilize".Translate() + " (" + "IncapableOfCapacity".Translate(WorkTypeDefOf.Doctor.gerundLabel) + ")", null, MenuOptionPriority.Default));
                            return;
                        }
                        if (gojuiceItem == null || !(gojuiceItem is GoJuice))
                        {
                            //opts.Add(new FloatMenuOption("FieldMedic_CannotStabilize".Translate() + " (" + "FieldMedic_NoMedicBag".Translate() + ")", null, MenuOptionPriority.Default));
                            return;
                        }

                        string label = "FieldMedic_Stabilize".Translate(patient.LabelCap);
                        Action action = delegate
                        {
                            var medicbag = (GoJuice)gojuiceItem;
                            Job job = new Job(FieldAdminister_JobDefOf.AdministerGoJuice, patient, medicbag);
                            job.count = 1;
                            pawn.jobs.TryTakeOrderedJob(job);
                        };
                        opts.Add(FloatMenuUtility.DecoratePrioritizedTask(new FloatMenuOption(label, action, MenuOptionPriority.Default, null, patient), pawn, patient, "ReservedBy"));
                    }
                }
            }
        }
    }
}
