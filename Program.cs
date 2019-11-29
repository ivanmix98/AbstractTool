using System;
using System.Collections.Generic;
using System.IO;

namespace AbstractTool
{
    class Program
    {
        static void Main(string[] args)
        {

            String desktop = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            string path = desktop + "/AbstractTool";
            try
            {
                if (Directory.Exists(path))
                {
                    GestioFitxers.setPath(ref path);
                    File.Exists(path);

                    String contador = GestioFitxers.contarPalabrasFichero(path);
                    String moreRepeatedWords = GestioFitxers.CincParaulesRepetides(path);
                    GestioFitxers.crearFicheroSalida(desktop, path, contador, moreRepeatedWords);
                }

                else
                {
                    Directory.CreateDirectory(path);
                    Console.WriteLine("Directori creat, dipositi allà els fitxers que desitja processar i torna a executar el programa ");
                }



            }
            catch
            {
                Console.Write("No existeix el fitxer");
            }

        }
    }
}
