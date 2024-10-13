define(['baseView', 'loading', 'emby-input', 'emby-button', 'emby-checkbox', 'emby-scroller', 'emby-select'], function (BaseView, loading) {
    'use strict';

    var phoenixAdultConfig = {
        pluginUniqueId: '91FDA366-558B-4A18-AAB8-2DC8627D2EFC'
    };

    function onSubmit(e) {

        e.preventDefault();

        var instance = this;
        var form = this.view;

        loading.show();

        ApiClient.getPluginConfiguration(phoenixAdultConfig.pluginUniqueId).then(function (config) {

            config.DisableCaching = form.querySelector('.DisableCaching').checked;
            config.DisableSSLCheck = form.querySelector('.DisableSSLCheck').checked;
            config.ProxyEnable = form.querySelector('.ProxyEnable').checked;
            config.ProxyHost = form.querySelector('.ProxyHost').value;
            config.ProxyPort = form.querySelector('.ProxyPort').value;
            config.ProxyLogin = form.querySelector('.ProxyLogin').value;
            config.ProxyPassword = form.querySelector('.ProxyPassword').value;
            config.FlareSolverrURL = form.querySelector('.FlareSolverrURL').value;
            config.UseFilePath = form.querySelector('.UseFilePath').checked;
            config.DefaultSiteName = form.querySelector('.DefaultSiteName').value;
            config.UseMetadataAPI = form.querySelector('.UseMetadataAPI').checked;
            config.MetadataAPIToken = form.querySelector('.MetadataAPIToken').value;
            config.DisableActors = form.querySelector('.DisableActors').checked;
            config.DisableGenres = form.querySelector('.DisableGenres').checked;
            config.DisableImageValidation = form.querySelector('.DisableImageValidation').checked;
            config.DisableImageSize = form.querySelector('.DisableImageSize').checked;
            config.DisableAutoIdentify = form.querySelector('.DisableAutoIdentify').checked;
            config.PreferedActorNameSource = form.querySelector('.PreferedActorNameSource').value;
            config.JAVActorNamingStyle = form.querySelector('.JAVActorNamingStyle').value;
            config.GenresSortingStyle = form.querySelector('.GenresSortingStyle').value;

            ApiClient.updatePluginConfiguration(phoenixAdultConfig.pluginUniqueId, config).then(Dashboard.processPluginConfigurationUpdateResult);
        });

        // Disable default form submission
        return false;
    }

    function View(view, params) {
        BaseView.apply(this, arguments);

        view.querySelector('form').addEventListener('submit', onSubmit.bind(this));
    }

    Object.assign(View.prototype, BaseView.prototype);

    View.prototype.onResume = function (options) {

        BaseView.prototype.onResume.apply(this, arguments);

        var instance = this;

        loading.show();
        ApiClient.getPluginConfiguration(phoenixAdultConfig.pluginUniqueId).then(function (config) {
            var view = instance.view;

            view.querySelector('.DisableCaching').checked = config.DisableCaching;
            view.querySelector('.DisableSSLCheck').checked = config.DisableSSLCheck;
            view.querySelector('.ProxyEnable').checked = config.ProxyEnable;
            view.querySelector('.ProxyHost').value = config.ProxyHost;
            view.querySelector('.ProxyPort').value = config.ProxyPort;
            view.querySelector('.ProxyLogin').value = config.ProxyLogin;
            view.querySelector('.ProxyPassword').value = config.ProxyPassword; 
            view.querySelector('.FlareSolverrURL').value = config.FlareSolverrURL;
            view.querySelector('.UseFilePath').checked = config.UseFilePath;
            view.querySelector('.DefaultSiteName').value = config.DefaultSiteName;
            view.querySelector('.UseMetadataAPI').checked = config.UseMetadataAPI;
            view.querySelector('.MetadataAPIToken').value = config.MetadataAPIToken;
            view.querySelector('.DisableActors').checked = config.DisableActors;
            view.querySelector('.DisableGenres').checked = config.DisableGenres;
            view.querySelector('.DisableImageValidation').checked = config.DisableImageValidation;
            view.querySelector('.DisableImageSize').checked = config.DisableImageSize;
            view.querySelector('.DisableAutoIdentify').checked = config.DisableAutoIdentify;
            view.querySelector('.PreferedActorNameSource').value = config.PreferedActorNameSource;
            view.querySelector('.JAVActorNamingStyle').value = config.JAVActorNamingStyle;
            view.querySelector('.GenresSortingStyle').value = config.GenresSortingStyle;

            loading.hide();
        });
    };

    return View;
});
