using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MapGen
{
    class Startup
    {
        [STAThread()]
        static void Main()
        {
            // Создание приложения
            Application app = new Application();
            // Создание главного окна.
            Presenter.Presenter presenter = new Presenter.Presenter(new Model.Model(), new View.Source.Classes.View());
            // Запуск приложения и отображение главного окна,
            app.Run((Window)presenter.MainWindow);
        }
    }
}
