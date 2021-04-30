using RimWorld;
using System.Collections.Generic;
using Verse;
using Verse.AI;

namespace JackDeg_FieldAdminister
{
    public class JobDriver_AdministerDrugs : JobDriver
    {
        private const float baseTendDuration = 60f;

        private Pawn Patient { get { return pawn.CurJob.targetA.Thing as Pawn; } }
        private Thing Drug { get { return pawn.CurJob.targetB.Thing; } }

        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            return pawn.Reserve(TargetA, job) && pawn.Reserve(TargetB, job);
        }
        protected override IEnumerable<Toil> MakeNewToils()
        {
            this.FailOn(() => Patient == null || Drug == null);
            this.FailOnDespawnedNullOrForbidden(TargetIndex.A);
            this.FailOnDestroyedNullOrForbidden(TargetIndex.B);
            this.FailOnNotDowned(TargetIndex.A);

            bool deleted = false;

            this.AddEndCondition(delegate
            {
                if (!deleted) return JobCondition.Ongoing;
                return JobCondition.Incompletable;
            });

            // Pick up drug and haul to patient
            yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.InteractionCell);

            // drug patient
            Log.Message("starting toil");
            int duration = (int)(1f / this.pawn.GetStatValue(StatDefOf.MedicalTendSpeed, true) * baseTendDuration);
            Toil waitToil = Toils_General.Wait(duration).WithProgressBarToilDelay(TargetIndex.A).PlaySustainerOrSound(SoundDefOf.Interact_Tend);
            yield return waitToil;
            Toil administerToil = new Toil();
            administerToil.initAction = delegate
            {
                Drug.Ingested(Patient, 0);
                deleted = true;
            };
            administerToil.defaultCompleteMode = ToilCompleteMode.Instant;
            yield return administerToil;
            yield return Toils_Jump.Jump(waitToil);
        }
    }
}

