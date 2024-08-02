using HahnCargoTruckLoader.Logic;

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Hello, Truck-Loader!");

        // Load the truck
        var truck = Initialize.LoadTruck();
        Console.WriteLine("Today's Truck is a " + truck.TruckType);

        // Get the crates
        var crates = Initialize.GetCrates();
        Console.WriteLine("You need to load " + crates.Count + " crates into it. Have FUN!");

        // Create the LoadingPlan
        var loadingPlan = new LoadingPlan(truck, crates);

        try
        {
            var loadingInstructions = loadingPlan.GetLoadingInstructions();
            Console.WriteLine("Checking Loading Plan...");

            // Simulate loading (make sure LoadingSimulator is implemented)
            LoadingSimulator sim = new LoadingSimulator(truck, crates);
            var result = sim.RunSimulation(loadingInstructions);

            Console.WriteLine(result ? "The plan does work!" : "The plan does NOT work!");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }

        Console.WriteLine("Hit any key to end the sim");
        Console.ReadKey();
    }
}
