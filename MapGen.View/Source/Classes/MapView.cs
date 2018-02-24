using System.Windows.Controls;

namespace MapGen.View.Source.Classes
{
    public class MapView
    {
        /// <summary>
        /// Id карты в базе данных.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Имя карты.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Ширина карты.
        /// </summary>
        public int Width { get; set; }

        /// <summary>
        /// Длина карты.
        /// </summary>
        public int Length { get; set; }

        /// <summary>
        /// Масштаб карты (1 : Scale).
        /// </summary>
        public int Scale { get; set; }


        public MapView(int id, string name, int width, int length, int scale)
        {
            Id = id;
            Name = name;
            Width = width;
            Length = length;
            Scale = scale;
        }

        /// <summary>
        /// Получить имя колонки на русском языке.
        /// </summary>
        /// <param name="field"></param>
        /// <returns></returns>
        public static string GetRussianNameField(DataGridAutoGeneratingColumnEventArgs field)
        {
            switch (field.PropertyName)
            {
                case "Id":
                {
                    return "ID";
                }
                case "Name":
                {
                    return "Имя карты";
                }
                case "Width":
                {
                    return "Ширина";
                }
                case "Length":
                {
                    return "Длина";
                }
                case "Scale":
                {
                    return "Масштаб";
                }
                default: return "";
            }
        }

    }
}
