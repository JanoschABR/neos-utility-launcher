using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JanoschR.NeosUtilityLauncher {
    public class ConfigEntry {

        public string id = Guid.NewGuid().ToString();
        public bool enabled = true;

        public string name = null;
        public string command = null;
        public string[] arguments = null;

        public bool autoRestart = false;
        public bool killOnExit = false;

        public int maxRestarts = 16;
    }
}
