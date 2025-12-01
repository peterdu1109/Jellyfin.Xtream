# Jellyfin.Xtream
![GitHub Downloads (all assets, all releases)](https://img.shields.io/github/downloads/Kevinjil/Jellyfin.Xtream/total)
![GitHub Downloads (all assets, latest release)](https://img.shields.io/github/downloads/Kevinjil/Jellyfin.Xtream/latest/total)
![GitHub commits since latest release](https://img.shields.io/github/commits-since/Kevinjil/Jellyfin.Xtream/latest)
![Dynamic YAML Badge](https://img.shields.io/badge/dynamic/yaml?url=https%3A%2F%2Fraw.githubusercontent.com%2FKevinjil%2FJellyfin.Xtream%2Frefs%2Fheads%2Fmaster%2Fbuild.yaml&query=targetAbi&label=Jellyfin%20ABI)
![Dynamic YAML Badge](https://img.shields.io/badge/dynamic/yaml?url=https%3A%2F%2Fraw.githubusercontent.com%2FKevinjil%2FJellyfin.Xtream%2Frefs%2Fheads%2Fmaster%2Fbuild.yaml&query=framework&label=.NET%20framework)

Le plugin Jellyfin.Xtream permet d'intégrer le contenu fourni par une [API compatible Xtream](https://xtream-ui.org/api-xtreamui-xtreamcode/) dans votre instance [Jellyfin](https://jellyfin.org/).

## Fonctionnalités

- **TV en direct** : Diffusez des chaînes IPTV en direct
- **Vidéo à la demande** : Accédez à des films et vidéos
- **Séries** : Regardez des séries complètes avec gestion des saisons et épisodes
- **Rattrapage TV** : Revoir les programmes diffusés précédemment
- **Dossiers "Derniers ajouts"** : Accès rapide aux films et séries récemment ajoutés
- **Cache EPG configurable** : Contrôlez la durée de mise en cache des données EPG
- **Logique de réessai** : Récupération automatique en cas d'échec temporaire de l'API

## Installation

Le plugin peut être installé via un dépôt de plugins personnalisé.
Pour ajouter le dépôt, suivez ces étapes :

1. Ouvrez votre tableau de bord admin et naviguez vers `Plugins`.
2. Sélectionnez l'onglet `Repositories` en haut de la page.
3. Cliquez sur le symbole `+` pour ajouter un dépôt.
4. Entrez `Jellyfin.Xtream` comme nom de dépôt.
5. Entrez [`https://kevinjil.github.io/Jellyfin.Xtream/repository.json`](https://kevinjil.github.io/Jellyfin.Xtream/repository.json) comme URL du dépôt.
6. Cliquez sur sauvegarder.

Pour installer ou mettre à jour le plugin, suivez ces étapes :

1. Ouvrez votre tableau de bord admin et naviguez vers `Plugins`.
2. Sélectionnez l'onglet `Catalog` en haut de la page.
3. Sous `Live TV`, sélectionnez `Jellyfin Xtream`.
4. (Optionnel) Sélectionnez la version du plugin souhaitée.
5. Cliquez sur `Install`.
6. Redémarrez votre serveur Jellyfin pour terminer l'installation.

## Configuration

Le plugin nécessite les informations de connexion pour une [API compatible Xtream](https://xtream-ui.org/api-xtreamui-xtreamcode/).
Les identifiants suivants doivent être correctement définis dans l'onglet de configuration `Credentials` du plugin sur le tableau de bord admin.

| Propriété           | Description                                                                                     |
| ------------------- | ----------------------------------------------------------------------------------------------- |
| URL de base         | L'URL du point de terminaison de l'API sans barre oblique finale, incluant le protocole (http/https) |
| Nom d'utilisateur   | Le nom d'utilisateur utilisé pour s'authentifier à l'API                                        |
| Mot de passe        | Le mot de passe utilisé pour s'authentifier à l'API                                             |
| Durée du cache EPG  | La durée en minutes pour mettre en cache les données EPG (par défaut : 60 minutes)              |

### TV en direct

1. Ouvrez l'onglet de configuration `Live TV`.
2. Sélectionnez les catégories, ou les chaînes individuelles dans les catégories, que vous souhaitez rendre disponibles.
3. Cliquez sur `Save` en bas de la page.
4. Ouvrez l'onglet de configuration `TV Overrides`.
5. Modifiez les numéros de chaîne, les noms et les icônes si vous le souhaitez.
6. Cliquez sur `Save` en bas de la page.

### Vidéo à la demande

1. Ouvrez l'onglet de configuration `Video On-Demand`.
2. Activez `Show this channel to users`.
3. Sélectionnez les catégories, ou les vidéos individuelles dans les catégories, que vous souhaitez rendre disponibles.
4. Cliquez sur `Save` en bas de la page.
5. Un dossier **"Derniers Films"** sera automatiquement créé pour accéder rapidement aux films récemment ajoutés.

### Séries

1. Ouvrez l'onglet de configuration `Series`.
2. Activez `Show this channel to users`.
3. Sélectionnez les catégories, ou les séries individuelles dans les catégories, que vous souhaitez rendre disponibles.
4. Cliquez sur `Save` en bas de la page.
5. Un dossier **"Dernières Séries"** sera automatiquement créé pour accéder rapidement aux séries récemment ajoutées.

### Rattrapage TV

1. Ouvrez l'onglet de configuration `Live TV`.
2. Activez `Show the catch-up channel to users`.
3. Cliquez sur `Save` en bas de la page.

## Problèmes connus

### Perte de confidentialité

Jellyfin publie les chemins distants dans l'API et dans l'interface utilisateur par défaut.
Comme le format Xtream pour les chemins distants inclut le nom d'utilisateur et le mot de passe, toute personne ayant accès à la bibliothèque aura accès à vos identifiants.
Utilisez ce plugin avec précaution sur les serveurs partagés.

## Dépannage

Assurez-vous d'avoir correctement configuré votre [réseau Jellyfin](https://jellyfin.org/docs/general/networking/) :

1. Ouvrez votre tableau de bord admin et naviguez vers `Networking`.
2. Configurez correctement vos `Published server URIs`.
   Par exemple : `all=https://jellyfin.example.com`

### Problèmes de connexion à l'API

Le plugin inclut désormais une logique de réessai automatique qui tentera de se reconnecter jusqu'à 3 fois en cas d'échec temporaire de l'API. Si les problèmes persistent, vérifiez :

- La validité de vos identifiants
- La disponibilité du serveur Xtream
- Les journaux Jellyfin pour plus de détails

## Changelog

### Version 0.8.0.0

- ✨ Ajout des dossiers "Derniers Films" et "Dernières Séries"
- ⚙️ Cache EPG configurable (par défaut 60 minutes)
- 🔄 Logique de réessai automatique pour les appels API
- 🇫🇷 Interface utilisateur entièrement traduite en français
- 🐛 Corrections diverses et améliorations de stabilité
