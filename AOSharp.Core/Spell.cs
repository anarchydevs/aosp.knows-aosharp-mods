﻿using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Linq;
using AOSharp.Common.GameData;
using AOSharp.Core.Imports;
using AOSharp.Core.Combat;
using SmokeLounge.AOtomation.Messaging.Messages.N3Messages;
using AOSharp.Core.GameData;

namespace AOSharp.Core
{
    public class Spell : DummyItem, ICombatAction
    {
        private const float CAST_TIMEOUT = 0.5f;

        public readonly Identity Identity;
        public readonly Nanoline Nanoline;
        public readonly int StackingOrder;
        public bool IsReady => GetIsReady();

        public static IEnumerable<Spell> List => GetSpellList();
        public static bool HasPendingCast => _pendingCast.Spell != null;
        private static (Spell Spell, double Timeout) _pendingCast;

        internal unsafe Spell(Identity identity) : base(identity)
        {
            Identity = identity;
            Nanoline = (Nanoline)GetStat(Stat.NanoStrain);
        }

        public static bool Find(int id, out Spell spell)
        {
            return (spell = List.FirstOrDefault(x => x.Identity.Instance == id)) != null;
        }

        public static bool Find(string name, out Spell spell)
        {
            return (spell = List.FirstOrDefault(x => x.Name == name)) != null;
        }

        public void Cast()
        {
            Cast(DynelManager.LocalPlayer);
        }

        public void Cast(SimpleChar target)
        {
            Targeting.SetTarget(target);

            Network.Send(new CharacterActionMessage()
            {
                Action = CharacterActionType.CastNano,
                Target = target.Identity,
                Parameter1 = (int)Identity.Type,
                Parameter2 = Identity.Instance
            });

            _pendingCast = (this, Time.NormalTime + CAST_TIMEOUT);
        }

        private unsafe bool GetIsReady()
        {
            IntPtr pEngine = N3Engine_t.GetInstance();

            if (pEngine == IntPtr.Zero)
                return false;

            Identity identity = Identity;
            return N3EngineClientAnarchy_t.IsFormulaReady(pEngine, &identity) == 1;
        }

        internal static void Update()
        {
            if (_pendingCast.Spell != null && _pendingCast.Timeout <= Time.NormalTime)
                _pendingCast.Spell = null;
        }

        public static Spell[] GetSpellsForNanoline(Nanoline nanoline)
        {
            return List.Where(x => x.Nanoline == nanoline).ToArray();
        }

        private unsafe static Spell[] GetSpellList()
        {
            IntPtr pEngine = N3Engine_t.GetInstance();

            if (pEngine == IntPtr.Zero)
                return new Spell[0];

            return N3EngineClientAnarchy_t.GetNanoSpellList(pEngine)->ToList().Select(x => new Spell(new Identity(IdentityType.NanoProgram, (*(MemStruct*)x).Id))).ToArray();
        }

        public bool Equals(Spell other)
        {
            if (object.ReferenceEquals(other, null))
                return false;

            return Identity == other.Identity;
        }

        public static bool operator ==(Spell a, Spell b)
        {
            if (object.ReferenceEquals(a, null))
                return object.ReferenceEquals(b, null);

            return a.Equals(b);
        }


        public static bool operator !=(Spell a, Spell b) => !(a == b);

        [StructLayout(LayoutKind.Explicit, Pack = 0)]
        private struct MemStruct
        {
            [FieldOffset(0x08)]
            public int Id;
        }
    }
}