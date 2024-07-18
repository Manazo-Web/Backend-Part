using Manazo.Models.Product;
using Manazo.Models.User;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;



namespace Manazo.Controllers
{
    [Route("api/db")]
    [ApiController]
    public class DbController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly CosmosClient _cosmosClient;
        private static Microsoft.Azure.Cosmos.Container? _container;

        public DbController(IConfiguration configuration, CosmosClient cosmosClient)
        {
            _configuration = configuration;
            _cosmosClient = cosmosClient;
        }

        private async Task<Microsoft.Azure.Cosmos.Container> GetDbContainer()
        {
            if (_container != null)
            {
                return _container;

            }
            else
            {
                String? endpoint = _configuration.GetSection("CosmosDb").GetSection("Endpoint").Value;
                String? key = _configuration.GetSection("CosmosDb").GetSection("Key").Value;
                String? databaseId = _configuration.GetSection("CosmosDb").GetSection("DatabaseId").Value;
                String? containerId = _configuration.GetSection("CosmosDb").GetSection("ContainerId").Value;

                CosmosClient cosmosClient = new(
                    endpoint, key,
                    new CosmosClientOptions()
                    {
                        ApplicationName = "Azure_PV111"
                    });

                Database database = await cosmosClient
                    .CreateDatabaseIfNotExistsAsync(databaseId);

                Microsoft.Azure.Cosmos.Container _container = await database
                    .CreateContainerIfNotExistsAsync(containerId, "/partitionKey");
                return _container;
            }
        }


        [HttpPost("AddUser")]
        public async Task<IActionResult> AddUser([FromBody] UserFormModel registrationData)
        {
            Microsoft.Azure.Cosmos.Container container = await GetDbContainer();

            // Добавление нового элемента в контейнер
            ItemResponse<UserFormModel> response = await container.CreateItemAsync(registrationData, new PartitionKey(registrationData.Id.ToString()));

            // Возвращаем успешный статус или можно вернуть созданный элемент
            return Ok();
        }
    

        [HttpPost("AddItem")]
        public async Task<IActionResult> AddItem([FromBody] ProductFormModel data)
        {
            Microsoft.Azure.Cosmos.Container container = await GetDbContainer();

            // Добавление нового элемента в контейнер
            ItemResponse<ProductFormModel> response = await container.CreateItemAsync(data, new PartitionKey(data.ProductId));

            // Возвращаем успешный статус или можно вернуть созданный элемент
            return Ok();
        }

        [HttpGet("GetUser")]
        public async Task<IActionResult> GetUser([FromQuery] string id)
        {
            try
            {
                Microsoft.Azure.Cosmos.Container container = await GetDbContainer();

                var user = await container.ReadItemAsync<UserFormModel>(id, new PartitionKey(id));
                return Ok(user.Resource);
            }
            catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return NotFound($"User with id {id} not found.");
            }
        }

        [HttpGet("GetItem")]
        public async Task<IActionResult> GetItem([FromQuery] string id)
        {
            try
            {
                Microsoft.Azure.Cosmos.Container container = await GetDbContainer();

                var product = await container.ReadItemAsync<ProductFormModel>(id, new PartitionKey(id));
                return Ok(product.Resource);
            }
            catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return NotFound($"Item with id {id} not found.");
            }
        }

        [HttpPost("UpdateUser")]
        public async Task<IActionResult> UpdateUser([FromQuery] string id, [FromBody] UserFormModel updatedUserData)
        {
            try
            {
                Microsoft.Azure.Cosmos.Container container = await GetDbContainer();

                var user = await container.ReadItemAsync<UserFormModel>(id, new PartitionKey(id));

                //  This we can use to update object's specific fields
                //  user.Resource.RegName = updatedUserData.RegName;
                //  user.Resource.RegSurname = updatedUserData.RegSurname;

                await container.ReplaceItemAsync(user.Resource, id, new PartitionKey(id));

                return Ok();
            }
            catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return NotFound($"User with id {id} not found.");
            }
        }

        [HttpPost("UpdateItem")]
        public async Task<IActionResult> UpdateItem([FromQuery] string id, [FromBody] ProductFormModel updatedItemData)
        {
            try
            {
                Microsoft.Azure.Cosmos.Container container = await GetDbContainer();

                var item = await container.ReadItemAsync<ProductFormModel>(id, new PartitionKey(id));

                //  This we can use to update object's specific fields
                //  item.Resource.ProductName = updatedItemData.ProductName;
                //  item.Resource.Description = updatedItemData.Description;

                await container.ReplaceItemAsync(item.Resource, id, new PartitionKey(id));

                return Ok();
            }
            catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return NotFound($"Item with id {id} not found.");
            }
        }

        [HttpPost("DeleteUser")]
        public async Task<IActionResult> DeleteUser([FromQuery] string id)
        {
            try
            {
                Microsoft.Azure.Cosmos.Container container = await GetDbContainer();
                var user = await container.ReadItemAsync<UserFormModel>(id, new PartitionKey(id));

                // Обозначаем пользователя как удаленного, но не физически удаляем из базы
                user.Resource.DeleteMe();

                // Замена объекта в контейнере
                await container.ReplaceItemAsync(user.Resource, id, new PartitionKey(id));

                return Ok();
            }
            catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return NotFound($"User with id {id} not found.");
            }
        }

        [HttpPost("DeleteItem")]
        public async Task<IActionResult> DeleteItem([FromQuery] string id)
        {
            try
            {
                Microsoft.Azure.Cosmos.Container container = await GetDbContainer();
                var item = await container.ReadItemAsync<ProductFormModel>(id, new PartitionKey(id));

                // Обозначаем товар как удаленный, но не физически удаляем из базы
                item.Resource.DeleteMe();

                // Замена объекта в контейнере
                await container.ReplaceItemAsync(item.Resource, id, new PartitionKey(id));

                return Ok();
            }
            catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return NotFound($"Item with id {id} not found.");
            }
        }
    }
}
