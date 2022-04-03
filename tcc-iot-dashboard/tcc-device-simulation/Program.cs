using System;

namespace tcc_device_simulation
{
    class Program
    {
        private static SimulationTask _simulationTask;
        private static SimulationOptions _simulationOptions = SimulationOptions.GetDefault();
        static void Main(string[] args)
        {
            Setup();

            string commandString = string.Empty;

            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("");
            Console.WriteLine("");
            Console.WriteLine("***********************************************************");
            Console.WriteLine("*       Simulação de dispositvos - Azure IoT Hub          *");
            Console.WriteLine("*                                                         *");
            Console.WriteLine("*            Digite um comando paras iniciar              *");
            Console.WriteLine("*                                                         *");
            Console.WriteLine("***********************************************************");
            Console.WriteLine("");

            while (!commandString.Equals("Exit"))
            {
                Console.ResetColor();
                Console.WriteLine("Digite um comando (setup | start | help | exit) >");
                commandString = Console.ReadLine();

                switch (commandString.ToUpper())
                {
                    case "SETUP":
                        Setup();
                        break;
                    case "START":
                        Start();
                        break;
                    case "HELP":
                        Help();
                        break;
                    case "EXIT":
                        Console.WriteLine("Tchau!");
                        return;
                    default:
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Comando Inválido.");
                        break;
                }
            }

            Console.WriteLine("\n\nPress any key to exit");
            Console.ReadKey();
        }

        private static void Setup()
        {
            
            Console.WriteLine("");
            Console.WriteLine("Insira a ConnectionString do IoTHub:");
            var iotHubConnectionString = Console.ReadLine();
            
            Console.WriteLine("");
            Console.WriteLine("Insira o tempo em minutos da simulação:");
            var minutesToRunString = Console.ReadLine();
            
            var iotHubConnectionService = new IoTHubConnectionService(iotHubConnectionString);
            _simulationTask = new SimulationTask(_simulationOptions, iotHubConnectionService, Int32.Parse(minutesToRunString));
        }

        private static void Help()
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("");
            Console.WriteLine("SETUP      - Setup the required information for the data generation.");
            Console.WriteLine("START      - Start the simulation of the devices updates.");
            Console.WriteLine("STATUS     - Print the current status of the configuration");
            Console.WriteLine("HELP       - Displays this page");
            Console.WriteLine("EXIT       - Closes this program");
            Console.WriteLine("");
            Console.ResetColor();
        }

        private static void Start()
        {
            if (_simulationTask == null)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Você deve primeiro configurar antes de iniciar a simulação.");
                Console.ResetColor();
                return;
            }

            Console.WriteLine($"A simulação irá rodar por  {_simulationOptions.SimulationTimeInSeconds} segundos.");
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            _simulationTask.Start();
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            ConsoleKeyInfo cki;
            do
            {
                Console.WriteLine("** Para interromper a simulação, pressiona a tecla `Escape`.");
                cki = Console.ReadKey();
            } while (cki.Key != ConsoleKey.Escape && _simulationTask.GetSimulation().Status != Constants.SimulationStatusFinished);
            _simulationTask.Stop();
        }
    }
}