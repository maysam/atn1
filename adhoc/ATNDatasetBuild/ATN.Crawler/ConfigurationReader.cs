using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace ATN.Crawler
{
    /// <summary>
    /// Supporting class to retrieve data out of appropriate .config files
    /// </summary>
    public static class ConfigurationReader
    {
        /// <summary>
        /// Static method to retrieve an object out of a configuration file
        /// </summary>
        /// <param name="Key">The name of the object to return</param>
        /// <param name="Type">The type of the object to return</param>
        /// <returns>The appropriate configuration value, or null if it does not exist</returns>
        public static T GetConfigurationValue<T>(string Key)
        {
            AppSettingsReader Reader = new AppSettingsReader();
            object objValue = Reader.GetValue(Key, typeof(T));
            if (objValue != null)
            {
                return (T)objValue;
            }
            else
            {
                throw new ConfigurationKeyException();
            }
        }
    }
    #region Supporting classes and types
    public class ConfigurationKeyException : Exception
    {
        public ConfigurationKeyException() : base("The searched for configuration key was not found") { }
    }
    #endregion
}
