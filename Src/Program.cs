using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace Docati.Api.Demo
{
    class Program
    {
        static void Main()
        {
            /*
            // The license is included in this project is an embedded resource (build-action). Make sure you replace it with your own.
            // NB: We sometimes replace it with a temporary trial license, so you may want to see if it just works.
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

            // Set the desired output format (Word, PDF or XPS) -- XPS only works when targeting .NET Full framework (4.6.1 or later), not on .NET Core
            var docFormat = DocumentFileFormat.PDF;
            var outputFilename = "TemplateResult." + (docFormat == DocumentFileFormat.Word ? "docx" : docFormat.ToString());

            // A memorystream is defined to hold the final document
            using (var doc = new MemoryStream())
            {
                string password = null; // Specify a password to encrypt the final document (password is required to open the document)

                // Although this code generates a single document, the created DocBuilder can be reused to create multiple documents. It will cache all loaded templates,
                // so they do not need to be loaded for every call to Build.
                using (var builder = new DocBuilder("Template.docx", resourceProvider))
                using (var data = Assembly.GetExecutingAssembly().GetManifestResourceStream("Docati.Api.Demo.data.xml")) // Just like the license file, the data file is loaded from embedded resource as well
                    builder.Build(data, DataFormat.Xml, doc, null, docFormat, password);

                // doc now contains the final document, so let's write it to disk
                File.WriteAllBytes(outputFilename, doc.ToArray()); // Check your bin/debug folder!
            }

            // Now try to load the generated document with the default program for the file extension
            // This will fail if you don't have Word, Adobe Reader, etc installed.
            // The file created successfully however, so you can locate the file yourself in the bin/Debug folder and open it manually.
            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
                Process.Start(outputFilename);

            Console.WriteLine($"{outputFilename} was successfully generated.");
        }
    }
}