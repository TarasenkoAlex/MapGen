using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using MapGen.Model.Database.EDM;
using MapGen.Model.General;

namespace MapGen.Model.Database.DbWorker
{
    public class DatabaseWorker
    {
        /// <summary>
        /// Объект для работы с репозитриями базы данных и подключением.
        /// </summary>
        private UnitOfWork.UnitOfWork _unitOfWork;

        /// <summary>
        /// Очередь запросов на вставку.
        /// </summary>
        private readonly ConcurrentQueue<Tuple<TypeQueue, object>> _queueInserts;

        /// <summary>
        /// есть ли подключение к базе данных.
        /// </summary>
        public bool IsConnected { get; set; }

        /// <summary>
        /// Имя файла базы данных.
        /// </summary>
        public string DbName;

        /// <summary>
        /// Количество элементов в очереди запросов.
        /// </summary>
        public int CountQueueInserts => _queueInserts.Count;

        /// <summary>
        /// Существует ли база данных.
        /// </summary>
        public bool IsExistDatabase => File.Exists(DbName);

        /// <summary>
        /// Локер для потокобезопасной работы.
        /// </summary>
        public object Locker;

        /// <summary>
        /// Создает клиента базы данных.
        /// </summary>
        public DatabaseWorker()
        {
            // Инициализация имени базы данных.
            DbName = $"{ResourceModel.DATABASE_DIR_PATH}\\{ResourceModel.DATABASE_FILE_NAME}";

            // Инициализация очереди запросов на вставку.
            _queueInserts = new ConcurrentQueue<Tuple<TypeQueue, object>>();

            // Запуск обработки очереди запросов на вставку.
            new Thread(ProcessingQueueInserts) {IsBackground = true}.Start();

            // Инициализация локера. 
            Locker = new object();
        }

        /// <summary>
        ///  Процесс обработки очериди запросов на вставку.
        /// </summary>
        private void ProcessingQueueInserts()
        {
            while (true)
            {
                if (_queueInserts.Count != 0)
                {
                    try
                    {
                        // Извлекаем элемент из очереди.
                        Tuple<TypeQueue, object> elementQueue;
                        _queueInserts.TryDequeue(out elementQueue);

                        // Смотрим что за операция.
                        switch (elementQueue.Item1)
                        {
                            // Смотрим что за элемент и производим операцию.
                            case TypeQueue.Insert:
                            {
                                if (elementQueue.Item2 is Map)
                                {
                                    // Добавляем в таблицу Maps.
                                    Map map = (Map) elementQueue.Item2;
                                    _unitOfWork.Maps.Create(map);
                                }
                                else if (elementQueue.Item2 is Point)
                                {
                                    // Добавляем в таблицу Maps.
                                    Point point = (Point) elementQueue.Item2;
                                    _unitOfWork.Points.Create(point);
                                }

                                break;
                            }
                            case TypeQueue.Save:
                            {
                                _unitOfWork.Save();
                                break;
                            }
                        }
                    }
                    catch
                    {
                        ProcessingQueueInserts();
                    }
                }

                Thread.Sleep(5);
            }
        }

        /// <summary>
        /// Создание базы данных.
        /// </summary>
        /// <param name="message">Сообщение с ошибкой.</param>
        /// <returns>Произошло ли успешное создание.</returns>
        public bool CreateDatabase(out string message)
        {
            lock (Locker)
            {
                message = string.Empty;
                if (IsExistDatabase)
                {
                    message = "База данных уже существует.";
                    return false;
                }

                try
                {
                    if (!Directory.Exists($"{ResourceModel.DATABASE_DIR_PATH}"))
                    {
                        Directory.CreateDirectory($"{ResourceModel.DATABASE_DIR_PATH}");
                    }

                    File.WriteAllBytes($"{ResourceModel.DATABASE_DIR_PATH}\\{ResourceModel.DATABASE_FILE_NAME}",
                        ResourceModel.MapGen);
                }
                catch (Exception ex)
                {
                    message = $"Ошибка во время создания файла базы данных. {Methods.CalcMessageException(ex)}";
                    return false;
                }

                return true;
            }
        }

        /// <summary>
        /// Подключение к базе данных.
        /// </summary>
        /// <param name="message">Сообщение с ошибкой.</param>
        /// <returns>Произошло ли успешное подключение.</returns>
        public bool Connect(out string message)
        {
            lock (Locker)
            {
                message = string.Empty;
                if (IsConnected)
                {
                    message = "Уже существует подключение к базе данных.";
                    return true;
                }

                try
                {
                    _unitOfWork = new UnitOfWork.UnitOfWork();
                }
                catch (Exception ex)
                {
                    IsConnected = false;
                    message = $"Ошибка во время подключения к базе данных. {Methods.CalcMessageException(ex)}";
                    return false;
                }

                IsConnected = true;
                return true;
            }
        }

        /// <summary>
        /// Отключение от базы данных.
        /// </summary>
        /// <param name="message">Сообщение с ошибкой.</param>
        /// <returns>Произошло лиуспешное отключение.</returns>
        public bool Disconnect(out string message)
        {
            lock (Locker)
            {
                message = string.Empty;
                if (!IsConnected)
                {
                    message = "Подключение к базе данных отсутствует.";
                    return true;
                }

                try
                {
                    _unitOfWork = null;
                }
                catch (Exception ex)
                {
                    IsConnected = true;
                    message = $"Ошибка во время отлючения от базы данных. {Methods.CalcMessageException(ex)}";
                    return false;
                }

                IsConnected = false;
                return true;
            }
        }

        /// <summary>
        /// Сохранение данных в базе данных на жестком диске.
        /// </summary>
        /// <param name="message">Сообщение с ошибкой.</param>
        /// <returns>Произошло ли успешное сохранение.</returns>
        public bool SaveAsync(out string message)
        {
            message = string.Empty;
            if (!IsConnected)
            {
                message = "Подключение к базе данных отсутствует.";
                return false;
            }

            message = string.Empty;
            try
            {
                _queueInserts.Enqueue(new Tuple<TypeQueue, object>(TypeQueue.Save, new object()));
            }
            catch (Exception ex)
            {
                message = Methods.CalcMessageException(ex);
                return false;
            }

            return true;
        }

        /// <summary>
        /// Вставка записи в таблицу Maps.
        /// </summary>
        /// <param name="map">Добавляемый элемент.</param>
        /// <param name="message">Сообщение с ошибкой.</param>
        /// <returns>Произошла ли успешная вставка.</returns>
        public bool InsertMapAsync(Map map, out string message)
        {
            message = string.Empty;
            if (!IsConnected)
            {
                message = "Подключение к базе данных отсутствует.";
                return false;
            }

            try
            {
                _queueInserts.Enqueue(new Tuple<TypeQueue, object>(TypeQueue.Insert, map));
            }
            catch (Exception ex)
            {
                message = $"Ошибка во время записи данных в таблицу Maps. {Methods.CalcMessageException(ex)}";
                return false;
            }

            return true;
        }

        /// <summary>
        /// Вставка записи в таблицу Maps.
        /// </summary>
        /// <param name="map">Добавляемый элемент.</param>
        /// <param name="message">Сообщение с ошибкой.</param>
        /// <returns>Произошла ли успешная вставка.</returns>
        public bool InsertMap(Map map, out string message)
        {
            message = string.Empty;
            if (!IsConnected)
            {
                message = "Подключение к базе данных отсутствует.";
                return false;
            }

            try
            {
                _unitOfWork.Maps.Create(map);
                _unitOfWork.Save();
            }
            catch (Exception ex)
            {
                message = $"Ошибка во время записи данных в таблицу Maps. {Methods.CalcMessageException(ex)}";
                return false;
            }



            return true;
        }

        /// <summary>
        /// Вставка записи в таблицу Maps.
        /// </summary>
        /// <param name="point">Добавляемый элемент.</param>
        /// <param name="message">Сообщение с ошибкой.</param>
        /// <returns>Произошла ли успешная вставка.</returns>
        public bool InsertPointAsync(Point point, out string message)
        {
            message = string.Empty;
            if (!IsConnected)
            {
                message = "Подключение к базе данных отсутствует.";
                return false;
            }

            try
            {
                _queueInserts.Enqueue(new Tuple<TypeQueue, object>(TypeQueue.Insert, point));
            }
            catch (Exception ex)
            {
                message = $"Ошибка во время записи данных в таблицу Points. {Methods.CalcMessageException(ex)}";
                return false;
            }

            return true;
        }

        /// <summary>
        /// Вставка записи в таблицу Maps.
        /// </summary>
        /// <param name="points">Добавляемый элемент.</param>
        /// <param name="message">Сообщение с ошибкой.</param>
        /// <returns>Произошла ли успешная вставка.</returns>
        public bool InsertPoint(List<Point> points, out string message)
        {
            message = string.Empty;
            if (!IsConnected)
            {
                message = "Подключение к базе данных отсутствует.";
                return false;
            }

            try
            {
                _unitOfWork.Points.Create(points);
                _unitOfWork.Save();
            }
            catch (Exception ex)
            {
                message = $"Ошибка во время записи данных в таблицу Points. {Methods.CalcMessageException(ex)}";
                return false;
            }

            return true;
        }

        /// <summary>
        /// Чтение таблицы Maps.
        /// </summary>
        /// <param name="maps">Элементы таблицы Maps.</param>
        /// <param name="message">Сообщение с ошибкой.</param>
        /// <returns>Получилось ли получить данные.</returns>
        public bool GetAllMaps(out List<Map> maps, out string message)
        {
            maps = new List<Map>();
            message = string.Empty;
            if (!IsConnected)
            {
                message = "Подключение к базе данных отсутствует.";
                return false;
            }

            try
            {
                var getallmaps = _unitOfWork.Maps.GetAll();
                foreach (Map map in getallmaps)
                {
                    maps.Add(map);
                }
            }
            catch (Exception ex)
            {
                message = $"Ошибка во время чтения базы данных. {Methods.CalcMessageException(ex)}";
                return false;
            }

            return true;
        }

        /// <summary>
        /// Чтение карты по его id.
        /// </summary>
        /// <param name="idm">Id карты.</param>
        /// <param name="map">Элемент Map.</param>
        /// <param name="message">Сообщение с ошибкой.</param>
        /// <returns>Получилось ли получить данные.</returns>
        public bool GetMap(int idm, out Map map, out string message)
        {
            map = new Map();
            message = string.Empty;
            if (!IsConnected)
            {
                message = "Подключение к базе данных отсутствует.";
                return false;
            }

            try
            {
                map = _unitOfWork.Maps.Get(idm);
                if (map == null)
                {
                    message = $"Не удалось найти элемент в базе данных.";
                    return false;
                }
            }
            catch (Exception ex)
            {
                message = $"Ошибка во время чтения базы данных. {Methods.CalcMessageException(ex)}";
                return false;
            }

            return true;
        }

        /// <summary>
        /// Чтение таблицы Points.
        /// </summary>
        /// <param name="idm">Id карты.</param>
        /// <param name="points">Точки карты.</param>
        /// <param name="message">Сообщение с ошибкой.</param>
        /// <returns>Получилось ли получить данные.</returns>
        public bool GetPoints(int idm, out Point[] points, out string message)
        {
            points = new Point[] { };
            message = string.Empty;
            if (!IsConnected)
            {
                message = "Подключение к базе данных отсутствует.";
                return false;
            }

            try
            {
                points = _unitOfWork.Points.Get(idm);
            }
            catch (Exception ex)
            {
                message = $"Ошибка во время чтения базы данных. {Methods.CalcMessageException(ex)}";
                return false;
            }

            return true;
        }
    }

    public enum TypeQueue
    {
        Insert,
        Save
    }
}
