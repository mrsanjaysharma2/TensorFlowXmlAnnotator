using SSS.ImageAnnotater.Model;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;

namespace SSS.ImageAnnotater
{
    public class Annotator
    {
        private static readonly double resizeValue = double.Parse(ConfigurationManager.AppSettings["resizingValue"]) / 100;
        private readonly string annotationLocation = ConfigurationManager.AppSettings["annotationLocation"];
        private readonly string imageLocation = ConfigurationManager.AppSettings["imageLocation"];
        private readonly string backupPath = ConfigurationManager.AppSettings["annotationsBackup"];
        private readonly long allowedSize = long.Parse(ConfigurationManager.AppSettings["allowedSizeInKb"]);
        private readonly bool isBackupRequired = bool.Parse(ConfigurationManager.AppSettings["backupContent"]);
        private readonly bool isRenamingRequired = bool.Parse(ConfigurationManager.AppSettings["renameFiles"]);
        private readonly bool filesAtSameLocation;

        public Annotator()
        {
            filesAtSameLocation = annotationLocation.Equals(imageLocation, StringComparison.InvariantCultureIgnoreCase);
        }

        public void Annotate(KeyValuePair<string, ImageMetadata> keyValuePair, bool fileSizeViolation=false)
        {
            string filePath = keyValuePair.Key;
            ImageMetadata annotation = keyValuePair.Value;
            if (fileSizeViolation)
            {
                annotation.Size.Height = Math.Round(annotation.Size.Height * resizeValue);
                annotation.Size.Width = Math.Round(annotation.Size.Width * resizeValue);
                annotation.DetectionObject.BoundedBox.XMin = Math.Round(annotation.DetectionObject.BoundedBox.XMin * resizeValue);
                annotation.DetectionObject.BoundedBox.YMin = Math.Round(annotation.DetectionObject.BoundedBox.YMin * resizeValue);
                annotation.DetectionObject.BoundedBox.XMax = Math.Round(annotation.DetectionObject.BoundedBox.XMax * resizeValue);
                annotation.DetectionObject.BoundedBox.YMax = Math.Round(annotation.DetectionObject.BoundedBox.YMax * resizeValue);
            }
            XmlSerializer serializer = new XmlSerializer(typeof(ImageMetadata), new XmlRootAttribute("annotation"));
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            
            using (XmlWriter writer = XmlWriter.Create(filePath, settings))
            {
                serializer.Serialize(writer, annotation);
            }
            Console.WriteLine($"Finished annotating {annotation.Filename}.");
        }

        public IDictionary<string, ImageMetadata> LoadFolderContent()
        {
            IDictionary<string,ImageMetadata> annotations = new Dictionary<string, ImageMetadata>();
            List<string> fileNamesWithSpaces = new List<string>();
            List<string> filesExceedingSizeLimit = new List<string>();
            bool fileNameViolation = false;
            bool fileSizeViolation = false;

            if (string.IsNullOrEmpty(annotationLocation))
                throw new ArgumentNullException(nameof(annotationLocation));

            if (string.IsNullOrEmpty(imageLocation))
                throw new ArgumentNullException(nameof(imageLocation));


            if (isRenamingRequired)
            {
                if (Directory.Exists(imageLocation))
                {
                    DirectoryInfo directoryInfo = new DirectoryInfo(imageLocation);
                    fileNamesWithSpaces.AddRange(directoryInfo.GetFiles().Where(x => x.Name.Contains(" ")).Select(x => x.FullName));
                }

                if (!filesAtSameLocation)
                {
                    DirectoryInfo directoryInfo = new DirectoryInfo(annotationLocation);
                    fileNamesWithSpaces.AddRange(directoryInfo.GetFiles("*.xml").Where(x => x.Name.Contains(' ')).Select(x => x.FullName));
                }
                if (fileNamesWithSpaces.Count > 0)
                   fileNamesWithSpaces= SanitizeFileNames(fileNamesWithSpaces);
            }

            string[] xmlFileNames = Directory.GetFiles(annotationLocation, "*.xml");

            foreach (var filename in xmlFileNames)
            {
                FileInfo fi = new FileInfo(filename);
                Console.WriteLine($"{fi.Name}- {fi.Length/1000}KB");

                if (fileNamesWithSpaces.Any(x => x.Equals(filename, StringComparison.InvariantCultureIgnoreCase)))
                    fileNameViolation = true;

                if (fi.Length > (allowedSize * 1000))
                    fileSizeViolation = true;

                if (!fileNameViolation && !fileSizeViolation)
                    continue;

                if (isBackupRequired)
                {
                    BackupFile(fi);
                }
                KeyValuePair<string, ImageMetadata> imageMetadata = default(KeyValuePair<string, ImageMetadata>);
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(ImageMetadata), new XmlRootAttribute("annotation"));
                using (XmlTextReader xmlReader = new XmlTextReader(fi.FullName))
                {
                    try
                    {
                        var metadata = (ImageMetadata)xmlSerializer.Deserialize(xmlReader);
                        if (metadata != null)
                        {
                            if (!string.Equals(fi.Name, metadata.Filename))
                            {
                                string nameWithoutExt = fi.Name.Split('.')[0];
                                metadata.Path = metadata.Path.Replace(metadata.Filename, nameWithoutExt);
                                metadata.Filename = nameWithoutExt;
                            }
                            imageMetadata = new KeyValuePair<string, ImageMetadata>(fi.FullName, metadata);
                            annotations.Add(imageMetadata);
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                        throw e;
                    }
                }

                if (imageMetadata.Value != null)
                    Annotate(imageMetadata, fileSizeViolation);

                fileNameViolation = false;
                fileSizeViolation = false;
            }

            return annotations;
        }

        private void BackupFile(FileInfo fileInfo)
        {
            if (!Directory.Exists(backupPath))
            {
                Directory.CreateDirectory(backupPath);
            }
            string destFile = Path.Combine(backupPath, fileInfo.Name);
            File.Copy(fileInfo.FullName, destFile, true);
            Console.WriteLine($"{fileInfo.Name} saved to backup location.");
        }

        private List<string> SanitizeFileNames(List<string> fileNames)
        {
            Console.WriteLine($"############# Started Renaming ##############");
            List<string> updatedFilePaths = new List<string>();
            foreach (var fileName in fileNames)
            {
                FileInfo fileInfo = new FileInfo(fileName);
                var renamedPath = Path.Combine(fileInfo.DirectoryName, fileInfo.Name.Replace(' ', '_'));
                File.Move(fileName, renamedPath);
                updatedFilePaths.Add(renamedPath);
                Console.WriteLine($"Renamed - {fileInfo.Name}.");
            }
            Console.WriteLine($"############# Finished Renaming ##############");
            return updatedFilePaths;
        }
    }
}
