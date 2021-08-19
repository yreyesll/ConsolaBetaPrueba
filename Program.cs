using System;
using ConsoleApp1Beta.Dto;
namespace ConsoleApp1Beta
{
    class Program
    {
        static void Main(string[] args)
        {

            Cliente c = new Cliente();
            var url = c.UrlVideos;   
            
            Console.WriteLine("esto es un cambio a las 19:40 pm");
            
            Console.ReadKey();
        }
    }
}
