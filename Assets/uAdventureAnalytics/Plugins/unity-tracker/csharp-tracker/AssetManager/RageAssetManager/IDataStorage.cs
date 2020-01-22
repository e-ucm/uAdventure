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

    /// <summary>
    /// Interface for data storage.
    /// </summary>
    public interface IDataStorage
    {
        #region Methods

        /// <summary>
        /// Deletes the given fileId.
        /// </summary>
        ///
        /// <param name="fileId"> The file identifier to delete. </param>
        ///
        /// <returns>
        /// true if it succeeds, false if it fails.
        /// </returns>
        Boolean Delete(String fileId);

        /// <summary>
        /// Check if exists the file with the given identifier.
        /// </summary>
        ///
        /// <param name="fileId"> The file identifier to delete. </param>
        ///
        /// <returns>
        /// true if it succeeds, false if it fails.
        /// </returns>
        Boolean Exists(String fileId);

        /// <summary>
        /// Gets the files.
        /// </summary>
        ///
        /// <remarks>
        /// A List&lt;String&gt; gave problems when compiled as PCL and added to a
        /// Xamarin Forms project containing iOS, Android and WinPhone subprojects.
        /// </remarks>
        ///
        /// <returns>
        /// An array of filenames.
        /// </returns>
        String[] Files();

        /// <summary>
        /// Loads the given file.
        /// </summary>
        ///
        /// <param name="fileId"> The file identifier to load. </param>
        ///
        /// <returns>
        /// A String with with the file contents.
        /// </returns>
        String Load(String fileId);

        /// <summary>
        /// Saves the given file.
        /// </summary>
        ///
        /// <param name="fileId">   The file identifier to delete. </param>
        /// <param name="fileData"> Information describing the file. </param>
        void Save(String fileId, String fileData);

        #endregion Methods
    }
}