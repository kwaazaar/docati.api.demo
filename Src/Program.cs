using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace Docati.Api.Demo
{
    // Make sure you restore any missing NuGet-package before compilation.
    // NB: Docati.Api requires .NET Framework 4.5.1 or later!
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

            // The standard ResourceProvider is used which reads the template from the current directory (eg: your bin/debug folder when you press F5).
            // The Template.docx has 'Copy to Output Directory' enabled to ensure the file is actually there.
            var resourceProvider = new ResourceProvider(new Uri(Environment.CurrentDirectory), false); // set allowExternalResources to true to enable loading templates outside the baseUri

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
                    builder.Build(data, DataFormat.Xml, doc, null, docFormat, password);

                // doc now contains the final document, so let's write it to disk
                File.WriteAllBytes(outputFilename, doc.ToArray()); // Check your bin/debug folder!
            }

            // Now load the generated document with the default program for the file extension
            // NB: This will fail if you don't have Word, Adobe Reader, etc installed!
            Process.Start(outputFilename);
        }
    }
}
