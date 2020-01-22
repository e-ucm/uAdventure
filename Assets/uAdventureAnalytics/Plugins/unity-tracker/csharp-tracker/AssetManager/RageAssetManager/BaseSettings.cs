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
namespace AssetPackage
{
    using System;
    using System.ComponentModel;
    using System.Reflection;

    /// <summary>
    /// A base settings.
    /// </summary>
    public class BaseSettings : ISettings
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the Swiss.BaseSettings class.
        /// </summary>
        public BaseSettings()
        {
            //! Initialize Settings to their specified default values.
            UpdateDefaultValues();
        }

        #endregion Constructors

        #region Methods

        /// <summary>
        /// Set the value of (Public Instance) properties to the <see cref="DefaultValueAttribute"/>'s
        /// Value of that property.
        /// </summary>
        private void UpdateDefaultValues()
        {
            BaseSettings.UpdateDefaultValues(this);
        }

        /// <summary>
        /// Set the value of (Public Instance) properties to the
        /// <see cref="DefaultValueAttribute"/>'s Value of that property.
        /// </summary>
        ///
        /// <param name="obj"> The object. </param>
        public static void UpdateDefaultValues(Object obj)
        {
            // GetProperties not PCL
            // BindingFlags not PCL
            // foreach (PropertyInfo pi in obj.GetType().GetRuntimeProperties(/*BindingFlags.Instance | BindingFlags.Public*/))
#if PORTABLE
            foreach (PropertyInfo pi in obj.GetType().GetRuntimeProperties())
#else
            foreach (PropertyInfo pi in obj.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public))
#endif

            {
                Boolean found = false;

                if (pi.CanWrite)
                {
                    foreach (Object att in pi.GetCustomAttributes(typeof(DefaultValueAttribute), false))
                    {
                        if (att is DefaultValueAttribute)
                        {
                            // Pretty Print Assigned Values.
                            // 
                            Object val = ((DefaultValueAttribute)att).Value;

                            if (val == null)
                            {
                                val = "null";
                            }
                            else if (val is String && String.IsNullOrEmpty(val.ToString()))
                            {
                                val = "\"\"";
                            }
                            else
                            {
                                val = String.Format("{0}", val);
                            }

                            found = true;

                            if (pi.CanWrite)
                            {
                                pi.SetValue(obj, ((DefaultValueAttribute)att).Value, new object[] { });

                                // Debug.WriteLine(String.Format("Updated {0}.{1} to {2}", obj.GetType().Name, pi.Name, val));
                            }
                            else
                            {
                                // Debug.WriteLine(String.Format("Error Updating Default Value of {0}.{1}", obj.GetType().Name, pi.Name));
                            }

                            break;
                        }
                    }

                    if (found)
                    {
                        continue;
                    }

                    // Debug.WriteLine(String.Format("No Default Value for {0}.{1}", obj.GetType().Name, pi.Name));
                }
            }
        }

        #endregion Methods
    }
}