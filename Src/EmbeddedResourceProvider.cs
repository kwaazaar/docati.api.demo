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
    public class EmbeddedResourceProvider : ResourceProvider
    {
        private readonly string asmPath;

        public EmbeddedResourceProvider()
            : base(new Uri(@"emb://", UriKind.Absolute), true) // BaseUri is set to 'emb://Docati.Api.Demo/'
        {
            asmPath = Assembly.GetExecutingAssembly().GetName().Name;
        }

        public override ResourceStream GetResource(Uri resourceUri)
        {
            var isEmbedded = !resourceUri.IsAbsoluteUri
                || resourceUri.AbsoluteUri.StartsWith(this.BaseUri.AbsoluteUri, StringComparison.OrdinalIgnoreCase);
            if (isEmbedded)
            {
                string embeddedResourceName = !resourceUri.IsAbsoluteUri
                    ? resourceUri.OriginalString
                    : resourceUri.PathAndQuery;

                string fullEmbeddedResourceName = asmPath + "." + embeddedResourceName;
                var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(fullEmbeddedResourceName);
                if (stream == null)
                    throw new FileNotFoundException("The embedded resource does not exist", fullEmbeddedResourceName);

                return base.GetResource(resourceUri, stream);
            }
            else
                return base.GetResource(resourceUri);
        }
    }
}
