using System.Collections.Generic;
using System.Drawing;
using System.Windows;

namespace FreeFilter
{
    //Указатель на функцию обработки прогресса выполнения задачи 
    public delegate void ProgressDelegate(double percent);
    
    public interface IImageHandler
    {
        //получение осмысленного имени обработчика 
        string HandlerName { get; }

        //Инициализация параметров обработчика 
        void init(SortedList<string, object> parameters);

        //Установка изображения‐источника 
        Bitmap Source { set; }

        //Получение изображения‐результата 
        Bitmap Result { get; }

        //Запуск обработки 
        void startHandle(ProgressDelegate progress);  
    }
}
