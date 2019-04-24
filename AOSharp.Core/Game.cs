﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AOSharp.Common.GameData;
using AOSharp.Core.Imports;
using AOSharp.Core.UI;
using AOSharp.Core.GameData;

namespace AOSharp.Core
{
    public static class Game
    {
        public delegate void OnUpdateEventHandler(float deltaTime);
        public static event OnUpdateEventHandler OnUpdate;

        public delegate void OnTeleportStartedEventHandler();
        public static event OnTeleportStartedEventHandler OnTeleportStarted;

        public delegate void OnTeleportEndedEventHandler();
        public static event OnTeleportEndedEventHandler OnTeleportEnded;

        public delegate void OnTeleportFailedEventHandler();
        public static event OnTeleportFailedEventHandler OnTeleportFailed;

        public static void SetMovement(MovementAction action)
        {
            IntPtr pEngine = N3Engine_t.GetInstance();

            if (pEngine == IntPtr.Zero)
                return;

            N3EngineClientAnarchy_t.MovementChanged(pEngine, action, 0, 0, true);
        }

        private static void OnUpdateInternal(float deltaTime)
        {
            UIController.UpdateViews();

            if (OnUpdate != null)
                OnUpdate(deltaTime);
        }

        private static void OnTeleportStartedInternal()
        {
            if (OnTeleportStarted != null)
                OnTeleportStarted();
        }

        private static void OnTeleportEndedInternal()
        {
            if (OnTeleportEnded != null)
                OnTeleportEnded();
        }

        private static void OnTeleportFailedInternal()
        {
            if (OnTeleportFailed != null)
                OnTeleportFailed();
        }
    }
}
