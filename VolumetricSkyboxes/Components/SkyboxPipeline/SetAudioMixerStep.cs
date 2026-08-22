using UnityEngine;

namespace VolumetricSkyboxes.Components.SkyboxPipeline
{
    public class SetAudioMixerStep : ISkyboxPipelineStep
    {
        public void Execute(VolumetricSkyboxContainer _, GameObject skybox)
        {
            var mixerGroup = VolumetricSkyboxesBootstrap.Instance!.skyboxesMixerGroup;
            foreach (var audio in skybox.GetComponentsInChildren<AudioSource>(true))
                audio.outputAudioMixerGroup = mixerGroup;
        }
    }
}