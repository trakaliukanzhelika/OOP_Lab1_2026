using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Google.Apis.Util.Store;

namespace Lab1
{
    public class GoogleDriveHelper
    {
        private const string clientId = Secrets.ClientId;
        private const string clientSecret = Secrets.ClientSecret;

        private static string[] Scopes = { DriveService.Scope.DriveFile };
        private const string ApplicationName = "MAUI Spreadsheet Lab";
        private const string FileName = "table.json"; 

        private DriveService _service;

        // User Authorization
        public async Task AuthenticateAsync()
        {
            if (_service != null) return;

            UserCredential credential;
            using (var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(
                $"{{\"installed\":{{\"client_id\":\"{clientId}\",\"client_secret\":\"{clientSecret}\"}}}}")))
            {
                // Token is stored locally
                string credPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "token.json");

                credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.FromStream(stream).Secrets,
                    Scopes,
                    "user",
                    CancellationToken.None,
                    new FileDataStore(credPath, true));
            }

            _service = new DriveService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName,
            });
        }

        // Saving file
        public async Task UploadFileAsync(string localFilePath)
        {
            await AuthenticateAsync();

            // Checking if file already exists
            var request = _service.Files.List();
            request.Q = $"name = '{FileName}' and trashed = false";
            var result = await request.ExecuteAsync();
            var existingFile = result.Files.FirstOrDefault();

            var fileMetadata = new Google.Apis.Drive.v3.Data.File()
            {
                Name = FileName
            };

            using (var stream = new FileStream(localFilePath, FileMode.Open))
            {
                if (existingFile != null)
                {
                    // Rewriting existing file
                    var updateRequest = _service.Files.Update(fileMetadata, existingFile.Id, stream, "application/json");
                    await updateRequest.UploadAsync();
                }
                else
                {
                    // Creating new file
                    var createRequest = _service.Files.Create(fileMetadata, stream, "application/json");
                    await createRequest.UploadAsync();
                }
            }
        }

        // Downloading file from Google Drive
        public async Task<bool> DownloadFileAsync(string localFilePath)
        {
            await AuthenticateAsync();

            var request = _service.Files.List();
            request.Q = $"name = '{FileName}' and trashed = false";
            var result = await request.ExecuteAsync();
            var file = result.Files.FirstOrDefault();

            if (file == null)
            {
                return false; 
            }

            var getRequest = _service.Files.Get(file.Id);
            using (var stream = new FileStream(localFilePath, FileMode.Create))
            {
                await getRequest.DownloadAsync(stream);
            }
            return true;
        }
    }
}
