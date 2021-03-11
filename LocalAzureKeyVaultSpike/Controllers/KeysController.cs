using System;
using LocalAzureKeyVaultSpike.Models;
using LocalAzureKeyVaultSpike.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LocalAzureKeyVaultSpike.Controllers
{
    [ApiController]
    [Route("keys")]
    [Authorize]
    public class KeysController : ControllerBase
    {
        private readonly IKeyVaultKeyService _keyVaultKeyService;

        public KeysController(IKeyVaultKeyService keyVaultKeyService)
        {
            _keyVaultKeyService = keyVaultKeyService;
        }

        [HttpPost]
        [Route("{id}/create")]
        [Produces("application/json")]
        [Consumes("application/json")]

        [ProducesResponseType(typeof(KeyResponse), StatusCodes.Status200OK)]
        public IActionResult CreateKey([FromRoute] string id, [FromQuery(Name = "api-version")] string apiVersion, [FromBody] CreateKeyModel requestBody)
        {
            var createdKey = _keyVaultKeyService.CreateKeyVaultKey(id, requestBody);

            return Ok(createdKey);
        }

        [HttpGet]
        [Route("{keyName}/{keyVersion}")]
        [Produces("application/json")]

        [ProducesResponseType(typeof(KeyResponse), StatusCodes.Status200OK)]
        public IActionResult GetKey([FromRoute] string keyName, [FromRoute] Guid keyVersion, [FromQuery(Name = "api-version")] string apiVersion)
        {
            return Ok(_keyVaultKeyService.GetKey(keyName, keyVersion));
        }

        [HttpPost]
        [Route("{keyName}/{keyVersion}/encrypt")]
        [Produces("application/json")]
        [Consumes("application/json")]
        public IActionResult Encrypt([FromRoute] string keyName, [FromRoute] Guid keyVersion, [FromQuery(Name = "api-version")] string apiVersion, [FromBody] KeyOperationParameters keyOperationParameters)
        {
            var result = _keyVaultKeyService.Encrypt(keyName, keyVersion, keyOperationParameters);
            return Ok(result);
        }

        [HttpPost]
        [Route("{keyName}/{keyVersion}/decrypt")]
        [Produces("application/json")]
        [Consumes("application/json")]
        public IActionResult Decrypt([FromRoute] string keyName, [FromRoute] Guid keyVersion, [FromQuery(Name = "api-version")] string apiVersion, [FromBody] KeyOperationParameters keyOperationParameters)
        {
            var result = _keyVaultKeyService.Decrypt(keyName, keyVersion, keyOperationParameters);
            return Ok(result);
        }
    }
}
