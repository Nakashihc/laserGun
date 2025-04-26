using System.Collections;
using System.Collections.Generic;
using System.IO.Ports;
using UnityEngine;

public class Cursor : MonoBehaviour
{
    SerialPort stream = new SerialPort("COM3", 115200);
    public string strReceived;

    public string[] strData = new string[4];
    public string[] strData_received = new string[4];
    public float qw, qx, qy, qz;

    public bool resetRotation = false; // Boolean untuk reset rotasi
    private Quaternion rotationOffset = Quaternion.identity; // Koreksi offset rotasi

    public Transform Kursor;

    void Start()
    {
        stream.Open();
    }

    void Update()
    {
        strReceived = stream.ReadLine();
        strData = strReceived.Split(',');

        if (strData.Length >= 4 &&
            !string.IsNullOrEmpty(strData[0]) &&
            !string.IsNullOrEmpty(strData[1]) &&
            !string.IsNullOrEmpty(strData[2]) &&
            !string.IsNullOrEmpty(strData[3]))
        {
            qw = float.Parse(strData[0]);
            qx = float.Parse(strData[1]);
            qy = float.Parse(strData[2]);
            qz = float.Parse(strData[3]);

            // Dapatkan rotasi sensor
            Quaternion sensorRotation = new Quaternion(-qy, -qz, qx, qw);

            // Jika reset aktif, hitung offset baru
            if (resetRotation)
            {
                rotationOffset = Quaternion.Inverse(sensorRotation);
                Kursor.rotation = Quaternion.identity; // Set ke rotasi default (0,0,0)
                resetRotation = false; // Matikan reset setelah selesai
            }
            else
            {
                // Aplikasikan rotasi dengan offset koreksi
                Quaternion targetRotation = rotationOffset * sensorRotation;

                // Konversi ke Euler angles untuk menghilangkan Z
                Vector3 euler = targetRotation.eulerAngles;
                euler.z = 0; // Hilangkan rotasi Z (yaw)

                // Terapkan rotasi terbatas (hanya X dan Y)
                Kursor.rotation = Quaternion.Euler(euler);
            }
        }
    }

    // Fungsi untuk mengaktifkan reset dari UI/script lain
    public void ResetCursorRotation()
    {
        resetRotation = true;
    }
}