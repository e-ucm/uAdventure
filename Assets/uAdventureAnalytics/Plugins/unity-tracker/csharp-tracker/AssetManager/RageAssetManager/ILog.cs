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
    /// Interface for logger.
    /// </summary>
    public interface ILog
    {
        /// <summary>
        /// Executes the log operation.
        /// 
        /// Implement this in Game Engine Code.
        /// </summary>
        ///
        /// <param name="severity"> The severity. </param>
        /// <param name="msg">      The message. </param>
        void Log(Severity severity, String msg);
    }

    /// <summary>
    /// Values that represent log severity.
    /// <br/>
    ///      See
    /// <a href="https://msdn.microsoft.com/en-us/library/office/ff604025(v=office.14).aspx">Trace
    /// and Event Log Severity Levels</a>
    /// </summary>
    public enum Severity : int
    {
        /// <summary>
        /// An enum constant representing the critical option.
        /// </summary>
        Critical = 1,

        /// <summary>
        /// An enum constant representing the error option.
        /// </summary>
        Error = 2,

        /// <summary>
        /// An enum constant representing the warning option.
        /// </summary>
        Warning = 4,

        /// <summary>
        /// An enum constant representing the information option.
        /// </summary>
        Information = 8,

        /// <summary>
        /// An enum constant representing the verbose option.
        /// </summary>
        Verbose = 16
    }

    /// <summary>
    /// Values that represent log levels.
    /// </summary>
    public enum LogLevel : int
    {
        /// <summary>
        /// An enum constant representing the critical option.
        /// </summary>
        Critical = Severity.Critical,
        /// <summary>
        /// An enum constant representing the error option.
        /// </summary>
        Error = Critical | Severity.Error,
        /// <summary>
        /// An enum constant representing the warning option.
        /// </summary>
        Warn = Error | Severity.Warning,
        /// <summary>
        /// An enum constant representing the information option.
        /// </summary>
        Info = Warn | Severity.Information,
        /// <summary>
        /// An enum constant representing all option.
        /// </summary>
        All = Severity.Critical | Severity.Error | Severity.Warning | Severity.Information | Severity.Verbose,
    }
}
