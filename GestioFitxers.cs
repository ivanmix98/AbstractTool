using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml;
using System.Linq;


namespace AbstractTool
{
    class GestioFitxers
    {
        /// <summary>
        /// Aquesta funció actualitza per referencia la ruta de la carpeta de forma que sigui la ruta del arxiu.txt 
        /// </summary>
        /// <param name="path">variable que conté la ruta de la carpeta</param>
        public static void setPath(ref string path)
        {
            Console.WriteLine("Escriu el nom del fitxer a analitzar:");
            String nomFile = Console.ReadLine();
            path = path + "/" + nomFile + ".txt";
        }

        /// <summary>
        /// Aquesta funció compte el total de paraules d'un fitxer i les retorna en una variable "contador"
        /// </summary>
        /// <param name="path">variable que conté la ruta del arxiu que volem comptar</param>
        /// <returns>retorna en forma de variable el total de paraules d'un fitxer</returns>
        public static string contarPalabrasFichero(string path)
        {
            StreamReader sr = new StreamReader(path);
            int counter = 0;
            string delim = " ,.;:¿?¡!";
            string[] paraules;
            string line;
            while (!sr.EndOfStream)
            {
                line = sr.ReadLine();
                paraules = line.Split(delim.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                counter += paraules.Length;
            }
            string contador = counter.ToString();
            return contador;
        }

        /// <summary>
        /// Aquesta funció crea el fitxer de sortida amb tota la info requerida del fitxer original
        /// </summary>
        /// <param name="desktop">variable de l'escriptori creada amb la biblioteca Environment</param>
        /// <param name="path">variable que conté la ruta del ficher que volem analitzar</param>
        /// <param name="contador">variable que conté el número de paraules del fitxer analitzat</param>
        /// <param name="moreRepeatedWords">variable que conté les 5 paraules més repetides del text analitzat</param>
        public static void crearFicheroSalida(string desktop, string path, string contador, string moreRepeatedWords)
        {
            String outputFile = desktop + "/AbstractTool/info.txt";

            Dictionary<string, string> info = new Dictionary<string, string>();
            FileInfo fi = new FileInfo(path);
            info.Add("Nom del fitxer: ", fi.Name);
            info.Add("Extensió: ", fi.Extension);
            info.Add("Data: ", fi.CreationTime.ToString());
            info.Add("Número de paraules: ", contador);
            info.Add("Temàtica: ", moreRepeatedWords);

            foreach (KeyValuePair<string, string> kvp in info)
            {
                File.AppendAllText(outputFile, kvp.Key + " " + kvp.Value + "\r\n");
            }
        }

        /// <summary>
        /// Aquesta funció compte cada paraula i emmagatzema en una variable les 5 més repetides
        /// </summary>
        /// <param name="path">conté la ruta del arxiu a analitzar</param>
        /// <returns>retorna una variable que conté les 5 paraules més repetides</returns>
        public static string CincParaulesRepetides(string path)
        {
            Dictionary<string, int> dictionary = new Dictionary<string, int>();
            string delimitadors = " ,.;:¿?¡!'";
            string[] paraules;
            string lines;
            StreamReader sr = new StreamReader(path);
            string invalidWordsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "AbstractTool", "InvalidWords.txt");
            string excludingWords = File.ReadAllText(invalidWordsPath);

            while (!sr.EndOfStream)
            {
                lines = sr.ReadLine();
                paraules = lines.Split(delimitadors.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

                for (int i = 0; i < paraules.Length; i++)
                {
                    paraules[i] = paraules[i].ToLower();

                    if (dictionary.ContainsKey(paraules[i]))
                    {
                        dictionary[paraules[i]]++;
                    }
                    else
                    {

                        if (!excludingWords.Contains(paraules[i]))
                        {
                            dictionary[paraules[i]] = 1;
                        }
                    }
                }
            }

            var MaxValuesDictionary = (from entry in dictionary select entry)
               .ToDictionary(pair => pair.Key, pair => pair.Value).Take(5);

            string moreRepeatedWords = "";
            int counterRepeatedWords = 1;
            foreach (KeyValuePair<string, int> max in MaxValuesDictionary)
            {
                if (counterRepeatedWords == 5)
                {
                    moreRepeatedWords += max.Key + ".";
                }
                else
                {
                    moreRepeatedWords += max.Key + ", ";
                }
                counterRepeatedWords++;
            }

            CrearXML(dictionary);

            return moreRepeatedWords;
        }

        /// <summary>
        /// Aquesta funció crea un XML amb les paraules repetides
        /// </summary>
        /// <param name="dictionary">variable que conté les claus i valors de les paraules repetides</param>
        public static void CrearXML(Dictionary<string, int> dictionary)
        {
            XmlDocument xml = new XmlDocument();
            XmlNode rootNode = xml.CreateElement("words");
            xml.AppendChild(rootNode);

            foreach (KeyValuePair<string, int> entry in dictionary)
            {
                XmlNode userNode = xml.CreateElement(entry.Key);
                XmlAttribute attribute = xml.CreateAttribute("occurrences");
                attribute.Value = entry.Value.ToString();
                userNode.Attributes.Append(attribute);
                userNode.InnerText = entry.Key;
                rootNode.AppendChild(userNode);
            }

            string xmlName = "paraules_repetides.xml";
            string xmlPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "AbstractTool", xmlName);

            xml.Save(xmlPath);
        }
    }
}
