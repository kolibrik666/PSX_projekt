using UnityEngine;
using System.IO;
using System;
using System.Security.Cryptography;
using Zenject;

public class Serializer : IInitializable
{
    [Inject] GameStartData _gameStartData;
    
    private const string _fileSaveName = "save";
    private const string keyString = "FJ6clSRP9pQuDC1/fxBXfniNr+NYyyazeDmt3VjniQI=";
    public string FileSaveName => _fileSaveName;
    byte[] key = Convert.FromBase64String(keyString);

    public void Initialize()
    {
    }

    public void SaveData<T>(T data, string fileName)
    {
        string json = JsonUtility.ToJson(data);
       
        byte[] encryptedBytes = EncryptStringToBytes_Aes(json, key);

        string filePath = Path.Combine(Application.persistentDataPath, fileName + ".data");
        File.WriteAllBytes(filePath, encryptedBytes);
        Debug.Log("Data saved to: " + filePath);
    }

    public T LoadData<T>(string fileName)
    {
        string filePath = Path.Combine(Application.persistentDataPath, fileName + ".data");

        if (!File.Exists(filePath))
        {
            Debug.LogWarning("Save file not found: " + filePath);
            Debug.LogWarning("Creating new save file with default values.");

            GameSetupData initialData = CreateInitialGameData<GameSetupData>();
            SaveData(initialData, fileName);
        }

        byte[] encryptedBytes = File.ReadAllBytes(filePath);
        string decryptedJson = DecryptBytesToString_Aes(encryptedBytes, key);

        return JsonUtility.FromJson<T>(decryptedJson);
    }
    public GameSetupData CreateInitialGameData<T>() where T : new()
    {
        GameSetupData initialSetupData = new GameSetupData
        {
            Saturation = _gameStartData.Saturation,
            Sanity = _gameStartData.Sanity,
            SurvivedDays = _gameStartData.SurvivedDays,
            Difficulty = _gameStartData.Difficulty
        };

        return initialSetupData;
    }
    private byte[] EncryptStringToBytes_Aes(string plainText, byte[] key)
    {
        byte[] encrypted;
        using (Aes aesAlg = Aes.Create())
        {
            aesAlg.Key = key;
            aesAlg.Mode = CipherMode.CBC;
            aesAlg.Padding = PaddingMode.PKCS7;

            aesAlg.GenerateIV(); 

            ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);
            using (MemoryStream msEncrypt = new MemoryStream())
            {
                msEncrypt.Write(aesAlg.IV, 0, aesAlg.IV.Length); 
                using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                {
                    swEncrypt.Write(plainText);
                }
                encrypted = msEncrypt.ToArray();
            }
        }
        return encrypted;
    }
    private string DecryptBytesToString_Aes(byte[] cipherText, byte[] key)
    {
        string plaintext = null;

        using (Aes aesAlg = Aes.Create())
        {
            aesAlg.Key = key;
            aesAlg.Mode = CipherMode.CBC;
            aesAlg.Padding = PaddingMode.PKCS7;

            byte[] iv = new byte[aesAlg.BlockSize / 8];
            Array.Copy(cipherText, iv, iv.Length);

            aesAlg.IV = iv;

            ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

            using (MemoryStream msDecrypt = new MemoryStream(cipherText, iv.Length, cipherText.Length - iv.Length))
            {
                using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                {
                    plaintext = srDecrypt.ReadToEnd();
                }
            }
        }

        return plaintext;
    }

    public static byte[] GenerateRandomKey(int keySize)
    {
        using (var rng = new RNGCryptoServiceProvider())
        {
            byte[] key = new byte[keySize / 8];
            rng.GetBytes(key);
            string keyString = Convert.ToBase64String(key);
            Debug.Log("Generated Key: " + keyString);

            return key;
        }

    }
}