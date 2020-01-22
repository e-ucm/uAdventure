namespace AssetManagerPackage
{
    using System;
    using System.IO;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Text;
    /// <summary>
    /// A rage extensions.
    /// </summary>
    public static class RageExtensions
    {
        /// <summary>
        /// A Type extension method that assemblies the given type.
        /// </summary>
        ///
        /// <remarks>
        /// Fix for incompatability between normal and portable code.
        /// </remarks>
        ///
        /// <param name="type"> The type to act on. </param>
        ///
        /// <returns>
        /// An Assembly.
        /// </returns>
        public static Assembly Assembly(this Type type)
        {
#if PORTABLE
            return type.GetTypeInfo().Assembly;
#else
            return type.Assembly;
#endif
        }

#if PORTABLE
        /// <summary>
        /// A Type extension method that gets a property.
        /// </summary>
        ///
        /// <remarks>
        /// Fix for incompatability between normal and portable code.
        /// </remarks>
        ///
        /// <param name="type"> The type to act on. </param>
        /// <param name="name"> The name. </param>
        ///
        /// <returns>
        /// The property.
        /// </returns>
        public static PropertyInfo GetProperty(this Type type, String name)
        {
            return type.GetRuntimeProperty(name);
        }
#endif

        /// <summary>
        /// A Type extension method that query if 'type' is class.
        /// </summary>
        ///
        /// <param name="type"> The type to act on. </param>
        ///
        /// <returns>
        /// true if class, false if not.
        /// </returns>
        public static Boolean IsClassFix(this Type type)
        {
#if PORTABLE
            return type.GetTypeInfo().IsClass;
#else
            return type.IsClass;
#endif
        }

        /// <summary>
        /// A Type extension method that query if 'type' is primitive.
        /// </summary>
        ///
        /// <param name="type"> The type to act on. </param>
        ///
        /// <returns>
        /// true if primitive, false if not.
        /// </returns>
        public static Boolean IsPrimitiveFix(this Type type)
        {
#if PORTABLE
            return type.GetTypeInfo().IsPrimitive;
#else
            return type.IsPrimitive;
#endif
        }

        /// <summary>
        /// A Type extension method that query if 'type' is serializable.
        /// </summary>
        ///
        /// <param name="type"> The type to act on. </param>
        ///
        /// <returns>
        /// true if serializable, false if not.
        /// </returns>
        public static Boolean IsSerializableFix(this Type type)
        {
#if PORTABLE
            return type.GetTypeInfo().IsSerializable;
#else
            return type.IsSerializable;
#endif
        }

        /// <summary>
        /// A Type extension method that query if 'type' is serializable.
        /// </summary>
        ///
        /// <param name="type"> The type to act on. </param>
        /// <param name="name"> The name. </param>
        ///
        /// <returns>
        /// true if serializable, false if not.
        /// </returns>
        public static MethodInfo MethodInfoFix(this Type type, String name)
        {
#if PORTABLE
            return type.GetTypeInfo().GetDeclaredMethod(name);
#else
            return type.GetMethod(name);
#endif
        }

        /// <summary>
        /// A Type extension method that query if 'type' is serializable.
        /// </summary>
        ///
        /// <param name="type">  The type to act on. </param>
        /// <param name="name">  The name. </param>
        /// <param name="types"> The types. </param>
        ///
        /// <returns>
        /// true if serializable, false if not.
        /// </returns>
        public static MethodInfo MethodInfoFix(this Type type, String name, Type[] types)
        {
#if PORTABLE

#warning we need to take types in account for some methods. Use type.GetTypeInfo().DeclaredMethods[0].GetParameters[0].ParamaterType
            return type.GetTypeInfo().GetDeclaredMethod(name/*, types*/);
#else
            return type.GetMethod(name, types);
#endif
        }

        /// <summary>
        /// Gets method information.
        /// </summary>
        ///
        /// <param name="expression"> The expression. </param>
        ///
        /// <returns>
        /// The method information.
        /// </returns>
        public static MethodInfo GetMethodInfo(Expression<Action> expression)
        {
            // Adapted from see http://blog.functionalfun.net/2009/10/getting-methodinfo-of-generic-method.html
            // 
            return GetMethodInfo((LambdaExpression)expression);
        }

        /// <summary>
        /// Gets method information.
        /// </summary>
        ///
        /// <typeparam name="T"> Generic type parameter. </typeparam>
        /// <param name="lex"> The expression. </param>
        ///
        /// <returns>
        /// The method information.
        /// </returns>
        public static MethodInfo GetMethodInfo<T>(Expression<Action<T>> lex)
        {
            // Adapted from see http://blog.functionalfun.net/2009/10/getting-methodinfo-of-generic-method.html
            // 
            return GetMethodInfo((LambdaExpression)lex);
        }

        /// <summary>
        /// Gets method information.
        /// </summary>
        ///
        /// <param name="lex"> The expression. </param>
        ///
        /// <returns>
        /// The method information.
        /// </returns>
        public static MethodInfo GetMethodInfo(LambdaExpression lex)
        {
            // Adapted from http://blog.functionalfun.net/2009/10/getting-methodinfo-of-generic-method.html
            // 
            MethodCallExpression bodyExpression = (lex.Body as MethodCallExpression);

            return bodyExpression == null ? null : bodyExpression.Method;
        }
    }


    /// <summary>
    /// A StringWriter UTF8.
    /// </summary>
    ///
    /// <remarks>
    /// Fix-up for XDocument Serialization defaulting to utf-16.
    /// </remarks>
    public class StringWriterUtf8 : StringWriter
    {
        #region Properties

        /// <summary>
        /// Gets the <see cref="T:System.Text.Encoding" /> in which the output is
        /// written.
        /// </summary>
        ///
        /// <value>
        /// The Encoding in which the output is written.
        /// </value>
        public override Encoding Encoding
        {
            get
            {
                return Encoding.UTF8;
            }
        }

        #endregion Properties
    }

#if PORTABLE
    /// <summary>
    /// Interface for serializable.
    /// </summary>
    public interface ISerializable
    {
#warning Not a good fixup as SerializationInfo is also missing in PCL (and everything around it using Binary Serialization)
    }
#endif
}
