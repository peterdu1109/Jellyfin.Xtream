// Copyright (C) 2025  Kevin Jilissen

// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.

// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.

// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <https://www.gnu.org/licenses/>.

using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Logging;

namespace Jellyfin.Xtream.Service;

/// <summary>
/// Service pour gérer les tokens de proxy sécurisés.
/// </summary>
public class StreamProxyService
{
    private readonly ILogger<StreamProxyService> _logger;
    private readonly ConcurrentDictionary<string, ProxyToken> _tokens = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="StreamProxyService"/> class.
    /// </summary>
    /// <param name="logger">Instance du logger.</param>
    public StreamProxyService(ILogger<StreamProxyService> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Génère un token sécurisé pour un stream.
    /// </summary>
    /// <param name="originalUrl">L'URL originale du stream.</param>
    /// <param name="expirationHours">Durée d'expiration en heures.</param>
    /// <returns>Le token généré.</returns>
    public string GenerateToken(string originalUrl, int expirationHours = 24)
    {
        // Générer un token unique
        string token = GenerateSecureToken();

        // Créer l'objet token avec expiration
        var proxyToken = new ProxyToken
        {
            Token = token,
            OriginalUrl = originalUrl,
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.AddHours(expirationHours)
        };

        // Stocker le token
        _tokens[token] = proxyToken;

        _logger.LogDebug("Token généré: {Token} (expire: {ExpiresAt})", token, proxyToken.ExpiresAt);

        // Nettoyer les tokens expirés
        CleanExpiredTokens();

        return token;
    }

    /// <summary>
    /// Résout un token pour obtenir l'URL originale.
    /// </summary>
    /// <param name="token">Le token à résoudre.</param>
    /// <returns>L'URL originale si le token est valide, null sinon.</returns>
    public string? ResolveToken(string token)
    {
        if (!_tokens.TryGetValue(token, out var proxyToken))
        {
            _logger.LogWarning("Token inconnu: {Token}", token);
            return null;
        }

        if (proxyToken.ExpiresAt < DateTime.UtcNow)
        {
            _logger.LogWarning("Token expiré: {Token}", token);
            _tokens.TryRemove(token, out _);
            return null;
        }

        _logger.LogDebug("Token résolu: {Token} -> {Url}", token, MaskUrl(proxyToken.OriginalUrl));
        return proxyToken.OriginalUrl;
    }

    /// <summary>
    /// Nettoie les tokens expirés.
    /// </summary>
    public void CleanExpiredTokens()
    {
        var now = DateTime.UtcNow;
        var expiredTokens = _tokens.Where(kvp => kvp.Value.ExpiresAt < now).Select(kvp => kvp.Key).ToList();

        foreach (var token in expiredTokens)
        {
            _tokens.TryRemove(token, out _);
        }

        if (expiredTokens.Count > 0)
        {
            _logger.LogInformation("Nettoyage de {Count} tokens expirés", expiredTokens.Count);
        }
    }

    /// <summary>
    /// Obtient le nombre de tokens actifs.
    /// </summary>
    /// <returns>Nombre de tokens actifs.</returns>
    public int GetActiveTokenCount()
    {
        CleanExpiredTokens();
        return _tokens.Count;
    }

    /// <summary>
    /// Génère un token sécurisé aléatoire.
    /// </summary>
    /// <returns>Token sécurisé.</returns>
    private static string GenerateSecureToken()
    {
        using var rng = RandomNumberGenerator.Create();
        var bytes = new byte[32];
        rng.GetBytes(bytes);
        return Convert.ToBase64String(bytes).Replace("+", "-", StringComparison.Ordinal).Replace("/", "_", StringComparison.Ordinal).Replace("=", string.Empty, StringComparison.Ordinal);
    }

    /// <summary>
    /// Masque une URL pour le logging (cache les credentials).
    /// </summary>
    /// <param name="url">L'URL à masquer.</param>
    /// <returns>URL masquée.</returns>
    private static string MaskUrl(string url)
    {
        if (string.IsNullOrEmpty(url))
        {
            return url;
        }

        // Masquer username/password dans l'URL
        var uri = new Uri(url);
        return $"{uri.Scheme}://{uri.Host}:***@{uri.PathAndQuery}";
    }

    /// <summary>
    /// Classe représentant un token de proxy.
    /// </summary>
    private sealed class ProxyToken
    {
        /// <summary>
        /// Gets or sets le token.
        /// </summary>
        public required string Token { get; set; }

        /// <summary>
        /// Gets or sets l'URL originale.
        /// </summary>
        public required string OriginalUrl { get; set; }

        /// <summary>
        /// Gets or sets la date de création.
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Gets or sets la date d'expiration.
        /// </summary>
        public DateTime ExpiresAt { get; set; }
    }
}
