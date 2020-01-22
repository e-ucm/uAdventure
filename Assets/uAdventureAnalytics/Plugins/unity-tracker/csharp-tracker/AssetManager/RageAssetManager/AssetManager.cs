/*
 * Copyright 2016 Open University of the Netherlands
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * This project has received funding from the European Union’s Horizon
 * 2020 research and innovation programme under grant agreement No 644187.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
#region platform notes
//
//! Embedded Resources & Android (does seem to apply to pure android code, not the pcl/sap):
//  See http://developer.xamarin.com/guides/android/application_fundamentals/resources_in_android/part_6_-_using_android_assets/ 
//  See https://github.com/xamarin/mobile-samples/blob/master/EmbeddedResources/EmbeddedResources.Droid/MainActivity.cs
// 
//! Naming:
//  Xamarin also seems to contain a class called AssetManager.
//  Xamarin.Droid refuses to load embedded resources outside the 'assets' directory.
//  
//! Deployment
//  Fast Deployment on Android causes problems with resources.
// 
//! Localization:
//   See http://developer.xamarin.com/guides/cross-platform/xamarin-forms/localization/
//   ResX files seem to be supported.
//
//! WinPhone/PCL
//  WinPhone seems to prefer PCL type assemblies. If the asset code is recompiled as PCL the Android project fails on a List<String> return type. 
//  The IDataStorage.Files() return type has been changed to String[].
//  Xpath doesn't seem to be supported in PCL (so a rewrite to the XmlSerializer of Version code was neccesary.
#endregion platform notes

namespace AssetManagerPackage
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;
    using AssetPackage;

    /// <summary>
    /// Manager for assets.
    /// </summary>
    public class AssetManager
    {
        #region Fields

        /// <summary>
        /// The instance.
        /// </summary>
        static readonly AssetManager _instance = new AssetManager();

        /// <summary>
        /// The assets.
        /// </summary>
        private Dictionary<String, IAsset> assets = new Dictionary<String, IAsset>();

        /// <summary>
        /// The identifier generator.
        /// </summary>
        private Int32 idGenerator = 0;

        /// <summary>
        /// The logger.
        /// </summary>
        private ILog logger = null;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Explicit static constructor tells # compiler not to mark type as beforefieldinit.
        /// </summary>
        static AssetManager()
        {
            //
        }

        /// <summary>
        /// Prevents a default instance of the AssetManager class from being created.
        /// </summary>
        private AssetManager()
        {
            // Nothing
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Visible when reflecting.
        /// </summary>
        ///
        /// <value>
        /// The instance.
        /// </value>
        public static AssetManager Instance
        {
            get
            {
                return _instance;
            }
        }

        /// <summary>
        /// Gets or sets the bridge.
        /// </summary>
        ///
        /// <value>
        /// The bridge.
        /// </value>
        public IBridge Bridge
        {
            get;
            set;
        }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Searches for the first asset by class.
        /// </summary>
        ///
        /// <param name="claz"> The claz. </param>
        ///
        /// <returns>
        /// The found asset by class.
        /// </returns>
        public IAsset findAssetByClass(String claz)
        {
            Regex mask = new Regex(String.Format(@"{0}_(\d+)", claz));

            return assets.First(p => mask.IsMatch(p.Key)).Value;
        }

        /// <summary>
        /// Searches for the first asset by identifier.
        /// </summary>
        ///
        /// <param name="id"> The identifier. </param>
        ///
        /// <returns>
        /// The found asset by identifier.
        /// </returns>
        public IAsset findAssetById(String id)
        {
            return assets[id];
        }

        /// <summary>
        /// Searches for assets by class.
        /// </summary>
        ///
        /// <param name="claz"> The claz. </param>
        ///
        /// <returns>
        /// The found assets by class.
        /// </returns>
        public List<IAsset> findAssetsByClass(String claz)
        {
            Regex mask = new Regex(String.Format(@"{0}_(\d+)", claz));

            // Return the values of all matching keys using the regex.
            return assets.Where(p => mask.IsMatch(p.Key)).Select(p => p.Value).ToList();
        }

        /// <summary>
        /// Registers the asset instance.
        /// </summary>
        ///
        /// <param name="asset"> The asset. </param>
        /// <param name="claz">  The claz. </param>
        ///
        /// <returns>
        /// A String.
        /// </returns>
        public String registerAssetInstance(IAsset asset, String claz)
        {
            foreach (KeyValuePair<String, IAsset> kvp in assets)
            {
                if (asset.Equals(kvp.Value))
                {
                    return kvp.Key;
                }
            }

            String Id = String.Format("{0}_{1}", claz, idGenerator++);

            Log(Severity.Verbose, "Registering Asset {0}/{1} as {2}", asset.GetType().Name, claz, Id);

            assets.Add(Id, asset);

            Log(Severity.Verbose, "Registered {0} Asset(s)", assets.Count);

            return Id;
        }

        /// <summary>
        /// Reports version and dependencies.
        /// </summary>
        ///
        /// <value>
        /// The version and dependencies report.
        /// </value>
        public String VersionAndDependenciesReport
        {
            get
            {
                const Int32 col1w = 40;
                const Int32 col2w = 32;

                StringBuilder report = new StringBuilder();

                report.AppendFormat("{0}{1}", "Asset".PadRight(col1w), "Depends on").AppendLine();
                report.AppendFormat("{0}+{1}", "".PadRight(col1w - 1, '-'), "".PadRight(col2w, '-')).AppendLine();

                foreach (KeyValuePair<String, IAsset> asset in assets)
                {
                    report.Append(String.Format("{0} v{1}", asset.Value.Class, asset.Value.Version).PadRight(col1w - 1));

                    // Console.WriteLine("[{0}]\r\n{1}=v{2}\t;{3}", asset.Key, asset.Value.Class, asset.Value.Version, asset.Value.Maturity);
                    Int32 cnt = 0;
                    foreach (KeyValuePair<String, String> dependency in asset.Value.Dependencies)
                    {
                        //! Better version matches (see Microsoft).
                        // 
                        //! https://msdn.microsoft.com/en-us/library/system.version(v=vs.110).aspx
                        //
                        //! dependency.value has min-max format (inclusive) like:
                        // 
                        //? v1.2.3-*        (v1.2.3 or higher)
                        //? v0.0-*          (all versions)
                        //? v1.2.3-v2.2     (v1.2.3 or higher less than or equal to v2.1)
                        //
                        String[] vrange = dependency.Value.Split('-');

                        Version low = null;

                        Version hi = null;

                        switch (vrange.Length)
                        {
                            case 1:
                                low = new Version(vrange[0]);
                                hi = low;
                                break;
                            case 2:
                                low = new Version(vrange[0]);
                                if (vrange[1].Equals("*"))
                                {
                                    hi = new Version(99, 99);
                                }
                                else
                                {
                                    hi = new Version(vrange[1]);
                                }
                                break;

                            default:
                                break;
                        }

                        Boolean found = false;

                        if (low != null)
                        {
                            foreach (IAsset dep in findAssetsByClass(dependency.Key))
                            {
                                // Console.WriteLine("Dependency {0}={1}",dep.Class, dep.Version);
                                Version vdep = new Version(dep.Version);
                                if (low <= vdep && vdep <= hi)
                                {
                                    found = true;
                                    break;
                                }
                            }

                            report.AppendFormat("|{0} v{1} [{2}]", dependency.Key, dependency.Value, found ? "resolved" : "missing").AppendLine();
                        }
                        else
                        {
                            report.AppendLine("error");
                        }

                        if (cnt != 0)
                        {
                            report.Append("".PadRight(col1w - 1));
                        }

                        cnt++;
                    }

                    if (cnt == 0)
                    {
                        report.AppendFormat("|{0}", "No dependencies").AppendLine();
                    }
                }

                report.AppendFormat("{0}+{1}", "".PadRight(col1w - 1, '-'), "".PadRight(col2w, '-')).AppendLine();

                return report.ToString();
            }
        }

        /// <summary>
        /// Logs.
        /// </summary>
        ///
        /// <param name="loglevel"> The loglevel. </param>
        /// <param name="format">   Describes the format to use. </param>
        /// <param name="args">     A variable-length parameters list containing
        ///                         arguments. </param>
        public void Log(Severity loglevel, String format, params object[] args)
        {
            Log(loglevel, String.Format(format, args));
        }

        /// <summary>
        /// Logs.
        /// </summary>
        ///
        /// <param name="loglevel"> The loglevel. </param>
        /// <param name="msg">      The message. </param>
        public void Log(Severity loglevel, String msg)
        {
            logger = getInterface<ILog>();

            if (logger != null)
            {
                logger.Log(loglevel, msg);
            }
        }

        /// <summary>
        /// Gets the interface.
        /// </summary>
        ///
        /// <typeparam name="T"> Generic type parameter. </typeparam>
        ///
        /// <returns>
        /// The interface.
        /// </returns>
        protected T getInterface<T>()
        {
            if (Bridge != null && Bridge is T)
            {
                return (T)Bridge;
            }

            return default(T);
        }

        /*
        /// <summary>
        /// Clears the registration.
        /// </summary>
        /// <remarks>Used for cleaning up in test suites (as static readonly _instance member cannot be destroyed).</remarks>
        [System.Diagnostics.Conditional("DEBUG")]
        public void ClearRegistration()
        {
            idGenerator = 0;

            assets.Clear();
        }
        */

        #endregion Methods

    }
}