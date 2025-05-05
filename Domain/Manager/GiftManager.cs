using Services.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Services.External_Services;
using Serilog;

namespace Domain.Manager
{
    internal class GiftManager
    {
        private readonly GiftStoreServices _externalService;
        private readonly ILogger<GiftManager> _logger;
        private static readonly Random _rnd = new();

        public GiftManager(
            ElectronicStoreService externalService,
            ILogger<GiftManager> logger)
        {
            _externalService = externalService;
            _logger = logger;
        }

        public async Task<GiftDto?> GetRandomGiftAsync(string ci)
        {
            _logger.LogInformation("Obteniendo lista de regalos para CI={CI}", ci);

            List<Electronic> items;
            try
            {
                items = await _externalService.GetAllElectronicItems();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al llamar a la API externa de regalos");
                return null;
            }

            if (items == null || !items.Any())
            {
                _logger.LogWarning("La API devolvió 0 regalos");
                return null;
            }

            // Selecciona uno al azar
            var picked = items[_rnd.Next(items.Count)];
            _logger.LogInformation("Regalo seleccionado ID={Id}, Name={Name}", picked.Id, picked.Name);

            // Mapea a tu DTO
            return new GiftDto
            {
                Id = picked.Id,
                Name = picked.Name,
                Data = new GiftDataDto
                {
                    Color = picked.Data?.Color,
                    Capacity = picked.Data?.Capacity
                }
            };
        }
    }
}
