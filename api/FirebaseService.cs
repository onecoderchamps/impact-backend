using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using FirebaseAdmin;
using FirebaseAdmin.Messaging;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.SecretManager.V1;

public class FirebaseService
{
    private static bool _isInitialized = false;

    public static void InitializeFirebase()
    {
        if (!_isInitialized)
        {
            // Ambil credential dari Secret Manager
            var credentialJson = GetSecret("firebase-service-account");
            
            // Inisialisasi Firebase dengan credential dari Secret Manager
            FirebaseApp.Create(new AppOptions()
            {
                Credential = GoogleCredential.FromJson(credentialJson)
            });

            _isInitialized = true;
        }
    }

    public static async Task<string> SendPushNotification(string deviceToken, string title, string body, string idOrder)
    {
        InitializeFirebase(); // Pastikan Firebase sudah diinisialisasi

        var message = new Message()
        {
            Token = deviceToken,
            Notification = new Notification()
            {
                Title = title,
                Body = body
            },
            Data = new Dictionary<string, string>
            {
                { "forceOpen", "true" }, // Bisa digunakan untuk membuka aplikasi otomatis
                { "idOrder", idOrder }, // Bisa digunakan untuk membuka aplikasi otomatis
                { "chat", "true" } // Bisa digunakan untuk membuka aplikasi otomatis
            }
        };

        string response = await FirebaseMessaging.DefaultInstance.SendAsync(message);
        return response; // Response dari Firebase
    }

    public static async Task<string> SendPushNotification2(string image,string deviceToken, string title, string body, string idOrder)
    {
        InitializeFirebase(); // Pastikan Firebase sudah diinisialisasi

        var message = new Message()
        {
            Token = deviceToken,
            Notification = new Notification()
            {
                Title = title,
                Body = body,
                ImageUrl = image
            },
            Data = new Dictionary<string, string>
            {
                { "forceOpen", "true" }, // Bisa digunakan untuk membuka aplikasi otomatis
                { "idOrder", idOrder }, // Bisa digunakan untuk membuka aplikasi otomatis
                { "chat", "true" } // Bisa digunakan untuk membuka aplikasi otomatis
            }
        };

        string response = await FirebaseMessaging.DefaultInstance.SendAsync(message);
        return response; // Response dari Firebase
    }

    private static string GetSecret(string secretName)
    {
        var client = SecretManagerServiceClient.Create();
        var secretVersionName = new SecretVersionName("trasgo", secretName, "latest");
        
        var response = client.AccessSecretVersion(secretVersionName);
        return response.Payload.Data.ToStringUtf8(); // Convert ke string JSON
    }
}
