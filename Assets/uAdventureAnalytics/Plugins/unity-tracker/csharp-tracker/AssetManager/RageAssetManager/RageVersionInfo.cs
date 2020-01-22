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
namespace AssetManagerPackage
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using System.Xml;
    using System.Xml.Serialization;

    /// <summary>
    /// Information about the rage version.
    /// </summary>
    [XmlRoot("version")]
    public class RageVersionInfo
    {
        /// <summary>
        /// Initializes a new instance of the AssetManagerPackage.RageVersionInfo
        /// class.
        /// </summary>
        public RageVersionInfo()
        {
            Dependencies = new Dependencies();
        }

        //<version>
        //  <id>asset</id>
        //  <major>1</major>
        //  <minor>2</minor>
        //  <build>3</build>
        //  <revision></revision>
        //  <maturity>alpha</maturity>
        //  <dependencies>
        //    <depends minVersion = "1.2.3" > Logger </ depends >
        //  </ dependencies >
        //</ version >

        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        ///
        /// <value>
        /// The identifier.
        /// </value>
        [XmlElement("id")]
        public String Id
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the major.
        /// </summary>
        ///
        /// <value>
        /// The major.
        /// </value>
        [XmlElement("major")]
        public Int32 Major
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the minor.
        /// </summary>
        ///
        /// <value>
        /// The minor.
        /// </value>
        [XmlElement("minor")]
        public Int32 Minor
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the build.
        /// </summary>
        ///
        /// <value>
        /// The build.
        /// </value>
        [XmlElement("build")]
        public Int32 Build
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the revision.
        /// </summary>
        ///
        /// <value>
        /// The revision.
        /// </value>
        [XmlElement("revision")]
        public Int32 Revision
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the maturity.
        /// </summary>
        ///
        /// <value>
        /// The maturity.
        /// </value>
        [XmlElement("maturity")]
        public String Maturity
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the dependencies.
        /// </summary>
        ///
        /// <value>
        /// The dependencies.
        /// </value>
        [XmlArray("dependencies")]
        [XmlArrayItem("depends")]
        public Dependencies Dependencies
        {
            get; set;
        }

        /// <summary>
        /// Loads version information.
        /// </summary>
        ///
        /// <param name="xml"> The XML. </param>
        public static RageVersionInfo LoadVersionInfo(String xml)
        {

            XmlSerializer ser = new XmlSerializer(typeof(RageVersionInfo));

            using (MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(xml)))
            {
                //! Use DataContractSerializer or DataContractJsonSerializer?
                //
                return (RageVersionInfo)ser.Deserialize(ms);
            }
        }

        /// <summary>
        /// Saves the version information.
        /// </summary>
        ///
        /// <returns>
        /// A String.
        /// </returns>
        public String SaveVersionInfo()
        {
            XmlSerializer ser = new XmlSerializer(GetType());

            using (StringWriterUtf8 textWriter = new StringWriterUtf8())
            {
                //! Use DataContractSerializer or DataContractJsonSerializer?
                // See https://msdn.microsoft.com/en-us/library/bb412170(v=vs.100).aspx
                // See https://msdn.microsoft.com/en-us/library/bb924435(v=vs.110).aspx
                // See https://msdn.microsoft.com/en-us/library/aa347875(v=vs.110).aspx
                //
                ser.Serialize(textWriter, this);

                textWriter.Flush();

                return textWriter.ToString();
            }
        }
    }

    /// <summary>
    /// A dependencies.
    /// </summary>
    [XmlRoot("dependencies")]
    public class Dependencies : List<Depends>
    {
        /// <summary>
        /// Initializes a new instance of the AssetManagerPackage.dependencies
        /// class.
        /// </summary>
        public Dependencies() : base()
        {
            //
        }

        //  <dependencies>
        //    <depends minVersion = "1.2.3" > Logger </ depends >
        //  </ dependencies >

    }

    /// <summary>
    /// A dependency.
    /// </summary>
    [XmlRoot("depends")]
    public class Depends
    {
        /// <summary>
        /// Initializes a new instance of the AssetManagerPackage.Dependency
        /// class.
        /// </summary>
        public Depends()
        {
            //
        }

        //    <depends minVersion = "1.2.3" > Logger </ depends >

        /// <summary>
        /// Gets or sets the minimum version.
        /// </summary>
        ///
        /// <value>
        /// The minimum version.
        /// </value>
        [XmlAttribute("minVersion")]
        public String minVersion
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the maximum version.
        /// </summary>
        ///
        /// <value>
        /// The maximum version.
        /// </value>
        [XmlAttribute("maxVersion")]
        public String maxVersion
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        ///
        /// <value>
        /// The name.
        /// </value>
        [XmlText]
        public String name
        {
            get; set;
        }
    }
}
