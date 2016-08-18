using Docati.Api;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Docati.Api.Demo
{
    /// <summary>
    /// Resource provider that supports loading of embedded resources
    /// </summary>
    public class EmbeddedResourceProvider : ResourceProvider
    {
        private readonly string asmPath;

        public EmbeddedResourceProvider()
            : base(new Uri(@"emb://" + Assembly.GetExecutingAssembly().GetName().Name + "/", UriKind.Absolute), true) // BaseUri is set to 'emb://Docati.Api.Demo/'
        {
            asmPath = Assembly.GetExecutingAssembly().GetName().Name; // Cannot grab from BasePath, since the Uri-class changes it to lower case
        }

        public override ResourceStream GetResource(Uri resourceUri)
        {
            var mayBeEmbeddedResource = !resourceUri.IsAbsoluteUri // We cannot be sure when the resource name is relative
                || this.BaseUri.IsBaseOf(resourceUri);

            if (mayBeEmbeddedResource)
            {
                var relResourceUri = resourceUri.IsAbsoluteUri ? this.BaseUri.MakeRelativeUri(resourceUri) : resourceUri;

                string embeddedResourceName = asmPath
                    + (relResourceUri.OriginalString[0] == '/' ? string.Empty : ".")
                    + relResourceUri.OriginalString.Replace('/', '.');

                var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(embeddedResourceName);
                if (stream != null) // Yep, turn out it exists!
                    return base.GetResource(resourceUri, stream);

                // If it fails to load, we just continue. It may be a regular resource after all.
            }

            return base.GetResource(resourceUri);
        }
    }
}
