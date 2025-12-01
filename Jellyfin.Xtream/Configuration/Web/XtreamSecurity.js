export default function (view) {
    view.addEventListener("viewshow", () => import(
        window.ApiClient.getUrl("web/ConfigurationPage", {
            name: "Xtream.js",
        })
    ).then((Xtream) => Xtream.default
    ).then((Xtream) => {
        const pluginId = Xtream.pluginConfig.UniqueId;
        Xtream.setTabs(4); // Index de l'onglet Sécurité

        Dashboard.showLoadingMsg();
        ApiClient.getPluginConfiguration(pluginId).then(function (config) {
            view.querySelector('#UseSecureProxy').checked = config.UseSecureProxy;
            view.querySelector('#TokenExpirationHours').value = config.TokenExpirationHours || 24;
            view.querySelector('#MaxConcurrentStreams').value = config.MaxConcurrentStreams || 0;
            view.querySelector('#EnableDetailedLogging').checked = config.EnableDetailedLogging;
            Dashboard.hideLoadingMsg();
        });

        const loadStats = () => {
            fetch('/Xtream/Proxy/Stats', {
                headers: {
                    'X-Emby-Token': ApiClient.accessToken()
                }
            }).then(response => response.json())
                .then(data => {
                    view.querySelector('#ActiveTokens').innerText = data.ActiveTokens || 0;
                    view.querySelector('#LastUpdate').innerText = new Date().toLocaleString('fr-FR');
                }).catch(() => {
                    view.querySelector('#ActiveTokens').innerText = 'Erreur';
                    view.querySelector('#LastUpdate').innerText = '-';
                });
        };

        // Charger les stats au démarrage
        loadStats();

        // Bouton refresh
        view.querySelector('#RefreshStats').addEventListener('click', () => {
            loadStats();
        });

        // Bouton clean
        view.querySelector('#CleanTokens').addEventListener('click', () => {
            fetch('/Xtream/Proxy/Clean', {
                method: 'POST',
                headers: {
                    'X-Emby-Token': ApiClient.accessToken()
                }
            }).then(response => response.json())
                .then(data => {
                    Dashboard.alert(`${data.TokensCleaned} tokens nettoyés. ${data.RemainingTokens} tokens restants.`);
                    loadStats();
                }).catch((error) => {
                    Dashboard.alert('Erreur lors du nettoyage des tokens.');
                });
        });

        // Formulaire de sauvegarde
        view.querySelector('#XtreamSecurityForm').addEventListener('submit', (e) => {
            Dashboard.showLoadingMsg();

            ApiClient.getPluginConfiguration(pluginId).then((config) => {
                config.UseSecureProxy = view.querySelector('#UseSecureProxy').checked;
                config.TokenExpirationHours = parseInt(view.querySelector('#TokenExpirationHours').value, 10);
                config.MaxConcurrentStreams = parseInt(view.querySelector('#MaxConcurrentStreams').value, 10);
                config.EnableDetailedLogging = view.querySelector('#EnableDetailedLogging').checked;

                ApiClient.updatePluginConfiguration(pluginId, config).then((result) => {
                    Dashboard.processPluginConfigurationUpdateResult(result);
                });
            });

            e.preventDefault();
            return false;
        });
    }));
}
