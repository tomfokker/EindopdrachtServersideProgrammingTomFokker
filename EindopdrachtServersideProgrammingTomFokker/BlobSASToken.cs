using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EindopdrachtServersideProgrammingTomFokker
{
    class BlobSASToken
    {
        public static string GetBlobSasUri(CloudBlobContainer container, string blobName)
        {
            string sasBlobToken;

            // Get blob reference
            CloudBlockBlob blob = container.GetBlockBlobReference(blobName);

            // Create access policy
            SharedAccessBlobPolicy adHocSAS = new SharedAccessBlobPolicy();
            adHocSAS.SharedAccessExpiryTime = DateTime.UtcNow.AddHours(24);
            adHocSAS.Permissions = SharedAccessBlobPermissions.Read;

            // Create SAS token for blob
            sasBlobToken = blob.GetSharedAccessSignature(adHocSAS);

            // Return SAS token
            return sasBlobToken;
        }
    }
}
