using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Editor模式下音频工具
/// </summary>
public static class AudioEditorUtils
{

    public static void PlayClip(AudioClip clip, int startSample, bool loop)
    {
        Assembly unityEditorAssembly = typeof(AudioImporter).Assembly;
        Type audioUtilClass = unityEditorAssembly.GetType("UnityEditor.AudioUtil");
        MethodInfo method = audioUtilClass.GetMethod(
            "PlayClip",
            BindingFlags.Static | BindingFlags.Public,
            null,
            new System.Type[] {
                typeof(AudioClip),
                typeof(Int32),
                typeof(Boolean)
        },
        null
        );
        method.Invoke(
            null,
            new object[] {
                clip,
                startSample,
                loop
        }
        );
    }

    public static void PlayClip(AudioClip clip, int startSample)
    {
        Assembly unityEditorAssembly = typeof(AudioImporter).Assembly;
        Type audioUtilClass = unityEditorAssembly.GetType("UnityEditor.AudioUtil");
        MethodInfo method = audioUtilClass.GetMethod(
            "PlayClip",
            BindingFlags.Static | BindingFlags.Public,
            null,
            new System.Type[] {
                typeof(AudioClip),
                typeof(Int32)
        },
        null
        );
        method.Invoke(
            null,
            new object[] {
                clip,
                startSample
        }
        );
    }

    public static void PlayClip(AudioClip clip)
    {
        Assembly unityEditorAssembly = typeof(AudioImporter).Assembly;
        Type audioUtilClass = unityEditorAssembly.GetType("UnityEditor.AudioUtil");
        MethodInfo method = audioUtilClass.GetMethod(
            "PlayClip",
            BindingFlags.Static | BindingFlags.Public,
            null,
            new System.Type[] {
                typeof(AudioClip)
        },
        null
        );
        method.Invoke(
            null,
            new object[] {
                clip
        }
        );
    }

    public static void StopClip(AudioClip clip)
    {
        Assembly unityEditorAssembly = typeof(AudioImporter).Assembly;
        Type audioUtilClass = unityEditorAssembly.GetType("UnityEditor.AudioUtil");
        MethodInfo method = audioUtilClass.GetMethod(
            "StopClip",
            BindingFlags.Static | BindingFlags.Public,
            null,
            new System.Type[] {
                typeof(AudioClip)
        },
        null
        );
        method.Invoke(
            null,
            new object[] {
                clip
        }
        );
    }

    public static void PauseClip(AudioClip clip)
    {
        Assembly unityEditorAssembly = typeof(AudioImporter).Assembly;
        Type audioUtilClass = unityEditorAssembly.GetType("UnityEditor.AudioUtil");
        MethodInfo method = audioUtilClass.GetMethod(
            "PauseClip",
            BindingFlags.Static | BindingFlags.Public,
            null,
            new System.Type[] {
                typeof(AudioClip)
        },
        null
        );
        method.Invoke(
            null,
            new object[] {
                clip
        }
        );
    }

    public static void ResumeClip(AudioClip clip)
    {
        Assembly unityEditorAssembly = typeof(AudioImporter).Assembly;
        Type audioUtilClass = unityEditorAssembly.GetType("UnityEditor.AudioUtil");
        MethodInfo method = audioUtilClass.GetMethod(
            "ResumeClip",
            BindingFlags.Static | BindingFlags.Public,
            null,
            new System.Type[] {
                typeof(AudioClip)
        },
        null
        );
        method.Invoke(
            null,
            new object[] {
                clip
        }
        );
    }

    public static void LoopClip(AudioClip clip, bool on)
    {
        Assembly unityEditorAssembly = typeof(AudioImporter).Assembly;
        Type audioUtilClass = unityEditorAssembly.GetType("UnityEditor.AudioUtil");
        MethodInfo method = audioUtilClass.GetMethod(
            "LoopClip",
            BindingFlags.Static | BindingFlags.Public,
            null,
            new System.Type[] {
                typeof(AudioClip),
                typeof(bool)
        },
        null
        );
        method.Invoke(
            null,
            new object[] {
                clip,
                on
        }
        );
    }

    public static bool IsClipPlaying(AudioClip clip)
    {
        Assembly unityEditorAssembly = typeof(AudioImporter).Assembly;
        Type audioUtilClass = unityEditorAssembly.GetType("UnityEditor.AudioUtil");
        MethodInfo method = audioUtilClass.GetMethod(
            "IsClipPlaying",
            BindingFlags.Static | BindingFlags.Public
            );

        bool playing = (bool)method.Invoke(
            null,
            new object[] {
                clip,
        }
        );

        return playing;
    }

    public static void StopAllClips()
    {
        Assembly unityEditorAssembly = typeof(AudioImporter).Assembly;
        Type audioUtilClass = unityEditorAssembly.GetType("UnityEditor.AudioUtil");
        MethodInfo method = audioUtilClass.GetMethod(
            "StopAllClips",
            BindingFlags.Static | BindingFlags.Public
            );

        method.Invoke(
            null,
            null
            );
    }

    public static float GetClipPosition(AudioClip clip)
    {
        Assembly unityEditorAssembly = typeof(AudioImporter).Assembly;
        Type audioUtilClass = unityEditorAssembly.GetType("UnityEditor.AudioUtil");
        MethodInfo method = audioUtilClass.GetMethod(
            "GetClipPosition",
            BindingFlags.Static | BindingFlags.Public
            );

        float position = (float)method.Invoke(
            null,
            new object[] {
                clip
        }
        );

        return position;
    }

    public static int GetClipSamplePosition(AudioClip clip)
    {
        Assembly unityEditorAssembly = typeof(AudioImporter).Assembly;
        Type audioUtilClass = unityEditorAssembly.GetType("UnityEditor.AudioUtil");
        MethodInfo method = audioUtilClass.GetMethod(
            "GetClipSamplePosition",
            BindingFlags.Static | BindingFlags.Public
            );

        int position = (int)method.Invoke(
            null,
            new object[] {
                clip
        }
        );

        return position;
    }

    public static void SetClipSamplePosition(AudioClip clip, int iSamplePosition)
    {
        Assembly unityEditorAssembly = typeof(AudioImporter).Assembly;
        Type audioUtilClass = unityEditorAssembly.GetType("UnityEditor.AudioUtil");
        MethodInfo method = audioUtilClass.GetMethod(
            "SetClipSamplePosition",
            BindingFlags.Static | BindingFlags.Public
            );

        method.Invoke(
            null,
            new object[] {
                clip,
                iSamplePosition
        }
        );
    }

    public static int GetSampleCount(AudioClip clip)
    {
        Assembly unityEditorAssembly = typeof(AudioImporter).Assembly;
        Type audioUtilClass = unityEditorAssembly.GetType("UnityEditor.AudioUtil");
        MethodInfo method = audioUtilClass.GetMethod(
            "GetSampleCount",
            BindingFlags.Static | BindingFlags.Public
            );

        int samples = (int)method.Invoke(
            null,
            new object[] {
                clip
        }
        );

        return samples;
    }

    public static int GetChannelCount(AudioClip clip)
    {
        Assembly unityEditorAssembly = typeof(AudioImporter).Assembly;
        Type audioUtilClass = unityEditorAssembly.GetType("UnityEditor.AudioUtil");
        MethodInfo method = audioUtilClass.GetMethod(
            "GetChannelCount",
            BindingFlags.Static | BindingFlags.Public
            );

        int channels = (int)method.Invoke(
            null,
            new object[] {
                clip
        }
        );

        return channels;
    }

    public static int GetBitRate(AudioClip clip)
    {
        Assembly unityEditorAssembly = typeof(AudioImporter).Assembly;
        Type audioUtilClass = unityEditorAssembly.GetType("UnityEditor.AudioUtil");
        MethodInfo method = audioUtilClass.GetMethod(
            "GetChannelCount",
            BindingFlags.Static | BindingFlags.Public
            );

        int bitRate = (int)method.Invoke(
            null,
            new object[] {
                clip
        }
        );

        return bitRate;
    }

    public static int GetBitsPerSample(AudioClip clip)
    {
        Assembly unityEditorAssembly = typeof(AudioImporter).Assembly;
        Type audioUtilClass = unityEditorAssembly.GetType("UnityEditor.AudioUtil");
        MethodInfo method = audioUtilClass.GetMethod(
            "GetBitsPerSample",
            BindingFlags.Static | BindingFlags.Public
            );

        int bits = (int)method.Invoke(
            null,
            new object[] {
                clip
        }
        );

        return bits;
    }

    public static int GetFrequency(AudioClip clip)
    {
        Assembly unityEditorAssembly = typeof(AudioImporter).Assembly;
        Type audioUtilClass = unityEditorAssembly.GetType("UnityEditor.AudioUtil");
        MethodInfo method = audioUtilClass.GetMethod(
            "GetFrequency",
            BindingFlags.Static | BindingFlags.Public
            );

        int frequency = (int)method.Invoke(
            null,
            new object[] {
                clip
        }
        );

        return frequency;
    }

    public static int GetSoundSize(AudioClip clip)
    {
        Assembly unityEditorAssembly = typeof(AudioImporter).Assembly;
        Type audioUtilClass = unityEditorAssembly.GetType("UnityEditor.AudioUtil");
        MethodInfo method = audioUtilClass.GetMethod(
            "GetSoundSize",
            BindingFlags.Static | BindingFlags.Public
            );

        int size = (int)method.Invoke(
            null,
            new object[] {
                clip
        }
        );

        return size;
    }

    public static Texture2D GetWaveForm(AudioClip clip, int channel, float width, float height)
    {
        Assembly unityEditorAssembly = typeof(AudioImporter).Assembly;
        Type audioUtilClass = unityEditorAssembly.GetType("UnityEditor.AudioUtil");
        MethodInfo method = audioUtilClass.GetMethod(
            "GetWaveForm",
            BindingFlags.Static | BindingFlags.Public
            );

        string path = AssetDatabase.GetAssetPath(clip);
        AudioImporter importer = (AudioImporter)AssetImporter.GetAtPath(path);

        Texture2D texture = (Texture2D)method.Invoke(
            null,
            new object[] {
                clip,
                importer,
                channel,
                width,
                height
        }
        );

        return texture;
    }

    public static Texture2D GetWaveFormFast(AudioClip clip, int channel, int fromSample, int toSample, float width, float height)
    {
        Assembly unityEditorAssembly = typeof(AudioImporter).Assembly;
        Type audioUtilClass = unityEditorAssembly.GetType("UnityEditor.AudioUtil");
        MethodInfo method = audioUtilClass.GetMethod(
            "GetWaveFormFast",
            BindingFlags.Static | BindingFlags.Public
            );

        Texture2D texture = (Texture2D)method.Invoke(
            null,
            new object[] {
                clip,
                channel,
                fromSample,
                toSample,
                width,
                height
        }
        );

        return texture;
    }

    public static void ClearWaveForm(AudioClip clip)
    {
        Assembly unityEditorAssembly = typeof(AudioImporter).Assembly;
        Type audioUtilClass = unityEditorAssembly.GetType("UnityEditor.AudioUtil");
        MethodInfo method = audioUtilClass.GetMethod(
            "ClearWaveForm",
            BindingFlags.Static | BindingFlags.Public
            );

        method.Invoke(
            null,
            new object[] {
                clip
        }
        );
    }

    public static bool HasPreview(AudioClip clip)
    {
        Assembly unityEditorAssembly = typeof(AudioImporter).Assembly;
        Type audioUtilClass = unityEditorAssembly.GetType("UnityEditor.AudioUtil");
        MethodInfo method = audioUtilClass.GetMethod(
            "GetSoundSize",
            BindingFlags.Static | BindingFlags.Public
            );

        bool hasPreview = (bool)method.Invoke(
            null,
            new object[] {
                clip
        }
        );

        return hasPreview;
    }

    public static bool IsCompressed(AudioClip clip)
    {
        Assembly unityEditorAssembly = typeof(AudioImporter).Assembly;
        Type audioUtilClass = unityEditorAssembly.GetType("UnityEditor.AudioUtil");
        MethodInfo method = audioUtilClass.GetMethod(
            "IsCompressed",
            BindingFlags.Static | BindingFlags.Public
            );

        bool isCompressed = (bool)method.Invoke(
            null,
            new object[] {
                clip
        }
        );

        return isCompressed;
    }

    public static bool IsStramed(AudioClip clip)
    {
        Assembly unityEditorAssembly = typeof(AudioImporter).Assembly;
        Type audioUtilClass = unityEditorAssembly.GetType("UnityEditor.AudioUtil");
        MethodInfo method = audioUtilClass.GetMethod(
            "IsStramed",
            BindingFlags.Static | BindingFlags.Public
            );

        bool isStreamed = (bool)method.Invoke(
            null,
            new object[] {
                clip
        }
        );

        return isStreamed;
    }

    public static double GetDuration(AudioClip clip)
    {
        Assembly unityEditorAssembly = typeof(AudioImporter).Assembly;
        Type audioUtilClass = unityEditorAssembly.GetType("UnityEditor.AudioUtil");
        MethodInfo method = audioUtilClass.GetMethod(
            "GetDuration",
            BindingFlags.Static | BindingFlags.Public
            );

        double duration = (double)method.Invoke(
            null,
            new object[] {
                clip
        }
        );

        return duration;
    }

    public static int GetFMODMemoryAllocated()
    {
        Assembly unityEditorAssembly = typeof(AudioImporter).Assembly;
        Type audioUtilClass = unityEditorAssembly.GetType("UnityEditor.AudioUtil");
        MethodInfo method = audioUtilClass.GetMethod(
            "GetFMODMemoryAllocated",
            BindingFlags.Static | BindingFlags.Public
            );

        int memoryAllocated = (int)method.Invoke(
            null,
            null
        );

        return memoryAllocated;
    }

    public static float GetFMODCPUUsage()
    {
        Assembly unityEditorAssembly = typeof(AudioImporter).Assembly;
        Type audioUtilClass = unityEditorAssembly.GetType("UnityEditor.AudioUtil");
        MethodInfo method = audioUtilClass.GetMethod(
            "GetFMODCPUUsage",
            BindingFlags.Static | BindingFlags.Public
            );

        float cpuUsage = (float)method.Invoke(
            null,
            null
            );

        return cpuUsage;
    }

    public static bool Is3D(AudioClip clip)
    {
        Assembly unityEditorAssembly = typeof(AudioImporter).Assembly;
        Type audioUtilClass = unityEditorAssembly.GetType("UnityEditor.AudioUtil");
        MethodInfo method = audioUtilClass.GetMethod(
            "Is3D",
            BindingFlags.Static | BindingFlags.Public
            );

        bool is3D = (bool)method.Invoke(
            null,
            new object[] {
                clip
        }
        );

        return is3D;
    }

    public static bool IsMovieAudio(AudioClip clip)
    {
        Assembly unityEditorAssembly = typeof(AudioImporter).Assembly;
        Type audioUtilClass = unityEditorAssembly.GetType("UnityEditor.AudioUtil");
        MethodInfo method = audioUtilClass.GetMethod(
            "IsMovieAudio",
            BindingFlags.Static | BindingFlags.Public
            );

        bool isMovieAudio = (bool)method.Invoke(
            null,
            new object[] {
                clip
        }
        );

        return isMovieAudio;
    }

    public static bool IsMOD(AudioClip clip)
    {
        Assembly unityEditorAssembly = typeof(AudioImporter).Assembly;
        Type audioUtilClass = unityEditorAssembly.GetType("UnityEditor.AudioUtil");
        MethodInfo method = audioUtilClass.GetMethod(
            "IsMOD",
            BindingFlags.Static | BindingFlags.Public
            );

        bool isMOD = (bool)method.Invoke(
            null,
            new object[] {
                clip
        }
        );

        return isMOD;
    }

    public static int GetMODChannelCount()
    {
        Assembly unityEditorAssembly = typeof(AudioImporter).Assembly;
        Type audioUtilClass = unityEditorAssembly.GetType("UnityEditor.AudioUtil");
        MethodInfo method = audioUtilClass.GetMethod(
            "GetMODChannelCount",
            BindingFlags.Static | BindingFlags.Public
            );

        int channels = (int)method.Invoke(
            null,
            null
        );

        return channels;
    }

    public static AnimationCurve GetLowpassCurve(AudioLowPassFilter lowPassFilter)
    {
        Assembly unityEditorAssembly = typeof(AudioImporter).Assembly;
        Type audioUtilClass = unityEditorAssembly.GetType("UnityEditor.AudioUtil");
        MethodInfo method = audioUtilClass.GetMethod(
            "GetLowpassCurve",
            BindingFlags.Static | BindingFlags.Public
            );

        AnimationCurve curve = (AnimationCurve)method.Invoke(
            null,
            new object[] {
                lowPassFilter
        }
        );

        return curve;
    }

    public static Vector3 GetListenerPos()
    {
        Assembly unityEditorAssembly = typeof(AudioImporter).Assembly;
        Type audioUtilClass = unityEditorAssembly.GetType("UnityEditor.AudioUtil");
        MethodInfo method = audioUtilClass.GetMethod(
            "GetListenerPos",
            BindingFlags.Static | BindingFlags.Public
            );

        Vector3 position = (Vector3)method.Invoke(
            null,
            null
        );

        return position;
    }

    public static void UpdateAudio()
    {
        Assembly unityEditorAssembly = typeof(AudioImporter).Assembly;
        Type audioUtilClass = unityEditorAssembly.GetType("UnityEditor.AudioUtil");
        MethodInfo method = audioUtilClass.GetMethod(
            "UpdateAudio",
            BindingFlags.Static | BindingFlags.Public
            );

        method.Invoke(
            null,
            null
            );
    }

    public static void SetListenerTransform(Transform t)
    {
        Assembly unityEditorAssembly = typeof(AudioImporter).Assembly;
        Type audioUtilClass = unityEditorAssembly.GetType("UnityEditor.AudioUtil");
        MethodInfo method = audioUtilClass.GetMethod(
            "SetListenerTransform",
            BindingFlags.Static | BindingFlags.Public
            );

        method.Invoke(
            null,
            new object[] {
                t
        }
        );
    }

    public static AudioType GetClipType(AudioClip clip)
    {
        Assembly unityEditorAssembly = typeof(AudioImporter).Assembly;
        Type audioUtilClass = unityEditorAssembly.GetType("UnityEditor.AudioUtil");
        MethodInfo method = audioUtilClass.GetMethod(
            "GetClipType",
            BindingFlags.Static | BindingFlags.Public
            );

        AudioType type = (AudioType)method.Invoke(
            null,
            new object[] {
                clip
        }
        );

        return type;
    }

    public static AudioType GetPlatformConversionType(AudioType inType, BuildTargetGroup targetGroup, AudioCompressionFormat format)
    {
        Assembly unityEditorAssembly = typeof(AudioImporter).Assembly;
        Type audioUtilClass = unityEditorAssembly.GetType("UnityEditor.AudioUtil");
        MethodInfo method = audioUtilClass.GetMethod(
            "GetPlatformConversionType",
            BindingFlags.Static | BindingFlags.Public
            );

        AudioType type = (AudioType)method.Invoke(
            null,
            new object[] {
                inType,
                targetGroup,
                format
        }
        );

        return type;
    }

    public static bool HaveAudioCallback(MonoBehaviour behaviour)
    {
        Assembly unityEditorAssembly = typeof(AudioImporter).Assembly;
        Type audioUtilClass = unityEditorAssembly.GetType("UnityEditor.AudioUtil");
        MethodInfo method = audioUtilClass.GetMethod(
            "HaveAudioCallback",
            BindingFlags.Static | BindingFlags.Public
            );

        bool hasCallback = (bool)method.Invoke(
            null,
            new object[] {
                behaviour
        }
        );

        return hasCallback;
    }

    public static int GetCustomFilterChannelCount(MonoBehaviour behaviour)
    {
        Assembly unityEditorAssembly = typeof(AudioImporter).Assembly;
        Type audioUtilClass = unityEditorAssembly.GetType("UnityEditor.AudioUtil");
        MethodInfo method = audioUtilClass.GetMethod(
            "GetCustomFilterChannelCount",
            BindingFlags.Static | BindingFlags.Public
            );

        int channels = (int)method.Invoke(
            null,
            new object[] {
                behaviour
        }
        );

        return channels;
    }

    public static int GetCustomFilterProcessTime(MonoBehaviour behaviour)
    {
        Assembly unityEditorAssembly = typeof(AudioImporter).Assembly;
        Type audioUtilClass = unityEditorAssembly.GetType("UnityEditor.AudioUtil");
        MethodInfo method = audioUtilClass.GetMethod(
            "GetCustomFilterProcessTime",
            BindingFlags.Static | BindingFlags.Public
            );

        int processTime = (int)method.Invoke(
            null,
            new object[] {
                behaviour
        }
        );

        return processTime;
    }

    public static float GetCustomFilterMaxIn(MonoBehaviour behaviour, int channel)
    {
        Assembly unityEditorAssembly = typeof(AudioImporter).Assembly;
        Type audioUtilClass = unityEditorAssembly.GetType("UnityEditor.AudioUtil");
        MethodInfo method = audioUtilClass.GetMethod(
            "GetCustomFilterMaxIn",
            BindingFlags.Static | BindingFlags.Public
            );

        float maxIn = (float)method.Invoke(
            null,
            new object[] {
                behaviour,
                channel
        }
        );

        return maxIn;
    }

    public static float GetCustomFilterMaxOut(MonoBehaviour behaviour, int channel)
    {
        Assembly unityEditorAssembly = typeof(AudioImporter).Assembly;
        Type audioUtilClass = unityEditorAssembly.GetType("UnityEditor.AudioUtil");
        MethodInfo method = audioUtilClass.GetMethod(
            "GetCustomFilterMaxOut",
            BindingFlags.Static | BindingFlags.Public
            );

        float maxOut = (float)method.Invoke(
            null,
            new object[] {
                behaviour,
                channel
        }
        );

        return maxOut;
    }
}

