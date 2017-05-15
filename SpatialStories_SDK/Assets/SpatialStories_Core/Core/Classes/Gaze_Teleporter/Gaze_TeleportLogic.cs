using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gaze
{
    public abstract class Gaze_TeleportLogic
    {
        protected Gaze_Teleporter teleporter;

        public Gaze_TeleportLogic(Gaze_Teleporter _teleporter)
        {
            teleporter = _teleporter;
        }

        public abstract void Setup();
        public abstract void Dispose();
        public abstract void Update();
    }
}
