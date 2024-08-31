using UnityEngine;

public class DistanceBasedAudio : MonoBehaviour
{
    public Transform targetObject; // 기준이 되는 오브젝트
    public AudioSource audioSource; // 소리를 재생할 오디오 소스
    public AudioClip[] audioClips; // 거리에 따라 변경될 오디오 클립 배열
    public float[] distances; // 각 오디오 클립이 변경될 거리 범위

    private int currentClipIndex = -1;

    void Update()
    {
        if (targetObject != null && audioSource != null && audioClips.Length == distances.Length)
        {
            // 오브젝트 간의 거리 계산
            float distance = Vector3.Distance(transform.position, targetObject.position);

            // 적절한 오디오 클립 선택
            int clipIndex = -1;
            for (int i = 0; i < distances.Length; i++)
            {
                if (distance <= distances[i])
                {
                    clipIndex = i;
                    break;
                }
            }

            // 오디오 클립이 변경될 때만 재생
            if (clipIndex != -1 && clipIndex != currentClipIndex)
            {
                currentClipIndex = clipIndex;
                audioSource.clip = audioClips[currentClipIndex];
                audioSource.Play();
            }
        }
    }
}