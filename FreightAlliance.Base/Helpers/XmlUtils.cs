// --------------------------------------------------------------------------------------------------------------------
// <copyright file="XmlUtils.cs" company="">
//   
// </copyright>
// <summary>
//   The xml utils.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace FreightAlliance.Base.Helpers
{
    using System;
    using System.IO;
    using System.Xml.Serialization;

    /// <summary>
    /// The xml utils.
    /// </summary>
    public class XmlUtils
    {
        #region Public Methods

        /// <summary>
        /// The load xml.
        /// </summary>
        /// <param name="type">
        /// The type.
        /// </param>
        /// <param name="filename">
        /// The filename.
        /// </param>
        /// <returns>
        /// The <see cref="object"/>.
        /// </returns>
        public static object LoadXml(Type type, string filename)
        {
            object result = null;
            try
            {
                using (var reader = new StreamReader(filename))
                {
                    try
                    {
                        var serializer = new XmlSerializer(type);
                        result = serializer.Deserialize(reader);
                    }
                    catch (Exception e)
                    {
                        return null;
                    }
                    finally
                    {
                        reader.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                return null;
            }

            return result;
        }

        /// <summary>
        /// The save xml.
        /// </summary>
        /// <param name="obj">
        /// The obj.
        /// </param>
        /// <param name="filename">
        /// The filename.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public static bool SaveXml(object obj, string filename)
        {
            var result = false;
            using (var writer = new StreamWriter(filename))
            {
                try
                {
                    var ns = new XmlSerializerNamespaces();
                    ns.Add(string.Empty, string.Empty);
                    var serializer = new XmlSerializer(obj.GetType());
                    serializer.Serialize(writer, obj, ns);
                    result = true;
                }
                catch (Exception e)
                {
                    // Логирование
                }
                finally
                {
                    writer.Close();
                }
            }

            return result;
        }

        #endregion Public Methods
    }
}