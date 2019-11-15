// Copyright 2019 Varjo Technologies Oy. All rights reserved.

using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

/// <summary>
/// A helper class for reading key-value pairs from Varjo configuration files
/// </summary>
public class VarjoConfiguration
{
    private readonly IDictionary<String, String> config = new Dictionary<String, String>();

    /// <summary>
    /// Loads configuration from given file. Throws an error if load fails.
    /// </summary>
    /// <param name="configfile">Path of the configuration file</param>
    public void Load(string configfile)
    {
        config.Clear();

        var lines = File.ReadAllLines(configfile);
        foreach (var line in lines)
        {
            // Skip empty and comment lines
            if ((line.Length == 0) || (line[0] == '#')) continue;

            // Find equals and skip line if not found
            var offset = line.IndexOf("=");
            if (offset <= 0) continue;

            // Split key value pair and add to dictionary
            var key = line.Substring(0, offset);
            var value = line.Substring(offset + 1);
            if (!config.ContainsKey(key))
            {
                config.Add(key, value);
            }
        }
    }

    /// <summary>
    /// Gets a value from the configuration
    /// </summary>
    /// <param name="key">Key of the value</param>
    /// <param name="defaultValue">Default value that is returned, if key is not found</param>
    /// <returns>Value from the loaded configuration</returns>
    public string GetValue(string key, string defaultValue)
    {
        string value;
        if (!config.TryGetValue(key, out value))
        {
            value = defaultValue;
        }

        return value;
    }

    /// <summary>
    /// Gets a value from the configuration and interprets it as boolean
    /// </summary>
    /// <param name="key">Key of the value</param>
    /// <param name="defaultValue">Default value that is returned, if key is not found</param>
    /// <returns>Value from the loaded configuration</returns>
    public bool GetBoolean(string key, bool defaultValue)
    {
        string value = GetValue(key, null);
        if (value == null)
        {
            return defaultValue;
        }

        value = value.ToLowerInvariant();
        return (value == "true") || (value == "on") || (value == "1");
    }

    /// <summary>
    /// Gets a value from the configuration and interprets it as integer
    /// </summary>
    /// <param name="key">Key of the value</param>
    /// <param name="defaultValue">Default value that is returned, if key is not found
    /// or if value could not be converted to integer</param>
    /// <returns>Value from the loaded configuration</returns>
    public int GetInteger(string key, int defaultValue)
    {
        string value = GetValue(key, null);
        if (value == null)
        {
            return defaultValue;
        }

        int result;
        if (!Int32.TryParse(value, out result))
        {
            Debug.LogWarning("Failed to parse configuation value (" + key + "=" + value + ") as integer.");
            return defaultValue;
        }

        return result;
    }
}
