using RimWorld;
using System.Collections.Generic;
using Verse;
using Verse.AI;

namespace JackDeg_FieldAdminister
{
    public class JobDriver_AdministerGoJuice : JobDriver
    {
        private const float baseTendDuration = 60f;

        private Pawn Patient { get { return pawn.CurJob.targetA.Thing as Pawn; } }
        private GoJuice GoJuice { get { return pawn.CurJob.targetB.Thing as GoJuice; } }

        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            return pawn.Reserve(TargetA, job) && pawn.Reserve(TargetB, job);
        }
        protected override IEnumerable<Toil> MakeNewToils()
        {
            this.FailOn(() => Patient == null || GoJuice == null);
            this.FailOnDespawnedNullOrForbidden(TargetIndex.A);
            this.FailOnDestroyedNullOrForbidden(TargetIndex.B);
            this.FailOnNotDowned(TargetIndex.A);
            this.AddEndCondition(delegate
            {
                if (Patient.health.Downed) return JobCondition.Ongoing;
                GoJuice.Destroy();
                return JobCondition.Incompletable;
            });

            // Pick up GoJuice and haul to patient
            yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.InteractionCell);

            // Stabilize patient
            Log.Message("starting toil");
            int duration = (int)(1f / this.pawn.GetStatValue(StatDefOf.MedicalTendSpeed, true) * baseTendDuration);
            Toil waitToil = Toils_General.Wait(duration).WithProgressBarToilDelay(TargetIndex.A).PlaySustainerOrSound(SoundDefOf.Interact_Tend);
            yield return waitToil;
            Toil administerToil = new Toil();
            administerToil.initAction = delegate
            {
                GoJuice.Ingested(Patient, 0);
            };
            administerToil.defaultCompleteMode = ToilCompleteMode.Instant;
            yield return administerToil;
            yield return Toils_Jump.Jump(waitToil);
        }
    }
}

