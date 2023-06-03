using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;

namespace Pathfinding {
	/// <summary>
	/// Helper for enabling or disabling compiler directives.
	/// Used only in the editor.
	/// </summary>
	public static class OptimizationHandler {
		public class DefineDefinition {
			public string name;
			public string description;
			public bool enabled;
			public bool consistent;
		}

		/// <summary>
		/// Various build targets that Unity have deprecated.
		/// There is apparently no way to figure out which these are without hard coding them.
		/// </summary>
		static readonly BuildTargetGroup[] deprecatedBuildTargets = new BuildTargetGroup[] {
			BuildTargetGroup.Unknown,
#if UNITY_5_4_OR_NEWER
			(BuildTargetGroup)16, /* BlackBerry */
#endif
#if UNITY_5_5_OR_NEWER
			(BuildTargetGroup)5, /* PS3 */
			(BuildTargetGroup)6, /* XBox360 */
			(BuildTargetGroup)15, /* WP8 */
#endif
#if UNITY_2017_4_OR_NEWER
			(BuildTargetGroup)2, /* WebPlayer */
			(BuildTargetGroup)20, /* PSM */
#endif
#if UNITY_2018_1_OR_NEWER
			(BuildTargetGroup)22, /* SamsungTV */
			(BuildTargetGroup)24, /* WiiU */
#endif
#if UNITY_2018_2_OR_NEWER
			(BuildTargetGroup)17, /* Tizen */
#endif
#if UNITY_2018_3_OR_NEWER
			(BuildTargetGroup)18, /* PSP2 */
			(BuildTargetGroup)23, /* Nintendo3DS */
#endif
		};

		static string GetPackageRootDirectory () {
			var rootDir = EditorResourceHelper.editorAssets + "/../../";

			return rootDir;
		}

		static Dictionary<BuildTargetGroup, List<string> > GetDefineSymbols () {
			var result = new Dictionary<BuildTargetGroup, List<string> >();

			var nonDeprecatedBuildTypes = typeof(BuildTargetGroup)
										  .GetFields(System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static)
										  .Where(fieldInfo => fieldInfo.GetCustomAttributes(typeof(System.ObsoleteAttribute), false).Length == 0)
										  .Select(fieldInfo => (BuildTargetGroup)fieldInfo.GetValue(null)).ToArray();

			for (int i = 0; i < nonDeprecatedBuildTypes.Length; i++) {
				// Kept for compatibility with older versions of Unity which did not always accurately add Obsolete attributes
				// (in particular Unity 2017.4 seems to miss marking the PSM build target as obsolete, the other ones seem accurate)
				if (deprecatedBuildTargets.Contains(nonDeprecatedBuildTypes[i])) continue;

#if UNITY_2021_3_OR_NEWER
				PlayerSettings.GetScriptingDefineSymbols(UnityEditor.Build.NamedBuildTarget.FromBuildTargetGroup(nonDeprecatedBuildTypes[i]), out var defines);
#else
				string defineString = PlayerSettings.GetScriptingDefineSymbolsForGroup(nonDeprecatedBuildTypes[i]);
				if (defineString == null) continue;

				var defines = defineString.Split(';').Select(s => s.Trim());
#endif
				result[nonDeprecatedBuildTypes[i]] = defines.ToList();
			}
			return result;
		}

		static void SetDefineSymbols (Dictionary<BuildTargetGroup, List<string> > symbols) {
			foreach (var pair in symbols) {
#if UNITY_2021_3_OR_NEWER
				string[] symbolsArr = pair.Value.Distinct().ToArray();
				PlayerSettings.SetScriptingDefineSymbols(UnityEditor.Build.NamedBuildTarget.FromBuildTargetGroup(pair.Key), symbolsArr);
#else
				var defineString = string.Join(";", pair.Value.Distinct().ToArray());
				PlayerSettings.SetScriptingDefineSymbolsForGroup(pair.Key, defineString);
#endif
			}
		}

		public static void EnableDefine (string name) {
			name = name.Trim();
			var newSymbols = GetDefineSymbols().ToDictionary(pair => pair.Key, pair => {
				pair.Value.Add(name);
				return pair.Value;
			});
			SetDefineSymbols(newSymbols);
		}

		public static void DisableDefine (string name) {
			name = name.Trim();
			var newSymbols = GetDefineSymbols().ToDictionary(pair => pair.Key, pair => {
				pair.Value.Remove(name);
				return pair.Value;
			});
			SetDefineSymbols(newSymbols);
		}

		public static void IsDefineEnabled (string name, out bool enabled, out bool consistent) {
			name = name.Trim();
			int foundEnabled = 0;
			int foundDisabled = 0;

			foreach (var pair in GetDefineSymbols()) {
				if (pair.Value.Contains(name)) {
					foundEnabled++;
				} else {
					foundDisabled++;
				}
			}

			enabled = foundEnabled > foundDisabled;
			consistent = (foundEnabled > 0) != (foundDisabled > 0);
		}

		public static List<DefineDefinition> FindDefines () {
			var path = GetPackageRootDirectory()+"/defines.csv";

			if (File.Exists(path)) {
				// Read a file consisting of lines with the format
				// NAME;Description
				// Ignore empty lines and lines which do not contain exactly 1 ';'
				var definePairs = File.ReadAllLines(path)
								  .Select(line => line.Trim())
								  .Where(line => line.Length > 0)
								  .Select(line => line.Split(';'))
								  .Where(opts => opts.Length == 2);

				return definePairs.Select(opts => {
					var def = new DefineDefinition { name = opts[0].Trim(), description = opts[1].Trim() };
					IsDefineEnabled(def.name, out def.enabled, out def.consistent);
					return def;
				}).ToList();
			}

			Debug.LogError("Could not find file '"+path+"'");
			return new List<DefineDefinition>();
		}

		public static void ApplyDefines (List<DefineDefinition> defines) {
			foreach (var define in defines) {
				if (define.enabled) {
					EnableDefine(define.name);
				} else {
					DisableDefine(define.name);
				}
			}
		}
	}
}
