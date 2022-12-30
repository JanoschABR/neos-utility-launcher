using FrooxEngine;
using NeosModLoader;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace JanoschR.NeosUtilityLauncher {
    public class ModCore : NeosMod {

        public override string Name => "NeosUtilityLauncher";
        public override string Author => "JanoschR";
        public override string Version => "1.0.0";
        public override string Link => "";

        public static ConfigFile config = null;

        public override void OnEngineInit () {
            string path = Path.Combine(Engine.Current.AppPath, "nml_mods", $"{Name}.json");
            Engine.Current.OnShutdown += OnEngineShutdown;

            if (!File.Exists(path)) {
                Error($"No config file found. (Expected location: {path})");
                return;
            }

            string json = File.ReadAllText(path);
            config = JsonConvert.DeserializeObject<ConfigFile>(json);

            if (config == null) {
                Error($"Config could not be deserialized.");
                return;
            }

            if (config.entries == null) return;
            foreach (ConfigEntry entry in config.entries) {
                HandleApplicationConfig(entry);
            }
        }

        protected static List<Tuple<ConfigEntry, Process>> processes = new List<Tuple<ConfigEntry, Process>>();
        protected static Dictionary<string, int> restarts = new Dictionary<string, int>();

        protected void HandleApplicationConfig (ConfigEntry entry) {
            Msg($"Starting {entry.name}...");

            Process process = new Process();
            process.StartInfo = new ProcessStartInfo() {
                FileName = entry.command,
                Arguments = string.Join(" ", entry.arguments),
                ErrorDialog = false,
                UseShellExecute = true
            };

            if (entry.maxRestarts > 64) {
                Warn($"{entry.name} has a very high (64+) restart limit!");
            }

            process.EnableRaisingEvents = true;
            process.Exited += new EventHandler((a, b) => HandleApplicationExited(entry, process));
            process.Start();

            processes.Add(new Tuple<ConfigEntry, Process>(entry, process));
        }

        protected void HandleApplicationExited (ConfigEntry entry, Process process) {
            Warn($"Process {process.Id} ({entry.name}) has exited! " +
                (entry.autoRestart ? $"({restarts.Get(entry.id, 1)}/{entry.maxRestarts})" : ""));

            if (entry.autoRestart) {
                restarts.Increment(entry.id);

                if (restarts[entry.id] <= entry.maxRestarts) {
                    HandleApplicationConfig(entry);
                } else {
                    Error($"Process {process.Id} ({entry.name}) has reached the restart limit.");
                }
            }
        }

        protected void OnEngineShutdown() {
            
            foreach (Tuple<ConfigEntry, Process> tuple in processes) {
                var config = tuple.Item1;
                var process = tuple.Item2;

                if (config.killOnExit) {
                    Warn($"Killing process {process.Id} ({config.name})...");
                    
                    try {
                        process.Kill();
                    } catch (Exception) {}
                }
            }
        }
    }
}
