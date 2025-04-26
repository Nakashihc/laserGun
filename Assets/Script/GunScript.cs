using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;
using System.Collections;
using System;
using Random = UnityEngine.Random;

public class PistolIMU : MonoBehaviour
{
    public Transform cursor;
    public float sensitivity = 50f; // Biar lebih responsif
    public bool simulator;

    [Header("Gun Settings")]
    public ParticleSystem ShootingSystem;
    public TrailRenderer BulletTrail;
    public ParticleSystem ImpactParticleSystem;
    public LayerMask Mask;
    public float BulletSpeed = 100f;

    [Header("Audio Settings")]
    public AudioClip GunShotSound;
    public AudioSource audioSource;

    [Header("Eject Bullet")]
    public GameObject BulletCasingPrefab;
    public float EjectForce = 1.5f;

    public void OnMessageArrived(string message)
    {
        string[] parts = message.Split(';');
        if (parts.Length < 2) return;

        string[] cursorData = parts[0].Replace("CURSOR:", "").Split(',');
        if (cursorData.Length < 2) return;

        if (float.TryParse(cursorData[0], out float x) && float.TryParse(cursorData[1], out float y))
        {
            float yaw = x * sensitivity;
            float pitch = y * sensitivity;

            Vector3 newCursorPos = new Vector3(Screen.width / 2 + yaw, Screen.height / 2 + pitch, 10);
            cursor.position = Camera.main.ScreenToWorldPoint(newCursorPos);
        }

        if (parts[1].StartsWith("FIRE:") && int.TryParse(parts[1].Replace("FIRE:", ""), out int fireFlag))
        {
            if (fireFlag == 1)
            {
                Fire();
            }
        }
    }

    private void Fire()
    {
        Vector3 spawnPosition = cursor.position;

        if (ShootingSystem != null)
        {
            Instantiate(ShootingSystem, spawnPosition, Quaternion.identity).Play();
        }

        if (GunShotSound != null && audioSource != null)
            audioSource.PlayOneShot(GunShotSound);

        Vector3 direction = Camera.main.transform.forward;

        if (Physics.Raycast(spawnPosition, direction, out RaycastHit hit, Mathf.Infinity, Mask))
        {
            TrailRenderer trail = Instantiate(BulletTrail, spawnPosition, Quaternion.identity);
            StartCoroutine(SpawnTrail(trail, hit.point, hit.normal, true));
        }
        else
        {
            TrailRenderer trail = Instantiate(BulletTrail, spawnPosition, Quaternion.identity);
            StartCoroutine(SpawnTrail(trail, spawnPosition + direction * 100, Vector3.zero, false));
        }

        EjectBullet(spawnPosition);
    }

    private IEnumerator SpawnTrail(TrailRenderer Trail, Vector3 HitPoint, Vector3 HitNormal, bool MadeImpact)
    {
        Vector3 startPosition = Trail.transform.position;
        float distance = Vector3.Distance(startPosition, HitPoint);
        float remainingDistance = distance;

        while (remainingDistance > 0)
        {
            Trail.transform.position = Vector3.Lerp(startPosition, HitPoint, 1 - (remainingDistance / distance));
            remainingDistance -= BulletSpeed * Time.deltaTime;
            yield return null;
        }

        Trail.transform.position = HitPoint;

        if (MadeImpact && ImpactParticleSystem != null)
            Instantiate(ImpactParticleSystem, HitPoint, Quaternion.LookRotation(HitNormal));

        Destroy(Trail.gameObject, Trail.time);
    }

    private void EjectBullet(Vector3 spawnPosition)
    {
        if (BulletCasingPrefab != null)
        {
            GameObject casing = Instantiate(BulletCasingPrefab, spawnPosition, Quaternion.identity);
            Rigidbody casingRb = casing.GetComponent<Rigidbody>();
            if (casingRb != null)
            {
                Vector3 ejectDirection = new Vector3(Random.Range(-1f, 1f), 1f, Random.Range(-1f, 1f)).normalized;
                casingRb.AddForce(ejectDirection * EjectForce, ForceMode.Impulse);
                casingRb.AddTorque(Random.insideUnitSphere * 2f, ForceMode.Impulse);
            }
            Destroy(casing, 2f);
        }
    }
}
