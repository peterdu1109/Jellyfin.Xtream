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
using System.Net;
using Jellyfin.Xtream.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Jellyfin.Xtream.Api;

/// <summary>
/// Contrôleur pour le proxy de streams.
/// </summary>
[ApiController]
[Route("Xtream/Proxy")]
public class ProxyController : ControllerBase
{
    private readonly ILogger<ProxyController> _logger;
    private readonly StreamProxyService _proxyService;

    /// <summary>
    /// Initializes a new instance of the <see cref="ProxyController"/> class.
    /// </summary>
    /// <param name="logger">Instance du logger.</param>
    /// <param name="proxyService">Service de proxy.</param>
    public ProxyController(ILogger<ProxyController> logger, StreamProxyService proxyService)
    {
        _logger = logger;
        _proxyService = proxyService;
    }

    /// <summary>
    /// Redirige vers le stream réel via un token.
    /// </summary>
    /// <param name="token">Le token de proxy.</param>
    /// <returns>Redirection vers le stream.</returns>
    [HttpGet("{token}")]
    [AllowAnonymous]
    public IActionResult RedirectToStream(string token)
    {
        _logger.LogDebug("Demande de proxy pour token: {Token}", token);

        var originalUrl = _proxyService.ResolveToken(token);

        if (string.IsNullOrEmpty(originalUrl))
        {
            _logger.LogWarning("Token invalide ou expiré: {Token}", token);
            return NotFound(new { error = "Token invalide ou expiré" });
        }

        _logger.LogInformation("Redirection vers stream (token: {Token})", token);
        return Redirect(originalUrl);
    }

    /// <summary>
    /// Obtient les statistiques du proxy.
    /// </summary>
    /// <returns>Statistiques.</returns>
    [HttpGet("Stats")]
    [Authorize(Policy = "RequiresElevation")]
    public IActionResult GetStats()
    {
        var stats = new
        {
            ActiveTokens = _proxyService.GetActiveTokenCount(),
            Timestamp = DateTime.UtcNow
        };

        return Ok(stats);
    }

    /// <summary>
    /// Nettoie les tokens expirés.
    /// </summary>
    /// <returns>Résultat du nettoyage.</returns>
    [HttpPost("Clean")]
    [Authorize(Policy = "RequiresElevation")]
    public IActionResult CleanExpiredTokens()
    {
        var beforeCount = _proxyService.GetActiveTokenCount();
        _proxyService.CleanExpiredTokens();
        var afterCount = _proxyService.GetActiveTokenCount();
        var cleaned = beforeCount - afterCount;

        _logger.LogInformation("Nettoyage manuel: {Cleaned} tokens supprimés", cleaned);

        return Ok(new
        {
            TokensCleaned = cleaned,
            RemainingTokens = afterCount
        });
    }
}
