using Docati.Api;
using System;
using System.IO;
using System.Reflection;

namespace Docati.Api.Demo
{
    // If this does not compile, you may need to remove and add the reference to Docati.Api.
    class Program
    {
        static void Main(string[] args)
        {
            // The license is included in this project as an embedded resource (build-action). Make sure you replace it with your own.
            // It only needs to be applied once in the app-domain, eg in a static constructor or inside an owin-startup class.
            //License.ApplyLicense(Assembly.GetExecutingAssembly().GetManifestResourceStream("Docati.Api.Demo.License.lic"));

            // A custom resource provider is used, to be able to load templates (and whatever they need) from embedded resources.
            // The standard ResoureProvider supports loading from disk and web/http addresses only.
            var resourceProvider = new EmbeddedResourceProvider();

            // A memorystream is defined to hold the final document
            MemoryStream doc = new MemoryStream();

            // Invoke the generator (the using-statements are required to make sure all internal streams etc are disposed.
            // Although this code generates a single document, the created DocBuilder can be reused to create multiple documents.
            using (var builder = new DocBuilder("emb://Template.docx", resourceProvider))
            {
                using (Stream data = Assembly.GetExecutingAssembly().GetManifestResourceStream("Docati.Api.Demo.data.xml")) // Just like the license file, the data file is loaded from embedded resource as well
                {
                    builder.Build(data, DataFormat.Xml, (Stream)doc, null, DocumentFileFormat.SameAsTemplate);
                }
            }

            // doc now contains the final document, so let's write it to disk
            File.WriteAllBytes("MyTemplate-Result.docx", doc.ToArray());
        }
    }
}
