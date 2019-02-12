using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Docati.Api.Demo
{
    // Make sure you restore any missing NuGet-package before compilation.
    // NB: Docati.Api requires .NET Framework 4.6.1 or later!
    class Program
    {
        static void Main(string[] args)
        {
            /*
            // The license is included in this project is an embedded resource (build-action). Make sure you replace it with your own.
            // The license only needs to be applied once in the app-domain, eg in a static constructor or inside an owin-startup class.
            using (var licenseStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Docati.Api.Demo.License.lic"))
            {
                // If licenseStream is null here, License.lic could not be found in the assembly. Did you set its buildaction to 'Embedded Resource'?
                License.ApplyLicense(licenseStream);
            }
            */

            // In this sample, we will use the free license, instead of the embedded license file. If you want to evaluate Docati without
            // limitations, please don't hesitate to contact us at support@docati.com and request a trial license.
            License.ApplyLicense("free"); // Check https://www.docati.com/pricing for more licensing details

            // The EmbeddedResourceProvider is used, since it's able to load templates (and whatever resources they need) from resources
            // embedded in this assembly. It derives from the standard ResoureProvider which supports loading from disk, network-folders
            // and web/http addresses.
            var resourceProvider = new EmbeddedResourceProvider();

            // Set the desired output format
            var docFormat = DocumentFileFormat.PDF;
            var outputFilename = "TemplateResult." + (docFormat == DocumentFileFormat.PDF ? "pdf" : "docx");

            // A memorystream is defined to hold the final document
            using (var doc = new MemoryStream())
            {
                string password = null; // Specify a password to encrypt the final document (password is required to open the document)

                // Although this code generates a single document, the created DocBuilder can be reused to create multiple documents. It will cache all loaded templates,
                // so they do not need to be loaded for every call to Build.
                using (var builder = new DocBuilder("Template.docx", resourceProvider))
                using (var data = Assembly.GetExecutingAssembly().GetManifestResourceStream("Docati.Api.Demo.data.xml")) // Just like the license file, the data file is loaded from embedded resource as well
                {
                    var processingLog = new ProcessingLog();
                    builder.Build(data, DataFormat.Xml, doc, processingLog, docFormat, password);
                    Console.WriteLine($"{processingLog.Count} entries found, {processingLog.Count(l => l.ProcessingResult == ProcessingResultType.Warning)} warning(s), {processingLog.Count(l => l.ProcessingResult == ProcessingResultType.Error)} error(s).");
                    processingLog.ForEach(l => Console.WriteLine($"{l.ElementName} - {l.ElementType} ({l.ProcessingResult}): {l.QueryResult} {l.ProcessingDetails}"));
                }

                // Please note: For netcoreapp2.1/netstandard2.0-usage, PDF format will fail. This is a known limitation of the PDF-library used by Docati and may be resolved in the future.
                // Use .NET Framework 4.6.1 (net461) or later as target platform (Windows only) when you need PDF.

                // doc now contains the final document, so let's write it to disk
                File.WriteAllBytes(outputFilename, doc.ToArray()); // Check your bin/debug folder!
            }

            // Now load the generated document with the default program for the file extension
            // NB: This will fail if you don't have Word, Adobe Reader, etc installed or when running under .NET Core.
            // The wás created successfully however, so you can locate the file yourself in the bin/Debug folder and open it manually.
            Process.Start(outputFilename);
        }
    }
}
