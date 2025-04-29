//using System.Net;
//using System.Net.Sockets;
//using System.Text;

//class UdpConnection
//{
//    // HILO POR CADA CLIENTE QUE SE CONECTA 

//    // puerto donde el servidor va a estar escuchando y a donde los clientes deben conectarse
//    private const int port = 11000;

//    // objeto que recibe y envia mensajes UDP
//    private static UdpClient listener = new UdpClient(port);

//    // lista con los jugadores que se han conectado con su IP y SU HILO para no tener que crear uno cada vez q se conctan
//    private static Dictionary<IPEndPoint, Thread> playersThreads = new Dictionary<IPEndPoint, Thread>();

//    public static void Main()
//    {
//        Console.WriteLine($"Servidor UDP escuchando en el puerto {port}...");

//        while (true)
//        {
//            try
//            {
//                // Recibir mensaje de cualquier cliente rechazando las que no sean el puerto nuestro (EN EL CLIENTE HABRA QUE CONECTARSE AL PUERTO 11000)
//                IPEndPoint client = new IPEndPoint(IPAddress.Any, 11000); 
//                // para cualquier cliente poner IPEndPoint(IPAddress.Any, 0) que admite de cualquier puerto y cliente

//                byte[] receivedBytes = listener.Receive(ref client);

//                // Verificar si el cliente ya tiene un hilo asignado
//                if (!playersThreads.ContainsKey(client))
//                {
//                    Console.WriteLine($"Nuevo cliente conectado: {client}");

//                    // Crear un hilo para manejar este cliente
//                    Thread clientThread = new Thread(() => HandleClient(client));
//                    playersThreads[client] = clientThread;
//                    clientThread.Start();
//                }

//            }
//            catch (SocketException ex)
//            {
//                Console.WriteLine($"Error de socket: {ex.Message}");
//            }
//            catch (Exception ex)
//            {
//                Console.WriteLine($"Error: {ex.Message}");
//            }
           
//        }

//    }

//    static void HandleClient(IPEndPoint clientEndPoint)
//    {
//        Console.WriteLine($"Manejando al cliente :) : {clientEndPoint}");

//        while (true)
//        {
//            try
//            {
//                // Recibe mensaje del cliente
//                byte[] receivedBytes = listener.Receive(ref clientEndPoint);
//                string message = Encoding.UTF8.GetString(receivedBytes);
//                Console.WriteLine($"Mensaje de {clientEndPoint}: {message}");

//                // Envia respuesta al mismo cliente
//                string responseMessage = $"Recibido grasiaas :) : {message}";
//                byte[] responseBytes = Encoding.UTF8.GetBytes(responseMessage);
//                listener.Send(responseBytes, responseBytes.Length, clientEndPoint);
//            }
//            catch (SocketException ex)
//            {
//                Console.WriteLine($"Error de socket con el cliente {clientEndPoint}: {ex.Message}");
//                break;
//            }
//            catch (Exception ex)
//            {
//                Console.WriteLine($"Error con el cliente {clientEndPoint}: {ex.Message}");
//                break;
//            }
//        }

//        // Si el cliente se desconecta, se elimina de la lista
//        playersThreads.Remove(clientEndPoint);
//        Console.WriteLine($"Cliente desconectado: {clientEndPoint}");
//    }
//}
