using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

namespace Docati.Api.Demo
{
    class Program
    {
        async static Task Main(string[] args)
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

            // Set the desired output format (Word, PDF or XPS) -- XPS only works when targeting .NET Full framework (4.6.1 or later), not on .NET Core
            var docFormat = DocumentFileFormat.PDF;
            var outputFilename = "TemplateResult." + (docFormat == DocumentFileFormat.Word ? "docx" : docFormat.ToString());
            var outputFolder = args.Length > 0 ? args[0] : Environment.CurrentDirectory;
            outputFilename = Path.Combine(outputFolder, outputFilename);

            // Just like the license file, the data file is loaded from embedded resource as well. This can of course be any stream
            using var data = Assembly.GetExecutingAssembly().GetManifestResourceStream("Docati.Api.Demo.data.json");

            // Although this code generates a single document, the created DocBuilder can be reused to create multiple documents. It will cache all loaded templates,
            // so they do not need to be loaded for every call to BuildAsync.
            using var builder = await DocBuilder.ForTemplateAsync("Template.docx");

            // Generate the document using the builder and passing the data for the dynamic fields (Docati placeholders)
            using var doc = await builder.BuildAsync(data, outputFormat: docFormat); // When using free license, this will take 2 sec. Request a trial license to remove this limitation.

            // doc now contains the final document, so let's write it to disk
            using (var outputStream = File.OpenWrite(outputFilename))
                await doc.CopyToAsync(outputStream); // Check your bin/debug folder!

            Console.WriteLine($"{outputFilename} was successfully generated.");

            // Now try to load the generated document with the default program for the file extension
            // This will fail if you don't have Word, Adobe Reader, etc installed.
            // The file created successfully however, so you can locate the file yourself in the bin/Debug folder and open it manually.
            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
                Process.Start(outputFilename);
        }
    }
}