using UnityEngine;

public class RotasiToPosition : MonoBehaviour
{
    [Header("Referensi Objek")]
    public Transform Gunn;  // Sumber rotasi
    public Transform Skop;  // Objek yang bergerak

    [Header("Skala Pergerakan")]
    public float sensitivityX = 0.01f; // Sensitivitas gerak X
    public float sensitivityY = 0.01f; // Sensitivitas gerak Y

    [Header("Batas Posisi")]
    public float minX = -5f;
    public float maxX = 5f;
    public float minY = -3f;
    public float maxY = 3f;

    private Vector3 _initialPosition;
    private Quaternion _initialRotation;

    void Start()
    {
        if (Gunn == null || Skop == null)
        {
            Debug.LogError("Gunn atau Skop belum di-set!");
            return;
        }

        _initialPosition = Skop.position;
        _initialRotation = Gunn.rotation;
    }

    void Update()
    {
        // Hitung perbedaan rotasi dari awal
        Quaternion deltaRotation = Gunn.rotation * Quaternion.Inverse(_initialRotation);
        
        // Konversi ke Euler (pastikan dalam range -180° sampai 180°)
        Vector3 deltaEuler = deltaRotation.eulerAngles;
        deltaEuler.x = NormalizeAngle(deltaEuler.x);
        deltaEuler.y = NormalizeAngle(deltaEuler.y);
        deltaEuler.z = 0; // Abaikan rotasi Z

        // Hitung posisi baru
        float newX = _initialPosition.x + (deltaEuler.y * sensitivityX); // Yaw (Y) ? X
        float newY = _initialPosition.y + (deltaEuler.x * sensitivityY); // Pitch (X) ? Y

        // Clamping
        newX = Mathf.Clamp(newX, minX, maxX);
        newY = Mathf.Clamp(newY, minY, maxY);

        // Update posisi Skop
        Skop.position = new Vector3(newX, newY, _initialPosition.z);
    }

    private float NormalizeAngle(float angle)
    {
        angle = angle % 360;
        if (angle > 180) angle -= 360;
        return angle;
    }

    // Fungsi reset (bisa dipanggil dari UI)
    public void ResetPosition()
    {
        _initialRotation = Gunn.rotation;
        Skop.position = _initialPosition;
    }
}