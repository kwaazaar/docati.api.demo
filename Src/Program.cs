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
            // The license is included in this project is an embedded resource (build-action). Make sure you replace it with your own.
            // Contact us at support@docati.com and ask for a trial license, if you're just evaluating Docati.

            // The license only needs to be applied once in the app-domain, eg in a static constructor or inside an owin-startup class.
            using (var licenseStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Docati.Api.Demo.License.lic"))
            {
                // If licenseStream is null here, License.lic could not be found in the assembly. Did you set its buildaction to 'Embedded Resource'?
                License.ApplyLicense(licenseStream);
            }

            // The base-uri provided as the first argument to the resourceprovider contains arguments. When retrieving resources, like the template itself
            // or any of its required resources (like images), the constructed full uri to the resource will have this same querystring.
            // In this example, the BaseUri points to a secured Azure storage account (blob container) and the querystring is used to provide the
            // required credentials to authenticate (Shared Access Signature (SAS)-token).
            var resourceProvider = new ResourceProvider(
                new Uri("https://docati1.blob.core.windows.net/private/?sv=2016-05-31&ss=b&srt=co&sp=r&se=2018-06-09T23:59:19Z&st=2017-04-02T15:59:19Z&spr=https&sig=HQiAyvXhg7fst8WXTmRDY2ytj00dqP%2B0Xolhr24Fw9A%3D"),
                false);

            // Set the desired output format
            var docFormat = DocumentFileFormat.SameAsTemplate;
            var outputFilename = "TemplateResult." + (docFormat == DocumentFileFormat.PDF ? "pdf" : "docx");

            // A memorystream is defined to hold the final document
            using (var doc = new MemoryStream())
            {
                string password = null; // Specify a password to encrypt the final document (password is required to open the document)

                // Although this code generates a single document, the created DocBuilder can be reused to create multiple documents. It will cache all loaded templates,
                // so they do not need to be loaded for every call to Build.
                using (var builder = new DocBuilder("ImageOfTest.docx", resourceProvider))
                using (var data = Assembly.GetExecutingAssembly().GetManifestResourceStream("Docati.Api.Demo.ImageOfTest.xml")) // Just like the license file, the data file is loaded from embedded resource as well
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
