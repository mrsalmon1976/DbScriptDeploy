using Newtonsoft.Json;
using DbScriptDeploy.UI.Models;
using DbScriptDeploy.UI.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using DbScriptDeploy.UI.Events;
using SystemWrapper.IO;
using StructureMap;
using NLog;

namespace DbScriptDeploy.UI.Services
{
    public interface ISettingsService
    {
        /// <summary>
        /// Gets the user settings.
        /// </summary>
        /// <returns></returns>
        Settings GetSettings();

        /// <summary>
        /// Saves the settings.
        /// </summary>
        /// <param name="settings">The settings.</param>
        void SaveSettings(Settings settings);
    }

    public class SettingsService : ISettingsService
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        private readonly string _settingsFilePath = null;
		private readonly IJsonPersistenceService _jsonPersistenceService = null;
        private Settings _settings = null;

        public SettingsService(string settingsFilePath, IJsonPersistenceService persistenceService) 
        {
			_jsonPersistenceService = persistenceService;
			_settingsFilePath = settingsFilePath;
        }


        /// <summary>
        /// Loads user settings from disk.
        /// </summary>
        /// <returns></returns>
        public Settings GetSettings()
        {
            if (_settings != null) return _settings;

			IFileWrap fileWrap = ObjectFactory.GetInstance<IFileWrap>();
            if (!fileWrap.Exists(_settingsFilePath))
            {
                _settings = Settings.DefaultSettings();
            }
            else
            {
                string settings = fileWrap.ReadAllText(_settingsFilePath);
                _settings = JsonConvert.DeserializeObject<Settings>(settings);
            }
            return _settings;
        }

        public void SaveSettings(Settings settings)
        {
            _settings = settings;
			_jsonPersistenceService.WriteFile(_settingsFilePath, _settings);
            logger.Info("Saved settings file {0}", _settingsFilePath);
        }

    }
}
