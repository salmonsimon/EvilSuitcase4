using System;
using ZSerializer;

[Serializable, SerializeGlobalData(GlobalDataType.Globally)]
public partial class Settings
{
    public float musicVolume = .5f;
    public float SFXVolume = .5f;

    public void ResetAudioSettingsToDefault()
    {
        musicVolume = .5f;
        SFXVolume = .5f;
    }
}
