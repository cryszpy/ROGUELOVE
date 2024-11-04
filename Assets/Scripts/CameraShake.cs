using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Unity.Mathematics;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    [SerializeField]
    private CinemachineVirtualCamera cam;

    public IEnumerator Shake(float duration, float amplitude, float frequency) {
        CinemachineBasicMultiChannelPerlin noise = cam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        noise.m_AmplitudeGain = amplitude;
        noise.m_FrequencyGain = frequency;

        yield return new WaitForSeconds(duration);

        noise.m_AmplitudeGain = 0;
        noise.m_FrequencyGain = 0;
    }
}
