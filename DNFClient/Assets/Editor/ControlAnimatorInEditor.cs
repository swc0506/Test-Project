using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Animator))]
public class ControlAnimatorInEditor : Editor
{
    private Animator animator;
    private bool isPlaying = false;
    private float animationTime = 0.0f;
    private float playbackSpeed = 1.0f;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (animator == null)
        {
            animator = (Animator)target;
        }

        // 显示一个按钮来开始或停止动画
        if (GUILayout.Button(isPlaying ? "Stop Animation" : "Play Animation"))
        {
            if (isPlaying)
            {
                StopAnimation();
            }
            else
            {
                PlayAnimation();
            }
        }

        // 控制动画进度
        EditorGUILayout.LabelField("Animation Time");
        animationTime = EditorGUILayout.Slider(animationTime, 0.0f, animator.GetCurrentAnimatorClipInfo(0)[0].clip.length);

        // 设置动画时间
        if (GUILayout.Button("Set Animation Time"))
        {
            SetAnimationTime(animationTime);
        }

        // 控制播放速度
        EditorGUILayout.LabelField("Playback Speed");
        playbackSpeed = EditorGUILayout.FloatField(playbackSpeed, GUILayout.Width(50));

        if (GUILayout.Button("Apply Playback Speed"))
        {
            ApplyPlaybackSpeed(playbackSpeed);
        }
    }

    private void PlayAnimation()
    {
        if (animator != null)
        {
            // 设置你想要播放的动画状态
            string stateName = "YourAnimationStateName"; // 替换为你实际的状态名称
            animator.Play(stateName, 0, animationTime / animator.GetCurrentAnimatorClipInfo(0)[0].clip.length);

            isPlaying = true;
        }
    }

    private void StopAnimation()
    {
        if (animator != null)
        {
            // 停止当前动画
            animator.StopPlayback();
            isPlaying = false;
        }
    }

    private void SetAnimationTime(float time)
    {
        if (animator != null)
        {
            // 获取当前动画剪辑
            AnimatorClipInfo[] clipInfo = animator.GetCurrentAnimatorClipInfo(0);
            if (clipInfo.Length > 0)
            {
                // 设置动画时间
                animator.Play(clipInfo[0].clip.name, 0, time / clipInfo[0].clip.length);
            }
        }
    }

    private void ApplyPlaybackSpeed(float speed)
    {
        if (animator != null)
        {
            // 应用播放速度
            animator.speed = speed;
        }
    }

    private void OnDisable()
    {
        // 当 Inspector 关闭时停止动画
        StopAnimation();
    }
}
