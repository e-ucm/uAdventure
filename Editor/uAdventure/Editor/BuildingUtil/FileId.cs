using System;
using System.Linq;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEditor;
using System.Reflection;

// Taken from http://www.superstarcoders.com/blogs/posts/md4-hash-algorithm-in-c-sharp.aspx
// Probably not the best implementation of MD4, but it works.

namespace uAdventure.Editor
{
    public class MD4 : HashAlgorithm
    {
        private uint _a;
        private uint _b;
        private uint _c;
        private uint _d;
        private uint[] _x;
        private int _bytesProcessed;

        public MD4()
        {
            _x = new uint[16];

            Initialize();
        }

        public override void Initialize()
        {
            _a = 0x67452301;
            _b = 0xefcdab89;
            _c = 0x98badcfe;
            _d = 0x10325476;

            _bytesProcessed = 0;
        }

        protected override void HashCore(byte[] array, int offset, int length)
        {
            ProcessMessage(Bytes(array, offset, length));
        }

        protected override byte[] HashFinal()
        {
            try
            {
                ProcessMessage(Padding());

                return new[] { _a, _b, _c, _d }.SelectMany(word => Bytes(word)).ToArray();
            }
            finally
            {
                Initialize();
            }
        }

        private void ProcessMessage(IEnumerable<byte> bytes)
        {
            foreach (byte b in bytes)
            {
                int c = _bytesProcessed & 63;
                int i = c >> 2;
                int s = (c & 3) << 3;

                _x[i] = (_x[i] & ~((uint)255 << s)) | ((uint)b << s);

                if (c == 63)
                {
                    Process16WordBlock();
                }

                _bytesProcessed++;
            }
        }

        private static IEnumerable<byte> Bytes(byte[] bytes, int offset, int length)
        {
            for (int i = offset; i < length; i++)
            {
                yield return bytes[i];
            }
        }

        private IEnumerable<byte> Bytes(uint word)
        {
            yield return (byte)(word & 255);
            yield return (byte)((word >> 8) & 255);
            yield return (byte)((word >> 16) & 255);
            yield return (byte)((word >> 24) & 255);
        }

        private IEnumerable<byte> Repeat(byte value, int count)
        {
            for (int i = 0; i < count; i++)
            {
                yield return value;
            }
        }

        private IEnumerable<byte> Padding()
        {
            return Repeat(128, 1)
               .Concat(Repeat(0, ((_bytesProcessed + 8) & 0x7fffffc0) + 55 - _bytesProcessed))
               .Concat(Bytes((uint)_bytesProcessed << 3))
               .Concat(Repeat(0, 4));
        }

        private void Process16WordBlock()
        {
            uint aa = _a;
            uint bb = _b;
            uint cc = _c;
            uint dd = _d;

            foreach (int k in new[] { 0, 4, 8, 12 })
            {
                aa = Round1Operation(aa, bb, cc, dd, _x[k], 3);
                dd = Round1Operation(dd, aa, bb, cc, _x[k + 1], 7);
                cc = Round1Operation(cc, dd, aa, bb, _x[k + 2], 11);
                bb = Round1Operation(bb, cc, dd, aa, _x[k + 3], 19);
            }

            foreach (int k in new[] { 0, 1, 2, 3 })
            {
                aa = Round2Operation(aa, bb, cc, dd, _x[k], 3);
                dd = Round2Operation(dd, aa, bb, cc, _x[k + 4], 5);
                cc = Round2Operation(cc, dd, aa, bb, _x[k + 8], 9);
                bb = Round2Operation(bb, cc, dd, aa, _x[k + 12], 13);
            }

            foreach (int k in new[] { 0, 2, 1, 3 })
            {
                aa = Round3Operation(aa, bb, cc, dd, _x[k], 3);
                dd = Round3Operation(dd, aa, bb, cc, _x[k + 8], 9);
                cc = Round3Operation(cc, dd, aa, bb, _x[k + 4], 11);
                bb = Round3Operation(bb, cc, dd, aa, _x[k + 12], 15);
            }

            unchecked
            {
                _a += aa;
                _b += bb;
                _c += cc;
                _d += dd;
            }
        }

        private static uint ROL(uint value, int numberOfBits)
        {
            return (value << numberOfBits) | (value >> (32 - numberOfBits));
        }

        private static uint Round1Operation(uint a, uint b, uint c, uint d, uint xk, int s)
        {
            unchecked
            {
                return ROL(a + ((b & c) | (~b & d)) + xk, s);
            }
        }

        private static uint Round2Operation(uint a, uint b, uint c, uint d, uint xk, int s)
        {
            unchecked
            {
                return ROL(a + ((b & c) | (b & d) | (c & d)) + xk + 0x5a827999, s);
            }
        }

        private static uint Round3Operation(uint a, uint b, uint c, uint d, uint xk, int s)
        {
            unchecked
            {
                return ROL(a + (b ^ c ^ d) + xk + 0x6ed9eba1, s);
            }
        }
    }

    public static class FileIDUtil
    {
        public static IEnumerable<Type> FindDerivedTypes(IEnumerable<Type> types, Type baseType)
        {
            return types.Where(t => baseType.IsAssignableFrom(t));
        }

        private static IEnumerable<Type> GetTypesInNamespace(string nameSpace)
        {
            return AppDomain.CurrentDomain.GetAssemblies()
                            .SelectMany(t => t.GetTypes())
                            .Where(t => t.IsClass && t.Namespace == nameSpace);
        }

        public static void GenerateGUIDMap()
        {
            // Last argument is output path
            var args = System.Environment.GetCommandLineArgs();
            string outputPath = args[args.Length - 1];

            var runnerAssemblyFile = "Assets/uAdventure/Plugins/uAdventureScripts.dll";
            var editorAssemblyFile = "Assets/uAdventure/Editor/Plugins/uAdventureEditor.dll";
            var trackerAssemblyFile = "Assets/uAdventure/Plugins/unity-tracker/UnityTracker.dll";

            Dictionary<Type, KeyValuePair<string, string>> guids = new Dictionary<Type, KeyValuePair<string, string>>();
            var monobehaviours = FindDerivedTypes(GetTypesInNamespace("uAdventure.Runner"), typeof(MonoBehaviour))
                .Select(t => new KeyValuePair<Type, string>(t, runnerAssemblyFile))
                        .Union(FindDerivedTypes(GetTypesInNamespace("uAdventure.GameSelector"), typeof(MonoBehaviour))
                .Select(t => new KeyValuePair<Type, string>(t, runnerAssemblyFile)))
                        .Union(FindDerivedTypes(GetTypesInNamespace("uAdventure.Editor"), typeof(EditorWindow))
                .Select(t => new KeyValuePair<Type, string>(t, editorAssemblyFile)))
                        .Union(FindDerivedTypes(GetTypesInNamespace("RAGE.Analytics"), typeof(MonoBehaviour))
                .Select(t => new KeyValuePair<Type, string>(t, trackerAssemblyFile)));

            var scripts = System.IO.Directory.GetFiles("./", "*.cs", System.IO.SearchOption.AllDirectories);

            foreach(var s in scripts)
            {
                Debug.Log("Script found: " + s);
            }

            foreach (var kv in monobehaviours)
            {
                // Iterate over all uAdventure monobehaviours
                Type type = kv.Key;
                string assemblyFile = kv.Value;
                Debug.Log("Type found: " + type.ToString());
                var path = Array.Find(scripts, s => s.EndsWith("\\" + type.Name + ".cs"));
                if (!string.IsNullOrEmpty(path))
                {
                    var cleanPath = path.Substring(2, path.Length - 2).Replace("\\", "/");
                    var guid = AssetDatabase.AssetPathToGUID(cleanPath);
                    guids[type] = new KeyValuePair<string, string>(guid, assemblyFile);
                    Debug.Log("Adding guid and file id for " + type.ToString() + ": " + Compute(type) + ", " + guid + ", " + assemblyFile);
                }
            }

            var fileInfo = new System.IO.FileInfo(outputPath);
            if (!System.IO.Directory.Exists(fileInfo.DirectoryName))
                System.IO.Directory.CreateDirectory(fileInfo.DirectoryName);
            System.IO.File.WriteAllText(outputPath, String.Join("\n", guids.Select(kv => kv.Value.Key + "," + Compute(kv.Key) + "," + kv.Value.Value).ToArray()));
        }

        public static void SwitchPrefabsGUIDsToDLL()
        {
            // Last two arguments are guidpath and dllpath
            var args = System.Environment.GetCommandLineArgs();
            string guidsPath = args[args.Length - 1];

            var text = System.IO.File.ReadAllLines(guidsPath);
            var guidToFileIdAndDllGUID = new Dictionary<string, KeyValuePair<string, string>>();
            var fileIdInDll = new Dictionary<string, string>();

            foreach (var tokens in text.Select(l => l.Split(',')))
            {
                guidToFileIdAndDllGUID[tokens[0]] = new KeyValuePair<string, string>(tokens[1], tokens[2]);
                fileIdInDll[tokens[1]] = tokens[2];
            }
            
            List<string> assetsToReimport = new List<string>();

            var layouts = System.IO.Directory.GetFiles(".\\", "*.wlt", System.IO.SearchOption.AllDirectories);
            Debug.Log("Layouts found: " + layouts.Length);
            foreach (var layout in layouts)
            {
                Debug.Log("Layout: " + layout);
                FixFile(layout, guidToFileIdAndDllGUID, fileIdInDll);
            }

            var scenes = System.IO.Directory.GetFiles(".\\", "*.unity", System.IO.SearchOption.AllDirectories);
            Debug.Log("Scenes found: " + scenes.Length);
            foreach (var scene in scenes)
            {
                Debug.Log("Scene: " + scene);
                if (FixFile(scene, guidToFileIdAndDllGUID, fileIdInDll))
                {
                    assetsToReimport.Add(scene);
                }
            }

            var prefabs = System.IO.Directory.GetFiles(".\\", "*.prefab", System.IO.SearchOption.AllDirectories);
            Debug.Log("Prefabs found: " + prefabs.Length);
            foreach (var prefab in prefabs)
            {
                Debug.Log("Prefab: " + prefab);
                if (FixFile(prefab, guidToFileIdAndDllGUID, fileIdInDll))
                {
                    assetsToReimport.Add(prefab);
                }
            }

            ImportAssets(assetsToReimport.ToArray());
        }
        private static void ImportAssets(string[] paths)
        {
            try
            {
                AssetDatabase.StartAssetEditing();
                foreach (string path in paths)
                    AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate | ImportAssetOptions.ImportRecursive);
            }
            finally
            {
                AssetDatabase.StopAssetEditing();
            }
        }

        private static bool FixFile(string file, Dictionary<string, KeyValuePair<string, string>> guidToFileIdAndDllGUID, Dictionary<string, string> fileIdInDll)
        {
            var fileText = System.IO.File.ReadAllLines(file);
            bool modified = false;
            for (int i = 0; i < fileText.Length; ++i)
            {
                if (fileText[i].StartsWith("MonoBehaviour:"))
                {
                    // Reactivate the object
                    fileText[i + 1] = "  m_ObjectHideFlags: 1";
                }

                if (fileText[i].StartsWith("  m_Script: "))
                {
                    Dictionary<string, string> attrs = ParseLine(fileText[i]);
                    var fileId = attrs["fileID"];
                    var isRuntime = fileId == "11500000";

                    var existsInRuntime = isRuntime && guidToFileIdAndDllGUID.ContainsKey(attrs["guid"]);
                    var existsCompiled = !isRuntime && fileIdInDll.ContainsKey(fileId);

                    if (!existsInRuntime && !existsCompiled)
                    {
                        if (isRuntime)
                        {
                            Debug.LogWarning("Couldn't find type for: " + attrs["guid"]);
                        }
                        continue;
                    }

                    string dll;
                    if(isRuntime)
                    {
                        // Is runtime script
                        var fileIdAndDll = guidToFileIdAndDllGUID[attrs["guid"]];
                        fileId = fileIdAndDll.Key;
                        dll = fileIdAndDll.Value;
                    }
                    else
                    {
                        // Is editor script
                        dll = fileIdInDll[fileId];
                    }

                    var guid = AssetDatabase.AssetPathToGUID(dll);
                    if (string.IsNullOrEmpty(guid))
                    {
                        Debug.LogWarning("Failed to get GUID for: " + dll);
                        continue;
                    }

                    attrs["fileID"] = fileId;
                    attrs["guid"] = guid;
                    fileText[i] = "  m_Script: " + EncodeLine(attrs);
                    Debug.Log("Fixed! " + fileText[i]);
                    modified = true;
                }
                /*var prefabAsset = AssetDatabase.LoadAllAssetsAtPath(prefab)[0];
                Debug.Log(prefabAsset.GetType());
                var prefabObject = new SerializedObject(prefabAsset);
                // Iterate over all uAdventure monobehaviours
                foreach (var obj in prefabObject.FindProperty("m_Component"))
                {
                    var property = obj as SerializedProperty;
                    Debug.Log("Component:");
                    var reference = property.FindPropertyRelative("component").objectReferenceValue;
                    Debug.Log(reference);
                    foreach(var val in reference.GetIterator())
                    {
                        var varlProp = val as SerializedProperty;
                        Debug.Log(varlProp.name);
                    }
                    var scriptProperty = reference.FindProperty("m_Script");
                    if(scriptProperty != null)
                    {
                        Debug.Log(scriptProperty.serializedObject);
                    }
                }*/
            }

            if (modified)
            {
                System.IO.File.WriteAllLines(file, fileText);
            }

            return modified;
        }

        private static string EncodeLine(Dictionary<string, string> attrs)
        {
            return "{" + attrs.Select(kv => kv.Key + ": " + kv.Value).Aggregate((s1, s2) => s1 + ", " + s2) + "}";
        }

        private static Dictionary<string, string> ParseLine(string line)
        {
            var start = line.IndexOf("{");
            string areaToParse = line.Substring(line.IndexOf("{") + 1, line.IndexOf("}") - start - 1);
            string[] tokens = areaToParse.Split(',');

            Dictionary<string, string> attrs = new Dictionary<string, string>();
            foreach (var token in tokens)
            {
                var dotsPos = token.IndexOf(':');
                var key = token.Substring(0, dotsPos).Trim();
                var value = token.Substring(dotsPos + 1).Trim();
                attrs.Add(key, value);
            }

            return attrs;
        }

        public static int Compute(Type t)
        {
            return ComputeName(t.Namespace + t.Name);
        }
        public static int ComputeName(string name)
        {
            string toBeHashed = "s\0\0\0" + name;

            using (HashAlgorithm hash = new MD4())
            {
                byte[] hashed = hash.ComputeHash(System.Text.Encoding.UTF8.GetBytes(toBeHashed));

                int result = 0;

                for (int i = 3; i >= 0; --i)
                {
                    result <<= 8;
                    result |= hashed[i];
                }

                return result;
            }
        }
    }
}