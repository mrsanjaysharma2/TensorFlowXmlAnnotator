using SSS.ImageAnnotater.Model;
using System;
using System.Collections.Generic;

namespace SSS.ImageAnnotater
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Annotator annotator = new Annotator();
                //parse the xml to object
                IDictionary<string, ImageMetadata> annotatedFiles = annotator.LoadFolderContent();
            }
            catch (Exception)
            {
                //Log the exceptions
                Console.WriteLine("Error Occured while annotating files.");
            }
           
            Console.ReadLine();
        }
    }
}
