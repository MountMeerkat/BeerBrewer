using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeerBrewerApp.Services
{
    public interface IConfigurationService
    {
        public abstract bool Exists(string key);
        public abstract void Delete(string key);
        public abstract string GetString(string key);
        public abstract bool GetBool(string key);
        public abstract int GetInt(string key);
        public abstract double GetDouble(string key);
        public abstract DateTime GetDateTime(string key);
        public abstract void SetString(string key, string value);
        public abstract void SetBool(string key, bool value);
        public abstract void SetInt(string key, int value);
        public abstract void SetDouble(string key, double value);
        public abstract void SetDateTime(string key, DateTime value);
    }
    internal class ConfigurationService : IConfigurationService
    {
        Dictionary<string, object> _loadedConfigurations;

        public ConfigurationService()
        {
            _loadedConfigurations = new Dictionary<string, object>();
        }
        public bool Exists(string key)
        {
            return _loadedConfigurations.ContainsKey(key) || Preferences.Default.ContainsKey(key);
        }
        public void Delete(string key)
        {
            if (Exists(key))
            {
                _loadedConfigurations.Remove(key);
                Preferences.Default.Remove(key);
            }
        }
        public string GetString(string key)
        {
            if (_loadedConfigurations.ContainsKey(key))
            {
                return _loadedConfigurations[key].ToString();
            }
            if (Preferences.Default.ContainsKey(key))
            {
                string value = Preferences.Default.Get(key, "404");
                _loadedConfigurations[key] = value;
                return value;
            }
            return null;
        }
        public bool GetBool(string key)
        {
            if (_loadedConfigurations.ContainsKey(key))
            {
                object rawValue = _loadedConfigurations[key];
                if (rawValue is bool value) return value;
            }
            if (Preferences.Default.ContainsKey(key))
            {
                bool value = Preferences.Default.Get(key, false);
                _loadedConfigurations[key] = value;
                return value;
            }
            return false;
        }
        public int GetInt(string key)
        {
            if (_loadedConfigurations.ContainsKey(key))
            {
                object rawValue = _loadedConfigurations[key];
                if (rawValue is int value) return value;
            }
            if (Preferences.Default.ContainsKey(key))
            {
                int value = Preferences.Default.Get(key, -1);
                _loadedConfigurations[key] = value;
                return value;
            }
            return -1;
        }
        public double GetDouble(string key)
        {
            if (_loadedConfigurations.ContainsKey(key))
            {
                object rawValue = _loadedConfigurations[key];
                if (rawValue is double value) return value;
            }
            if (Preferences.Default.ContainsKey(key))
            {
                double value = Preferences.Default.Get(key, -1);
                _loadedConfigurations[key] = value;
                return value;
            }
            return -1;
        }
        public DateTime GetDateTime(string key)
        {
            if (_loadedConfigurations.ContainsKey(key))
            {
                object rawValue = _loadedConfigurations[key];
                if (rawValue is DateTime value) return value;
            }
            if (Preferences.Default.ContainsKey(key))
            {
                DateTime value = Preferences.Default.Get(key, DateTime.Now);
                _loadedConfigurations[key] = value;
                return value;
            }
            return DateTime.Now;
        }
        public void SetString(string key, string value)
        {
            if (_loadedConfigurations.ContainsKey(key))
            {
                _loadedConfigurations.Remove(key);
                _loadedConfigurations.Add(key, value);
            }
            else
            {
                _loadedConfigurations.Add(key, value);
            }
            if (Preferences.Default.ContainsKey(key))
            {
                Preferences.Default.Remove(key);
                Preferences.Default.Set(key, value);
            }
            else
            {
                Preferences.Default.Set(key, value);
            }
        }
        public void SetBool(string key, bool value)
        {
            if (_loadedConfigurations.ContainsKey(key))
            {
                _loadedConfigurations.Remove(key);
                _loadedConfigurations.Add(key, value);
            }
            else
            {
                _loadedConfigurations.Add(key, value);
            }
            if (Preferences.Default.ContainsKey(key))
            {
                Preferences.Default.Remove(key);
                Preferences.Default.Set(key, value);
            }
            else
            {
                Preferences.Default.Set(key, value);
            }
        }
        public void SetInt(string key, int value)
        {
            if (_loadedConfigurations.ContainsKey(key))
            {
                _loadedConfigurations.Remove(key);
                _loadedConfigurations.Add(key, value);
            }
            else
            {
                _loadedConfigurations.Add(key, value);
            }
            if (Preferences.Default.ContainsKey(key))
            {
                Preferences.Default.Remove(key);
                Preferences.Default.Set(key, value);
            }
            else
            {
                Preferences.Default.Set(key, value);
            }
        }
        public void SetDouble(string key, double value)
        {
            if (_loadedConfigurations.ContainsKey(key))
            {
                _loadedConfigurations.Remove(key);
                _loadedConfigurations.Add(key, value);
            }
            else
            {
                _loadedConfigurations.Add(key, value);
            }
            if (Preferences.Default.ContainsKey(key))
            {
                Preferences.Default.Remove(key);
                Preferences.Default.Set(key, value);
            }
            else
            {
                Preferences.Default.Set(key, value);
            }
        }
        public void SetDateTime(string key, DateTime value)
        {
            if (_loadedConfigurations.ContainsKey(key))
            {
                _loadedConfigurations.Remove(key);
                _loadedConfigurations.Add(key, value);
            }
            else
            {
                _loadedConfigurations.Add(key, value);
            }
            if (Preferences.Default.ContainsKey(key))
            {
                Preferences.Default.Remove(key);
                Preferences.Default.Set(key, value);
            }
            else
            {
                Preferences.Default.Set(key, value);
            }
        }
    }
}
