using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;


namespace Laba_2
{
    public class UdpServer
    {
        //Объявление и инициализация логгера
        private static Logger logger = LogManager.GetCurrentClassLogger();
        private const int Port = 2006;
        private static DataController DC = new DataController();
        static async Task Main(string[] args)
        {
            //Cоздание экземпляра класса UdpClient с параметром Port
            using UdpClient udpClient = new(Port);
            //cinemas = ReadData();

            //Запись в лог информации о запуске сервера и прослушивании порта
            logger.Info($"Server started. Listening on port {Port}");

            while (true)
            {
                //Получение данных от клиента
                var result = await udpClient.ReceiveAsync();
                //Преобразование байтового массива в строку
                string request = Encoding.UTF8.GetString(result.Buffer);
                //Запись в лог информации о полученном запросе от клиента
                logger.Info($"Received request from {result.RemoteEndPoint}: {request}");
                //Обработка запроса
                string response = ProcessRequest(request);
                //Преобразование строки в байтовый массив
                byte[] responseBytes = Encoding.UTF8.GetBytes(response);
                //Отправка ответа клиенту
                _ = udpClient.SendAsync(responseBytes, responseBytes.Length, result.RemoteEndPoint);
                //Запись в лог информации о отправленном ответе клиенту
                logger.Info($"Sent response to {result.RemoteEndPoint}: {response}");
            }   
        }

        private static string ProcessRequest(string request)
        {
            string[] parts = request.Split(',');
            string command = parts[0];
            switch (command)
            {
                case "1":
                    //Вывод всех записей
                    return GetAllFilms();
                case "2":
                    //Вывод записи по номеру
                    try
                    {
                        return GetFilm(int.Parse(parts[1]));
                    }
                    catch (Exception)
                    {
                        return "Недопустимый индекс.";
                    }
                case "3":
                    //Удаление записи
                    try
                    {
                        DeleteCinema(int.Parse(parts[1]));
                        return "Фильм удален.";
                    }
                    catch (Exception)
                    {

                        return "Недопустимый индекс.";
                    }

                case "4":
                    //Добавление записи
                    try
                    {
                        AddCinema(parts[1], parts[2], bool.Parse(parts[3]), int.Parse(parts[4]));
                        return "Фильм добавлен.";
                    }
                    catch (Exception)
                    {
                        return "Данные введены неверно!";
                    }
                default:
                    return "Недопустимая команда.";
            }
        }

        private static string GetAllFilms()
        {
            // Создание экземпляра класса
            StringBuilder sb = new StringBuilder();
            //Итерация по списку cinemas и добавление строк в экземпляр класса
            for (int i = 0; i < DC.GetCinemas().Count; i++)
            {
                string cinemaString = $"Запись {i + 1}: \nНазвание фильма: {DC.GetCinemas()[i].Film} \nДата и время показа: {DC.GetCinemas()[i].DateTime} \nНаличие свободных мест: {DC.GetCinemas()[i].Available_seats} \nКоличество свободных мест: {DC.GetCinemas()[i].Total_seats} \n";
                sb.AppendLine(cinemaString);
            }
            return sb.ToString();
        }

        private static string GetFilm(int index)
        {
            Cinema cinema = DC.GetCinemas()[index - 1];
            if (cinema != null)
            {
                return $"Запись {index}: \nНазвание фильма: {cinema.Film} \nДата и время показа: {cinema.DateTime} \nНаличие свободных мест: {cinema.Available_seats} \nКоличество свободных мест: {cinema.Total_seats} \n";
            }
            return "Недопустимый индекс.";


        }
        private static void DeleteCinema(int index)
        {
            Cinema cinema = DC.GetCinemas()[index - 1];
            if (cinema != null)
            {
                DC.DeleteFilm(cinema.ID);
            }
        }

        private static void AddCinema(string film, string datetime, bool available_seats, int total_seats)
        {
            DC.AddFilm(new Cinema { Film = film, DateTime = datetime, Available_seats = available_seats, Total_seats = total_seats });
        }


    }
}
