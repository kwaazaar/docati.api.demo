using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace Docati.Api.Demo
{
    // Make sure you restore any missing NuGet-package before compilation.
    class Program
    {
        static void Main(string[] args)
        {
            // The license is included in this project is an embedded resource (build-action). Make sure you replace it with your own.
            // Contact us at support@docati.com and ask for a trial license, if you're just evaluating Docati.

            // The license only needs to be applied once in the app-domain, eg in a static constructor or inside an owin-startup class.
            License.ApplyLicense(Assembly.GetExecutingAssembly().GetManifestResourceStream("Docati.Api.Demo.License.lic"));

            // A custom resource provider is used, to be able to load templates (and whatever resources they need!) from embedded resources.
            // The standard ResoureProvider supports loading from disk and web/http addresses only, so it will not suffice
            var resourceProvider = new EmbeddedResourceProvider();

            // Set the desired output format
            var docFormat = DocumentFileFormat.SameAsTemplate;
            var outputFilename = "TemplateResult." + (docFormat == DocumentFileFormat.PDF ? "pdf" : "docx");

            // A memorystream is defined to hold the final document
            using (var doc = new MemoryStream())
            {
                // Although this code generates a single document, the created DocBuilder can be reused to create multiple documents. It will cache all loaded templates,
                // so they do not need to be loaded for every call to Build.
                using (var builder = new DocBuilder("Template.docx", resourceProvider))
                using (var data = Assembly.GetExecutingAssembly().GetManifestResourceStream("Docati.Api.Demo.data.xml")) // Just like the license file, the data file is loaded from embedded resource as well
                    builder.Build(data, DataFormat.Xml, doc, null, docFormat);

                // doc now contains the final document, so let's write it to disk (change the extension if you 
                File.WriteAllBytes(outputFilename, doc.ToArray()); // Check your bin/debug folder!
            }

            // Now load the generated document with the default program for the file extension
            // NB: This will fail if you don't have Word, Adobe Reader, etc installed!
            Process.Start(outputFilename);
        }
    }
}
