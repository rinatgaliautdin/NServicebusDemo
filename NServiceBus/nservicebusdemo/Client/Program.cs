using System;
using System.Threading.Tasks;
using NServiceBus;
using Shared;

class Program
{
 
    static void Main()
    {
        AsyncMain().GetAwaiter().GetResult();
    }

    static async Task AsyncMain()
    {
        Console.Title = "NSBDemo.Client";

        var endpointConfiguration = new EndpointConfiguration(endpointName: "NSBDemo.Client");


        endpointConfiguration.SendFailedMessagesTo("error");

        endpointConfiguration.UseSerialization<JsonSerializer>();

        endpointConfiguration.EnableInstallers();

        endpointConfiguration.UsePersistence<InMemoryPersistence>();

        var endpointInstance = await Endpoint.Start(endpointConfiguration).ConfigureAwait(false);

        try
        {
            await SendOrder(endpointInstance);
        }
        finally
        {
            await endpointInstance.Stop().ConfigureAwait(false);
        }
    }
 

 
    static async Task SendOrder(IEndpointInstance endpointInstance)
    {
        Console.WriteLine("Press enter to send a message");
        Console.WriteLine("Press any key to exit");

        while (true)
        {
            var key = Console.ReadKey();
            Console.WriteLine();

            if (key.Key != ConsoleKey.Enter)
            {
                return;
            }
            var id = Guid.NewGuid();

            var placeOrder = new PlaceOrder
            {
                Product = "TShirt",
                Id = id
            };

            await endpointInstance.Send("NSBDemo.Server", placeOrder);
            Console.WriteLine($"Sent a PlaceOrder message with id: {id:N}");
        }
    }
 
}
