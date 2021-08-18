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
            
            Console.WriteLine("esto es un cambio a las 11:55 am");
            Console.Write("esto es un cambio a las 11:59 am");
            Console.Write("esto es un cambio a las 12:20pm");
            Console.ReadKey();
        }
    }
}
